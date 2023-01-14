using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Melee.NINCombos.WARCombo_Frost;

namespace XIVAutoAttack.Combos.Melee.NINCombos;

internal sealed class WARCombo_Frost : WARCombo_Base
{
    public override string GameVersion => "6.18";

    public override string Author => "Frost";

    /// <summary>
    /// 描述
    /// </summary>
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.Description, "可能还不如那个呢,调整了一些技能的优先级"},
    };

    /// <summary>
    /// 自定义选项
    /// </summary>
    /// <returns></returns>
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseThrillofBattlePre", false, "倒计时11s时使用战栗,关爱学者")
            .SetBool("UseShakeItOffPre", false, "倒计时2s时使用摆脱")
            .SetBool("UseTomahawkPre", false, "mt倒计时1s时飞斧开怪");
    }

    /// <summary>
    /// 倒计时时使用的技能
    /// </summary>
    /// <param name="remainTime"></param>
    /// <returns></returns>
    private protected override IAction CountDownAction(float remainTime)
    {
        if (Config.GetBoolByName("UseTomahawkPre") && remainTime <= 1 && Tomahawk.ShouldUse(out _)) return Tomahawk;//提前1s飞斧开怪
        if (Config.GetBoolByName("UseThrillofBattlePre") && remainTime <= 11 && ThrillofBattle.ShouldUse(out _)) return ThrillofBattle;//提前11s战栗
        if (Config.GetBoolByName("UseShakeItOffPre") && remainTime <= 1 && ShakeItOff.ShouldUse(out _)) return ShakeItOff;//提前2s摆脱
        return base.CountDownAction(remainTime);
    }

    /// <summary>
    /// 通常GCD
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected override bool GeneralGCD(out IAction act)
    {
        //大招
        if (SteelCyclone.ShouldUse(out act)) return true;//钢铁旋风
        if (InnerBeast.ShouldUse(out act)) return true;//原初之魂
        if (PrimalRend.ShouldUse(out act, mustUse: true) && !IsMoving)//蛮荒崩裂
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }


        //aoe
        if (MythrilTempest.ShouldUse(out act)) return true;//秘银暴风
        if (Overpower.ShouldUse(out act)) return true;//超压斧

        //单体
        if (StormsEye.ShouldUse(out act)) return true; //暴风碎 红斩
        if (StormsPath.ShouldUse(out act)) return true;// 暴风斩 绿斩
        if (Maim.ShouldUse(out act)) return true;// 凶残裂
        if (HeavySwing.ShouldUse(out act)) return true;// 重劈

        //远程
        if (Tomahawk.ShouldUse(out act)) return true;//飞斧
        return false;
    }

    /// <summary>
    /// 单体防御能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            if (TargetUpdater.HostileTargets.Count() > 1)
            {
                //原初的直觉（减伤10%）
                if (RawIntuition.ShouldUse(out act)) return true;
            }


            //复仇（减伤30%）
            if (Vengeance.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.ShouldUse(out act)) return true;


            //原初的直觉（减伤10%）
            if (RawIntuition.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// 范围防御能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShakeItOff.ShouldUse(out act, mustUse: true)) return true;//摆脱
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;//雪仇

        return false;
    }

    /// <summary>
    /// 攻击能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {

        if (Player.HasStatus(true, StatusID.SurgingTempest))
        {
            //狂暴
            if (Berserk.ShouldUse(out act)) return true;
        }

        //战嚎
        if (Infuriate.ShouldUse(out act, emptyOrSkipCombo: true) && Player.HasStatus(true, StatusID.SurgingTempest)) return true;


        //群山隆起
        if (Orogeny.ShouldUse(out act)) return true;
        //动乱 
        if (Upheaval.ShouldUse(out act)) return true;

        //猛攻
        if (Onslaught.ShouldUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 单体治疗能力技
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //原初的勇猛
        if (NascentFlash.ShouldUse(out act)) return true;//奶奶队友
        if (Player.GetHealthRatio() < 0.4f//命不久矣
            && !Player.HasStatus(true, StatusID.Holmgang))//不在无敌中
        {
            //战栗
            if (ThrillofBattle.ShouldUse(out act)) return true;
            //泰然自若
            if (Equilibrium.ShouldUse(out act)) return true;
            //原初的直觉
            if (RawIntuition.ShouldUse(out act)) return true;
        }
        return false;
    }
}
