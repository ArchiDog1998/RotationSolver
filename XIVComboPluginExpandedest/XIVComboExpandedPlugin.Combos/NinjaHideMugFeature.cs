using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class NinjaHideMugFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 2245u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 2245)
		{
			byte ninki = CustomCombo.GetJobGauge<NINGauge>().Ninki;
			IconReplacer.CooldownData cooldown = CustomCombo.GetCooldown(16493u);
			if (CustomCombo.HasCondition((ConditionFlag)26))
			{
				if (ninki >= 50 && level > 68 && CustomCombo.IsEnabled(CustomComboPreset.NinjaLiangPuFeature))
				{
					if (!cooldown.IsCooldown && level > 79)
					{
						return 16493u;
					}
					if (cooldown.IsCooldown)
					{
						return 7402u;
					}
				}
				return 2248u;
			}
		}
		return actionID;
	}
}
