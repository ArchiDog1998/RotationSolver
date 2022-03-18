namespace XIVComboExpandedPlugin.Combos;

internal class WarriorStormsEyeCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 45u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 45)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && CustomCombo.HasEffect(1177))
			{
				return CustomCombo.OriginalHook(3549u);
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 31 && level >= 4)
				{
					return 37u;
				}
				if (lastComboMove == 37 && level >= 50)
				{
					return 45u;
				}
			}
			return 31u;
		}
		return actionID;
	}
}
