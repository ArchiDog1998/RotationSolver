using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using RotationSolver.Combos.Script;
using RotationSolver.Configuration;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using RotationSolver.Windows;
using System;
using RotationSolver.Windows.ComboConfigWindow;
using RotationSolver.Commands;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{

    private readonly WindowSystem windowSystem;

    private static ComboConfigWindow _comboConfigWindow;
    internal static ScriptComboWindow _scriptComboWindow;
    public string Name => "Rotation Solver";

    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();

        Service.IconReplacer = new IconReplacer();

        _comboConfigWindow = new();
        _scriptComboWindow = new();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_comboConfigWindow);
        windowSystem.AddWindow(_scriptComboWindow);

        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += OverlayWindow.Draw;

        MajorUpdater.Enable();
        Watcher.Enable();
        CountDown.Enable();


        Service.Localization = new LocalizationManager();
#if DEBUG
        Service.Localization.ExportLocalization();
#endif

        ChangeWindowHeader();
    }

    internal static void ChangeWindowHeader()
    {
        _comboConfigWindow.WindowName = LocalizationManager.RightLang.ConfigWindow_Header
            + typeof(ComboConfigWindow).Assembly.GetName().Version.ToString();

        _scriptComboWindow.WindowName = LocalizationManager.RightLang.Scriptwindow_Header
            + typeof(ScriptComboWindow).Assembly.GetName().Version.ToString();

        RSCommands.Disable();
        RSCommands.Enable();
    }

    public void Dispose()
    {
        RSCommands.Disable();
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.Draw -= OverlayWindow.Draw;
        Service.IconReplacer.Dispose();

        Service.Localization.Dispose();

        MajorUpdater.Dispose();
        Watcher.Dispose();
        CountDown.Dispose();

        IconSet.Dispose();
    }

    private void OnOpenConfigUi()
    {
        _comboConfigWindow.IsOpen = true;
    }

    internal static void OpenScriptWindow(IScriptCombo combo)
    {
        _scriptComboWindow.TargetCombo = combo;
        _scriptComboWindow.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _comboConfigWindow.Toggle();
    }
}
