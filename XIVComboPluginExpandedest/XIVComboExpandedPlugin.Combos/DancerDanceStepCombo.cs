using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class DancerDanceStepCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 15997u, 15998u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		switch (actionID)
		{
		case 15997u:
		{
			DNCGauge jobGauge2 = CustomCombo.GetJobGauge<DNCGauge>();
			if (level >= 15 && jobGauge2.IsDancing && CustomCombo.HasEffect(1818))
			{
				if (jobGauge2.CompletedSteps < 2)
				{
					return jobGauge2.NextStep;
				}
				return CustomCombo.OriginalHook(15997u);
			}
			return 15997u;
		}
		case 15998u:
		{
			DNCGauge jobGauge = CustomCombo.GetJobGauge<DNCGauge>();
			if (level >= 70 && jobGauge.IsDancing && CustomCombo.HasEffect(1819) && jobGauge.CompletedSteps < 4)
			{
				return jobGauge.NextStep;
			}
			return CustomCombo.OriginalHook(15998u);
		}
		default:
			return actionID;
		}
	}
}
