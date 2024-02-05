using Microsoft.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace RotationSolver.SourceGenerators;
internal static class Util
{
    public static TS? GetParent<TS>(this SyntaxNode? node) where TS : SyntaxNode
    {
        if (node == null) return null;
        if (node is TS result) return result;
        return GetParent<TS>(node.Parent);
    }

    public static string GetFullMetadataName(this ISymbol s)
    {
        if (s == null || s is INamespaceSymbol)
        {
            return string.Empty;
        }

        while (s != null && s is not ITypeSymbol)
        {
            s = s.ContainingSymbol;
        }

        if (s == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.GetTypeSymbolName());

        s = s.ContainingSymbol;
        while (!IsRootNamespace(s))
        {
            try
            {
                sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + '.');
            }
            catch
            {
                break;
            }

            s = s.ContainingSymbol;
        }

        return sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            return symbol is INamespaceSymbol s && s.IsGlobalNamespace;
        }
    }

    private static string GetTypeSymbolName(this ISymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayTypeSymbol) //Array
        {
            return arrayTypeSymbol.ElementType.GetFullMetadataName() + "[]";
        }

        var str = symbol.MetadataName;
        if (symbol is INamedTypeSymbol symbolType)//Generic
        {
            var strs = str.Split('`');
            if (strs.Length < 2) return str;
            str = strs[0];

            str += "<" + string.Join(", ", symbolType.TypeArguments.Select(p => p.GetFullMetadataName())) + ">";
        }
        return str;
    }

    public static string Table(this string str) => "    " + str.Replace("\n", "\n    ");

    public static string ToPascalCase(this string input)
    {
        return string.Join(".", input.Split('.').Select(ConvertToPascalCase));

        static string ConvertToPascalCase(string input)
        {
            Regex invalidCharsRgx = new(@"[^_a-zA-Z0-9]");
            Regex whiteSpace = new(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(input, "_"), string.Empty)
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }
    }
}
