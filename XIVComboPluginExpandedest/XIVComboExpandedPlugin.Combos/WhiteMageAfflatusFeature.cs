using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class WhiteMageAfflatusFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageAfflatusFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 135u, 124u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		switch (actionID)
		{
		case 135u:
		{
			WHMGauge jobGauge2 = CustomCombo.GetJobGauge<WHMGauge>();
			if (CustomCombo.IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryFeature) && level >= 74 && jobGauge2.BloodLily == 3)
			{
				return 16535u;
			}
			if (level >= 52 && jobGauge2.Lily > 0)
			{
				return 16531u;
			}
			return actionID;
		}
		case 124u:
		{
			WHMGauge jobGauge = CustomCombo.GetJobGauge<WHMGauge>();
			if (CustomCombo.IsEnabled(CustomComboPreset.WhiteMageRaptureMiseryFeature) && level >= 74 && jobGauge.BloodLily == 3)
			{
				return 16535u;
			}
			if (level >= 76 && jobGauge.Lily > 0)
			{
				return 16534u;
			}
			return actionID;
		}
		default:
			return actionID;
		}
	}
}
