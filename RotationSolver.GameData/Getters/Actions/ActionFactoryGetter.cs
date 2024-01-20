using System.Data;

namespace RotationSolver.GameData.Getters.Actions;

internal class ActionFactoryGetter(Lumina.GameData gameData)
    : ActionGetterBase(gameData)
{
    protected override string ToCode(Lumina.Excel.GeneratedSheets.Action item)
    {
        var name = GetName(item);

        return $$"""
        public static IBaseAction Create{{item.RowId}}()
        {
            IBaseAction action = new BaseAction(ActionID.{{name}});
            Modify{{name}}(ref action);
            return action;
        });

        /// <summary>
        /// Modify {{GetDescName(item)}}
        /// </summary>
        static partial void Modify{{item.Name.RawString.ToPascalCase()}}_{{item.RowId}}(ref IBaseAction action);
        """;
    }
}
