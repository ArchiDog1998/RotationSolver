using Dalamud.Interface.Colors;
using Dalamud.Logging;
using RotationSolver.Data;
using RotationSolver.Localization;
using System.Diagnostics;

namespace RotationSolver.Helpers;

internal static class RotationHelper
{
    private static readonly Dictionary<Assembly, AssemblyInfo> _assemblyInfos = new();

    public static List<LoadedAssembly> LoadedCustomRotations { get; } = new List<LoadedAssembly>();

    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
        if (_assemblyInfos.TryGetValue(assembly, out var info))
        {
            return info;
        }

        var name = assembly.GetName().Name;
        var location = assembly.Location;
        var version = assembly.GetName().Version?.ToString();
        var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        var trademark = assembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark;

        var assemblyInfo = new AssemblyInfo(name, version, location, description, company, product, trademark);

        _assemblyInfos[assembly] = assemblyInfo;

        return assemblyInfo;
    }

    //public static Assembly GetTypeAssembly(this ICustomRotation rotation)
    //{
    //    try
    //    {
    //        return rotation.GetType().Assembly;
    //    }
    //    catch (Exception ex)
    //    {
    //        PluginLog.LogError($"Failed to get assembly for rotation {rotation.GetType().Name}: {ex}");
    //        return null;
    //    }
    //}

    public static Vector4 GetColor(this ICustomRotation rotation)
    {
        if (!rotation.IsValid)
        {
            return ImGuiColors.DPSRed;
        }

        if (rotation.IsBeta())
        {
            return ImGuiColors.DalamudOrange;
        }

        return ImGuiColors.DalamudWhite;
    }

    public static bool IsBeta(this ICustomRotation rotation)
    {
        var betaAttribute = rotation.GetType().GetCustomAttribute<BetaRotationAttribute>();
        return betaAttribute != null;
    }

    public static Assembly LoadCustomRotationAssembly(string filePath)
    {
        var directoryInfo = new FileInfo(filePath).Directory;
        var loadContext = new RotationLoadContext(directoryInfo);
        var assembly = loadContext.LoadFromFile(filePath);

        var assemblyName = assembly.GetName().Name;
        var author = GetAuthor(filePath, assemblyName);

        var attr = assembly.GetCustomAttribute<AssemblyLinkAttribute>();
        var assemblyInfo = new AssemblyInfo(
            assemblyName,
            author,
            filePath,
            attr?.SupportLink,
            attr?.HelpLink,
            attr?.ChangeLog,
            attr?.Donate);

        var existingAssembly = GetAssemblyFromPath(filePath);
        if (existingAssembly != null)
        {
            _assemblyInfos.Remove(existingAssembly);
        }

        _assemblyInfos[assembly] = assemblyInfo;

        var loadedAssembly = new LoadedAssembly(
            filePath,
            File.GetLastWriteTimeUtc(filePath).ToString());

        LoadedCustomRotations.RemoveAll(item => item.FilePath == loadedAssembly.FilePath);
        LoadedCustomRotations.Add(loadedAssembly);

        return assembly;
    }

    private static Assembly GetAssemblyFromPath(string filePath)
    {
        foreach (var asm in _assemblyInfos)
        {
            if (asm.Value.FilePath == filePath)
            {
                return asm.Key;
            }
        }
        return null;
    }

    private static string GetAuthor(string filePath, string assemblyName)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);
        return string.IsNullOrWhiteSpace(fileVersionInfo.CompanyName) ? assemblyName : fileVersionInfo.CompanyName;
    }
}
