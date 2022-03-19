using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class PaladinGoringBladeCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinGoringBladeCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 3538u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 3538)
        {
            if (comboTime > 0f)
            {
                if (lastComboMove == 15 && level >= 54)
                {
                    return 3538u;
                }
                if (lastComboMove == 9 && level >= 4)
                {
                    return 15u;
                }
            }
            return 9u;
        }
        return actionID;
    }
}
