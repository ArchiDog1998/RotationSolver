using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData.Getters;

internal class ItemGetter(Lumina.GameData gameData)
    : ExcelRowGetter<Item, MemberDeclarationSyntax>(gameData)
{

    protected override bool AddToList(Item item)
    {
        if (item.ItemSearchCategory.Row != 43) return false;
        if (item.FilterGroup is not 10 and not 16 and not 19) return false;

        return true;
    }

    protected override string ToName(Item item)
    {
        return item.Singular.RawString;
    }

    protected override MemberDeclarationSyntax[] ToNodes(Item item, string name)
    {
        var desc = item.Description.RawString ?? string.Empty;

        desc = $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";

        var descName = $"<see href=\"https://garlandtools.org/db/#item/{item.RowId}\"><strong>{item.Name.RawString}</strong></see> [{item.RowId}]";


        var field = FieldDeclaration(
            VariableDeclaration(
                GenericName(
                    Identifier("global::System.Lazy"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("global::RotationSolver.Basic.Actions.IBaseItem")))))
            .WithVariables(
                SingletonSeparatedList(
                    VariableDeclarator(
                        Identifier($"_{name}Creator"))
                    .WithInitializer(
                        EqualsValueClause(
                            ImplicitObjectCreationExpression()
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            ParenthesizedLambdaExpression()
                                            .WithExpressionBody(
                                                ObjectCreationExpression(
                                                    IdentifierName("global::RotationSolver.Basic.Actions.BaseItem"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(item.RowId))))))))))))))))
            .AddAttributeLists(GeneratedCodeAttribute(typeof(SyntaxHelper)))
            .WithModifiers(
                TokenList(
                    [
                        Token(SyntaxKind.PrivateKeyword),
                        Token(SyntaxKind.ReadOnlyKeyword)]));

        var property = PropertyDeclaration(
            IdentifierName("global::RotationSolver.Basic.Actions.IBaseItem"),
            Identifier(name))
        .WithModifiers(
            TokenList(
                Token(SyntaxKind.PublicKeyword)))
        .WithExpressionBody(
            ArrowExpressionClause(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName($"_{name}Creator"),
                    IdentifierName("Value"))))
        .WithSemicolonToken(
            Token(SyntaxKind.SemicolonToken))
        .AddAttributeLists(GeneratedCodeAttribute(typeof(SyntaxHelper)).WithXmlComment($$"""
            /// <summary>
            /// {{descName}}
            /// {{desc}}
            /// </summary>
            """));

        return [field, property];
    }
}
