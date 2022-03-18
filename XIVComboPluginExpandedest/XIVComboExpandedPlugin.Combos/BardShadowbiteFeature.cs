using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BardShadowbiteFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardShadowbiteFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 106u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 106)
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
			if (level >= 72 && CustomCombo.HasEffect(3002))
			{
				return 16494u;
			}
		}
		return actionID;
	}
}
