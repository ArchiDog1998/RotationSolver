using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;

namespace RotationSolver.Combos.CustomCombo;

internal abstract partial class CustomRotation
{
    private IAction GCD(byte abilityRemain, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        IAction act = RSCommands.NextAction;
        if (act is BaseAction a && a != null && a.IsRealGCD && a.ShouldUse(out _, mustUse: true, skipDisable: true)) return act;

        if (EmergencyGCD(out act)) return act;

        var specialType = RSCommands.SpecialType;

        if (EsunaRaise(out act, abilityRemain, false)) return act;
        if (specialType == SpecialCommandType.MoveForward && MoveGCD(out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return act;
        }
        if (TargetUpdater.HPNotFull && ActionUpdater.InCombat)
        {
            if ((specialType == SpecialCommandType.HealArea || CanHealAreaSpell) && HealAreaGCD(out act)) return act;
            if ((specialType == SpecialCommandType.HealSingle || CanHealSingleSpell) && HealSingleGCD(out act)) return act;
        }
        if (specialType == SpecialCommandType.DefenseArea && DefenseAreaGCD(out act)) return act;
        if (specialType == SpecialCommandType.DefenseSingle && DefenseSingleGCD(out act)) return act;

        //自动防御
        if (helpDefenseAOE && DefenseAreaGCD(out act)) return act;
        if (helpDefenseSingle && DefenseSingleGCD(out act)) return act;

        if (GeneralGCD(out var action)) return action;

        //硬拉或者开始奶人
        if (Service.Configuration.RaisePlayerBySwift && (HaveSwift || !Swiftcast.IsCoolDown)
            && EsunaRaise(out act, abilityRemain, true)) return act;
        if (TargetUpdater.HPNotFull && HaveHostilesInRange && ActionUpdater.InCombat)
        {
            if (CanHealAreaSpell && HealAreaGCD(out act)) return act;
            if (CanHealSingleSpell && HealSingleGCD(out act)) return act;
        }
        if (Service.Configuration.RaisePlayerByCasting && EsunaRaise(out act, abilityRemain, true)) return act;

        return null;
    }

    private bool EsunaRaise(out IAction act, byte actabilityRemain, bool mustUse)
    {
        act = null;

        if (Raise == null) return false;

        var specialType = RSCommands.SpecialType;

        //有某些非常危险的状态。
        if (specialType == SpecialCommandType.EsunaShieldNorth && TargetUpdater.WeakenPeople.Any() || TargetUpdater.DyingPeople.Any())
        {
            if (Job.GetJobRole() == JobRole.Healer && Esuna.ShouldUse(out act, mustUse: true)) return true;
        }

        //蓝量不足，就不复活了。
        if (Player.CurrentMp <= Service.Configuration.LessMPNoRaise) return false;

        //有人死了，看看能不能救。
        if (Service.Configuration.RaiseAll ? TargetUpdater.DeathPeopleAll.Any() : TargetUpdater.DeathPeopleParty.Any())
        {
            if (Service.ClientState.LocalPlayer.ClassJob.Id == 35)
            {
                if (HaveSwift && Raise.ShouldUse(out act)) return true;
            }
            else if (specialType == SpecialCommandType.RaiseShirk || HaveSwift || !Swiftcast.IsCoolDown && actabilityRemain > 0 || mustUse)
            {
                if (Raise.ShouldUse(out _))
                {
                    if (mustUse && Swiftcast.ShouldUse(out act)) return true;
                    act = Raise;
                    return true;
                }
            }
        }
        act = null;
        return false;
    }

    /// <summary>
    /// 在倒计时的时候返回这个函数里面的技能。
    /// </summary>
    /// <param name="remainTime">距离战斗开始的时间(s)</param>
    /// <returns>要使用的技能</returns>
    private protected virtual IAction CountDownAction(float remainTime) => null;

    /// <summary>
    /// 一些非常紧急的GCD战技，优先级最高
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergencyGCD(out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// 常规GCD技能
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool GeneralGCD(out IAction act);

    /// <summary>
    /// 移动GCD技能
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool MoveGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 单体治疗GCD
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 范围治疗GCD
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaGCD(out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 单体防御GCD
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenseSingleGCD(out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// 范围防御GCD
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenseAreaGCD(out IAction act)
    {
        act = null; return false;
    }
}
