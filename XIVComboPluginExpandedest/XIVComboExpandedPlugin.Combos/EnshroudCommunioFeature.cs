namespace XIVComboExpandedPlugin.Combos;

internal class EnshroudCommunioFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperEnshroudCommunioFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 24394u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 24394 && level >= 90 && CustomCombo.HasEffect(2593))
		{
			return 24398u;
		}
		return actionID;
	}
}
