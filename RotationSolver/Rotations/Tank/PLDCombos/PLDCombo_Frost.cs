using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Tank.PLDCombos;

internal sealed class PLDCombo_Frost : PLDRotation_Base
{
    public override string GameVersion => "6.18";
    public override string RotationName => "Frost";
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.Description, "远离且不移动时会读圣灵；aoe时优先安魂圣环；战逃外弃打1赎罪（需求技速2.40-2.45）"},
    };
    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseDivineVeilPre", false, "倒计时15s使用幕帘")
                                            .SetBool("UseHolySpiritPre", true, "预读圣灵");
    }
    private protected override IAction CountDownAction(float remainTime)
    {
        if (Configs.GetBool("UseDivineVeilPre") && remainTime <= 15 && DivineVeil.ShouldUse(out _)) return DivineVeil;//提前15s幕帘
        if (Configs.GetBool("UseHolySpiritPre") && remainTime <= 2 && HolySpirit.ShouldUse(out _)) return HolySpirit;//提前2s圣灵
        return base.CountDownAction(remainTime);
    }/*
    //紧急使用的GCD
    private protected override bool EmergencyGCD(out IAction act)
    {
        if (Player.HasStatus(true, StatusID.Requiescat)
            && Player.WillStatusEndGCD(1, 0, true, StatusID.Requiescat))
        {
             if (Confiteor.ShouldUse(out act, mustUse: true))return true;//安魂快无了赶紧悔罪
        }
        /*if ((Target.HasStatus(true, StatusID.GoringBlade) && Target.WillStatusEndGCD(3, 0, true, StatusID.GoringBlade))//沥血快断了
            ||  (Target.HasStatus(true, StatusID.BladeofValor) && Target.WillStatusEndGCD(3, 0, true, StatusID.BladeofValor)))//英勇之剑快断了
        {
            if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;// 英勇之剑
            if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;// 真理之剑
            if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;// 信念之剑
            if (GoringBlade.ShouldUse(out act)) return true;// 沥血剑
            if (RiotBlade.ShouldUse(out act)) return true;// 暴乱剑
            if (FastBlade.ShouldUse(out act)) return true;// 先锋剑
        
        act= null;
        return false;
    }}*/


    //通常GCD
    private protected override bool GeneralGCD(out IAction act)
    {


        //大招
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;// 英勇之剑
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;// 真理之剑
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;// 信念之剑
        if (Confiteor.ShouldUse(out act, mustUse: true)
            && Player.StatusStack(true, StatusID.Requiescat) == 1) return true;//悔罪


        //AOE
        if (Player.HasStatus(true, StatusID.Requiescat)//有安魂buff
            && HolyCircle.ShouldUse(out act)) return true;// 圣环
        if (Prominence.ShouldUse(out act)) return true;// 日珥斩
        if (TotalEclipse.ShouldUse(out act)) return true;// 全蚀斩

        //单体
        if (!Player.HasStatus(true, StatusID.FightOrFlight)//没有战逃buff
            && Player.HasStatus(true, StatusID.Requiescat)//有安魂buff
            && HolySpirit.ShouldUse(out act)) return true;//圣灵
        if (Atonement.ShouldUse(out act) && IsLastGCD(true, Atonement, RageofHalone))//赎罪剑
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)) return true;//战逃内打完
            if (Player.StatusStack(true, StatusID.SwordOath) != 1 && !Player.HasStatus(true, StatusID.FightOrFlight)) return true;//战逃外丢一个赎罪
        }
        if (GoringBlade.ShouldUse(out act)) return true;// 沥血剑
        if (RageofHalone.ShouldUse(out act)) return true;// 战女神之怒(王权剑)
        if (RiotBlade.ShouldUse(out act)) return true;// 暴乱剑
        if (FastBlade.ShouldUse(out act)) return true;// 先锋剑

        //远程
        if (IsMoving//移动中
            && ShieldLob.ShouldUse(out act)) return true;//投盾
        if (HolySpirit.ShouldUse(out act)) return true;//圣灵

        return false;
    }

    //范围防御能力技
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;// 雪仇
        if (DivineVeil.ShouldUse(out act)) return true;//圣光幕帘
        if (PassageofArms.ShouldUse(out act)) return true;//武装戍卫
        return false;
    }

    //单体防御能力技
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (Rampart.ShouldUse(out act)) return true;//铁壁（减伤20%）
        if (Sentinel.ShouldUse(out act)) return true; //预警（减伤30%）
        if (Reprisal.ShouldUse(out act)) return true;//雪仇
        if (Sheltron.ShouldUse(out act)) return true;// 盾阵

        /*
        if (Intervention.ShouldUse(out act)) return true;//干预
        if (Cover.ShouldUse(out act)) return true;//保护
        */
        return false;
    }
    //紧急使用的能力
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsAnySameAction(true, RiotBlade) || nextGCD.IsAnySameAction(true, GoringBlade))//插在暴乱剑或沥血剑前
        {
            if (FightorFlight.ShouldUse(out act) && abilityRemain == 1) return true;//战逃
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }
    //攻击能力技
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (HolyCircle.ShouldUse(out _))//借用圣环的判断
        {
            if (Requiescat.ShouldUse(out act, mustUse: true)) return true;//aoe时优先安魂
        }

        if (CircleofScorn.ShouldUse(out act, mustUse: true))//厄运流转
        {
            if (FightorFlight.ElapsedAfterGCD(2)) return true; //开场延后
        }
        if (SpiritsWithin.ShouldUse(out act, mustUse: true))//深奥之灵(偿赎剑)
        {
            if (FightorFlight.ElapsedAfterGCD(2)) return true; //开场延后
        }
        if (FastBlade.ShouldUse(out _))//距离不远
                                       //&& !IsMoving) //不在移动中 
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)//有战逃buff
                && Intervene.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;//调停放空
            //if (Intervene.ShouldUse(out act)) return true;//调停
        }

        if (Requiescat.ShouldUse(out act))//安魂祈祷
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)//持有战逃buff
            && Player.WillStatusEnd(17, true, StatusID.FightOrFlight)) return true;//战逃buff时间小于17s
            if (!Player.HasStatus(true, StatusID.FightOrFlight)//没有战逃buff
            && !FightorFlight.WillHaveOneCharge(13)) return true;//战逃冷却在13s以上
        }
        if (OathGauge == 100 && Player.CurrentHp < Player.MaxHp)//忠义已满且不满血
        {
            if (HaveShield && Sheltron.ShouldUse(out act)) return true;//盾阵

        }
        return false;
    }
}
