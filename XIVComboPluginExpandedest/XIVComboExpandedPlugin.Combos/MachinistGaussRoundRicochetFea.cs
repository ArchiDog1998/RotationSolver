namespace XIVComboExpandedPlugin.Combos;

internal class MachinistGaussRoundRicochetFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistGaussRoundRicochetFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 2874u, 2890u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2874 || actionID == 2890)
		{
			if (level >= 50)
			{
				return CustomCombo.CalcBestAction(actionID, 2874u, 2890u);
			}
			return 2874u;
		}
		return actionID;
	}
}
