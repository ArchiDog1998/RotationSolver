using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackEnochianFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackEnochianFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { (uint)BLM.Actions.Blizzard4, (uint)BLM.Actions.Fire4 };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        BLMGauge jobGauge = GetJobGauge<BLMGauge>();
        if (level >= (byte)BLM.Levels.Blizzard4 && jobGauge.InUmbralIce)
        {
            return (uint)BLM.Actions.Blizzard4;
        }
        if (level >= (byte)BLM.Levels.Fire4)
        {
            return (uint)BLM.Actions.Fire4;
        }
        return actionID;
    }
}
