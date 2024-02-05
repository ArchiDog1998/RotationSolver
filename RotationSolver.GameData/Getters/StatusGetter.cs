using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters;
internal class StatusGetter(Lumina.GameData gameData)
    : ExcelRowGetter<Status>(gameData)
{
    private readonly List<string> _addedNames = [];

    protected override void BeforeCreating()
    {
        _addedNames.Clear();
        base.BeforeCreating();
    }

    protected override bool AddToList(Status item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;
        return true;
    }

    protected override string ToCode(Status item)
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

        var desc = item.Description.RawString;

        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        var cate = item.StatusCategory switch
        {
            1 => " ↑",
            2 => " ↓",
            _ => string.Empty,
        };

        return $"""
        /// <summary>
        /// <see href="https://garlandtools.org/db/#status/{item.RowId}"><strong>{item.Name.RawString.Replace("&", "and")}</strong></see>{cate}{jobs}
        /// <para>{desc.Replace("\n", "</para>\n/// <para>")}</para>
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
