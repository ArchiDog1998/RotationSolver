using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow
{
    internal partial class RotationConfigWindow
    {
        private void DrawHelpTab()
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_HelpItem_Description);

            ImGui.SameLine();

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

            if (ImGui.BeginChild("Help Infomation", new Vector2(0f, -1f), true))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                StateCommandType.Smart.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                StateCommandType.Manual.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                StateCommandType.Cancel.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.EndSpecial.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.HealArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.HealSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.DefenseArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.DefenseSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.MoveForward.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.MoveBack.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.EsunaStanceNorth.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.RaiseShirk.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.AntiKnockback.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();

                SpecialCommandType.Burst.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);
                ImGui.Separator();


                ImGui.PopStyleVar();
            }
        }
    }
}
