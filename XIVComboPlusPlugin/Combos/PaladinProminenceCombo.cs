using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class PaladinProminenceCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinProminenceCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16457u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16457)
        {
            if (comboTime > 0f && lastComboMove == 7381 && level >= 40)
            {
                return 16457u;
            }
            return 7381u;
        }
        return actionID;
    }
}
