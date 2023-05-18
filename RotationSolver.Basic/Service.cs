using Dalamud;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Gui.Toast;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiScene;
using Lumina.Excel;
using RotationSolver.Basic.Configuration;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic;

public class Service : IDisposable
{
    public const string Command = "/rotation";

    private delegate IntPtr CountdownTimerDelegate(IntPtr p1);

    /// <summary>
    ///https://github.com/xorus/EngageTimer/blob/main/Game/CountdownHook.cs
    /// </summary>
    [Signature("48 89 5C 24 ?? 57 48 83 EC 40 8B 41", DetourName = nameof(CountdownTimerFunc))]
    private readonly Hook<CountdownTimerDelegate> _countdownTimerHook = null;

    private static IntPtr _countDown = IntPtr.Zero;

    static float _lastTime = 0;
    private bool _disposed;

    public static float CountDownTime { get; private set; }


    public Service()
    {
        SignatureHelper.Initialise(this);
        Framework.Update += Framework_Update;
        _countdownTimerHook?.Enable();
    }

    private void Framework_Update(Framework framework)
    {
        var value = _countDown == IntPtr.Zero ? 0 : Math.Max(0, Marshal.PtrToStructure<float>(_countDown + 0x2c));
        if (_lastTime == value) CountDownTime = 0;
        else CountDownTime = _lastTime = value;
    }

    private IntPtr CountdownTimerFunc(IntPtr value)
    {
        _countDown = value;
        return _countdownTimerHook!.Original(value);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _countdownTimerHook?.Dispose();
            Framework.Update -= Framework_Update;
        }
        _disposed = true;
    }
    public static PluginConfiguration Config { get; set; }
    public static PluginConfiguration Default { get; } = new PluginConfiguration();

    public static ActionID GetAdjustedActionId(ActionID id)
        => (ActionID)GetAdjustedActionId((uint)id);

    public static unsafe uint GetAdjustedActionId(uint id)
    => ActionManager.Instance()->GetAdjustedActionId(id);


    public unsafe static IEnumerable<IntPtr> GetAddons<T>() where T : struct
    {
        if(typeof(T).GetCustomAttribute<Addon>() is not Addon on) return Array.Empty<nint>();

        return on.AddonIdentifiers
            .Select(str => Svc.GameGui.GetAddonByName(str, 1))
            .Where(ptr => ptr != IntPtr.Zero);
    }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => DataManager.GetExcelSheet<T>();

    internal static TextureWrap GetTextureIcon(uint id) => DataManager.GetImGuiTextureIcon(id);
    internal static TextureWrap GetTexture(string path) => DataManager.GetImGuiTexture(path);

    [PluginService]
    private static DataManager DataManager { get; set; }

    [PluginService]
    public static TargetManager TargetManager { get; private set; }

    [PluginService]
    public static PartyList PartyList { get; private set; }

    [PluginService]
    public static DtrBar DtrBar { get; private set; }

    [PluginService]
    public static ToastGui ToastGui { get; private set; }
    [PluginService]
    public static FlyTextGui FlyTextGui { get; private set; }
    [PluginService]
    public static KeyState KeyState { get; private set; }
    [PluginService]
    public static GamepadState GamepadState { get; private set; }
    [PluginService]
    public static Framework Framework { get; private set; }

    [PluginService]
    public static DutyState DutyState { get; private set; }

    public static ClientLanguage Language => Svc.ClientState.ClientLanguage;

}
