namespace XIVComboExpandedPlugin.Combos;

internal class SummonerFurtherRuinFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerFurtherRuinFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[3] { 163u, 172u, 3579u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if ((actionID == 163 || actionID == 172 || actionID == 3579) && CustomCombo.HasEffect(2701) && CustomCombo.OriginalHook(163u) != 25820 && CustomCombo.OriginalHook(163u) != 16514)
		{
			return 7426u;
		}
		return actionID;
	}
}
