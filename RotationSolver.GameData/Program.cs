using Lumina;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using RotationSolver.GameData.Getters;
using RotationSolver.GameData.Getters.Actions;
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
res.AddResource("ActionCategory", new ActionCategoryGetter(gameData).GetCode());
res.AddResource("Action", new ActionFactoryGetter(gameData).GetCode());

var rotations = gameData.GetExcelSheet<ClassJob>()!
    .Where(job => job.JobIndex > 0)
    .Select(job => new RotationGetter(gameData, job))
    .ToDictionary(getter => getter.GetName(), getter => getter.GetCode());
res.AddResource("Rotation", JsonConvert.SerializeObject(rotations, Formatting.Indented));

res.Generate();

Console.WriteLine("Finished!");
