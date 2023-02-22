using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Tank.PLD;

internal sealed class PLD_Default : PLD_Base
{
    public override string GameVersion => "6.18";
    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseDivineVeilPre", false, "DivineVeilPre in 15 seconds.")
                                            .SetBool("UseHolySpiritPre", true, "use HolySpirit in 2 seconds");
    }
    private protected override IAction CountDownAction(float remainTime)
    {
        if (Configs.GetBool("UseHolySpiritPre") && remainTime <= HolySpirit.CastTime + Service.Configuration.CountDownAhead
            && HolySpirit.CanUse(out _)) return HolySpirit;//提前2s圣灵

        if (Configs.GetBool("UseDivineVeilPre") && remainTime <= 15 && DivineVeil.CanUse(out _)) return DivineVeil;//提前15s幕帘

        return base.CountDownAction(remainTime);
    }
    

    //通常GCD
    private protected override bool GeneralGCD(out IAction act)
    {
        //大招
        if (BladeofValor.CanUse(out act, mustUse: true)) return true;// 英勇之剑
        if (BladeofTruth.CanUse(out act, mustUse: true)) return true;// 真理之剑
        if (BladeofFaith.CanUse(out act, mustUse: true)) return true;// 信念之剑
        if (Confiteor.CanUse(out act, mustUse: true)
            && Player.StatusStack(true, StatusID.Requiescat) == 1) return true;//悔罪


        //AOE
        if (Player.HasStatus(true, StatusID.Requiescat)//有安魂buff
            && HolyCircle.CanUse(out act)) return true;// 圣环
        if (Prominence.CanUse(out act)) return true;// 日珥斩
        if (TotalEclipse.CanUse(out act)) return true;// 全蚀斩

        //单体
        if (!Player.HasStatus(true, StatusID.FightOrFlight)//没有战逃buff
            && Player.HasStatus(true, StatusID.Requiescat)//有安魂buff
            && HolySpirit.CanUse(out act)) return true;//圣灵
        if (Atonement.CanUse(out act) && IsLastGCD(true, Atonement, RageofHalone))//赎罪剑
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)) return true;//战逃内打完
            if (Player.StatusStack(true, StatusID.SwordOath) != 1 && !Player.HasStatus(true, StatusID.FightOrFlight)) return true;//战逃外丢一个赎罪
        }
        if (GoringBlade.CanUse(out act)) return true;// 沥血剑
        if (RageofHalone.CanUse(out act)) return true;// 战女神之怒(王权剑)
        if (RiotBlade.CanUse(out act)) return true;// 暴乱剑
        if (FastBlade.CanUse(out act)) return true;// 先锋剑

        //远程
        if (IsMoving//移动中
            && ShieldLob.CanUse(out act)) return true;//投盾
        if (HolySpirit.CanUse(out act)) return true;//圣灵

        return false;
    }

    [RotationDesc(ActionID.Reprisal, ActionID.DivineVeil, ActionID.PassageofArms)]
    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Reprisal.CanUse(out act, mustUse: true)) return true;// 雪仇
        if (DivineVeil.CanUse(out act)) return true;//圣光幕帘
        if (PassageofArms.CanUse(out act)) return true;//武装戍卫
        return false;
    }

    [RotationDesc(ActionID.Rampart, ActionID.Sentinel, ActionID.Reprisal, ActionID.Sheltron)]
    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Rampart.CanUse(out act)) return true;//铁壁（减伤20%）
        if (Sentinel.CanUse(out act)) return true; //预警（减伤30%）
        if (Reprisal.CanUse(out act)) return true;//雪仇
        if (Sheltron.CanUse(out act)) return true;// 盾阵
        
        //if (Intervention.ShouldUse(out act)) return true;//干预
        //if (Cover.ShouldUse(out act)) return true;//保护
        
        return false;
    }
    //紧急使用的能力
    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsTheSameTo(true, RiotBlade, GoringBlade))//插在暴乱剑或沥血剑前
        {
            if (FightorFlight.CanUse(out act) && abilitiesRemaining == 1) return true;//战逃
        }
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }
    //攻击能力技
    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (HolyCircle.CanUse(out _))//借用圣环的判断
        {
            if (Requiescat.CanUse(out act, mustUse: true)) return true;//aoe时优先安魂
        }

        if (CircleofScorn.CanUse(out act, mustUse: true))//厄运流转
        {
            if (FightorFlight.ElapsedAfterGCD(2)) return true; //开场延后
        }
        if (SpiritsWithin.CanUse(out act, mustUse: true))//深奥之灵(偿赎剑)
        {
            if (FightorFlight.ElapsedAfterGCD(2)) return true; //开场延后
        }
        if (FastBlade.CanUse(out _))//距离不远
                                    //&& !IsMoving) //不在移动中 
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)//有战逃buff
                && Intervene.CanUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;//调停放空
            //if (Intervene.ShouldUse(out act)) return true;//调停
        }

        if (Requiescat.CanUse(out act))//安魂祈祷
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)//持有战逃buff
            && Player.WillStatusEnd(17, true, StatusID.FightOrFlight)) return true;//战逃buff时间小于17s
            if (!Player.HasStatus(true, StatusID.FightOrFlight)//没有战逃buff
            && !FightorFlight.WillHaveOneCharge(13)) return true;//战逃冷却在13s以上
        }
        if (OathGauge == 100 && Player.CurrentHp < Player.MaxHp)//忠义已满且不满血
        {
            if (HasTankStance && Sheltron.CanUse(out act)) return true;//盾阵

        }
        return false;
    }
}
