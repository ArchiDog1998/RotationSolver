using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.GameData.Getters.Actions;
using System.Xml.Linq;

namespace RotationSolver.GameData.Getters;

internal class RotationGetter(Lumina.GameData gameData, ClassJob job)
{
    public string GetName()
    {
        return (job.Name.ToString() + " Rotation").ToPascalCase();
    }

    public string GetCode()
    {
        var jobName = job.Name.ToString();

        var jobs = $"Job.{job.Abbreviation}";
        if (job.RowId != 28 && job.RowId != job.ClassJobParent.Row)
        {
            jobs += $", Job.{job.ClassJobParent.Value?.Abbreviation ?? "ADV"}";
        }

        var jobGauge = job.Abbreviation == "BLU" ? string.Empty : $"static {job.Abbreviation}Gauge JobGauge => Svc.Gauges.Get<{job.Abbreviation}Gauge>();";

        var rotationsGetter = new ActionSingleRotationGetter(gameData, job);
        var traitsGetter = new TraitRotationGetter(gameData, job);

        var rotationsCode = rotationsGetter.GetCode();
        var traitsCode = traitsGetter.GetCode();

        return $$"""
         /// <summary>
         /// <see href="https://na.finalfantasyxiv.com/jobguide/{{jobName?.Replace(" ", "").ToLower()}}"><strong>{{jobName}}</strong></see>
         /// <br>Number of Actions: {{rotationsGetter.Count}}</br>
         /// <br>Number of Traits: {{traitsGetter.Count}}</br>
         /// </summary>
         [Jobs({{jobs}})]
         public abstract partial class {{GetName()}} : CustomRotation
         {
             {{jobGauge}}

         #region Actions
         {{rotationsCode.Table()}}

         {{Util.ArrayNames("AllBaseActions", "IBaseAction",
         "public override", [.. rotationsGetter.AddedNames]).Table()}}

         {{GetLBInRotation(job.LimitBreak1.Value, 1)}}
         {{GetLBInRotation(job.LimitBreak2.Value, 2)}}
         {{GetLBInRotation(job.LimitBreak3.Value, 3)}}
         {{GetLBInRotationPvP(gameData.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.FirstOrDefault(i => i.ActionCategory.Row is 15
            && ((bool?)i.ClassJobCategory.Value?.GetType().GetRuntimeProperty(job.Abbreviation)?.GetValue(i.ClassJobCategory.Value) ?? false)))}}

         #endregion

         #region Traits
         
         {{traitsCode.Table()}}

         {{Util.ArrayNames("AllTraits", "IBaseTrait",
         "public override", [..traitsGetter.AddedNames]).Table()}}
         #endregion
         }
         """;
    }

    private string GetLBInRotation(Lumina.Excel.GeneratedSheets.Action? action, int index)
    {
        if (action == null) return string.Empty;
        if (action.RowId == 0) return string.Empty;

        var code = GetLBPvE(action, out var name);

        return code + "\n" + $"""
            /// <summary>
            /// {action.GetDescName()}
            /// {GetDesc(action)}
            /// </summary>
            private sealed protected override IBaseAction LimitBreak{index} => {name};
            """;
    }
    private string GetLBPvE(Lumina.Excel.GeneratedSheets.Action action, out string name)
    {
        name = action.Name.RawString.ToPascalCase() + $"PvE";
        var descName = action.GetDescName();

        return action.ToCode(name, descName, GetDesc(action), false);
    }
    private string GetLBInRotationPvP(Lumina.Excel.GeneratedSheets.Action? action)
    {
        if (action == null) return string.Empty;
        if (action.RowId == 0) return string.Empty;

        var code = GetLBPvP(action, out var name);

        return code + "\n" + $"""
            /// <summary>
            /// {action.GetDescName()}
            /// {GetDesc(action)}
            /// </summary>
            private sealed protected override IBaseAction LimitBreakPvP => {name};
            """;
    }

    private string GetLBPvP(Lumina.Excel.GeneratedSheets.Action action, out string name)
    {
        name = action.Name.RawString.ToPascalCase() + $"PvP";
        var descName = action.GetDescName();

        return action.ToCode(name, descName, GetDesc(action), false);
    }

    private string GetDesc(Lumina.Excel.GeneratedSheets.Action item)
    {
        var desc = gameData.GetExcelSheet<ActionTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }
}
