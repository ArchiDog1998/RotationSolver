using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BardStraightShotUpgradeFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardStraightShotUpgradeFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 97u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 97)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.BardApexFeature))
			{
				BRDGauge jobGauge = CustomCombo.GetJobGauge<BRDGauge>();
				if (level >= 80 && jobGauge.SoulVoice == 100)
				{
					return 16496u;
				}
				if (level >= 86 && CustomCombo.HasEffect(2692))
				{
					return 25784u;
				}
			}
			if (level >= 2 && CustomCombo.HasEffect(122))
			{
				return CustomCombo.OriginalHook(98u);
			}
		}
		return actionID;
	}
}
