namespace XIVComboExpandedPlugin.Combos;

internal class SCHYingJitFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHYingJitFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 167u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3586)
		{
			IconReplacer.CooldownData cooldown = CustomCombo.GetCooldown(3586u);
			if (BuffDuration(792) > 5f || lastComboMove == 3586 || cooldown.IsCooldown)
			{
				return 186u;
			}
			return 3586u;
		}
		return actionID;
	}
}
