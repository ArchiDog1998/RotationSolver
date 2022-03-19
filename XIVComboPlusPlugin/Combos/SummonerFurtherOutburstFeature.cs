using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SummonerFurtherOutburstFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerFurtherOutburstFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 16511u, 25826u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == 16511 || actionID == 25826) && HasEffect(2701) && OriginalHook(163u) != 25820 && OriginalHook(163u) != 16514)
        {
            return 7426u;
        }
        return actionID;
    }
}
