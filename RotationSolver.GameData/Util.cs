using System.Text.RegularExpressions;

namespace RotationSolver.GameData;
internal static partial class Util
{
    public static string OnlyAscii(this string input) => new string(input.Where(char.IsAscii).ToArray());

    public  static string ToPascalCase(this string input)
    {
        var pascalCase = InvalidCharsRgx().Replace(WhiteSpace().Replace(input, "_"), string.Empty)
            .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => StartWithLowerCaseChar().Replace(w, m => m.Value.ToUpper()))
            .Select(w => FirstCharFollowedByUpperCasesOnly().Replace(w, m => m.Value.ToLower()))
            .Select(w => LowerCaseNextToNumber().Replace(w, m => m.Value.ToUpper()))
            .Select(w => UpperCaseInside().Replace(w, m => m.Value.ToLower()));

        return string.Concat(pascalCase);
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
