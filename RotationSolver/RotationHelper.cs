using Dalamud.Interface.Colors;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.Interop;
using Lumina.Excel;
using Lumina.Excel.CustomSheets;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Text;

namespace RotationSolver;

internal record AssemblyInfo(string Name, string Author, string Path, string Support, string Help, string ChangeLog, string Donate);

internal static class RotationHelper
{
    private class RotationLoadContext : AssemblyLoadContext
    {
        readonly DirectoryInfo _directory;

        static readonly Dictionary<string, Assembly> _handledAssemblies;
        public RotationLoadContext(DirectoryInfo directoryInfo) : base(true)
        {
            _directory = directoryInfo;
        }

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
            };

            _handledAssemblies = new Dictionary<string, Assembly>();

            foreach (var assembly in assemblies)
            {
                _handledAssemblies.Add(assembly.GetName().Name, assembly);
            }
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name != null && _handledAssemblies.TryGetValue(assemblyName.Name, out Assembly value))
            {
                return value;
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
            if (!File.Exists(pdbPath))
            {
                PluginLog.Information($"Failed to load{pdbPath}");
                return LoadFromStream(file);
            }
            using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var assembly = LoadFromStream(file, pdbFile);
            return assembly;
        }
    }

    public static readonly Dictionary<Assembly, AssemblyInfo> _assemblyInfos = new();

    public static string[] AllowedAssembly { get; private set; } = Array.Empty<string>();
    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
        if(_assemblyInfos.TryGetValue(assembly, out var value))
        {
            return value;
        }
        return _assemblyInfos[assembly] = new AssemblyInfo(assembly.GetName().Name, "Unknown", assembly.Location, null, null, null, null);
    }
    public static async void LoadList()
    {
        using var client = new HttpClient();
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
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        name = "Unknown";
        if (rotation == null)
        {
            return false;
        }
        var assembly = rotation.GetType().Assembly;
        name = assembly.GetName().Name;

        return assembly.IsAllowed();
    }

    public static bool IsAllowed(this Assembly assembly)
    {
        if (_assemblyInfos.TryGetValue(assembly, out var info))
        {
            return AllowedAssembly.Contains(info.Name + " - " + info.Author);
        }
        return false;
    }

    public static Vector4 GetColor(this ICustomRotation rotation)
        => ! rotation.IsValid ? ImGuiColors.DPSRed :
        !rotation.IsAllowed(out _) ? ImGuiColors.DalamudViolet : rotation.IsBeta() 
        ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudWhite ;

    public static bool IsBeta(this ICustomRotation rotation)
        => rotation.GetType().GetCustomAttribute<BetaRotationAttribute>() != null;

    public static string GetAuthor(string path, string name)
    {
        try
        {
            return  FileVersionInfo.GetVersionInfo(path)?.CompanyName 
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

        var name = assembly.GetName().Name;

        var attr = assembly.GetCustomAttribute<AssemblyLinkAttribute>();
        _assemblyInfos[assembly] = new AssemblyInfo(name, GetAuthor(filePath, name), filePath, attr?.SupportLink, attr?.HelpLink, attr?.ChangeLog, attr?.Donate);
        return assembly;
    }
}
