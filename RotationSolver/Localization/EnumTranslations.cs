using RotationSolver.Data;

namespace RotationSolver.Localization;

internal static class EnumTranslations
{
    internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.EndSpecial => type.ToSpecialString(role),
        _ => UiString.SpecialCommandType_Start.Local() + type.ToSpecialString(role),
    };

    internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Off => UiString.SpecialCommandType_Cancel.Local(),
        _ => type.ToStateString(role),
    };

    internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => UiString.SpecialCommandType_HealArea.Local(),
        SpecialCommandType.HealSingle => UiString.SpecialCommandType_HealSingle.Local(),
        SpecialCommandType.DefenseArea => UiString.SpecialCommandType_DefenseArea.Local(),
        SpecialCommandType.DefenseSingle => UiString.SpecialCommandType_DefenseSingle.Local(),
        SpecialCommandType.DispelStancePositional => role switch
        {
            JobRole.Tank => UiString.SpecialCommandType_TankStance.Local(),
            JobRole.Healer => UiString.SpecialCommandType_Dispel.Local(),
            JobRole.Melee => UiString.SpecialCommandType_Positional.Local(),
            _ => nameof(SpecialCommandType.DispelStancePositional),
        },
        SpecialCommandType.RaiseShirk => role switch
        {
            JobRole.Tank => UiString.SpecialCommandType_Shirk.Local(),
            JobRole.Healer => UiString.SpecialCommandType_Raise.Local(),
            _ => nameof(SpecialCommandType.RaiseShirk),
        },
        SpecialCommandType.MoveForward => UiString.SpecialCommandType_MoveForward.Local(),
        SpecialCommandType.MoveBack => UiString.SpecialCommandType_MoveBack.Local(),
        SpecialCommandType.AntiKnockback => UiString.SpecialCommandType_AntiKnockback.Local(),
        SpecialCommandType.Burst => UiString.SpecialCommandType_Burst.Local(),
        SpecialCommandType.EndSpecial => UiString.SpecialCommandType_EndSpecial.Local(),
        SpecialCommandType.Speed => UiString.SpecialCommandType_Speed.Local(),
        SpecialCommandType.LimitBreak => UiString.SpecialCommandType_LimitBreak.Local(),
        SpecialCommandType.NoCasting => UiString.SpecialCommandType_NoCasting.Local(),
        _ => string.Empty,
    };

    internal static string ToStateString(this StateCommandType type, JobRole _) => type switch
    {
        StateCommandType.Auto => UiString.SpecialCommandType_Smart.Local() + DataCenter.TargetingType.Local(),
        StateCommandType.Manual => UiString.SpecialCommandType_Manual.Local(),
        StateCommandType.Off => UiString.SpecialCommandType_Off.Local(),
        _ => string.Empty,
    };
}
