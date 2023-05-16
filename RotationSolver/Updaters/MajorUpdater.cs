using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    public static bool IsValid => Service.Conditions.Any() && Service.Player != null && !SocialUpdater.InPvp;
    public static bool ShouldPreventActions => Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.PreventActions)
            && Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.PreventActionsDuty)
            && Service.Conditions[ConditionFlag.BoundByDuty]
            && !Service.DutyState.IsDutyStarted
        || Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.PreventActions)
            && !DataCenter.HasHostilesInMaxRange;

#if DEBUG
    private static readonly Dictionary<int, bool> _values = new();
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
                if (_values.TryGetValue(i, out bool value) && value != newValue && indexs[i] != 48 && indexs[i] != 27)
                {
                    //Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
                }
                _values[i] = newValue;
            }
        }
#endif

        try
        {
            SocialUpdater.UpdateSocial();
            PreviewUpdater.UpdatePreview();
            ActionUpdater.UpdateActionInfo();

            if (!ShouldPreventActions)
            {
                ActionUpdater.DoAction();
            }

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
        ActionSequencerUpdater.Enable(Service.Interface.ConfigDirectory.FullName + "\\Conditions");

        SocialUpdater.Enable();
    }

    static bool _work;
    private static async void UpdateWork()
    {
        if (!IsValid) return;
        if (_work) return;
        _work = true;

        try
        {
            PreviewUpdater.UpdateCastBarState();
            TargetUpdater.UpdateTarget();
            
            if (Service.Config.AutoLoadCustomRotations)
            {
                await RotationUpdater.LocalRotationWatcher();
            }

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
        ActionSequencerUpdater.SaveFiles();
        SocialUpdater.Disable();
    }
}
