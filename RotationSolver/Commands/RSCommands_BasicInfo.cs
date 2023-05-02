using Dalamud.Game.Command;
using RotationSolver.Localization;

namespace RotationSolver.Commands;

public static partial class RSCommands
{
    internal static TargetingType TargetingType
    {
        get
        {
            if (Service.Config.TargetingTypes.Count == 0)
            {
                Service.Config.TargetingTypes.Add(TargetingType.Big);
                Service.Config.TargetingTypes.Add(TargetingType.Small);
                Service.Config.Save();
            }

            return Service.Config.TargetingTypes[Service.Config.TargetingIndex %= Service.Config.TargetingTypes.Count];
        }
    }

    internal static void Enable()
        => Service.CommandManager.AddHandler(Service.Command, new CommandInfo(OnCommand)
        {
            HelpMessage = LocalizationManager.RightLang.Commands_Rotation,
            ShowInHelp = true,
        });

    internal static void Disable() => Service.CommandManager.RemoveHandler(Service.Command);

    private static void OnCommand(string command, string arguments)
    {
        DoOneCommand(arguments);
    }

    private static void DoOneCommand(string str)
    {
        if (TryGetOneEnum<StateCommandType>(str, out var stateType))
        {
            DoStateCommandType(stateType);
        }
        else if (TryGetOneEnum<SpecialCommandType>(str, out var specialType))
        {
            DoSpecialCommandType(specialType);
        }
        else if (TryGetOneEnum<OtherCommandType>(str, out var otherType))
        {
            DoOtherCommand(otherType, str[otherType.ToString().Length..].Trim());
        }
        else
        {
            RotationSolverPlugin.OpenConfigWindow();
        }
    }

    private static bool TryGetOneEnum<T>(string str, out T type) where T : struct, Enum
    {
        type = default;
        try
        {
            type = Enum.GetValues<T>().First(c => str.StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase));
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static string GetCommandStr<T>(this T command, string extraCommand = "")
        where T : struct, Enum
    {
        var cmdStr = Service.Command + " " + command.ToString();
        if (!string.IsNullOrEmpty(extraCommand))
        {
            cmdStr += " " + extraCommand;
        }
        return cmdStr;
    }
}
