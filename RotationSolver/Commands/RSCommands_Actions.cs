using Dalamud.Game.ClientState.Conditions;
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
            if (DataCenter.InHighEndDuty && !RotationUpdater.RightNowRotation.IsAllowed(out var str))
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
                if (Service.Config.KeyBoardNoise) Task.Run(() => PulseSimulation(nextAction.AdjustedID));
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

        static bool started = false;
        static async void PulseSimulation(uint id)
        {
            if (started) return;
            started = true;
            try
            {
                for (int i = 0; i < new Random().Next(Service.Config.KeyBoardNoiseMin,
                    Service.Config.KeyBoardNoiseMax); i++)
                {
                    PreviewUpdater.PulseActionBar(id);
                    var time = Service.Config.KeyBoardNoiseTimeMin + 
                        new Random().NextDouble() * (Service.Config.KeyBoardNoiseTimeMax - Service.Config.KeyBoardNoiseTimeMin);
                    await Task.Delay((int)(time * 1000));
                }
            }
            finally { started = false; }
            started = false;
        }

        internal static void ResetSpecial() => DoSpecialCommandType(SpecialCommandType.EndSpecial, false);
        private static void CancelState()
        {
            if (DataCenter.StateType != StateCommandType.Cancel) DoStateCommandType(StateCommandType.Cancel);
        }

        static float _lastCountdownTime = 0;
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
            //Cancel when pull
            else if (Service.CountDownTime == 0 && _lastCountdownTime > 0.2f)
            {
                _lastCountdownTime = 0;
                CancelState();
            }
            //Auto start at count Down.
            else if (Service.Config.StartOnCountdown && Service.CountDownTime > 0)
            {
                _lastCountdownTime = Service.CountDownTime;
                if (DataCenter.StateType == StateCommandType.Cancel)
                {
                    DoStateCommandType(StateCommandType.Smart);
                }
            }
        }
    }
}
