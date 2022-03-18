namespace XIVComboExpandedPlugin.Combos;

internal class DragoonCoerthanTormentCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonCoerthanTormentCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 16477u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 16477)
		{
			if (comboTime > 0f)
			{
				if (lastComboMove == 7397 && level >= 72)
				{
					return 16477u;
				}
				if ((lastComboMove == 86 || lastComboMove == 25770) && level >= 62)
				{
					return 7397u;
				}
			}
			return CustomCombo.OriginalHook(86u);
		}
		return actionID;
	}
}
