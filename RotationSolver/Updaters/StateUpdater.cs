using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Condition;
using RotationSolver.Basic.Configuration.Timeline;
using System.Runtime.CompilerServices;

namespace RotationSolver.Updaters;
internal static class StateUpdater
{
    private static bool CanUseHealAction =>
        //PvP
        (DataCenter.IsPvP)
        //Job
        || (DataCenter.Role == JobRole.Healer || Service.Config.UseHealWhenNotAHealer)
        && Service.Config.AutoHeal
        && (DataCenter.InCombat && CustomRotation.IsLongerThan(Service.Config.AutoHealTimeToKill)
            || Service.Config.HealOutOfCombat);

    public static void UpdateState()
    {
        DataCenter.CommandStatus = StatusFromCmdOrCondition();
        DataCenter.AutoStatus = StatusFromAutomatic();
    }

    static RandomDelay 
        _healDelay1 = new(() => Service.Config.HealDelay),
        _healDelay2 = new(() => Service.Config.HealDelay),
        _healDelay3 = new(() => Service.Config.HealDelay),
        _healDelay4 = new(() => Service.Config.HealDelay);

    private static AutoStatus StatusFromAutomatic()
    {
        var hasTimeline = OtherConfiguration.TerritoryConfig.Timeline.Any(p => p.Value.Any(i => i is not DrawingTimeline));

        AutoStatus status = AutoStatus.None;

        if (DataCenter.DeathTarget is not null)
        {
            status |= AutoStatus.Raise;
        }

        if (DataCenter.Role is JobRole.Melee && ActionUpdater.NextGCDAction != null
            && Service.Config.AutoUseTrueNorth)
        {
            var id = ActionUpdater.NextGCDAction.ID;
            if (ConfigurationHelper.ActionPositional.TryGetValue((ActionID)id, out var positional)
                && positional != ActionUpdater.NextGCDAction.Target.Target?.FindEnemyPositional()
                && (ActionUpdater.NextGCDAction.Target.Target?.HasPositional() ?? false))
            {
                status |= AutoStatus.Positional;
            }
        }

        var noHeal = DataCenter.Role is JobRole.Healer && hasTimeline;
        if (DataCenter.HPNotFull && CanUseHealAction && !noHeal)
        {
            var singleAbility = ShouldHealSingle(StatusHelper.SingleHots,
                Service.Config.HealthSingleAbility,
                Service.Config.HealthSingleAbilityHot);

            var singleSpell = ShouldHealSingle(StatusHelper.SingleHots,
                Service.Config.HealthSingleSpell,
                Service.Config.HealthSingleSpellHot);

            var onlyHealSelf = Service.Config.OnlyHealSelfWhenNoHealer 
                && DataCenter.Role != JobRole.Healer;

            var canHealSingleAbility = onlyHealSelf ? ShouldHealSingle(Player.Object, StatusHelper.SingleHots,
                Service.Config.HealthSingleAbility, Service.Config.HealthSingleAbilityHot)
                : singleAbility > 0;

            var canHealSingleSpell = onlyHealSelf ? ShouldHealSingle(Player.Object, StatusHelper.SingleHots,         
                Service.Config.HealthSingleSpell, Service.Config.HealthSingleSpellHot)
                : singleSpell > 0;

            var canHealAreaAbility = singleAbility > 2;
            var canHealAreaSpell = singleSpell > 2;

            if (DataCenter.PartyMembers.Length > 2)
            {
                //TODO: Beneficial area status.
                var ratio = GetHealingOfTimeRatio(Player.Object, StatusHelper.AreaHots);

                if (!canHealAreaAbility)
                    canHealAreaAbility = DataCenter.PartyMembersDifferHP < Service.Config.HealthDifference && DataCenter.PartyMembersAverHP < Lerp(Service.Config.HealthAreaAbility, Service.Config.HealthAreaAbilityHot, ratio);

                if (!canHealAreaSpell)
                    canHealAreaSpell = DataCenter.PartyMembersDifferHP < Service.Config.HealthDifference && DataCenter.PartyMembersAverHP < Lerp(Service.Config.HealthAreaSpell, Service.Config.HealthAreaSpellHot, ratio);
            }

            if (_healDelay1.Delay(canHealAreaAbility))
            {
                status |= AutoStatus.HealAreaAbility;
            }
            if (_healDelay2.Delay(canHealAreaSpell))
            {
                status |= AutoStatus.HealAreaSpell;
            }
            if (_healDelay3.Delay(canHealSingleAbility))
            {
                status |= AutoStatus.HealSingleAbility;
            }
            if (_healDelay4.Delay(canHealSingleSpell))
            {
                status |= AutoStatus.HealSingleSpell;
            }
        }

        if (DataCenter.InCombat)
        {
            if (Service.Config.UseDefenseAbility)
            {
                if (DataCenter.IsHostileCastingAOE && !hasTimeline)
                {
                    status |= AutoStatus.DefenseArea;
                }

                if (DataCenter.IsHostileCastingKnockback && !hasTimeline && Service.Config.UseKnockback)
                {
                    status |= AutoStatus.AntiKnockback;
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

                    var tarOnMe = DataCenter.AllHostileTargets.Where(t => t.DistanceToPlayer() <= 3
                    && t.TargetObject == Player.Object);
                    var tarOnMeCount = tarOnMe.Count();
                    var attackedCount = tarOnMe.Count(ObjectHelper.IsAttacked);
                    var attacked = (float)attackedCount / tarOnMeCount > 0.7f;

                    //A lot targets are targeting on me.
                    if (tarOnMeCount >= Service.Config.AutoDefenseNumber
                        && Player.Object.GetHealthRatio() <= Service.Config.HealthForAutoDefense
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
                && (Service.Config.AutoProvokeForTank
                || DataCenter.AllianceMembers.Count(o => o.IsJobCategory(JobRole.Tank)) < 2)
                && DataCenter.ProvokeTarget != null)
            {
                status |= AutoStatus.Provoke;
            }
        }

        if (DataCenter.DispelTarget != null)
        {
            if (DataCenter.DispelTarget.StatusList.Any(StatusHelper.IsDangerous))
            {
                status |= AutoStatus.Dispel;
            }
            else if (!DataCenter.HasHostilesInRange || Service.Config.DispelAll
            || (DataCenter.IsPvP))
            {
                status |= AutoStatus.Dispel;
            }
        }

        if (DataCenter.InterruptTarget != null)
        {
            status |= AutoStatus.Interrupt;
        }

        if (Service.Config.AutoTankStance && DataCenter.Role == JobRole.Tank
            && !DataCenter.AllianceMembers.Any(t => t.IsJobCategory(JobRole.Tank) && t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.TankStanceStatus))
            && !CustomRotation.HasTankStance)
        {
            status |= AutoStatus.TankStance;
        }

        if (DataCenter.IsMoving && DataCenter.NotInCombatDelay && Service.Config.AutoSpeedOutOfCombat)
        {
            status |= AutoStatus.Speed;
        }

        return status;
    }
    static float GetHealingOfTimeRatio(BattleChara target, params StatusID[] statusIds)
    {
        const float buffWholeTime = 15;

        var buffTime = target.StatusTime(false, statusIds);

        return Math.Min(1, buffTime / buffWholeTime);
    }

    static int ShouldHealSingle(StatusID[] hotStatus, float healSingle, float healSingleHot) => DataCenter.PartyMembers.Count(p => ShouldHealSingle(p, hotStatus, healSingle, healSingleHot));

    static bool ShouldHealSingle(BattleChara target, StatusID[] hotStatus, float healSingle, float healSingleHot)
    {
        if (target == null) return false;

        var ratio = GetHealingOfTimeRatio(target, hotStatus);

        var h = target.GetHealthRatio();
        if (h == 0 || !target.NeedHealing()) return false;

        return h < Lerp(healSingle, healSingleHot, ratio);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Lerp(float a, float b, float ratio)
    {
        return a + (b - a) * ratio;
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
            SpecialCommandType.DispelStancePositional => AutoStatus.Dispel
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
            DataCenter.RightSet.DispelStancePositionalConditionSet);
        AddStatus(ref status, AutoStatus.Raise | AutoStatus.Shirk, DataCenter.RightSet.RaiseShirkConditionSet);
        AddStatus(ref status, AutoStatus.MoveForward, DataCenter.RightSet.MoveForwardConditionSet);
        AddStatus(ref status, AutoStatus.MoveBack, DataCenter.RightSet.MoveBackConditionSet);
        AddStatus(ref status, AutoStatus.AntiKnockback, DataCenter.RightSet.AntiKnockbackConditionSet);

        if (!status.HasFlag(AutoStatus.Burst) && Service.Config.AutoBurst)
        {
            status |= AutoStatus.Burst;
        }
        AddStatus(ref status, AutoStatus.Speed, DataCenter.RightSet.SpeedConditionSet);
        AddStatus(ref status, AutoStatus.LimitBreak, DataCenter.RightSet.LimitBreakConditionSet);

        return status;
    }

    private static void AddStatus(ref AutoStatus status, AutoStatus flag, ConditionSet set)
    {
        AddStatus(ref status, flag, () => set.IsTrue(DataCenter.RightNowRotation) ?? false);
    }

    private static void AddStatus(ref AutoStatus status, AutoStatus flag, Func<bool> getValue)
    {
        if (status.HasFlag(flag) | !getValue()) return;
        status |= flag;
    }
}
