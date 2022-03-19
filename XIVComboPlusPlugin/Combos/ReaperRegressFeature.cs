using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class ReaperRegressFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperRegressFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 24401u, 24402u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == 24402 || actionID == 24401) && level >= 74 && HasEffect(2595))
        {
            return 24403u;
        }
        return actionID;
    }
}
