using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.GameData.Getters.Actions;

internal class ActionIdGetter(Lumina.GameData gameData)
    : ActionGetterBase(gameData)
{
    protected override string ToCode(Action item)
    {
        var name = GetName(item);

        return $"""
        /// <summary>
        /// {item.GetDescName()}
        /// {GetDesc(item)}
        /// </summary>
        {name} = {item.RowId},
        """;
    }
}
