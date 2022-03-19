using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class DancerAoeMultibutton : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerAoeMultibutton;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 15993u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 15993)
        {
            if (level >= 35 && HasEffect(2693))
            {
                return 15995u;
            }
            if (level >= 45 && HasEffect(2694))
            {
                return 15996u;
            }
            if (lastComboMove == 15993 && level >= 25)
            {
                return 15994u;
            }
        }
        return actionID;
    }
}
