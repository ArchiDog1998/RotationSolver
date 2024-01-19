namespace DefaultRotations.Ranged;

[BetaRotation]
[SourceCode(Path = "main/DefaultRotations/Ranged/MCH_Default.cs")]
[LinkDescription("https://cdn.discordapp.com/attachments/277968251789639680/1086348727691780226/mch_rotation.png")]
[RotationDesc(ActionID.Wildfire)]
public sealed class MCH_Default : MCH_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.38";

    public override string RotationName => "Delayed Tools Opener";

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < CountDownAhead)
        {
            if (AirAnchor.CanUse(out var act1)) return act1;
            else if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act1)) return act1;
        }
        if (remainTime < 2 && UseBurstMedicine(out var act)) return act;
        if (remainTime < 5 && Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.IgnoreClippingCheck)) return act;
        return base.CountDownAction(remainTime);
    }
    
    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool(CombatType.PvE, "MCH_Reassemble", true, "Use Reassamble with ChainSaw");
    }
    
    protected override bool GeneralGCD(out IAction act)
    {
        //Overheated
        if (AutoCrossbow.CanUse(out act)) return true;
        if (HeatBlast.CanUse(out act)) return true;

        //Long Cds
        if (BioBlaster.CanUse(out act)) return true;
        if (!SpreadShot.CanUse(out _))
        {
            if (AirAnchor.CanUse(out act)) return true;
            else if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act)) return true;

            if (Drill.CanUse(out act)) return true;
        }

        if (!CombatElapsedLessGCD(4) && ChainSaw.CanUse(out act, CanUseOption.MustUse)) return true;

        //Aoe
        if (ChainSaw.CanUse(out act)) return true;
        if (SpreadShot.CanUse(out act)) return true;

        //Single
        if (CleanShot.CanUse(out act)) return true;
        if (SlugShot.CanUse(out act)) return true;
        if (SplitShot.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (Configs.GetBool("MCH_Reassemble") && ChainSaw.EnoughLevel && nextGCD.IsTheSameTo(true, ChainSaw))
        {
            if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        if (Ricochet.CanUse(out act, CanUseOption.MustUse)) return true;
        if (GaussRound.CanUse(out act, CanUseOption.MustUse)) return true;

        if (!Drill.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShot)
            || nextGCD.IsTheSameTo(false, AirAnchor, ChainSaw, Drill))
        {
            if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (IsBurst)
        {
            if (UseBurstMedicine(out act)) return true;
            if ((IsLastAbility(false, Hypercharge) || Heat >= 50) && !CombatElapsedLess(10)
                && Wildfire.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }

        if (!CombatElapsedLess(12) && CanUseHypercharge(out act)) return true;
        if (CanUseRookAutoturret(out act)) return true;

        if (BarrelStabilizer.CanUse(out act)) return true;

        if (CombatElapsedLess(8)) return false;

        var option = CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo;
        if (GaussRound.CurrentCharges <= Ricochet.CurrentCharges)
        {
            if (Ricochet.CanUse(out act, option)) return true;
        }
        if (GaussRound.CanUse(out act, option)) return true;

        return base.AttackAbility(out act);
    }

    //private static bool AirAnchorBlockTime(float time)
    //{
    //    if (AirAnchor.EnoughLevel)
    //    {
    //        return AirAnchor.IsCoolingDown && AirAnchor.WillHaveOneCharge(time);
    //    }
    //    else
    //    {
    //        return HotShot.IsCoolingDown && HotShot.WillHaveOneCharge(time);
    //    }
    //}

    private static bool CanUseRookAutoturret(out IAction act)
    {
        act = null;
        if (AirAnchor.EnoughLevel)
        {
            if (!AirAnchor.IsCoolingDown || AirAnchor.ElapsedAfter(18)) return false;
        }
        else
        {
            if (!HotShot.IsCoolingDown || HotShot.ElapsedAfter(18)) return false;
        }

        return RookAutoturret.CanUse(out act);
    }

    const float REST_TIME = 6f;
    private static bool CanUseHypercharge(out IAction act)
    {
        act = null;

        //if (BarrelStabilizer.IsCoolingDown && BarrelStabilizer.WillHaveOneChargeGCD(8))
        //{
        //    if (AirAnchorBlockTime(8)) return false;
        //}
        //else
        //{
        //    if (AirAnchorBlockTime(12)) return false;
        //}

        //Check recast.
        if (!SpreadShot.CanUse(out _))
        {
            if (AirAnchor.EnoughLevel)
            {
                if (AirAnchor.WillHaveOneCharge(REST_TIME)) return false;
            }
            else
            {
                if (HotShot.EnoughLevel && HotShot.WillHaveOneCharge(REST_TIME)) return false;
            }
        }
        if (Drill.EnoughLevel && Drill.WillHaveOneCharge(REST_TIME)) return false;
        if (ChainSaw.EnoughLevel && ChainSaw.WillHaveOneCharge(REST_TIME)) return false;

        return Hypercharge.CanUse(out act);
    }
}
