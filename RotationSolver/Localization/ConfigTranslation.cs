using RotationSolver.Basic.Configuration;

namespace RotationSolver.Localization;

internal static class ConfigTranslation
{
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
        PluginConfigFloat.ActionAhead => LocalizationManager.RightLang.ConfigWindow_Param_ActionAhead,
        PluginConfigFloat.MinLastAbilityAdvanced => LocalizationManager.RightLang.ConfigWindow_Param_MinLastAbilityAdvanced,
        PluginConfigFloat.CountDownAhead => LocalizationManager.RightLang.ConfigWindow_Param_CountDownAhead,
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigVector4 config) => config switch
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

    public static string ToCommand(this JobConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this JobConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this PluginConfigInt config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this PluginConfigBool config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this PluginConfigFloat config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this PluginConfigVector4 config) => config switch
    {
        _ => string.Empty,
    };

    public static LinkDescription[] ToAction(this JobConfigInt config) => config switch
    {
        _ => null,
    };

    public static LinkDescription[] ToAction(this JobConfigFloat config) => config switch
    {
        _ => null,
    };

    public static LinkDescription[] ToAction(this PluginConfigInt config) => config switch
    {
        _ => null,
    };

    public static LinkDescription[] ToAction(this PluginConfigBool config) => config switch
    {
        _ => null,
    };

    public static LinkDescription[] ToAction(this PluginConfigFloat config) => config switch
    {
        _ => null,
    };

    public static LinkDescription[] ToAction(this PluginConfigVector4 config) => config switch
    {
        _ => null,
    };
}
