using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets2;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using System.Configuration;

namespace RotationSolver.Updaters;
internal static class StateUpdater
{
    private static bool CanUseHealAction =>
        //PvP
        DataCenter.Territory.IsPvpZone
        //Job
        || (DataCenter.Role == JobRole.Healer || Service.Config.UseHealWhenNotAHealer)
        && Service.Config.AutoHeal
        && CustomRotation.IsLongerThan(Service.Config.GetValue(PluginConfigFloat.AutoHealTimeToKill));

    public static void UpdateState()
    {
        DataCenter.CommandStatus = StatusFromCmdOrCondition();
        DataCenter.AutoStatus = StatusFromAutomatic();
    }

    private static AutoStatus StatusFromAutomatic()
    {
        AutoStatus status = AutoStatus.None;

        if (DataCenter.DeathTarget is not null)
        {
            status |= AutoStatus.Raise;
        }

        if ((DataCenter.HPNotFull || DataCenter.Role != JobRole.Healer) && CanUseHealAction
            && (DataCenter.InCombat || Service.Config.HealOutOfCombat))
        {
            if (DataCenter.CanHealAreaAbility)
            {
                status |= AutoStatus.HealAreaAbility;
            }
            if (DataCenter.CanHealAreaSpell)
            {
                status |= AutoStatus.HealAreaSpell;
            }
            if (DataCenter.CanHealSingleAbility)
            {
                status |= AutoStatus.HealSingleAbility;
            }
            if (DataCenter.CanHealSingleSpell)
            {
                status |= AutoStatus.HealSingleSpell;
            }
        }

        if (DataCenter.InCombat)
        {
            if (Service.Config.UseDefenseAbility)
            {
                if (DataCenter.IsHostileCastingAOE)
                {
                    status |= AutoStatus.DefenseArea;
                }

                if (DataCenter.Role == JobRole.Healer || DataCenter.Job == ECommons.ExcelServices.Job.PLD) // Help defense.
                {
                    if (DataCenter.PartyMembers.Any((tank) =>
                    {
                        var attackingTankObj = DataCenter.AllHostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                        if (attackingTankObj.Count() != 1) return false;

                        return DataCenter.IsHostileCastingToTank;
                    }))
                    {
                        status |= AutoStatus.DefenseSingle;
                    }
                }
                else if (DataCenter.Role == JobRole.Tank) // defense self.
                {
                    var movingHere = (float)DataCenter.NumberOfHostilesInRange / DataCenter.NumberOfHostilesInMaxRange > 0.3f;

                    var tarOnMe = DataCenter.TarOnMeTargets.Where(t => t.DistanceToPlayer() <= 3);
                    var tarOnMeCount = tarOnMe.Count();
                    var attackedCount = tarOnMe.Count(ObjectHelper.IsAttacked);
                    var attacked = (float)attackedCount / tarOnMeCount > 0.7f;

                    //A lot targets are targeting on me.
                    if (tarOnMeCount >= Service.Config.GetValue(PluginConfigInt.AutoDefenseNumber)
                        && Player.Object.GetHealthRatio() <= Service.Config.GetValue(JobConfigFloat.HealthForAutoDefense)
                        && movingHere && attacked)
                    {
                        status |= AutoStatus.DefenseSingle;
                    }

                    //Big damage casting action.
                    if (DataCenter.IsHostileCastingToTank)
                    {
                        status |= AutoStatus.DefenseSingle;
                    }
                }
            }


            if (DataCenter.Role == JobRole.Tank
                && (Service.Config.GetValue(PluginConfigBool.AutoProvokeForTank) || DataCenter.AllianceTanks.Count() < 2)
                && DataCenter.CanProvoke)
            {
                status |= AutoStatus.Provoke;
            }
        }


        if ((!DataCenter.HasHostilesInRange || Service.Config.GetValue(PluginConfigBool.EsunaAll) || (DataCenter.Territory?.IsPvpZone ?? false))
            && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
        {
            status |= AutoStatus.Dispel;
        }

        if (DataCenter.CanInterruptTargets.Any())
        {
            status |= AutoStatus.Interrupt;
        }


        if (Service.Config.GetValue(PluginConfigBool.AutoTankStance) && DataCenter.Role == JobRole.Tank
            && !DataCenter.AllianceTanks.Any(t => t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.TankStanceStatus))
            && !CustomRotation.HasTankStance)
        {
            status |= AutoStatus.TankStance;
        }

        if (DataCenter.IsMoving && DataCenter.NotInCombatDelay && Service.Config.GetValue(PluginConfigBool.AutoSpeedOutOfCombat))
        {
            status |= AutoStatus.Speed;
        }

        return status;
    }


