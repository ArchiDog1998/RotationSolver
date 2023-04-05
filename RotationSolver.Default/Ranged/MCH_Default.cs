namespace RotationSolver.Default.Ranged;

[BetaRotation]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Ranged/MCH_Default.cs")]
[LinkDescription("https://cdn.discordapp.com/attachments/277968251789639680/1086348727691780226/mch_rotation.png")]
[RotationDesc(ActionID.Wildfire)]
public sealed class MCH_Default : MCH_Base
{
    public override string GameVersion => "6.38";

    public override string RotationName => "General Purpose";

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Service.Config.CountDownAhead)
        {
            if (AirAnchor.CanUse(out var act1)) return act1;
            else if(!AirAnchor.EnoughLevel && HotShot.CanUse(out act1)) return act1;
        }
        if (remainTime < 2 && UseBurstMedicine(out var act)) return act;
        if (remainTime < 5 && Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return act;
        return base.CountDownAction(remainTime);
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

        return false;
    }

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (Ricochet.CanUse(out act, CanUseOption.MustUse)) return true;
        if (GaussRound.CanUse(out act, CanUseOption.MustUse)) return true;

        if (!Drill.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShot)
            || nextGCD.IsTheSameTo(false, AirAnchor, Drill, ChainSaw))
        {
            if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            if (UseBurstMedicine(out act)) return true;
            if ((IsLastAbility(false, Hypercharge) || Heat >= 50)
                && !CombatElapsedLess(12)
                && Wildfire.CanUse(out act)) return true;
        }

        if (!CombatElapsedLess(12) && CanUseHypercharge(out act)) return true;
        if (!AirAnchorBlockTime(8)
            && RookAutoturret.CanUse(out act)) return true;

        if (BarrelStabilizer.CanUse(out act)) return true;

        if (CombatElapsedLess(10)) return false;

        var option = CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo;
        if (GaussRound.CurrentCharges <= Ricochet.CurrentCharges)
        {
            if (Ricochet.CanUse(out act, option)) return true;
        }
        if (GaussRound.CanUse(out act, option)) return true;
        
        return false;
    }

    private static bool AirAnchorBlockTime(float time)
    {
        if (AirAnchor.EnoughLevel)
        {
            return AirAnchor.IsCoolingDown && AirAnchor.WillHaveOneCharge(time);
        }
        else
        {
            return HotShot.IsCoolingDown && HotShot.WillHaveOneCharge(time);
        }
    }

    const float REST_TIME = 6f;
    private static bool CanUseHypercharge(out IAction act)
    {
        act = null;

        if(BarrelStabilizer.IsCoolingDown && BarrelStabilizer.WillHaveOneChargeGCD(8))
        {
            if (AirAnchorBlockTime(8)) return false;
        }
        else
        {
            if (AirAnchorBlockTime(12)) return false;
        }

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

        if (Hypercharge.CanUse(out act)) return true;
        return false;
    }
}
