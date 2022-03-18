namespace XIVComboExpandedPlugin.Combos;

internal class SummonerDemiCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerDemiCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[4] { 7427u, 25831u, 3581u, 25800u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7427 || actionID == 25831 || actionID == 3581 || actionID == 25800)
		{
			if (CustomCombo.OriginalHook(163u) == 25820 && level >= 70)
			{
				return 7429u;
			}
			if (CustomCombo.OriginalHook(163u) == 16514)
			{
				return 16516u;
			}
		}
		return actionID;
	}
}
