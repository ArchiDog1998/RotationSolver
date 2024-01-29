using Clipper2Lib;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    static RotationConfigWindow? _rotationConfigWindow;
    static ControlWindow? _controlWindow;
    static NextActionWindow? _nextActionWindow;
    static CooldownWindow? _cooldownWindow;

    static readonly List<IDisposable> _dis = new();
    public static string Name => "Rotation Solver";

    public static DalamudLinkPayload OpenLinkPayload { get; private set; } = null!;
    public static DalamudLinkPayload? HideWarningLinkPayload { get; private set; }
    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        ThreadLoadImageHandler.TryGetIconTextureWrap(0, false, out _);
        IconSet.InIt();

        //Init!
        Clipper.InflatePaths(new PathsD(new PathD[] { Clipper.MakePath(new double[] { 0, 0, 1, 1 }) }), 0, JoinType.Round, EndType.Joined);

        _dis.Add(new Service());
        try
        {
            Service.Config = JsonConvert.DeserializeObject<ConfigsNew>(
                File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName), new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                })
                ?? new ConfigsNew();
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to load config");
            Service.Config = new ConfigsNew();
        }

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
        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;

        PainterManager.Init();
        MajorUpdater.Enable();
        Watcher.Enable();
        OtherConfiguration.Init();
        LocalizationManager.InIt();
        ChangeUITranslation();

        OpenLinkPayload = pluginInterface.AddChatLinkHandler(0, (id, str) =>
        {
            if (id == 0) OpenConfigWindow();
        });
        HideWarningLinkPayload = pluginInterface.AddChatLinkHandler(1, (id, str) =>
        {
            if (id == 1)
            {
                Service.Config.SetBoolRaw(PluginConfigBool.HideWarning, true);
                Svc.Chat.Print("Warning has been hidden.");
            }
        });
        Task.Run(async () =>
        {
            await DownloadHelper.DownloadAsync();
            await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.Download);
        });
    }

    internal static void ChangeUITranslation()
    {
        _rotationConfigWindow.WindowName = "ConfigWindowHeader".Local("Rotation Solver Settings v")
            + typeof(RotationConfigWindow).Assembly.GetName().Version?.ToString() ?? "?.?.?";

        RSCommands.Disable();
        RSCommands.Enable();
    }

    public async void Dispose()
    {
        RSCommands.Disable();
        Watcher.Disable();

        Svc.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw -= windowSystem.Draw;

        foreach (var item in _dis)
        {
            item.Dispose();
        }
        _dis?.Clear();

        LocalizationManager.Dispose();
        MajorUpdater.Dispose();
        PainterManager.Dispose();
        await OtherConfiguration.Save();

        ECommonsMain.Dispose();

        Service.Config.Save();
    }

    private void OnOpenConfigUi()
    {
        _rotationConfigWindow.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _rotationConfigWindow.Toggle();
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

        _nextActionWindow.IsOpen = isValid && Service.Config.GetValue(PluginConfigBool.ShowNextActionWindow);

        isValid &= !Service.Config.GetValue(PluginConfigBool.OnlyShowWithHostileOrInDuty)
                || Svc.Condition[ConditionFlag.BoundByDuty]
                || DataCenter.AllHostileTargets.Any(o => o.DistanceToPlayer() <= 25);

        _controlWindow.IsOpen = isValid && Service.Config.GetValue(PluginConfigBool.ShowControlWindow);
        _cooldownWindow.IsOpen = isValid && Service.Config.GetValue(PluginConfigBool.ShowCooldownWindow);
    }
}
