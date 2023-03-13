using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions;
using RotationSolver.Basic;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.UI;
using RotationSolver.Updaters;
using RotationSolver.Windows.RotationConfigWindow;
using System.Reflection;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    private static RotationConfigWindow _comboConfigWindow;

    readonly List<IDisposable> _dis = new List<IDisposable>();
    public string Name => "Rotation Solver";

    public unsafe RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        try
        {
            Service.Config = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        }
        catch
        {
            Service.Config = new PluginConfiguration();
        }
        //Service.Address = new PluginAddressResolver();
        //Service.Address.Setup();

        _comboConfigWindow = new();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(_comboConfigWindow);

        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += OverlayWindow.Draw;

        MajorUpdater.Enable();
        TimeLineUpdater.Enable(pluginInterface.ConfigDirectory.FullName);
        SocialUpdater.Enable();
        _dis.Add(new Watcher());
        _dis.Add(new IconReplacer());
        _dis.Add(new MovingController());

        var manager = new LocalizationManager();
        _dis.Add(manager);
#if DEBUG
        manager.ExportLocalization();
#endif
        ChangeUITranslation();

        var locs = new string[] { "RotationSolver.dll", "RotationSolver.Basic.dll" };
        var assemblies = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ICustomRotation)).Location), "*.dll")
            .Where(l => !locs.Any(t => l.Contains(t))).Select(Assembly.LoadFrom);

        RotationUpdater.Assemblies = assemblies.ToArray();

        foreach (var a in assemblies)
        {
            Service.ChatGui.Print(a.FullName);

            foreach (var t in a.GetTypes())
            {
                Service.ChatGui.Print(t.FullName);
            }
        }

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

        foreach (var item in _dis)
        {
            item.Dispose();
        }

        MajorUpdater.Dispose();
        TimeLineUpdater.SaveFiles();
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
