using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class RedMageVerprocCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageVerprocCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 7511u, 7510u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		switch (actionID)
		{
		case 7511u:
		{
			RDMGauge jobGauge2 = CustomCombo.GetJobGauge<RDMGauge>();
			if (lastComboMove == 16530 && level >= 90)
			{
				return 25858u;
			}
			if ((lastComboMove == 7525 || lastComboMove == 7526) && level >= 80)
			{
				return 16530u;
			}
			if (level >= 70 && jobGauge2.ManaStacks == 3)
			{
				return 7526u;
			}
			if (level >= 68 && jobGauge2.ManaStacks == 3)
			{
				return 7525u;
			}
			if (CustomCombo.IsEnabled(CustomComboPreset.RedMageVerprocComboPlus) && level >= 10 && (CustomCombo.HasEffect(1249) || CustomCombo.HasEffect(1238) || CustomCombo.HasEffect(167) || CustomCombo.HasEffect(2560)))
			{
				return CustomCombo.OriginalHook(7507u);
			}
			if (CustomCombo.IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeatureStone) && level >= 10 && !CustomCombo.HasCondition((ConditionFlag)26) && !CustomCombo.HasEffect(1235))
			{
				return CustomCombo.OriginalHook(7507u);
			}
			if (CustomCombo.HasEffect(1235))
			{
				return 7511u;
			}
			return CustomCombo.OriginalHook(7524u);
		}
		case 7510u:
		{
			RDMGauge jobGauge = CustomCombo.GetJobGauge<RDMGauge>();
			if (level >= 90 && lastComboMove == 16530)
			{
				return 25858u;
			}
			if (level >= 80 && (lastComboMove == 7525 || lastComboMove == 7526))
			{
				return 16530u;
			}
			if (level >= 68 && jobGauge.ManaStacks == 3)
			{
				return 7525u;
			}
			if (CustomCombo.IsEnabled(CustomComboPreset.RedMageVerprocComboPlus) && level >= 4 && (CustomCombo.HasEffect(1249) || CustomCombo.HasEffect(1238) || CustomCombo.HasEffect(167) || CustomCombo.HasEffect(2560)))
			{
				return CustomCombo.OriginalHook(7505u);
			}
			if (CustomCombo.IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeatureFire) && level >= 4 && !CustomCombo.HasCondition((ConditionFlag)26) && !CustomCombo.HasEffect(1234))
			{
				return CustomCombo.OriginalHook(7505u);
			}
			if (CustomCombo.HasEffect(1234))
			{
				return 7510u;
			}
			return CustomCombo.OriginalHook(7524u);
		}
		default:
			return actionID;
		}
	}
}
