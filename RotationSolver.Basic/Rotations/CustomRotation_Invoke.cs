using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;

namespace RotationSolver.Rotations.CustomRotation;

public abstract partial class CustomRotation
{
    public bool TryInvoke(out IAction newAction, out IAction gcdAction)
    {
        newAction = gcdAction = null;
        if (!IsEnabled)
        {
            return false;
        }

        MoveTarget = (MoveForwardAbility(1, out var act, recordTarget: false) && act is BaseAction a) ? a.Target : null;
        UpdateInfo();

        newAction = Invoke(out gcdAction);

        return newAction != null;
    }

    private IAction Invoke(out IAction gcdAction)
    {
        var countDown = CountDown.CountDownTime;
        if (countDown > 0)
        {
            gcdAction = null;
            return CountDownAction(countDown);
        }

        byte abilityRemain = DataCenter.AbilityRemainCount;
        var helpDefenseAOE = Service.Config.UseDefenseAbility && DataCenter.IsHostileCastingAOE;

        bool helpDefenseSingle = false;
        if (Job.GetJobRole() == JobRole.Healer || Job.RowId == (uint)ClassJobID.Paladin)
        {
            if (DataCenter.PartyTanks.Any((tank) =>
            {
                var attackingTankObj = DataCenter.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                if (attackingTankObj.Count() != 1) return false;

                return DataCenter.IsHostileCastingToTank;
            })) helpDefenseSingle = true;
        }

        gcdAction = GCD(abilityRemain, helpDefenseAOE, helpDefenseSingle);

        if (gcdAction != null && gcdAction is IBaseAction GcdAction)
        {
            //if (GcdAction.IsMeleeAction())
            //{
            //    OverlayWindow.MeleeAction = GcdAction;
            //    //Sayout!
            //    if (GcdAction.EnemyPositional != EnemyPositional.None && GcdAction.Target.HasPositional()
            //         && !Player.HasStatus(true, StatusID.TrueNorth))
            //    {
            //        if (CheckAction(GcdAction.ID))
            //        {
            //            string positional = GcdAction.EnemyPositional.ToName();
            //            if (Service.Config.SayPositional) Watcher.Speak(positional);
            //            if (Service.Config.FlytextPositional) Service.ToastGui.ShowQuest(" " + positional, new Dalamud.Game.Gui.Toast.QuestToastOptions()
            //            {
            //                IconId = GcdAction.IconID,
            //            });
            //        }
            //    }
            //}

            if (abilityRemain == 0 || DataCenter.WeaponTotal < DataCenter.CastingTotal) return GcdAction;

            if (Ability(abilityRemain, GcdAction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

            return GcdAction;
        }
        else if (gcdAction == null)
        {
            if (Ability(abilityRemain, Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
            return null;
        }
        return gcdAction;
    }

    protected virtual IAction CountDownAction(float remainTime) => null;


    uint _lastSayingGCDAction;
    DateTime lastTime;
    bool CheckAction(uint actionID)
    {
        if ((_lastSayingGCDAction != actionID || DateTime.Now - lastTime > new TimeSpan(0, 0, 3)) && DataCenter.StateType != StateCommandType.Cancel)
        {
            _lastSayingGCDAction = actionID;
            lastTime = DateTime.Now;
            return true;
        }
        else return false;
    }
}
