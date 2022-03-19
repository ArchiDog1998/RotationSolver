using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class BardShadowbiteFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardShadowbiteFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 106u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 106)
        {
            if (IsEnabled(CustomComboPreset.BardApexFeature))
            {
                BRDGauge jobGauge = GetJobGauge<BRDGauge>();
                if (level >= 80 && jobGauge.SoulVoice == 100)
                {
                    return 16496u;
                }
                if (level >= 86 && HasEffect(2692))
                {
                    return 25784u;
                }
            }
            if (level >= 72 && HasEffect(3002))
            {
                return 16494u;
            }
        }
        return actionID;
    }
}
