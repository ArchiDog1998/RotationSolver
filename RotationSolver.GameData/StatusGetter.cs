using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData;
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

        return $"""
        /// <summary>
        /// {item.Description.RawString.OnlyAscii().Replace("\n", "\n/// ")}
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
