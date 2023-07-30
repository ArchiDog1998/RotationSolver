using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    /// <summary>
    /// The range of the action.
    /// </summary>
    public float Range => ActionManager.GetActionRange(ID);

    /// <summary>
    /// The effect range of the action.
    /// </summary>
    public float EffectRange => (ActionID)ID == ActionID.LiturgyOfTheBell ? 20 : _action?.EffectRange ?? 0;
    internal ActionID[] ComboIdsNot { private get; init; } = null;

    internal ActionID[] ComboIds { private get; init; } = null;

    /// <summary>
    /// Status that this action provides.
    /// </summary>
    public StatusID[] StatusProvide { get; init; } = null;

    /// <summary>
    /// Status that this action needs.
    /// </summary>
    public virtual StatusID[] StatusNeed { get; init; } = null;

    /// <summary>
    /// Some check that this action needs.
    /// </summary>
    public Func<BattleChara, bool, bool> ActionCheck { get; init; } = null;

    private bool WillCooldown
    {
        get
        {
            if (!IsGeneralGCD && IsCoolingDown)
            {
                if (IsRealGCD)
                {
                    if (!WillHaveOneChargeGCD(0, 0)) return false;
                }
                else
                {
                    if ((Job)Player.Object.ClassJob.Id != Job.BLU
                        && ChoiceTarget != TargetFilter.FindTargetForMoving
                        && DataCenter.LastAction == (ActionID)AdjustedID) return false;

                    if (!HasOneCharge && RecastTimeRemainOneChargeRaw > DataCenter.ActionRemain) return false;
                }
            }

            return true;
        }
    }

    internal static bool SkipDisable { get; set; } = false;

    /// <summary>
    /// Can this action be used.
    /// </summary>
    /// <param name="act"></param>
    /// <param name="option"></param>
    /// <param name="aoeCount"></param>
    /// <param name="gcdCountForAbility"></param>
    /// <returns></returns>
    public unsafe virtual bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte aoeCount = 0, byte gcdCountForAbility = 0)
    {
        option |= OtherOption;

        act = this;
        var mustUse = option.HasFlag(CanUseOption.MustUse);

        var player = Player.Object;
        if (player == null) return false;
        Target = player;

        if (!SkipDisable && !IsEnabled) return false;
        
        if (DataCenter.DisabledAction != null && DataCenter.DisabledAction.Contains(ID)) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID)))
            return false;

        if (!EnoughLevel) return false;

        if (DataCenter.CurrentMp < MPNeed) return false;

        if (StatusNeed != null)
        {
            if (!player.HasStatus(true, StatusNeed)) return false;
        }

        if (StatusProvide != null && !mustUse)
        {
            if (player.HasStatus(true, StatusProvide)) return false;
        }

        if (!WillCooldown) return false;

        if (!option.HasFlag(CanUseOption.EmptyOrSkipCombo))
        {
            if (IsGeneralGCD)
            {
                if (!CheckForCombo()) return false;
            }
            else
            {
                if (RecastTimeRemain > DataCenter.WeaponRemain + DataCenter.WeaponTotal * gcdCountForAbility)
                    return false;
            }
        }

        if(!IsRealGCD)
        {
            if (option.HasFlag(CanUseOption.OnLastAbility))
            {
                if (DataCenter.NextAbilityToNextGCD > AnimationLockTime + DataCenter.Ping + DataCenter.MinAnimationLock) return false;
            }
            else if (!option.HasFlag(CanUseOption.IgnoreClippingCheck))
            {
                if (DataCenter.NextAbilityToNextGCD < AnimationLockTime) return false;
            }
        }

        //Need casting.
        if (CastTime > 0 && !player.HasStatus(true, CustomRotation.Swiftcast.StatusProvide))
        {
            //Is knocking back.
            if (DateTime.Now > DataCenter.KnockbackStart && DateTime.Now < DataCenter.KnockbackFinished) return false;

            if (DataCenter.NoPoslock && DataCenter.IsMoving && !option.HasFlag(CanUseOption.IgnoreCastCheck)) return false;
        }

        if (IsGeneralGCD && IsEot && IsFriendly && IActionHelper.IsLastGCD(true, this)
            && DataCenter.TimeSinceLastAction.TotalSeconds < 3) return false;

        if (!FindTarget(mustUse, aoeCount, out var target) || target == null) return false;

        if (ActionCheck != null && !ActionCheck(target, mustUse)) return false;

        Target = target;
        if (!option.HasFlag(CanUseOption.IgnoreTarget)) _targetId = target.ObjectId;
        return true;
    }

    private bool CheckForCombo()
    {
        if (ComboIdsNot != null)
        {
            if (ComboIdsNot.Contains(DataCenter.LastComboAction)) return false;
        }

        var comboActions = _action.ActionCombo?.Row != 0
            ? new ActionID[] { (ActionID)_action.ActionCombo.Row }
            : Array.Empty<ActionID>();
        if (ComboIds != null) comboActions = comboActions.Union(ComboIds).ToArray();

        if (comboActions.Length > 0)
        {
            if (comboActions.Contains(DataCenter.LastComboAction))
            {
                if (DataCenter.ComboTime < DataCenter.WeaponRemain) return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Use this action.
    /// </summary>
    /// <returns></returns>
    public unsafe bool Use()
    {
        var loc = new FFXIVClientStructs.FFXIV.Common.Math.Vector3() { X = Position.X, Y = Position.Y, Z = Position.Z };

        if (_action.TargetArea)
        {
            return ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Player.Object.ObjectId, &loc);
        }
        else if(Svc.Objects.SearchById(_targetId) == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, _targetId);
        }
    }
}
