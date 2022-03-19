namespace XIVComboPlus.Combos;

internal class DragoonFullThrustCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonFullThrustCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 84u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 84)
        {
            if (level >= 58 && HasEffect(803))
            {
                return 3556u;
            }
            if (level >= 56 && HasEffect(802))
            {
                return 3554u;
            }
            if (comboTime > 0f)
            {
                if (lastComboMove == 78 && level >= 26)
                {
                    return OriginalHook(84u);
                }
                if ((lastComboMove == 75 || lastComboMove == 16479) && level >= 4)
                {
                    return 78u;
                }
            }
            return OriginalHook(75u);
        }
        return actionID;
    }
}
