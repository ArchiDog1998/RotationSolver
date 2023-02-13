using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using RotationSolver.Configuration;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;

namespace RotationSolver;

internal class Service
{
    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager* CharacterManager
        => FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager.Instance();

    internal static unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* Player
        => Control.Instance()->LocalPlayer;
    internal static LocalizationManager Localization { get; set; }

    internal static PluginConfiguration Configuration { get; set; }

    internal static IconReplacer IconReplacer { get; set; }

    internal static PluginAddressResolver Address { get; set; }

    [PluginService]
    internal static DalamudPluginInterface Interface { get; private set; }

    [PluginService]
    internal static ChatGui ChatGui { get; private set; }

    [PluginService]
    public static GameGui GameGui { get; private set; }

    [PluginService]
    internal static ClientState ClientState { get; private set; }

    [PluginService]
    internal static CommandManager CommandManager { get; private set; }

    [PluginService]
    internal static Condition Conditions { get; private set; }

    [PluginService]
    internal static DataManager DataManager { get; private set; }

    [PluginService]
    internal static JobGauges JobGauges { get; private set; }

    [PluginService]
    internal static ObjectTable ObjectTable { get; private set; }

    [PluginService]
    internal static TargetManager TargetManager { get; private set; }

    [PluginService]
    internal static PartyList PartyList { get; private set; }

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

}
