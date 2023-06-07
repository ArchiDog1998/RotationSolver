using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;
using Module = ECommons.Module;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    static RotationConfigWindow _comboConfigWindow;
    static ControlWindow _controlWindow;
    static NextActionWindow _nextActionWindow;
    static CooldownWindow _cooldownWindow;

    static readonly List<IDisposable> _dis = new();
    public string Name => "Rotation Solver";

    public static DalamudLinkPayload LinkPayload { get; private set; }
    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, Module.DalamudReflector);
        ThreadLoadImageHandler.TryGetIconTextureWrap(0, false, out _);

        try
        {
            Service.Config = JsonConvert.DeserializeObject<PluginConfiguration>(
                File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName)) 
                ?? new PluginConfiguration();
        }
        catch
        {
            Service.Config = new PluginConfiguration();
        }

        _comboConfigWindow = new();
        _controlWindow = new();
        _nextActionWindow = new();
        _cooldownWindow = new();

        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_comboConfigWindow);
        windowSystem.AddWindow(_controlWindow);
        windowSystem.AddWindow(_nextActionWindow);
        windowSystem.AddWindow(_cooldownWindow);

        Svc.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;

        MajorUpdater.Enable();
        PainterManager.Init();
        Watcher.Enable();
        OtherConfiguration.Init();
        _dis.Add(new MovingController());
        _dis.Add(new LocalizationManager());
#if DEBUG
        LocalizationManager.ExportLocalization();
#endif
        ChangeUITranslation();

        Task.Run(async () =>
        {
            await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.Download);
            await RotationHelper.LoadListAsync();
        });

        LinkPayload = pluginInterface.AddChatLinkHandler(0, (id, str) =>
        {
            if(id == 0) OpenConfigWindow();
        });
    }

    internal static void ChangeUITranslation()
    {
        _comboConfigWindow.WindowName = LocalizationManager.RightLang.ConfigWindow_Header
            + typeof(RotationConfigWindow).Assembly.GetName().Version.ToString();

        RSCommands.Disable();
        RSCommands.Enable();
    }

    public void Dispose()
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

        MajorUpdater.Dispose();
        PainterManager.Dispose();
        OtherConfiguration.Save();

        ECommonsMain.Dispose();
    }

    private void OnOpenConfigUi()
    {
        _comboConfigWindow.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _comboConfigWindow.Toggle();
    }

    static RandomDelay validDelay = new(() => (0.2f, 0.2f));

    internal static void UpdateDisplayWindow()
    {
        var isValid = validDelay.Delay(MajorUpdater.IsValid
        && (!Service.Config.OnlyShowWithHostileOrInDuty
                || Svc.Condition[ConditionFlag.BoundByDuty]
                || DataCenter.AllHostileTargets.Any(o => o.DistanceToPlayer() <= 25))
            && RotationUpdater.RightNowRotation != null
            && !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
            && !Svc.Condition[ConditionFlag.Occupied38] //Treasure hunt.
            && !Svc.Condition[ConditionFlag.BetweenAreas]
            && !Svc.Condition[ConditionFlag.BetweenAreas51]
            && !Svc.Condition[ConditionFlag.WaitingForDuty]
            && (!Svc.Condition[ConditionFlag.UsingParasol] || Player.Object.StatusFlags.HasFlag(Dalamud.Game.ClientState.Objects.Enums.StatusFlags.WeaponOut))
            && !Svc.Condition[ConditionFlag.OccupiedInQuestEvent]);

        _controlWindow.IsOpen = isValid && Service.Config.ShowControlWindow;
        _nextActionWindow.IsOpen = isValid && Service.Config.ShowNextActionWindow;
        _cooldownWindow.IsOpen = isValid && Service.Config.ShowCooldownWindow;
    }
}
