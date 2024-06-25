using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using RotationSolver.Commands;
using RotationSolver.Helpers;
using RotationSolver.UI;
using System.Runtime.InteropServices;
using XIVConfigUI;

namespace RotationSolver.Updaters;

internal static class MajorUpdater
{
    public static bool IsValid => Svc.Condition.Any()
        && !Svc.Condition[ConditionFlag.BetweenAreas]
        && !Svc.Condition[ConditionFlag.BetweenAreas51]
        && !Svc.Condition[ConditionFlag.LoggingOut]
        && Player.Available;

    static bool _showedWarning, _work;
    static Exception? _threadException;
    static DateTime _lastUpdatedWork = DateTime.Now;

    private unsafe static void FrameworkUpdate(IFramework framework)
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

        if (!_showedWarning)
        {
            _showedWarning = true;
            ShowWarning();
        }

        try
        {
            SocialUpdater.UpdateSocial();
            PreviewUpdater.UpdatePreview();

            if (Service.Config.TeachingMode && ActionUpdater.NextAction != null)
            {
                //Sprint action id is 3 however the id in hot bar is 4.
                var id = ActionUpdater.NextAction.AdjustedID;
                PainterManager.ActionIds.Add(id == (uint)ActionID.SprintPvE ? 4 : id);
            }
            ActionUpdater.UpdateActionInfo();

            var canDoAction = ActionUpdater.CanDoAction();
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
            if (_threadException != ex)
            {
                _threadException = ex;
                Svc.Log.Error(ex, "Main Thread Exception");
            }
        }

        try
        {
            if (_work) return;
            if (DateTime.Now - _lastUpdatedWork < TimeSpan.FromSeconds(Service.Config.MinUpdatingTime))
                return;

            _work = true;
            _lastUpdatedWork = DateTime.Now;

            UpdateWork();
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Worker Exception");
        }
    }

    private static void ShowWarning()
    {
        if ((int)Svc.ClientState.ClientLanguage == 4)
        {
            var warning = "Rotation Solver 未进行国服适配并不提供相关支持! 建议使用国服的插件，如：";
            Svc.Toasts.ShowError(warning + "AE Assist 3.0！");

            var seString = new SeString(new TextPayload(warning),
                Svc.PluginInterface.AddChatLinkHandler(2, (id, str) =>
                {
                    if (id == 2)
                    {
                        Util.OpenLink("https://discord.gg/w2DPwRRZuT");
                    }
                }),
                new UIForegroundPayload(31),
                new TextPayload("AE Assist 3.0"),
                UIForegroundPayload.UIForegroundOff,
                RawPayload.LinkTerminator,
                new TextPayload("！"));

            Svc.Chat.Print(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = seString,
                Type = Dalamud.Game.Text.XivChatType.ErrorMessage,
            });
        }

        if (!Svc.PluginInterface.InstalledPlugins.Any(p => p.InternalName == "Avarice"))
        {
            UiString.AvariceWarning.Local().ShowWarning(0);
        }
    }

    public static void Enable()
    {
        ActionSequencerUpdater.Enable(Svc.PluginInterface.ConfigDirectory.FullName + "\\Conditions");
        SocialUpdater.Enable();
        RaidTimeUpdater.EnableAsync();

        Svc.Framework.Update += FrameworkUpdate;
    }

    static Exception? _innerException;
    private static void UpdateWork()
    {
        if (!IsValid)
        {
            ActionUpdater.NextAction = ActionUpdater.NextGCDAction = null;
            return;
        }

        try
        {
            TargetUpdater.UpdateTarget();
            StateUpdater.UpdateState();

            if (Service.Config.AutoLoadCustomRotations)
            {
                RotationUpdater.LocalRotationWatcher();
            }

            RotationUpdater.UpdateRotation();

            RaidTimeUpdater.UpdateTimeline();
            ActionSequencerUpdater.UpdateActionSequencerAction();
            ActionUpdater.UpdateNextAction();

            RSCommands.UpdateRotationState();
            PainterManager.UpdateSettings();
        }
        catch (Exception ex)
        {
            if (_innerException != ex)
            {
                _innerException = ex;
                Svc.Log.Error(ex, "Inner Worker Exception");
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

        Callback.Fire(notification, false, 0, 2);
    }

    static DateTime _nextOpenTime = DateTime.Now;
    static uint _lastChest = 0;
    private unsafe static void OpenChest()
    {
        if (!Service.Config.AutoOpenChest) return;
        var player = Player.Object;

        var treasure = Svc.Objects.FirstOrDefault(o =>
        {
            if (o == null) return false;
            var dis = Vector3.Distance(player.Position, o.Position) - player.HitboxRadius - o.HitboxRadius;
            if (dis > 0.5f) return false;

            var address = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)o.Address;
            if ((ObjectKind)address->ObjectKind != ObjectKind.Treasure) return false;

            //Opened!
            foreach (var item in Loot.Instance()->Items)
            {
                if (item.ChestObjectId == o.EntityId) return false;
            }

            return true;
        });

        if (treasure == null) return;
        if (DateTime.Now < _nextOpenTime) return;
        if (treasure.EntityId == _lastChest && DateTime.Now - _nextOpenTime < TimeSpan.FromSeconds(10)) return;

        _nextOpenTime = DateTime.Now.AddSeconds(new Random().NextDouble() + 0.2);
        _lastChest = treasure.EntityId;

        try
        {
            Svc.Targets.Target = treasure;

            TargetSystem.Instance()->InteractWithObject((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)treasure.Address);

            Notify.Plain($"Try to open the chest {treasure.Name}");
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to open the chest!");
        }

        if (!Service.Config.AutoCloseChestWindow) return;
        _closeWindowTime = DateTime.Now.AddSeconds(0.5);
    }

    public static void Dispose()
    {
        Svc.Framework.Update -= FrameworkUpdate;
        PreviewUpdater.Dispose();
        ActionSequencerUpdater.SaveFiles();
        SocialUpdater.Disable();
        ActionUpdater.ClearNextAction();
        RaidTimeUpdater.Disable();
    }
}
