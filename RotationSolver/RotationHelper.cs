using Dalamud.Interface.Colors;
using Dalamud.Logging;
using System.Diagnostics;
using System.Text;

namespace RotationSolver;

internal static class RotationHelper
{
    public static string[] AllowedAssembly { get; private set; } = new string[0];
    public static string[] DefaultAssembly { get; private set; } = new string[] 
    { 
        "RotationSolver.Default",
    };

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

            try
            {
                var bts = await client.GetByteArrayAsync("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/defaultList.json");
                DefaultAssembly = JsonConvert.DeserializeObject<string[]>(Encoding.Default.GetString(bts));
            }
            catch (Exception ex)
            {
                PluginLog.Log(ex, "Failed to load default List.");
            }
        }
    }

    public static bool IsDefault(this ICustomRotation rotation)
    {
        var type = rotation.GetType();
        if (!DefaultAssembly.Contains(type.Assembly.GetName().Name)) return false;
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

        return DefaultAssembly.Contains(name) || AllowedAssembly.Contains(name);
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
