using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters.Actions;
internal class ActionCategoryGetter(Lumina.GameData gameData)
    : ExcelRowGetter<ActionCategory>(gameData)
{
    private readonly List<string> _addedNames = [];

    protected override void BeforeCreating()
    {
        _addedNames.Clear();
        base.BeforeCreating();
    }

    protected override bool AddToList(ActionCategory item)
    {
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        return true;
    }

    protected override string ToCode(ActionCategory item)
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
        /// 
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
