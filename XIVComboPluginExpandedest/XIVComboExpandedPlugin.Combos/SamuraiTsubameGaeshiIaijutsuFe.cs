using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class SamuraiTsubameGaeshiIaijutsuFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiTsubameGaeshiIaijutsuFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 16483u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (actionID == 16483)
		{
			SAMGauge jobGauge = CustomCombo.GetJobGauge<SAMGauge>();
			if (level >= 76 && (int)jobGauge.Sen == 0)
			{
				return CustomCombo.OriginalHook(16483u);
			}
			return CustomCombo.OriginalHook(7867u);
		}
		return actionID;
	}
}
