namespace XIVComboExpandedPlugin.Combos;

internal class MachinistDrillAirAnchorFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistHotShotDrillChainsawFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[4] { 2872u, 16500u, 16498u, 25788u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2872 || actionID == 16500 || actionID == 16498 || actionID == 25788)
		{
			if (level >= 90)
			{
				return CustomCombo.CalcBestAction(actionID, 25788u, 16500u, 16498u);
			}
			if (level >= 76)
			{
				return CustomCombo.CalcBestAction(actionID, 16500u, 16498u);
			}
			if (level >= 58)
			{
				return CustomCombo.CalcBestAction(actionID, 16498u, 2872u);
			}
			return 2872u;
		}
		return actionID;
	}
}
