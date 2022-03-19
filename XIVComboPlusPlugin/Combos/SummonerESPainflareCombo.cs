using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SummonerESPainflareCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerESPainflareCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 3578u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 3578)
        {
            if (!GetJobGauge<SMNGauge>().HasAetherflowStacks)
            {
                return 16510u;
            }
            if (level >= 40)
            {
                return 3578u;
            }
            return 16510u;
        }
        return actionID;
    }
}
