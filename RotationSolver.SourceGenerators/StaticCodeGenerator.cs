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
        GenerateStatus(context);
        GenerateActionID(context);
        GenerateContentType(context);
        GenerateActionCate(context);
        GenerateBaseRotation(context);
        GenerateRotations(context);
        GenerateOpCode(context);
    }

    private static void GenerateOpCode(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// The opcode
            /// </summary>
            public enum OpCode : ushort
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.OpCode.Table()}}
            }
            """;

        context.AddSource("OpCode.g.cs", code);
    }

    private static void GenerateStatus(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// The id of the status.
            /// </summary>
            public enum StatusID : ushort
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.StatusId.Table()}}
            }
            """;

        context.AddSource("StatusID.g.cs", code);
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

    private static void GenerateActionCate(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// 
            /// </summary>
            public enum ActionCate : byte
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.ActionCategory.Table()}}
            }
            """;

        context.AddSource("ActionCate.g.cs", code);
    }

    private static void GenerateActionID(SourceProductionContext context)
    {
        var code = $$"""
            namespace RotationSolver.Basic.Data;

            /// <summary>
            /// The id of the status.
            /// </summary>
            public enum ActionID : uint
            {
                /// <summary>
                /// 
                /// </summary>
                None = 0,
            {{Properties.Resources.ActionId.Table()}}
            }
            """;

        context.AddSource("ActionID.g.cs", code);
    }

    private static void GenerateBaseRotation(SourceProductionContext context)
    {
        context.AddSource("CustomRotation.g.cs", Properties.Resources.Action);
        context.AddSource("DutyRotation.g.cs", Properties.Resources.DutyAction);
    }

    private static void GenerateRotations(SourceProductionContext context)
    {
        context.AddSource($"BaseRotations.g.cs", Properties.Resources.Rotation);
    }
}
