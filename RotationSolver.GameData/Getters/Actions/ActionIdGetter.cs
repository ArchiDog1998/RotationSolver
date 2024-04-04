using Microsoft.CodeAnalysis.CSharp.Syntax;
using Action = Lumina.Excel.GeneratedSheets.Action;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters.Actions;

internal class ActionIdGetter(Lumina.GameData gameData)
    : ActionGetterBase<EnumMemberDeclarationSyntax>(gameData)
{
    protected override EnumMemberDeclarationSyntax[] ToNodes(Action item, string name)
    {
        return [EnumMember(name, (ushort)item.RowId).WithXmlComment($"""
        /// <summary>
        /// {item.GetDescName()}
        /// {item.GetDesc(_gameData)}
        /// </summary>
        """)];
    }
}
