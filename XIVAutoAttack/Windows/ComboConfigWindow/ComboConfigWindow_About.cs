using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System.Numerics;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawAbout()
    {
        if(ImGui.BeginChild("About", new Vector2(0, 0), true))
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "此插件开源免费，请勿从任何渠道付费购买此插件。\n如果已经从付费渠道获得此插件，请立即发起退款、提供差评并举报卖家");
            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudRed, "包括但不限于以下闲鱼小店（排名不分先后）:");
            ImGui.TextColored(ImGuiColors.DalamudYellow, "五颜六色的猪  小玉超可爱  纷乱雪月花  EmetSelch  麦麦麦麦  叶怀雨雨");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped("插件作者：ArchiDog1998（秋水）保留最终解释权");
            ImGui.TextWrapped("联合开发者：汐ベMoon, gamous, 逆光, sciuridae564, 玖祁, 牙刷play");
            ImGui.TextColored(ImGuiColors.ParsedGreen, "本插件版本更新发布于Github");
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            if (ImGui.Button("点击加入Discord进行讨论"))
            {
                Util.OpenLink("https://discord.gg/wJHTXxrEv7");
            }
            if (ImGui.Button("点击查看Wiki"))
            {
                Util.OpenLink("https://archidog1998.github.io/XIVAutoAttack/");
            }
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }
}
