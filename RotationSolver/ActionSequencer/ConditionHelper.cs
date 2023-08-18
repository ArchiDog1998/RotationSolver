using ECommons.GameHelpers;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;

namespace RotationSolver.ActionSequencer;

internal class ConditionHelper
{
    public static bool CheckBaseAction(ICustomRotation rotation, ActionID id, ref BaseAction action)
    {
        if (id != ActionID.None && (action == null || (ActionID)action.ID != id))
        {
            action = rotation.AllBaseActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == id);
        }
        if (action == null || !Player.Available) return false;
        return true;
    }

    public static void CheckMemberInfo<T>(ICustomRotation rotation, string name, ref T value) where T : MemberInfo
    {
        if (!string.IsNullOrEmpty(name) && (value == null || value.Name != name))
        {
            if (typeof(T).IsAssignableFrom(typeof(PropertyInfo)))
            {
                value = (T)(MemberInfo)rotation.GetType().GetPropertyInfo(name);
            }
            else if (typeof(T).IsAssignableFrom(typeof(MethodInfo)))
            {
                value = (T)(MemberInfo)rotation.GetType().GetMethodInfo(name);
            }
        }
    }

    public static void DrawByteEnum<T>(string name, ref T value, Func<T, string> function) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var index = Array.IndexOf(values, value);
        var names = values.Select(function).ToArray();

        if(ImGuiHelper.SelectableCombo(name, names, ref index))
        {
            value = values[index];
        }
    }

    public static bool DrawDragFloat(string name, ref float value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragFloat(name, ref value);
    }

    public static bool DrawDragInt(string name, ref int value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragInt(name, ref value);
    }

    public static bool DrawCheckBox(string name, ref int value, string desc = "")
    {
        ImGui.SameLine();

        var @bool = value != 0;

        var result = false;
        if (ImGui.Checkbox(name, ref @bool))
        {
            value = @bool ? 1 : 0;
            result = true;
        }

        ImguiTooltips.HoveredTooltip(desc);

        return result;
    }

    internal static void SearchItemsReflection<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
        if (ImGuiHelper.SelectableButton(name + "##" + popId))
        {
            if (!ImGui.IsPopupOpen(popId)) ImGui.OpenPopup(popId);
        }

        if (ImGui.BeginPopup(popId))
        {
            var searchingKey = searchTxt;

            var members = actions.Select(m => (m, m.GetMemberName()))
                .OrderBy(s => s.Item2.Split(' ')
                .Min(c => RotationConfigWindow.StringComparer.Distance(c, searchingKey)));

            ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, members.Max(i => ImGuiHelpers.GetButtonSize(i.Item2).X)));
            ImGui.InputTextWithHint("##Searching the member", LocalizationManager.RightLang.ConfigWindow_Actions_MemberName, ref searchTxt, 128);

            ImGui.Spacing();

                foreach (var member in members)
                {
                    if (ImGui.Selectable(member.Item2))
                    {
                        selectAction?.Invoke(member.m);
                        ImGui.CloseCurrentPopup();
                    }
                }

            ImGui.EndPopup();
        }
    }

    public static float IconSizeRaw => ImGuiHelpers.GetButtonSize("H").Y;
    public static float IconSize => IconSizeRaw * ImGuiHelpers.GlobalScale;
    private const int count = 8;
    public static void ActionSelectorPopUp(string popUpId, CollapsingHeaderGroup group, ICustomRotation rotation, Action<IAction> action, Action others = null)
    {
        if (group != null && ImGui.BeginPopup(popUpId))
        {
            others?.Invoke();

            group.ClearCollapsingHeader();

            foreach (var pair in RotationUpdater.GroupActions(rotation.AllBaseActions))
            {
                group.AddCollapsingHeader(() => pair.Key, () =>
                {
                    var index = 0;
                    foreach (var item in pair.OrderBy(t => t.ID))
                    {
                        if (!item.GetTexture(out var icon)) continue;

                        if (index++ % count != 0)
                        {
                            ImGui.SameLine();
                        }

                        ImGui.BeginGroup();
                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * IconSize, group.GetHashCode().ToString()))
                        {
                            action?.Invoke(item);
                            ImGui.CloseCurrentPopup();
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, IconSize, 1);
                        ImGui.EndGroup();
                    }
                });
            }
            group.Draw();
            ImGui.EndPopup();
        }
    }
}
