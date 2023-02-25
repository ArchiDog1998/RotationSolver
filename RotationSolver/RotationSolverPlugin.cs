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
using System.Threading.Tasks;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{

    private readonly WindowSystem windowSystem;

    private static RotationConfigWindow _comboConfigWindow;
    public string Name => "Rotation Solver";

    public unsafe RotationSolverPlugin(DalamudPluginInterface pluginInterface)
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

        MajorUpdater.Enable();
        TimeLineUpdater.Enable(pluginInterface.ConfigDirectory.FullName);
        SocialUpdater.Enable();
        Watcher.Enable();
        CountDown.Enable();

        Service.Localization = new LocalizationManager();
#if DEBUG
        Service.Localization.ExportLocalization();
#endif
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ChangeUITranslation();
    }

    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
        RSCommands.UpdateStateNamePlate();
        SocialUpdater.CanSaying = true;
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
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;

        RSCommands.Disable();
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.Draw -= OverlayWindow.Draw;

        Service.IconReplacer.Dispose();
        Service.Localization.Dispose();

        MajorUpdater.Dispose();
        TimeLineUpdater.SaveFiles();
        Watcher.Dispose();
        CountDown.Dispose();
        SocialUpdater.Disable();

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
