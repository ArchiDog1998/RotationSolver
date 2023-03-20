using Dalamud.Interface.Colors;
using RotationSolver.Basic.Rotations;
using System.Diagnostics;
using System.Numerics;

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
        => DefaultAssembly == GetId(rotation);
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        if (rotation == null)
        {
            name = "Unknown";
            return false;
        }
        name = rotation.GetType().Assembly.GetName().Name;
        return _allowedAssembly.Contains(GetId(rotation));
    }

    internal static string GetId(this ICustomRotation rotation)
    {
        return Convert.ToBase64String(rotation.GetType().Assembly.GetName().GetPublicKeyToken());
    }

    public static Vector4 GetColor(this ICustomRotation rotation)
        => rotation.IsAllowed(out _) ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudViolet;

    public static string GetAuthor(this ICustomRotation rotation)
    {
        try
        {
            return FileVersionInfo.GetVersionInfo(rotation.GetType().Assembly.Location)?.CompanyName ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}
