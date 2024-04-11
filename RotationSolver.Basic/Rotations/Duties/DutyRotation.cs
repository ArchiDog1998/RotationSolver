using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RotationSolver.Basic.Rotations.Duties;

partial class DutyRotation : IDisposable
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    #region GCD
    public virtual bool EmergencyGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool GeneralGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool RaiseGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DispelGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveForwardGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealSingleGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealAreaGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseSingleGCD(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseAreaGCD(out IAction? act)
    {
        act = null; return false;
    }
    #endregion

    #region Ability
    public virtual bool InterruptAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool AntiKnockbackAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool ProvokeAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveForwardAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool MoveBackAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealSingleAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool HealAreaAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseSingleAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool DefenseAreaAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool SpeedAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool GeneralAbility(out IAction? act)
    {
        act = null; return false;
    }

    public virtual bool AttackAbility(out IAction? act)
    {
        act = null; return false;
    }

    #endregion
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
    /// Log
    /// </summary>
    public static IPluginLog Log => Svc.Log;

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

    /// <summary>
    /// The map effects.
    /// </summary>
    public static IEnumerable<MapEffectData> MapEffects => DataCenter.MapEffects.Reverse();

    /// <summary>
    /// The object Effects.
    /// </summary>
    public static IEnumerable<ObjectEffectData> ObjectEffects => DataCenter.ObjectEffects.Reverse();

    /// <summary>
    /// The vfx effects.
    /// </summary>
    public static IEnumerable<VfxNewData> VfxNewData => DataCenter.VfxNewData.Reverse();
    #endregion

    #region Duty
    /// <summary>
    /// The timeline Items.
    /// </summary>
    public static TimelineItem[] TimelineItems => DataCenter.TimelineItems;

    /// <summary>
    /// The Raid Time.
    /// </summary>
    public static float RaidTimeRaw => DataCenter.RaidTimeRaw;
    #endregion

    #region Drawing
    /// <summary>
    /// When anyone casting this, what should do.
    /// </summary>
    protected virtual Dictionary<(float time, uint actionId), Action> CastingAction { get; } = [];
    private readonly Dictionary<(float time, uint actionId), DateTime> _usedTime = [];
    internal void UpdateInfo()
    {
        UpdateCasting();
        UpdateDrawing();
    }

    private void UpdateCasting()
    {
        if (CastingAction.Count == 0) return;

        foreach (var target in DataCenter.AllHostileTargets)
        {
            if (!target.IsCasting) continue;
            if (target.TotalCastTime < 2.5) continue;

            var last = target.TotalCastTime - target.CurrentCastTime;
            var id = target.CastActionId;
            CanInvoke(last, id);
        }
    }

    private void CanInvoke(float last, uint id)
    {
        foreach ((var key, var value) in CastingAction)
        {
            if (key.actionId != id) continue;
            if (key.time < last) continue;
            if (key.time - 1 > last) continue;
            if (_usedTime.TryGetValue(key, out var time))
            {
                if ((DateTime.Now - time).TotalSeconds < 1.5) continue;
            }

            _usedTime[key] = DateTime.Now;
            value?.Invoke();
        }
    }

    /// <summary>
    /// Update every frame for drawings
    /// </summary>
    public virtual void UpdateDrawing()
    {

    }

    /// <summary>
    /// When a new actor showned.
    /// </summary>
    /// <param name="actor"></param>
    public virtual void OnNewActor(GameObject actor)
    {

    }

    /// <summary>
    /// When a new actor effect is created.
    /// </summary>
    /// <param name="data"></param>
    public virtual void OnActorVfxNew(in VfxNewData data)
    {

    }

    /// <summary>
    /// When on the object effect
    /// </summary>
    /// <param name="data"></param>
    public virtual void OnObjectEffect(in ObjectEffectData data)
    {

    }

    /// <summary>
    /// When on the map effect.
    /// </summary>
    /// <param name="data"></param>
    public virtual void OnMapEffect(in MapEffectData data)
    {

    }

    /// <summary>
    /// Any actions from the enemy to us.
    /// </summary>
    /// <param name="data"></param>
    public virtual void OnActionFromEnemy(in ActionEffectSet data)
    {

    }

    /// <summary>
    /// To destroy all drawing.
    /// </summary>
    public virtual void DestroyAllDrawing()
    {

    }
    #endregion

    /// <inheritdoc/>
    public void Dispose()
    {
        DestroyAllDrawing();
        GC.SuppressFinalize(this);
    }

    internal IAction[] AllActions
    {
        get
        {
            var properties = this.GetType().GetRuntimeProperties()
                .Where(p => DataCenter.DutyActions.Contains(p.GetCustomAttribute<IDAttribute>()?.ID ?? uint.MaxValue));

            if (properties == null || !properties.Any()) return [];

            return [.. properties.Select(p => (IAction)p.GetValue(this)!)];
        }
    }
}
