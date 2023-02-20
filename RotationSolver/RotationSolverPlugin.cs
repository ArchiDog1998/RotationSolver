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

        if ((int)Service.ClientState.ClientLanguage is < 0 or > 3) return;

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
        Watcher.Enable();
        CountDown.Enable();

        Service.Localization = new LocalizationManager();
#if DEBUG
        Service.Localization.ExportLocalization();
#endif
        Service.DutyState.DutyStarted += DutyState_DutyStarted;
        Service.DutyState.DutyCompleted += DutyState_DutyCompleted;
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ChangeUITranslation();
    }

    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
        RSCommands.UpdateStateNamePlate();
    }

    private async void DutyState_DutyCompleted(object sender, ushort e)
    {
        await Task.Delay(new Random().Next(4000, 6000));

        Service.Configuration.DutyEnd.AddMacro();
    }

    private void DutyState_DutyStarted(object sender, ushort e)
    {
        var territory = Service.DataManager.GetExcelSheet<TerritoryType>().GetRow(e);
        if (territory?.ContentFinderCondition?.Value?.HighEndDuty ?? false)
        {
            var str = territory.PlaceName?.Value?.Name.ToString() ?? "High-end Duty";
            Service.ToastGui.ShowError(string.Format(LocalizationManager.RightLang.HighEndWarning, str));
        }

        Service.Configuration.DutyStart.AddMacro();
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
        Service.DutyState.DutyStarted -= DutyState_DutyStarted;
        Service.DutyState.DutyCompleted -= DutyState_DutyCompleted;
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
