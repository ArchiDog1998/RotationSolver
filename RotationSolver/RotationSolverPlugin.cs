using Clipper2Lib;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
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
using System.Drawing;
using System.Runtime.InteropServices;
using Module = ECommons.Module;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    static RotationConfigWindow _rotationConfigWindow;
    static ControlWindow _controlWindow;
    static NextActionWindow _nextActionWindow;
    static CooldownWindow _cooldownWindow;

    static readonly List<IDisposable> _dis = new();
    public string Name => "Rotation Solver";

    public static DalamudLinkPayload OpenLinkPayload { get; private set; }
    public static DalamudLinkPayload HideWarningLinkPayload { get; private set; }
    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, Module.DalamudReflector, Module.ObjectFunctions);
        ThreadLoadImageHandler.TryGetIconTextureWrap(0, false, out _);
        IconSet.InIt();

        //Init!
        Clipper.InflatePaths(new PathsD(new PathD[] { Clipper.MakePath(new double[] {0, 0, 1, 1 }) }), 0, JoinType.Round, EndType.Joined);

        _dis.Add(new Service());
        try
        {
            Service.Config = JsonConvert.DeserializeObject<PluginConfig>(
                File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName), new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    Error = delegate (object sender, Newtonsoft.Json.Serialization. ErrorEventArgs args)
                    {
                        args.ErrorContext.Handled = true;
                    }
                }) 
                ?? PluginConfig.Create();
        }
        catch(Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to load config");
            Service.Config = PluginConfig.Create(); ;
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
        _dis.Add(new LocalizationManager());
        ChangeUITranslation();

        OpenLinkPayload = pluginInterface.AddChatLinkHandler(0, (id, str) =>
        {
            if (id == 0) OpenConfigWindow();
        });
        HideWarningLinkPayload = pluginInterface.AddChatLinkHandler(1, (id, str) =>
        {
            if (id == 1)
            {
                Service.Config.SetValue(PluginConfigBool.HideWarning, true);
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
        _rotationConfigWindow.WindowName = LocalizationManager.RightLang.ConfigWindow_Header
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
            && RotationUpdater.RightNowRotation != null
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
