using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.GameData.Getters;

internal class ActionGetter(Lumina.GameData gameData) 
    : ExcelRowGetter<Action>(gameData)
{
    private readonly List<string> _addedNames = [];

    protected override void BeforeCreating()
    {
        _addedNames.Clear();
        base.BeforeCreating();
    }

    protected override bool AddToList(Action item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;
        return true;
    }

    protected override string ToCode(Action item)
    {
        var name = item.Name.RawString.ToPascalCase();
        if (_addedNames.Contains(name))
        {
            name += "_" + item.RowId.ToString();
        }
        else
        {
            _addedNames.Add(name);
        }

        var desc = gameData.GetExcelSheet<ActionTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        var cate = item.IsPvP ? " <i>PvP</i>" : " <i>PvE</i>";

        return $"""
        /// <summary>
        /// <see href="https://garlandtools.org/db/#action/{item.RowId}"><strong>{item.Name.RawString}</strong></see>{cate}{jobs}
        /// <para>{desc.Replace("\n", "</para>\n/// <para>")}</para>
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
