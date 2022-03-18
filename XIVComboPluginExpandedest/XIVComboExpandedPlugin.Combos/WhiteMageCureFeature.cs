namespace XIVComboExpandedPlugin.Combos;

internal class WhiteMageCureFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageCureFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 135u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 135 && level < 30)
		{
			return 120u;
		}
		return actionID;
	}
}
