using XIVAutoAction.Localization;

namespace XIVAutoAction.Combos.CustomCombo;

public enum DescType : byte
{
    Description,
    BreakingAction,
    HealArea,
    HealSingle,
    DefenseArea,
    DefenseSingle,
    MoveAction,
    OtherCommands,
}

internal static class DescTypeExtension
{
    public static string ToName(this DescType type) => type switch
    {
        DescType.Description => LocalizationManager.RightLang.DescType_Description,
        DescType.BreakingAction => LocalizationManager.RightLang.DescType_BreakingAction,
        DescType.HealArea => LocalizationManager.RightLang.DescType_HealArea,
        DescType.HealSingle => LocalizationManager.RightLang.DescType_HealSingle,
        DescType.DefenseArea => LocalizationManager.RightLang.DescType_DefenseArea,
        DescType.DefenseSingle => LocalizationManager.RightLang.DescType_DefenseSingle,
        DescType.MoveAction => LocalizationManager.RightLang.DescType_MoveAction,
        _ => string.Empty,
    };
}
