
namespace RotationSolver.Default.Tank;

[LinkDescription("https://xiv.sleepyshiba.com/pld/img/63-60stentative2.png")]
[RotationDesc("The whole rotation's burst\nis base on:")]
[RotationDesc(ActionID.FightOrFlight)]
[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Tank/PLD_Default.cs")]
public class PLD_Default : PLD_Base
{
    public override string GameVersion => "6.31";
    public override string RotationName => "Tentative v1.2";

    public override string Description => "Please work well!";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseDivineVeilPre", false, "DivineVeilPre in 15 seconds counting down.")
            .SetBool("UseHolyWhenAway", true, "Use HolyCircle or HolySpirit when far from enemies");
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < HolySpirit.CastTime + Service.Config.CountDownAhead
            && HolySpirit.CanUse(out var act)) return act;

        if (remainTime < 15 && Configs.GetBool("UseDivineVeilPre")
            && DivineVeil.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;

        if (abilitiesRemaining == 1 && InCombat)
        {
            if (UseBurstMedicine(out act)) return true;
            if (InBurst && !CombatElapsedLess(5) && FightOrFlight.CanUse(out act)) return true;
        }
        if (CombatElapsedLess(8)) return false;

        if (CircleOfScorn.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Expiacion.CanUse(out act, CanUseOption.MustUse)) return true;
        if (SpiritsWithin.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Player.WillStatusEndGCD(6, 0, true, StatusID.FightOrFlight)
            && Requiescat.CanUse(out act, CanUseOption.MustUse)) return true;

        var option = CanUseOption.MustUse;
        if(HasFightOrFlight) option |= CanUseOption.EmptyOrSkipCombo;
        if (!IsMoving && Intervene.CanUse(out act, option)) return true;

        if (HasTankStance && OathGauge == 100 && Sheltron.CanUse(out act)) return true;

        return false;
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

        return false;
    }

    [RotationDesc(ActionID.Reprisal, ActionID.DivineVeil)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Reprisal.CanUse(out act, CanUseOption.MustUse)) return true;
        if (DivineVeil.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.PassageOfArms)]
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (PassageOfArms.CanUse(out act)) return true;
        return base.HealAreaAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.Sentinel, ActionID.Rampart, ActionID.Bulwark, ActionID.Sheltron, ActionID.Reprisal)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            //10
            if (OathGauge >= 90 && UseOath(out act)) return true;

            //30
            if (Sentinel.CanUse(out act)) return true;

            //20
            if (Rampart.CanUse(out act)) return true;

            //10
            if (UseOath(out act)) return true;
            if (Bulwark.CanUse(out act)) return true;
        }

        if (Reprisal.CanUse(out act)) return true;

        return false;
    }

    private static bool UseOath(out IAction act)
    {
        if (HasTankStance)
        {
            if (Sheltron.CanUse(out act)) return true;
        }
        else
        {
            if (Intervention.CanUse(out act)) return true;
        }
        return false;
    }
}
