using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    private static void LoadActionConfigAndSetting(ref IBaseAction action)
    {
        //TODO: better target type check. (NoNeed?)
        //TODO: better friendly check.
        //TODO: load the config from the configuration.
    }

    static partial void ModifyAddlePvE(ref IBaseAction action)
    {
        action.ActionCheck = (b, m) => !b.HasStatus(false, StatusID.Addle);
    }

    static partial void ModifySwiftcastPvE(ref IBaseAction action)
    {
        action.Option = ActionOption.Buff;
        action.StatusProvide =
        [
            StatusID.Swiftcast,
            StatusID.Triplecast,
            StatusID.Dualcast,
        ];
    }

    static partial void ModifyEsunaPvE(ref IBaseAction action)
    {
        action.ChoiceTarget = (tars, mustUse) =>
        {
            if (DyingPeople.Any())
            {
                return DyingPeople.OrderBy(ObjectHelper.DistanceToPlayer).First();
            }
            else if (WeakenPeople.Any())
            {
                return WeakenPeople.OrderBy(ObjectHelper.DistanceToPlayer).First();
            }
            return null;
        };
    }

    static partial void ModifyLucidDreamingPvE(ref IBaseAction action)
    {
        action.ActionCheck = (b, m) => Player.CurrentMp < 6000 && InCombat;
    }

    static partial void ModifySecondWindPvE(ref IBaseAction action)
    {
        action.ActionCheck = (b, m) => Player?.GetHealthRatio() < Service.Config.GetValue(Configuration.JobConfigFloat.HealthSingleAbility) && InCombat;
    }

    static partial void ModifyArmsLengthPvE(ref IBaseAction action)
    {
        action.Option = ActionOption.Defense | ActionOption.EndSpecial;
    }

    static partial void ModifyRampartPvE(ref IBaseAction action)
    {
        action.Option = ActionOption.Defense;
        action.StatusProvide =
        [
            StatusID.Superbolide, StatusID.HallowedGround,
            StatusID.Rampart, StatusID.Bulwark,
            StatusID.Bloodwhetting,
            StatusID.Vengeance,
            StatusID.Sentinel,
            StatusID.ShadowWall,
            StatusID.Nebula,
            .. StatusHelper.NoNeedHealingStatus,
        ];
        action.ActionCheck = BaseAction.TankDefenseSelf;
    }

    static partial void ModifyProvokePvE(ref IBaseAction action)
    {
        action.FilterForHostiles = b => TargetFilter.ProvokeTarget(b);
    }

    static partial void ModifyShirkPvE(ref IBaseAction action)
    {
        action.ChoiceTarget = (friends, mustUse) => TargetFilter.GetJobCategory(friends, JobRole.Tank)?.FirstOrDefault();
    }

    static partial void ModifyBloodbathPvE(ref IBaseAction action)
    {
        action.ActionCheck = (t, m) => Player?.GetHealthRatio() < Service.Config.GetValue(Configuration.JobConfigFloat.HealthSingleAbility) && InCombat && HasHostilesInRange;
    }

    static partial void ModifyFeintPvE(ref IBaseAction action)
    {
        action.ActionCheck = (b, m) => !b.HasStatus(false, StatusID.Feint);
    }

    static partial void ModifyInterjectPvE(ref IBaseAction action)
    {
        action.FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt);
    }

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LowBlow { get; } = new RoleAction(ActionID.LowBlow, new JobRole[] { JobRole.Tank })
    //{
    //    FilterForHostiles = bs => bs.Where((Func<BattleChara, bool>)(b =>
    //    {
    //        if (b.IsBossFromIcon() || IsMoving || b.CastActionId == 0) return false;

    //        if (!b.IsCastInterruptible || Interject.IsCoolingDown) return true;
    //        return false;
    //    })),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LegSweep { get; } = new RoleAction(ActionID.LegSweep, new JobRole[] { JobRole.Melee })
    //{
    //    FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction HeadGraze { get; } = new RoleAction(ActionID.HeadGraze, new JobRole[] { JobRole.RangedPhysical })
    //{
    //    FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction SureCast { get; } = new RoleAction(ActionID.Surecast,
    //    new JobRole[] { JobRole.RangedMagical, JobRole.Healer }, ActionOption.Heal);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction TrueNorth { get; } = new RoleAction(ActionID.TrueNorth,
    //    new JobRole[] { JobRole.Melee }, ActionOption.Heal)
    //{
    //    StatusProvide = new StatusID[] { StatusID.TrueNorth, StatusID.RightEye },
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Peloton { get; } = new RoleAction(ActionID.Peloton, new JobRole[] { JobRole.RangedPhysical }, ActionOption.Friendly)
    //{
    //    ActionCheck = (b, m) =>
    //    {
    //        if (!NotInCombatDelay) return false;
    //        var players = PartyMembers.GetObjectInRadius(20);
    //        if (players.Any(ObjectHelper.InCombat)) return false;
    //        return players.Any(p => p.WillStatusEnd(3, false, StatusID.Peloton));
    //    },
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Sprint { get; } = new BaseAction(ActionID.Sprint, ActionOption.Friendly)
    //{
    //    StatusProvide = new StatusID[] { StatusID.Dualcast },
    //};

    private protected virtual IBaseAction Raise => null;
    private protected virtual IBaseAction LimitBreak => null;
    private protected virtual IBaseAction TankStance => null;

    //#endregion

    //#region PvE Limitbreak
    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction ShieldWall { get; } = new RoleAction(ActionID.ShieldWall, new JobRole[] { JobRole.Tank }, ActionOption.Defense)
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 1,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Stronghold { get; } = new RoleAction(ActionID.Stronghold, new JobRole[] { JobRole.Tank }, ActionOption.Defense)
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 2,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction HealingWind { get; } = new RoleAction(ActionID.HealingWind, new JobRole[] { JobRole.Healer }, ActionOption.Heal)
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 1,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction BreathOfTheEarth { get; } = new RoleAction(ActionID.BreathOfTheEarth, new JobRole[] { JobRole.Healer }, ActionOption.Heal)
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 2,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Braver { get; } = new RoleAction(ActionID.Braver, new JobRole[] { JobRole.Melee })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 1,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Bladedance { get; } = new RoleAction(ActionID.Bladedance, new JobRole[] { JobRole.Melee })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 2,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction BigShot { get; } = new RoleAction(ActionID.BigShot, new JobRole[] { JobRole.RangedPhysical })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 1,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Desperado { get; } = new RoleAction(ActionID.Desperado, new JobRole[] { JobRole.RangedPhysical })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 2,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Skyshard { get; } = new RoleAction(ActionID.Skyshard, new JobRole[] { JobRole.RangedMagical })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 1,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction Starstorm { get; } = new RoleAction(ActionID.Starstorm, new JobRole[] { JobRole.RangedMagical })
    //{
    //    ActionCheck = (b, m) => LimitBreakLevel == 2,
    //};
    //#endregion

    //#region Duty Action
    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantRaise { get; } = new RoleAction(ActionID.VariantRaise,
    //    new JobRole[] { JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical, },
    //    ActionOption.Friendly | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantRaise2 { get; } = new RoleAction(ActionID.VariantRaiseIi,
    //[JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical,],
    //ActionOption.Friendly | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantUltimatum { get; } = new BaseAction(ActionID.VariantUltimatum, ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantCure { get; } = new RoleAction(ActionID.VariantCure,
    //    [JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical],
    //    ActionOption.Heal | ActionOption.DutyAction | ActionOption.EndSpecial);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantCure2 { get; } = new RoleAction(ActionID.VariantCure_33862,
    //    [JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical],
    //    ActionOption.Heal | ActionOption.DutyAction | ActionOption.EndSpecial);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantSpiritDart { get; } = new RoleAction(ActionID.VariantSpiritDart,
    //    [JobRole.Healer, JobRole.Tank], ActionOption.Dot | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantSpiritDart2 { get; } = new RoleAction(ActionID.VariantSpiritDart_33863,
    //    [JobRole.Healer, JobRole.Tank], ActionOption.Dot | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantRampart { get; } = new RoleAction(ActionID.VariantRampart,
    //    [JobRole.Melee, JobRole.Healer, JobRole.RangedMagical, JobRole.RangedPhysical,], ActionOption.Buff | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction VariantRampart2 { get; } = new RoleAction(ActionID.VariantRampart_33864,
    //    [JobRole.Melee, JobRole.Healer, JobRole.RangedMagical, JobRole.RangedPhysical], ActionOption.Buff | ActionOption.DutyAction);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostSpellforge { get; } = new BaseAction(ActionID.LostSpellforge,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostSpellforge],
    //    ActionCheck = (b, m) => LostSpellforge.Target?.HasStatus(false, StatusID.MagicalAversion) ?? false,
    //    ChoiceTarget = (targets, mustUse) => targets.FirstOrDefault(t => (Job)t.ClassJob.Id switch
    //    {
    //        Job.WAR
    //        or Job.GNB
    //        or Job.MNK
    //        or Job.SAM
    //        or Job.DRG
    //        or Job.MCH
    //        or Job.DNC

    //        or Job.PLD
    //        or Job.DRK
    //        or Job.NIN
    //        or Job.BRD
    //        or Job.RDM
    //        => true,

    //        _ => false,
    //    }),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostSteelsting { get; } = new BaseAction(ActionID.LostSteelsting,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostSteelsting],
    //    ActionCheck = (b, m) => LostSteelsting.Target?.HasStatus(false, StatusID.PhysicalAversion) ?? false,
    //    ChoiceTarget = (targets, mustUse) => targets.FirstOrDefault(t => (Job)t.ClassJob.Id switch
    //    {
    //        Job.WHM
    //        or Job.SCH
    //        or Job.AST
    //        or Job.SGE
    //        or Job.BLM
    //        or Job.SMN

    //        or Job.PLD
    //        or Job.DRK
    //        or Job.NIN
    //        or Job.BRD
    //        or Job.RDM
    //        => true,

    //        _ => false,
    //    }),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostRampage { get; } = new BaseAction(ActionID.LostRampage,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostRampage],
    //    ActionCheck = (b, m) => LostRampage.Target?.HasStatus(false, StatusID.PhysicalAversion) ?? false,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostBurst { get; } = new BaseAction(ActionID.LostBurst,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostBurst],
    //    ActionCheck = (b, m) => LostBurst.Target?.HasStatus(false, StatusID.MagicalAversion) ?? false,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostBravery { get; } = new BaseAction(ActionID.LostBravery,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostBravery],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostProtect { get; } = new BaseAction(ActionID.LostProtect,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostProtect, StatusID.LostProtectIi],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostShell { get; } = new BaseAction(ActionID.LostShell,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostShell, StatusID.LostShellIi],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostProtect2 { get; } = new BaseAction(ActionID.LostProtectIi,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostProtectIi],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostShell2 { get; } = new BaseAction(ActionID.LostShellIi,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostShellIi],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostBubble { get; } = new BaseAction(ActionID.LostBubble,
    //    ActionOption.DutyAction | ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.LostBubble],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostStoneskin { get; } = new BaseAction(ActionID.LostStoneskin,
    //    ActionOption.DutyAction | ActionOption.Defense)
    //{
    //    ChoiceTarget = TargetFilter.FindAttackedTarget,
    //    StatusProvide = [StatusID.Stoneskin],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostStoneskin2 { get; } = new BaseAction(ActionID.LostStoneskinIi,
    //    ActionOption.DutyAction | ActionOption.Defense)
    //{
    //    StatusProvide = [StatusID.Stoneskin],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostFlarestar { get; } = new BaseAction(ActionID.LostFlareStar,
    //ActionOption.DutyAction)
    //{
    //    StatusProvide = [StatusID.LostFlareStar],
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction LostSeraphStrike { get; } = new BaseAction(ActionID.LostSeraphStrike,
    //    ActionOption.DutyAction)
    //{
    //    StatusProvide = [StatusID.ClericStance_2484],
    //};
    //#endregion

    //#region PvP
    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction PvP_StandardIssueElixir { get; } = new BaseAction(ActionID.StandardissueElixir, ActionOption.Heal)
    //{
    //    ActionCheck = (t, m) => !HasHostilesInMaxRange
    //        && (t.CurrentMp <= t.MaxMp / 3 || t.CurrentHp <= t.MaxHp / 3)
    //        && !IsLastAction(ActionID.StandardissueElixir),
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction PvP_Recuperate { get; } = new BaseAction(ActionID.Recuperate, ActionOption.Heal)
    //{
    //    ActionCheck = (t, m) => t.MaxHp - t.CurrentHp > 15000,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction PvP_Purify { get; } = new BaseAction(ActionID.Purify_29056, ActionOption.Heal)
    //{
    //    ActionCheck = (t, m) => Player?.StatusList.Any(s => s.GameData.CanDispel) ?? false,
    //};

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction PvP_Guard { get; } = new BaseAction(ActionID.Guard, ActionOption.Defense);

    ///// <summary>
    ///// 
    ///// </summary>
    //public static IBaseAction PvP_Sprint { get; } = new BaseAction(ActionID.Sprint_29057, ActionOption.Friendly)
    //{
    //    StatusProvide = [StatusID.Sprint_1342],
    //};
    //#endregion

    IBaseAction[] _allBaseActions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public virtual IBaseAction[] AllBaseActions => _allBaseActions ??= GetBaseActions(GetType()).ToArray();

    IAction[] _allActions;
    public IAction[] AllActions => _allActions ??= Array.Empty<IAction>().Union(GetBaseItems(GetType())).Union(AllBaseActions).ToArray();

    IBaseTrait[] _allTraits;
    public IBaseTrait[] AllTraits => _allTraits ??= GetIEnoughLevel<IBaseTrait>(GetType()).ToArray();

    PropertyInfo[] _allBools;
    public PropertyInfo[] AllBools => _allBools ??= GetType().GetStaticProperties<bool>();

    PropertyInfo[] _allBytes;
    public PropertyInfo[] AllBytesOrInt => _allBytes ??= GetType().GetStaticProperties<byte>().Union(GetType().GetStaticProperties<int>()).ToArray();

    PropertyInfo[] _allFloats;
    public PropertyInfo[] AllFloats => _allFloats ??= GetType().GetStaticProperties<float>();

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    private IEnumerable<IBaseAction> GetBaseActions(Type type)
    {
        return GetIEnoughLevel<IBaseAction>(type);
    }

    private IEnumerable<IBaseItem> GetBaseItems(Type type)
    {
        return GetIEnoughLevel<IBaseItem>(type).Where(a => a is not MedicineItem medicine || medicine.InType(this)).Reverse();
    }

    private IEnumerable<T> GetIEnoughLevel<T>(Type type) where T : IEnoughLevel
    {
        if (type == null) return Array.Empty<T>();

        var acts = from prop in type.GetProperties()
                   where typeof(T).IsAssignableFrom(prop.PropertyType) && !(prop.GetMethod?.IsPrivate ?? true)
                   select (T)prop.GetValue(this) into act
                   where act != null
                   orderby act.Level
                   select act;

        return acts.Union(GetIEnoughLevel<T>(type.BaseType));
    }
}
