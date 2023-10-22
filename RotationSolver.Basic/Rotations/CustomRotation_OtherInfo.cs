using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace RotationSolver.Basic.Rotations;
public abstract partial class CustomRotation
{
    #region Player
    /// <summary>
    /// This is the player.
    /// </summary>
    protected static PlayerCharacter Player => ECommons.GameHelpers.Player.Object;

    /// <summary>
    /// Does player have swift cast, dual cast or triple cast.
    /// </summary>
    public static bool HasSwift => Player?.HasStatus(true, Swiftcast.StatusProvide) ?? false;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasTankStance => Player?.HasStatus(true, StatusHelper.TankStanceStatus) ?? false;

    /// <summary>
    /// Check the player is moving, such as running, walking or jumping.
    /// </summary>
    public static bool IsMoving => DataCenter.IsMoving;

    /// <summary>
    /// Is in combat.
    /// </summary>
    public static bool InCombat => DataCenter.InCombat;

    static RandomDelay _notInCombatDelay = new(() =>
        (Service.Config.GetValue(Configuration.PluginConfigFloat.NotInCombatDelayMin),
        Service.Config.GetValue(Configuration.PluginConfigFloat.NotInCombatDelayMax)));

    /// <summary>
    /// Is out of combat.
    /// </summary>
    public static bool NotInCombatDelay => _notInCombatDelay.Delay(!InCombat);

    /// <summary>
    /// Player's MP.
    /// </summary>
    public static uint CurrentMp => DataCenter.CurrentMp;

    /// <summary>
    /// Condition.
    /// </summary>
    protected static ICondition Condition => Svc.Condition;

    #endregion

    #region Friends
    /// <summary>
    /// Has the comapnion now.
    /// </summary>
    public static bool HasCompanion => DataCenter.HasCompanion;

    /// <summary>
    /// Party member.
    /// </summary>
    protected static IEnumerable<BattleChara> PartyMembers => DataCenter.PartyMembers;

    /// <summary>
    /// Party tanks.
    /// </summary>
    protected static IEnumerable<BattleChara> PartyTanks => DataCenter.PartyTanks;

    /// <summary>
    /// Party healers.
    /// </summary>
    protected static IEnumerable<BattleChara> PartyHealers => DataCenter.PartyHealers;

    /// <summary>
    /// Alliance members.
    /// </summary>
    protected static IEnumerable<BattleChara> AllianceMembers => DataCenter.AllianceMembers;

    /// <summary>
    /// Alliance Tanks.
    /// </summary>
    protected static IEnumerable<BattleChara> AllianceTanks => DataCenter.AllianceTanks;

    /// <summary>
    /// Weaken People
    /// </summary>
    protected static IEnumerable<BattleChara> WeakenPeople => DataCenter.WeakenPeople;

    /// <summary>
    /// The people is dying.
    /// </summary>
    protected static IEnumerable<BattleChara> DyingPeople => DataCenter.DyingPeople;

    /// <summary>
    /// The ratio of members that in burst 2min.
    /// </summary>
    public static float RatioOfMembersIn2minsBurst => DataCenter.RatioOfMembersIn2minsBurst;


    /// <summary>
    /// Whether the number of party members is 8.
    /// </summary>
    public static bool IsFullParty => PartyMembers.Count() is 8;

    /// <summary>
    /// party members HP.
    /// </summary>
    protected static IEnumerable<float> PartyMembersHP => DataCenter.PartyMembersHP;

    /// <summary>
    /// Min HP in party members.
    /// </summary>
    public static float PartyMembersMinHP => DataCenter.PartyMembersMinHP;

    /// <summary>
    /// Average HP in party members.
    /// </summary>
    public static float PartyMembersAverHP => DataCenter.PartyMembersAverHP;
    #endregion

    #region Target
    /// <summary>
    /// The player's target.
    /// </summary>
    protected static BattleChara Target => Svc.Targets.Target is BattleChara b ? b : Player;

