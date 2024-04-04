using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        context.RegisterSourceOutput(compilation, (spc, source) => Execute(spc));
    }

    private static void Execute(SourceProductionContext context)
    {
        var assembly = typeof(StaticCodeGenerator).Assembly;
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(".txt")) continue;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var streamReader = new StreamReader(stream);
            var name = resourceName.Split('.').Reverse().ElementAt(1);
            var code = streamReader.ReadToEnd();
            context.AddSource(name + ".g.cs", code);
        }
    }
}
