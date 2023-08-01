namespace RotationSolver.Basic.Actions;

/// <summary>
/// The base action.
/// </summary>
public interface IBaseAction : IAction
{
    /// <summary>
    /// Attack Type
    /// </summary>
    AttackType AttackType { get; }

    /// <summary>
    /// Aspect
    /// </summary>
    Aspect Aspect { get; }

    /// <summary>
    /// MP for casting.
    /// </summary>
    uint MPNeed { get; }

    /// <summary>
    /// Casting time
    /// </summary>
    float CastTime { get; }

    /// <summary>
    /// Range of this action.
    /// </summary>
    float Range { get; }

    /// <summary>
    /// Effect range of this action.
    /// </summary>
    float EffectRange { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool IsFriendly { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool IsEot { get; }

    /// <summary>
    /// If player has these statuses from player self, this action will not used.
    /// </summary>
    StatusID[] StatusProvide { get; }

    /// <summary>
    /// If player doesn't have these statuses from player self, this action will not used.
    /// </summary>
    StatusID[] StatusNeed { get; }

    /// <summary>
    /// Check for this action, but not for the rotation. It is some additional conditions for this action.
    /// Input data is the target for this action.
    /// </summary>
    Func<BattleChara, bool, bool> ActionCheck { get; }

    /// <summary>
    /// The way to choice the target.
    /// </summary>
    Func<IEnumerable<BattleChara>, bool, BattleChara> ChoiceTarget { get; }

    /// <summary>
    /// Is a GCD action.
    /// </summary>
    bool IsRealGCD { get; }

    /// <summary>
    /// Is a simple gcd action, without other cooldown.
    /// </summary>
    bool IsGeneralGCD { get; }

    /// <summary>
    /// The filter for hostiles.
    /// </summary>
    Func<IEnumerable<BattleChara>, IEnumerable<BattleChara>> FilterForHostiles { get; }

    /// <summary>
    /// Is this action a duty action.
    /// </summary>
    bool IsDutyAction { get; }

    /// <summary>
    /// Is this duty action on the slot.
    /// </summary>
    bool IsDutyActionOnSlot { get; }

    /// <summary>
    /// Can I use this action at this time. It will check a lot of things.
    /// Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.
    /// </summary>
    /// <param name="act"></param>
    /// <param name="option">Options about using this method.</param>
    /// <param name="aoeCount">How many targets do you want this skill to affect</param>
    /// <param name="gcdCountForAbility">The count of gcd for ability to delay. Only used in BLM right now</param>
    /// <returns>Should I use.</returns>
    bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte aoeCount = 0, byte gcdCountForAbility = 0);

    #region CoolDown
    /// <summary>
    /// Current charges count.
    /// </summary>
    ushort CurrentCharges { get; }

    /// <summary>
    /// Max charges count.
    /// </summary>
    ushort MaxCharges { get; }

    /// <summary>
    /// At least has one Charge
    /// </summary>
    bool HasOneCharge { get; }

    /// <summary>
    /// Has it been in cooldown for <paramref name="gcdCount"/> gcds and <paramref name="offset"/> abilities?
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    bool ElapsedOneChargeAfterGCD(uint gcdCount = 0, float offset = 0);

    /// <summary>
    /// Has it been in cooldown for <paramref name="time"/> seconds?
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    bool ElapsedOneChargeAfter(float time);

    /// <summary>
    /// Has it been in cooldown for <paramref name="gcdCount"/> gcds and <paramref name="offset"/> abilities?
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    bool ElapsedAfterGCD(uint gcdCount = 0, float offset = 0);

    /// <summary>
    /// Has it been in cooldown for <paramref name="time"/> seconds?
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    bool ElapsedAfter(float time);

    /// <summary>
    /// Will have at least one charge after <paramref name="gcdCount"/> gcds and <paramref name="offset"/> abilities?
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    bool WillHaveOneChargeGCD(uint gcdCount = 0, float offset = 0);

    /// <summary>
    /// Will have at least one charge after <paramref name="remain"/> seconds?
    /// </summary>
    /// <param name="remain"></param>
    /// <returns></returns>
    bool WillHaveOneCharge(float remain);
    #endregion

    #region Target
    /// <summary>
    /// If target has these statuses from player self, this aciton will not used.
    /// </summary>
    StatusID[] TargetStatus { get; }

    /// <summary>
    /// Action using position.
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// The target of this action.
    /// </summary>
    BattleChara Target { get; }

    /// <summary>
    /// Is target a boss.
    /// </summary>
    bool IsTargetBoss { get; }

    /// <summary>
    /// Is target will die immediately.
    /// </summary>
    bool IsTargetDying { get; }

    /// <summary>
    /// Is this action's target type is target only one.
    /// </summary>
    bool IsSingleTarget { get; }
    #endregion
}
