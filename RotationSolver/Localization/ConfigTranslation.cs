using RotationSolver.Basic.Configuration;

namespace RotationSolver.Localization;

internal static class ConfigTranslation
{
    public static string ToName(this JobConfigString config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this JobConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this JobConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigBool config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigVector4 config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this JobConfigString config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this JobConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this JobConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigBool config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigVector4 config) => config switch
    {
        _ => string.Empty,
    };
}
