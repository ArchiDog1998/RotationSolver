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
    public static float CountDownTime { get; private set; }

    private static GetChatBoxModuleDelegate GetChatBox { get; set; }

    public Service()
    {
        //https://github.com/BardMusicPlayer/Hypnotoad-Plugin/blob/develop/HypnotoadPlugin/Offsets/Offsets.cs#L18
        GetChatBox = Marshal.GetDelegateForFunctionPointer<GetChatBoxModuleDelegate>(
            SigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9"));

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
        _countdownTimerHook?.Dispose();
        Framework.Update -= Framework_Update;
    }
    public static PluginConfiguration Config { get; set; }
    public static PluginConfiguration Default { get; } = new PluginConfiguration();

    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* RawPlayer
    => Control.Instance()->LocalPlayer;


    public static ActionID GetAdjustedActionId(ActionID id)
        => (ActionID)GetAdjustedActionId((uint)id);

    public static unsafe uint GetAdjustedActionId(uint id)
    => ActionManager.Instance()->GetAdjustedActionId(id);

    [PluginService]
    public static DalamudPluginInterface Interface { get; private set; }
    [PluginService]
    public static SigScanner SigScanner { get; private set; }

    [PluginService]
    public static ChatGui ChatGui { get; private set; }

    [PluginService]
    public static GameGui GameGui { get; set; }

    public static bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos) => GameGui.WorldToScreen(worldPos, out screenPos);

    public unsafe static IEnumerable<IntPtr> GetAddons<T>() where T : struct
    {
        if(typeof(T).GetCustomAttribute<Addon>() is not Addon on) return new IntPtr[0];

        return on.AddonIdentifiers
            .Select(str => GameGui.GetAddonByName(str, 1))
            .Where(ptr => ptr != IntPtr.Zero);
    }

    public static PlayerCharacter Player => ClientState.LocalPlayer;
    [PluginService]
    public static ClientState ClientState { get; set; }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => DataManager.GetExcelSheet<T>();

    internal static TextureWrap GetTextureIcon(uint id) => DataManager.GetImGuiTextureIcon(id);
    internal static TextureWrap GetTexture(string path) => DataManager.GetImGuiTexture(path);

    [PluginService]
    private static DataManager DataManager { get; set; }


    [PluginService]
    public static CommandManager CommandManager { get; private set; }

    [PluginService]
    public static Condition Conditions { get; private set; }

    [PluginService]
    internal static JobGauges JobGauges { get; private set; }

    [PluginService]
    public static ObjectTable ObjectTable { get; private set; }

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

    public static ClientLanguage Language => ClientState.ClientLanguage;


    private delegate void GetChatBoxModuleDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);


    /// <summary>
    /// Submit text/command to outgoing chat.
    /// Can be used to enter chat commands.
    /// </summary>
    /// <param name="text">Text to submit.</param>
    public unsafe static void SubmitToChat(string text)
    {
        IntPtr uiModule = GameGui.GetUIModule();

        using (ChatPayload payload = new ChatPayload(text))
        {
            IntPtr mem1 = Marshal.AllocHGlobal(400);
            Marshal.StructureToPtr(payload, mem1, false);

            GetChatBox(uiModule, mem1, IntPtr.Zero, 0);

            Marshal.FreeHGlobal(mem1);
        }
    }
}
