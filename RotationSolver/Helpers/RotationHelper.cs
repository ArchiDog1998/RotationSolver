using Dalamud.Interface.Colors;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.Interop;
using Lumina.Excel;
using Lumina.Excel.CustomSheets;

using RotationSolver.Updaters;

using System.Diagnostics;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Helpers;

internal record AssemblyInfo(string Name, string Author, string Path, string Support, string Help, string ChangeLog, string Donate);

internal class LoadedAssembly
{
    public string Path { get; set; }
    public string LastModified { get; set; }
}

internal static class RotationHelper
{
    public static List<LoadedAssembly> LoadedCustomRotations = new();

    public static readonly Dictionary<Assembly, AssemblyInfo> _assemblyInfos = new();

    public static string[] AllowedAssembly { get; private set; } = Array.Empty<string>();
    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
        if (_assemblyInfos.TryGetValue(assembly, out var value))
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
        => !rotation.IsValid ? ImGuiColors.DPSRed :
        !rotation.IsAllowed(out _) ? ImGuiColors.DalamudViolet : rotation.IsBeta()
        ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudWhite;

    public static bool IsBeta(this ICustomRotation rotation)
        => rotation.GetType().GetCustomAttribute<BetaRotationAttribute>() != null;

    public static string GetAuthor(string path, string name)
    {
        try
        {
            return FileVersionInfo.GetVersionInfo(path)?.CompanyName
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

        Assembly tempAsm = null;

        foreach (var asm in _assemblyInfos)
        {
            if (asm.Value.Path == filePath)
            {
                tempAsm = asm.Key;
            }
        }

        if (tempAsm != null)
        {
            _assemblyInfos.Remove(tempAsm);
        }

        var attr = assembly.GetCustomAttribute<AssemblyLinkAttribute>();
        _assemblyInfos[assembly] = new AssemblyInfo(name, GetAuthor(filePath, name), filePath, attr?.SupportLink, attr?.HelpLink, attr?.ChangeLog, attr?.Donate);


        var loaded = new LoadedAssembly();
        loaded.Path = filePath;
        loaded.LastModified = File.GetLastWriteTimeUtc(filePath).ToString();
        var idx = LoadedCustomRotations.FindIndex(item => item.Path == loaded.Path);
        if (idx != -1) LoadedCustomRotations.RemoveAt(idx);
        LoadedCustomRotations.Add(loaded);

        return assembly;
    }
}
