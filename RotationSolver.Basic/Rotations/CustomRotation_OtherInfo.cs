using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.Basic.Rotations;
partial class CustomRotation
{
    #region Player
    /// <summary>
    /// This is the player.
    /// </summary>
    protected static PlayerCharacter Player => ECommons.GameHelpers.Player.Object;

    /// <summary>
    /// Does player have swift cast, dual cast or triple cast.
    /// </summary>
    [Description("Has Swift")]
    public static bool HasSwift => Player?.HasStatus(true, StatusHelper.SwiftcastStatus) ?? false;

    /// <summary>
    /// 
    /// </summary>
    [Description("Has tank stance")]
    public static bool HasTankStance => Player?.HasStatus(true, StatusHelper.TankStanceStatus) ?? false;

    /// <summary>
    /// Check the player is moving, such as running, walking or jumping.
    /// </summary>
    [Description("Is Moving or Jumping")]
    public static bool IsMoving => DataCenter.IsMoving;

    /// <summary>
    /// Is in combat.
    /// </summary>
    [Description("In Combat")]
    public static bool InCombat => DataCenter.InCombat;

    /// <summary>
    /// Is out of combat.
    /// </summary>
    [Description("Not In Combat Delay")]
    public static bool NotInCombatDelay => DataCenter.NotInCombatDelay;

    /// <summary>
    /// Player's MP.
    /// </summary>
    [Description("Player's MP")]
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
    [Description("Has companion")]
    public static bool HasCompanion => DataCenter.HasCompanion;

    /// <summary>
    /// Party member.
    /// </summary>
    protected static IEnumerable<BattleChara> PartyMembers => DataCenter.PartyMembers;

    /// <summary>
    /// Alliance members.
    /// </summary>
    protected static IEnumerable<BattleChara> AllianceMembers => DataCenter.AllianceMembers;

    /// <summary>
    /// Whether the number of party members is 8.
    /// </summary>
    [Description("Is Full Party")]
    public static bool IsFullParty => PartyMembers.Count() is 8;

    /// <summary>
    /// party members HP.
    /// </summary>
    protected static IEnumerable<float> PartyMembersHP => DataCenter.PartyMembersHP;

    /// <summary>
    /// Min HP in party members.
    /// </summary>
    [Description("Min HP in party members.")]
    public static float PartyMembersMinHP => DataCenter.PartyMembersMinHP;

    /// <summary>
    /// Average HP in party members.
    /// </summary>
    [Description("Average HP in party members.")]
    public static float PartyMembersAverHP => DataCenter.PartyMembersAverHP;
    #endregion

    #region Target
    /// <summary>
    /// The player's target.
    /// <br>WARNING: You'd better not use it. Because this target isn't the action's target. Try to use <see cref="IBaseAction.Target"/> or <seealso cref="HostileTarget"/> instead after using <seealso cref="IBaseAction.CanUse(out IAction, bool, bool, bool, bool, bool, bool, bool, byte)"/></br>
    /// </summary>
    [Obsolete("You'd better not use it. More information in summary.")]
    protected static BattleChara Target => Svc.Targets.Target is BattleChara b ? b : Player;

    /// <summary>
    /// The player's target, or null if no valid target. (null clears the target)
    /// </summary>
    protected static BattleChara? CurrentTarget => Svc.Targets.Target is BattleChara b ? b : null;

    /// <summary>
    /// The last attacked hostile target.
    /// </summary>
    protected static BattleChara? HostileTarget => DataCenter.HostileTarget;

    /// <summary>
    /// Is there any hostile target in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    [Description("Has hostiles in Range")]
    public static bool HasHostilesInRange => DataCenter.HasHostilesInRange;

    /// <summary>
    /// Is there any hostile target in 25 yalms?
    /// </summary>
    [Description("Has hostiles in 25 yalms")]
    public static bool HasHostilesInMaxRange => DataCenter.HasHostilesInMaxRange;

    /// <summary>
    /// How many hostile targets in range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    [Description("The number of hostiles in Range")]
    public static int NumberOfHostilesInRange => DataCenter.NumberOfHostilesInRange;

    /// <summary>
    /// How many hostile targets in max range (25 yalms) regardless of job
    /// </summary>
    [Description("The number of hostiles in max Range")]
    public static int NumberOfHostilesInMaxRange => DataCenter.NumberOfHostilesInMaxRange;

    /// <summary>
    /// How many hostile targets in range? 25 for ranged jobs and healer, 3 for melee and tank. This is all can attack.
    /// </summary>
    [Description("The number of all hostiles in Range")]
    public static int NumberOfAllHostilesInRange => DataCenter.NumberOfAllHostilesInRange;

    /// <summary>
    /// How many hostile targets in max range (25 yalms) regardless of job. This is all can attack.
    /// </summary>
    [Description("The number of all hostiles in max Range")]
    public static int NumberOfAllHostilesInMaxRange => DataCenter.NumberOfAllHostilesInMaxRange;

