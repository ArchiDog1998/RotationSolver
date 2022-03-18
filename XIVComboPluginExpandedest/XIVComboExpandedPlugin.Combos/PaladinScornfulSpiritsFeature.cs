namespace XIVComboExpandedPlugin.Combos;

internal class PaladinScornfulSpiritsFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 29u, 23u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 29 || actionID == 23)
		{
			if (level >= 86)
			{
				return CustomCombo.CalcBestAction(actionID, 25747u, 23u);
			}
			if (level >= 50)
			{
				return CustomCombo.CalcBestAction(actionID, 29u, 23u);
			}
			return 29u;
		}
		return actionID;
	}
}
