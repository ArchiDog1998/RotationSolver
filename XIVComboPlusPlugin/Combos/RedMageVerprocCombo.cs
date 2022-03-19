using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class RedMageVerprocCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageVerprocCombo;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 7511u, 7510u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        switch (actionID)
        {
            case 7511u:
                {
                    RDMGauge jobGauge2 = GetJobGauge<RDMGauge>();
                    if (lastComboMove == 16530 && level >= 90)
                    {
                        return 25858u;
                    }
                    if ((lastComboMove == 7525 || lastComboMove == 7526) && level >= 80)
                    {
                        return 16530u;
                    }
                    if (level >= 70 && jobGauge2.ManaStacks == 3)
                    {
                        return 7526u;
                    }
                    if (level >= 68 && jobGauge2.ManaStacks == 3)
                    {
                        return 7525u;
                    }
                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus) && level >= 10 && (HasEffect(1249) || HasEffect(1238) || HasEffect(167) || HasEffect(2560)))
                    {
                        return OriginalHook(7507u);
                    }
                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeatureStone) && level >= 10 && !HasCondition((ConditionFlag)26) && !HasEffect(1235))
                    {
                        return OriginalHook(7507u);
                    }
                    if (HasEffect(1235))
                    {
                        return 7511u;
                    }
                    return OriginalHook(7524u);
                }
            case 7510u:
                {
                    RDMGauge jobGauge = GetJobGauge<RDMGauge>();
                    if (level >= 90 && lastComboMove == 16530)
                    {
                        return 25858u;
                    }
                    if (level >= 80 && (lastComboMove == 7525 || lastComboMove == 7526))
                    {
                        return 16530u;
                    }
                    if (level >= 68 && jobGauge.ManaStacks == 3)
                    {
                        return 7525u;
                    }
                    if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus) && level >= 4 && (HasEffect(1249) || HasEffect(1238) || HasEffect(167) || HasEffect(2560)))
                    {
                        return OriginalHook(7505u);
                    }
                    if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeatureFire) && level >= 4 && !HasCondition((ConditionFlag)26) && !HasEffect(1234))
                    {
                        return OriginalHook(7505u);
                    }
                    if (HasEffect(1234))
                    {
                        return 7510u;
                    }
                    return OriginalHook(7524u);
                }
            default:
                return actionID;
        }
    }
}
