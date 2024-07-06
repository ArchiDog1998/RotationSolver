using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations.Basic;

namespace DefaultRotations.Melee;

[Rotation("Default", CombatType.PvE, GameVersion = "7.0")]
[SourceCode(Path = "main/DefaultRotations/Melee/VPR_Default.cs")]
[Api(2)]
public sealed class VPR_Default : ViperRotation
{
    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    /*protected override IAction? CountDownAction(float remainTime)
    {

    }*/
    #endregion

    #region Emergency Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (InCombat && SerpentsIrePvE.CanUse(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        if (SerpentsTailPvE.CanUse(out act)) return true;
        if (TwinfangPvE.CanUse(out act)) return true;
        if (TwinbloodPvE.CanUse(out act)) return true;


        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        act = null;

        if (SlitherPvE.CanUse(out act)) return true;

        return base.MoveForwardAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        return base.GeneralGCD(out act);
    }

    private bool AttackGCD(out IAction? act, bool burst)
    {
        if (HindstingStrikePvE.CanUse(out act)) return true;
        if (HindsbaneFangPvE.CanUse(out act)) return true;
        if (FlankstingStrikePvE.CanUse(out act)) return true;
        if (FlanksbaneFangPvE.CanUse(out act)) return true;
        if (SwiftskinsStingPvE.CanUse(out act)) return true;
        if (HuntersStingPvE.CanUse(out act)) return true;
        if (DreadFangsPvE.CanUse(out act)) return true;
        if (SteelFangsPvE.CanUse(out act)) return true;
        if (WrithingSnapPvE.CanUse(out act)) return true;
        return false;
        //return base.AttackGCD(out act);
    }
    #endregion

    #region Extra Methods
    // Extra private helper methods for determining the usability of specific abilities under certain conditions.
    // These methods simplify the main logic by encapsulating specific checks related to abilities' cooldowns and prerequisites.
    private bool CanUseExamplePvE(out IAction? act)
    {
        act = null;
        return false;
    }
    #endregion
}