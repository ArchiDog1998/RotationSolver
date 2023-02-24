using Dalamud.Game;
using Dalamud.Logging;
using RotationSolver.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    private static bool IsValid => Service.Conditions.Any() && Service.ClientState.LocalPlayer != null;

//#if DEBUG
//    private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
//#endif
    private static void FrameworkUpdate(Framework framework)
    {
        if (!IsValid) return;

//#if DEBUG
//        //Get changed condition.
//        string[] enumNames = Enum.GetNames(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
//        int[] indexs = (int[])Enum.GetValues(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
//        if (enumNames.Length == indexs.Length)
//        {
//            for (int i = 0; i < enumNames.Length; i++)
//            {
//                string key = enumNames[i];
//                bool newValue = Service.Conditions[(Dalamud.Game.ClientState.Conditions.ConditionFlag)indexs[i]];
//                if (_valus.ContainsKey(i) && _valus[i] != newValue && indexs[i] != 48 && indexs[i] != 27)
//                {
//                    Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
//                }
//                _valus[i] = newValue;
//            }
//        }
//#endif

        if (!Service.Configuration.UseWorkTask)
        {
            UpdateWork();
        }

        PreviewUpdater.UpdatePreview();
        ActionUpdater.DoAction();
        MacroUpdater.UpdateMacro();
    }

    static bool _work = true;
    static DateTime _lastUpdate = DateTime.MinValue;
    static readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);
    static int _frameCount = 0;
    public static string FrameCount { get; private set; }
    public static void Enable()
    {
        Service.Framework.Update += FrameworkUpdate;
        MovingUpdater.Enable();

        Task.Run(async () =>
        {
            while (_work)
            {
                if (!Service.Configuration.UseWorkTask || !Service.Conditions.Any() || Service.ClientState.LocalPlayer == null)
                {
                    await Task.Delay(200);
                    continue;
                }

                try
                {
                    UpdateWork();
                }
                catch(Exception ex) 
                {
                    PluginLog.Error(ex, "TaskException");
                }

                await Task.Delay(Service.Configuration.WorkTaskDelay);

                CalculateFPS();
            }
        });
    }

    private static void CalculateFPS()
    {
        var now = DateTime.Now;
        var span = now - _lastUpdate;
        if (span > _oneSecond)
        {
            FrameCount = _frameCount.ToString();
            _lastUpdate = now;
            _frameCount = 0;
        }
        else
        {
            _frameCount++;
        }
    }

    private static void UpdateWork()
    {
        if (!IsValid) return;

        ActionUpdater.UpdateActionInfo();
        PreviewUpdater.UpdateCastBarState();
        TargetUpdater.UpdateTarget();

        RotationUpdater.UpdateRotation();

        TimeLineUpdater.UpdateTimelineAction();
        ActionUpdater.UpdateNextAction();
        RSCommands.UpdateRotationState();
    }

    public static void Dispose()
    {
        _work = false;
        Service.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
        MovingUpdater.Dispose();
    }
}
