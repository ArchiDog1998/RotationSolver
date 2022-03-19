using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SamuraiOkaCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiOkaCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 7485u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7485)
        {
            if (level >= 50 && HasEffect(1233))
            {
                return 7485u;
            }
            if (comboTime > 0f && (lastComboMove == 7483 || lastComboMove == 25780) && level >= 45)
            {
                return 7485u;
            }
            return OriginalHook(7483u);
        }
        return actionID;
    }
}
