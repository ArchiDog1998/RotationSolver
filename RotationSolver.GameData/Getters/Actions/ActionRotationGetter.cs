using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters.Actions;
internal class ActionRotationGetter(Lumina.GameData gameData, ClassJob job)
    : ActionGetterBase(gameData)
{
    protected override bool AddToList(Lumina.Excel.GeneratedSheets.Action item)
    {
        if (!base.AddToList(item)) return false;

        var category = item.ClassJobCategory.Value;
        if (category == null) return false;
        var jobName = job.Abbreviation.RawString;
        return (bool?)category.GetType().GetRuntimeProperty(jobName)?.GetValue(category) ?? false;
    }

    protected override string ToCode(Lumina.Excel.GeneratedSheets.Action item)
    {
        var name = GetName(item);

        return $$"""
        private readonly Lazy<IBaseAction> _{{name}}Creator = new(ActionFactory.Create{{item.RowId}});
        /// <summary>
        /// {{GetDescName(item)}}
        /// {{GetDesc(item)}}
        /// </summary>
        public IBaseAction {{name}} => _{{name}}Creator.Value;
        """;
    }
}
