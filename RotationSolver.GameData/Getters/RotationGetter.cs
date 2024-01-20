using Lumina.Excel.GeneratedSheets;
using RotationSolver.GameData.Getters.Actions;

namespace RotationSolver.GameData.Getters;

internal class RotationGetter(Lumina.GameData gameData, ClassJob job)
{
    public string GetName()
    {
        return (job.NameEnglish.RawString + " Rotation").ToPascalCase();
    }

    public string GetCode()
    {
        var jobName = job.NameEnglish.RawString;

        var jobs = $"Job.{job.Abbreviation}";
        if (job.RowId != 28 && job.RowId != job.ClassJobParent.Row)
        {
            jobs += $", Job.{job.ClassJobParent.Value?.Abbreviation ?? "ADV"}";
        }

        return $$"""
         using ECommons.DalamudServices;
         using ECommons.ExcelServices;
         using RotationSolver.Basic.Actions;
         using RotationSolver.Basic.Traits;

         namespace RotationSolver.Basic.Rotations.Basic;

         /// <summary>
         /// <see href="https://na.finalfantasyxiv.com/jobguide/{{jobName.Replace(" ", "").ToLower()}}"><strong>{{jobName}}</strong></see>
         /// </summary>
         public abstract partial class {{GetName()}} : CustomRotation
         {

             public sealed override Job[] Jobs => new[] { {{jobs}} };
             static {{job.Abbreviation}}Gauge JobGauge => Svc.Gauges.Get<{{job.Abbreviation}}Gauge>();

         #region Actions
         {{new ActionRotationGetter(gameData, job).GetCode().Table()}}
         #endregion

         #region Traits
         {{new TraitRotationGetter(gameData, job).GetCode().Table()}}
         #endregion
         }
         """;
    }
}
