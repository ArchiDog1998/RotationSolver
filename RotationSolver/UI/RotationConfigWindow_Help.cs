using Dalamud.Utility;
using RotationSolver.Localization;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    private static void DrawHelpTab()
    {
        if(ImGuiHelper.IconButton(FontAwesomeIcon.Code, "Github"))
        {
            Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver");
        }

        ImGui.SameLine();

        if(ImGuiHelper.IconButton(FontAwesomeIcon.HandPaper, "Discord"))
        {
            Util.OpenLink("https://discord.gg/4fECHunam9");
        }

        ImGui.SameLine();

        if (ImGuiHelper.IconButton(FontAwesomeIcon.Book, "Wiki"))
        {
            Util.OpenLink("https://archidog1998.github.io/RotationSolver/");
        }

        ImGui.SameLine();

        if (ImGuiHelper.IconButton(FontAwesomeIcon.History, "ChangeLog"))
        {
            Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver/blob/release/CHANGELOG.md");
        }

        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF5E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD5E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA5E5BFF);

        if (ImGuiHelper.IconButton(FontAwesomeIcon.Coffee, "Support"))
        {
            Util.OpenLink("https://ko-fi.com/archited");
        }
        ImGui.PopStyleColor(3);

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_HelpItem_Description);

        if (ImGui.BeginChild("Help Information", new Vector2(0f, -1f), true))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

            StateCommandType.Auto.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

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

            SpecialCommandType.Speed.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

            SpecialCommandType.EsunaStanceNorth.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

            SpecialCommandType.RaiseShirk.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

            SpecialCommandType.AntiKnockback.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

            SpecialCommandType.Burst.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

            ImGui.PopStyleVar();

            ImGui.EndChild();
        }
    }
}