    /// <summary>
    /// Shortcut for Target.IsDying();
    /// </summary>
    public static bool IsTargetDying => Target?.IsDying() ?? false;

    /// <summary>
    /// Shortcut for Target.IsBoss();
    /// </summary>
    public static bool IsTargetBoss => Target?.IsBoss() ?? false;


    /// <summary>
    /// Is there any hostile target in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    public static bool HasHostilesInRange => DataCenter.HasHostilesInRange;

    /// <summary>
    /// How many hostile targets in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    public static int NumberOfHostilesInRange => DataCenter.NumberOfHostilesInRange;

    /// <summary>
    /// How many hostile targets in max range (25 yalms) regardless of job
    /// </summary>
    public static int NumberOfHostilesInMaxRange => DataCenter.NumberOfHostilesInMaxRange;

    /// <summary>
    /// All hostile Targets.
    /// </summary>
    protected static IEnumerable<BattleChara> HostileTargets => DataCenter.HostileTargets;

    /// <summary>
    /// How many hostile targets in range? 25 for ranged jobs and healer, 3 for melee and tank. This is all can attack.
    /// </summary>
    public static int NumberOfAllHostilesInRange => DataCenter.NumberOfAllHostilesInRange;

    /// <summary>
    /// How many hostile targets in max range (25 yalms) regardless of job. This is all can attack.
    /// </summary>
    public static int NumberOfAllHostilesInMaxRange => DataCenter.NumberOfAllHostilesInMaxRange;

    /// <summary>
    /// All hostile Targets. This is all can attack.
    /// </summary>
    protected static IEnumerable<BattleChara> AllHostileTargets => DataCenter.AllHostileTargets;

    /// <summary>
    /// Average dead time of hostiles.
    /// </summary>
    public static float AverageTimeToKill => DataCenter.AverageTimeToKill;

    /// <summary>
    /// Is the <see cref="AverageTimeToKill"/> larger than <paramref name="time"/>.
    /// </summary>
    /// <param name="time">Time</param>
    /// <returns>Is Longer.</returns>
    public static bool IsLongerThan(float time)
    {
        if (IsInHighEndDuty) return true;
        return AverageTimeToKill > time;
    }

    /// <summary>
    /// Now, it is attacking the mobs!
    /// </summary>
    public static bool MobsTime => DataCenter.MobsTime;
    #endregion

    #region Command
    /// <summary>
    /// Is in burst right now? Usually it used with team support actions. Please use <see cref="IsBurst"/> instead.
    /// </summary>
    [Obsolete("It will be removed in the future version", true)]
    public static bool InBurst => IsBurst;

    /// <summary>
    /// Is in burst right now? Usually it used with team support actions.
    /// </summary>
    public static bool IsBurst => DataCenter.IsBurst || Service.Config.GetValue(Configuration.PluginConfigBool.AutoBurst);

    /// <summary>
    /// Is in the command heal area.
    /// </summary>
    public static bool IsHealArea => DataCenter.IsHealArea;

    /// <summary>
    /// Is in the command heal single.
    /// </summary>
    public static bool IsHealSingle => DataCenter.IsHealSingle;

    /// <summary>
    /// Is in the command defense area.
    /// </summary>
    public static bool IsDefenseArea => DataCenter.IsDefenseArea;

    /// <summary>
    /// Is in the command defense single.
    /// </summary>
    public static bool IsDefenseSingle => DataCenter.IsDefenseSingle;

    /// <summary>
    /// Is in the command Esuna Stance North.
    /// </summary>
    public static bool IsEsunaStanceNorth => DataCenter.IsEsunaStanceNorth;

    /// <summary>
    /// Is in the command Raise Shirk.
    /// </summary>
    public static bool IsRaiseShirk => DataCenter.IsRaiseShirk;

    /// <summary>
    /// Is in the command move forward.
    /// </summary>
    public static bool IsMoveForward => DataCenter.IsMoveForward;

