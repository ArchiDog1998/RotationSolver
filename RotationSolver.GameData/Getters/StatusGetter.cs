using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters;
internal class StatusGetter(Lumina.GameData gameData)
    : ExcelRowGetter<Status, EnumMemberDeclarationSyntax>(gameData)
{
    protected override bool AddToList(Status item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;
        return true;
    }

    protected override string ToName(Status item) => item.Name.RawString;

    protected override EnumMemberDeclarationSyntax[] ToNodes(Status item, string name)
    {
        var desc = item.Description.RawString;

        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        var cate = item.StatusCategory switch
        {
            1 => " ↑",
            2 => " ↓",
            _ => string.Empty,
        };

        return [EnumMember(name, (ushort)item.RowId).WithXmlComment($"""
        /// <summary>
        /// <see href="https://garlandtools.org/db/#status/{item.RowId}"><strong>{item.Name.RawString.Replace("&", "and")}</strong></see>{cate}{jobs}
        /// <para>{desc.Replace("\n", "</para>\n/// <para>")}</para>
        /// </summary>
        """)];
    }
}
