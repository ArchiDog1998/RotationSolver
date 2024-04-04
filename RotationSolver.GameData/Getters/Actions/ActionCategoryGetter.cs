using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters.Actions;
internal class ActionCategoryGetter(Lumina.GameData gameData)
    : ExcelRowGetter<ActionCategory, EnumMemberDeclarationSyntax>(gameData)
{
    protected override bool AddToList(ActionCategory item)
    {
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        return true;
    }

    protected override EnumMemberDeclarationSyntax[] ToNodes(ActionCategory item, string name)
    {
        return [EnumMember(name, (byte)item.RowId).WithXmlComment($"""
        /// <summary/>
        """)];
    }

    protected override string ToName(ActionCategory item)
    {
        return item.Name.RawString;
    }
}
