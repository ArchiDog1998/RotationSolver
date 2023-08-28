using ECommons.GameHelpers;
using RotationSolver.Basic.Traits;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class TraitCondition : BaseCondition
{
    public uint TraitID { get; set; } = 0;
    private IBaseTrait _trait;
    public bool Condition { get; set; }

    public override bool IsTrueInside(ICustomRotation rotation)
    {
        CheckBaseTrait(rotation);
        if (_trait == null || !Player.Available) return false;

        var result = _trait.EnoughLevel;
        return Condition ? !result : result;
    }

    private const int count = 8;
    public override void DrawInside(ICustomRotation rotation)
    {
        CheckBaseTrait(rotation);

        var name = _trait?.Name ?? string.Empty;
        var popUpKey = "Trait Condition Pop Up" + GetHashCode().ToString();

        if (ImGui.BeginPopup(popUpKey))
        {
            var index = 0;
            foreach (var trait in rotation.AllTraits)
            {
                if (!trait.GetTexture(out var traitIcon)) continue;

                if (index++ % count != 0)
                {
                    ImGui.SameLine();
                }

                ImGui.BeginGroup();
                var cursor = ImGui.GetCursorPos();
                if (ImGuiHelper.NoPaddingNoColorImageButton(traitIcon.ImGuiHandle, Vector2.One * ConditionHelper.IconSize, trait.GetHashCode().ToString()))
                {
                    TraitID = trait.ID;
                    ImGui.CloseCurrentPopup();
                }
                ImGuiHelper.DrawActionOverlay(cursor, ConditionHelper.IconSize, -1);
                ImGui.EndGroup();

                var tooltip = trait.Name;
                if (!string.IsNullOrEmpty(tooltip)) ImguiTooltips.HoveredTooltip(tooltip);

            }
            ImGui.EndPopup();
        }

        if (_trait?.GetTexture(out var icon) ?? false || IconSet.GetTexture(4, out icon))
        {
            var cursor = ImGui.GetCursorPos();
            if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionHelper.IconSize, GetHashCode().ToString()))
            {
                if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
            }
            ImGuiHelper.DrawActionOverlay(cursor, ConditionHelper.IconSize, -1);
            ImguiTooltips.HoveredTooltip(name);
        }

        ImGui.SameLine();
        var i = 0;
        ImGuiHelper.SelectableCombo($"##Category{GetHashCode()}", new string[]
        {
            LocalizationManager.RightLang.ActionConditionType_EnoughLevel
        }, ref i);
        ImGui.SameLine();

        var condition = Condition ? 1 : 0;
        if (ImGuiHelper.SelectableCombo($"##Comparation{GetHashCode()}", new string[]
                {
                    LocalizationManager.RightLang.ActionSequencer_Is,
                    LocalizationManager.RightLang.ActionSequencer_Isnot,
                }, ref condition))
        {
            Condition = condition > 0;
        }
    }

    private void CheckBaseTrait(ICustomRotation rotation)
    {
        if (TraitID != 0 && (_trait == null || _trait.ID != TraitID))
        {
            _trait = rotation.AllTraits.FirstOrDefault(a => a.ID == TraitID);
        }
    }
}
