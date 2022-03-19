using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class BardBloodletterFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.Disabled;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 110u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 110)
        {
            if (level >= 60)
            {
                return CalcBestAction(actionID, 110u, 3558u, 3562u);
            }
            if (level >= 54)
            {
                return CalcBestAction(actionID, 110u, 3558u);
            }
            if (level >= 12)
            {
                return 110u;
            }
        }
        return actionID;
    }
}
