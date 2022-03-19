using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class NinjaTCJMeisuiFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaTCJMeisuiFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 7403u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7403 && level >= 72 && HasEffect(507))
        {
            return 16489u;
        }
        return actionID;
    }
}
