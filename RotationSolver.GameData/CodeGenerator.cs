using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using RotationSolver.GameData.Getters;
using RotationSolver.GameData.Getters.Actions;
using System.Net;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData;

internal static class CodeGenerator
{
    public static async Task CreateCode(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        await GetDutyRotation(gameData, dirInfo);
        await GetBaseRotation(gameData, dirInfo);
        await GetRotations(gameData, dirInfo);
        await GetStatusID(gameData, dirInfo);
        await GetActionID(gameData, dirInfo);
        await GetContentType(gameData, dirInfo);
        await GetActionCategory(gameData, dirInfo);
        await GetOpCode(dirInfo);
    }

    private static async Task GetDutyRotation(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        var dutyRotationBase = new ActionDutyRotationGetter(gameData);
        var rotationNodes = dutyRotationBase.GetNodes();


        var type = ClassDeclaration("DutyRotation")
            .WithModifiers(
                TokenList(
                    [
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.AbstractKeyword),
                        Token(SyntaxKind.PartialKeyword)]))
            .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator))
            .WithXmlComment($$"""
             /// <summary>
             /// The Custom Rotation.
             /// <br>Number of Actions: {{dutyRotationBase.Count}}</br>
             /// </summary>
             """))
            .AddMembers(rotationNodes);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Rotations.Duties").AddMembers(type);

        await SaveNode(majorNameSpace, dirInfo, "DutyRotation");
    }
    private static async Task GetBaseRotation(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        var rotationBase = new ActionRoleRotationGetter(gameData);
        var rotationCodes = rotationBase.GetNodes();
        var rotationItems = new ItemGetter(gameData);
        var rotationItemCodes = rotationItems.GetNodes();

        var type = ClassDeclaration("CustomRotation")
            .WithModifiers(
                TokenList(
                    [
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AbstractKeyword),
                    Token(SyntaxKind.PartialKeyword)]))
            .WithXmlComment($$"""
             /// <summary>
             /// The Custom Rotation.
             /// <br>Number of Actions: {{rotationBase.Count}}</br>
             /// </summary>
             """)
            .AddMembers([.. rotationCodes, .. rotationItemCodes,
            .. GetArrayProperty("global::RotationSolver.Basic.Actions.IBaseAction", "AllBaseActions", [SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword], [.. rotationBase.AddedNames]),
            .. GetArrayProperty("global::RotationSolver.Basic.Actions.IBaseItem", "AllItems", [SyntaxKind.PublicKeyword], [.. rotationItems.AddedNames]),

            ]);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Rotations").AddMembers(type);

        await SaveNode(majorNameSpace, dirInfo, "CustomRotation");
    }


    private static async Task GetRotations(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        foreach (var job in gameData.GetExcelSheet<ClassJob>()!
            .Where(job => job.JobIndex > 0))
        {
            await GetRotation(gameData, job, dirInfo);
        }
    }

    private static async Task GetRotation(Lumina.GameData gameData, ClassJob job, DirectoryInfo dirInfo)
    {
        var className = (job.NameEnglish.RawString + " Rotation").ToPascalCase();
        var jobName = job.NameEnglish.RawString;

        var rotationsGetter = new ActionSingleRotationGetter(gameData, job);
        var traitsGetter = new TraitRotationGetter(gameData, job);

        List<MemberDeclarationSyntax> list = [.. rotationsGetter.GetNodes(), .. traitsGetter.GetNodes(),

        .. GetArrayProperty("global::RotationSolver.Basic.Traits.IBaseTrait", "AllTraits", [SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword], [.. traitsGetter.AddedNames])];

        if (!job.IsLimitedJob)
        {
            var jobgaugeName = IdentifierName($"global::Dalamud.Game.ClientState.JobGauge.Types.{job.Abbreviation}Gauge");
            var jobgauge = PropertyDeclaration(jobgaugeName, Identifier("JobGauge"))
                    .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.StaticKeyword)))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::ECommons.DalamudServices.Svc"),
                                        IdentifierName("Gauges")),
                                    GenericName(
                                        Identifier("Get"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                jobgaugeName)))))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            list.Add(jobgauge);
        }

        var rotationNames = rotationsGetter.AddedNames;
        if (job.LimitBreak1.Value is Lumina.Excel.GeneratedSheets.Action a
            && a.RowId != 0)
        {
            list.AddRange(GetLBInRotation(a, 1, gameData));
            rotationNames.Add("LimitBreak1");
        }
        if (job.LimitBreak2.Value is Lumina.Excel.GeneratedSheets.Action b
            && b.RowId != 0)
        {
            list.AddRange(GetLBInRotation(b, 2, gameData));
            rotationNames.Add("LimitBreak2");
        }
        if (job.LimitBreak3.Value is Lumina.Excel.GeneratedSheets.Action c
            && c.RowId != 0)
        {
            list.AddRange(GetLBInRotation(c, 3, gameData));
            rotationNames.Add("LimitBreak3");
        }

        list.AddRange(GetArrayProperty("global::RotationSolver.Basic.Actions.IBaseAction", "AllBaseActions", [SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword], [.. rotationNames]));

        var type = ClassDeclaration(className)
            .WithModifiers(
                TokenList([
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AbstractKeyword),
                    Token(SyntaxKind.PartialKeyword)]))
            .WithBaseList(
                BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(IdentifierName("global::RotationSolver.Basic.Rotations.CustomRotation")))))
            .WithXmlComment($$"""
            /// <summary>
            /// <see href="https://na.finalfantasyxiv.com/jobguide/{{jobName.Replace(" ", "").ToLower()}}"><strong>{{jobName}}</strong></see>
            /// <br>Number of Actions: {{rotationsGetter.Count}}</br>
            /// <br>Number of Traits: {{traitsGetter.Count}}</br>
            /// </summary>
            """)
            .WithAttributeLists(SingletonList(JobsAttribute(job)))
            .AddMembers([.. list]);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Rotations.Basic").AddMembers(type);

        await SaveNode(majorNameSpace, dirInfo, className);
    }

    private static MemberDeclarationSyntax[] GetLBInRotation(Lumina.Excel.GeneratedSheets.Action action, int index, Lumina.GameData gameData)
    {
        var declarations = GetLBPvE(action, out var name, gameData);

        var property = PropertyDeclaration(
            IdentifierName("global::RotationSolver.Basic.Actions.IBaseAction"),
            Identifier("LimitBreak" + index.ToString()))
        .WithModifiers(
            TokenList(
                [
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.SealedKeyword),
                    Token(SyntaxKind.ProtectedKeyword),
                    Token(SyntaxKind.OverrideKeyword)]))
        .WithExpressionBody(
            ArrowExpressionClause(
                IdentifierName(name)))
        .WithSemicolonToken(
            Token(SyntaxKind.SemicolonToken))
        .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)).WithXmlComment($"/// <inheritdoc cref=\"{name}\"/>"));

        return [.. declarations, property];
    }

    private static MemberDeclarationSyntax[] GetLBPvE(Lumina.Excel.GeneratedSheets.Action action, out string name, Lumina.GameData gameData)
    {
        name = action.Name.RawString.ToPascalCase() + $"PvE";
        var descName = action.GetDescName();

        return action.ToNodes(name, descName, action.GetDesc(gameData), false);
    }

    private static MemberDeclarationSyntax[] GetArrayProperty(string typeName, string propertyName, SyntaxKind[] keywords, string[] items)
    {
        var fieldName = "_" + propertyName;
        return [GetArrayField(typeName, fieldName), m_GetArrayProperty(typeName, propertyName, fieldName, keywords, items)];

        static FieldDeclarationSyntax GetArrayField(string typeName, string fieldName)
        {
            return FieldDeclaration(
                VariableDeclaration(
                    NullableType(
                        ArrayType(
                            IdentifierName(typeName))
                        .WithRankSpecifiers(
                            SingletonList(
                                ArrayRankSpecifier(
                                    SingletonSeparatedList<ExpressionSyntax>(
                                        OmittedArraySizeExpression()))))))
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(
                            Identifier(fieldName))
                        .WithInitializer(
                            EqualsValueClause(
                                LiteralExpression(
                                    SyntaxKind.NullLiteralExpression))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PrivateKeyword)))
            .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)));
        }

        static PropertyDeclarationSyntax m_GetArrayProperty(string typeName, string propertyName, string fieldName, SyntaxKind[] keywords, string[] items)
        {
            var tokens = new List<SyntaxNodeOrToken>();
            foreach (var item in items)
            {
                if (tokens.Count > 0)
                {
                    tokens.Add(Token(SyntaxKind.CommaToken));
                }
                tokens.Add(ExpressionElement(IdentifierName(item)));
            }

            if (keywords.Contains(SyntaxKind.OverrideKeyword))
            {
                if (tokens.Count > 0)
                {
                    tokens.Add(Token(SyntaxKind.CommaToken));
                }
                tokens.Add(SpreadElement(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    BaseExpression(),
                    IdentifierName(propertyName))));
            }

            return PropertyDeclaration(
                ArrayType(
                    IdentifierName(typeName))
                .WithRankSpecifiers(
                    SingletonList(
                        ArrayRankSpecifier(
                            SingletonSeparatedList<ExpressionSyntax>(
                                OmittedArraySizeExpression())))),
                Identifier(propertyName))
            .WithModifiers(TokenList(keywords.Select(Token).ToArray()))
            .WithExpressionBody(
                ArrowExpressionClause(
                    AssignmentExpression(
                        SyntaxKind.CoalesceAssignmentExpression,
                        IdentifierName(fieldName),
                        CollectionExpression(
                            SeparatedList<CollectionElementSyntax>(tokens)))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            .WithXmlComment("/// <inheritdoc/>")
            .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)));
        }
    }

    private static AttributeListSyntax JobsAttribute(ClassJob job)
    {
        var list = new List<SyntaxNodeOrToken>()
        {
            GetJobArgument(job.Abbreviation)
        };

        if (job.RowId != 28 && job.RowId != job.ClassJobParent.Row)
        {
            list.Add(Token(SyntaxKind.CommaToken));
            list.Add(GetJobArgument(job.ClassJobParent.Value?.Abbreviation ?? "ADV"));
        }

        return AttributeList(
           SingletonSeparatedList(
               Attribute(IdentifierName("global::RotationSolver.Basic.Attributes.Jobs"))
               .WithArgumentList(
                   AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(list)))));

        static AttributeArgumentSyntax GetJobArgument(string name)
        {
            return AttributeArgument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                   IdentifierName("global::ECommons.ExcelServices.Job"), IdentifierName(name)));
        }
    }
    private static async Task GetContentType(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        await GetEnums(gameData, dirInfo, new ContentTypeGetter(gameData).GetNodes(), "TerritoryContentType", "The TerritoryContentType", SyntaxKind.ByteKeyword);

    }
    private static async Task GetActionCategory(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        await GetEnums(gameData, dirInfo, new ActionCategoryGetter(gameData).GetNodes(), "ActionCate", "The ActionCate", SyntaxKind.ByteKeyword);

    }
    private static async Task GetActionID(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        await GetEnums(gameData, dirInfo, new ActionIdGetter(gameData).GetNodes(), "ActionID", "The id of the action", SyntaxKind.UIntKeyword);
    }

    private static async Task GetStatusID(Lumina.GameData gameData, DirectoryInfo dirInfo)
    {
        await GetEnums(gameData, dirInfo, new StatusGetter(gameData).GetNodes(), "StatusID", "The id of the status", SyntaxKind.UShortKeyword);
    }

    private static async Task GetEnums(Lumina.GameData gameData, DirectoryInfo dirInfo, EnumMemberDeclarationSyntax[] members,
        string name, string description, SyntaxKind type)
    {
        var enumDefinition = EnumDeclaration(name)
            .AddBaseListTypes(SimpleBaseType(PredefinedType(Token(type))))
            .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)).WithXmlComment($"/// <summary> {description} </summary>"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers([EnumMember("None", 0).WithXmlComment($"/// <summary/>"), .. members]);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Data").AddMembers(enumDefinition);

        await SaveNode(majorNameSpace, dirInfo, name);
    }

    private static async Task GetOpCode(DirectoryInfo dirInfo)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        using var result = await client.GetAsync("https://raw.githubusercontent.com/karashiiro/FFXIVOpcodes/master/opcodes.json");

        if (result.StatusCode != HttpStatusCode.OK) return;
        var responseStream = await result.Content.ReadAsStringAsync();

        var enums = JToken.Parse(responseStream)[0]!["lists"]!.Children()
            .SelectMany(i => i.Children()).SelectMany(i => i.Children()).Cast<JObject>()
            .Select(i =>
            {
                var name = (string)((JValue)i["name"]!).Value!;
                var value = (ushort)(long)((JValue)i["opcode"]!).Value!;
                var description = name!.Space();

                var desc = AttributeList(SingletonSeparatedList(DescriptionAttribute(description))).WithXmlComment($"/// <summary> {description} </summary>");

                return EnumMember(name, value).AddAttributeLists(desc);
            });

        var enumDefinition = EnumDeclaration("OpCode")
            .AddBaseListTypes(SimpleBaseType(PredefinedType(Token(SyntaxKind.UShortKeyword))))
            .AddAttributeLists(GeneratedCodeAttribute(typeof(CodeGenerator)).WithXmlComment($"/// <summary> The OpCode </summary>"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers([EnumMember("None", 0).WithXmlComment($"/// <summary/>"), ..enums]);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Data").AddMembers(enumDefinition);

        await SaveNode(majorNameSpace, dirInfo, "OpCode");
    }
}
