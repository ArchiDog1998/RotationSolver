namespace XIVComboExpandedPlugin.Combos;

internal class SamuraiKashaCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiKashaCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 7482u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 7482)
		{
			if (level >= 50 && CustomCombo.HasEffect(1233))
			{
				return 7482u;
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 7479 && level >= 40)
				{
					return 7482u;
				}
				if (lastComboMove == 7477 && level >= 18)
				{
					return 7479u;
				}
			}
			return 7477u;
		}
		return actionID;
	}
}
