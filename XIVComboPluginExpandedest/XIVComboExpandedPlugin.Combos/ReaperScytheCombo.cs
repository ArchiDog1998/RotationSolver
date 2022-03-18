namespace XIVComboExpandedPlugin.Combos;

internal class ReaperScytheCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperScytheCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 24377u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 24377)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.ReaperSoulReaverGuillotineFeature) && level >= 70 && (CustomCombo.HasEffect(2587) || CustomCombo.HasEffect(2593)))
			{
				return CustomCombo.OriginalHook(24384u);
			}
			if (comboTime > 0f && lastComboMove == 24376 && level >= 45)
			{
				return 24377u;
			}
			return 24376u;
		}
		return actionID;
	}
}
