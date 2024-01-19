namespace DefaultRotations.Tank;

[LinkDescription("https://xiv.sleepyshiba.com/pld/img/63-60stentative2.png")]
[RotationDesc("The whole rotation's burst\nis base on:")]
[RotationDesc(ActionID.FightOrFlight)]
[SourceCode(Path = "main/DefaultRotations/Tank/PLD_Default.cs")]
public class PLD_Default : PLD_Base
{
    public override CombatType Type => CombatType.PvE;

    public override string GameVersion => "6.31";

    public override string RotationName => "Tentative v1.2";

    public override string Description => "Please work well!";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool(CombatType.PvE, "UseDivineVeilPre", false, "Use Divine Veil at 15 seconds remaining on Countdown")
            .SetBool(CombatType.PvE, "UseHolyWhenAway", true, "Use Holy Circle or Holy Spirit when out of melee range")
            .SetBool(CombatType.PvE, "UseShieldBash", true, "Use Shield Bash when Low Blow is cooling down");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < HolySpirit.CastTime + CountDownAhead
            && HolySpirit.CanUse(out var act)) return act;

        if (remainTime < 15 && Configs.GetBool("UseDivineVeilPre")
            && DivineVeil.CanUse(out act, CanUseOption.IgnoreClippingCheck)) return act;

        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(out IAction act)
    {
        act = null;

        if (InCombat)
        {
            if (UseBurstMedicine(out act)) return true;
            if (IsBurst && !CombatElapsedLess(5) && FightOrFlight.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }
        if (CombatElapsedLess(8)) return false;

        if (CircleOfScorn.CanUse(out act, CanUseOption.MustUse)) return true;
        if (SpiritsWithin.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Player.WillStatusEndGCD(6, 0, true, StatusID.FightOrFlight)
            && Requiescat.CanUse(out act, CanUseOption.MustUse)) return true;

        var option = CanUseOption.MustUse;
        if (HasFightOrFlight) option |= CanUseOption.EmptyOrSkipCombo;
        if (!IsMoving && Intervene.CanUse(out act, option)) return true;

        if (HasTankStance && OathGauge == 100 && UseOath(out act)) return true;

        return base.AttackAbility(out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        if (Player.HasStatus(true, StatusID.Requiescat))
        {
            if (Confiteor.CanUse(out act, CanUseOption.MustUse))
            {
                if (Player.HasStatus(true, StatusID.ConfiteorReady)) return true;
                if (Confiteor.ID != Confiteor.AdjustedID) return true;
            }
            if (HolyCircle.CanUse(out act)) return true;
            if (HolySpirit.CanUse(out act)) return true;
        }

        //AOE
        if (HasDivineMight && HolyCircle.CanUse(out act)) return true;
        if (Prominence.CanUse(out act)) return true;
        if (TotalEclipse.CanUse(out act)) return true;

        //Single
        if (!CombatElapsedLess(8) && HasFightOrFlight && GoringBlade.CanUse(out act)) return true; // Dot
        if (!FightOrFlight.WillHaveOneChargeGCD(2))
        {
            if (!FightOrFlight.WillHaveOneChargeGCD(6) &&
                HasDivineMight && HolySpirit.CanUse(out act)) return true;
            if (RageOfHalone.CanUse(out act)) return true;
            if (Atonement.CanUse(out act)) return true;
        }
        //123
        if( Configs.GetBool("UseShieldBash") && ShieldBash.CanUse(out act)) return true;

        if (RageOfHalone.CanUse(out act)) return true;
        if (RiotBlade.CanUse(out act)) return true;
        if (FastBlade.CanUse(out act)) return true;

        //Range
        if (Configs.GetBool("UseHolyWhenAway"))
        {
            if (HolyCircle.CanUse(out act)) return true;
            if (HolySpirit.CanUse(out act)) return true;
        }
        if (ShieldLob.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    [RotationDesc(ActionID.Reprisal, ActionID.DivineVeil)]
    protected override bool DefenseAreaAbility(out IAction act)
    {
        if (Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;
        if (DivineVeil.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    [RotationDesc(ActionID.PassageOfArms)]
    protected override bool HealAreaAbility(out IAction act)
    {
        if (PassageOfArms.CanUse(out act)) return true;
        return base.HealAreaAbility(out act);
    }

    [RotationDesc(ActionID.Sentinel, ActionID.Rampart, ActionID.Bulwark, ActionID.Sheltron, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        //10
        if (Bulwark.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        if (UseOath(out act, CanUseOption.OnLastAbility)) return true;
        //30
        if ((!Rampart.IsCoolingDown || Rampart.ElapsedAfter(60)) && Sentinel.CanUse(out act)) return true;

        //20
        if (Sentinel.IsCoolingDown && Sentinel.ElapsedAfter(60) && Rampart.CanUse(out act)) return true;

        if (Reprisal.CanUse(out act)) return true;

        return base.DefenseSingleAbility(out act);
    }

    private static bool UseOath(out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Sheltron.CanUse(out act, option)) return true;
        if (Intervention.CanUse(out act, option)) return true;

        return false;
    }
}
