using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.UI;
using System.Runtime.InteropServices;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    public static bool IsValid => Svc.Condition.Any() 
        && !Svc.Condition[ConditionFlag.BetweenAreas] 
        && !Svc.Condition[ConditionFlag.BetweenAreas51]
        && Player.Available && !SocialUpdater.InPvp;

    public static bool ShouldPreventActions => Service.Config.GetValue(PluginConfigBool.PreventActions)
            && (Service.Config.GetValue(PluginConfigBool.PreventActionsDuty)
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
            ActionUpdater.ClearNextAction();
            CustomRotation.MoveTarget = null;

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

            if (Service.Config.GetValue(PluginConfigBool.TeachingMode) && ActionUpdater.NextAction!= null)
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

            CloseWindow();
            OpenChest();
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
            if (Service.Config.GetValue(PluginConfigBool.UseWorkTask))
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

            if (Service.Config.GetValue(PluginConfigBool.AutoLoadCustomRotations))
            {
                RotationUpdater.LocalRotationWatcher();
            }

            RotationUpdater.UpdateRotation();
            
            ActionSequencerUpdater.UpdateActionSequencerAction();
            ActionUpdater.UpdateNextAction();

            RSCommands.UpdateRotationState();
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

    static DateTime _closeWindowTime = DateTime.Now;
    private unsafe static void CloseWindow()
    {
        if (_closeWindowTime < DateTime.Now) return;

        var needGreedWindow = Svc.GameGui.GetAddonByName("NeedGreed", 1);
        if (needGreedWindow == IntPtr.Zero) return;

        var notification = (AtkUnitBase*)Svc.GameGui.GetAddonByName("_Notification", 1);
        if (notification == null) return;

        var atkValues = (AtkValue*)Marshal.AllocHGlobal(2 * sizeof(AtkValue));
        atkValues[0].Type = atkValues[1].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int;
        atkValues[0].Int = 0;
        atkValues[1].Int = 2;
        try
        {
            notification->FireCallback(2, atkValues);
        }
        catch (Exception ex)
        {
            PluginLog.Warning(ex, "Failed to close the window!");
        }
        finally
        {
            Marshal.FreeHGlobal(new IntPtr(atkValues));
        }
    }

    static DateTime _nextOpenTime = DateTime.Now;
    static uint _lastChest = 0;
    private unsafe static void OpenChest()
    {
        if (!Service.Config.GetValue(PluginConfigBool.AutoOpenChest)) return;
        var player = Player.Object;

        var treasure = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null) return false;
            var dis = Vector3.Distance(player.Position, o.Position) - player.HitboxRadius - o.HitboxRadius;
            if (dis > 0.5f) return false;

            var address = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)o.Address;
            if ((ObjectKind)address->ObjectKind != ObjectKind.Treasure) return false;

            //Opened!
            foreach (var item in Loot.Instance()->ItemArraySpan)
            {
                if (item.ChestObjectId == o.ObjectId) return false;
            }

            return true;
        });

        if (treasure == null) return;
        if (DateTime.Now < _nextOpenTime) return;
        if (treasure.ObjectId == _lastChest && DateTime.Now - _nextOpenTime < TimeSpan.FromSeconds(10)) return;

        _nextOpenTime = DateTime.Now.AddSeconds(new Random().NextDouble() + 0.2);
        _lastChest = treasure.ObjectId;

        try
        {
            Svc.Targets.Target =treasure;

            TargetSystem.Instance()->InteractWithObject((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)treasure.Address);

            Notify.Plain($"Try to open the chest {treasure.Name}");
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to open the chest!");
        }

        if (!Service.Config.GetValue(PluginConfigBool.AutoCloseChestWindow)) return;
        _closeWindowTime = DateTime.Now.AddSeconds(0.5);
    }

    public static void Dispose()
    {
        Svc.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
        ActionSequencerUpdater.SaveFiles();
        SocialUpdater.Disable();
        ActionUpdater.ClearNextAction();
    }
}
