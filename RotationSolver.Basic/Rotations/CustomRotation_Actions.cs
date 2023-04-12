namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    internal class RoleAction : BaseAction
    {
        private JobRole[] _roles;
        internal RoleAction(ActionID actionID, JobRole[] roles, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false, bool isTimeline = false) : base(actionID, isFriendly, shouldEndSpecial, isEot, isTimeline)
        {
            _roles = roles;
        }

        internal bool InRole(JobRole role) => _roles.Contains(role);
    }

    /// <summary>
    /// 昏乱
    /// </summary>
    public static IBaseAction Addle { get; } = new RoleAction(ActionID.Addle, new JobRole[] { JobRole.RangedMagical }, isFriendly: true, isTimeline: true)
    {
        ActionCheck = b => !b.HasStatus(false, StatusID.Addle),
    };

    /// <summary>
    /// 即刻咏唱
    /// </summary>
    public static IBaseAction Swiftcast { get; } = new RoleAction(ActionID.SwiftCast, new JobRole[] { JobRole.RangedMagical, JobRole.Healer }, true)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.SwiftCast,
            StatusID.TripleCast,
            StatusID.DualCast,
        }
    };

    /// <summary>
    /// 康复
    /// </summary>
    public static IBaseAction Esuna { get; } = new RoleAction(ActionID.Esuna, new JobRole[] { JobRole.Healer }, true)
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
    /// 营救
    /// </summary>
    public static IBaseAction Rescue { get; } = new RoleAction(ActionID.Rescue, new JobRole[] { JobRole.Healer }, true, isTimeline: true);

    /// <summary>
    /// 沉静
    /// </summary>
    public static IBaseAction Repose { get; } = new RoleAction(ActionID.Repose, new JobRole[] { JobRole.Healer });

    /// <summary>
    /// 醒梦（如果MP低于6000那么使用）
    /// </summary>
    public static IBaseAction LucidDreaming { get; } = new RoleAction(ActionID.LucidDreaming,
        new JobRole[] { JobRole.Healer, JobRole.RangedMagical }, true)
    {
        ActionCheck = b => Player.CurrentMp < 6000 && InCombat,
    };

    /// <summary>
    /// 内丹
    /// </summary>
    public static IBaseAction SecondWind { get; } = new RoleAction(ActionID.SecondWind,
        new JobRole[] { JobRole.RangedPhysical, JobRole.Melee }, true, isTimeline: true)
    {
        ActionCheck = b => Player?.GetHealthRatio() < Service.Config.HealthSingleAbility && InCombat,
    };

    /// <summary>
    /// 亲疏自行
    /// </summary>
    public static IBaseAction ArmsLength { get; } = new RoleAction(ActionID.ArmsLength, new JobRole[] { JobRole.Tank, JobRole.Melee, JobRole.RangedPhysical }, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// 铁壁
    /// </summary>
    public static IBaseAction Rampart { get; } = new RoleAction(ActionID.Rampart, new JobRole[] { JobRole.Tank }, true, isTimeline: true)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.SuperBolide, StatusID.HallowedGround,
            StatusID.Rampart, StatusID.Bulwark,
            StatusID.RawIntuition, StatusID.BloodWhetting,
            StatusID.Vengeance,
            StatusID.Sentinel,  
            StatusID.ShadowWall,
            StatusID.Nebula,

            //TODO:BLU Debuff
        }.Union(StatusHelper.NoNeedHealingStatus).ToArray(),
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 挑衅
    /// </summary>
    public static IBaseAction Provoke { get; } = new RoleAction(ActionID.Provoke, new JobRole[] { JobRole.Tank }, isTimeline: true)
    {
        FilterForHostiles = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// 雪仇
    /// </summary>
    public static IBaseAction Reprisal { get; } = new RoleAction(ActionID.Reprisal, new JobRole[] { JobRole.Tank }, isTimeline: true);

    /// <summary>
    /// 退避
    /// </summary>
    public static IBaseAction Shirk { get; } = new RoleAction(ActionID.Shirk, new JobRole[] { JobRole.Tank }, true)
    {
        ChoiceTarget = (friends, mustUse) => TargetFilter.GetJobCategory(friends, JobRole.Tank)?.FirstOrDefault(),
    };

    /// <summary>
    /// 浴血
    /// </summary>
    public static IBaseAction Bloodbath { get; } = new RoleAction(ActionID.Bloodbath, new JobRole[] { JobRole.Melee }, true, isTimeline: true)
    {
        ActionCheck = SecondWind.ActionCheck,
    };

    /// <summary>
    /// 牵制
    /// </summary>
    public static IBaseAction Feint { get; } = new RoleAction(ActionID.Feint, new JobRole[] { JobRole.Melee }, isTimeline: true)
    {
        ActionCheck = b => !b.HasStatus(false, StatusID.Feint),
    };

    /// <summary>
    /// 插言
    /// </summary>
    public static IBaseAction Interject { get; } = new RoleAction(ActionID.Interject, new JobRole[] { JobRole.Tank })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 下踢
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
    /// 扫腿
    /// </summary>
    public static IBaseAction LegSweep { get; } = new RoleAction(ActionID.LegSweep, new JobRole[] { JobRole.Melee })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 伤头
    /// </summary>
    public static IBaseAction HeadGraze { get; } = new RoleAction(ActionID.HeadGraze, new JobRole[] { JobRole.RangedPhysical })
    {
        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
    };

    /// <summary>
    /// 沉稳咏唱
    /// </summary>
    public static IBaseAction SureCast { get; } = new RoleAction(ActionID.SureCast,
        new JobRole[] { JobRole.RangedMagical, JobRole.Healer }, true, shouldEndSpecial: true);

    /// <summary>
    /// 真北
    /// </summary>
    public static IBaseAction TrueNorth { get; } = new RoleAction(ActionID.TrueNorth,
        new JobRole[] { JobRole.Melee }, true, shouldEndSpecial: true)
    {
        StatusProvide = new StatusID[] { StatusID.TrueNorth },
    };

    /// <summary>
    /// 速行
    /// </summary>
    public static IBaseAction Peloton { get; } = new RoleAction(ActionID.Peloton, new JobRole[] { JobRole.RangedPhysical }, true)
    {
        ActionCheck = b => NotInCombatDelay && PartyMembers.GetObjectInRadius(20)
            .Any(p => p.WillStatusEnd(3, false, StatusID.Peloton)),
    };

    private protected virtual IBaseAction Raise => null;
    private protected virtual IBaseAction TankStance => null;

    /// <summary>
    /// 当前这个类所有的BaseAction
    /// </summary>
    public virtual IBaseAction[] AllBaseActions => GetBaseActions(GetType()).ToArray();

    public IAction[] AllActions => new IAction[0].Union(GetBaseItems(GetType())).Union(AllBaseActions).ToArray();

    /// <summary>
    /// 这个类所有的公开bool值
    /// </summary>
    public PropertyInfo[] AllBools => GetType().GetStaticProperties<bool>();

    /// <summary>
    /// 这个类所有的公开float值
    /// </summary>
    public PropertyInfo[] AllBytes => GetType().GetStaticProperties<byte>();

    public MethodInfo[] AllTimes => GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 1 && types[0].ParameterType == typeof(float);
    });

    public MethodInfo[] AllGCDs => GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 2 && types[0].ParameterType == typeof(uint) && types[1].ParameterType == typeof(uint);
    });

    private IEnumerable<IBaseAction> GetBaseActions(Type type)
    {
        return GetIActions(type).OfType<IBaseAction>().Where(a => a is RoleAction role ? role.InRole(Job.GetJobRole()) : true);
    }

    private IEnumerable<IBaseItem> GetBaseItems(Type type)
    {
        return GetIActions(type).OfType<IBaseItem>().Where(a => a is MedicineItem medicine ? medicine.InType(this) : true).Reverse();
    }

    private IEnumerable<IAction> GetIActions(Type type)
    {
        if (type == null) return new IAction[0];

        var acts = from prop in type.GetProperties()
                   where typeof(IAction).IsAssignableFrom(prop.PropertyType) && !(prop.GetMethod?.IsPrivate ?? true)
                   select (IAction)prop.GetValue(this) into act
                   where act != null
                   orderby act.ID
                   select act;

        return acts.Union(GetIActions(type.BaseType));
    }
}
