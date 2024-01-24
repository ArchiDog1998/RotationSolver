namespace RotationSolver.GameData.Getters.Actions;
internal abstract class ActionRotationGetterBase(Lumina.GameData gameData)
    : ActionGetterBase(gameData)
{
    protected override string ToCode(Lumina.Excel.GeneratedSheets.Action item)
    {
        var name = GetName(item);
        var descName = GetDescName(item);

        var isDuty = IsDutyAction;

        if (isDuty)
        {
            descName += " Duty Action";
        }

        return $$"""
        private readonly Lazy<IBaseAction> _{{name}}Creator = new(() => 
        {
            IBaseAction action = new BaseAction(ActionID.{{name}}, {{isDuty.ToString().ToLower()}});
            CustomRotation.LoadActionConfigAndSetting(ref action);

            var setting = action.Setting;
            Modify{{name}}(ref setting);
            action.Setting = setting;

            return action;
        });

        /// <summary>
        /// Modify {{descName}}
        /// </summary>
        static partial void Modify{{name}}(ref ActionSetting setting);

        /// <summary>
        /// {{descName}}
        /// {{GetDesc(item)}}
        /// </summary>
        {{(item.ActionCategory.Row is 9 or 15 ? "private" : "public")}} IBaseAction {{name}} => _{{name}}Creator.Value;
        """;
    }

    public abstract bool IsDutyAction { get; }
}
