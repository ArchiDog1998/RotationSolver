using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

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

        public override bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte aoeCount = 0, byte gcdCountForAbility = 0)
        {
            return base.CanUse(out act, option, aoeCount, gcdCountForAbility)
                && Player != null && InRole(Player.ClassJob.GameData.GetJobRole());
        }
    }

    #region PvE
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
        FilterForHostiles = bs => bs.Where((Func<BattleChara, bool>)(b =>
        {
            if (b.IsBossFromIcon() || IsMoving || b.CastActionId == 0) return false;

            if (!b.IsCastInterruptible || Interject.IsCoolingDown) return true;
            return false;
        })),
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
        ActionCheck = (b, m) =>
        {
            if (!NotInCombatDelay) return false;
            var players = PartyMembers.GetObjectInRadius(20);
            if (players.Any(ObjectHelper.InCombat)) return false;
            return players.Any(p => p.WillStatusEnd(3, false, StatusID.Peloton));
        },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Sprint { get; } = new BaseAction(ActionID.Sprint, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.DualCast },
    };

    private protected virtual IBaseAction Raise => null;
    private protected virtual IBaseAction LimitBreak => null;
    private protected virtual IBaseAction TankStance => null;

    #endregion

    #region PvE Limitbreak
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ShieldWall { get; } = new RoleAction(ActionID.ShieldWall, new JobRole[] { JobRole.Tank }, ActionOption.Defense)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Stronghold { get; } = new RoleAction(ActionID.Stronghold, new JobRole[] { JobRole.Tank }, ActionOption.Defense)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HealingWind { get; } = new RoleAction(ActionID.HealingWind, new JobRole[] { JobRole.Healer }, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BreathOfTheEarth { get; } = new RoleAction(ActionID.BreathOfTheEarth, new JobRole[] { JobRole.Healer }, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Braver { get; } = new RoleAction(ActionID.Braver, new JobRole[] { JobRole.Melee })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Bladedance { get; } = new RoleAction(ActionID.Bladedance, new JobRole[] { JobRole.Melee })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction BigShot { get; } = new RoleAction(ActionID.BigShot, new JobRole[] { JobRole.RangedPhysical })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Desperado { get; } = new RoleAction(ActionID.Desperado, new JobRole[] { JobRole.RangedPhysical })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Skyshard { get; } = new RoleAction(ActionID.Skyshard, new JobRole[] { JobRole.RangedMagical })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Starstorm { get; } = new RoleAction(ActionID.Starstorm, new JobRole[] { JobRole.RangedMagical })
    {
        ActionCheck = (b, m) => LimitBreakLevel == 2,
    };
    #endregion

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
    public static IBaseAction VariantRaise2 { get; } = new RoleAction(ActionID.VariantRaise2,
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
        new JobRole[] { JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical },
        ActionOption.Heal | ActionOption.DutyAction | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantCure2 { get; } = new RoleAction(ActionID.VariantCure2,
        new JobRole[] { JobRole.Melee, JobRole.Tank, JobRole.RangedMagical, JobRole.RangedPhysical },
        ActionOption.Heal | ActionOption.DutyAction | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantSpiritDart { get; } = new RoleAction(ActionID.VariantSpiritDart,
        new JobRole[] { JobRole.Healer, JobRole.Tank }, ActionOption.Dot | ActionOption.DutyAction);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction VariantSpiritDart2 { get; } = new RoleAction(ActionID.VariantSpiritDart2,
        new JobRole[] { JobRole.Healer, JobRole.Tank }, ActionOption.Dot | ActionOption.DutyAction);

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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostSpellforge { get; } = new BaseAction(ActionID.LostSpellforge,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostSpellforge },
        ActionCheck = (b, m) => LostSpellforge.Target?.HasStatus(false, StatusID.MagicalAversion) ?? false,
        ChoiceTarget = (targets, mustUse) => targets.FirstOrDefault(t => (Job)t.ClassJob.Id switch
        {
            Job.WAR
            or Job.GNB
            or Job.MNK
            or Job.SAM
            or Job.DRG
            or Job.MCH
            or Job.DNC

            or Job.PLD
            or Job.DRK
            or Job.NIN
            or Job.BRD
            or Job.RDM
            => true,

            _ => false,
        }),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostSteelsting { get; } = new BaseAction(ActionID.LostSteelsting,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostSteelsting },
        ActionCheck = (b, m) => LostSteelsting.Target?.HasStatus(false, StatusID.PhysicalAversion) ?? false,
        ChoiceTarget = (targets, mustUse) => targets.FirstOrDefault(t => (Job)t.ClassJob.Id switch
        {
            Job.WHM
            or Job.SCH
            or Job.AST
            or Job.SGE
            or Job.BLM
            or Job.SMN

            or Job.PLD
            or Job.DRK
            or Job.NIN
            or Job.BRD
            or Job.RDM
            => true,

            _ => false,
        }),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostRampage { get; } = new BaseAction(ActionID.LostRampage,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostRampage },
        ActionCheck = (b, m) => LostRampage.Target?.HasStatus(false, StatusID.PhysicalAversion) ?? false,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostBurst { get; } = new BaseAction(ActionID.LostBurst,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostBurst },
        ActionCheck = (b, m) => LostBurst.Target?.HasStatus(false, StatusID.MagicalAversion) ?? false,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostBravery { get; } = new BaseAction(ActionID.LostBravery,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostBravery },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostProtect { get; } = new BaseAction(ActionID.LostProtect,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostProtect, StatusID.LostProtect2 },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostShell { get; } = new BaseAction(ActionID.LostShell,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostShell, StatusID.LostShell2 },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostProtect2 { get; } = new BaseAction(ActionID.LostProtect2,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostProtect2 },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostShell2 { get; } = new BaseAction(ActionID.LostShell2,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostShell2 },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostBubble { get; } = new BaseAction(ActionID.LostBubble,
        ActionOption.DutyAction | ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.LostBubble },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostStoneskin { get; } = new BaseAction(ActionID.LostStoneskin,
        ActionOption.DutyAction | ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        StatusProvide = new StatusID[] { StatusID.LostStoneskin },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostStoneskin2 { get; } = new BaseAction(ActionID.LostStoneskin2,
        ActionOption.DutyAction | ActionOption.Defense)
    {
        StatusProvide = new StatusID[] { StatusID.LostStoneskin },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostFlarestar { get; } = new BaseAction(ActionID.LostFlarestar,
    ActionOption.DutyAction)
    {
        StatusProvide = new StatusID[] { StatusID.LostFlarestar },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LostSeraphStrike { get; } = new BaseAction(ActionID.LostSeraphStrike,
        ActionOption.DutyAction)
    {
        StatusProvide = new StatusID[] { StatusID.LostSeraphStrike },
    };
    #endregion

    #region PvP
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_StandardIssueElixir { get; } = new BaseAction(ActionID.PvP_StandardIssueElixir, ActionOption.Heal)
    {
        ActionCheck = (t, m) => !HasHostilesInMaxRange
            && (t.CurrentMp <= t.MaxMp / 3 || t.CurrentHp <= t.MaxHp / 3)
            && !IsLastAction(ActionID.PvP_StandardIssueElixir),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Recuperate { get; } = new BaseAction(ActionID.PvP_Recuperate, ActionOption.Heal)
    {
        ActionCheck = (t, m) => t.MaxHp - t.CurrentHp > 15000,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Purify { get; } = new BaseAction(ActionID.PvP_Purify, ActionOption.Heal)
    {
        ActionCheck = (t, m) => Player?.StatusList.Any(s => s.GameData.CanDispel) ?? false,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Guard { get; } = new BaseAction(ActionID.PvP_Guard, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Sprint { get; } = new BaseAction(ActionID.PvP_Sprint, ActionOption.Friendly)
    {
        StatusProvide = new StatusID[] { StatusID.PvP_Sprint },
    };
    #endregion

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
        return GetIEnoughLevel<IBaseAction>(type).Where(a => a is not RoleAction role || role.InRole(ClassJob.GetJobRole()));
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
