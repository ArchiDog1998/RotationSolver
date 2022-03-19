namespace XIVComboPlus.Combos;

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkAoECombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 70u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 70)
        {
            if (level >= 50 && HasEffect(110))
            {
                return 70u;
            }
            if (level >= 52 && HasEffect(2513))
            {
                return 70u;
            }
            if (level >= 26 && HasEffect(107))
            {
                return OriginalHook(62u);
            }
            if (level >= 45 && HasEffect(108))
            {
                return 16473u;
            }
            if (level >= 30 && HasEffect(109))
            {
                return 70u;
            }
            return 62u;
        }
        return actionID;
    }
}
