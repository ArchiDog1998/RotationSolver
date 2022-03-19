using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class GunbreakerSolidBarrelCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerSolidBarrelCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16145u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16145)
        {
            GNBGauge jobGauge = GetJobGauge<GNBGauge>();
            IconReplacer.CooldownData cooldown = GetCooldown(16138u);
            IconReplacer.CooldownData cooldown2 = GetCooldown(16146u);
            byte ammoComboStep = GetJobGauge<GNBGauge>().AmmoComboStep;
            IconReplacer.CooldownData cooldown3 = GetCooldown(16153u);
            if (IsEnabled(CustomComboPreset.GunbreakerZiDongeature))
            {
                if (HasEffect(1831) && level >= 54 && !cooldown3.IsCooldown)
                {
                    return 16153u;
                }
                if (level >= 70)
                {
                    if (HasEffect(1842))
                    {
                        return 16156u;
                    }
                    if (HasEffect(1843))
                    {
                        return 16157u;
                    }
                    if (HasEffect(1844))
                    {
                        return 16158u;
                    }
                }
                if (level >= 60 && (cooldown.CooldownRemaining > 20f || cooldown.CooldownRemaining <= 20f && HasEffect(1831)))
                {
                    if (ammoComboStep == 0 && jobGauge.Ammo >= 1 && !cooldown2.IsCooldown && level >= 60)
                    {
                        return 16146u;
                    }
                    switch (ammoComboStep)
                    {
                        case 1:
                            return 16147u;
                        case 2:
                            return 16150u;
                    }
                }
                if (HasEffect(1831) && level >= 30 && jobGauge.Ammo >= 1)
                {
                    return OriginalHook(16162u);
                }
            }
            if (comboTime > 0f)
            {
                if (lastComboMove == 16139 && level >= 26)
                {
                    return 16145u;
                }
                if (lastComboMove == 16137 && level >= 4)
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature) && level >= 72)
                    {
                        int num = level >= 88 ? 3 : 2;
                        if (jobGauge.Ammo == num)
                        {
                            return 16163u;
                        }
                    }
                    return 16139u;
                }
            }
            return 16137u;
        }
        return actionID;
    }
}
