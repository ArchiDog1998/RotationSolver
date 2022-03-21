using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace XIVComboPlus;

internal class Service
{
    internal static PluginConfiguration Configuration { get; set; }

    internal static IconReplacer IconReplacer { get; set; }

    internal static PluginAddressResolver Address { get; set; }

    [PluginService]
    internal static DalamudPluginInterface Interface { get; private set; }

    [PluginService]
    internal static ChatGui ChatGui { get; private set; }

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
    public static Framework Framework { get; private set; }
}
