namespace RotationSolver.GameData.Getters.Actions;
internal abstract class ActionRotationGetterBase(Lumina.GameData gameData)
    : ActionGetterBase(gameData)
{
    protected override string ToCode(Lumina.Excel.GeneratedSheets.Action item)
    {
        var name = GetName(item);
        var descName = item.GetDescName();

        return item.ToCode(name, descName, GetDesc(item), IsDutyAction);
    }

    public abstract bool IsDutyAction { get; }
}