    /// <summary>
    /// All hostile Targets. This is all can attack.
    /// </summary>
    protected static IEnumerable<BattleChara> AllHostileTargets => DataCenter.AllHostileTargets;

    /// <summary>
    /// Average dead time of hostiles.
    /// </summary>
    [Description("Average time to kill")]
    public static float AverageTimeToKill => DataCenter.AverageTimeToKill;

    /// <summary>
    /// The level of LB.
    /// </summary>
    [Description("Limit Break Level")]
    public unsafe static byte LimitBreakLevel
    {
        get
        {
            var controller = UIState.Instance()->LimitBreakController;
            var barValue = *(ushort*)&controller.BarValue;
            if (barValue == 0) return 0;
            return (byte)(controller.CurrentValue / barValue);
        }
    }

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
    [Description("Mobs Time")]
    public static bool MobsTime => DataCenter.MobsTime;
    #endregion

    #region Others
    /// <summary>
    /// True for On, false for off.
    /// </summary>
    [Description("The state of auto. True for on.")]
    public static bool AutoState => DataCenter.State;

    /// <summary>
    /// Ture for Manual Target, false for Auto Target.
    /// </summary>
    [Description("The state of manual. True for manual.")]
    public static bool IsManual => DataCenter.IsManual;

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
    [Description("Is in the high-end duty")]
    public static bool IsInHighEndDuty => DataCenter.IsInHighEndDuty;

    /// <summary>
    /// Is player in duty.
    /// </summary>
    [Description("Is player in duty")]
    public static bool IsInDuty => Svc.Condition[ConditionFlag.BoundByDuty];

    /// <summary>
    /// 
    /// </summary>
    [Description("Your ping")]
    public static float Ping => DataCenter.Ping;

    /// <summary>
    /// 
    /// </summary>
    [Description("Time from next ability to next GCD")]
    public static float NextAbilityToNextGCD => DataCenter.NextAbilityToNextGCD;

    public static IEnumerable<MapEffectData> MapEffects => DataCenter.MapEffects.Reverse();
    public static IEnumerable<ObjectEffectData> ObjectEffects => DataCenter.ObjectEffects.Reverse();
    public static IEnumerable<VfxNewData> VfxNewData => DataCenter.VfxNewData.Reverse();
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [Description("Can heal area ability")]
    public virtual bool CanHealAreaAbility => true;

    /// <summary>
    /// 
    /// </summary>
    [Description("Can heal area spell")]
    public virtual bool CanHealAreaSpell => true;

    /// <summary>
    /// 
    /// </summary>
    [Description("Can heal single ability")]
    public virtual bool CanHealSingleAbility => true;

    /// <summary>
    /// 
    /// </summary>
    [Description("Can heal single area")]
    public virtual bool CanHealSingleSpell => true;

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
    [Description("Just used GCD")]
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
    [Description("Just used Ability")]
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
    [Description("Just used Action")]
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
        CountingOfCombatTimeUsing++;
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
    [Description("Combat time")]
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
    [Description("Stop moving time")]
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
    [Description("Count Down ahead")]
    public static float CountDownAhead => Service.Config.CountDownAhead;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of Area Ability")]
    public static float HealthAreaAbility => Service.Config.HealthAreaAbility;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of Area spell")]
    public static float HealthAreaSpell => Service.Config.HealthAreaSpell;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of Area Ability Hot")]
    public static float HealthAreaAbilityHot => Service.Config.HealthAreaAbilityHot;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of Area spell Hot")]
    public static float HealthAreaSpellHot => Service.Config.HealthAreaSpellHot;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of single ability")]
    public static float HealthSingleAbility => Service.Config.HealthSingleAbility;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of single spell")]
    public static float HealthSingleSpell => Service.Config.HealthSingleSpell;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of single ability Hot")]
    public static float HealthSingleAbilityHot => Service.Config.HealthSingleAbilityHot;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of single spell Hot")]
    public static float HealthSingleSpellHot => Service.Config.HealthSingleSpellHot;

    /// <summary>
    /// 
    /// </summary>
    [Description("Health of dying tank")]
    public static float HealthForDyingTanks => Service.Config.HealthForDyingTanks;
    #endregion

    /// <summary>
    /// In in the burst status.
    /// </summary>
    [Description("Is burst")]
    public static bool IsBurst => MergedStatus.HasFlag(AutoStatus.Burst);

    /// <summary>
    /// The merged status, which contains <see cref="AutoState"/> and <see cref="CommandStatus"/>.
    /// </summary>
    public static AutoStatus MergedStatus => DataCenter.MergedStatus;

    /// <summary>
    /// The automatic status, which is checked from RS.
    /// </summary>
    public static AutoStatus AutoStatus => DataCenter.AutoStatus;

    /// <summary>
    /// The CMD status, which is checked from the player.
    /// </summary>
    public static AutoStatus CommandStatus => DataCenter.CommandStatus;
}
