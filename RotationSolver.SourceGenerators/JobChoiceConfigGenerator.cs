﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace RotationSolver.SourceGenerators;

[Generator(LanguageNames.CSharp)]

public class JobChoiceConfigGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName
            ("RotationSolver.Basic.Attributes.JobChoiceConfigAttribute",
            static (node, _) => node is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Parent: FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or StructDeclarationSyntax } } },
            static (n, ct) => ((VariableDeclaratorSyntax)n.TargetNode, n.SemanticModel))
            .Where(m => m.Item1 != null);
        context.RegisterSourceOutput(provider.Collect(), Execute);
    }

    private void Execute(SourceProductionContext context, ImmutableArray<(VariableDeclaratorSyntax, SemanticModel SemanticModel)> array)
    {
        var typeGrps = array.GroupBy(variable => variable.Item1.Parent!.Parent!.Parent!);

        foreach (var grp in typeGrps)
        {
            var type = (TypeDeclarationSyntax)grp.Key;

            var nameSpace = type.GetParent<BaseNamespaceDeclarationSyntax>()?.Name.ToString() ?? "Null";

            var classType = type is ClassDeclarationSyntax ? "class" : "struct";

            var className = type.Identifier.Text;

            var propertyCodes = new List<string>();
            foreach (var (variableInfo, model) in grp)
            {
                var typeSymbol = model.GetDeclaredSymbol(type) as ITypeSymbol;

                var field = (FieldDeclarationSyntax)variableInfo.Parent!.Parent!;

                var variableName = variableInfo.Identifier.ToString();
                var propertyName = variableName.ToPascalCase();

                if (variableName == propertyName)
                {
                    //context.DiagnosticWarning(variableInfo.Identifier.GetLocation(),
                    //    "Please don't use Pascal Case to name your field!");
                    continue;
                }

                var fieldTypeStr = field.Declaration.Type;
                var fieldType = model.GetTypeInfo(fieldTypeStr).Type!;
                var fieldStr = fieldType.GetFullMetadataName();

                var names = new List<string>();
                foreach (var attrSet in field.AttributeLists)
                {
                    if (attrSet == null) continue;
                    foreach (var attr in attrSet.Attributes)
                    {
                        if (model.GetSymbolInfo(attr).Symbol?.GetFullMetadataName()
                            is "RotationSolver.Basic.Attributes.UIAttribute"
                            or "RotationSolver.Basic.Attributes.UnitAttribute"
                            or "RotationSolver.Basic.Attributes.RangeAttribute"
                            or "RotationSolver.Basic.Attributes.JobChoiceConfigAttribute"
                            or "RotationSolver.Basic.Attributes.LinkDescriptionAttribute")
                        {
                            names.Add(attr.ToString());
                        }
                    }
                }

                var attributeStr = names.Count == 0 ? "" : $"[{string.Join(", ", names)}]";
                var propertyCode = $$"""
                        [JsonProperty]
                        private Dictionary<Job, Dictionary<string, {{fieldStr}}>> {{variableName}}Dict = [];

                        [JsonIgnore]
                        {{attributeStr}}
                        public {{fieldStr}} {{propertyName}}
                        {
                            get
                            {
                                if (!{{variableName}}Dict.TryGetValue(DataCenter.Job, out var dict))
                                {
                                    dict = {{variableName}}Dict[DataCenter.Job] = new();
                                }
                                
                                if (!dict.TryGetValue(RotationChoice, out var value))
                                {
                                    value = dict[RotationChoice] = {{variableName}};
                                }

                                return value;
                            }
                            set
                            {
                                {{variableName}}Dict[DataCenter.Job][RotationChoice] = value;
                            }
                        }
                """;

                propertyCodes.Add(propertyCode);
            }

            if (propertyCodes.Count == 0) continue;

            var code = $$"""
             using ECommons.ExcelServices;

             namespace {{nameSpace}}
             {
                 partial {{classType}} {{className}}
                 {

             {{string.Join("\n \n", propertyCodes)}}

                 }
             }
             """;

            context.AddSource($"{nameSpace}_{className}.g.cs", code);
        }
    }

}
