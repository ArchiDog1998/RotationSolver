using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
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
    /// The level of the player.
    /// </summary>
    protected static byte Level => (byte)ECommons.GameHelpers.Player.Level;

    /// <summary>
    /// Does player have swift cast, dual cast or triple cast.
    /// </summary>
    protected static bool HasSwift => Player?.HasStatus(true, Swiftcast.StatusProvide) ?? false;

    /// <summary>
    /// 
    /// </summary>
    protected static bool HasTankStance => Player?.HasStatus(true, StatusHelper.TankStanceStatus) ?? false;

    /// <summary>
    /// Check the player is moving, such as running, walking or jumping.
    /// </summary>
    protected static bool IsMoving => DataCenter.IsMoving;

    /// <summary>
    /// Is in combat.
    /// </summary>
    protected static bool InCombat => DataCenter.InCombat;

    static RandomDelay _notInCombatDelay = new(() =>
        (Service.Config.NotInCombatDelayMin, Service.Config.NotInCombatDelayMax));
    protected static bool NotInCombatDelay => _notInCombatDelay.Delay(!InCombat);

    /// <summary>
    /// Player's MP.
    /// </summary>
    protected static uint CurrentMp => DataCenter.CurrentMp;

    /// <summary>
    /// Condition.
    /// </summary>
    protected static Condition Condition => Svc.Condition;

    #endregion

    #region Friends

    protected static bool HasCompanion => DataCenter.HasCompanion;
    protected static IEnumerable<BattleChara> PartyMembers => DataCenter.PartyMembers;
    protected static IEnumerable<BattleChara> PartyTanks => DataCenter.PartyTanks;
    protected static IEnumerable<BattleChara> PartyHealers => DataCenter.PartyHealers;
    protected static IEnumerable<BattleChara> AllianceMembers => DataCenter.AllianceMembers;
    protected static IEnumerable<BattleChara> AllianceTanks => DataCenter.AllianceTanks;
    protected static IEnumerable<BattleChara> WeakenPeople => DataCenter.WeakenPeople;
    protected static IEnumerable<BattleChara> DyingPeople => DataCenter.DyingPeople;

    protected static float RatioOfMembersIn2minsBurst => DataCenter.RatioOfMembersIn2minsBurst;


    /// <summary>
    /// Whether the number of party members is 8.
    /// </summary>
    protected static bool IsFullParty => PartyMembers.Count() is 8;

    protected static IEnumerable<float> PartyMembersHP => DataCenter.PartyMembersHP;
    protected static float PartyMembersMinHP => DataCenter.PartyMembersMinHP;
    protected static float PartyMembersAverHP => DataCenter.PartyMembersAverHP;
    #endregion

    #region Target
    /// <summary>
    /// The player's target.
    /// </summary>
    protected static BattleChara Target => Svc.Targets.Target is BattleChara b ? b : Player;

    /// <summary>
    /// Shortcut for Target.IsDying();
    /// </summary>
    protected static bool IsTargetDying => Target?.IsDying() ?? false;

    /// <summary>
    /// Shortcut for Target.IsBoss();
    /// </summary>
    protected static bool IsTargetBoss => Target?.IsBoss() ?? false;


    /// <summary>
    /// Is there any hostile target in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    protected static bool HasHostilesInRange => DataCenter.HasHostilesInRange;

    /// <summary>
    /// How many hostile targets in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    protected static int NumberOfHostilesInRange => DataCenter.NumberOfHostilesInRange;

    /// <summary>
    /// How many hostile targets in max range (25 yalms) regardless of job
    /// </summary>
    protected static int NumberOfHostilesInMaxRange => DataCenter.NumberOfHostilesInMaxRange;

    protected static IEnumerable<BattleChara> HostileTargets => DataCenter.HostileTargets;

    #endregion

    #region Command

    /// <summary>
    /// Is in burst right now? Usually it used with team support actions.
    /// </summary>
    protected static bool InBurst => DataCenter.SpecialType == SpecialCommandType.Burst || Configuration.PluginConfiguration.GetValue(SettingsCommand.AutoBurst);

    bool CanUseHealAction => Job.GetJobRole() == JobRole.Healer || Service.Config.UseHealWhenNotAHealer;

    protected virtual bool CanHealAreaAbility => DataCenter.CanHealAreaAbility && CanUseHealAction;

    protected virtual bool CanHealAreaSpell => DataCenter.CanHealAreaSpell && CanUseHealAction;

    protected virtual bool CanHealSingleAbility => DataCenter.CanHealSingleAbility && CanUseHealAction;

    protected virtual bool CanHealSingleSpell => DataCenter.CanHealSingleSpell && CanUseHealAction;

    protected static SpecialCommandType SpecialType => DataCenter.SpecialType;
    protected static StateCommandType StateType => DataCenter.StateType;
    #endregion

    #region GCD
    protected static float WeaponRemain => DataCenter.WeaponRemain;

    protected static float WeaponTotal => DataCenter.WeaponTotal;

    protected static float WeaponElapsed => DataCenter.WeaponElapsed;
    #endregion

    protected static ClientLanguage Language => Svc.ClientState.ClientLanguage;
    protected static TerritoryContentType TerritoryContentType => DataCenter.TerritoryContentType;

    protected static float Ping => DataCenter.Ping;

    protected static float NextAbilityToNextGCD => DataCenter.NextAbilityToNextGCD;

    public static uint AdjustId(uint id) => Service.GetAdjustedActionId(id);
    public static ActionID AdjustId(ActionID id) => Service.GetAdjustedActionId(id);

    /// <summary>
    ///  The actions that were used by player successfully. The first one is the latest successfully used one.
    /// </summary>
    protected static ActionRec[] RecordActions => DataCenter.RecordActions;

    /// <summary>
    /// How much time has passed since the last action was released.
    /// </summary>
    protected static TimeSpan TimeSinceLastAction => DataCenter.TimeSinceLastAction;

    /// <summary>
    /// Check for GCD Record.
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastGCD(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastGCD(isAdjust, actions);

    /// <summary>
    /// Check for GCD Record.
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastGCD(params ActionID[] ids)
        => IActionHelper.IsLastGCD(ids);

    /// <summary>
    /// Check for ability Record.
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAbility(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastAbility(isAdjust, actions);

    /// <summary>
    /// Check for ability Record.
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAbility(params ActionID[] ids)
        => IActionHelper.IsLastAbility(ids);

    /// <summary>
    /// Check for action Record.
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAction(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastAction(isAdjust, actions);

    /// <summary>
    /// Check for action Record.
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAction(params ActionID[] ids)
        => IActionHelper.IsLastAction(ids);

    /// <summary>
    /// Is the thing still there after <paramref name="gcdCount"/> gcds and <paramref name="offset"/> abilities.
    /// </summary>
    /// <param name="remain">jobgauge time</param>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool EndAfterGCD(float remain, uint gcdCount = 0, float offset = 0)
        => CooldownHelper.RecastAfterGCD(remain, gcdCount, offset);

    /// <summary>
    /// Is the thing still there after <paramref name="remainNeed"/> seconds
    /// </summary>
    /// <param name="remain">jobgauge time</param>
    /// <param name="remainNeed">seconds</param>
    /// <returns></returns>
    protected static bool EndAfter(float remain, float remainNeed)
        => CooldownHelper.RecastAfter(remain, remainNeed);

    /// <summary>
    /// Whether the battle lasted less than <paramref name="time"/> seconds
    /// </summary>
    /// <param name="time">time in second.</param>
    /// <returns></returns>
    public static bool CombatElapsedLess(float time)
    {
        if (!InCombat) return true;
        return (DataCenter.CombatTime + DataCenter.WeaponRemain).IsLessThan(time);
    }

    public static bool CombatElapsedLessGCD(int GCD) => CombatElapsedLess(GCD * DataCenter.WeaponTotal);

    public MethodInfo[] AllLast => GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 2
            && types[0].ParameterType == typeof(bool)
            && types[1].ParameterType == typeof(IAction[]);
    });

    protected static int NumberOfHostilesIn(float range)
    => DataCenter.HostileTargets.Count(o => o.DistanceToPlayer() <= range);
}
