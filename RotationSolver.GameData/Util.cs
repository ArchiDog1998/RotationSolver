﻿using Lumina.Excel.GeneratedSheets;
using System.Text.RegularExpressions;

namespace RotationSolver.GameData;
internal static partial class Util
{
    public static bool IsSingleJobForCombat(this ClassJobCategory jobCategory)
    {
        if (jobCategory.RowId == 68) return true; // ACN SMN SCH 
        var str = jobCategory.Name.RawString.Replace(" ", "");
        if (!str.All(char.IsUpper)) return false;
        if (str.Length is not 3 and not 6) return false;
        return true;
    }

    public static string Table(this string str) => "    " + str.Replace("\n", "\n    ");

    public static string Space(this string str)
    {
        string result = string.Empty;

        bool lower = false;
        foreach (var c in str)
        {
            var isLower = char.IsLower(c);
            if (lower && !isLower)
            {
                result += ' ';
            }
            lower = isLower;
            result += c;
        }

        return result;
    }

    public static string OnlyAscii(this string input) => new(input.Where(char.IsAscii).ToArray());

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

    public static string GetDescName(this Lumina.Excel.GeneratedSheets.Action action)
    {
        var jobs = action.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        var cate = action.IsPvP ? "-<i>PvP</i>" : "-<i>PvE</i>";

        return $"<see href=\"https://garlandtools.org/db/#action/{action.RowId}\"><strong>{action.Name.RawString}</strong></see>{cate}{jobs} [{action.RowId}] [{action.ActionCategory.Value?.Name.RawString ?? string.Empty}]";
    }

    public static  string GetDesc(this Lumina.Excel.GeneratedSheets.Action item, Lumina.GameData gameData)
    {
        var desc = gameData.GetExcelSheet<ActionTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }
}
