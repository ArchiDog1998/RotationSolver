namespace XIVComboExpandedPlugin.Combos;

internal class RedMageAoECombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageAoECombo;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 16525u, 16524u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if ((actionID == 16525 || actionID == 16524) && (CustomCombo.HasEffect(1249) || CustomCombo.HasEffect(1238) || CustomCombo.HasEffect(167) || CustomCombo.HasEffect(2560)))
		{
			return CustomCombo.OriginalHook(16526u);
		}
		return actionID;
	}
}
