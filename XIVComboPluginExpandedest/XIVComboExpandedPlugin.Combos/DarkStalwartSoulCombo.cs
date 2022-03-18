namespace XIVComboExpandedPlugin.Combos;

internal class DarkStalwartSoulCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkStalwartSoulCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 16468u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 16468)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.DarkDeliriumFeature) && level >= 64 && level >= 68 && CustomCombo.HasEffect(1972))
			{
				return 7391u;
			}
			if (comboTime > 0f && lastComboMove == 3621 && level >= 72)
			{
				return 16468u;
			}
			return 3621u;
		}
		return actionID;
	}
}
