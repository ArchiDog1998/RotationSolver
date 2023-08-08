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
    [Obsolete("Please use EnoughLevel of action or trait instead.", true)]
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

    /// <summary>
    /// Is out of combat.
    /// </summary>
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
    /// <summary>
    /// Has the comapnion now.
    /// </summary>
    protected static bool HasCompanion => DataCenter.HasCompanion;

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
    protected static float RatioOfMembersIn2minsBurst => DataCenter.RatioOfMembersIn2minsBurst;


    /// <summary>
    /// Whether the number of party members is 8.
    /// </summary>
    protected static bool IsFullParty => PartyMembers.Count() is 8;

    /// <summary>
    /// party members HP.
    /// </summary>
    protected static IEnumerable<float> PartyMembersHP => DataCenter.PartyMembersHP;

    /// <summary>
    /// Min HP in party members.
    /// </summary>
    protected static float PartyMembersMinHP => DataCenter.PartyMembersMinHP;

    /// <summary>
    /// Average HP in party members.
    /// </summary>
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

    /// <summary>
    /// All hostile Targets.
    /// </summary>
    protected static IEnumerable<BattleChara> HostileTargets => DataCenter.HostileTargets;

    #endregion

    #region Command

    /// <summary>
    /// Is in burst right now? Usually it used with team support actions.
    /// </summary>
    protected static bool InBurst => DataCenter.SpecialType == SpecialCommandType.Burst || Service.Config.GetValue(SettingsCommand.AutoBurst);

    bool _canUseHealAction => (ClassJob.GetJobRole() == JobRole.Healer || Service.Config.UseHealWhenNotAHealer) && Service.Config.GetValue(SettingsCommand.AutoHeal);

    /// <summary>
    /// 
    /// </summary>
    protected virtual bool CanHealAreaAbility => DataCenter.CanHealAreaAbility && _canUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    protected virtual bool CanHealAreaSpell => DataCenter.CanHealAreaSpell && _canUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    protected virtual bool CanHealSingleAbility => DataCenter.CanHealSingleAbility && _canUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    protected virtual bool CanHealSingleSpell => DataCenter.CanHealSingleSpell && _canUseHealAction;

    /// <summary>
    /// 
    /// </summary>
    protected static SpecialCommandType SpecialType => DataCenter.SpecialType;

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Please use State or IsManual instead!", true)]
    protected static StateCommandType StateType => StateCommandType.None;

    /// <summary>
    /// True for On, false for off.
    /// </summary>
    protected static bool State => DataCenter.State;

    /// <summary>
    /// Ture for Manual Target, false for Auto Target.
    /// </summary>
    protected static bool IsManual => DataCenter.IsManual;
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
    protected static bool IsInHighEndDuty => DataCenter.IsInHighEndDuty;

    /// <summary>
    /// 
    /// </summary>
    protected static float Ping => DataCenter.Ping;

    /// <summary>
    /// 
    /// </summary>
    protected static float NextAbilityToNextGCD => DataCenter.NextAbilityToNextGCD;

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
    ///  The actions that were used by player successfully. The first one is the latest successfully used one.
    /// </summary>
    protected static ActionRec[] RecordActions => DataCenter.RecordActions;

    /// <summary>
    /// How much time has passed since the last action was released.
    /// </summary>
    protected static TimeSpan TimeSinceLastAction => DataCenter.TimeSinceLastAction;

    /// <summary>
    /// Check for GCD Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastGCD(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastGCD(isAdjust, actions);

    /// <summary>
    /// Check for GCD Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastGCD(params ActionID[] ids)
        => IActionHelper.IsLastGCD(ids);

    /// <summary>
    /// Check for ability Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAbility(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastAbility(isAdjust, actions);

    /// <summary>
    /// Check for ability Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAbility(params ActionID[] ids)
        => IActionHelper.IsLastAbility(ids);

    /// <summary>
    /// Check for action Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="isAdjust">Check for adjust id not raw id.</param>
    /// <param name="actions">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAction(bool isAdjust, params IAction[] actions)
        => IActionHelper.IsLastAction(isAdjust, actions);

    /// <summary>
    /// Check for action Record.
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="ids">True if any of this is matched.</param>
    /// <returns></returns>
    protected static bool IsLastAction(params ActionID[] ids)
        => IActionHelper.IsLastAction(ids);

    /// <summary>
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="GCD"></param>
    /// <returns></returns>
    protected static bool CombatElapsedLessGCD(int GCD) => CombatElapsedLess(GCD * DataCenter.WeaponTotal);

    /// <summary>
    /// Whether the battle lasted less than <paramref name="time"/> seconds
    /// <br>WARNING: Do Not make this method the main of your rotation.</br>
    /// </summary>
    /// <param name="time">time in second.</param>
    /// <returns></returns>
    protected static bool CombatElapsedLess(float time) => CombatTime <= time;

    /// <summary>
    /// 
    /// </summary>
    protected static float CombatTime => InCombat ? DataCenter.CombatTimeRaw + DataCenter.WeaponRemain : 0;

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
    /// The time of stoping moving.
    /// </summary>
    protected static float StopMovingTime => IsMoving ? 0 : DataCenter.StopMovingRaw + DataCenter.WeaponRemain;

    /// <summary>
    /// Time from GCD.
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static float GCDTime(uint gcdCount = 0, float offset = 0)
        => DataCenter.GCDTime(gcdCount, offset);

    /// <summary>
    /// All last actions.
    /// </summary>
    public MethodInfo[] AllLast => GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 2
            && types[0].ParameterType == typeof(bool)
            && types[1].ParameterType == typeof(IAction[]);
    });

    /// <summary>
    /// The number of hostils in range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    [Obsolete]
    protected static int NumberOfHostilesIn(float range)
    => DataCenter.HostileTargets.Count(o => o.DistanceToPlayer() <= range);

    #region Service
    /// <summary>
    /// The countDond ahead.
    /// </summary>
    protected static float CountDownAhead => Service.Config.CountDownAhead;

    /// <summary>
    /// 
    /// </summary>
    protected float HealthAreaAbility => Jobs.FirstOrDefault().GetHealthAreaAbility();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthAreaSpell => Jobs.FirstOrDefault().GetHealthAreaSpell();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthAreaAbilityHot => Jobs.FirstOrDefault().GetHealthAreaAbilityHot();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthAreaSpellHot => Jobs.FirstOrDefault().GetHealthAreaSpellHot();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthSingleAbility => Jobs.FirstOrDefault().GetHealthSingleAbility();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthSingleSpell => Jobs.FirstOrDefault().GetHealthSingleSpell();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthSingleAbilityHot => Jobs.FirstOrDefault().GetHealthSingleAbilityHot();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthSingleSpellHot => Jobs.FirstOrDefault().GetHealthSingleSpellHot();

    /// <summary>
    /// 
    /// </summary>
    protected float HealthForDyingTanksDefault => Jobs.FirstOrDefault().GetHealthForDyingTank();
    #endregion
}
