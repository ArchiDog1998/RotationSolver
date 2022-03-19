using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class DancerFanDanceCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerFanDanceCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 16007u, 16008u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16007 || actionID == 16008)
        {
            if (level >= 86 && HasEffect(2699))
            {
                return 25791u;
            }
            if (level >= 66 && HasEffect(1820))
            {
                return 16009u;
            }
        }
        return actionID;
    }
}
