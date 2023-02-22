using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Tank.WAR;

internal sealed class WAR_Default : WAR_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    static WAR_Default()
    {
        InnerBeast.RotationCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest);
    }

    [RotationDesc(ActionID.ShakeItOff, ActionID.Reprisal)]
    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShakeItOff.CanUse(out act, mustUse: true)) return true;
        if (Reprisal.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    [RotationDesc(ActionID.PrimalRend)]
    private protected override bool MoveForwardGCD(out IAction act)
    {
        if (PrimalRend.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //¸ã¸ã¹¥»÷
        if (PrimalRend.CanUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //ÊÞ»êÊä³ö
        //¸ÖÌúÐý·ç
        if (SteelCyclone.CanUse(out act)) return true;
        //Ô­³õÖ®»ê
        if (InnerBeast.CanUse(out act)) return true;

        //ÈºÌå
        if (MythrilTempest.CanUse(out act)) return true;
        if (Overpower.CanUse(out act)) return true;

        //µ¥Ìå
        if (StormsEye.CanUse(out act)) return true;
        if (StormsPath.CanUse(out act)) return true;
        if (Maim.CanUse(out act)) return true;
        if (HeavySwing.CanUse(out act)) return true;

        //¹»²»×Å£¬Ëæ±ã´òÒ»¸ö°É¡£
        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Tomahawk.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.RawIntuition, ActionID.Vengeance, ActionID.Rampart, ActionID.RawIntuition, ActionID.Reprisal)]
    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            if (TargetUpdater.HostileTargets.Count() > 1)
            {
                //Ô­³õµÄÖ±¾õ£¨¼õÉË10%£©
                if (RawIntuition.CanUse(out act)) return true;
            }

            //¸´³ð£¨¼õÉË30%£©
            if (Vengeance.CanUse(out act)) return true;

            //Ìú±Ú£¨¼õÉË20%£©
            if (Rampart.CanUse(out act)) return true;

            //Ô­³õµÄÖ±¾õ£¨¼õÉË10%£©
            if (RawIntuition.CanUse(out act)) return true;
        }
        //½µµÍ¹¥»÷
        //Ñ©³ð
        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //±¬·¢
        if (!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            //¿ñ±©
            if (!InnerRelease.IsCoolingDown && Berserk.CanUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            //Õ½Àõ
            if (ThrillofBattle.CanUse(out act)) return true;
            //Ì©È»×ÔÈô ×ÔÄÌ°¡£¡
            if (Equilibrium.CanUse(out act)) return true;
        }

        //ÄÌ¸ö¶ÓÓÑ°¡¡£
        if (!HasTankStance && NascentFlash.CanUse(out act)) return true;

        //Õ½º¿
        if (Infuriate.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //ÆÕÍ¨¹¥»÷
        //ÈºÉ½Â¡Æð
        if (Orogeny.CanUse(out act)) return true;
        //¶¯ÂÒ 
        if (Upheaval.CanUse(out act)) return true;

        //¸ã¸ã¹¥»÷
        if (Onslaught.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}
