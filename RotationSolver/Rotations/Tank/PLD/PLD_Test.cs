using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Rotations.Tank.PLD;

internal class PLD_Test : PLD_Base
{
    public override string GameVersion => "6.31";
    public override string RotationName => "Test";

    public override string Description => "NO PRE-PULL FOF OPENER";
    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseDivineVeilPre", false, "DivineVeilPre in 15 seconds counting down.");
    }
    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= HolySpirit.CastTime + Service.Configuration.CountDownAhead
            && HolySpirit.CanUse(out var act)) return act;

        if (Configs.GetBool("UseDivineVeilPre") && remainTime <= 15
            && DivineVeil.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (abilitiesRemaining == 1 && nextGCD.IsTheSameTo(true, RiotBlade, GoringBlade))
        {
            if (FightorFlight.CanUse(out act)) return true;
        }
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if(CombatElapsedLess(6) )
        {
            if(FightorFlight.CanUse(out act)) return true;
        }
        else if(InBurst)
        {
            if(UseBurstMedicine(out act)) return true;
        }

        if (CircleofScorn.CanUse(out act, mustUse: true)) return true;
        if (SpiritsWithin.CanUse(out act, mustUse: true)) return true;
        if (Expiacion.CanUse(out act, mustUse: true)) return true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (BladeofValor.CanUse(out act, mustUse: true)) return true;
        if (BladeofTruth.CanUse(out act, mustUse: true)) return true;
        if (BladeofFaith.CanUse(out act, mustUse: true)) return true;
        if (Confiteor.CanUse(out act, mustUse: true)) return true;

        //AOE
        if (Player.HasStatus(true, StatusID.Requiescat)
            && HolyCircle.CanUse(out act)) return true;
        if (Prominence.CanUse(out act)) return true;
        if (TotalEclipse.CanUse(out act)) return true;

        //Single
        if (GoringBlade.CanUse(out act)) return true; // Dot
        if (RageofHalone.CanUse(out act)) return true;
        if (RiotBlade.CanUse(out act)) return true;
        if (FastBlade.CanUse(out act)) return true;

        //Range
        if (HolySpirit.CanUse(out act)) return true;
        if (ShieldLob.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Reprisal, ActionID.DivineVeil, ActionID.PassageofArms)]
    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Reprisal.CanUse(out act, mustUse: true)) return true;
        if (DivineVeil.CanUse(out act)) return true;
        if (PassageofArms.CanUse(out act)) return true;
        return false;
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
