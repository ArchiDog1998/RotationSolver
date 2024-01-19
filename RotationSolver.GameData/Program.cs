// See https://aka.ms/new-console-template for more information
using Lumina;
using Lumina.Excel.GeneratedSheets;
using System.Resources.NetStandard;
using System.Text.RegularExpressions;

var gameData = new GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack", new LuminaOptions
{
    LoadMultithreaded = true,
    CacheFileResources = true,
    PanicOnSheetChecksumMismatch = false,
    DefaultExcelLanguage = Lumina.Data.Language.English,
});

var statuses = gameData.GetExcelSheet<Status>();

if (statuses == null) return;

Dictionary<string, byte> _count = [];
var status =string.Join("\n", statuses.Where(UseStatus).Select(StatusString));

var dirInfo = new DirectoryInfo(typeof(Program).Assembly.Location);
dirInfo = dirInfo.Parent!.Parent!.Parent!.Parent!.Parent!;

using var res = new ResXResourceWriter(dirInfo.FullName + "\\RotationSolver.SourceGenerators\\Properties\\Resources.resx");

res.AddResource("StatusId", status);
res.Generate();

Console.WriteLine(status);

static bool UseStatus(Status status)
{
    if (status.ClassJobCategory.Row == 0) return false;
    var name = status.Name.RawString;
    if (string.IsNullOrEmpty(name)) return false;
    if (!name.All(char.IsAscii)) return false;
    return true;
}

string StatusString(Status status)
{
    var name = ConvertToPascalCase(status.Name.RawString);
    if (_count.ContainsKey(name))
    {
        name += "_" + (++_count[name]).ToString();
    }
    else
    {
        _count[name] = 1;
    }

    var desc = new string(status.Description.RawString.Where(char.IsAscii).ToArray());

    return $"""
        /// <summary>
        /// {desc.Replace("\n", "\n/// ")}
        /// </summary>
        {name} = {status.RowId},
        """;
}


static string ConvertToPascalCase(string input)
{
    Regex invalidCharsRgx = new(@"[^_a-zA-Z0-9]");
    Regex whiteSpace = new(@"(?<=\s)");
    Regex startsWithLowerCaseChar = new("^[a-z]");
    Regex firstCharFollowedByUpperCasesOnly = new("(?<=[A-Z])[A-Z0-9]+$");
    Regex lowerCaseNextToNumber = new("(?<=[0-9])[a-z]");
    Regex upperCaseInside = new("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

    var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(input, "_"), string.Empty)
        .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
        .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
        .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
        .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

    return string.Concat(pascalCase);
}