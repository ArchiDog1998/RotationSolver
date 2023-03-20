using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Actions.BaseAction;

public partial class BaseAction
{
    public float Range => ActionManager.GetActionRange(ID);

    public ActionID[] ComboIdsNot { private get; set; } = null;

    public ActionID[] ComboIds { private get; set; } = null;

    public StatusID[] StatusProvide { get; set; } = null;

    public virtual StatusID[] StatusNeed { get; set; } = null;

    public Func<BattleChara, bool> ActionCheck { get; set; } = null;

    public Func<BattleChara, bool> RotationCheck { get; set; } = null;

    private bool WillCooldown
    {
        get
        {
            if (!IsGeneralGCD && IsCoolingDown)
            {
                if (IsRealGCD)
                {
                    if (!WillHaveOneChargeGCD()) return false;
                }
                else
                {
                    if ((ClassJobID)Service.Player.ClassJob.Id != ClassJobID.BlueMage
                        && ChoiceTarget != TargetFilter.FindTargetForMoving
                        && DataCenter.LastAction == (ActionID)AdjustedID) return false;

                    if (!WillHaveOneCharge(DataCenter.AbilityRemain, false)) return false;
                }
            }

            return true;
        }
    }

    public unsafe virtual bool CanUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false, uint gcdCountForAbility = 0, bool recordTarget = true)
    {
        act = this;

        var player = Service.Player;
        if (player == null) return false;

        if (!skipDisable && !IsEnabled) return false;

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

        if (!emptyOrSkipCombo)
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

        if (CastTime > 0 && DataCenter.IsMoving &&
            !player.HasStatus(true, CustomRotation.Swiftcast.StatusProvide)) return false;

        if (!FindTarget(mustUse, out var target) || target == null) return false;

        if (ActionCheck != null && !ActionCheck(target)) return false;
        if (!skipDisable && RotationCheck != null && !RotationCheck(target)) return false;

        Target = target;
        if(recordTarget) _targetId = target.ObjectId;
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

        return _action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.Player.ObjectId, &loc) :
            ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, _targetId);
    }
}
