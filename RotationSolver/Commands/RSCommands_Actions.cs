using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Runtime.InteropServices;

namespace RotationSolver.Commands
{
    internal static partial class RSCommands
    {
        private static DateTime _fastClickStopwatch = DateTime.Now;

        internal static unsafe void DoAnAction(bool isGCD)
        {
            if (StateType == StateCommandType.Cancel) return;

            var localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer == null) return;

            //0.2s内，不能重复按按钮。
            if (DateTime.Now - _fastClickStopwatch < new TimeSpan(0, 0, 0, 0, 200)) return;
            _fastClickStopwatch = DateTime.Now;

            //Do Action
            var nextAction = ActionUpdater.NextAction;
#if DEBUG
            //if(nextAction is BaseAction acti)
            //Service.ChatGui.Print($"Will Do {acti} {ActionUpdater.WeaponElapsed}");
#endif
            if (nextAction == null) return;
            if (!isGCD && nextAction is BaseAction act1 && act1.IsRealGCD) return;

            if (nextAction.Use())
            {
                if (Service.Configuration.KeyBoardNoise) PreviewUpdater.PulseAtionBar(nextAction.AdjustedID);
                if (nextAction is BaseAction act)
                {
#if DEBUG
                    //Service.ChatGui.Print($"{act}, {act.Target.Name}, {ActionUpdater.AbilityRemainCount}, {ActionUpdater.WeaponElapsed}");
#endif
                    //Change Target
                    if (Service.TargetManager.Target is not PlayerCharacter && (act.Target?.CanAttack() ?? false))
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
            if (Service.ClientState.LocalPlayer.CurrentHp == 0
                || Service.Conditions[ConditionFlag.LoggingOut]
                || Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent])
            {
                CancelState();
            }
            else if (Service.Configuration.AutoOffBetweenArea && (
                Service.Conditions[ConditionFlag.BetweenAreas]
                || Service.Conditions[ConditionFlag.BetweenAreas51]))
            {
                CancelState();
            }
            //Auto start at count Down.
            else if (Service.Configuration.StartOnCountdown && CountDown.CountDownTime > 0)
            {
                if (StateType == StateCommandType.Cancel)
                {
                    DoStateCommandType(StateCommandType.Smart);
                }
            }
        }

        /// <summary>
        /// Submit text/command to outgoing chat.
        /// Can be used to enter chat commands.
        /// </summary>
        /// <param name="text">Text to submit.</param>
        public unsafe static void SubmitToChat(string text)
        {
            IntPtr uiModule = Service.GameGui.GetUIModule();

            using (ChatPayload payload = new ChatPayload(text))
            {
                IntPtr mem1 = Marshal.AllocHGlobal(400);
                Marshal.StructureToPtr(payload, mem1, false);

                Service.Address.GetChatBox(uiModule, mem1, IntPtr.Zero, 0);

                Marshal.FreeHGlobal(mem1);
            }
        }
    }
}
