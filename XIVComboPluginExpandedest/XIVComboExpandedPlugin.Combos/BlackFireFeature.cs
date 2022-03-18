using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BlackFireFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFireFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 141u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 141)
		{
			BLMGauge jobGauge = CustomCombo.GetJobGauge<BLMGauge>();
			if (level >= 35 && (!jobGauge.InAstralFire || CustomCombo.HasEffect(165)))
			{
				return 152u;
			}
			return CustomCombo.OriginalHook(141u);
		}
		return actionID;
	}
}
