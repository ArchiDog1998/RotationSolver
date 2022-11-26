using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedMagicial.RDMCombos.RDMCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.RDMCombos;

internal sealed class RDMCombo_Default : RDMCombo_Base<CommandType>
{
    public override string Author => "秋水";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体治疗, $"{Vercure}"},
        {DescType.范围防御, $"{MagickBarrier}"},
        {DescType.移动技能, $"{CorpsAcorps}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "使用赤治疗获得即刻")
            .SetBool("UseCorpsAcorps", false, "近战范围内使用短兵相接");
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        act = null;
        //鼓励要放到魔回刺或者魔Z斩或魔划圆斩之后
        if (nextGCD.IsAnySameAction(true, Zwerchhau, Redoublement, Moulinet))
        {
            if (Service.Configuration.AutoBreak && Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //开场爆发的时候释放。
        if (Service.Configuration.AutoBreak && GetRightValue(WhiteMana) && GetRightValue(BlackMana))
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //倍增要放到魔连攻击之后
        if (ManaStacks == 3 || Level < 68 && !nextGCD.IsAnySameAction(true, Zwerchhau, Riposte))
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (SettingBreak)
        {
            if (!canUseMagic(act) && Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }

        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50) && !Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //促进满了且预备buff没满就用。 
            if (abilityRemain == 2 && Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;

            //即刻咏唱
            if (!Player.HasStatus(true, StatusID.Acceleration)
                && Swiftcast.ShouldUse(out act, mustUse: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;
        }

        //攻击四个能力技。
        if (ContreSixte.ShouldUse(out act, mustUse: true)) return true;
        if (Fleche.ShouldUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        if (CorpsAcorps.ShouldUse(out act) && !IsMoving)
        {
            if (CorpsAcorps.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        if (Config.GetBoolByName("UseCorpsAcorps"))
        {
            if (Target.DistanceToPlayer() < 3)
            {
                if (CorpsAcorps.ShouldUse(out act)) return true;
            }
        }


        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (ManaStacks == 3) return false;

        #region 常规输出
        if (!Verthunder2.ShouldUse(out _))
        {
            if (Verfire.ShouldUse(out act)) return true;
            if (Verstone.ShouldUse(out act)) return true;
        }

        //试试看散碎
        if (Scatter.ShouldUse(out act)) return true;
        //平衡魔元
        if (WhiteMana < BlackMana)
        {
            if (Veraero2.ShouldUse(out act)) return true;
            if (Veraero.ShouldUse(out act)) return true;
        }
        else
        {
            if (Verthunder2.ShouldUse(out act)) return true;
            if (Verthunder.ShouldUse(out act)) return true;
        }
        if (Jolt.ShouldUse(out act)) return true;
        #endregion

        //震荡刷火炎和飞石


        //赤治疗，加即刻，有连续咏唱或者即刻的话就不放了
        if (Config.GetBoolByName("UseVercure") && Vercure.ShouldUse(out act)
            ) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (CorpsAcorps.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //混乱
        if (Addle.ShouldUse(out act)) return true;
        if (MagickBarrier.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergencyGCD(out IAction act)
    {
        byte level = Level;
        #region 远程三连
        //如果魔元结晶满了。
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana && level >= 70)
            {
                if (Verholy.ShouldUse(out act, mustUse: true)) return true;
            }
            if (Verflare.ShouldUse(out act, mustUse: true)) return true;
        }

        //焦热
        if (Scorch.ShouldUse(out act, mustUse: true)) return true;

        //决断
        if (Resolution.ShouldUse(out act, mustUse: true)) return true;
        #endregion

        #region 近战三连


        if (Moulinet.ShouldUse(out act)) return true;
        if (Zwerchhau.ShouldUse(out act)) return true;
        if (Redoublement.ShouldUse(out act)) return true;

        //如果倍增好了，或者魔元满了，或者正在爆发，或者处于开场爆发状态，就马上用！
        bool mustStart = Player.HasStatus(true, StatusID.Manafication) ||
                         BlackMana == 100 || WhiteMana == 100 || !Embolden.IsCoolDown;

        //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。
        if (!mustStart)
        {
            if (BlackMana == WhiteMana) return false;

            //要求较小的魔元不带触发，也可以强制要求跳过判断。
            if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady))
                {
                    return false;
                }
            }
            if (WhiteMana > BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerfireReady))
                {
                    return false;
                }
            }

            //看看有没有即刻相关的技能。
            if (Player.HasStatus(true, Vercure.BuffsProvide))
            {
                return false;
            }

            //如果倍增的时间快到了，但还是没好。
            if (Embolden.WillHaveOneChargeGCD(10))
            {
                return false;
            }
        }
        #endregion

        #region 开启爆发
        //要来可以使用近战三连了。
        if (Moulinet.ShouldUse(out act))
        {
            if (BlackMana >= 60 && WhiteMana >= 60) return true;
        }
        else
        {
            if (BlackMana >= 50 && WhiteMana >= 50 && Riposte.ShouldUse(out act)) return true;
        }
        if (ManaStacks > 0 && Riposte.ShouldUse(out act)) return true;
        #endregion

        return false;
    }

    //判定焦热决断能不能使用
    private bool canUseMagic(IAction act)
    {
        //return IsLastAction(true, Scorch) || IsLastAction(true, Resolution) || IsLastAction(true, Verholy) || IsLastAction(true, Verflare);
        return Scorch.ShouldUse(out act) || Resolution.ShouldUse(out act);
    }
}

