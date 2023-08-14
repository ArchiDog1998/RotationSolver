namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    internal class RoleAction : BaseAction
    {
        private readonly JobRole[] _roles;
        internal RoleAction(ActionID actionID, JobRole[] roles, ActionOption option = ActionOption.None) 
            : base(actionID, option)
        {
            _roles = roles;
        }

        internal bool InRole(JobRole role) => _roles.Contains(role);
    }

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Addle { get; } = new RoleAction(ActionID.Addle, new JobRole[] { JobRole.RangedMagical }, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !b.HasStatus(false, StatusID.Addle),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Swiftcast { get; } = new RoleAction(ActionID.SwiftCast, new JobRole[] { JobRole.RangedMagical, JobRole.Healer }, ActionOption.Buff)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.SwiftCast,
            StatusID.TripleCast,
            StatusID.DualCast,
        }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Esuna { get; } = new RoleAction(ActionID.Esuna, new JobRole[] { JobRole.Healer }, ActionOption.Heal)
    {
        ChoiceTarget = (tars, mustUse) =>
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
        },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Rescue { get; } = new RoleAction(ActionID.Rescue, new JobRole[] { JobRole.Healer }, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Repose { get; } = new RoleAction(ActionID.Repose, new JobRole[] { JobRole.Healer });

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LucidDreaming { get; } = new RoleAction(ActionID.LucidDreaming,
        new JobRole[] { JobRole.Healer, JobRole.RangedMagical }, ActionOption.Buff)
    {
        ActionCheck = (b, m) => Player.CurrentMp < 6000 && InCombat,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SecondWind { get; } = new RoleAction(ActionID.SecondWind,
        new JobRole[] { JobRole.RangedPhysical, JobRole.Melee }, ActionOption.Heal)
    {
        ActionCheck = (b, m) => Player?.GetHealthRatio() < Service.Config.GetValue(DataCenter.Job, Configuration.JobConfigFloat.HealthSingleAbility) && InCombat,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ArmsLength { get; } = new RoleAction(ActionID.ArmsLength, new JobRole[] { JobRole.Tank, JobRole.Melee, JobRole.RangedPhysical }, ActionOption.Defense | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Rampart { get; } = new RoleAction(ActionID.Rampart, new JobRole[] { JobRole.Tank }, ActionOption.Defense)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.SuperBolide, StatusID.HallowedGround,
            StatusID.Rampart, StatusID.Bulwark,
            StatusID.BloodWhetting,
            StatusID.Vengeance,
            StatusID.Sentinel,  
            StatusID.ShadowWall,
            StatusID.Nebula,

            //TODO:BLU Debuff
        }.Union(StatusHelper.NoNeedHealingStatus).ToArray(),
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Provoke { get; } = new RoleAction(ActionID.Provoke, new JobRole[] { JobRole.Tank })
    {
        FilterForHostiles = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Reprisal { get; } = new RoleAction(ActionID.Reprisal, new JobRole[] { JobRole.Tank });

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Shirk { get; } = new RoleAction(ActionID.Shirk, new JobRole[] { JobRole.Tank }, ActionOption.Friendly)
    {
        ChoiceTarget = (friends, mustUse) => TargetFilter.GetJobCategory(friends, JobRole.Tank)?.FirstOrDefault(),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Bloodbath { get; } = new RoleAction(ActionID.Bloodbath, new JobRole[] { JobRole.Melee }, ActionOption.Heal)
    {
        ActionCheck = (t, m) => SecondWind.ActionCheck(t, m) && HasHostilesInRange,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Feint { get; } = new RoleAction(ActionID.Feint, new JobRole[] { JobRole.Melee }, ActionOption.Defense)
    {
        ActionCheck = (b, m) => !b.HasStatus(false, StatusID.Feint),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Interject { get; } = new RoleAction(ActionID.Interject, new JobRole[] { JobRole.Tank })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LowBlow { get; } = new RoleAction(ActionID.LowBlow, new JobRole[] { JobRole.Tank })
    {
        FilterForHostiles = bs => bs.Where(b =>
        {
            if (b.IsBoss() || IsMoving || b.CastActionId == 0) return false;

            if (!b.IsCastInterruptible || Interject.IsCoolingDown) return true;
            return false;
        }),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LegSweep { get; } = new RoleAction(ActionID.LegSweep, new JobRole[] { JobRole.Melee })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HeadGraze { get; } = new RoleAction(ActionID.HeadGraze, new JobRole[] { JobRole.RangedPhysical })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SureCast { get; } = new RoleAction(ActionID.SureCast,
        new JobRole[] { JobRole.RangedMagical, JobRole.Healer }, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction TrueNorth { get; } = new RoleAction(ActionID.TrueNorth,
        new JobRole[] { JobRole.Melee }, ActionOption.Heal)
    {
        StatusProvide = new StatusID[] { StatusID.TrueNorth, StatusID.RightEye },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Peloton { get; } = new RoleAction(ActionID.Peloton, new JobRole[] { JobRole.RangedPhysical }, ActionOption.Friendly)
    {
        ActionCheck = (b, m) => NotInCombatDelay && PartyMembers.GetObjectInRadius(20)
            .Any(p => p.WillStatusEnd(3, false, StatusID.Peloton) && !p.StatusFlags.HasFlag(Dalamud.Game.ClientState.Objects.Enums.StatusFlags.InCombat)),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Sprint { get; } = new BaseAction(ActionID.Sprint, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] {StatusID.DualCast},
    };

    private protected virtual IBaseAction Raise => null;
    private protected virtual IBaseAction TankStance => null;

    #region Duty Action
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantRaise { get; } = new RoleAction(ActionID.VariantRaise,
        new JobRole[] { JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical, },
        ActionOption.Friendly | ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantUltimatum { get; } = new BaseAction(ActionID.VariantUltimatum, ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantCure { get; } = new RoleAction(ActionID.VariantCure,
        new JobRole[] { JobRole.Melee, JobRole.Tank,JobRole.RangedMagical, JobRole.RangedPhysical },
        ActionOption.Heal | ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantCure2 { get; } = new RoleAction(ActionID.VariantCure2,
        new JobRole[] { JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical }, ActionOption.Heal | ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantSpiritDart { get; } = new RoleAction(ActionID.VariantSpiritDart,
        new JobRole[] { JobRole.Healer, JobRole.Tank }, ActionOption.Dot | ActionOption.DutyAction)
    {
        TargetStatus = new StatusID[] { StatusID.VariantSpiritDart },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantSpiritDart2 { get; } = new RoleAction(ActionID.VariantSpiritDart2,
        new JobRole[] { JobRole.Healer, JobRole.Tank }, ActionOption.Dot | ActionOption.DutyAction)
    {
        TargetStatus = new StatusID[] { StatusID.VariantSpiritDart },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantRampart { get; } = new RoleAction(ActionID.VariantRampart,
        new JobRole[] { JobRole.Melee, JobRole.Healer, JobRole.RangedMagical, JobRole.RangedPhysical, }, ActionOption.Buff | ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantRampart2 { get; } = new RoleAction(ActionID.VariantRampart2,
        new JobRole[] { JobRole.Melee, JobRole.Healer, JobRole.RangedMagical, JobRole.RangedPhysical }, ActionOption.Buff | ActionOption.DutyAction);
    #endregion

    IBaseAction[] _allBaseActions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public virtual IBaseAction[] AllBaseActions => _allBaseActions ??= GetBaseActions(GetType()).ToArray();

    IAction[] _allActions;
    public IAction[] AllActions => _allActions ??= Array.Empty<IAction>().Union(GetBaseItems(GetType())).Union(AllBaseActions).ToArray();

    PropertyInfo[] _allBools;
    public PropertyInfo[] AllBools => _allBools ??= GetType().GetStaticProperties<bool>();

    PropertyInfo[] _allBytes;
    public PropertyInfo[] AllBytes => _allBytes ??= GetType().GetStaticProperties<byte>();

    MethodInfo[] _allTimes;
    public MethodInfo[] AllTimes => _allTimes ??= GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 1 && types[0].ParameterType == typeof(float);
    });

    MethodInfo[] _allGCDs;
    public MethodInfo[] AllGCDs => _allGCDs ??= GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 2 && types[0].ParameterType == typeof(uint) && types[1].ParameterType == typeof(uint);
    });
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    private IEnumerable<IBaseAction> GetBaseActions(Type type)
    {
        return GetIActions(type).OfType<IBaseAction>().Where(a => a is not RoleAction role || role.InRole(ClassJob.GetJobRole()));
    }

    private IEnumerable<IBaseItem> GetBaseItems(Type type)
    {
        return GetIActions(type).OfType<IBaseItem>().Where(a => a is not MedicineItem medicine || medicine.InType(this)).Reverse();
    }

    private IEnumerable<IAction> GetIActions(Type type)
    {
        if (type == null) return Array.Empty<IAction>();

        var acts = from prop in type.GetProperties()
                   where typeof(IAction).IsAssignableFrom(prop.PropertyType) && !(prop.GetMethod?.IsPrivate ?? true)
                   select (IAction)prop.GetValue(this) into act
                   where act != null
                   orderby act.ID
                   select act;

        return acts.Union(GetIActions(type.BaseType));
    }
}
