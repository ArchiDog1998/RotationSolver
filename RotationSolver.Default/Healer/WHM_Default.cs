namespace RotationSolver.Default.Healer;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Healer/WHM_Default.cs")]
public sealed class WHM_Default : WHM_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseLilyWhenFull", true, "Auto use Lily when full")
                                            .SetBool("UsePreRegen", false, "Regen on Tank in 5 seconds.");
    }
    public static IBaseAction RegenDefense { get; } = new BaseAction(ActionID.Regen, true, isEot: true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        TargetStatus = Regen.TargetStatus,
    };

    protected override bool GeneralGCD(out IAction act)
    {
        //苦难之心
        if (AfflatusMisery.CanUse(out act, mustUse: true)) return true;

        //泄蓝花 团队缺血时优先狂喜之心
        bool liliesNearlyFull = Lily == 2 && LilyAfter(17);
        bool liliesFullNoBlood = Lily == 3 && BloodLily < 3;
        if (Configs.GetBool("UseLilyWhenFull") && (liliesNearlyFull || liliesFullNoBlood) && AfflatusMisery.EnoughLevel)
        {
            if (PartyMembersAverHP < 0.7)
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

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //加个神速咏唱
        if (PresenseOfMind.CanUse(out act)) return true;

        //加个法令
        if (HasHostilesInRange && Assize.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
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

    [RotationDesc(ActionID.AfflatusSolace, ActionID.Regen, ActionID.Cure2, ActionID.Cure)]
    protected override bool HealSingleGCD(out IAction act)
    {
        //安慰之心
        if (AfflatusSolace.CanUse(out act)) return true;

        //再生
        if (Regen.CanUse(out act)
            && (IsMoving || Regen.Target.GetHealthRatio() > 0.4)) return true;

        //救疗
        if (Cure2.CanUse(out act)) return true;

        //治疗
        if (Cure.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Benediction, ActionID.Asylum, ActionID.DivineBenison, ActionID.Tetragrammaton)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Benediction.CanUse(out act) &&
            Benediction.Target.GetHealthRatio() < 0.3) return true;

        //庇护所
        if (!IsMoving && Asylum.CanUse(out act)) return true;

        //神祝祷
        if (DivineBenison.CanUse(out act)) return true;

        //神名
        if (Tetragrammaton.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.AfflatusRapture, ActionID.Medica2, ActionID.Cure3, ActionID.Medica)]
    protected override bool HealAreaGCD(out IAction act)
    {
        //狂喜之心
        if (AfflatusRapture.CanUse(out act)) return true;

        int hasMedica2 = PartyMembers.Count((n) => n.HasStatus(true, StatusID.Medica2));

        //医济 在小队半数人都没有医济buff and 上次没放医济时使用
        if (Medica2.CanUse(out act) && hasMedica2 < PartyMembers.Count() / 2 && !IsLastAction(true, Medica2)) return true;

        //愈疗
        if (Cure3.CanUse(out act)) return true;

        //医治
        if (Medica.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Asylum)]
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Asylum.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.DivineBenison, ActionID.Aquaveil)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //神祝祷
        if (DivineBenison.CanUse(out act)) return true;

        //水流幕
        if (Aquaveil.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Temperance, ActionID.LiturgyOfTheBell)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //节制
        if (Temperance.CanUse(out act)) return true;

        //礼仪之铃
        if (LiturgyoftheBell.CanUse(out act)) return true;
        return false;
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Stone.CastTime + Service.Config.CountDownAhead
            && Stone.CanUse(out var act)) return act;

        if (Configs.GetBool("UsePreRegen") && remainTime <= 5 && remainTime > 3)
        {
            if (RegenDefense.CanUse(out act)) return act;
            if (DivineBenison.CanUse(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }
}