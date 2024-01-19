using Microsoft.CodeAnalysis;
using System.Text;

namespace RotationSolver.SourceGenerators;
internal static class Util
{
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
}
