using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;

namespace RotationSolver.Rotations.Tank.PLD;

[RotationDesc("The whole rotation's burst\nis base on:")]
[RotationDesc(ActionID.FightorFlight)]
internal class PLD_Default : PLD_Base
{
    public override string GameVersion => "6.31";
    public override string RotationName => "Default";

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
            if (InBurst && !CombatElapsedLess(5) && FightorFlight.CanUse(out act)) return true;
        }
        if (CombatElapsedLess(8)) return false;

        if (CircleofScorn.CanUse(out act, mustUse: true)) return true;
        if (Expiacion.CanUse(out act, mustUse: true)) return true;
        if (SpiritsWithin.CanUse(out act, mustUse: true)) return true;

        if (Player.WillStatusEndGCD(6, 0, true, StatusID.FightOrFlight)
            && Requiescat.CanUse(out act, mustUse: true)) return true;

        if (!IsMoving && Intervene.CanUse(out act, true, HasFightOrFlight)) return true;

        if (HasTankStance && OathGauge == 100 && Sheltron.CanUse(out act)) return true;

        return false;
    }

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
        if (HasDivineMight && HolyCircle.CanUse(out act)) return true;
        if (Prominence.CanUse(out act)) return true;
        if (TotalEclipse.CanUse(out act)) return true;

        //Single
        if (!CombatElapsedLess(8) && HasFightOrFlight && GoringBlade.CanUse(out act)) return true; // Dot
        if (!FightorFlight.WillHaveOneChargeGCD(2))
        {
            if (!FightorFlight.WillHaveOneChargeGCD(6) && 
                HasDivineMight && HolySpirit.CanUse(out act)) return true;
            if (RageofHalone.CanUse(out act)) return true;
            if (Atonement.CanUse(out act)) return true;
        }
        //123
        if (RageofHalone.CanUse(out act)) return true;
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
