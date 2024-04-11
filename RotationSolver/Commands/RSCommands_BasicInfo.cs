﻿namespace RotationSolver.Commands;

public static partial class RSCommands
{
    internal static void DoOneCommand(string str)
    {
        if (TryGetOneEnum<StateCommandType>(str, out var stateType))
        {
            var intStr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (!int.TryParse(intStr, out var index)) index = -1;
            DoStateCommandType(stateType, index);
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

    internal static string GetCommandStr(this Enum command, string extraCommand = "")
    {
        var cmdStr = Service.COMMAND + " " + command.ToString();
        if (!string.IsNullOrEmpty(extraCommand))
        {
            cmdStr += " " + extraCommand;
        }
        return cmdStr;
    }
}
