namespace XIVComboExpandedPlugin.Combos;

internal class NinjaBunshinKamaitachiFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaBunshinKamaitachiFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 16493u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 16493 && level >= 82 && CustomCombo.HasEffect(1954))
		{
			return 25774u;
		}
		return actionID;
	}
}
