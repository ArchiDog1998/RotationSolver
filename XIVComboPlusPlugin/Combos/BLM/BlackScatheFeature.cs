using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackScatheFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackScatheFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 156u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 156)
        {
            BLMGauge jobGauge = GetJobGauge<BLMGauge>();
            if (level >= 80 && jobGauge.PolyglotStacks > 0)
            {
                return 16507u;
            }
        }
        return actionID;
    }
}
