namespace DefaultRotations.Melee;

[BetaRotation]
[RotationDesc(ActionID.ArcaneCircle)]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/rpr/double_communio.png")]
[LinkDescription("https://www.thebalanceffxiv.com/img/jobs/rpr/rpr_6.3_early_enshroud.png")]
[SourceCode(Path = "main/DefaultRotations/Melee/RPR_Default.cs")]
public sealed class RPR_Default : RPR_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.38";

    public override string RotationName => "Early Enshroud";

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Harpe.CastTime + CountDownAhead
            && Harpe.CanUse(out var act)) return act;

        if (SoulSow.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    private static bool Reaping(out IAction act)
    {
        if (GrimReaping.CanUse(out act)) return true;
        if (Player.HasStatus(true, StatusID.EnhancedCrossReaping) || !Player.HasStatus(true, StatusID.EnhancedVoidReaping))
        {
            if (CrossReaping.CanUse(out act)) return true;
        }
        else
        {
            if (VoidReaping.CanUse(out act)) return true;
        }
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (SoulSow.CanUse(out act)) return true;

        if (WhorlOfDeath.CanUse(out act)) return true;
        if (ShadowOfDeath.CanUse(out act)) return true;

        if (HasEnshrouded)
        {
            if (ShadowOfDeath.CanUse(out act)) return true;

            if (LemureShroud > 1)
            {
                if (PlentifulHarvest.EnoughLevel && ArcaneCircle.WillHaveOneCharge(9) &&
                   (LemureShroud == 4 && (HostileTarget?.WillStatusEnd(30, true, StatusID.DeathsDesign) ?? false) || LemureShroud == 3 && (HostileTarget?.WillStatusEnd(50, true, StatusID.DeathsDesign) ?? false)))
                {
                    if (ShadowOfDeath.CanUse(out act, CanUseOption.MustUse)) return true;
                }

                if(Reaping(out act)) return true;
            }
            if (LemureShroud == 1)
            {
                if (Communio.EnoughLevel)
                {
                    if (!IsMoving && Communio.CanUse(out act, CanUseOption.MustUse))
                    {
                        return true;
                    }
                    else
                    {
                        if (ShadowOfDeath.CanUse(out act, IsMoving ? CanUseOption.MustUse : CanUseOption.None)) return true;
                    }
                }
                else
                {
                    if (Reaping(out act)) return true;
                }
            }
        }

        if (HasSoulReaver)
        {
            if (Guillotine.CanUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.EnhancedGibbet))
            {
                if (Gibbet.CanUse(out act)) return true;
            }
            else
            {
                if (Gallows.CanUse(out act)) return true;
            }
        }

        if (!CombatElapsedLessGCD(2) && PlentifulHarvest.CanUse(out act, CanUseOption.MustUse)) return true;

        if (SoulScythe.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        if (SoulSlice.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;

        if (NightmareScythe.CanUse(out act)) return true;
        if (SpinningScythe.CanUse(out act)) return true;

        if (InfernalSlice.CanUse(out act)) return true;
        if (WaxingSlice.CanUse(out act)) return true;
        if (Slice.CanUse(out act)) return true;

        if (InCombat && HarvestMoon.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Harpe.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        var IsTargetBoss = HostileTarget?.IsBossFromTTK() ?? false;
        var IsTargetDying = HostileTarget?.IsDying() ?? false;

        if (IsBurst)
        {
            if (UseBurstMedicine(out act))
            {
                if (CombatElapsedLess(10))
                {
                    if (!CombatElapsedLess(5)) return true;
                }
                else
                {
                    if(ArcaneCircle.WillHaveOneCharge(5)) return true;
                }
            }
            if ((HostileTarget?.HasStatus(true, StatusID.DeathsDesign) ?? false)
                && ArcaneCircle.CanUse(out act)) return true;
        }

        if (IsTargetBoss && IsTargetDying ||
           !Configs.GetBool("EnshroudPooling") && Shroud >= 50 ||
           Configs.GetBool("EnshroudPooling") && Shroud >= 50 &&
           (!PlentifulHarvest.EnoughLevel ||
           Player.HasStatus(true, StatusID.ArcaneCircle) ||
           ArcaneCircle.WillHaveOneCharge(8) ||
           !Player.HasStatus(true, StatusID.ArcaneCircle) && ArcaneCircle.WillHaveOneCharge(65) && !ArcaneCircle.WillHaveOneCharge(50) ||
           !Player.HasStatus(true, StatusID.ArcaneCircle) && Shroud >= 90))
        {
            if (Enshroud.CanUse(out act)) return true;
        }

        if (HasEnshrouded && (Player.HasStatus(true, StatusID.ArcaneCircle) || LemureShroud < 3))
        {
            if (LemuresScythe.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
            if (LemuresSlice.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }

        if (PlentifulHarvest.EnoughLevel && !Player.HasStatus(true, StatusID.ImmortalSacrifice) && !Player.HasStatus(true, StatusID.BloodSownCircle) || !PlentifulHarvest.EnoughLevel)
        {
            if (Gluttony.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (!Player.HasStatus(true, StatusID.BloodSownCircle) && !Player.HasStatus(true, StatusID.ImmortalSacrifice) && (Gluttony.EnoughLevel && !Gluttony.WillHaveOneChargeGCD(4) || !Gluttony.EnoughLevel || Soul == 100))
        {
            if (GrimSwathe.CanUse(out act)) return true;
            if (BloodStalk.CanUse(out act)) return true;
        }

        return base.AttackAbility(out act);
    }
}