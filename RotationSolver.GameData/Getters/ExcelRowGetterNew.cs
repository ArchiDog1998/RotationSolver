using Lumina.Excel;
using Microsoft.CodeAnalysis;

namespace RotationSolver.GameData.Getters;

internal abstract class ExcelRowGetterNew<T, TSyntax>(Lumina.GameData gameData) 
    where T : ExcelRow
    where TSyntax : SyntaxNode
{
    private readonly List<string> _addedNames = [];

    protected Lumina.GameData _gameData = gameData;
    public int Count { get; private set; } = 0;

    protected abstract bool AddToList(T item);
    protected abstract TSyntax[] ToNodes(T item, string name);

    protected abstract string ToName(T item);

    public TSyntax[] GetCodes()
    {
        var items = _gameData.GetExcelSheet<T>();

        if (items == null) return [];
        _addedNames.Clear();

        var filteredItems = items.Where(AddToList);
        Count = filteredItems.Count();

        return [..filteredItems.SelectMany(item => 
        {
            var name = ToName(item).ToPascalCase();
            if (_addedNames.Contains(name))
            {
                name += "_" + item.RowId.ToString();
            }
            else
            {
                _addedNames.Add(name);
            }
            return ToNodes(item, name);
        })];
    }
}
