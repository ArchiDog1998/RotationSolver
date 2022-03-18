namespace XIVComboExpandedPlugin.Combos;

internal class NinjaArmorCrushCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaArmorCrushCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 3563u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3563)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature))
			{
				if (level >= 90 && CustomCombo.HasEffect(2691))
				{
					return CustomCombo.OriginalHook(25777u);
				}
				if (level >= 90 && CustomCombo.HasEffect(2690))
				{
					return 25777u;
				}
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 2242 && level >= 54)
				{
					return 3563u;
				}
				if (lastComboMove == 2240 && level >= 4)
				{
					return 2242u;
				}
			}
			return 2240u;
		}
		return actionID;
	}
}
