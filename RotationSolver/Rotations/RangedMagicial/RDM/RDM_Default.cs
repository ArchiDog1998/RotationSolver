using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.RangedMagicial.RDM;

internal sealed class RDM_Default : RDM_Base
{
    public override string GameVersion => "6.31";

    public override string RotationName => "Default";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Vercure}"},
        {DescType.DefenseArea, $"{MagickBarrier}"},
        {DescType.MoveAction, $"{CorpsAcorps}"},
    };

    public bool CanStartMeleeCombo 
    {
        get
        {
            if (Player.HasStatus(true, StatusID.Manafication, StatusID.Embolden) ||
                             BlackMana == 100 || WhiteMana == 100) return true;

            //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。

            if (BlackMana == WhiteMana) return false;

            //要求较小的魔元不带触发，也可以强制要求跳过判断。
            if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady))
                {
                    return false;
                }
            }
            if (WhiteMana > BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerfireReady))
                {
                    return false;
                }
            }

            if (Player.HasStatus(true, Vercure.StatusProvide)) return false;

            if (Embolden.WillHaveOneChargeGCD(5)) return false;

            return true;
        }
    }

    static RDM_Default()
    {
        Acceleration.RotationCheck = b => InCombat;
    }

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "Use Vercure for Dualcast");
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Verthunder.CastTime + Service.Configuration.WeaponInterval
            && Verthunder.CanUse(out var act)) return act;

        //Remove Swift
        StatusHelper.StatusOff(StatusID.Dualcast);
        StatusHelper.StatusOff(StatusID.Acceleration);
        StatusHelper.StatusOff(StatusID.Swiftcast);

        return base.CountDownAction(remainTime);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (ManaStacks == 3) return false;

        if (!Verthunder2.CanUse(out _))
        {
            if (Verfire.CanUse(out act)) return true;
            if (Verstone.CanUse(out act)) return true;
        }

        if (Scatter.CanUse(out act)) return true;
        if (WhiteMana < BlackMana)
        {
            if (Veraero2.CanUse(out act) && BlackMana - WhiteMana != 5) return true;
            if (Veraero.CanUse(out act) && BlackMana - WhiteMana != 6) return true;
        }
        else
        {
            if (Verthunder2.CanUse(out act)) return true;
            if (Verthunder.CanUse(out act)) return true;
        }
        if (Jolt.CanUse(out act)) return true;

        if (Configs.GetBool("UseVercure") && Vercure.CanUse(out act)) return true;

        return false;
    }


    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        if (MagickBarrier.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergencyGCD(out IAction act)
    {
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana)
            {
                if (Verholy.CanUse(out act, mustUse: true)) return true;
            }
            if (Verflare.CanUse(out act, mustUse: true)) return true;
        }

        if (Resolution.CanUse(out act, mustUse: true)) return true;
        if (Scorch.CanUse(out act, mustUse: true)) return true;


        if (IsLastGCD(true, Moulinet) && Moulinet.CanUse(out act, mustUse: true)) return true;
        if (Zwerchhau.CanUse(out act)) return true;
        if (Redoublement.CanUse(out act)) return true;

        if (!CanStartMeleeCombo) return false;

        if (Moulinet.CanUse(out act))
        {
            if (BlackMana >= 60 && WhiteMana >= 60) return true;
        }
        else
        {
            if (BlackMana >= 50 && WhiteMana >= 50 && Riposte.CanUse(out act)) return true;
        }
        if (ManaStacks > 0 && Riposte.CanUse(out act)) return true;

        return false;
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        act = null;
        if (CombatElapsedLess(4)) return false;

        if (InBurst && Embolden.CanUse(out act, mustUse: true)) return true;

        //Use Manafication after embolden.
        if ((Player.HasStatus(true, StatusID.Embolden) || IsLastAbility(ActionID.Embolden))
            && Manafication.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //Swift
        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50) 
            &&(CombatElapsedLess(4) || !Manafication.WillHaveOneChargeGCD(0, 1)))
        {
            if(!Player.HasStatus(true, StatusID.VerfireReady, StatusID.VerstoneReady))
            {
                if (Swiftcast.CanUse(out act)) return true;
                if (Acceleration.CanUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if(InBurst && UseTincture(out act)) return true;

        //Attack abilities.
        if (ContreSixte.CanUse(out act, mustUse: true)) return true;
        if (Fleche.CanUse(out act)) return true;

        if (Engagement.CanUse(out act, emptyOrSkipCombo: true)) return true;
        if (CorpsAcorps.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}

