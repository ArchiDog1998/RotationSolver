using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class SamuraiShoha2Feature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiShoha2Feature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 7491u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7491)
		{
			SAMGauge jobGauge = CustomCombo.GetJobGauge<SAMGauge>();
			if (level >= 82 && jobGauge.MeditationStacks >= 3)
			{
				return 25779u;
			}
		}
		return actionID;
	}
}
