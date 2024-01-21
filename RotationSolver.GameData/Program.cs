using Lumina;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using RotationSolver.GameData;
using RotationSolver.GameData.Getters;
using RotationSolver.GameData.Getters.Actions;
using System.Resources.NetStandard;

var gameData = new GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack", new LuminaOptions
{
    LoadMultithreaded = true,
    CacheFileResources = true,
    PanicOnSheetChecksumMismatch = false,
    DefaultExcelLanguage = Language.English,
});

var dirInfo = new DirectoryInfo(typeof(Program).Assembly.Location);
dirInfo = dirInfo.Parent!.Parent!.Parent!.Parent!.Parent!;

using var res = new ResXResourceWriter(dirInfo.FullName + "\\RotationSolver.SourceGenerators\\Properties\\Resources.resx");

res.AddResource("StatusId", new StatusGetter(gameData).GetCode());
res.AddResource("ContentType", new ContentTypeGetter(gameData).GetCode());
res.AddResource("ActionId", new ActionIdGetter(gameData).GetCode());
res.AddResource("ActionCategory", new ActionCategoryGetter(gameData).GetCode());

var rotationBase = new ActionMultiRotationGetter(gameData);
var rotationCodes = rotationBase.GetCode();

res.AddResource("Action", $$"""
    using RotationSolver.Basic.Actions;
    
    namespace RotationSolver.Basic.Rotations;
    
    /// <summary>
    /// The Custom Rotation.
    /// <br>Number of Actions: {{rotationBase.Count}}</br>
    /// </summary>
    public partial class CustomRotation
    {
    {{rotationCodes.Table()}}
    }
    """);

var rotations = gameData.GetExcelSheet<ClassJob>()!
    .Where(job => job.JobIndex > 0)
    .Select(job => new RotationGetter(gameData, job))
    .ToDictionary(getter => getter.GetName(), getter => getter.GetCode());
res.AddResource("Rotation", JsonConvert.SerializeObject(rotations, Formatting.Indented));

res.Generate();

Console.WriteLine("Finished!");
