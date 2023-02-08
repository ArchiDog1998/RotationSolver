using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Linq;
using System.Reflection;
using static RotationSolver.SigReplacers.Watcher;

namespace RotationSolver.Rotations.CustomRotation;

internal abstract partial class CustomRotation
{
    /// <summary>
    /// This is the player.
    /// </summary>
    protected static PlayerCharacter Player => Service.ClientState.LocalPlayer;

    /// <summary>
    /// The level of the player.
    /// </summary>
    protected static byte Level => Player?.Level ?? 0;

    /// <summary>
    /// Does player have swift cast, dual cast or triple cast.
    /// </summary>
    internal static bool HasSwift => Player?.HasStatus(true, Swiftcast.StatusProvide) ?? false;

    /// <summary>
    /// 
    /// </summary>
    internal static bool HasShield => Player?.HasStatus(true, StatusHelper.SheildStatus) ?? false;

    /// <summary>
    /// Check the player is moving, such as running, walking or jumping.
    /// </summary>
    protected static bool IsMoving => MovingUpdater.IsMoving;

    /// <summary>
    /// Whether the number of party members is 8.
    /// </summary>
    protected static bool IsFullParty => TargetUpdater.PartyMembers.Count() is 8;

    /// <summary>
    /// Is in burst right now? Usually it used with team support actions.
    /// </summary>
    protected static bool InBurst => RSCommands.SpecialType == SpecialCommandType.Burst || Service.Configuration.AutoBurst;

    /// <summary>
    /// Is in combat.
    /// </summary>
    protected static bool InCombat => ActionUpdater.InCombat;

    static RandomDelay _notInCombatDelay = new RandomDelay(() =>
        (Service.Configuration.NotInCombatDelayMin, Service.Configuration.NotInCombatDelayMax));
    protected static bool NotInCombatDelay => _notInCombatDelay.Delay(!InCombat);

    /// <summary>
    /// The player's target.
    /// </summary>
    protected static BattleChara Target => Service.TargetManager.Target is BattleChara b ? b : Player;

    /// <summary>
    /// Shortcut for Target.IsDying();
    /// </summary>
    internal static bool IsTargetDying => Target?.IsDying() ?? false;

    /// <summary>
    /// Shortcut for Target.IsBoss();
    /// </summary>
    internal static bool IsTargetBoss => Target?.IsBoss() ?? false;


    /// <summary>
    /// Is there any hostile target in the range? 25 for ranged jobs and healer, 3 for melee and tank.
    /// </summary>
    protected static bool HasHostilesInRange => TargetUpdater.HasHostilesInRange;

    private bool _canUseHealAction => Job.GetJobRole() == JobRole.Healer || Service.Configuration.UseHealWhenNotAHealer;

    protected virtual bool CanHealAreaAbility => TargetUpdater.CanHealAreaAbility && _canUseHealAction;

    protected virtual bool CanHealAreaSpell => TargetUpdater.CanHealAreaSpell && _canUseHealAction;

    protected virtual bool CanHealSingleAbility => TargetUpdater.CanHealSingleAbility && _canUseHealAction;

    protected virtual bool CanHealSingleSpell => TargetUpdater.CanHealSingleSpell && _canUseHealAction;

    /// <summary>
    /// Actions successfully released. The first one is the latest one.
    /// </summary>
    protected static ActionRec[] RecordActions => Watcher.RecordActions;

    /// <summary>
    /// How much time has passed since the last action was released.
    /// </summary>
    protected static TimeSpan TimeSinceLastAction => Watcher.TimeSinceLastAction;

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
    /// Is the thing still there after <paramref name="gcdCount"/> gcds and <paramref name="abilityCount"/> abilities.
    /// </summary>
    /// <param name="remain">jobgauge time</param>
    /// <param name="gcdCount"></param>
    /// <param name="abilityCount"></param>
    /// <returns></returns>
    protected static bool EndAfterGCD(float remain, uint gcdCount = 0, uint abilityCount = 0)
        => CooldownHelper.RecastAfterGCD(remain, gcdCount, abilityCount);

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
        return CooldownHelper.ElapsedAfter(time, (float)ActionUpdater.CombatTime.TotalSeconds);
    }

    public MethodInfo[] AllLast => GetType().GetStaticBoolMethodInfo(m =>
    {
        var types = m.GetParameters();
        return types.Length == 2
            && types[0].ParameterType == typeof(bool)
            && types[1].ParameterType == typeof(IAction[]);
    });
}
