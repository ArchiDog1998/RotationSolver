using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using XIVAutoAttack.Configuration;
using Dalamud.Game.ClientState.GamePad;

namespace XIVAutoAttack;

public class Service
{

    [PluginService]
    public static DalamudPluginInterface Interface { get; private set; }

    [PluginService]
    public static ChatGui ChatGui { get; private set; }

    [PluginService]
    public static GameGui GameGui { get; private set; }

    [PluginService]
    public static ClientState ClientState { get; private set; }

    [PluginService]
    public static CommandManager CommandManager { get; private set; }

    [PluginService]
    public static Condition Conditions { get; private set; }

    [PluginService]
    public static DataManager DataManager { get; private set; }

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

}
