using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class DancerFlourishFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFlourishFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16013u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16013)
        {
            if (level >= 40 && HasEffect(2694))
            {
                return 15992u;
            }
            if (level >= 86 && HasEffect(2699))
            {
                return 25791u;
            }
            if (level >= 20 && HasEffect(2693))
            {
                return 15991u;
            }
            if (level >= 66 && HasEffect(1820))
            {
                return 16009u;
            }
        }
        return actionID;
    }
}
