namespace XIVComboExpandedPlugin.Combos;

internal class SamuraiGekkoCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiGekkoCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 7481u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7481)
		{
			if (level >= 50 && CustomCombo.HasEffect(1233))
			{
				return 7481u;
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 7478 && level >= 30)
				{
					return 7481u;
				}
				if (lastComboMove == 7477 && level >= 4)
				{
					return 7478u;
				}
			}
			return 7477u;
		}
		return actionID;
	}
}
