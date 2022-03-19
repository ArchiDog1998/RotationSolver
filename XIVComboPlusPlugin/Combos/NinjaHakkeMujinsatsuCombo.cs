namespace XIVComboPlus.Combos;

internal class NinjaHakkeMujinsatsuCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHakkeMujinsatsuCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16488u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16488)
        {
            if (comboTime > 0f && lastComboMove == 2254 && level >= 52)
            {
                return 16488u;
            }
            return 2254u;
        }
        return actionID;
    }
}
