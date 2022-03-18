using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class WarriorMythrilTempestCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 16462u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 16462)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && CustomCombo.HasEffect(1177))
			{
				return CustomCombo.OriginalHook(3550u);
			}
			if (comboTime > 0f && lastComboMove == 41 && level >= 40)
			{
				byte beastGauge = CustomCombo.GetJobGauge<WARGauge>().BeastGauge;
				if (CustomCombo.IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature) && beastGauge >= 90 && level >= 74)
				{
					return CustomCombo.OriginalHook(3550u);
				}
				return 16462u;
			}
			return 41u;
		}
		return actionID;
	}
}
