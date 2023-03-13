using RotationSolver.Basic.Rotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver;

internal static class RotationHelper
{
    const string DefaultAssembly = "RotationSolver.Default";
    static readonly string[] _allowedAssembly = new string[]
    {
        DefaultAssembly,
    };

    public static bool IsDefault(this ICustomRotation rotation)
        => DefaultAssembly == rotation.GetType().Assembly.GetName().Name;
    
    public static bool IsAllowed(this ICustomRotation rotation, out string name)
    {
        name = rotation.GetType().Assembly.GetName().Name;
        return _allowedAssembly.Contains(rotation.GetType().Assembly.GetName().Name);
    }

    public static string GetAuthor(this ICustomRotation rotation)
    {
        return FileVersionInfo.GetVersionInfo(rotation.GetType().Assembly.Location)?.CompanyName ?? "Unnamed";
    }
}
