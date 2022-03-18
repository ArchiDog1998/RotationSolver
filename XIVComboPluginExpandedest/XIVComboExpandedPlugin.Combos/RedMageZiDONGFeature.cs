using System;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class RedMageZiDONGFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageZiDONGFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 7524u, 7503u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7524 || actionID == 7503)
		{
			RDMGauge jobGauge = CustomCombo.GetJobGauge<RDMGauge>();
			if (Math.Abs(jobGauge.BlackMana - jobGauge.WhiteMana) < 21 && CustomCombo.HasEffect(1234) && !CustomCombo.HasEffect(1249) && !CustomCombo.HasEffect(167))
			{
				return 7510u;
			}
			if (Math.Abs(jobGauge.BlackMana - jobGauge.WhiteMana) < 21 && CustomCombo.HasEffect(1235) && !CustomCombo.HasEffect(1249) && !CustomCombo.HasEffect(167))
			{
				return 7511u;
			}
			if (jobGauge.BlackMana >= jobGauge.WhiteMana && level >= 70)
			{
				if (CustomCombo.HasEffect(1235) && !CustomCombo.HasEffect(1249) && !CustomCombo.HasEffect(167))
				{
					return 7511u;
				}
				if (CustomCombo.HasEffect(1249) || CustomCombo.HasEffect(167))
				{
					return 7507u;
				}
			}
			if (jobGauge.BlackMana < jobGauge.WhiteMana && level >= 70)
			{
				if (CustomCombo.HasEffect(1234))
				{
					return 7510u;
				}
				if (CustomCombo.HasEffect(1249) || CustomCombo.HasEffect(167))
				{
					return 7505u;
				}
			}
			return CustomCombo.OriginalHook(7503u);
		}
		return actionID;
	}
}
