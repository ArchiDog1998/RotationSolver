using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.Melee.DRG;

internal sealed class DRG_Default : DRG_Base
{
    public override string GameVersion => "6.18";

    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("DRG_ShouldDelay", true, "Delay the dragon?")
            .SetBool("DRG_Opener", false, "Opener in lv.88")
            .SetBool("DRG_SafeMove", true, "Moving save");
    }

    [RotationDesc(ActionID.SpineshatterDive, ActionID.DragonfireDive)]
    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain > 1)
        {
            if (SpineshatterDive.CanUse(out act, emptyOrSkipCombo: true)) return true;
            if (DragonfireDive.CanUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsTheSameTo(true, FullThrust, CoerthanTorment)
            || Player.HasStatus(true, StatusID.LanceCharge) && nextGCD.IsTheSameTo(false, FangandClaw))
        {
            //龙剑
            if (abilityRemain == 1 && LifeSurge.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            //猛枪
            if (LanceCharge.CanUse(out act, mustUse: true))
            {
                if (abilitiesRemaining == 1 && !Player.HasStatus(true, StatusID.PowerSurge)) return true;
                if (Player.HasStatus(true, StatusID.PowerSurge)) return true;
            }

            //巨龙视线
            if (DragonSight.CanUse(out act, mustUse: true)) return true;

            //战斗连祷
            if (BattleLitany.CanUse(out act, mustUse: true)) return true;
        }

        //死者之岸
        if (Nastrond.CanUse(out act, mustUse: true)) return true;

        //坠星冲
        if (Stardiver.CanUse(out act, mustUse: true)) return true;

        //高跳
        if (HighJump.EnoughLevel)
        {
            if (HighJump.CanUse(out act)) return true;
        }
        else
        {
            if (Jump.CanUse(out act)) return true;
        }

        //尝试进入红龙血
        if (Geirskogul.CanUse(out act, mustUse: true)) return true;

        //破碎冲
        if (SpineshatterDive.CanUse(out act, emptyOrSkipCombo: true))
        {
            if (Player.HasStatus(true, StatusID.LanceCharge) && LanceCharge.ElapsedAfterGCD(3)) return true;
        }
        if (Player.HasStatus(true, StatusID.PowerSurge) && SpineshatterDive.CurrentCharges != 1 && SpineshatterDive.CanUse(out act)) return true;

        //幻象冲
        if (MirageDive.CanUse(out act)) return true;

        //龙炎冲
        if (DragonfireDive.CanUse(out act, mustUse: true))
        {
            if (Player.HasStatus(true, StatusID.LanceCharge) && LanceCharge.ElapsedAfterGCD(3)) return true;
        }

        //天龙点睛
        if (WyrmwindThrust.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        #region 群伤
        if (CoerthanTorment.CanUse(out act)) return true;
        if (SonicThrust.CanUse(out act)) return true;
        if (DoomSpike.CanUse(out act)) return true;

        #endregion

        #region 单体
        if (Configs.GetBool("ShouldDelay"))
        {
            if (WheelingThrust.CanUse(out act)) return true;
            if (FangandClaw.CanUse(out act)) return true;
        }
        else
        {
            if (FangandClaw.CanUse(out act)) return true;
            if (WheelingThrust.CanUse(out act)) return true;
        }

        if (FullThrust.CanUse(out act)) return true;
        if (ChaosThrust.CanUse(out act)) return true;

        //看看是否需要续Buff
        if (Player.WillStatusEndGCD(5, 0, true, StatusID.PowerSurge))
        {
            if (Disembowel.CanUse(out act)) return true;
        }

        if (VorpalThrust.CanUse(out act)) return true;
        if (TrueThrust.CanUse(out act)) return true;

        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (PiercingTalon.CanUse(out act)) return true;

        return false;

        #endregion
    }
}
