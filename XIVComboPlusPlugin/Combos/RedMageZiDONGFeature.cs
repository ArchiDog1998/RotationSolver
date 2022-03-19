using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class RedMageZiDONGFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageZiDONGFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 7524u, 7503u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7524 || actionID == 7503)
        {
            RDMGauge jobGauge = GetJobGauge<RDMGauge>();
            if (Math.Abs(jobGauge.BlackMana - jobGauge.WhiteMana) < 21 && HasEffect(1234) && !HasEffect(1249) && !HasEffect(167))
            {
                return 7510u;
            }
            if (Math.Abs(jobGauge.BlackMana - jobGauge.WhiteMana) < 21 && HasEffect(1235) && !HasEffect(1249) && !HasEffect(167))
            {
                return 7511u;
            }
            if (jobGauge.BlackMana >= jobGauge.WhiteMana && level >= 70)
            {
                if (HasEffect(1235) && !HasEffect(1249) && !HasEffect(167))
                {
                    return 7511u;
                }
                if (HasEffect(1249) || HasEffect(167))
                {
                    return 7507u;
                }
            }
            if (jobGauge.BlackMana < jobGauge.WhiteMana && level >= 70)
            {
                if (HasEffect(1234))
                {
                    return 7510u;
                }
                if (HasEffect(1249) || HasEffect(167))
                {
                    return 7505u;
                }
            }
            return OriginalHook(7503u);
        }
        return actionID;
    }
}
