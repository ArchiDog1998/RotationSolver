using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace RotationSolver.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class StaticCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => s is ClassDeclarationSyntax,
            (c, _) => (ClassDeclarationSyntax)c.Node).Where(i => i is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, (spc, source) => Execute(spc, source.Left, source.Right));
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> typeList)
    {
        GenerateStatus(context);
        GenerateContentType(context);
    }

    private static void GenerateStatus(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// The id of the status.
            /// </summary>
            public enum StatusId : ushort
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.StatusId.Table()}}
            }
            """;

        context.AddSource("StatusId.g.cs", code);
    }

    private static void GenerateContentType(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// 
            /// </summary>
            public enum TerritoryContentType : byte
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.ContentType.Table()}}
            }
            """;

        context.AddSource("TerritoryContentType.g.cs", code);
    }
}
