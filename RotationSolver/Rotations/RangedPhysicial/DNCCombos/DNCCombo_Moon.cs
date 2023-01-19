using RotationSolver.Actions;
using RotationSolver.Combos.Basic;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Combos.RangedPhysicial.DNCCombos;

internal sealed class DNCCombo_Moon : DNCRotation_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "汐ベMoon";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseArea, $"{ShieldSamba}"},
        {DescType.HealArea, $"{CuringWaltz}, {Improvisation}"},
        {DescType.MoveAction, $"{EnAvant}"},
    };

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 15)
        {
            if (StandardStep.ShouldUse(out _, mustUse: true)) return StandardStep;
            IAction act;
            if (ExcutionStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        //跳舞状态禁止使用
        if (IsDancing) return false;

        //进攻之探戈
        if (Devilment.ShouldUse(out act))
        {
            if (SettingBreak && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        //应急换舞伴
        if (UseClosedPosition(out act)) return true;

        //百花
        if (Flourish.ShouldUse(out act)) return true;

        //扇舞·急
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            //扇舞·破
            if (FanDance2.ShouldUse(out act)) return true;
            //扇舞·序
            if (FanDance.ShouldUse(out act)) return true;
        }

        //扇舞·终
        if (FanDance4.ShouldUse(out act, mustUse: true))
        {
            if (TechnicalStep.EnoughLevel && TechnicalStep.IsCoolDown && TechnicalStep.WillHaveOneChargeGCD()) return false;
            return true;
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //绑定舞伴
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1) && ClosedPosition.ShouldUse(out act)) return true;

        //结束舞步
        if (FinishStepGCD(out act)) return true;

        //执行舞步
        if (ExcutionStepGCD(out act)) return true;

        //技巧舞步
        if (SettingBreak && InCombat && TechnicalStep.ShouldUse(out act, mustUse: true)) return true;

        //攻击GCD
        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return false;
    }

    /// <summary>
    /// 攻击GCD
    /// </summary>
    /// <param name="act"></param>
    /// <param name="breaking"></param>
    /// <returns></returns>
    private bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        //跳舞状态禁止使用
        if (IsDancing) return false;

        //剑舞
        if ((breaking || Esprit >= 85) && SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //提拉纳
        if (Tillana.ShouldUse(out act, mustUse: true)) return true;

        //流星舞
        if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        //使用标准舞步
        if (UseStandardStep(out act)) return true;

        //触发AOE
        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;
        //触发单体
        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;

        //基础AOE
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;
        //基础单体
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;

        return false;
    }

    /// <summary>
    /// 使用标准舞步
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.ShouldUse(out act, mustUse: true)) return false;

        //等级低于玩家太多不跳舞,都直接秒了还跳啥舞
        if (Level - Target.Level > 10) return false;

        //周围没有敌人不跳舞
        if (!HaveHostilesInRange) return false;

        //技巧舞步状态和快冷却好时不释放
        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolDown && TechnicalStep.WillHaveOneCharge(5))) return false;

        return true;
    }

    /// <summary>
    /// 结束舞步
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseFinishStepGCD(out IAction act)
    {
        if (!FinishStepGCD(out act)) return false;

        if (Target.IsBoss()) return true;

        if (Windmill.ShouldUse(out _)) return true;

        if (TargetUpdater.HostileTargets.GetObjectInRadius(25).Count() >= 3) return false;

        return false;
    }

    /// <summary>
    /// 应急换舞伴
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseClosedPosition(out IAction act)
    {
        if (!ClosedPosition.ShouldUse(out act)) return false;

        //应急换舞伴
        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.Target != friend) return true;
                    break;
                }
            }
        }
        //else if (ClosedPosition.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}
