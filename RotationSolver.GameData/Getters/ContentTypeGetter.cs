using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters;
internal class ContentTypeGetter(Lumina.GameData gameData)
    : ExcelRowGetter<ContentType, EnumMemberDeclarationSyntax>(gameData)
{
    protected override bool AddToList(ContentType item)
    {
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        return true;
    }

    protected override EnumMemberDeclarationSyntax[] ToNodes(ContentType item, string name)
    {
        return [EnumMember(name, (byte)item.RowId).WithXmlComment($"""
        /// <summary/>
        """)];
    }

    protected override string ToName(ContentType item)
    {
        return item.Name.RawString;
    }
}
