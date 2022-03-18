namespace XIVComboExpandedPlugin.Combos;

internal class AstrologianBeneficFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianBeneficFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 3610u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3610 && level < 26)
		{
			return 3594u;
		}
		return actionID;
	}
}
