using Lumina;
using RotationSolver.GameData.Getters;
using System.Resources.NetStandard;

var gameData = new GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack", new LuminaOptions
{
    LoadMultithreaded = true,
    CacheFileResources = true,
    PanicOnSheetChecksumMismatch = false,
    DefaultExcelLanguage = Lumina.Data.Language.English,
});

var dirInfo = new DirectoryInfo(typeof(Program).Assembly.Location);
dirInfo = dirInfo.Parent!.Parent!.Parent!.Parent!.Parent!;

using var res = new ResXResourceWriter(dirInfo.FullName + "\\RotationSolver.SourceGenerators\\Properties\\Resources.resx");

res.AddResource("StatusId", new StatusGetter(gameData).GetCode());
res.AddResource("ContentType", new ContentTypeGetter(gameData).GetCode());
res.AddResource("ActionId", new ActionGetter(gameData).GetCode());

res.Generate();

Console.WriteLine("Finished!");