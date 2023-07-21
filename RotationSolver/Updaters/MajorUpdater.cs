using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Commands;
using RotationSolver.UI;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    public static bool IsValid => Svc.Condition.Any() 
        && !Svc.Condition[ConditionFlag.BetweenAreas] 
        && !Svc.Condition[ConditionFlag.BetweenAreas51]
        && Player.Available && !SocialUpdater.InPvp;

    public static bool ShouldPreventActions => Service.Config.GetValue(SettingsCommand.PreventActions)
            && (Service.Config.GetValue(SettingsCommand.PreventActionsDuty)
            && Svc.Condition[ConditionFlag.BoundByDuty]
            && !Svc.DutyState.IsDutyStarted
            || !DataCenter.HasHostilesInMaxRange);

#if DEBUG
    private static readonly Dictionary<int, bool> _values = new();
#endif

    static bool _showed;
    static Exception _threadException;
    private unsafe static void FrameworkUpdate(Framework framework)
    {
        PainterManager.ActionIds.Clear();
        RotationSolverPlugin.UpdateDisplayWindow();
        if (!IsValid)
        {
            TargetUpdater.ClearTarget();
            return;
        }
        if ((int)Svc.ClientState.ClientLanguage == 4 && !_showed)
        {
            _showed = true;
            var warning = "Rotation Solver 未进行国服适配并不提供相关支持!";
            Svc.Toasts.ShowError(warning);
            Svc.Chat.PrintError(warning);
        }


#if DEBUG
        //Get changed condition.
        string[] enumNames = Enum.GetNames(typeof(ConditionFlag));
        int[] indexs = (int[])Enum.GetValues(typeof(ConditionFlag));
        if (enumNames.Length == indexs.Length)
        {
            for (int i = 0; i < enumNames.Length; i++)
            {
                string key = enumNames[i];
                bool newValue = Svc.Condition[(ConditionFlag)indexs[i]];
                if (_values.TryGetValue(i, out bool value) && value != newValue && indexs[i] != 48 && indexs[i] != 27)
                {
                    //var str = indexs[i].ToString() + " " + key + ": " + newValue.ToString();
                    //Svc.Chat.Print(str);
                    //Svc.Toasts.ShowQuest(str);
                }
                _values[i] = newValue;
            }
        }
#endif

        try
        {
            SocialUpdater.UpdateSocial();
            PreviewUpdater.UpdatePreview();
            if (Service.Config.TeachingMode && ActionUpdater.NextAction!= null)
            {
                //Sprint action id is 3 however the id in hot bar is 4.
                var id = ActionUpdater.NextAction.AdjustedID;
                PainterManager.ActionIds.Add(id == (uint)ActionID.Sprint ? 4 : id);
            }
            ActionUpdater.UpdateActionInfo();

            var canDoAction = false;
            if (!ShouldPreventActions)
            {
                canDoAction = ActionUpdater.CanDoAction();
            }

            MovingUpdater.UpdateCanMove(canDoAction);
            if (canDoAction)
            {
                RSCommands.DoAction();
            }

            MacroUpdater.UpdateMacro();
        }
        catch (Exception ex)
        {
            if(_threadException != ex)
            {
                _threadException = ex;
                PluginLog.Error(ex, "Main Thread Exception");
            }
        }

        try
        {
            if (Service.Config.UseWorkTask)
            {
                Task.Run(UpdateWork);
            }
            else
            {
                UpdateWork();
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Worker Exception");
        }
    }

    public static void Enable()
    {
        ActionSequencerUpdater.Enable(Svc.PluginInterface.ConfigDirectory.FullName + "\\Conditions");
        SocialUpdater.Enable();

        Svc.Framework.Update += FrameworkUpdate;
    }

    static bool _work;
    static Exception _innerException;
    private static void UpdateWork()
    {
        if (!IsValid)
        {
            ActionUpdater.NextAction = ActionUpdater.NextGCDAction = null;
            CustomRotation.MoveTarget = null;
            return;
        }
        if (_work) return;
        _work = true;

        try
        {
            TargetUpdater.UpdateTarget();
            
            if (Service.Config.AutoLoadCustomRotations)
            {
                RotationUpdater.LocalRotationWatcher();
            }

            RotationUpdater.UpdateRotation();
            
            ActionSequencerUpdater.UpdateActionSequencerAction();
            ActionUpdater.UpdateNextAction();

            RSCommands.UpdateRotationState();

            InputUpdater.UpdateCommand();
        }
        catch (Exception ex)
        {
            if(_innerException != ex)
            {
                _innerException = ex;
                PluginLog.Error(ex, "Inner Worker Exception");
            }
        }

        _work = false;
    }

    public static void Dispose()
    {
        Svc.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
        ActionSequencerUpdater.SaveFiles();
        SocialUpdater.Disable();
        ActionUpdater.Dispose();
    }
}
