using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        static DateTime _fastClickStopwatch = DateTime.MinValue;
        static byte _loop = 0;
        static StateCommandType _lastState;

        internal static unsafe void DoAnAction(bool isGCD)
        {
            if (_lastState == StateCommandType.Cancel 
                || DataCenter.StateType == StateCommandType.Cancel)
            {
                _lastState = DataCenter.StateType;
                return;
            }
            _lastState = DataCenter.StateType;

            if (!Player.Available) return;

            //Do not click the button in random time.
            if (DateTime.Now - _fastClickStopwatch < TimeSpan.FromMilliseconds(new Random().Next(
                (int)(Service.Config.ClickingDelayMin * 1000), (int)(Service.Config.ClickingDelayMax * 1000)))) return;
            _fastClickStopwatch = DateTime.Now;

            if (!isGCD && ActionUpdater.NextAction is IBaseAction act1 && act1.IsRealGCD) return;

            DoAction();
        }

        public static void DoAction()
        {
            //Do Action
            var nextAction = ActionUpdater.NextAction;
            if (nextAction == null) return;

#if DEBUG
            //if (nextAction is BaseAction acti)
            //    Svc.Chat.Print($"Will Do {acti}");
#endif
            if (DataCenter.InHighEndDuty && !RotationUpdater.RightNowRotation.IsAllowed(out var str))
            {
                if ((_loop %= 5) == 0)
                {
                    Svc.Toasts.ShowError(string.Format(LocalizationManager.RightLang.HighEndBan, str));
                }
                _loop++;
                return;
            }

            if (Service.Config.KeyBoardNoise)
            {
                PreviewUpdater.PulseActionBar(nextAction.AdjustedID);
            }

            if (nextAction.Use())
            {
                if (nextAction is BaseAction act)
                {
                    if (Service.Config.KeyBoardNoise)
                        Task.Run(() => PulseSimulation(nextAction.AdjustedID));

                    if (act.ShouldEndSpecial) ResetSpecial();
#if DEBUG
                    //Svc.Chat.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                    //Change Target
                    if (((Svc.Targets.Target?.IsNPCEnemy() ?? true)
                        || Svc.Targets.Target?.GetObjectKind() == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Treasure)
                        && (act.Target?.IsNPCEnemy() ?? false))
                    {
                        Svc.Targets.SetTarget(act.Target);
                    }
                }

            }
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
                    var time = Service.Config.ClickingDelayMin + 
                        new Random().NextDouble() * (Service.Config.ClickingDelayMax - Service.Config.ClickingDelayMin);
                    await Task.Delay((int)(time * 1000));
                }
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Pulse Failed!");
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
            if (Svc.Condition[ConditionFlag.LoggingOut])
            {
                CancelState();
            }
            else if (Service.Config.AutoOffWhenDead 
                && Player.Available
                && Player.Object.CurrentHp == 0)
            {
                CancelState();
            }
            else if (Service.Config.AutoOffCutScene
                && Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent])
            {
                CancelState();
            }
            else if (Service.Config.AutoOffBetweenArea && (
                Svc.Condition[ConditionFlag.BetweenAreas]
                || Svc.Condition[ConditionFlag.BetweenAreas51]))
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
                    DoStateCommandType(StateCommandType.Auto);
                }
            }
        }
    }
}
