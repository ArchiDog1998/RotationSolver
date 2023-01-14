using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System.Numerics;
using XIVAutoAction.Localization;

namespace XIVAutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawAbout()
    {
        if (ImGui.BeginChild("About", new Vector2(0, 0), true))
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.Configwindow_About_Declaration);
            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.Configwindow_About_XianYu);
            ImGui.TextColored(ImGuiColors.DalamudYellow, "五颜六色的猪  小玉超可爱  纷乱雪月花  叶怀雨雨\nff14最后一个机工士  用户_163750520");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_About_Owner);
            ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_About_Collaborators);
            ImGui.TextColored(ImGuiColors.ParsedGreen, LocalizationManager.RightLang.Configwindow_About_Github);
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_About_Discord))
            {
                Util.OpenLink("https://discord.gg/wJHTXxrEv7");
            }
            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_About_Wiki))
            {
                Util.OpenLink("https://archidog1998.github.io/XIVAutoAttack/");
            }
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }
}
