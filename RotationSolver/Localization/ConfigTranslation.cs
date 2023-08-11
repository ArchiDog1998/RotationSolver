using ECommons.Configuration;
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

    public static string ToCommand(this JobConfigInt config) => ToCommandStr(config);
    public static string ToCommand(this JobConfigFloat config) => ToCommandStr(config);
    public static string ToCommand(this PluginConfigInt config) => ToCommandStr(config);
    public static string ToCommand(this PluginConfigBool config) => ToCommandStr(config);
    public static string ToCommand(this PluginConfigFloat config)  => ToCommandStr(config);
    private static string ToCommandStr(object obj)
        => Service.Command + " " + OtherCommandType.Settings.ToString() + " " + obj.ToString();

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
        PluginConfigFloat.ActionAhead => new LinkDescription[]
        {
            new LinkDescription()
            {
                Path = "https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/HowAndWhenToClick.svg",
                Description = "This plugin helps you to use the right action during the combat. Here is a guide about the different options.",
            },
        },
        //PluginConfigFloat.MinLastAbilityAdvanced => new LinkDescription[]
        //{
        //},
        _ => null,
    };

    public static LinkDescription[] ToAction(this PluginConfigVector4 config) => config switch
    {
        _ => null,
    };
}
