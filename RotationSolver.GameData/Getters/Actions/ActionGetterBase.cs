using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.GameData.Getters.Actions;
internal abstract class ActionGetterBase(Lumina.GameData gameData)
    : ExcelRowGetter<Action>(gameData)
{
    private readonly List<string> _addedNames = [];
    private string[] _notCombatJobs = [];
    protected override void BeforeCreating()
    {
        _addedNames.Clear();
        _notCombatJobs = [.. gameData.GetExcelSheet<ClassJob>()!.Where(c =>
        {
            return c.ClassJobCategory.Row is 32 or 33;
        }).Select(c => c.Abbreviation.RawString)];
        base.BeforeCreating();
    }

    protected override bool AddToList(Action item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;

        //No crafting or gathering.
        var category = item.ClassJobCategory.Value;
        if (category == null) return false;

        if (category.RowId == 1) return true;

        if (_notCombatJobs.Any(name =>
        {
            return (bool?)category.GetType().GetRuntimeProperty(name)?.GetValue(category) ?? false;
        }))
        {
            return false;
        }

        return true;
    }

    protected string GetName(Action item)
    {
        var name = item.Name.RawString.ToPascalCase()
            + (item.IsPvP ? "PvP" : "PvE");

        if (_addedNames.Contains(name))
        {
            name += "_" + item.RowId.ToString();
        }
        else
        {
            _addedNames.Add(name);
        }
        return name;
    }

    protected static string GetDescName(Action item)
    {
        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        var cate = item.IsPvP ? " <i>PvP</i>" : " <i>PvE</i>";

        return $"<see href=\"https://garlandtools.org/db/#action/{item.RowId}\"><strong>{item.Name.RawString}</strong></see>{cate}{jobs} [{item.RowId}] [{item.ActionCategory.Value?.Name.RawString ?? string.Empty}]";
    }

    protected string GetDesc(Action item)
    {
        var desc = gameData.GetExcelSheet<ActionTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }
}
