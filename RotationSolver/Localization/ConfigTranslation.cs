using RotationSolver.Basic.Configuration;
using RotationSolver.UI;

namespace RotationSolver.Localization;

internal static class ConfigTranslation
{
    [Obsolete]
    public static string ToCommandStr(object obj, string extra = "")
    {
        var result = Service.COMMAND + " " + OtherCommandType.Settings.ToString() + " " + obj.ToString();
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }
}
