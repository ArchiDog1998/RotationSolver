using Dalamud.Game;
using Dalamud.Logging;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    public static bool IsValid => Service.Conditions.Any() && Service.Player != null && !SocialUpdater.InPvp;

#if DEBUG
    private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
#endif

    private static void FrameworkUpdate(Framework framework)
    {
        RotationSolverPlugin.UpdateDisplayWindow();
        if (!IsValid)
        {
            TargetUpdater.ClearTarget();
            return;
        }

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
                    //Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
                }
                _valus[i] = newValue;
            }
        }
#endif

        try
        {
            SocialUpdater.UpdateSocial();
            PreviewUpdater.UpdatePreview();
            ActionUpdater.UpdateActionInfo();

            ActionUpdater.DoAction();

            MacroUpdater.UpdateMacro();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Main Thread Exception");
        }

        if (Service.Config.UseWorkTask)
        {
            try
            {
                Task.Run(UpdateWork);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Worker Exception");
            }
        }
        else
        {
            UpdateWork();
        }
    }

    public static void Enable()
    {
        Service.Framework.Update += FrameworkUpdate;
    }

    static bool _work;
    private static void UpdateWork()
    {
        if (!IsValid) return;
        if (_work) return;
        _work = true;

        try
        {
            PreviewUpdater.UpdateCastBarState();
            TargetUpdater.UpdateTarget();

            RotationUpdater.UpdateRotation();

            ActionSequencerUpdater.UpdateActionSequencerAction();
            ActionUpdater.UpdateNextAction();
            RSCommands.UpdateRotationState();

            InputUpdater.UpdateCommand();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Inner Worker Exception");
        }

        _work = false;
    }

    public static void Dispose()
    {
        Service.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
    }
}
