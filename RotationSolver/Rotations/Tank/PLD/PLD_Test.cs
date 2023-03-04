using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;

namespace RotationSolver.Rotations.Tank.PLD;

[RotationDesc("The whole rotation's burst\nis base on:")]
[RotationDesc(ActionID.FightorFlight)]
internal class PLD_Test : PLD_Base
{
    public override string GameVersion => "6.31";
    public override string RotationName => "Test";

    public override string Description => "Tentative v1.2\nPlease work well!";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseDivineVeilPre", false, "DivineVeilPre in 15 seconds counting down.");
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < HolySpirit.CastTime + Service.Configuration.CountDownAhead
            && HolySpirit.CanUse(out var act)) return act;

        if (remainTime < 15 && Configs.GetBool("UseDivineVeilPre")
            && DivineVeil.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;

        if (abilitiesRemaining == 1 && InCombat)
        {
            if (UseBurstMedicine(out act)) return true;
            if (InBurst && FightorFlight.CanUse(out act)) return true;
        }
        if (CombatElapsedLess(5)) return false;

        if (CircleofScorn.CanUse(out act, mustUse: true)) return true;
        if (Expiacion.CanUse(out act, mustUse: true)) return true;
        if (SpiritsWithin.CanUse(out act, mustUse: true)) return true;

        if (Player.WillStatusEndGCD(4, 0, true, StatusID.FightOrFlight)
            && Requiescat.CanUse(out act, mustUse: true)) return true;

        if (!IsMoving && Intervene.CanUse(out act, true, HasFightOrFlight)) return true;

        if (HasTankStance && OathGauge == 100 && Sheltron.CanUse(out act)) return true;

        return false;
    }

    private static bool UseHoly => HasDivineMight && !FightorFlight.WillHaveOneChargeGCD(1);

    private protected override bool GeneralGCD(out IAction act)
    {
        if(Player.HasStatus(true, StatusID.Requiescat))
        {
            if (Confiteor.CanUse(out act, mustUse: true))
            {
                if (Player.HasStatus(true, StatusID.ConfiteorReady)) return true;
                if (Confiteor.ID != Confiteor.AdjustedID) return true;
            }
            if (HolyCircle.CanUse(out act)) return true;
            if (HolySpirit.CanUse(out act)) return true;
        }


        //AOE
        if (UseHoly && HolyCircle.CanUse(out act)) return true;
        if (Prominence.CanUse(out act)) return true;
        if (TotalEclipse.CanUse(out act)) return true;

        //Single
        if (!CombatElapsedLess(7) && GoringBlade.CanUse(out act)) return true; // Dot
        if (UseHoly && HolySpirit.CanUse(out act)) return true;
        if (RageofHalone.CanUse(out act)) return true;
        if (!FightorFlight.WillHaveOneChargeGCD(1) && Atonement.CanUse(out act)) return true;
        if (RiotBlade.CanUse(out act)) return true;
        if (FastBlade.CanUse(out act)) return true;

        //Range
        if (HolySpirit.CanUse(out act)) return true;
        if (ShieldLob.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Reprisal, ActionID.DivineVeil)]
    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Reprisal.CanUse(out act, mustUse: true)) return true;
        if (DivineVeil.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.PassageofArms)]
    private protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (PassageofArms.CanUse(out act)) return true;
        return base.HealAreaAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.Sentinel, ActionID.Rampart, ActionID.Bulwark, ActionID.Sheltron, ActionID.Reprisal)]
    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            //10
            if (OathGauge >= 90 && Sheltron.CanUse(out act)) return true;

            //30
            if (Sentinel.CanUse(out act)) return true;

            //20
            if (Rampart.CanUse(out act)) return true;

            //10
            if (Bulwark.CanUse(out act)) return true;
            if (Sheltron.CanUse(out act)) return true;
        }

        if (Reprisal.CanUse(out act)) return true;

        return false;
    }
}
