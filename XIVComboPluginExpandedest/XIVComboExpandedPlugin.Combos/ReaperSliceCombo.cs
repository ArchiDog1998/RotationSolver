namespace XIVComboExpandedPlugin.Combos;

internal class ReaperSliceCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSliceCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 24375u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 24375)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.ReaperSoulReaverGibbetFeature) && level >= 70 && (CustomCombo.HasEffect(2587) || CustomCombo.HasEffect(2593)))
			{
				if (CustomCombo.HasEffect(2588))
				{
					return CustomCombo.OriginalHook(24382u);
				}
				if (CustomCombo.HasEffect(2589))
				{
					return CustomCombo.OriginalHook(24383u);
				}
				if (CustomCombo.IsEnabled(CustomComboPreset.ReaperSoulReaverGibbetOption))
				{
					return CustomCombo.OriginalHook(24383u);
				}
				return CustomCombo.OriginalHook(24382u);
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 24374 && level >= 30)
				{
					return 24375u;
				}
				if (lastComboMove == 24373 && level >= 5)
				{
					return 24374u;
				}
			}
			return 24373u;
		}
		return actionID;
	}
}
