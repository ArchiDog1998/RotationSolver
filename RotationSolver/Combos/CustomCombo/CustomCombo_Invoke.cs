using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using RotationSolver.Windows;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;

namespace RotationSolver.Combos.CustomCombo;

internal abstract partial class CustomCombo
{
    public bool TryInvoke(out IAction newAction)
    {
        newAction = null;
        if (!IsEnabled)
        {
            return false;
        }

        UpdateInfo();

        newAction = Invoke();
        //没获得对象
        return newAction != null;
    }

    private IAction Invoke()
    {
        //倒计时专用能力。
        var countDown = CountDown.CountDownTime;
        if (countDown > 0) return CountDownAction(countDown);

        byte abilityRemain = ActionUpdater.AbilityRemainCount;

        //防AOE
        var helpDefenseAOE = Service.Configuration.UseDefenceAbility && TargetUpdater.IsHostileAOE;

        //防单体
        bool helpDefenseSingle = false;
        //是个骑士或者奶妈
        if (Job.GetJobRole() == JobRole.Healer || Service.ClientState.LocalPlayer.ClassJob.Id == 19)
        {
            if (TargetUpdater.PartyTanks.Any((tank) =>
            {
                var attackingTankObj = TargetUpdater.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                if (attackingTankObj.Count() != 1) return false;

                return TargetUpdater.IsHostileTank;
            })) helpDefenseSingle = true;
        }

        IAction act = GCD(abilityRemain, helpDefenseAOE, helpDefenseSingle);

        if (act != null && act is BaseAction GCDaction)
        {
            //Sayout!
            if (GCDaction.EnermyLocation != EnemyLocation.None && GCDaction.Target.HasLocationSide()
                 && !Player.HasStatus(true, StatusID.TrueNorth))
            {
                if (CheckAction(GCDaction.ID))
                {
                    string location = GCDaction.EnermyLocation.ToName();
                    if (Service.Configuration.SayingLocation) Watcher.Speak(location);
                    if (Service.Configuration.ShowLocation) Service.ToastGui.ShowQuest(" " + location, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                    {
                        IconId = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(
                            (uint)Service.IconReplacer.OriginalHook((ActionID)GCDaction.ID)).Icon,
                    });
                    OverlayWindow.EnemyLocationTarget = GCDaction.Target;
                    OverlayWindow.ShouldLocation = GCDaction.EnermyLocation;
                }
            }
            else
            {
                OverlayWindow.ShouldLocation = EnemyLocation.None;
            }

            if (abilityRemain == 0 || ActionUpdater.WeaponTotal < ActionUpdater._lastCastingTotal) return GCDaction;

            if (Ability(abilityRemain, GCDaction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

            return GCDaction;
        }
        else if (act == null)
        {
            OverlayWindow.ShouldLocation = EnemyLocation.None;
            if (Ability(abilityRemain, Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
            return null;
        }
        else
        {
            OverlayWindow.ShouldLocation = EnemyLocation.None;
        }
        return act;
    }

    uint _lastSayingGCDAction;
    DateTime lastTime;
    private bool CheckAction(uint actionID)
    {
        if ((_lastSayingGCDAction != actionID || DateTime.Now - lastTime > new TimeSpan(0, 0, 3)) && RotationSolverCommands.StateType != StateCommandType.Cancel)
        {
            _lastSayingGCDAction = actionID;
            lastTime = DateTime.Now;
            return true;
        }
        else return false;
    }
}
