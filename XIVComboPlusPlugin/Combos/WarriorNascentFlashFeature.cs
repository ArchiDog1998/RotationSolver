using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class WarriorNascentFlashFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16464u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16464)
        {
            if (level >= 76)
            {
                return 16464u;
            }
            return 3551u;
        }
        return actionID;
    }
}
