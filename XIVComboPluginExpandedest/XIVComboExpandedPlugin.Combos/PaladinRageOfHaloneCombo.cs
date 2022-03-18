namespace XIVComboExpandedPlugin.Combos;

internal class PaladinRageOfHaloneCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinRageOfHaloneCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 21u, 3539u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 21 || actionID == 3539)
		{
			if (CustomCombo.IsEnabled(CustomComboPreset.PaladinAtonementFeature) && level >= 76 && CustomCombo.HasEffect(1902))
			{
				return 16460u;
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 15 && level >= 26)
				{
					return CustomCombo.OriginalHook(21u);
				}
				if (lastComboMove == 9 && level >= 4)
				{
					return 15u;
				}
			}
			return 9u;
		}
		return actionID;
	}
}
