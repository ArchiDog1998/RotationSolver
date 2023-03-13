using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Basic.Data;
using RotationSolver.Localization;
using RotationSolver.UI;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow
{
    internal partial class RotationConfigWindow
    {
        private void DrawHelpTab()
        {
            if (ImGui.Button("Github"))
            {
                Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver");
            }

            ImGui.SameLine();

            if (ImGui.Button("Discord"))
            {
                Util.OpenLink("https://discord.gg/4fECHunam9");
            }

            ImGui.SameLine();

            if (ImGui.Button("Wiki"))
            {
                Util.OpenLink("https://archidog1998.github.io/RotationSolver/");
            }

            ImGui.SameLine();

            if (ImGui.Button("Changelog"))
            {
                Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver/blob/release/CHANGELOG.md");
            }

            ImGui.SameLine();

            var support = "Support on Ko-fi";
            ImGui.SetCursorPosX(ImGui.GetWindowSize().X - ImGui.CalcTextSize(support).X - ImGui.GetStyle().ItemSpacing.X * 2);
            ImGui.PushStyleColor(ImGuiCol.Button, 0xFF5E5BFF);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD5E5BFF);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA5E5BFF);
            if (ImGui.Button(support))
            {
                Util.OpenLink("https://ko-fi.com/rotationsolver");
            }
            ImGui.PopStyleColor(3);

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_HelpItem_Description);

            if (ImGui.BeginChild("Help Infomation", new Vector2(0f, -1f), true))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                StateCommandType.Smart.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                StateCommandType.Manual.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                StateCommandType.Cancel.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                ImGui.Separator();

                SpecialCommandType.EndSpecial.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.HealArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.HealSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.DefenseArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.DefenseSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.MoveForward.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.MoveBack.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.EsunaStanceNorth.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.RaiseShirk.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.AntiKnockback.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                SpecialCommandType.Burst.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

                ImGui.PopStyleVar();
            }
        }
    }
}
