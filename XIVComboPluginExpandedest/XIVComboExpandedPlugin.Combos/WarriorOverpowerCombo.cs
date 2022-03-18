using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class WarriorOverpowerCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorOverpowerCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 41u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 41)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && CustomCombo.HasEffect(1177))
			{
				return CustomCombo.OriginalHook(3550u);
			}
			byte beastGauge = CustomCombo.GetJobGauge<WARGauge>().BeastGauge;
			if (comboTime > 0f && lastComboMove == 41 && level >= 40)
			{
				if (beastGauge >= 90 && level >= 74 && CustomCombo.IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
				{
					return CustomCombo.OriginalHook(3550u);
				}
				return 16462u;
			}
		}
		return actionID;
	}
}
