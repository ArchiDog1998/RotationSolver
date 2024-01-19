using Lumina.Excel;

namespace RotationSolver.GameData;
internal abstract class ExcelRowGetter<T>(Lumina.GameData gameData) where T :ExcelRow
{
    protected abstract bool AddToList(T item);
    protected abstract string ToCode(T item);

    protected virtual void BeforeCreating() { }

    public string GetCode()
    {
        var items = gameData.GetExcelSheet<T>();

        if (items == null) return string.Empty;
        BeforeCreating();
        return string.Join("\n", items.Where(AddToList).Select(ToCode));
    }
}
