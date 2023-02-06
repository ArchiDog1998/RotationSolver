//本插件不得以任何形式在国服中使用。

using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Commands;
using RotationSolver.Configuration;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using RotationSolver.Windows;
using RotationSolver.Windows.RotationConfigWindow;
using System;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{

    private readonly WindowSystem windowSystem;

    private static RotationConfigWindow _comboConfigWindow;
    public string Name => "Rotation Solver";

    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        try
        {
            Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        }
        catch
        {
            Service.Configuration = new PluginConfiguration();
        }
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();

        Service.IconReplacer = new IconReplacer();

        _comboConfigWindow = new();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_comboConfigWindow);

        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += OverlayWindow.Draw;
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        MajorUpdater.Enable();
        TimeLineUpdater.Enable(pluginInterface.ConfigDirectory.FullName);
        Watcher.Enable();
        CountDown.Enable();

        Service.Localization = new LocalizationManager();
#if DEBUG
        Service.Localization.ExportLocalization();
#endif

        ChangeUITranslation();
    }

    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
#if DEBUG
        Service.ChatGui.Print($"Terrritory: {e}");
        var territory = Service.DataManager.GetExcelSheet<TerritoryType>().GetRow(e);
        if (territory != null)
        {
            Service.ChatGui.Print($"Terrritory Name: {territory.PlaceName}");
            Service.ChatGui.Print($"Terrritory Icon: {territory.PlaceNameIcon}");
            Service.ChatGui.Print($"Terrritory Zone Icon: {territory.PlaceNameRegionIcon}");
        }
#endif

        if (!Service.Configuration.AutoOffBetweenArea) return;
        if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty95]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundToDuty97]) return;
        RSCommands.CancelState();
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
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.Draw -= OverlayWindow.Draw;
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;

        Service.IconReplacer.Dispose();

        Service.Localization.Dispose();

        MajorUpdater.Dispose();
        TimeLineUpdater.SaveFiles();
        Watcher.Dispose();
        CountDown.Dispose();

        IconSet.Dispose();
    }

    private void OnOpenConfigUi()
    {
        _comboConfigWindow.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _comboConfigWindow.Toggle();
    }
}
