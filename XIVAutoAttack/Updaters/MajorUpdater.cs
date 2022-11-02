using Dalamud.Game;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Updaters
{
    internal static class MajorUpdater
    {
#if DEBUG
        private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
#endif
        private unsafe static void FrameworkUpdate(Framework framework)
        {
            if (!Service.Conditions.Any() || Service.ClientState.LocalPlayer == null) return;

            PreviewUpdater.UpdateCastBar();
#if DEBUG
            //Get changed condition.
            string[] enumNames = Enum.GetNames(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            int[] indexs = (int[])Enum.GetValues(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            if (enumNames.Length == indexs.Length)
            {
                for (int i = 0; i < enumNames.Length; i++)
                {
                    string key = enumNames[i];
                    bool newValue = Service.Conditions[(Dalamud.Game.ClientState.Conditions.ConditionFlag)indexs[i]];
                    if (_valus.ContainsKey(i) && _valus[i] != newValue && indexs[i] != 48 && indexs[i] != 27)
                    {
                        Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
                    }
                    _valus[i] = newValue;
                }
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    bool newValue = Service.Conditions[i];
            //    if (_valus.ContainsKey(i) && _valus[i] != newValue)
            //    {
            //        Service.ToastGui.ShowQuest(i.ToString());
            //    }
            //    _valus[i] = newValue;
            //}
#endif
            //Update State.
            PreviewUpdater.UpdateEntry();

            ActionUpdater.UpdateActionInfo();

            TargetUpdater.UpdateHostileTargets();
            TargetUpdater.UpdateFriends();

            MovingUpdater.UpdateLocation();

            ActionUpdater.DoAction();
            MacroUpdater.UpdateMacro();
        }

        public static void Enable()
        {
            Service.Framework.Update += FrameworkUpdate;
            MovingUpdater.Enable();
        }

        public static void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;
            ActionUpdater.Dispose();
            PreviewUpdater.Dispose();
            MovingUpdater.Dispose();
        }
    }
}
