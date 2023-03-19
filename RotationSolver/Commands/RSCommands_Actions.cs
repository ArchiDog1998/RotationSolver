using Dalamud.Game.ClientState.Conditions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        static DateTime _fastClickStopwatch = DateTime.Now;
        static readonly TimeSpan _fastSpan = new TimeSpan(0, 0, 0, 0, 200);
        static byte _loop = 0;
        internal static unsafe void DoAnAction(bool isGCD)
        {
            if (DataCenter.StateType == StateCommandType.Cancel) return;

            var localPlayer = Service.Player;
            if (localPlayer == null) return;

            //0.2s内，不能重复按按钮。
            if (DateTime.Now - _fastClickStopwatch < _fastSpan) return;
            _fastClickStopwatch = DateTime.Now;

            //Do Action
            var nextAction = ActionUpdater.NextAction;
            if (nextAction == null) return;
#if DEBUG
            //if (nextAction is BaseAction acti)
            //    Service.ChatGui.Print($"Will Do {acti} {ActionUpdater.WeaponElapsed}");
#endif
            if (SocialUpdater.InHighEndDuty && !RotationUpdater.RightNowRotation.IsAllowed(out var str))
            {
                if (_loop % 5 == 0)
                {
                    Service.ToastGui.ShowError(string.Format(LocalizationManager.RightLang.HighEndBan, str));
                }
                _loop++;
                _loop %= 5;
                return;
            }

            if (!isGCD && nextAction is BaseAction act1 && act1.IsRealGCD) return;

            if (nextAction.Use())
            {
                if (nextAction is BaseAction a && a.ShouldEndSpecial) ResetSpecial();
                if (Service.Config.KeyBoardNoise) PreviewUpdater.PulseActionBar(nextAction.AdjustedID);
                if (nextAction is BaseAction act)
                {
#if DEBUG
                    //Service.ChatGui.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                    //Change Target
                    if ((Service.TargetManager.Target?.IsNPCEnemy() ?? true) 
                        && (act.Target?.IsNPCEnemy() ?? false))
                    {
                        Service.TargetManager.SetTarget(act.Target);
                    }
                }
            }
            return;
        }

        internal static void ResetSpecial() => DoSpecialCommandType(SpecialCommandType.EndSpecial, false);
        private static void CancelState()
        {
            if (DataCenter.StateType != StateCommandType.Cancel) DoStateCommandType(StateCommandType.Cancel);
        }

        internal static void UpdateRotationState()
        {
            if (Service.Conditions[ConditionFlag.LoggingOut])
            {
                CancelState();
            }
            else if (Service.Config.AutoOffWhenDead 
                && Service.Player.CurrentHp == 0)
            {
                CancelState();
            }
            else if (Service.Config.AutoOffCutScene
                && Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent])
            {
                CancelState();
            }
            else if (Service.Config.AutoOffBetweenArea && (
                Service.Conditions[ConditionFlag.BetweenAreas]
                || Service.Conditions[ConditionFlag.BetweenAreas51]))
            {
                CancelState();
            }
            //Auto start at count Down.
            else if (Service.Config.StartOnCountdown && Service.CountDownTime > 0)
            {
                if (DataCenter.StateType == StateCommandType.Cancel)
                {
                    DoStateCommandType(StateCommandType.Smart);
                }
            }
        }
    }
}
