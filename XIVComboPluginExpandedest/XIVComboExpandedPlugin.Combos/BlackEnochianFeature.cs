using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BlackEnochianFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackEnochianFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { BLM.Fire4, BLM.Blizzard4 };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
        BLMGauge jobGauge = CustomCombo.GetJobGauge<BLMGauge>();
        if (level >= BLM.Levels.Blizzard4 && jobGauge.InUmbralIce)
        {
            return BLM.Blizzard4;
        }
        if (level >= BLM.Levels.Fire4)
        {
            return BLM.Fire4;
        }
        return actionID;
	}
}
