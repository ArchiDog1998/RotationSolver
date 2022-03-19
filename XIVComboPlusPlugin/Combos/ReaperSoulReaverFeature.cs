using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class ReaperSoulReaverFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSoulReaverGallowsFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 24378u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 24378 && level >= 70 && (HasEffect(2587) || HasEffect(2593)))
        {
            if (IsEnabled(CustomComboPreset.ReaperSoulReaverGallowsOption))
            {
                return OriginalHook(24382u);
            }
            return OriginalHook(24383u);
        }
        return actionID;
    }
}
