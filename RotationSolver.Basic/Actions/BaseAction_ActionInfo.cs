using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    public float Range => ActionManager.GetActionRange(ID);

    internal ActionID[] ComboIdsNot { private get; init; } = null;

    internal ActionID[] ComboIds { private get; init; } = null;

    public StatusID[] StatusProvide { get; init; } = null;

    public virtual StatusID[] StatusNeed { get; init; } = null;

    public Func<BattleChara, bool> ActionCheck { get; init; } = null;

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
                    if ((ClassJobID)Service.Player.ClassJob.Id != ClassJobID.BlueMage
                        && ChoiceTarget != TargetFilter.FindTargetForMoving
                        && DataCenter.LastAction == (ActionID)AdjustedID) return false;

                    if (!WillHaveOneCharge(DataCenter.ActionRemain, false)) return false;
                }
            }

            return true;
        }
    }

    public unsafe virtual bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte gcdCountForAbility = 0)
    {
        option |= OtherOption;

        act = this;
        var mustUse = option.HasFlag(CanUseOption.MustUse);

        var player = Service.Player;
        if (player == null) return false;

        if (!option.HasFlag(CanUseOption.SkipDisable) && !IsEnabled) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID)))
            return false;

        if (!EnoughLevel) return false;

        if (player.CurrentMp < MPNeed) return false;

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

        if (!option.HasFlag(CanUseOption.IgnoreCastCheck) && CastTime > 0 && DataCenter.IsMoving &&
            !player.HasStatus(true, CustomRotation.Swiftcast.StatusProvide)) return false;

        if (IsGeneralGCD && IsEot && IsFriendly && IActionHelper.IsLastGCD(true, this)
            && DataCenter.TimeSinceLastAction.TotalSeconds < 3) return false;

        if (!FindTarget(mustUse, out var target) || target == null) return false;

        if (ActionCheck != null && !ActionCheck(target)) return false;

        Target = target;
        if(!option.HasFlag(CanUseOption.IgnoreTarget)) _targetId = target.ObjectId;
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
            : new ActionID[0];
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

    public unsafe bool Use()
    {
        var loc = new FFXIVClientStructs.FFXIV.Common.Math.Vector3() { X = _position.X, Y = _position.Y, Z = _position.Z };

        if (_action.TargetArea)
        {
            return ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.Player.ObjectId, &loc);
        }
        else if(Service.ObjectTable.SearchById(_targetId) == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, _targetId);
        }
    }
}
