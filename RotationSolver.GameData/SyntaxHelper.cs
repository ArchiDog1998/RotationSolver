using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RotationSolver.GameData;

public static class SyntaxHelper
{
    public static MemberDeclarationSyntax[] ToNodes(this Lumina.Excel.GeneratedSheets.Action item,
        string actionName, string actionDescName, string desc, bool isDuty)
    {
        //if (isDuty)
        //{
        //    actionDescName += " Duty Action";
        //}

        var field = ParseSyntax<FieldDeclarationSyntax>($$"""
         private readonly global::System.Lazy<global::RotationSolver.Basic.Actions.IBaseAction> _{{actionName}}Creator = new(() => 
         {
             global::RotationSolver.Basic.Actions.IBaseAction action = new global::RotationSolver.Basic.Actions.BaseAction(global::RotationSolver.Basic.Data.ActionID.{{actionName}}, {{isDuty.ToString().ToLower()}});
             CustomRotation.LoadActionSetting(ref action);
         
             var setting = action.Setting;
             Modify{{actionName}}(ref setting);
             action.Setting = setting;
         
             return action;
         });
         """).AddAttributeLists(GeneratedCodeAttribute(typeof(SyntaxHelper)));

        var modifyMethod = MethodDeclaration(
            PredefinedType(Token(SyntaxKind.VoidKeyword)),
            Identifier($"Modify{actionName}"))
        .WithModifiers(TokenList([Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)]))
        .WithParameterList(
            ParameterList(SingletonSeparatedList(
                    Parameter(Identifier("setting"))
                    .WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                    .WithType(IdentifierName("global::RotationSolver.Basic.Actions.ActionSetting")))))
        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        .AddAttributeLists(GeneratedCodeAttribute(typeof(SyntaxHelper)).WithXmlComment($$"""
            /// <summary>
            /// Modify <inheritdoc cref="{{actionName}}"/>
            /// </summary>
            """));

        var attributes = GeneratedCodeAttribute(typeof(SyntaxHelper));
        if (isDuty)
        {
            attributes = attributes.WithAttributes(SingletonSeparatedList(IDAttribute(item.RowId)));
        }

        var property = PropertyDeclaration(
            IdentifierName("global::RotationSolver.Basic.Actions.IBaseAction"),
            Identifier(actionName))
        .WithModifiers(
            TokenList(
                Token(item.ActionCategory.Row is 9 ? SyntaxKind.PrivateKeyword : SyntaxKind.PublicKeyword)))
        .WithExpressionBody(
            ArrowExpressionClause(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName($"_{actionName}Creator"),
                    IdentifierName("Value"))))
        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        .AddAttributeLists(attributes).WithXmlComment($$"""
            /// <inheritdoc cref="global::RotationSolver.Basic.Data.ActionID.{{actionName}}"/>
            """);

        return [field, modifyMethod, property];
    }

    private static AttributeSyntax IDAttribute(uint id)
    {
        var attributeArgument = AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(id)));
        return Attribute(IdentifierName("global::RotationSolver.Basic.Attributes.ID"), AttributeArgumentList(SingletonSeparatedList(attributeArgument)));
    }

    public static TNode ParseSyntax<TNode>(string code) where TNode : MemberDeclarationSyntax
    {
        return (TNode)((CompilationUnitSyntax)ParseSyntaxTree(code).GetRoot()).Members[0];
    }

    public static AttributeListSyntax GeneratedCodeAttribute(Type generator)
    {
        return AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
           .AddArgumentListArguments(
               AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(generator.FullName ?? generator.Name))),
               AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(generator.Assembly.GetName().Version?.ToString() ?? "1.0.0"))))));
    }
    public static EnumMemberDeclarationSyntax EnumMember(string name, byte value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }
    public static EnumMemberDeclarationSyntax EnumMember(string name, ushort value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }

    public static EnumMemberDeclarationSyntax EnumMember(string name, uint value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }

    public static AttributeSyntax DescriptionAttribute(string description)
    {
        var attributeArgument = AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(description)));
        return Attribute(IdentifierName("global::System.ComponentModel.Description"), AttributeArgumentList(SingletonSeparatedList(attributeArgument)));
    }

    public static TSyntax WithXmlComment<TSyntax>(this TSyntax node, string comment)
        where TSyntax : SyntaxNode
    {
        return node.WithLeadingTrivia(TriviaList([Comment(comment)]));
        //return node.WithLeadingTrivia(TriviaList([.. comment.Split('\n').Select(Comment)]));
    }

    public static BaseNamespaceDeclarationSyntax NamespaceDeclaration(string name)
    {
        return FileScopedNamespaceDeclaration(ParseName(name))
            .WithLeadingTrivia(TriviaList(
                Comment("// <auto-generated/>"),
                Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))));
    }

    public static async Task SaveNode(SyntaxNode node, DirectoryInfo dirInfo, string name)
    {
        var path = dirInfo.FullName + $"\\RotationSolver.SourceGenerators\\Resources\\{name}.txt";
        if (File.Exists(path))
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }
        await using var streamWriter = new StreamWriter(path, false);
        node.NormalizeWhitespace().WriteTo(streamWriter);
        File.SetAttributes(path, FileAttributes.ReadOnly);
    }
}
