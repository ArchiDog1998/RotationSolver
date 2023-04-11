using Dalamud.Interface.Colors;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.Interop;
using Lumina.Excel;
using RotationSolver.Updaters;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Text;

namespace RotationSolver;

internal static class RotationHelper
{
    private class RotationLoadContext : AssemblyLoadContext
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

        internal Assembly LoadFromFile(string filePath)
        {
            using var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pdbPath = Path.ChangeExtension(filePath, ".pdb");
            if (!File.Exists(pdbPath)) return LoadFromStream(file);
            using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var assembly = LoadFromStream(file, pdbFile);
            return assembly;
        }


    }

    public static Dictionary<string, string> AssemblyPaths = new Dictionary<string, string>();

    public static string[] AllowedAssembly { get; private set; } = new string[0];
    static readonly SortedList<Assembly, string> _authors = new SortedList<Assembly, string>();
    public static async void LoadList()
    {
        using (var client = new HttpClient())
        {
            try
            {
                var bts = await client.GetByteArrayAsync("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/whitelist.json");
                AllowedAssembly = JsonConvert.DeserializeObject<string[]>(Encoding.Default.GetString(bts));
            }
            catch (Exception ex)
            {
                PluginLog.Log(ex, "Failed to load white List.");
            }
        }
    }
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        if (rotation == null)
        {
            name = "Unknown";
            return false;
        }
        var assembly = rotation.GetType().Assembly;
        name = assembly.GetName().Name;

        return AllowedAssembly.Contains(name + " - " + assembly.GetAuthor());
    }

    public static Vector4 GetColor(this ICustomRotation rotation)
        => !rotation.IsAllowed(out _) ? ImGuiColors.DalamudViolet : rotation.IsBeta() 
        ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudWhite ;

    public static bool IsBeta(this ICustomRotation rotation)
        => rotation.GetType().GetCustomAttribute<BetaRotationAttribute>() != null;

    public static string GetAuthor(this Assembly assembly)
    {
        if (_authors.TryGetValue(assembly, out var author)) return author;
        try
        {
            var name = assembly.GetName().Name;
            return _authors[assembly] = 
                (AssemblyPaths.TryGetValue(name, out var path) 
                ? FileVersionInfo.GetVersionInfo(path)?.CompanyName : name)
                ?? name ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    public static Assembly LoadFrom(string filePath)
    {
        var loadContext = new RotationLoadContext(new FileInfo(filePath).Directory);
        var assembly = loadContext.LoadFromFile(filePath);
        AssemblyPaths[assembly.GetName().Name] = filePath;
        return assembly;
    }
}