    /// <summary>
    /// Is in the command move back.
    /// </summary>
    public static bool IsMoveBack => DataCenter.IsMoveBack;

    /// <summary>
    /// Is in the command anti knockback.
    /// </summary>
    public static bool IsAntiKnockback => DataCenter.IsAntiKnockback;

    /// <summary>
    /// Is in the command speed.
    /// </summary>
    public static bool IsSpeed => DataCenter.IsSpeed;

    private bool CanUseHealAction => 
        //Job
        (ClassJob.GetJobRole() == JobRole.Healer || Service.Config.GetValue(Configuration.PluginConfigBool.UseHealWhenNotAHealer)) 
        && Service.Config.GetValue(Configuration.PluginConfigBool.AutoHeal)
        && IsLongerThan(Service.Config.GetValue(Configuration.PluginConfigFloat.AutoHealTimeToKill));

    /// <summary>
    /// 
    /// </summary>
    public virtual bool CanHealAreaAbility => DataCenter.CanHealAreaAbility && CanUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    public virtual bool CanHealAreaSpell => DataCenter.CanHealAreaSpell && CanUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    public virtual bool CanHealSingleAbility => DataCenter.CanHealSingleAbility && CanUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    public virtual bool CanHealSingleSpell => DataCenter.CanHealSingleSpell && CanUseHealAction;

    /// <summary>
    /// True for On, false for off.
    /// </summary>
    public static bool AutoState => DataCenter.State;

    /// <summary>
    /// Ture for Manual Target, false for Auto Target.
    /// </summary>
    public static bool IsManual => DataCenter.IsManual;
    #endregion

    #region GCD

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponRemain => DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponTotal => DataCenter.WeaponTotal;

    /// <summary>
    /// 
    /// </summary>
    protected static float WeaponElapsed => DataCenter.WeaponElapsed;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected static ClientLanguage Language => Svc.ClientState.ClientLanguage;

    /// <summary>
    /// 
    /// </summary>
    protected static TerritoryContentType TerritoryContentType => DataCenter.TerritoryContentType;

    /// <summary>
    /// Is player in high-end duty.
    /// </summary>
    public static bool IsInHighEndDuty => DataCenter.IsInHighEndDuty;

    /// <summary>
    /// Is player in duty.
    /// </summary>
    public static bool IsInDuty => Svc.Condition[ConditionFlag.BoundByDuty];

    /// <summary>
    /// 
    /// </summary>
    public static float Ping => DataCenter.Ping;

    /// <summary>
    /// 
    /// </summary>
    public static float NextAbilityToNextGCD => DataCenter.NextAbilityToNextGCD;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static uint AdjustId(uint id) => Service.GetAdjustedActionId(id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ActionID AdjustId(ActionID id) => Service.GetAdjustedActionId(id);

    /// <summary>
    /// 
    /// </summary>
    public double AverageCountOfLastUsing { get; internal set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int MaxCountOfLastUsing { get; internal set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public double AverageCountOfCombatTimeUsing { get; internal set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public int MaxCountOfCombatTimeUsing { get; internal set; } = 0;
    internal long CountOfTracking { get; set; } = 0;

    internal static int CountingOfLastUsing { get; set; } = 0;
    internal static int CountingOfCombatTimeUsing { get; set; } = 0;


    /// <summary>
    ///  The actions that were used by player successfully. The first one is the latest successfully used one.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    protected static ActionRec[] RecordActions
    {
        get
        {
            CountingOfLastUsing++;
            return DataCenter.RecordActions;
        }
    }

    /// <summary>
    /// How much time has passed since the last action was released.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    protected static TimeSpan TimeSinceLastAction
    {
        get
        {
            CountingOfLastUsing++;
            return DataCenter.TimeSinceLastAction;
        }
    }

    /// <summary>
    /// Check for GCD Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastGCD(bool isAdjust, params IAction[] actions)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastGCD(isAdjust, actions);
    }

    /// <summary>
    /// Check for GCD Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastGCD(params ActionID[] ids)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastGCD(ids);
    }

    /// <summary>
    /// Check for ability Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastAbility(bool isAdjust, params IAction[] actions)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastAbility(isAdjust, actions);
    }

    /// <summary>
    /// Check for ability Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastAbility(params ActionID[] ids)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastAbility(ids);
    }

