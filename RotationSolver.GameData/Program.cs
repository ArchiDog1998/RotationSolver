using Lumina;
using Lumina.Data;
using RotationSolver.GameData;

var gameData = new GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack", new LuminaOptions
{
    LoadMultithreaded = true,
    CacheFileResources = true,
    PanicOnSheetChecksumMismatch = false,
    DefaultExcelLanguage = Language.English,
});

var dirInfo = new DirectoryInfo(typeof(Program).Assembly.Location);
dirInfo = dirInfo.Parent!.Parent!.Parent!.Parent!.Parent!;

await CodeGenerator.CreateCode(gameData, dirInfo);

Console.WriteLine("Finished!");