using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace XIVComboExpandedPlugin.Combos;

internal class DarkSouleaterCombo : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkSouleaterCombo;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 3632u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3632)
		{
			IconReplacer.CooldownData cooldown = CustomCombo.GetCooldown(3617u);
			IconReplacer.CooldownData cooldown2 = CustomCombo.GetCooldown(16472u);
			byte blood = CustomCombo.GetJobGauge<DRKGauge>().Blood;
			if (blood >= 50 && !cooldown2.IsCooldown && level >= 80 && CustomCombo.IsEnabled(CustomComboPreset.DRKFoLeiFeature) && (double)cooldown.CooldownRemaining > 0.7)
			{
				return 16472u;
			}
			if (blood >= 70 && CustomCombo.IsEnabled(CustomComboPreset.DRKOvercapFeature) && CustomCombo.HasEffect(742))
			{
				return 7391u;
			}
			if (CustomCombo.IsEnabled(CustomComboPreset.DarkDeliriumFeature) && level >= 62 && level >= 68 && CustomCombo.HasEffect(1972))
			{
				return 7392u;
			}
			if (comboTime > 0f)
			{
				if (lastComboMove == 3623 && level >= 26)
				{
					if (blood >= 70 && CustomCombo.IsEnabled(CustomComboPreset.DRKOvercapFeature))
					{
						return 7392u;
					}
					return 3632u;
				}
				if (lastComboMove == 3617 && level >= 2)
				{
					if (CustomCombo.IsEnabled(CustomComboPreset.DRKMPOvercapFeature))
					{
						PlayerCharacter? localPlayer = CustomCombo.LocalPlayer;
						if (localPlayer != null && ((Character)localPlayer).CurrentMp > 8000)
						{
							if (level >= 30 && level < 40 && (double)cooldown.CooldownRemaining > 0.7)
							{
								return 16466u;
							}
							if (level >= 40 && (double)cooldown.CooldownRemaining > 0.7)
							{
								return CustomCombo.OriginalHook(16467u);
							}
						}
					}
					if (blood >= 70 && CustomCombo.IsEnabled(CustomComboPreset.DRKOvercapFeature) && CustomCombo.HasEffect(742))
					{
						return 7392u;
					}
					return 3623u;
				}
			}
			return 3617u;
		}
		return actionID;
	}
}
