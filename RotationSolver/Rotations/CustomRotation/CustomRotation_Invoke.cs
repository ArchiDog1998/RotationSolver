using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using RotationSolver.Windows;
using System;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;

public abstract partial class CustomRotation
{
    public bool TryInvoke(out IAction newAction)
    {
        newAction = null;
        if (!IsEnabled)
        {
            return false;
        }

        MoveTarget = (MoveForwardAbility(1, out var act, recordTarget: false) && act is BaseAction a) ? a.Target : null;
        UpdateInfo();

        newAction = Invoke();
        //没获得对象
        return newAction != null;
    }

    private IAction Invoke()
    {
        var countDown = CountDown.CountDownTime;
        if (countDown > 0) return CountDownAction(countDown);

        byte abilityRemain = ActionUpdater.AbilityRemainCount;
        var helpDefenseAOE = Service.Configuration.UseDefenceAbility && TargetUpdater.IsHostileCastingAOE;

        bool helpDefenseSingle = false;
        if (Job.GetJobRole() == JobRole.Healer || Job.RowId == (uint)ClassJobID.Paladin)
        {
            if (TargetUpdater.PartyTanks.Any((tank) =>
            {
                var attackingTankObj = TargetUpdater.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                if (attackingTankObj.Count() != 1) return false;

                return TargetUpdater.IsHostileCastingToTank;
            })) helpDefenseSingle = true;
        }

        IAction act = GCD(abilityRemain, helpDefenseAOE, helpDefenseSingle);

        OverlayWindow.MeleeAction = null;
        if (act != null && act is IBaseAction GcdAction)
        {
            if (GcdAction.IsMeleeAction())
            {
                OverlayWindow.MeleeAction = GcdAction;
                //Sayout!
                if (GcdAction.EnermyPositonal != EnemyPositional.None && GcdAction.Target.HasPositional()
                     && !Player.HasStatus(true, StatusID.TrueNorth))
                {
                    if (CheckAction(GcdAction.ID))
                    {
                        string positional = GcdAction.EnermyPositonal.ToName();
                        if (Service.Configuration.SayPotional) Watcher.Speak(positional);
                        if (Service.Configuration.FlytextPositional) Service.ToastGui.ShowQuest(" " + positional, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = GcdAction.IconID,
                        });
                    }
                }
            }

            if (abilityRemain == 0 || ActionUpdater.WeaponTotal < ActionUpdater._lastCastingTotal) return GcdAction;

            if (Ability(abilityRemain, GcdAction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

            return GcdAction;
        }
        else if (act == null)
        {
            if (Ability(abilityRemain, Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
            return null;
        }
        return act;
    }

    protected virtual IAction CountDownAction(float remainTime) => null;


    uint _lastSayingGCDAction;
    DateTime lastTime;
    private bool CheckAction(uint actionID)
    {
        if ((_lastSayingGCDAction != actionID || DateTime.Now - lastTime > new TimeSpan(0, 0, 3)) && RSCommands.StateType != StateCommandType.Cancel)
        {
            _lastSayingGCDAction = actionID;
            lastTime = DateTime.Now;
            return true;
        }
        else return false;
    }
}
