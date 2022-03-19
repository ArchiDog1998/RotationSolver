using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SummonerShinyOutburstFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerShinyOutburstFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 16511u, 25826u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16511 || actionID == 25826)
        {
            if (IsEnabled(CustomComboPreset.SummonerMountainBusterFeature) && OriginalHook(25822u) == 25836)
            {
                return OriginalHook(25822u);
            }
            if (OriginalHook(25884u) != 25884)
            {
                return OriginalHook(25884u);
            }
        }
        return actionID;
    }
}
