using Dalamud.Interface.Colors;
using System.Diagnostics;
using System.Net;

namespace RotationSolver;

internal static class RotationHelper
{
    const string DefaultAssembly = "RotationSolver.Default";
    public static string[] AllowedAssembly { get; set; } = new string[0];

    public static bool IsDefault(this ICustomRotation rotation)
    {
        var type = rotation.GetType();
        if (DefaultAssembly != type.Assembly.GetName().Name) return false;
        return type.Name.Contains("Default", StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        if (rotation == null)
        {
            name = "Unknown";
            return false;
        }
        name = rotation.GetType().Assembly.GetName().Name;

        return name == DefaultAssembly || AllowedAssembly.Contains(name);
    }

    public static Vector4 GetColor(this ICustomRotation rotation)
        => !rotation.IsAllowed(out _) ? ImGuiColors.DalamudViolet : rotation.IsBeta() 
        ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudWhite ;

    public static bool IsBeta(this ICustomRotation rotation)
        => rotation.GetType().GetCustomAttribute<BetaRotationAttribute>() != null;


    public static string GetAuthor(this ICustomRotation rotation)
        => rotation.GetType().Assembly.GetAuthor();

    public static string GetAuthor(this Assembly assembly)
    {
        try
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location)?.CompanyName
                ?? assembly.GetName().Name
                ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}
