namespace RotationSolver.Default.Ranged;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Ranged/DNC_Default.cs")]
internal sealed class DNC_Default : DNC_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 15)
        {
            if (StandardStep.CanUse(out var act, mustUse: true)) return act;
            if (ExcutionStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        //跳舞状态禁止使用
        if (IsDancing) return false;

        //进攻之探戈
        if (Devilment.CanUse(out act))
        {
            if (InBurst && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        //应急换舞伴
        if (UseClosedPosition(out act)) return true;

        //百花
        if (Flourish.CanUse(out act)) return true;

        //扇舞·急
        if (FanDance3.CanUse(out act, mustUse: true)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            //扇舞·破
            if (FanDance2.CanUse(out act)) return true;
            //扇舞·序
            if (FanDance.CanUse(out act)) return true;
        }

        //扇舞·终
        if (FanDance4.CanUse(out act, mustUse: true))
        {
            if (TechnicalStep.EnoughLevel && TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneChargeGCD()) return false;
            return true;
        }

        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //绑定舞伴
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1) && ClosedPosition.CanUse(out act)) return true;

        //结束舞步
        if (FinishStepGCD(out act)) return true;

        //执行舞步
        if (ExcutionStepGCD(out act)) return true;

        //技巧舞步
        if (InBurst && InCombat && TechnicalStep.CanUse(out act, mustUse: true)) return true;

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
    bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        //跳舞状态禁止使用
        if (IsDancing) return false;

        //剑舞
        if ((breaking || Esprit >= 85) && SaberDance.CanUse(out act, mustUse: true)) return true;

        //提拉纳
        if (Tillana.CanUse(out act, mustUse: true)) return true;

        //流星舞
        if (StarfallDance.CanUse(out act, mustUse: true)) return true;

        //使用标准舞步
        if (UseStandardStep(out act)) return true;

        //触发AOE
        if (Bloodshower.CanUse(out act)) return true;
        if (Fountainfall.CanUse(out act)) return true;
        //触发单体
        if (RisingWindmill.CanUse(out act)) return true;
        if (ReverseCascade.CanUse(out act)) return true;

        //基础AOE
        if (Bladeshower.CanUse(out act)) return true;
        if (Windmill.CanUse(out act)) return true;
        //基础单体
        if (Fountain.CanUse(out act)) return true;
        if (Cascade.CanUse(out act)) return true;

        return false;
    }

    /// <summary>
    /// 使用标准舞步
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.CanUse(out act, mustUse: true)) return false;
        if (Player.WillStatusEndGCD(2, 0, true, StatusID.StandardFinish)) return true;

        //等级低于玩家太多不跳舞,都直接秒了还跳啥舞
        if (Level - Target.Level > 10) return false;

        //周围没有敌人不跳舞
        if (!HasHostilesInRange) return false;

        //技巧舞步状态和快冷却好时不释放
        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneCharge(5))) return false;

        return true;
    }

    /// <summary>
    /// 应急换舞伴
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool UseClosedPosition(out IAction act)
    {
        if (!ClosedPosition.CanUse(out act)) return false;

        //应急换舞伴
        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition1))
        {
            foreach (var friend in PartyMembers)
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

    private static bool FinishStepGCD(out IAction act)
    {
        act = null;
        if (!IsDancing) return false;

        //标准舞步结束
        if (Player.HasStatus(true, StatusID.StandardStep) && (Player.WillStatusEnd(1, true, StatusID.StandardStep) || CompletedSteps == 2 && Player.WillStatusEnd(1, true, StatusID.StandardFinish))
            || StandardFinish.CanUse(out _, mustUse: true))
        {
            act = StandardStep;
            return true;
        }

        //技巧舞步结束
        if (Player.HasStatus(true, StatusID.TechnicalStep) && Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || TechnicalFinish.CanUse(out _, mustUse: true))
        {
            act = TechnicalStep;
            return true;
        }

        return false;
    }
}
