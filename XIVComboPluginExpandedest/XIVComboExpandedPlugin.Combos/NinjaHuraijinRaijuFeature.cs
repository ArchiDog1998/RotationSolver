namespace XIVComboExpandedPlugin.Combos;

internal class NinjaHuraijinRaijuFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHuraijinRaijuFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 25876u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 25876)
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
		return actionID;
	}
}
