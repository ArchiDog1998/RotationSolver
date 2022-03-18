using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class DragoonDiveFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonDiveFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[3] { 95u, 96u, 16480u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 95 || actionID == 96 || actionID == 16480)
		{
			if (level >= 80)
			{
				if (CustomCombo.GetJobGauge<DRGGauge>().IsLOTDActive)
				{
					return CustomCombo.CalcBestAction(actionID, 95u, 96u, 16480u);
				}
				return CustomCombo.CalcBestAction(actionID, 95u, 96u);
			}
			if (level >= 50)
			{
				return CustomCombo.CalcBestAction(actionID, 95u, 96u);
			}
			if (level >= 45)
			{
				return 95u;
			}
		}
		return actionID;
	}
}
