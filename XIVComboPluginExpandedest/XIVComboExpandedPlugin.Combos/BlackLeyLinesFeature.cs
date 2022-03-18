namespace XIVComboExpandedPlugin.Combos;

internal class BlackLeyLinesFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackLeyLinesFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 3573u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3573 && level >= 62 && CustomCombo.HasEffect(737))
		{
			return 7419u;
		}
		return actionID;
	}
}
