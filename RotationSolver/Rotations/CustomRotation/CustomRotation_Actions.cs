using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System;
using System.Linq;
using System.Reflection;

namespace RotationSolver.Rotations.CustomRotation;

internal abstract partial class CustomRotation
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
    public static IBaseAction Addle { get; } = new RoleAction(ActionID.Addle, new JobRole[] { JobRole.RangedMagicial }, isFriendly: true, isTimeline: true)
    {
        ActionCheck = b => !b.HasStatus(false, StatusID.Addle),
    };

    /// <summary>
    /// 即刻咏唱
    /// </summary>
    public static IBaseAction Swiftcast { get; } = new RoleAction(ActionID.Swiftcast, new JobRole[] { JobRole.RangedMagicial, JobRole.Healer }, true)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.Swiftcast,
            StatusID.Triplecast,
            StatusID.Dualcast,
        }
    };

    /// <summary>
    /// 康复
    /// </summary>
    public static IBaseAction Esuna { get; } = new RoleAction(ActionID.Esuna, new JobRole[] { JobRole.Healer }, true)
    {
        ChoiceTarget = (tars, mustUse) =>
        {
            if (TargetUpdater.DyingPeople.Any())
            {
                return TargetUpdater.DyingPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
            }
            else if (TargetUpdater.WeakenPeople.Any())
            {
                return TargetUpdater.WeakenPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
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
        new JobRole[] { JobRole.Healer, JobRole.RangedMagicial }, true)
    {
        ActionCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000 && InCombat,
    };

    /// <summary>
    /// 内丹
    /// </summary>
    public static IBaseAction SecondWind { get; } = new RoleAction(ActionID.SecondWind,
        new JobRole[] { JobRole.RangedPhysical, JobRole.Melee }, true, isTimeline: true)
    {
        ActionCheck = b => Service.ClientState.LocalPlayer?.GetHealthRatio() < Service.Configuration.HealthSingleAbility && InCombat,
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
            StatusID.Superbolide, StatusID.HallowedGround,
            StatusID.Rampart1, StatusID.Rampart2, StatusID.Rampart3,
            //原初的直觉和血气
            StatusID.RawIntuition, StatusID.Bloodwhetting,
            //复仇
            StatusID.Vengeance,
            //预警
            StatusID.Sentinel,  
            //暗影墙
            StatusID.ShadowWall,
            //星云
            StatusID.Nebula,

            //TODO:BLU的减伤技能
        }.Union(StatusHelper.NoNeedHealingStatus).ToArray(),
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 挑衅
    /// </summary>
    public static IBaseAction Provoke { get; } = new RoleAction(ActionID.Provoke, new JobRole[] { JobRole.Tank }, isTimeline: true)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
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
    public static IBaseAction Interject { get; } = new RoleAction(ActionID.Interject, new JobRole[] { JobRole.Tank });

    /// <summary>
    /// 下踢
    /// </summary>
    public static IBaseAction LowBlow { get; } = new RoleAction(ActionID.LowBlow, new JobRole[] { JobRole.Tank })
    {
        ActionCheck = b => !b.IsBoss() && !MovingUpdater.IsMoving,
    };

    /// <summary>
    /// 扫腿
    /// </summary>
    public static IBaseAction LegSweep { get; } = new RoleAction(ActionID.LegSweep, new JobRole[] { JobRole.Melee });

    /// <summary>
    /// 伤头
    /// </summary>
    public static IBaseAction HeadGraze { get; } = new RoleAction(ActionID.HeadGraze, new JobRole[] { JobRole.RangedPhysical });

    /// <summary>
    /// 沉稳咏唱
    /// </summary>
    public static IBaseAction Surecast { get; } = new RoleAction(ActionID.Surecast,
        new JobRole[] { JobRole.RangedMagicial, JobRole.Healer }, true, shouldEndSpecial: true);

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
        ActionCheck = b => !InCombat && TargetUpdater.PartyMembers.GetObjectInRadius(20).Any(p => !p.HasStatus(false, StatusID.Peloton)),
    };

    private protected virtual IBaseAction Raise => null;
    private protected virtual IBaseAction Shield => null;

    /// <summary>
    /// 当前这个类所有的BaseAction
    /// </summary>
    public virtual IBaseAction[] AllActions => GetBaseActions(GetType());

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

    private IBaseAction[] GetBaseActions(Type type)
    {
        if (type == null) return new IBaseAction[0];

        var acts = from prop in type.GetProperties()
                   where typeof(IBaseAction).IsAssignableFrom(prop.PropertyType)
                   select (IBaseAction)prop.GetValue(this) into act
                   orderby act.ID
                   where act is RoleAction role ? role.InRole(Job.GetJobRole()) : true
                   select act;

        return acts.Union(GetBaseActions(type.BaseType)).ToArray();
    }
}
