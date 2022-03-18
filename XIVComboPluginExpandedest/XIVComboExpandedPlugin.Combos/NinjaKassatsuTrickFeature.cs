namespace XIVComboExpandedPlugin.Combos;

internal class NinjaKassatsuTrickFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 2264u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2264)
		{
			if (level >= 10 && CustomCombo.HasEffect(614))
			{
				return 2258u;
			}
			if (level >= 45 && CustomCombo.HasEffect(507))
			{
				return 2264u;
			}
		}
		return actionID;
	}
}
