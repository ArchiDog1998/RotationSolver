using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static RotationSolver.GameData.SyntaxHelper;

namespace RotationSolver.GameData;
internal static class OpCodeGetter
{
    public static async Task GetOpCode(DirectoryInfo dirInfo)
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
            .AddAttributeLists(GeneratedCodeAttribute(typeof(OpCodeGetter)).WithXmlComment($"/// <summary> The OpCode </summary>"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers([EnumMember("None", 0).WithXmlComment($"/// <summary/>")
                , ..enums]);

        var majorNameSpace = NamespaceDeclaration("RotationSolver.Basic.Data").AddMembers(enumDefinition);

        await SaveNode(majorNameSpace, dirInfo, "OpCode");
    }
}
