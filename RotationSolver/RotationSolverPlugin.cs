using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.Havok;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;
using System.Text;
using System.Text.Unicode;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    static RotationConfigWindow _comboConfigWindow;
    static ControlWindow _controlWindow;
    static NextActionWindow _nextActionWindow;
    static CooldownWindow _cooldownWindow;

    static readonly List<IDisposable> _dis = new List<IDisposable>();
    public string Name => "Rotation Solver";

    public static DalamudLinkPayload LinkPayload { get; private set; }
    public unsafe RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        try
        {
            Service.Config = JsonConvert.DeserializeObject<PluginConfiguration>(
                File.ReadAllText(Service.Interface.ConfigFile.FullName)) 
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

        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += OverlayWindow.Draw;

        MajorUpdater.Enable();
        ActionSequencerUpdater.Enable(pluginInterface.ConfigDirectory.FullName + "\\Conditions");
        IActionHelper.GetAnimationLockTimeAsync(pluginInterface.ConfigDirectory.FullName);
        SocialUpdater.Enable();
        _dis.Add(new Watcher());
        _dis.Add(new MovingController());

        var manager = new LocalizationManager();
        _dis.Add(manager);
#if DEBUG
        manager.ExportLocalization();
#endif
        ChangeUITranslation();

        RotationUpdater.GetAllCustomRotations(RotationUpdater.DownloadOption.Donwload);
        RotationHelper.LoadList();

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
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.Draw -= OverlayWindow.Draw;

        foreach (var item in _dis)
        {
            item.Dispose();
        }
        _dis?.Clear();

        MajorUpdater.Dispose();
        ActionSequencerUpdater.SaveFiles();
        SocialUpdater.Disable();

        IconSet.Dispose();

#if DEBUG
        var directory = @"E:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\Resources";
#else
        var directory = Service.Interface.ConfigDirectory.FullName;
#endif
        if (!Directory.Exists(directory)) return;

        //Default values.
        var path = Path.Combine(directory, $"{nameof(IActionHelper.AnimationLockTime)}.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(IActionHelper.AnimationLockTime, Formatting.Indented));
    }

    private void OnOpenConfigUi()
    {
        _comboConfigWindow.IsOpen = true;
    }

    internal static void OpenConfigWindow()
    {
        _comboConfigWindow.Toggle();
    }

    static RandomDelay validDelay = new RandomDelay(() => (0.2f, 0.2f));

    internal static void UpdateDisplayWindow()
    {
        var isValid = validDelay.Delay(MajorUpdater.IsValid
        && (!Service.Config.OnlyShowWithHostileOrInDuty
                || Service.Conditions[ConditionFlag.BoundByDuty]
                || DataCenter.AllHostileTargets.Any(o => o.DistanceToPlayer() <= 25))
            && RotationUpdater.RightNowRotation != null
            && !Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent]
            && !Service.Conditions[ConditionFlag.Occupied38] //Treasure hunt.
            && !Service.Conditions[ConditionFlag.BetweenAreas]
            && !Service.Conditions[ConditionFlag.BetweenAreas51]
            && !Service.Conditions[ConditionFlag.WaitingForDuty]
            && !Service.Conditions[ConditionFlag.OccupiedInQuestEvent]);

        _controlWindow.IsOpen = isValid && Service.Config.ShowControlWindow;
        _nextActionWindow.IsOpen = isValid && Service.Config.ShowNextActionWindow;
        _cooldownWindow.IsOpen = isValid && Service.Config.ShowCooldownWindow;
    }
}
