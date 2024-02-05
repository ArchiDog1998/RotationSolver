using Lumina.Excel;

namespace RotationSolver.GameData.Getters;
internal abstract class ExcelRowGetter<T>(Lumina.GameData gameData) where T : ExcelRow
{
    protected Lumina.GameData _gameData = gameData;
    public int Count { get; private set; } = 0;

    protected abstract bool AddToList(T item);
    protected abstract string ToCode(T item);

    protected virtual void BeforeCreating() { }


    public string GetCode()
    {
        var items = _gameData.GetExcelSheet<T>();

        if (items == null) return string.Empty;
        BeforeCreating();

        var filteredItems = items.Where(AddToList);
        Count = filteredItems.Count();

        return string.Join("\n", filteredItems.Select(ToCode));
    }
}
