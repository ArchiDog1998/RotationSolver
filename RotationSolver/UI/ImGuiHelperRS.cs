using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ECommons.LanguageHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Data;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;

namespace RotationSolver.UI;

internal static class ImGuiHelperRS
{
    internal static void SetNextWidthWithName(string name)
    {
        ImGui.SetNextItemWidth(Math.Max(80 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(name).X + 30 * ImGuiHelpers.GlobalScale));
    }

    const float INDENT_WIDTH = 180;

    internal static void DisplayCommandHelp(this Enum command, string extraCommand = "", Func<Enum, string>? getHelp = null, bool sameLine = true)
    {
        var cmdStr = command.GetCommandStr(extraCommand);

        if (ImGui.Button(cmdStr))
        {
            Svc.Commands.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImguiTooltips.ShowTooltip($"{UiString.ConfigWindow_Helper_RunCommand.Local()}: {cmdStr}\n{UiString.ConfigWindow_Helper_CopyCommand.Local()}: {cmdStr}");

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
        if (ImGui.DragInt($"{UiString.ConfigWindow_Events_MacroIndex.Local()}##MacroIndex{info.GetHashCode()}",
            ref info.MacroIndex, 1, -1, 99))
        {
            Service.Config.Save();
        }

        ImGui.SameLine();

        if (ImGui.Checkbox($"{UiString.ConfigWindow_Events_ShareMacro.Local()}##ShareMacro{info.GetHashCode()}",
            ref info.IsShared))
        {
            Service.Config.Save();
        }
    }

    public static void DisplayEvent(this ActionEventInfo info)
    {
        if (ImGui.InputText($"{UiString.ConfigWindow_Events_ActionName.Local()}##ActionName{info.GetHashCode()}",
            ref info.Name, 100))
        {
            Service.Config.Save();
        }

        info.DisplayMacro();
    }

    public static void SearchCombo<T>(string popId, string name, ref string searchTxt, T[] items, Func<T, string> getSearchName, Action<T> selectAction, string searchingHint, ImFontPtr? font = null, Vector4? color = null)
    {
        if (SelectableButton(name + "##" + popId, font, color))
        {
            if (!ImGui.IsPopupOpen(popId)) ImGui.OpenPopup(popId);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        using var popUp = ImRaii.Popup(popId);
        if (!popUp.Success) return;

        if (items == null || items.Length == 0)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "ConfigWindow_Condition_NoItemsWarning".Loc("There are no items!"));
            return;
        }

        var searchingKey = searchTxt;

        var members = items.Select(m => (m, getSearchName(m)))
            .OrderByDescending(s => Searchable.Similarity(s.Item2, searchingKey));

        ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, members.Max(i => ImGuiHelpers.GetButtonSize(i.Item2).X)));
        ImGui.InputTextWithHint("##Searching the member", searchingHint, ref searchTxt, 128);

        ImGui.Spacing();

        ImRaii.IEndObject? child = null;
        if (members.Count() >= 15)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(0, 300), new Vector2(500, 300));
            child = ImRaii.Child(popId);
            if (!child) return;
        }

        foreach (var member in members)
        {
            if (ImGui.Selectable(member.Item2))
            {
                selectAction?.Invoke(member.m);
                ImGui.CloseCurrentPopup();
            }
        }
        child?.Dispose();
    }

    public static unsafe bool SelectableCombo(string popUp, string[] items, ref int index, ImFontPtr? font = null, Vector4? color = null)
    {
        var count = items.Length;
        var originIndex = index;
        index = Math.Max(0, index) % count;
        var name = items[index] + "##" + popUp;

        var result = originIndex != index;

        if (SelectableButton(name, font, color))
        {
            if (count < 3)
            {
                index = (index + 1) % count;
                result = true;
            }
            else
            {
                if (!ImGui.IsPopupOpen(popUp)) ImGui.OpenPopup(popUp);
            }
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        ImGui.SetNextWindowSizeConstraints(Vector2.Zero, Vector2.One * 500);
        if (ImGui.BeginPopup(popUp))
        {
            for (int i = 0; i < count; i++)
            {
                if (ImGui.Selectable(items[i]))
                {
                    index = i;
                    result = true;
                }
            }
            ImGui.EndPopup();
        }

        return result;
    }

    public static unsafe bool SelectableButton(string name, ImFontPtr? font = null, Vector4? color = null)
    {
        List<IDisposable> disposables = new(2);
        if (font != null)
        {
            disposables.Add(ImRaii.PushFont(font.Value));
        }
        if (color != null)
        {
            disposables.Add(ImRaii.PushColor(ImGuiCol.Text, color.Value));
        }
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderActive)));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderHovered)));
        ImGui.PushStyleColor(ImGuiCol.Button, 0);
        var result = ImGui.Button(name);
        ImGui.PopStyleColor(3);
        foreach (var item in disposables)
        {
            item.Dispose();
        }

        return result;
    }

    public static bool IsInRect(Vector2 leftTop, Vector2 size)
    {
        var pos = ImGui.GetMousePos() - leftTop;
        if (pos.X <= 0 || pos.Y <= 0 || pos.X >= size.X || pos.Y >= size.Y) return false;
        return true;
    }

    public static void Draw(this CombatType type)
    {
        if (type.HasFlag(CombatType.PvE))
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudYellow, " PvE");
        }
        if (type.HasFlag(CombatType.PvP))
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.TankBlue, " PvP");
        }
        if (type == CombatType.None)
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudRed, " None of PvE or PvP!");
        }
    }
}