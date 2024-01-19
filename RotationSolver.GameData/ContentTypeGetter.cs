using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData;
internal class ContentTypeGetter(Lumina.GameData gameData)
    : ExcelRowGetter<ContentType>(gameData)
{
    protected override bool AddToList(ContentType item)
    {
        if (string.IsNullOrEmpty(item.Name.RawString)) return false;
        return true;
    }

    protected override string ToCode(ContentType item)
    {
        var name = item.Name.RawString.ToPascalCase();
        return $"""
        /// <summary>
        /// 
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
