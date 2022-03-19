using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class DancerSingleTargetMultibutton : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerSingleTargetMultibutton;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 15989u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 15989)
        {
            if (level >= 20 && HasEffect(2693))
            {
                return 15991u;
            }
            if (level >= 40 && HasEffect(2694))
            {
                return 15992u;
            }
            if (lastComboMove == 15989 && level >= 2)
            {
                return 15990u;
            }
        }
        return actionID;
    }
}
