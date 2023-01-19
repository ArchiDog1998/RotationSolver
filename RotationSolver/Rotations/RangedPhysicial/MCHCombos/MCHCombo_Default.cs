using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using System.Collections.Generic;
using System.Linq;
using RotationSolver.Updaters;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.RangedPhysicial.MCHCombos;

internal sealed class MCHCombo_Default : MCHRotation_Base
{
    public override string GameVersion => "6.28";

    /// <summary>
    /// 4人本小怪快死了
    /// </summary>
    private static bool isDyingNotBoss => !Target.IsBoss() && IsTargetDying && TargetUpdater.PartyMembers.Count() is > 1 and <= 4;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.Description, $"标准循环会在野火前攒热量来打偶数分钟爆发.\n                     AOE和攻击小怪时不会释放野火"},
        {DescType.BreakingAction, $"{Wildfire}"},
        {DescType.DefenseArea, $"{Tactician}"},
    };

    public override string RotationName => "汐ベMoon";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("MCH_Opener", true, "标准起手")
            .SetBool("MCH_Automaton", true, "机器人吃团辅")
            .SetBool("MCH_Reassemble", true, "整备优先链锯")
            .SetBool("DelayHypercharge", false, "强制在GCD后半段使用");
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //策动
        if (Tactician.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //不在战斗中时重置起手
        if (!InCombat)
        {
            //开场前整备,空气锚和钻头必须冷却好
            if (AirAnchor.EnoughLevel && (!AirAnchor.IsCoolDown || !Drill.IsCoolDown) && Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        //群体常规GCD
        //AOE,毒菌冲击
        if (Bioblaster.ShouldUse(out act)) return true;
        if (ChainSaw.ShouldUse(out act)) return true;
        if (IsOverheated && AutoCrossbow.ShouldUse(out act)) return true;
        if (SpreadShot.ShouldUse(out act)) return true;

        if (!IsOverheated || IsOverheated && OverheatedEndAfterGCD())
        {
            //单体,四个牛逼的技能。先空气锚再钻头
            if (AirAnchor.ShouldUse(out act)) return true;
            else if (!AirAnchor.EnoughLevel && HotShot.ShouldUse(out act)) return true;
            if (Drill.ShouldUse(out act)) return true;
            if (ChainSaw.ShouldUse(out act, mustUse: true))
            {
                if (Player.HasStatus(true, StatusID.Reassemble)) return true;
                if (!Configs.GetBool("MCH_Opener") || Wildfire.IsCoolDown) return true;
                if (AirAnchor.IsCoolDown && AirAnchor.ElapsedAfterGCD(4) && Drill.IsCoolDown && Drill.ElapsedAfterGCD(3)) return true;
                if (AirAnchor.IsCoolDown && AirAnchor.ElapsedAfterGCD(3) && Drill.IsCoolDown && Drill.ElapsedAfterGCD(4)) return true;
            }
        }

        //过热状态
        if (IsOverheated && HeatBlast.ShouldUse(out act)) return true;

        //单体常规GCD
        if (CleanShot.ShouldUse(out act)) return true;
        if (SlugShot.ShouldUse(out act)) return true;
        if (SplitShot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //提前5秒整备
        if (remainTime <= 5 && Reassemble.ShouldUse(out _, emptyOrSkipCombo: true)) return Reassemble;
        return base.CountDownAction(remainTime);
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //等级小于钻头时,绑定狙击弹
        if (!Drill.EnoughLevel && nextGCD.IsAnySameAction(true, CleanShot))
        {
            if (Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //等级小于90时,整备不再留层数
        if ((!ChainSaw.EnoughLevel || !Configs.GetBool("MCH_Reassemble"))
            && nextGCD.IsAnySameAction(false, AirAnchor, Drill))
        {
            if (Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //整备优先链锯
        if (Configs.GetBool("MCH_Reassemble") && nextGCD.IsAnySameAction(true, ChainSaw))
        {
            if (Reassemble.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        //如果接下来要搞三大金刚了，整备吧！
        if (ChainSaw.EnoughLevel && nextGCD.IsAnySameAction(true, AirAnchor, Drill))
        {
            if (Reassemble.ShouldUse(out act)) return true;
        }
        //起手在链锯前释放野火
        if (nextGCD.IsAnySameAction(true, ChainSaw) && !IsLastGCD(true, HeatBlast))
        {
            if (SettingBreak && Configs.GetBool("MCH_Opener") && Wildfire.ShouldUse(out act)) return true;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //野火
        if (SettingBreak && CanUseWildfire(out act)) return true;

        //车式浮空炮塔
        if (CanUseRookAutoturret(out act)) return true;

        //起手虹吸弹、弹射
        if (Ricochet.CurrentCharges == Ricochet.MaxCharges && Ricochet.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        if (GaussRound.CurrentCharges == GaussRound.MaxCharges && GaussRound.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;

        //枪管加热
        if (BarrelStabilizer.ShouldUse(out act)) return true;

        //超荷
        if (CanUseHypercharge(out act) && (Configs.GetBool("MCH_Opener") && abilityRemain == 1 || !Configs.GetBool("MCH_Opener"))) return true;

        if (GaussRound.CurrentCharges <= Ricochet.CurrentCharges)
        {
            //弹射
            if (Ricochet.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }
        //虹吸弹
        if (GaussRound.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;

        act = null!;
        return false;
    }

    /// <summary>
    /// 判断能否使用野火
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseWildfire(out IAction act)
    {
        if (!Wildfire.ShouldUse(out act)) return false;

        if (Heat < 50 && !IsOverheated) return false;

        //小怪和AOE期间不打野火
        if (SpreadShot.ShouldUse(out _) || TargetUpdater.PartyMembers.Count() is > 1 and <= 4 && !Target.IsBoss()) return false;

        //在过热时
        if (IsLastAction(true, Hypercharge)) return true;

        if (ChainSaw.EnoughLevel && !ChainSaw.IsCoolDown) return false;

        if (Hypercharge.IsCoolDown) return false;

        //当上一个技能是钻头,空气锚,热冲击时不释放野火
        if (IsLastGCD(true, Drill, HeatBlast, AirAnchor)) return false;

        return true;
    }

    /// <summary>
    /// 判断能否使用超荷
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseHypercharge(out IAction act)
    {
        if (!Hypercharge.ShouldUse(out act) || Player.HasStatus(true, StatusID.Reassemble)) return false;

        //有野火buff必须释放超荷
        if (Player.HasStatus(true, StatusID.Wildfire)) return true;

        //4人本小怪快死了不释放
        //if (isDyingNotBoss) return false;

        //在三大金刚还剩8秒冷却好时不释放超荷
        if (Drill.EnoughLevel && Drill.WillHaveOneChargeGCD(3)) return false;
        if (AirAnchor.EnoughLevel && AirAnchor.WillHaveOneCharge(3)) return false;
        if (ChainSaw.EnoughLevel && (ChainSaw.IsCoolDown && ChainSaw.WillHaveOneCharge(3) || !ChainSaw.IsCoolDown) && Configs.GetBool("MCH_Opener")) return false;

        //小怪AOE和4人本超荷判断
        if (SpreadShot.ShouldUse(out _))
        {
            if (!AutoCrossbow.EnoughLevel) return false;
            return true;
        }

        //等级低于野火
        if (!Wildfire.EnoughLevel) return true;

        //野火前攒热量
        if (!Wildfire.WillHaveOneChargeGCD(5) && Wildfire.WillHaveOneChargeGCD(18))
        {
            //如果期间热量溢出超过5,就释放一次超荷
            if (IsLastGCD((ActionID)Drill.ID) && Heat >= 85) return true;
            return false;
        }
        else return true;
    }

    /// <summary>
    /// 判断能否使用机器人
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseRookAutoturret(out IAction act)
    {
        if (!RookAutoturret.ShouldUse(out act, mustUse: true)) return false;

        //4人本小怪快死了不释放
        if (isDyingNotBoss) return false;

        //如果上一个技能是野火不释放
        if (IsLastAction((ActionID)Wildfire.ID)) return false;

        //电量等于100,强制释放
        if (Battery == 100 && ChainSaw.EnoughLevel && !ChainSaw.WillHaveOneCharge(13)) return true;

        //小怪,AOE,不吃团辅判断
        if (!Configs.GetBool("MCH_Automaton") || !Target.IsBoss() && !IsMoving || Level < Wildfire.ID) return true;
        if (SpreadShot.ShouldUse(out _) && !Target.IsBoss() && IsMoving) return false;

        //机器人吃团辅判断
        if (AirAnchor.IsCoolDown && AirAnchor.WillHaveOneChargeGCD() && Battery > 80) return true;
        if (ChainSaw.WillHaveOneCharge(4) || ChainSaw.IsCoolDown && !ChainSaw.ElapsedAfterGCD(3) && Battery <= 60) return true;

        return false;
    }
}
