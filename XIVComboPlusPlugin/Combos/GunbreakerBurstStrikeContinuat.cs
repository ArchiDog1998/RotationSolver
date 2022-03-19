using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class GunbreakerBurstStrikeContinuation : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerBurstStrikeCont;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16162u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16162 && level >= 86 && HasEffect(2686))
        {
            return 25759u;
        }
        return actionID;
    }
}
