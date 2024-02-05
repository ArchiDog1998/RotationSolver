//using RotationSolver.Basic.Traits;

//namespace RotationSolver.Basic.Rotations.Basic;

//partial class DancerRotation
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public override MedicineType MedicineType => MedicineType.Dexterity;

//    /// <summary>
//    /// 
//    /// </summary>
//    public static bool IsDancing => JobGauge.IsDancing;

//    /// <summary>
//    /// 
//    /// </summary>
//    public static byte Esprit => JobGauge.Esprit;

//    /// <summary>
//    /// 
//    /// </summary>
//    public static byte Feathers => JobGauge.Feathers;

//    /// <summary>
//    /// 
//    /// </summary>
//    public static byte CompletedSteps => JobGauge.CompletedSteps;

//    static partial void ModifyCascadePvE(ref ActionSetting setting)
//    {
//        setting.StatusProvide = [StatusID.SilkenSymmetry];
//    }

//    static partial void ModifyFountainPvE(ref ActionSetting setting)
//    {
//        setting.StatusProvide = [StatusID.SilkenFlow];
//    }

//    static partial void ModifyReverseCascadePvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.SilkenSymmetry, StatusID.FlourishingSymmetry];
//    }

//    static partial void ModifyFountainfallPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.SilkenFlow, StatusID.FlourishingFlow];
//    }

//    static partial void ModifyFanDancePvE(ref ActionSetting setting)
//    {
//        setting.ActionCheck = () => Feathers > 0;
//        setting.StatusProvide = [StatusID.ThreefoldFanDance];
//    }

//    static partial void ModifyWindmillPvE(ref ActionSetting setting)
//    {
//        setting.StatusProvide = [StatusID.SilkenSymmetry];
//    }

//    static partial void ModifyBladeshowerPvE(ref ActionSetting setting)
//    {
//        setting.StatusProvide = [StatusID.SilkenFlow];
//    }

//    static partial void ModifyRisingWindmillPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.SilkenSymmetry, StatusID.FlourishingSymmetry];
//    }

//    static partial void ModifyBloodshowerPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.SilkenFlow, StatusID.FlourishingFlow];
//    }

//    static partial void ModifyFanDanceIiPvE(ref ActionSetting setting)
//    {
//        setting.ActionCheck = () => Feathers > 0;
//        setting.StatusProvide = [StatusID.ThreefoldFanDance];
//    }

//    static partial void ModifyFanDanceIiiPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.ThreefoldFanDance];
//    }

//    static partial void ModifyFanDanceIvPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.FourfoldFanDance];
//    }

//    static partial void ModifySaberDancePvE(ref ActionSetting setting)
//    {
//        setting.ActionCheck = () => Esprit >= 50;
//    }

//    static partial void ModifyStarfallDancePvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.FlourishingStarfall];
//    }

//    static partial void ModifyTillanaPvE(ref ActionSetting setting)
//    {
//        setting.StatusNeed = [StatusID.FlourishingFinish];
//    }

//    static partial void ModifyEnAvantPvE(ref ActionSetting setting)
//    {
//        setting.IsFriendly = true;
//    }

//    static partial void ModifyShieldSambaPvE(ref ActionSetting setting)
//    {
//        setting.StatusFromSelf = false;
//        setting.StatusProvide = StatusHelper.RangePhysicalDefense;
//    }

//    static partial void ModifyClosedPositionPvE(ref ActionSetting setting)
//    {
//        setting.TargetType = TargetType.Melee;
//    }
//}

///// <summary>
///// The base class of Dancer.
///// </summary>
//public abstract class DdRotation : CustomRotation
//{
//    #region Support
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction ClosedPosition { get; } = new BaseAction(ActionID.ClosedPosition, ActionOption.Buff)
//    {
//        ChoiceTarget = (Targets, mustUse) =>
//        {
//            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
//            //Remove Weak
//            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)
//            //Remove other partner.
//            && (!b.HasStatus(false, StatusID.ClosedPosition_2026) || b.HasStatus(true, StatusID.ClosedPosition_2026)));

//            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical).FirstOrDefault();
//        },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction Devilment { get; } = new BaseAction(ActionID.Devilment)
//    {
//        ActionCheck = (b, m) => IsLongerThan(10)
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction Flourish { get; } = new BaseAction(ActionID.Flourish)
//    {
//        StatusNeed = new[] { StatusID.StandardFinish },
//        StatusProvide = new[]
//        {
//            StatusID.ThreefoldFanDance,
//            StatusID.FourfoldFanDance,
//        },
//        ActionCheck = (b, m) => InCombat,
//    };
//    #endregion

