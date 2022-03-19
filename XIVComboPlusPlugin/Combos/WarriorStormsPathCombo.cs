using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class WarriorStormsPathCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 42u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 42)
        {
            if (comboTime > 0f)
            {
                if (lastComboMove == 37 && level >= 26)
                {
                    return 42u;
                }
                if (lastComboMove == 31 && level >= 4)
                {
                    return 37u;
                }
            }
            return 31u;
        }
        return actionID;
    }
}
