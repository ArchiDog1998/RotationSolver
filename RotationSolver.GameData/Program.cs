using Lumina;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using RotationSolver.GameData;
using RotationSolver.GameData.Getters;
using RotationSolver.GameData.Getters.Actions;
using System.Net;
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

var rotationBase = new ActionRoleRotationGetter(gameData);
var rotationCodes = rotationBase.GetCode();
var rotationItems = new ItemGetter(gameData);
var rotationItemCodes = rotationItems.GetCode();

res.AddResource("Action", $$"""
    using RotationSolver.Basic.Actions;
    
    namespace RotationSolver.Basic.Rotations;
    
    /// <summary>
    /// The Custom Rotation.
    /// <br>Number of Actions: {{rotationBase.Count}}</br>
    /// </summary>
    public abstract partial class CustomRotation
    {
    #region Actions
    {{rotationCodes.Table()}}

    {{Util.ArrayNames("AllBaseActions", "IBaseAction",
    "public virtual", [.. rotationBase.AddedNames]).Table()}}
    #endregion

    #region Items
    {{rotationItemCodes.Table()}}

    {{Util.ArrayNames("AllItems", "IBaseItem",
    "public ", [.. rotationItems.AddedNames]).Table()}}
    #endregion
    }
    """);

var dutyRotationBase = new ActionDutyRotationGetter(gameData);
rotationCodes = dutyRotationBase.GetCode();

res.AddResource("DutyAction", $$"""
    using RotationSolver.Basic.Actions;
    
    namespace RotationSolver.Basic.Rotations.Duties;
    
    /// <summary>
    /// The Custom Rotation.
    /// <br>Number of Actions: {{dutyRotationBase.Count}}</br>
    /// </summary>
    public abstract partial class DutyRotation
    {
    {{rotationCodes.Table()}}
    }
    """);

var header = """
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

""";

var rotations = gameData.GetExcelSheet<ClassJob>()!
    .Where(job => job.JobIndex > 0)
    .Select(job => new RotationGetter(gameData, job).GetCode());
res.AddResource("Rotation", header + string.Join("\n\n", rotations));

res.Generate();

await OpCodeGetter.GetOpCode(dirInfo);

Console.WriteLine("Finished!");