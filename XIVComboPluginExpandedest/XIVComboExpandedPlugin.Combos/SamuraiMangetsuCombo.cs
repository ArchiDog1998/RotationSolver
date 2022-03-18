namespace XIVComboExpandedPlugin.Combos;

internal class SamuraiMangetsuCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiMangetsuCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 7484u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7484)
		{
			if (level >= 50 && CustomCombo.HasEffect(1233))
			{
				return 7484u;
			}
			if (comboTime > 0f && (lastComboMove == 7483 || lastComboMove == 25780) && level >= 35)
			{
				return 7484u;
			}
			return CustomCombo.OriginalHook(7483u);
		}
		return actionID;
	}
}
