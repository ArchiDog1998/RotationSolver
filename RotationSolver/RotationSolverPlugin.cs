using Clipper2Lib;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using Lumina.Excel.GeneratedSheets2;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;
using XIVPainter.Enum;
using XIVPainter.Vfx;

namespace RotationSolver;

public sealed class RotationSolverPlugin : IDalamudPlugin, IDisposable
{
    private readonly WindowSystem windowSystem;

    static RotationConfigWindow? _rotationConfigWindow;
    static ControlWindow? _controlWindow;
    static NextActionWindow? _nextActionWindow;
    static CooldownWindow? _cooldownWindow;

    static readonly List<IDisposable> _dis = [];
    public static string Name => "Rotation Solver";

    public static DalamudLinkPayload OpenLinkPayload { get; private set; } = null!;
    public static DalamudLinkPayload? HideWarningLinkPayload { get; private set; }
    public RotationSolverPlugin(DalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        ThreadLoadImageHandler.TryGetIconTextureWrap(0, true, out _);
        IconSet.InIt();

        //Init!
        Clipper.InflatePaths(new PathsD([Clipper.MakePath(new double[] { 0, 0, 1, 1 })]), 0, JoinType.Round, EndType.Joined);

        _dis.Add(new Service());
        try
        {
            Service.Config = JsonConvert.DeserializeObject<Configs>(
                File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName), new ITimelineItemConverter())
                ?? new Configs();
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to load config");
            Service.Config = new Configs();
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
            //_ = new StaticVfx(GroundOmenFriendly.Circle1, Player.Object, new Vector3(1, 1, 3));

            _ = new ActorVfx(Player.Object, Player.Object, "vfx/lockon/eff/m0618trg_a0k1.avfx");

            //Task.Run(async () =>
            //{
            //    var allLockOn = Svc.Data.GetExcelSheet<Lockon>()!.ToArray();
            //    allLockOn = allLockOn[460..];
            //    Svc.Log.Error(DateTime.Now.AddSeconds(allLockOn.Length * 5).ToLongTimeString());

            //    foreach (var item in allLockOn)
            //    {
            //        if (item == null) continue;

            //        var name = item.Unknown0.RawString;

            //        if (string.IsNullOrEmpty(name)) continue;

            //        var path = $"vfx/lockon/eff/{name}.avfx";
            //        var vfx2 = new ActorVfx(Player.Object, Player.Object,
            //            path);

            //        Svc.Toasts.ShowError(name);

            //        await Task.Delay(4500);

            //        vfx2.Dispose();

            //        await Task.Delay(500);
            //    }

            //    Svc.Toasts.ShowQuest("That's All Lock On", new Dalamud.Game.Gui.Toast.QuestToastOptions()
            //    {
            //        DisplayCheckmark = true,
            //        PlaySound = true,
            //    });
            //});
        }
#endif
    }

    internal static void ChangeUITranslation()
    {
        _rotationConfigWindow!.WindowName = UiString.ConfigWindowHeader.Local()
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
    }
}
