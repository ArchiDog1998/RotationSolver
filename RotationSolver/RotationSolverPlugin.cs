using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using NRender;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.UI;
using RotationSolver.UI.ConfigWindows;
using RotationSolver.Updaters;
using RotationSolver.Vfx;
using System.IO;
using XIVConfigUI;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.GroupPoseModule;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    internal static RotationConfigWindow? _rotationConfigWindow;
    static ControlWindow? _controlWindow;
    static ActionGroupWindow? _actionGrpWindow;
    static NextActionWindow? _nextActionWindow;
    static CooldownWindow? _cooldownWindow;

    static readonly List<IDisposable> _dis = [];
    public static string Name => "Rotation Solver";

    public RotationSolverPlugin(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        XIVConfigUIMain.Init(pluginInterface, Service.COMMAND, "Open config window.", RSCommands.DoOneCommand,
            typeof(Configs), typeof(UiString), typeof(TargetingType), typeof(WhyActionCantUse), typeof(AutoStatus));
        XIVConfigUIMain.ShowTooltip = () => Service.Config.ShowTooltips;

        _dis.Add(new Service(GetDrawing));
        _rotationConfigWindow = new();
        _controlWindow = new();
        _actionGrpWindow = new();
        _nextActionWindow = new();
        _cooldownWindow = new();

        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_rotationConfigWindow);
        windowSystem.AddWindow(_controlWindow);
        windowSystem.AddWindow(_actionGrpWindow);
        windowSystem.AddWindow(_nextActionWindow);
        windowSystem.AddWindow(_cooldownWindow);

        Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.OpenMainUi += OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw += OnDraw;

        PainterManager.Init();
        _ = new Ipc();

        MajorUpdater.Enable();
        Watcher.Enable();
        OtherConfiguration.Init();
        ImGuiHelperRS.Init();

        WarningHelper.OpenLinkPayload = pluginInterface.AddChatLinkHandler(0, (id, str) =>
        {
            if (id == 0) OpenConfigWindow();
        });
        WarningHelper.HideWarningLinkPayload = pluginInterface.AddChatLinkHandler(1, (id, str) =>
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
        }
#endif
    }

    private static IDisposable? GetDrawing(OmenData data)
    {
        if (data.Item != null) return data.Item;
        if (string.IsNullOrEmpty(data.Path)) return null;

        switch (data.Type)
        {
            case OmenDataType.Static:
                switch (data.Path)
                {
                    case StaticOmen.Donut:
                        var outerRadius = MathF.Max(data.Scale.X, data.Scale.Y);
                        var innerRadius = MathF.Min(data.Scale.X, data.Scale.Y);

                        var result = CreateStaticVfx(data.Location, data.Path, outerRadius, outerRadius, data.Color);
                        result.Radian = innerRadius / outerRadius;
                        return result;


                    case StaticOmen.Fan:
                        var angle = data.Scale.X / 180 * MathF.PI;
                        var radius = data.Scale.Y;

                        result = CreateStaticVfx(data.Location, data.Path, radius, radius, data.Color);
                        result.Radian = angle;
                        return result;

                    default:
                        result = CreateStaticVfx(data.Location, data.Path, data.Scale.X, data.Scale.Y, data.Color);
                        return result;
                }

            case OmenDataType.LockOn:
                var obj = data.Location.Object;
                if (obj == null) return null;
                return new ActorVfx(data.Path.LockOn(), obj, obj);

            case OmenDataType.Channeling:
                obj = data.Location.Object;
                if (obj == null) return null;
                return new ActorVfx(data.Path.Channeling(), obj, data.Location.Target ?? obj);
        }

        return null;
    }

    private static StaticVfx CreateStaticVfx(LocationDescription loc, string path, float size1, float size2, Vector4 color)
    {
        const float Height = 1f;

        if (loc.Object != null)
        {
            var result = new StaticVfx(path, new Vector3(size1, Height, size2), loc.Object, color)
            {
                Offset = loc.Position,
                Rotation = loc.Rotation,
            };

            if (loc.Target != null)
            {
                result.UpdateEveryFrame = () =>
                {
                    var dir = loc.Target.Position - loc.Object.Position;
                    result.Rotation = MathF.Atan2(dir.X, dir.Z) + loc.Rotation;
                };
            }

            return result;
        }
        else
        {
            return new StaticVfx(path, new Vector3(size1, Height, size2), loc.Position, color, loc.Rotation);
        }
    }

    private void OnDraw()
    {
        if (Svc.GameGui.GameUiHidden) return;
        windowSystem.Draw();
    }

    public void Dispose()
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
        OtherConfiguration.Save().Wait();
        Service.Config.Save();

        ECommonsMain.Dispose();
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
        _actionGrpWindow!.IsOpen = isValid && Service.Config.ShowActionGroupWindow 
            && Service.Config.ActionGroups.Any(ItemConfig => ItemConfig.IsValid);
    }
}
