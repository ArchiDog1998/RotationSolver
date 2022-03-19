//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class NinjaRenShuFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaRenShuFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 2259u, 2263u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        int hutonTimer = GetJobGauge<NINGauge>().HutonTimer;
//        if (actionID == 2259 && hutonTimer >= 5)
//        {
//            IconReplacer.CooldownData cooldown = GetCooldown(2258u);
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 1 && level > 45)
//            {
//                return OriginalHook(2261u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && (BuffStacks(496) == 9 && cooldown.CooldownRemaining >= 17f || HasEffect(507) || BuffStacks(496) == 57) && level > 45)
//            {
//                return OriginalHook(2260u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 9 && level > 45 && hutonTimer >= 5 && cooldown.CooldownRemaining < 17f && !HasEffect(507))
//            {
//                return OriginalHook(2263u);
//            }
//            if (HasEffect(497) && HasEffect(496) && BuffStacks(496) == 1 && level >= 76)
//            {
//                return OriginalHook(2263u);
//            }
//            if (HasEffect(497) && HasEffect(496) && BuffStacks(496) == 13 && level >= 76)
//            {
//                return OriginalHook(2260u);
//            }
//            if (HasEffect(1186) && BuffStacks(1186) == 1 && level >= 70)
//            {
//                return OriginalHook(2261u);
//            }
//            if (HasEffect(1186) && BuffStacks(1186) == 9 && level >= 70)
//            {
//                return OriginalHook(2263u);
//            }
//        }
//        if (actionID == 2259 && hutonTimer < 5)
//        {
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 3 && level > 45)
//            {
//                return OriginalHook(2261u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 11 && level > 45)
//            {
//                return OriginalHook(2259u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 27 && level > 45)
//            {
//                return OriginalHook(2260u);
//            }
//            return OriginalHook(2263u);
//        }
//        if (actionID == 2263 && hutonTimer >= 5)
//        {
//            if (HasEffect(496) && BuffStacks(496) == 3 && level > 35)
//            {
//                return OriginalHook(2259u);
//            }
//            if (HasEffect(496) && BuffStacks(496) == 7 && level > 35)
//            {
//                return OriginalHook(2260u);
//            }
//            if (HasEffect(1186) && BuffStacks(1186) == 3 && level >= 70)
//            {
//                return OriginalHook(2259u);
//            }
//            if (HasEffect(1186) && BuffStacks(1186) == 7 && level >= 70)
//            {
//                return OriginalHook(2261u);
//            }
//        }
//        if (actionID == 2263 && hutonTimer < 5)
//        {
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 3 && level > 45)
//            {
//                return OriginalHook(2261u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 11 && level > 45)
//            {
//                return OriginalHook(2259u);
//            }
//            if (HasEffect(496) && !HasEffect(497) && BuffStacks(496) == 27 && level > 45)
//            {
//                return OriginalHook(2260u);
//            }
//            return OriginalHook(2263u);
//        }
//        return actionID;
//    }
//}
