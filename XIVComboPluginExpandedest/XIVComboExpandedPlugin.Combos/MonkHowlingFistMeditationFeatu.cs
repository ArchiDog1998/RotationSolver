using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class MonkHowlingFistMeditationFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkHowlingFistMeditationFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 25763u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 25763)
		{
			MNKGauge jobGauge = CustomCombo.GetJobGauge<MNKGauge>();
			if (level >= 15 && jobGauge.Chakra < 5)
			{
				return 3546u;
			}
			return CustomCombo.OriginalHook(25763u);
		}
		return actionID;
	}
}
