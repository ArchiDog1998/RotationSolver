using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SamuraiIaijutsuShohaFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiIaijutsuShohaFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 7867u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7867)
        {
            SAMGauge jobGauge = GetJobGauge<SAMGauge>();
            if (level >= 80 && jobGauge.MeditationStacks >= 3)
            {
                return 16487u;
            }
        }
        return actionID;
    }
}
