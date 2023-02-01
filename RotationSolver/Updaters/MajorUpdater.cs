using Dalamud.Game;
using RotationSolver.Commands;
using System.Threading.Tasks;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    //#if DEBUG
    //    private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
    //#endif
    private static void FrameworkUpdate(Framework framework)
    {
        if (!Service.Conditions.Any() || Service.ClientState.LocalPlayer == null) return;

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

        if (Service.Configuration.UseWorkTask)
        {
            UpdateWork();
        }

        PreviewUpdater.UpdatePreview();
        ActionUpdater.DoAction();
        MacroUpdater.UpdateMacro();
    }

    static bool _quit = false;
    public static void Enable()
    {
        Service.Framework.Update += FrameworkUpdate;
        Task.Run(() =>
        {
            while (true)
            {
                if(_quit) return;
                if (!Service.Configuration.UseWorkTask || !Service.Conditions.Any() || Service.ClientState.LocalPlayer == null)
                {
                    Task.Delay(200);
                    continue;
                }

                UpdateWork();
                Task.Delay(Service.Configuration.WorkTaskDelay);
            }
        });
        MovingUpdater.Enable();
    }

    private static void UpdateWork()
    {
        PreviewUpdater.UpdateCastBarState();
        ActionUpdater.UpdateActionInfo();
        TargetUpdater.UpdateTarget();
        MovingUpdater.UpdateLocation();

        TimeLineUpdater.UpdateTimelineAction();
        ActionUpdater.UpdateNextAction();
        RSCommands.UpdateRotationState();

    }

    public static void Dispose()
    {
        _quit = true;
        Service.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
        MovingUpdater.Dispose();
    }
}
