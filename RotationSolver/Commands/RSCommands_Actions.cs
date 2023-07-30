using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        static DateTime _lastClickTime = DateTime.MinValue;
        static bool _lastState;

        internal static unsafe bool CanDoAnAction(bool isGCD)
        {
            if (!_lastState || !DataCenter.State)
            {
                _lastState = DataCenter.State;
                return false;
            }
            _lastState = DataCenter.State;

            if (!Player.Available) return false;

            //Do not click the button in random time.
            if (DateTime.Now - _lastClickTime < TimeSpan.FromMilliseconds(new Random().Next(
                (int)(Service.Config.ClickingDelayMin * 1000), (int)(Service.Config.ClickingDelayMax * 1000)))) return  false;
            _lastClickTime = DateTime.Now;

            if (!isGCD && ActionUpdater.NextAction is IBaseAction act1 && act1.IsRealGCD) return false;

            return true;
        }
        internal static DateTime _lastUsedTime = DateTime.MinValue;
        internal static uint _lastActionID;
        public static void DoAction()
        {
            var wrong = new Random().NextDouble() < Service.Config.MistakeRatio && ActionUpdater.WrongAction != null;
            var nextAction = wrong ? ActionUpdater.WrongAction : ActionUpdater.NextAction;
            if (nextAction == null) return;

            if (wrong)
            {
                Svc.Toasts.ShowError(string.Format(LocalizationManager.RightLang.ClickingMistakeMessage, nextAction));
                ControlWindow.Wrong = nextAction;
                ControlWindow.DidTime = DateTime.Now;
            }

#if DEBUG
            //if (nextAction is BaseAction acti)
            //    Svc.Chat.Print($"Will Do {acti}");
#endif

            if (Service.Config.KeyBoardNoise)
            {
                PreviewUpdater.PulseActionBar(nextAction.AdjustedID);
            }

            if (nextAction.Use())
            {
                _lastActionID = nextAction.AdjustedID;
                _lastUsedTime = DateTime.Now;

                if (nextAction is BaseAction act)
                {
                    if (Service.Config.KeyBoardNoise)
                        Task.Run(() => PulseSimulation(nextAction.AdjustedID));

                    if (act.ShouldEndSpecial) ResetSpecial();
#if DEBUG
                    //Svc.Chat.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                    //Change Target
                    if (act.Target != null && (Service.Config.TargetFriendly && !DataCenter.IsManual || ((Svc.Targets.Target?.IsNPCEnemy() ?? true)
                        || Svc.Targets.Target?.GetObjectKind() == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Treasure)
                        && act.Target.IsNPCEnemy()))
                    {
                        Svc.Targets.Target = act.Target;
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

        internal static void ResetSpecial()
        {
            DoSpecialCommandType(SpecialCommandType.EndSpecial, false);
        }
        private static void CancelState()
        {
            if (DataCenter.State) DoStateCommandType(StateCommandType.Cancel);
        }

        static float _lastCountdownTime = 0;
        internal static void UpdateRotationState()
        {
            if(ActionUpdater._cancelTime != DateTime.MinValue && 
                (!DataCenter.State || DataCenter.InCombat))
            {
                ActionUpdater._cancelTime = DateTime.MinValue;
            }

            var target = DataCenter.AllHostileTargets.FirstOrDefault(t => t.TargetObjectId == Player.Object.ObjectId);

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
            //Auto manual on being attacked by someone.
            else if (Service.Config.StartOnAttackedBySomeone && target != null
                && !target.IsDummy())
            {
                if(!DataCenter.State)
                {
                    DoStateCommandType(StateCommandType.Manual);
                }
            }
            //Auto start at count Down.
            else if (Service.Config.StartOnCountdown && Service.CountDownTime > 0)
            {
                _lastCountdownTime = Service.CountDownTime;
                if (!DataCenter.State)
                {
                    DoStateCommandType(StateCommandType.Auto);
                }
            }
            //Cancel when after combat.
            else if (ActionUpdater._cancelTime != DateTime.MinValue
                && DateTime.Now > ActionUpdater._cancelTime)
            {
                CancelState();
                ActionUpdater._cancelTime = DateTime.MinValue;
            }
        }
    }
}
