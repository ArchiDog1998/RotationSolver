using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class MachinistOverdriveFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverdriveFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 2864u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2864 || actionID == 16501)
		{
			MCHGauge jobGauge = CustomCombo.GetJobGauge<MCHGauge>();
			if (level >= 40 && jobGauge.IsRobotActive)
			{
				return CustomCombo.OriginalHook(7415u);
			}
		}
		return actionID;
	}
}
