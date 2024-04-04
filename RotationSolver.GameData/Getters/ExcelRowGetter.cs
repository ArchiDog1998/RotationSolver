using Lumina.Excel;
using Microsoft.CodeAnalysis;

namespace RotationSolver.GameData.Getters;

internal abstract class ExcelRowGetter<T, TSyntax>(Lumina.GameData gameData) 
    where T : ExcelRow
    where TSyntax : SyntaxNode
{
    public List<string> AddedNames { get; } = [];

    protected Lumina.GameData _gameData = gameData;
    public int Count { get; private set; } = 0;

    protected abstract bool AddToList(T item);
    protected abstract TSyntax[] ToNodes(T item, string name);

    protected abstract string ToName(T item);

    public TSyntax[] GetNodes()
    {
        var items = _gameData.GetExcelSheet<T>();

        if (items == null) return [];
        AddedNames.Clear();

        var filteredItems = items.Where(AddToList);
        Count = filteredItems.Count();

        return [..filteredItems.SelectMany(item => 
        {
            var name = ToName(item).ToPascalCase();
            if (AddedNames.Contains(name))
            {
                name += "_" + item.RowId.ToString();
            }
            else
            {
                AddedNames.Add(name);
            }
            return ToNodes(item, name);
        })];
    }
}
