namespace XIVComboExpandedPlugin.Combos;

internal class NinjaKassatsuChiJinFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 2261u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2261 && level >= 76 && CustomCombo.HasEffect(497))
		{
			return 18807u;
		}
		return actionID;
	}
}
