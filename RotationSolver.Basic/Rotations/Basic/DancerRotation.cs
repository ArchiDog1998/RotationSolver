namespace RotationSolver.Basic.Rotations.Basic;

partial class DancerRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// 
    /// </summary>
    public static bool IsDancing => JobGauge.IsDancing;

    /// <summary>
    /// 
    /// </summary>
    public static byte Esprit => JobGauge.Esprit;

    /// <summary>
    /// 
    /// </summary>
    public static byte Feathers => JobGauge.Feathers;

    /// <summary>
    /// 
    /// </summary>
    public static byte CompletedSteps => JobGauge.CompletedSteps;

    static partial void ModifyCascadePvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SilkenSymmetry];
    }

    static partial void ModifyFountainPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SilkenFlow];
    }

    static partial void ModifyReverseCascadePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SilkenSymmetry, StatusID.FlourishingSymmetry];
    }

    static partial void ModifyFountainfallPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SilkenFlow, StatusID.FlourishingFlow];
    }

    static partial void ModifyFanDancePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Feathers > 0;
        setting.StatusProvide = [StatusID.ThreefoldFanDance];
    }

    static partial void ModifyWindmillPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SilkenSymmetry];
    }

    static partial void ModifyBladeshowerPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.SilkenFlow];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyRisingWindmillPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SilkenSymmetry, StatusID.FlourishingSymmetry];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyBloodshowerPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.SilkenFlow, StatusID.FlourishingFlow];
    }

    static partial void ModifyFanDanceIiPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Feathers > 0;
        setting.StatusProvide = [StatusID.ThreefoldFanDance];
        setting.CreateConfig = () => new()
        {
            AoeCount = 2,
        };
    }

    static partial void ModifyFanDanceIiiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.ThreefoldFanDance];
    }

    static partial void ModifyFanDanceIvPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.FourfoldFanDance];
    }

    static partial void ModifySaberDancePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Esprit >= 50;
    }

    static partial void ModifyStarfallDancePvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.FlourishingStarfall];
    }

    static partial void ModifyTillanaPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.FlourishingFinish];
    }

    static partial void ModifyEnAvantPvE(ref ActionSetting setting)
    {
        setting.IsFriendly = true;
    }

    static partial void ModifyEnAvantPvP(ref ActionSetting setting)
    {
        setting.SpecialType = SpecialActionType.MovingForward;
    }

    static partial void ModifyShieldSambaPvE(ref ActionSetting setting)
    {
        setting.StatusFromSelf = false;
        setting.StatusProvide = StatusHelper.RangePhysicalDefense;
    }

    static partial void ModifyClosedPositionPvE(ref ActionSetting setting)
    {
        setting.TargetType = TargetType.Melee;
        setting.ActionCheck = () => !AllianceMembers.Any(b => b.HasStatus(true, StatusID.ClosedPosition_2026));
    }

    static partial void ModifyDevilmentPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyFlourishPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.StandardFinish];
        setting.StatusProvide = [StatusID.ThreefoldFanDance, StatusID.FourfoldFanDance];
        setting.ActionCheck = () => InCombat;
    }

    static partial void ModifyTechnicalStepPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.StandardFinish];
        setting.UnlockedByQuestID = 68790;
    }

    static partial void ModifyDoubleTechnicalFinishPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.StandardStep, StatusID.TechnicalStep];
        setting.CreateConfig = () => new()
        {
            TimeToKill = 20,
        };
    }

    static partial void ModifyDoubleStandardFinishPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.StandardStep];
        setting.ActionCheck = () => IsDancing && CompletedSteps == 2 && Service.GetAdjustedActionId(ActionID.StandardStepPvE) == ActionID.DoubleStandardFinishPvE;
    }

    static partial void ModifyQuadrupleTechnicalFinishPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.TechnicalStep];
        setting.ActionCheck = () => IsDancing && CompletedSteps == 4 && Service.GetAdjustedActionId(ActionID.TechnicalStepPvE) == ActionID.QuadrupleTechnicalFinishPvE;
    }

    static partial void ModifyEmboitePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (ActionID)JobGauge.NextStep == ActionID.EmboitePvE;
    }

    static partial void ModifyEntrechatPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (ActionID)JobGauge.NextStep == ActionID.EntrechatPvE;
    }

    static partial void ModifyJetePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (ActionID)JobGauge.NextStep == ActionID.JetePvE;
    }

    static partial void ModifyPirouettePvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => (ActionID)JobGauge.NextStep == ActionID.PirouettePvE;
    }

    #region Step
    /// <summary>
    /// Finish the dance.
    /// </summary>
    /// <param name="act"></param>
    /// <param name="finishNow">Finish the dance as soon as possible</param>
    /// <returns></returns>
    protected bool DanceFinishGCD(out IAction? act, bool finishNow = false)
    {
        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2)
        {
            if (DoubleStandardFinishPvE.CanUse(out act, skipAoeCheck: true))
            {
                return true;
            }
            if (Player.WillStatusEnd(1, true, StatusID.StandardStep, StatusID.StandardFinish) || finishNow)
            {
                act = StandardStepPvE;
                return true;
            }
            return false;
        }

        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4)
        {
            if (QuadrupleTechnicalFinishPvE.CanUse(out act, skipAoeCheck: true))
            {
                return true;
            }
            if (Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || finishNow)
            {
                act = TechnicalStepPvE;
                return true;
            }
            return false;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// Do the dancing steps.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected bool ExecuteStepGCD(out IAction? act)
    {
        if (!IsDancing)
        {
            act = null;
            return false;
        }

        if (EmboitePvE.CanUse(out act)) return true;
        if (EntrechatPvE.CanUse(out act)) return true;
        if (JetePvE.CanUse(out act)) return true;
        if (PirouettePvE.CanUse(out act)) return true;
        return false;
    }
    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <param name="nextGCD"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.EnAvantPvE)]
    protected sealed override bool MoveForwardAbility(IAction nextGCD, out IAction act)
    {
        if (EnAvantPvE.CanUse(out act, usedUp:true)) return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <param name="nextGCD"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.CuringWaltzPvE, ActionID.ImprovisationPvE)]
    protected sealed override bool HealAreaAbility(IAction nextGCD, out IAction act)
    {
        if (CuringWaltzPvE.CanUse(out act, usedUp: true)) return true;
        if (ImprovisationPvE.CanUse(out act, usedUp: true)) return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <param name="nextGCD"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.ShieldSambaPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction act)
    {
        if (ShieldSambaPvE.CanUse(out act, usedUp: true)) return true;
        return false;
    }
}
