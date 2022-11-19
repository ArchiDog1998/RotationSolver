using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedPhysicial.DNCCombos.DNCCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.DNCCombos;

internal sealed class DNCCombo_Default : DNCCombo_Base<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetFloat("Technical_over", 3, "大舞冷却好之前几个GCD开始无视过量攒幻扇,伶俐", min: 0, max: 5, speed: 0.02f);
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围防御, $"{ShieldSamba}"},
        {DescType.范围治疗, $"{CuringWaltz}, {Improvisation}"},
        {DescType.移动技能, $"{EnAvant}"},
    };

    public override string Author => "秋水";

    //大舞窗口
    private static bool _TechnicalFinish => Player.HasStatus(true, StatusID.TechnicalFinish);
    private float Technical_over => Config.GetFloatByName("Technical_over");


    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //应急换舞伴 有舞伴技能先不换
        if (Player.HasStatus(true, StatusID.ClosedPosition1) &&
            !Player.HasStatus(true, StatusID.StandardFinish) &&
            !Player.HasStatus(true, StatusID.Devilment))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.ShouldUse(out act) && ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (ClosedPosition.ShouldUse(out act)) return true;

        //尝试大舞后绑定探戈
        if (Player.HasStatus(true, StatusID.TechnicalFinish)
        && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //扇舞·急
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //百花
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;


        //扇舞 大舞可以满一点
        if (!TechnicalStep.WillHaveOneChargeGCD((uint)Technical_over) && (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel))
        {
            if (FanDance2.ShouldUse(out act)) return true;
            if (FanDance.ShouldUse(out act)) return true;
        }
        if (FanDance4.ShouldUse(out act, mustUse: true)) return true; //时间长可以延后 优先处理幻扇溢出

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
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

    private protected override bool GeneralGCD(out IAction act)
    {
        //绑舞伴
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1)
            && ClosedPosition.ShouldUse(out act)) return true;
        //跳舞
        if (FinishStepGCD(out act)) return true;

        //开始跳舞
        if (StandardStep.ShouldUse(out act, mustUse: true)) return true;
        if (SettingBreak && TechnicalStep.ShouldUse(out act, mustUse: true)) return true;
        #region 大舞爆发
        if (SettingBreak && Player.HasStatus(true, StatusID.TechnicalFinish))
        {
            //提纳拉
            if (Tillana.ShouldUse(out act, mustUse: true)) return true;
            //剑舞
            if (SaberDance.ShouldUse(out act, mustUse: true)) return true;
            //流星舞
            if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;
        }
        #endregion
        #region 普通攒资源
        #region 剑舞逻辑
        if (!TechnicalStep.WillHaveOneChargeGCD((uint)Technical_over) && Esprit >= 85 && SaberDance.ShouldUse(out act, mustUse: true)) return true;
        #endregion


        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;

        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;

        //aoe
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;
        //single
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;
        #endregion

        return false;
    }

    //开局15s使用小舞
    private protected override IAction CountDownAction(float remainTime)
    {
        IAction act = null;
        if (remainTime <= 15)
        {
            if (StandardStep.ShouldUse(out _)) return StandardStep;
            if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps < 2)
            {
                if (excutionStepGCD(out act)) return act;
            }
        }
        return base.CountDownAction(remainTime);
    }
}
