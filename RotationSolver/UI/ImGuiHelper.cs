using ECommons.DalamudServices;
using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System.ComponentModel;

namespace RotationSolver.UI;

internal static class ImGuiHelper
{
    public static void HoveredString(string text, Action selected = null)
    {
        if (ImGui.IsItemHovered())
        {
            ImguiTooltips.ShowTooltip(text);

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                selected?.Invoke();
            }
        }
    }

    internal static void SetNextWidthWithName(string name)
    {
        ImGui.SetNextItemWidth(Math.Max(80 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(name).X + 30 * ImGuiHelpers.GlobalScale));
    }

    internal static void SearchCombo<T>(string popId, string name, ref string searchTxt, T[] data, Action<T> selectAction) where T : ITexture
    {
        if (ImGui.BeginCombo(popId, name, ImGuiComboFlags.HeightLargest))
        {
            SearchItems(ref searchTxt, data, selectAction);
            ImGui.EndCombo();
        }
    }

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> data, Action<T> selectAction) where T : ITexture
    {
        SearchItems(ref searchTxt, data, i => i.Name, selectAction, i =>
        {
            if(i.GetTexture(out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, new Vector2(24, 24));
            }
        }, texture => texture.Description);
    }

    internal static void SearchItemsReflection<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        if (ImGui.BeginCombo(popId, name, ImGuiComboFlags.HeightLargest))
        {
            SearchItems(ref searchTxt, actions, i => i.GetMemberName(), selectAction);

            ImGui.EndCombo();
        }
    }

    public static string GetMemberName(this MemberInfo info)
    {
        if (LocalizationManager.RightLang.MemberInfoName.TryGetValue(info.Name, out var memberName)) return memberName;

        return info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
    }

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> actions, Func<T, string> getName, Action<T> selectAction, Action<T> extraDraw = null, Func<T, string> getDesc = null)
    {
        ImGui.Text(LocalizationManager.RightLang.ActionSequencer_SearchBar + ": ");
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("##SearchBar", ref searchTxt, 16);

        if (!string.IsNullOrWhiteSpace(searchTxt))
        {
            var src = searchTxt;
            var l = new Levenshtein();
            actions = actions.OrderBy(a => l.Distance(getName(a), src)).ToArray();
        }

        if (ImGui.BeginChild($"##ActionsCandidateList", new Vector2(200, 400), true))
        {
            foreach (var item in actions)
            {
                if (extraDraw != null)
                {
                    extraDraw(item);
                    ImGui.SameLine();
                }

                if (ImGui.Selectable(getName(item)))
                {
                    selectAction?.Invoke(item);

                    ImGui.CloseCurrentPopup();
                }

                if (getDesc != null && ImGui.IsItemHovered())
                {
                    var desc = getDesc(item);
                    ImguiTooltips.ShowTooltip(desc);
                }
            }
            ImGui.EndChild();
        }
    }

    const float INDENT_WIDTH = 180;

    internal static void DisplayCommandHelp<T>(this T command, string extraCommand = "", Func<T, string> getHelp = null, bool sameLine = true) where T : struct, Enum
    {
        var cmdStr = command.GetCommandStr(extraCommand);

        if (ImGui.Button(cmdStr))
        {
            Svc.Commands.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImguiTooltips.ShowTooltip($"{LocalizationManager.RightLang.ConfigWindow_Helper_RunCommand}: {cmdStr}\n{LocalizationManager.RightLang.ConfigWindow_Helper_CopyCommand}: {cmdStr}");

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }

        var help = getHelp?.Invoke(command);

        if (!string.IsNullOrEmpty(help))
        {
            if (sameLine)
            {
                ImGui.SameLine();
                ImGui.Indent(INDENT_WIDTH);
            }
            ImGui.Text(" → ");
            ImGui.SameLine();
            ImGui.TextWrapped(help);
            if (sameLine)
            {
                ImGui.Unindent(INDENT_WIDTH);
            }
        }
    }

    public static void DisplayMacro(this MacroInfo info)
    {
        ImGui.SetNextItemWidth(50);
        if (ImGui.DragInt($"{LocalizationManager.RightLang.ConfigWindow_Events_MacroIndex}##MacroIndex{info.GetHashCode()}",
            ref info.MacroIndex, 1, -1, 99))
        {
            Service.Config.Save();
        }

        ImGui.SameLine();

        if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Events_ShareMacro}##ShareMacro{info.GetHashCode()}",
            ref info.IsShared))
        {
            Service.Config.Save();
        }
    }

    public static void DisplayEvent(this ActionEventInfo info)
    {
        if (ImGui.InputText($"{LocalizationManager.RightLang.ConfigWindow_Events_ActionName}##ActionName{info.GetHashCode()}",
            ref info.Name, 100))
        {
            Service.Config.Save();
        }

        info.DisplayMacro();
    }

    public unsafe static ImFontPtr GetFont(float size)
    {
        var style = new Dalamud.Interface.GameFonts.GameFontStyle(Dalamud.Interface.GameFonts.GameFontStyle.GetRecommendedFamilyAndSize(Dalamud.Interface.GameFonts.GameFontFamily.Axis, size));
        var font = Svc.PluginInterface.UiBuilder.GetGameFontHandle(style).ImFont;

        if((IntPtr)font.NativePtr == IntPtr.Zero) 
        {
            return ImGui.GetFont();
        }
        font.Scale = size / style.BaseSizePt;
        return font;
    }
}
