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
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiScene;
using Lumina.Excel;
using RotationSolver.Configuration;
using RotationSolver.Data;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic;

public class Service
{
    public const string Command = "/rotation";
    public static GetChatBoxModuleDelegate GetChatBox { get; private set; }

    public Service(DalamudPluginInterface pluginInterface, SigScanner scanner)
    {
        pluginInterface.Create<Service>();

        //https://github.com/BardMusicPlayer/Hypnotoad-Plugin/blob/7928be6735daf28e94121c3cf1c1dbbef0d97bcf/HypnotoadPlugin/Offsets/Offsets.cs#L18
        GetChatBox = Marshal.GetDelegateForFunctionPointer<GetChatBoxModuleDelegate>(
            scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9"));
    }
    public static PluginConfiguration Config { get; set; }

    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* RawPlayer
    => Control.Instance()->LocalPlayer;


    public static ActionID GetAdjustedActionId(ActionID id)
        => (ActionID)GetAdjustedActionId((uint)id);

    public static unsafe uint GetAdjustedActionId(uint id)
    => ActionManager.Instance()->GetAdjustedActionId(id);

    [PluginService]
    public static DalamudPluginInterface Interface { get; private set; }

    [PluginService]
    public static ChatGui ChatGui { get; private set; }

    [PluginService]
    public static GameGui GameGui { get; private set; }

    public static PlayerCharacter Player => ClientState.LocalPlayer;
    [PluginService]
    public static ClientState ClientState { get; set; }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => DataManager.GetExcelSheet<T>();

    internal static TextureWrap GetTextureIcon(uint id) => DataManager.GetImGuiTextureIcon(id);

    [PluginService]
    private static DataManager DataManager { get; set; }


    [PluginService]
    public static CommandManager CommandManager { get; private set; }

    [PluginService]
    public static Condition Conditions { get; private set; }

    [PluginService]
    public static JobGauges JobGauges { get; private set; }

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


    public delegate void GetChatBoxModuleDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);


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
