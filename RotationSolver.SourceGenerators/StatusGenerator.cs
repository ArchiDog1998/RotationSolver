using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace RotationSolver.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class StatusGenerator : IIncrementalGenerator
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

    var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// 
            /// </summary>
            public enum StatusId : ushort
            {
                None = 0,
            {{Properties.Resources.StatusId.Table()}}
            }
            """;

        context.AddSource("StatusId.g.cs", code);
    }
}
