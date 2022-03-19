namespace XIVComboPlus.Combos;

internal class SamuraiYukikazeCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiYukikazeCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 7480u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7480)
        {
            if (level >= 50 && HasEffect(1233))
            {
                return 7480u;
            }
            if (comboTime > 0f && lastComboMove == 7477 && level >= 50)
            {
                return 7480u;
            }
            return 7477u;
        }
        return actionID;
    }
}
