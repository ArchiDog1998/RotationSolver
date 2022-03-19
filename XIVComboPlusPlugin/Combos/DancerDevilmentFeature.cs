using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class DancerDevilmentFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDevilmentFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16011u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16011 && level >= 90 && HasEffect(2700))
        {
            return 25792u;
        }
        return actionID;
    }
}
