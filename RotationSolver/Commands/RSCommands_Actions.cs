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
        private static DateTime _fastClickStopwatch = DateTime.Now;

        static readonly string[] _allowedAssembly = new string[]
        {

        };

        internal static unsafe void DoAnAction(bool isGCD)
        {
            if (StateType == StateCommandType.Cancel) return;
            if(SocialUpdater.InHighEndDuty && RotationUpdater.RightNowRotation.GetType().Assembly.FullName is string str && !_allowedAssembly.Contains(str))
            {
                Service.ToastGui.ShowError(string.Format(LocalizationManager.RightLang.HighEndBan, str));
                return;
            }

            var localPlayer = Service.Player;
            if (localPlayer == null) return;

            //0.2s内，不能重复按按钮。
            if (DateTime.Now - _fastClickStopwatch < new TimeSpan(0, 0, 0, 0, 200)) return;
            _fastClickStopwatch = DateTime.Now;

            //Do Action
            var nextAction = ActionUpdater.NextAction;
#if DEBUG
            //if (nextAction is BaseAction acti)
            //    Service.ChatGui.Print($"Will Do {acti} {ActionUpdater.WeaponElapsed}");
#endif
            if (nextAction == null) return;
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
        internal static void CancelState()
        {
            if (StateType != StateCommandType.Cancel) DoStateCommandType(StateCommandType.Cancel);
        }

        internal static void UpdateRotationState()
        {
            if (Service.Player.CurrentHp == 0
                || Service.Conditions[ConditionFlag.LoggingOut])
            {
                CancelState();
            }
            else if (Service.Config.AutoOffBetweenArea && (
                Service.Conditions[ConditionFlag.BetweenAreas]
                || Service.Conditions[ConditionFlag.BetweenAreas51])
                || Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent])
            {
                CancelState();
            }
            //Auto start at count Down.
            else if (Service.Config.StartOnCountdown && Service.CountDownTime > 0)
            {
                if (StateType == StateCommandType.Cancel)
                {
                    DoStateCommandType(StateCommandType.Smart);
                }
            }
        }

    }
}
