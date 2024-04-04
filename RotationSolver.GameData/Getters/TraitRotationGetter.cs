using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters;
internal class TraitRotationGetter(Lumina.GameData gameData, ClassJob job)
    : ExcelRowGetter<Trait, PropertyDeclarationSyntax>(gameData)
{
    protected override string ToName(Trait item) => item.Name.RawString + "Trait";

    protected override bool AddToList(Trait item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;

        var category = item.ClassJobCategory.Value;
        if (category == null) return false;
        var jobName = job.Abbreviation.RawString;
        return (bool?)category.GetType().GetRuntimeProperty(jobName)?.GetValue(category) ?? false;
    }

    private static string GetDescName(Trait item)
    {
        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        return $"<see href=\"https://garlandtools.org/db/#action/{50000 + item.RowId}\"><strong>{item.Name.RawString}</strong></see>{jobs} [{item.RowId}]";
    }

    private string GetDesc(Trait item)
    {
        var desc = _gameData.GetExcelSheet<TraitTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }

    protected override PropertyDeclarationSyntax[] ToNodes(Trait item, string name)
    {
        return [PropertyDeclaration(ParseTypeName("global::RotationSolver.Basic.Traits.IBaseTrait"), name)
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))
                .WithInitializer(
            EqualsValueClause(
                ObjectCreationExpression(
                    IdentifierName("global::RotationSolver.Basic.Traits.BaseTrait"))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(item.RowId))))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithXmlComment($"""
                 /// <summary>
                 /// {GetDescName(item)}
                 /// {GetDesc(item)}
                 /// </summary>
                 """)];
    }
}
