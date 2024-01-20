using Lumina.Excel.GeneratedSheets;
using System.Text.RegularExpressions;

namespace RotationSolver.GameData;
internal static partial class Util
{
    public static bool IsSingleJobForCombat(this ClassJobCategory jobCategory)
    {
        var str = jobCategory.Name.RawString.Replace(" ", "");
        if (!str.All(char.IsUpper)) return false;
        if (str.Length is not 3 and not 6) return false;
        return true;
    }

    public static string Table(this string str) => "    " + str.Replace("\n", "\n    ");

    public static string OnlyAscii(this string input) => new string(input.Where(char.IsAscii).ToArray());

    public  static string ToPascalCase(this string input)
    {
        var pascalCase = InvalidCharsRgx().Replace(WhiteSpace().Replace(input, "_"), string.Empty)
            .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => StartWithLowerCaseChar().Replace(w, m => m.Value.ToUpper()))
            .Select(w => FirstCharFollowedByUpperCasesOnly().Replace(w, m => m.Value.ToLower()))
            .Select(w => LowerCaseNextToNumber().Replace(w, m => m.Value.ToUpper()))
            .Select(w => UpperCaseInside().Replace(w, m => m.Value.ToLower()));

        var result = string.Concat(pascalCase);

        if (result.Length > 0 && char.IsNumber(result[0]))
        {
            result = "_" + result;
        }
        return result;
    }

    [GeneratedRegex("[^_a-zA-Z0-9]")]
    private static partial Regex InvalidCharsRgx();
    [GeneratedRegex("(?<=\\s)")]
    private static partial Regex WhiteSpace();
    [GeneratedRegex("^[a-z]")]
    private static partial Regex StartWithLowerCaseChar();
    [GeneratedRegex("(?<=[A-Z])[A-Z0-9]+$")]
    private static partial Regex FirstCharFollowedByUpperCasesOnly();
    [GeneratedRegex("(?<=[0-9])[a-z]")]
    private static partial Regex LowerCaseNextToNumber();
    [GeneratedRegex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))")]
    private static partial Regex UpperCaseInside();
}
