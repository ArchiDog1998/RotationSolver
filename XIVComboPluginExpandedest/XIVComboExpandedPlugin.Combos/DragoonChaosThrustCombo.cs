namespace XIVComboExpandedPlugin.Combos;

internal class DragoonChaosThrustCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonChaosThrustCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 88u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 88)
		{
			if (level >= 56 && CustomCombo.HasEffect(802))
			{
				return 3554u;
			}
			if (level >= 58 && CustomCombo.HasEffect(803))
			{
				return 3556u;
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 87 && level >= 50)
				{
					return CustomCombo.OriginalHook(88u);
				}
				if ((lastComboMove == 75 || lastComboMove == 16479) && level >= 18)
				{
					return 87u;
				}
			}
			return CustomCombo.OriginalHook(75u);
		}
		return actionID;
	}
}
