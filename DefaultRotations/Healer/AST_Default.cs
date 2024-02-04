namespace DefaultRotations.Healer;

[Rotation(Name = "Default", GameVersion ="6.28")]
[RotationDesc(ActionID.DivinationPvE)]
[SourceCode(Path = "main/DefaultRotations/Healer/AST_Default.cs")]
public sealed class AST_Default : AstrologianRotation
{
    public override CombatType Type => CombatType.PvE;

    protected override IRotationConfigSet CreateConfiguration()
        => base.CreateConfiguration()
            .SetFloat(ConfigUnitType.Seconds, CombatType.PvE, "UseEarthlyStarTime", 15, "Use Earthly Star during countdown timer.", 4, 20);

    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < MaleficPvE.Info.CastTime + CountDownAhead
            && MaleficPvE.CanUse(out var act)) return act;
        if (remainTime < 3 && UseBurstMedicine(out act)) return act;
        if (remainTime is < 4 and > 3 && AspectedBeneficPvE.CanUse(out act)) return act;
        if (remainTime < Configs.GetFloat("UseEarthlyStarTime")
            && EarthlyStarPvE.CanUse(out act)) return act;
        if (remainTime < 30 && DrawPvE.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    [RotationDesc(ActionID.CelestialIntersectionPvE, ActionID.ExaltationPvE)]
    protected override bool DefenseSingleAbility(out IAction? act)
    {
        if (CelestialIntersectionPvE.CanUse(out act, isEmpty:true)) return true;
        if (ExaltationPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }

    [RotationDesc(ActionID.MacrocosmosPvE)]
    protected override bool DefenseAreaGCD(out IAction? act)
    {
        if (MacrocosmosPvE.CanUse(out act)) return true;
        return base.DefenseAreaGCD(out act);
    }

    [RotationDesc(ActionID.CollectiveUnconsciousPvE)]
    protected override bool DefenseAreaAbility(out IAction? act)
    {
        if (CollectiveUnconsciousPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        ////Add AspectedBeneficwhen not in combat.
        //if (NotInCombatDelay && AspectedBeneficDefensePvE.CanUse(out act)) return true;

        if (GravityPvE.CanUse(out act)) return true;

        if (CombustPvE.CanUse(out act)) return true;
        if (MaleficPvE.CanUse(out act)) return true;
        if (CombustPvE.CanUse(out act, skipStatusProvideCheck: true )) return true;

        return base.GeneralGCD(out act);
    }

    [RotationDesc(ActionID.AspectedHeliosPvE, ActionID.HeliosPvE)]
    protected override bool HealAreaGCD(out IAction? act)
    {
        if (AspectedHeliosPvE.CanUse(out act)) return true;
        if (HeliosPvE.CanUse(out act)) return true;
        return base.HealAreaGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (base.EmergencyAbility(nextGCD, out act)) return true;

        if (!InCombat) return false;

        if (nextGCD.IsTheSameTo(true, AspectedHeliosPvE, HeliosPvE))
        {
            if (HoroscopePvE.CanUse(out act)) return true;
            if (NeutralSectPvE.CanUse(out act)) return true;
        }

        if (nextGCD.IsTheSameTo(true, BeneficPvE, BeneficIiPvE, AspectedBeneficPvE))
        {
            if (SynastryPvE.CanUse(out act)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(out IAction? act)
    {
        if (DrawPvE.CanUse(out act)) return true;
        if (RedrawPvE.CanUse(out act)) return true;
        return base.GeneralAbility(out act);
    }

    [RotationDesc(ActionID.AspectedBeneficPvE, ActionID.BeneficIiPvE, ActionID.BeneficPvE)]
    protected override bool HealSingleGCD(out IAction? act)
    {
        if (AspectedBeneficPvE.CanUse(out act)
            && (IsMoving
            || AspectedBeneficPvE.Target!.Value.Target.GetHealthRatio() > 0.4)) return true;

        if (BeneficIiPvE.CanUse(out act)) return true;
        if (BeneficPvE.CanUse(out act)) return true;

        return base.HealSingleGCD(out act);
    }

    protected override bool AttackAbility(out IAction? act)
    {
        if (IsBurst && !IsMoving
            && DivinationPvE.CanUse(out act)) return true;

        if (MinorArcanaPvE.CanUse(out act, isEmpty:true)) return true;

        if (DrawPvE.CanUse(out act, isEmpty: IsBurst)) return true;

        if (IsMoving && LightspeedPvE.CanUse(out act)) return true;

        if (!IsMoving)
        {
            if (!Player.HasStatus(true, StatusID.EarthlyDominance, StatusID.GiantDominance))
            {
                if (EarthlyStarPvE.CanUse(out act)) return true;
            }
            if (AstrodynePvE.CanUse(out act)) return true;
        }

        if (DrawnCrownCard == CardType.LORD || MinorArcanaPvE.Cooldown.WillHaveOneChargeGCD(1, 0))
        {
            if (MinorArcanaPvE.CanUse(out act)) return true;
        }

        if (RedrawPvE.CanUse(out act)) return true;
        if (PlayCard(out act)) return true;

        return base.AttackAbility(out act);
    }

    [RotationDesc(ActionID.EssentialDignityPvE, ActionID.CelestialIntersectionPvE, ActionID.CelestialOppositionPvE,
        ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealSingleAbility(out IAction? act)
    {
        if (EssentialDignityPvE.CanUse(out act)) return true;
        if (CelestialIntersectionPvE.CanUse(out act, isEmpty:true)) return true;

        if (DrawnCrownCard == CardType.LADY 
            && MinorArcanaPvE.CanUse(out act)) return true;

        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (!Player.HasStatus(true, StatusID.HoroscopeHelios, StatusID.Horoscope) && HoroscopePvE.CanUse(out act)) return true;

        if ((Player.HasStatus(true, StatusID.HoroscopeHelios)
            || PartyMembersMinHP < 0.3)
            && HoroscopePvE.CanUse(out act)) return true;

        return base.HealSingleAbility(out act);
    }

    [RotationDesc(ActionID.CelestialOppositionPvE, ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealAreaAbility(out IAction act)
    {
        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoroscopeHelios) && HoroscopePvE.CanUse(out act)) return true;

        if (DrawnCrownCard == CardType.LADY && MinorArcanaPvE.CanUse(out act)) return true;

        return base.HealAreaAbility(out act);
    }
}
