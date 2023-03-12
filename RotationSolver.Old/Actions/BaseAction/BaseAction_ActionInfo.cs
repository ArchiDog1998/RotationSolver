using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Linq;

namespace RotationSolver.Actions.BaseAction;

internal partial class BaseAction
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
                    if ((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id != ClassJobID.BlueMage
                        && ChoiceTarget != TargetFilter.FindTargetForMoving
                        && Watcher.LastAction == (ActionID)AdjustedID) return false;

                    if (!WillHaveOneCharge(ActionUpdater.AbilityRemain, false)) return false;
                }
            }

            return true;
        }
    }

    public unsafe virtual bool CanUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false, uint gcdCountForAbility = 0, bool recordTarget = true)
    {
        act = this;

        if (Service.ClientState.LocalPlayer == null) return false;
        var player = Service.ClientState.LocalPlayer;

        if (!skipDisable && !IsEnabled) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Spell, AdjustedID)))
            return false;

        if (!EnoughLevel) return false;

        if (Service.ClientState.LocalPlayer.CurrentMp < MPNeed) return false;

        if (StatusNeed != null)
        {
            if (!Service.ClientState.LocalPlayer.HasStatus(true, StatusNeed)) return false;
        }

        if (StatusProvide != null && !mustUse)
        {
            if (Service.ClientState.LocalPlayer.HasStatus(true, StatusProvide)) return false;
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
                if (RecastTimeRemain > ActionUpdater.WeaponRemain + ActionUpdater.WeaponTotal * gcdCountForAbility)
                    return false;
            }
        }

        if (CastTime > 0 && MovingUpdater.IsMoving &&
            !player.HasStatus(true, CustomRotation.Swiftcast.StatusProvide)) return false;

        if (!FindTarget(mustUse, out var target)) return false;

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
            if (ComboIdsNot.Contains(ActionUpdater.LastComboAction)) return false;
        }

        var comboActions = _action.ActionCombo?.Row != 0
            ? new ActionID[] { (ActionID)_action.ActionCombo.Row }
            : new ActionID[0];
        if (ComboIds != null) comboActions = comboActions.Union(ComboIds).ToArray();

        if (comboActions.Length > 0)
        {
            if (comboActions.Contains(ActionUpdater.LastComboAction))
            {
                if (ActionUpdater.ComboTime < ActionUpdater.WeaponRemain) return false;
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

        if (ShouldEndSpecial) RSCommands.ResetSpecial();

        return _action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ID, Service.ClientState.LocalPlayer.ObjectId, &loc) :
            ActionManager.Instance()->UseAction(ActionType.Spell, AdjustedID, _targetId);
    }
}
