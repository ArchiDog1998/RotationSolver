namespace XIVComboExpandedPlugin.Combos;

internal class BardRainOfDeathFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardRainOfDeathFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 117u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 117)
		{
			if (level >= 60)
			{
				return CustomCombo.CalcBestAction(actionID, 117u, 3558u, 3562u);
			}
			if (level >= 54)
			{
				return CustomCombo.CalcBestAction(actionID, 117u, 3558u);
			}
			if (level >= 45)
			{
				return 117u;
			}
		}
		return actionID;
	}
}