    /// <summary>
    /// Check for action Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastAction(bool isAdjust, params IAction[] actions)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastAction(isAdjust, actions);
    }

    /// <summary>
    /// Check for action Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    public static bool IsLastAction(params ActionID[] ids)
    {
        CountingOfLastUsing++;
        return IActionHelper.IsLastAction(ids);
    }

    /// <summary>
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="GCD"></param>
    /// <returns></returns>
    protected static bool CombatElapsedLessGCD(int GCD)
    {
        CountingOfCombatTimeUsing ++;
        return CombatElapsedLess(GCD * DataCenter.WeaponTotal);
    }

    /// <summary>
    /// Whether the battle lasted less than <paramref name="time"/> seconds
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="time">time in second.</param>
    /// <returns></returns>
    protected static bool CombatElapsedLess(float time)
    {
        CountingOfCombatTimeUsing++;
        return CombatTime <= time;
    }

    /// <summary>
    /// The combat time.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    public static float CombatTime
    {
        get
        {
            CountingOfCombatTimeUsing++;
            return InCombat ? DataCenter.CombatTimeRaw + DataCenter.WeaponRemain : 0;
        }
    }

    /// <summary>
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="GCD"></param>
    /// <returns></returns>
    protected static bool StopMovingElapsedLessGCD(int GCD) => StopMovingElapsedLess(GCD * DataCenter.WeaponTotal);

    /// <summary>
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="time">time in second.</param>
    /// <returns></returns>
    protected static bool StopMovingElapsedLess(float time) => StopMovingTime <= time;

    /// <summary>
    /// The time of stopping moving.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    public static float StopMovingTime => IsMoving ? 0 : DataCenter.StopMovingRaw + DataCenter.WeaponRemain;

    /// <summary>
    /// Time from GCD.
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static float GCDTime(uint gcdCount = 0, float offset = 0)
        => DataCenter.GCDTime(gcdCount, offset);

    #region Service
    /// <summary>
    /// The count down ahead.
    /// </summary>
    public static float CountDownAhead => Service.Config.GetValue(Configuration.PluginConfigFloat.CountDownAhead);

    /// <summary>
    /// 
    /// </summary>
    public float HealthAreaAbility => Jobs.FirstOrDefault().GetHealthAreaAbility();

    /// <summary>
    /// 
    /// </summary>
    public float HealthAreaSpell => Jobs.FirstOrDefault().GetHealthAreaSpell();

    /// <summary>
    /// 
    /// </summary>
    public float HealthAreaAbilityHot => Jobs.FirstOrDefault().GetHealthAreaAbilityHot();

    /// <summary>
    /// 
    /// </summary>
    public float HealthAreaSpellHot => Jobs.FirstOrDefault().GetHealthAreaSpellHot();

    /// <summary>
    /// 
    /// </summary>
    public float HealthSingleAbility => Jobs.FirstOrDefault().GetHealthSingleAbility();

    /// <summary>
    /// 
    /// </summary>
    public float HealthSingleSpell => Jobs.FirstOrDefault().GetHealthSingleSpell();

    /// <summary>
    /// 
    /// </summary>
    public float HealthSingleAbilityHot => Jobs.FirstOrDefault().GetHealthSingleAbilityHot();

    /// <summary>
    /// 
    /// </summary>
    public float HealthSingleSpellHot => Jobs.FirstOrDefault().GetHealthSingleSpellHot();

    /// <summary>
    /// 
    /// </summary>
    public float HealthForDyingTanksDefault => Jobs.FirstOrDefault().GetHealthForDyingTank();
    #endregion
}
