using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters;
internal class ContentTypeGetter(Lumina.GameData gameData)
    : ExcelRowGetter<ContentType>(gameData)
{
    protected override bool AddToList(ContentType item)
    {
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
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
