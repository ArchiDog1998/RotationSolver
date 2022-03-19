namespace XIVComboPlus.Combos;

internal class DragoonJumpFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonJumpFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 92u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 92)
        {
            if (level >= 68 && HasEffect(1243))
            {
                return 7399u;
            }
            return OriginalHook(92u);
        }
        return actionID;
    }
}
