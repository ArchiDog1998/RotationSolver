using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System.Numerics;
using AutoAction.Localization;

namespace AutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawAbout()
    {
        if (ImGui.BeginChild("About", new Vector2(0, 0), true))
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.Configwindow_About_Declaration);
            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudRed, LocalizationManager.RightLang.Configwindow_About_XianYu);
            ImGui.TextColored(ImGuiColors.DalamudYellow, "五颜六色的猪  小玉超可爱  纷乱雪月花  叶怀雨雨  麦麦麦麦\nff14最后一个机工士  用户_163750520(QQ 3338951094)  爱吃零食的小玲\n雪猫猫喜欢吃  做个好人  海洋饼干  梦想汗水坚持  人在现在能拍有货留Q");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_About_Owner);
            ImGui.TextColored(ImGuiColors.ParsedGreen, LocalizationManager.RightLang.Configwindow_About_Github);
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            if (ImGui.Button("Github"))
            {
                Util.OpenLink("https://github.com/moewcorp/AutoAction");
            }
            //if (ImGui.Button(LocalizationManager.RightLang.Configwindow_About_Discord))
            //{
            //    Util.OpenLink("https://discord.gg/wJHTXxrEv7");
            //}
            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_About_Wiki))
            {
                Util.OpenLink("https://moewcorp.github.io/AutoAction/"+WikiLang(Service.Interface.UiLanguage));
            }
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }

    private string WikiLang(string lang){
        return lang switch
        {
            "zh" => "#/zh-cn/",
            _ => ""
        };
    }
}
