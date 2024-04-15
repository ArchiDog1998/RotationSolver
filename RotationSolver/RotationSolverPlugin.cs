using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.UI;
using RotationSolver.Updaters;
using XIVConfigUI;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    internal static RotationConfigWindow? _rotationConfigWindow;
    static ControlWindow? _controlWindow;
    static NextActionWindow? _nextActionWindow;
    static CooldownWindow? _cooldownWindow;

    static readonly List<IDisposable> _dis = [];
    public static string Name => "Rotation Solver";

    public static DalamudLinkPayload OpenLinkPayload { get; private set; } = null!;
    public static DalamudLinkPayload? HideWarningLinkPayload { get; private set; }
    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        XIVConfigUIMain.Init(pluginInterface, Service.COMMAND, "Open config window.", RSCommands.DoOneCommand);
        XIVConfigUIMain.ShowTooltip = () => Service.Config.ShowTooltips;

        _dis.Add(new Service());
        _rotationConfigWindow = new();
        _controlWindow = new();
        _nextActionWindow = new();
        _cooldownWindow = new();

        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_rotationConfigWindow);
        windowSystem.AddWindow(_controlWindow);
        windowSystem.AddWindow(_nextActionWindow);
        windowSystem.AddWindow(_cooldownWindow);

        Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.OpenMainUi += OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw += OnDraw;

        PainterManager.Init();

        MajorUpdater.Enable();
        Watcher.Enable();
        OtherConfiguration.Init();

        OpenLinkPayload = pluginInterface.AddChatLinkHandler(0, (id, str) =>
        {
            if (id == 0) OpenConfigWindow();
        });
        HideWarningLinkPayload = pluginInterface.AddChatLinkHandler(1, (id, str) =>
        {
            if (id == 1)
            {
                Service.Config.HideWarning.Value = true;
                Svc.Chat.Print("Warning has been hidden.");
            }
        });
        Task.Run(async () =>
        {
            await DownloadHelper.DownloadAsync();
            await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.Download);
        });

#if DEBUG
        if (Player.Available)
        {
            //_ = XIVPainterMain.ShowOff();
        }
#endif
    }

    private void OnDraw()
    {
        if (Svc.GameGui.GameUiHidden) return;
        windowSystem.Draw();
    }

    public async void Dispose()
    {
        DataCenter.RightNowDutyRotation?.Dispose();
        DataCenter.RightNowRotation?.Dispose();

        Watcher.Disable();

        Svc.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw -= OnDraw;

        foreach (var item in _dis)
        {
            item.Dispose();
        }
        _dis?.Clear();

        MajorUpdater.Dispose();
        PainterManager.Dispose();
        XIVConfigUIMain.Dispose();
        await OtherConfiguration.Save();

        ECommonsMain.Dispose();

        Service.Config.Save();
    }

    private void OnOpenConfigUi()
    {
        _rotationConfigWindow!.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _rotationConfigWindow?.Toggle();
    }

    static RandomDelay validDelay = new(() => (0.2f, 0.2f));

    internal static void UpdateDisplayWindow()
    {
        var isValid = validDelay.Delay(MajorUpdater.IsValid
            && DataCenter.RightNowRotation != null
            && !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
            && !Svc.Condition[ConditionFlag.Occupied38] //Treasure hunt.
            && !Svc.Condition[ConditionFlag.WaitingForDuty]
            && (!Svc.Condition[ConditionFlag.UsingParasol] || Player.Object.StatusFlags.HasFlag(Dalamud.Game.ClientState.Objects.Enums.StatusFlags.WeaponOut))
            && !Svc.Condition[ConditionFlag.OccupiedInQuestEvent]);

        _nextActionWindow!.IsOpen = isValid && Service.Config.ShowNextActionWindow;

        isValid &= !Service.Config.OnlyShowWithHostileOrInDuty
                || Svc.Condition[ConditionFlag.BoundByDuty]
                || DataCenter.AllHostileTargets.Any(o => o.DistanceToPlayer() <= 25);

        _controlWindow!.IsOpen = isValid && Service.Config.ShowControlWindow;
        _cooldownWindow!.IsOpen = isValid && Service.Config.ShowCooldownWindow;
    }
}
