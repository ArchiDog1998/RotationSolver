namespace XIVComboExpandedPlugin.Combos;

internal class ReaperHarvestFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperHarvestFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 24405u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 24405 && level >= 88 && CustomCombo.HasEffect(2592))
		{
			return 24385u;
		}
		return actionID;
	}
}
