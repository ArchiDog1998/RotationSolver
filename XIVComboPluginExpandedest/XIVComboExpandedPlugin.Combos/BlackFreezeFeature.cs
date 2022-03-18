using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class BlackFreezeFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFreezeFlareFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[2] { 159u, 162u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 159 || actionID == 162)
		{
			BLMGauge jobGauge = CustomCombo.GetJobGauge<BLMGauge>();
			if (level >= 40 && jobGauge.InUmbralIce)
			{
				return 159u;
			}
			if (level >= 50)
			{
				return 162u;
			}
		}
		return actionID;
	}
}
