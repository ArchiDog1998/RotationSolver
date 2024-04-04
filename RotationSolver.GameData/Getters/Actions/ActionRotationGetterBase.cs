using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RotationSolver.GameData.Getters.Actions;
internal abstract class ActionRotationGetterBase(Lumina.GameData gameData)
    : ActionGetterBase<MemberDeclarationSyntax>(gameData)
{
    protected override MemberDeclarationSyntax[] ToNodes(Lumina.Excel.GeneratedSheets.Action item, string name)
    {
        var descName = item.GetDescName();

        return item.ToNodes(name, descName, item.GetDesc(_gameData), IsDutyAction);
    }

    public abstract bool IsDutyAction { get; }
}