//    #region Step
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction StandardStep { get; } = new BaseAction(ActionID.StandardStep)
//    {
//        StatusProvide = new[]
//        {
//            StatusID.StandardStep,
//            StatusID.TechnicalStep,
//        },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction TechnicalStep { get; } = new BaseAction(ActionID.TechnicalStep)
//    {
//        StatusNeed = new[]
//        {
//            StatusID.StandardFinish,
//        },
//        StatusProvide = StandardStep.StatusProvide,
//        ActionCheck = (b, m) => IsLongerThan(20),
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    protected static IBaseAction StandardFinish { get; } = new BaseAction(ActionID.DoubleStandardFinish)
//    {
//        StatusNeed = new[] { StatusID.StandardStep },
//        ActionCheck = (b, m) => IsDancing && CompletedSteps == 2 && Service.GetAdjustedActionId(ActionID.StandardStep) == ActionID.DoubleStandardFinish,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    protected static IBaseAction TechnicalFinish { get; } = new BaseAction(ActionID.QuadrupleTechnicalFinish)
//    {
//        StatusNeed = new[] { StatusID.TechnicalStep },
//        ActionCheck = (b, m) => IsDancing && CompletedSteps == 4 && Service.GetAdjustedActionId(ActionID.TechnicalStep) == ActionID.QuadrupleTechnicalFinish,
//    };

//    private static IBaseAction Emboite { get; } = new BaseAction(ActionID.Emboite)
//    {
//        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Emboite,
//    };

//    private static IBaseAction Entrechat { get; } = new BaseAction(ActionID.Entrechat)
//    {
//        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
//    };

//    private static IBaseAction Jete { get; } = new BaseAction(ActionID.Jete)
//    {
//        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Jete,
//    };

//    private static IBaseAction Pirouette { get; } = new BaseAction(ActionID.Pirouette)
//    {
//        ActionCheck = (b, m) => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
//    };

//    /// <summary>
//    /// Finish the dance.
//    /// </summary>
//    /// <param name="act"></param>
//    /// <param name="finishNow">Finish the dance as soon as possible</param>
//    /// <returns></returns>
//    protected static bool DanceFinishGCD(out IAction act, bool finishNow = false)
//    {
//        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2)
//        {
//            if (StandardFinish.CanUse(out act, CanUseOption.MustUse))
//            {
//                return true;
//            }
//            if (Player.WillStatusEnd(1, true, StatusID.StandardStep, StatusID.StandardFinish) || finishNow)
//            {
//                act = StandardStep;
//                return true;
//            }
//            return false;
//        }

//        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4)
//        {
//            if (TechnicalFinish.CanUse(out act, CanUseOption.MustUse))
//            {
//                return true;
//            }
//            if (Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || finishNow)
//            {
//                act = TechnicalStep;
//                return true;
//            }
//            return false;
//        }

//        act = null;
//        return false;
//    }

//    /// <summary>
//    /// Do the dancing steps.
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    protected static bool ExecuteStepGCD(out IAction act)
//    {
//        if (!IsDancing)
//        {
//            act = null;
//            return false;
//        }

//        if (Emboite.CanUse(out act)) return true;
//        if (Entrechat.CanUse(out act)) return true;
//        if (Jete.CanUse(out act)) return true;
//        if (Pirouette.CanUse(out act)) return true;
//        return false;
//    }
//    #endregion

//    #region Traits
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait IncreasedActionDamage { get; } = new BaseTrait(251);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait FourfoldFantasy { get; } = new BaseTrait(252);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait IncreasedActionDamage2 { get; } = new BaseTrait(253);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedEnAvant { get; } = new BaseTrait(254);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EspritTrait { get; } = new BaseTrait(255);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedEnAvant2 { get; } = new BaseTrait(256);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedTechnicalFinish { get; } = new BaseTrait(453);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedEsprit { get; } = new BaseTrait(454);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedFlourish { get; } = new BaseTrait(455);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedShieldSamba { get; } = new BaseTrait(456);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseTrait EnhancedDevilment { get; } = new BaseTrait(457);

//    #endregion

//    private protected override IBaseAction LimitBreak => CrimsonLotus;

//    /// <summary>
//    /// LB
//    /// </summary>
//    public static IBaseAction CrimsonLotus { get; } = new BaseAction(ActionID.CrimsonLotus)
//    {
//        ActionCheck = (b, m) => LimitBreakLevel == 3,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    [RotationDesc(ActionID.EnAvant)]
//    protected sealed override bool MoveForwardAbility(out IAction act)
//    {
//        if (EnAvant.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        return false;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    [RotationDesc(ActionID.CuringWaltz, ActionID.Improvisation)]
//    protected sealed override bool HealAreaAbility(out IAction act)
//    {
//        if (CuringWaltz.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        if (Improvisation.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        return false;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    [RotationDesc(ActionID.ShieldSamba)]
//    protected sealed override bool DefenseAreaAbility(out IAction act)
//    {
//        if (ShieldSamba.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
//        return false;
//    }
//}
