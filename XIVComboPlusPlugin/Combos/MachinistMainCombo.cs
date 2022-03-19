using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class MachinistMainCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistMainCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 2873u, 7413u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 2873 || actionID == 7413)
        {
            IconReplacer.CooldownData cooldown = GetCooldown(2876u);
            MCHGauge jobGauge = GetJobGauge<MCHGauge>();
            IconReplacer.CooldownData cooldown2 = GetCooldown(16498u);
            IconReplacer.CooldownData cooldown3 = GetCooldown(16500u);
            float num = cooldown2.CooldownRemaining - cooldown.CooldownRemaining;
            if (IsEnabled(CustomComboPreset.MachinistZiDongFeature))
            {
                if (jobGauge.IsOverheated && level >= 35)
                {
                    return 7410u;
                }
                if (cooldown.IsCooldown && cooldown2.CooldownRemaining <= 1f && num < 0f && level >= 58)
                {
                    return 16498u;
                }
                if (cooldown.IsCooldown && cooldown3.CooldownRemaining <= 1f && level >= 76)
                {
                    return 16500u;
                }
            }
            if (comboTime > 0f)
            {
                if (lastComboMove == 2868 && level >= 26)
                {
                    return OriginalHook(2873u);
                }
                if (lastComboMove == 2866 && level >= 2)
                {
                    return OriginalHook(2868u);
                }
            }
            return OriginalHook(2866u);
        }
        return actionID;
    }
}
