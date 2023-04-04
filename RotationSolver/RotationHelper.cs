using Dalamud.Interface.Colors;
using RotationSolver.Basic.Rotations;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;

namespace RotationSolver;

internal static class RotationHelper
{
    const string DefaultAssembly = "RotationSolver.Default";
    static readonly string[] _allowedAssembly = new string[]
    {
        DefaultAssembly,
        "RotationSolver.Old",
        //"RotationSolver.Extra",
    };

    public static bool IsDefault(this ICustomRotation rotation)
        => DefaultAssembly == rotation.GetType().Assembly.GetName().Name;
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        if (rotation == null)
        {
            name = "Unknown";
            return false;
        }
        name = rotation.GetType().Assembly.GetName().Name;
        return _allowedAssembly.Contains(name);
    }

    public static Vector4 GetColor(this ICustomRotation rotation)
        => rotation.IsAllowed(out _) ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudViolet;

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
