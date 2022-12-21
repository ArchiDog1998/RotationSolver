using System;
using System.Data;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.CustomCombo;

internal abstract partial class CustomCombo<TCmd> where TCmd : Enum
{
    private bool Ability(byte abilityRemain, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        act = CommandController.NextAction;
        if (act is BaseAction a && a != null && !a.IsRealGCD && a.ShouldUse(out _, mustUse: true, skipDisable: true)) return true;

        if (!Service.Configuration.UseAbility || Player.TotalCastTime - Player.CurrentCastTime > Service.Configuration.WeaponInterval)
        {
            act = null;
            return false;
        }

        if (EmergencyAbility(abilityRemain, nextGCD, out act)) return true;
        var role = Job.GetJobRole();

        if (TargetUpdater.CanInterruptTargets.Any())
        {
            switch (role)
            {
                case JobRole.Tank:
                    if (Interject.ShouldUse(out act)) return true;
                    break;

                case JobRole.Melee:
                    if (LegSweep.ShouldUse(out act)) return true;
                    break;

                case JobRole.RangedPhysical:
                    if (HeadGraze.ShouldUse(out act)) return true;
                    break;
            }
        }
        
        if (role == JobRole.Tank)
        {
            if (CommandController.RaiseOrShirk)
            {
                if (Shirk.ShouldUse(out act)) return true;
            }

            if (CommandController.EsunaOrShield && Shield.ShouldUse(out act)) return true;

            if (Service.Configuration.AutoShield)
            {
                //Alive Tanks with shield.
                var defensesTanks = TargetUpdater.AllianceTanks.Where(t => t.CurrentHp != 0 && t.HasStatus(false, StatusHelper.SheildStatus));
                if (defensesTanks == null || defensesTanks.Count() == 0)
                {
                    if (!HaveShield && Shield.ShouldUse(out act)) return true;
                }
            }
        }

        if (CommandController.AntiRepulsion)
        {
            switch (role)
            {
                case JobRole.Tank:
                case JobRole.Melee:
                    if (ArmsLength.ShouldUse(out act)) return true;
                    break;
                case JobRole.Healer:
                    if (Surecast.ShouldUse(out act)) return true;
                    break;
                case JobRole.RangedPhysical:
                    if (ArmsLength.ShouldUse(out act)) return true;
                    break;
                case JobRole.RangedMagicial:
                    if (Surecast.ShouldUse(out act)) return true;
                    break;
            }
        }
        if (CommandController.EsunaOrShield && role == JobRole.Melee)
        {
            if (TrueNorth.ShouldUse(out act)) return true;
        }

        if (CommandController.DefenseArea && DefenceAreaAbility(abilityRemain, out act)) return true;
        if (CommandController.DefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        if (TargetUpdater.HPNotFull || Service.ClientState.LocalPlayer.ClassJob.Id == 25)
        {
            if (ShouldUseHealAreaAbility(abilityRemain, out act)) return true;
            if (ShouldUseHealSingleAbility(abilityRemain, out act)) return true;
        }

        //防御
        if (HaveHostilesInRange)
        {
            //防AOE
            if (helpDefenseAOE && Service.Configuration.UseDefenceAbility)
            {
                if (DefenceAreaAbility(abilityRemain, out act)) return true;
                if (role is JobRole.Melee or JobRole.RangedPhysical or JobRole.RangedMagicial)
                {
                    //防卫
                    if (DefenceSingleAbility(abilityRemain, out act)) return true;
                }
            }

            //防单体
            if (role == JobRole.Tank)
            {
                var haveTargets = TargetFilter.ProvokeTarget(TargetUpdater.HostileTargets);
                if ((Service.Configuration.AutoProvokeForTank || TargetUpdater.AllianceTanks.Count() < 2)
                    && haveTargets.Count() != TargetUpdater.HostileTargets.Count())

                {
                    //开盾挑衅
                    if (!HaveShield && Shield.ShouldUse(out act)) return true;
                    if (Provoke.ShouldUse(out act, mustUse: true)) return true;
                }

                if (HaveShield && Service.Configuration.UseDefenceAbility)
                {
                    var tarOnmeCount = TargetUpdater.TarOnMeTargets.Count();

                    //被群殴呢
                    if (tarOnmeCount > 1 && !IsMoving)
                    {
                        if (ArmsLength.ShouldUse(out act)) return true;
                        if (DefenceSingleAbility(abilityRemain, out act)) return true;
                    }

                    //就一个打我，需要正在对我搞事情。
                    if (tarOnmeCount == 1)
                    {
                        var tar = TargetUpdater.TarOnMeTargets.First();
                        if (TargetUpdater.IsHostileTank)
                        {
                            //防卫
                            if (DefenceSingleAbility(abilityRemain, out act)) return true;
                        }
                    }
                }
            }

            //辅助防卫
            if (helpDefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        }

        if (CommandController.Move && MoveAbility(abilityRemain, out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return true;
        }


        //恢复/下踢
        switch (role)
        {
            case JobRole.Tank:
                if (LowBlow.ShouldUse(out act)) return true;
                break;

            case JobRole.Melee:
                if (SecondWind.ShouldUse(out act)) return true;
                if (Bloodbath.ShouldUse(out act)) return true;
                break;

            case JobRole.Healer:
            case JobRole.RangedMagicial:
                if (JobIDs[0] == ClassJobID.BlackMage) break;
                if (LucidDreaming.ShouldUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (SecondWind.ShouldUse(out act)) return true;
                break;
        }

        if (!InCombat && IsMoving && role == JobRole.RangedPhysical
            && !Service.ClientState.LocalPlayer.HasStatus(false, StatusID.Peloton)
            && Peloton.ShouldUse(out act, mustUse: true)) return true;

        if (GeneralAbility(abilityRemain, out act)) return true;
        if (HaveHostilesInRange && AttackAbility(abilityRemain, out act)) return true;
        return false;
    }

    private bool ShouldUseHealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (CommandController.HealArea || CanHealAreaAbility) && ActionUpdater.InCombat && HealAreaAbility(abilityRemain, out act);
    }

    private bool ShouldUseHealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (CommandController.HealSingle || CanHealSingleAbility) && ActionUpdater.InCombat && HealSingleAbility(abilityRemain, out act);
    }

    /// <summary>
    /// 覆盖写一些用于攻击的能力技，只有附近有敌人的时候才会有效。
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool AttackAbility(byte abilityRemain, out IAction act);
    /// <summary>
    /// 覆盖写一些用于因为后面的GCD技能而要适应的能力技能
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (Job.GetJobRole() is JobRole.Healer or JobRole.RangedMagicial &&
            action.CastTime >= 5 && Swiftcast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Service.Configuration.AutoUseTrueNorth && abilityRemain == 1 && action.EnermyLocation != EnemyLocation.None && action.Target != null)
            {
                if (action.EnermyLocation != action.Target.FindEnemyLocation() && action.Target.HasLocationSide())
                {
                    if (TrueNorth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
        }

        act = null;
        return false;
    }

    /// <summary>
    /// 常规的能力技，啥时候都能使用。
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 移动能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool MoveAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 单体治疗的能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 单体防御能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// 范围防御能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// 范围治疗的能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
}
