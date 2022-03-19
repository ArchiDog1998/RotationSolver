using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SummonerMountainBusterFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerMountainBusterFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 25883u, 25884u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == 25883 || actionID == 25884) && OriginalHook(25822u) == 25836)
        {
            return OriginalHook(25822u);
        }
        return actionID;
    }
}
