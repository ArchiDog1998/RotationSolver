namespace XIVComboExpandedPlugin.Combos;

internal class SummonerShinyRuinFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerShinyRuinFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[3] { 163u, 172u, 3579u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 163 || actionID == 172 || actionID == 3579)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.SummonerMountainBusterFeature) && CustomCombo.OriginalHook(25822u) == 25836)
			{
				return CustomCombo.OriginalHook(25822u);
			}
			if (CustomCombo.OriginalHook(25883u) != 25883)
			{
				return CustomCombo.OriginalHook(25883u);
			}
		}
		return actionID;
	}
}
