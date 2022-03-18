using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BlackBlizzardFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackBlizzardFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 142u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 142)
		{
			BLMGauge jobGauge = CustomCombo.GetJobGauge<BLMGauge>();
			if (level >= 35 && !jobGauge.InUmbralIce)
			{
				return 154u;
			}
		}
		return actionID;
	}
}
