using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters.Actions;

internal class ActionSingleRotationGetter(Lumina.GameData gameData, ClassJob job)
    : ActionRotationGetterBase(gameData)
{
    protected override bool AddToList(Lumina.Excel.GeneratedSheets.Action item)
    {
        if (!base.AddToList(item)) return false;

        var category = item.ClassJobCategory.Value;
        if (category == null) return false;

        if (!category.IsSingleJobForCombat()) return false;

        var jobName = job.Abbreviation.RawString;
        return (bool?)category.GetType().GetRuntimeProperty(jobName)?.GetValue(category) ?? false;
    }
}

internal class ActionMultiRotationGetter(Lumina.GameData gameData)
    : ActionRotationGetterBase(gameData)
{
    protected override bool AddToList(Lumina.Excel.GeneratedSheets.Action item)
    {
        if (!base.AddToList(item)) return false;

        var category = item.ClassJobCategory.Value;
        if (category == null) return false;

        if (category.IsSingleJobForCombat()) return false;

        return true;
    }
}
