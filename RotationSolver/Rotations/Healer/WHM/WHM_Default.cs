using System.Collections.Generic;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Updaters;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Healer.WHM;
internal sealed class WHM_Default : WHM_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseLilyWhenFull", true, "Auto use Lily when full")
                                            .SetBool("UsePreRegen", false, "Regen on Tank in 5 seconds.");
    }
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealArea, $"{AfflatusRapture}, {Medica2}, {Cure3}, {Medica}\n{Asylum}"},
        {DescType.HealSingle, $"{AfflatusSolace}, {Regen}, {Cure2}, {Cure}\n{Tetragrammaton},{DivineBenison}"},
        {DescType.DefenseArea, $"{Temperance}, {LiturgyoftheBell}"},
        {DescType.DefenseSingle, $"{DivineBenison}, {Aquaveil}"},
    };
    private protected override bool GeneralGCD(out IAction act)
    {
        //苦难之心
        if (AfflatusMisery.CanUse(out act, mustUse: true)) return true;

        //泄蓝花 团队缺血时优先狂喜之心
        bool liliesNearlyFull = Lily == 2 && LilyAfter(17);
        bool liliesFullNoBlood = Lily == 3 && BloodLily < 3;
        if (Configs.GetBool("UseLilyWhenFull") && (liliesNearlyFull || liliesFullNoBlood) && AfflatusMisery.EnoughLevel)
        {
            if (TargetUpdater.PartyMembersAverHP < 0.7)
            {
                if (AfflatusRapture.CanUse(out act)) return true;
            }
            if (AfflatusSolace.CanUse(out act)) return true;

        }

        //群体输出
        if (Holy.CanUse(out act)) return true;

        //单体输出
        if (Aero.CanUse(out act, mustUse: IsMoving)) return true;
        if (Stone.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //加个神速咏唱
        if (PresenseOfMind.CanUse(out act)) return true;

        //加个法令
        if (HaveHostilesInRange && Assize.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //加个无中生有
        if (nextGCD is BaseAction action && action.MPNeed >= 1000 &&
            ThinAir.CanUse(out act)) return true;

        //加个全大赦,狂喜之心 医济医治愈疗
        if (nextGCD.IsTheSameTo(true, AfflatusRapture, Medica, Medica2, Cure3))
        {
            if (PlenaryIndulgence.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //安慰之心
        if (AfflatusSolace.CanUse(out act)) return true;

        //再生
        if (Regen.Target.GetHealthRatio() > 0.4 && Regen.CanUse(out act)) return true;

        //救疗
        if (Cure2.CanUse(out act)) return true;

        //治疗
        if (Cure.CanUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //天赐 大资源救急用
        if (Benediction.Target.GetHealthRatio() < 0.3
            && Benediction.CanUse(out act)) return true;

        //庇护所
        if (!IsMoving && Asylum.CanUse(out act)) return true;

        //神祝祷
        if (DivineBenison.CanUse(out act)) return true;

        //神名
        if (Tetragrammaton.CanUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        //狂喜之心
        if (AfflatusRapture.CanUse(out act)) return true;

        var PartyMembers = TargetUpdater.PartyMembers;
        int hasMedica2 = PartyMembers.Count((n) => n.HasStatus(true, StatusID.Medica2));

        //医济 在小队半数人都没有医济buff and 上次没放医济时使用
        if (Medica2.CanUse(out act) && hasMedica2 < PartyMembers.Count() / 2 && !IsLastAction(true, Medica2)) return true;

        //愈疗
        if (Cure3.CanUse(out act)) return true;

        //医治
        if (Medica.CanUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //庇护所
        if (Asylum.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //神祝祷
        if (DivineBenison.CanUse(out act)) return true;

        //水流幕
        if (Aquaveil.CanUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //节制
        if (Temperance.CanUse(out act)) return true;

        //礼仪之铃
        if (LiturgyoftheBell.CanUse(out act)) return true;
        return false;
    }

    //开局5s使用再生和盾给开了盾姿的t
    private protected override IAction CountDownAction(float remainTime)
    {
        if (Configs.GetBool("UsePreRegen") && remainTime <= 5 && remainTime > 3 && DivineBenison.CanUse(out _))
        {
            if (DivineBenison.CanUse(out _))
            {
                return DivineBenison;
            }
            if (Regen.CanUse(out _))
            {
                return Regen;
            }
        }
        return base.CountDownAction(remainTime);
    }
}