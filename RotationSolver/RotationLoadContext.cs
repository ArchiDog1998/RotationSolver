using Dalamud.Plugin;
using FFXIVClientStructs.Interop;
using Lumina.Excel;
using System.Runtime.Loader;

namespace RotationSolver;

public class RotationLoadContext : AssemblyLoadContext
{
    DirectoryInfo _directory;

    static Dictionary<string, Assembly> _handledAssemblies;
    public RotationLoadContext(DirectoryInfo directoryInfo) : base(true)
    {
        _directory = directoryInfo;
    }

    static RotationLoadContext()
    {
        _handledAssemblies = new Dictionary<string, Assembly>()
        {
            ["RotationSolver"] = typeof(RotationSolverPlugin).Assembly,
            ["FFXIVClientStructs"] = typeof(Resolver).Assembly,
            ["Dalamud"] = typeof(DalamudPluginInterface).Assembly,
            ["RotationSolver.Basic"] = typeof(DataCenter).Assembly,
            ["Lumina"] = typeof(SheetAttribute).Assembly,
            ["Lumina.Excel"] = typeof(ExcelRow).Assembly,
        };
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name != null && _handledAssemblies.ContainsKey(assemblyName.Name))
        {
            return _handledAssemblies[assemblyName.Name];
        }

        var file = Path.Join(_directory.FullName, $"{assemblyName.Name}.dll");
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

    private Assembly LoadFromFile(string filePath)
    {
        using var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var pdbPath = Path.ChangeExtension(filePath, ".pdb");
        if (!File.Exists(pdbPath)) return LoadFromStream(file);
        using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return LoadFromStream(file, pdbFile);
    }

    public static Assembly LoadFrom(string filePath)
    {
        var loadContext = new RotationLoadContext(new FileInfo(filePath).Directory);
        return loadContext.LoadFromFile(filePath);
    }
}
