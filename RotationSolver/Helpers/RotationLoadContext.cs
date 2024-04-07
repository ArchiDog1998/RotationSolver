using Dalamud.Plugin;
using ECommons.DalamudServices;
using FFXIVClientStructs.Interop;
using Lumina.Excel;
using Lumina.Excel.CustomSheets;
using System.Runtime.Loader;

namespace RotationSolver.Helpers;

internal class RotationLoadContext(DirectoryInfo? directoryInfo) 
    : AssemblyLoadContext(true)
{
    readonly DirectoryInfo? _directory = directoryInfo;

    private static readonly Dictionary<string, Assembly> _handledAssemblies = [];

    static RotationLoadContext()
    {
        var assemblies = new Assembly[]
        {
            typeof(RotationSolverPlugin).Assembly,
            typeof(Resolver).Assembly,
            typeof(DalamudPluginInterface).Assembly,
            typeof(DataCenter).Assembly,
            typeof(SheetAttribute).Assembly,
            typeof(QuestDialogueText).Assembly,
            typeof(XIVConfigUI.XIVConfigUIMain).Assembly,
        };

        foreach (var assembly in assemblies)
        {
            _handledAssemblies.Add(assembly.GetName().Name ?? string.Empty, assembly);
        }
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name != null && _handledAssemblies.TryGetValue(assemblyName.Name, out var value))
        {
            return value;
        }

        var file = Path.Join(_directory?.FullName ?? string.Empty, $"{assemblyName.Name}.dll");
        if (File.Exists(file))
        {
            try
            {
                return LoadFromFile(file);
            }
            catch
            {
                //
            }
        }
        return base.Load(assemblyName);
    }

    internal Assembly LoadFromFile(string filePath)
    {
        using var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var pdbPath = Path.ChangeExtension(filePath, ".pdb");
        if (!File.Exists(pdbPath))
        {
            Svc.Log.Information($"Failed to find {pdbPath}");
            return LoadFromStream(file);
        }
        using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            return LoadFromStream(file, pdbFile);
        }
        catch
        {
            return LoadFromStream(file);
        }
    }
}
