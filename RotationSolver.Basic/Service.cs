using Dalamud;
using Dalamud.Game;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
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
        Svc.Framework.Update += Framework_Update;
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
            Svc.Framework.Update -= Framework_Update;
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

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => Svc.Data.GetExcelSheet<T>();

    internal static TextureWrap GetTextureIcon(uint id) => Svc.Data.GetImGuiTextureIcon(id);
    internal static TextureWrap GetTexture(string path) => Svc.Data.GetImGuiTexture(path);

    [PluginService]
    public static DtrBar DtrBar { get; private set; }

    [PluginService]
    public static DutyState DutyState { get; private set; }

    public static ClientLanguage Language => Svc.ClientState.ClientLanguage;

}