    private static AutoStatus StatusFromCmdOrCondition()
    {
        var status = DataCenter.SpecialType switch
        {
            SpecialCommandType.HealArea => AutoStatus.HealAreaSpell
                                | AutoStatus.HealAreaAbility,
            SpecialCommandType.HealSingle => AutoStatus.HealSingleSpell
                                | AutoStatus.HealSingleAbility,
            SpecialCommandType.DefenseArea => AutoStatus.DefenseArea,
            SpecialCommandType.DefenseSingle => AutoStatus.DefenseSingle,
            SpecialCommandType.EsunaStanceNorth => AutoStatus.Dispel
                                | AutoStatus.TankStance
                                | AutoStatus.Positional,
            SpecialCommandType.RaiseShirk => AutoStatus.Raise
                                | AutoStatus.Shirk,
            SpecialCommandType.MoveForward => AutoStatus.MoveForward,
            SpecialCommandType.MoveBack => AutoStatus.MoveBack,
            SpecialCommandType.AntiKnockback => AutoStatus.AntiKnockback,
            SpecialCommandType.Burst => AutoStatus.Burst,
            SpecialCommandType.Speed => AutoStatus.Speed,
            SpecialCommandType.LimitBreak => AutoStatus.LimitBreak,
            _ => AutoStatus.None,
        };

        AddStatus(ref status, AutoStatus.HealAreaSpell | AutoStatus.HealAreaAbility, DataCenter.RightSet.HealAreaConditionSet);
        AddStatus(ref status, AutoStatus.HealSingleSpell | AutoStatus.HealSingleAbility, DataCenter.RightSet.HealSingleConditionSet);
        AddStatus(ref status, AutoStatus.DefenseArea, DataCenter.RightSet.DefenseAreaConditionSet);
        AddStatus(ref status, AutoStatus.DefenseSingle, DataCenter.RightSet.DefenseSingleConditionSet);

        AddStatus(ref status, AutoStatus.Dispel | AutoStatus.TankStance | AutoStatus.Positional,
            DataCenter.RightSet.EsunaStanceNorthConditionSet);
        AddStatus(ref status, AutoStatus.Raise | AutoStatus.Shirk, DataCenter.RightSet.RaiseShirkConditionSet);
        AddStatus(ref status, AutoStatus.MoveForward, DataCenter.RightSet.MoveForwardConditionSet);
        AddStatus(ref status, AutoStatus.MoveBack, DataCenter.RightSet.MoveBackConditionSet);
        AddStatus(ref status, AutoStatus.AntiKnockback, DataCenter.RightSet.AntiKnockbackConditionSet);

        if (!status.HasFlag(AutoStatus.Burst) && Service.Config.GetValue(PluginConfigBool.AutoBurst))
        {
            status |= AutoStatus.Burst;
        }
        AddStatus(ref status, AutoStatus.Speed, DataCenter.RightSet.SpeedConditionSet);
        AddStatus(ref status, AutoStatus.LimitBreak, DataCenter.RightSet.LimitBreakConditionSet);

        return status;
    }

    private static void AddStatus(ref AutoStatus status, AutoStatus flag, ConditionSet set)
    {
        AddStatus(ref status, flag, () => set.IsTrue(DataCenter.RightNowRotation));
    }

    private static void AddStatus(ref AutoStatus status, AutoStatus flag, Func<bool> getValue)
    {
        if (status.HasFlag(flag) | !getValue()) return;

        status |= flag;
    }
}
