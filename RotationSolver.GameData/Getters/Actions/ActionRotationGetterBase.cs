namespace RotationSolver.GameData.Getters.Actions;
internal abstract class ActionRotationGetterBase(Lumina.GameData gameData)
    : ActionGetterBase(gameData)
{
    protected override string ToCode(Lumina.Excel.GeneratedSheets.Action item)
    {
        var name = GetName(item);
        var descName = GetDescName(item);

        return $$"""
        private readonly Lazy<IBaseAction> _{{name}}Creator = new(() => 
        {
            IBaseAction action = new BaseAction(ActionID.{{name}});
            Modify{{name}}(ref action);
            return action;
        });

        /// <summary>
        /// Modify {{GetDescName(item)}}
        /// </summary>
        static partial void Modify{{name}}(ref IBaseAction action);

        /// <summary>
        /// {{descName}}
        /// {{GetDesc(item)}}
        /// </summary>
        public IBaseAction {{name}} => _{{name}}Creator.Value;
        """;
    }
}
