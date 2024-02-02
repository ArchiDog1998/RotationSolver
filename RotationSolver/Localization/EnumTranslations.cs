namespace RotationSolver.Localization;

internal static class EnumTranslations
{
    internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.EndSpecial => type.ToSpecialString(role),
        _ => "SpecialCommandType_Start".Local("Start ") + type.ToSpecialString(role),
    };

    internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Cancel => "SpecialCommandType_Cancel".Local("Cancel"),
        _ => type.ToStateString(role),
    };

    internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => "SpecialCommandType_HealArea".Local("Heal Area"),
        SpecialCommandType.HealSingle => "SpecialCommandType_HealSingle".Local("Heal Single"),
        SpecialCommandType.DefenseArea => "SpecialCommandType_DefenseArea".Local("Defense Area"),
        SpecialCommandType.DefenseSingle => "SpecialCommandType_DefenseSingle".Local("Defense Single"),
        SpecialCommandType.DispelStancePositional => role switch
        {
            JobRole.Tank => "SpecialCommandType_TankStance".Local("Tank Stance"),
            JobRole.Healer => "SpecialCommandType_Dispel".Local("Dispel"),
            JobRole.Melee => "SpecialCommandType_Positional".Local("Positional"),
            _ => nameof(SpecialCommandType.DispelStancePositional),
        },
        SpecialCommandType.RaiseShirk => role switch
        {
            JobRole.Tank => "SpecialCommandType_Shirk".Local("Shirk"),
            JobRole.Healer => "SpecialCommandType_Raise".Local("Raise"),
            _ => nameof(SpecialCommandType.RaiseShirk),
        },
        SpecialCommandType.MoveForward => "SpecialCommandType_MoveForward".Local("Move Forward"),
        SpecialCommandType.MoveBack => "SpecialCommandType_MoveBack".Local("Move Back"),
        SpecialCommandType.AntiKnockback => "SpecialCommandType_AntiKnockback".Local("Anti Knockback"),
        SpecialCommandType.Burst => "SpecialCommandType_Burst".Local("Burst"),
        SpecialCommandType.EndSpecial => "SpecialCommandType_EndSpecial".Local("End Special"),
        SpecialCommandType.Speed => "SpecialCommandType_Speed".Local("Speed"),
        SpecialCommandType.LimitBreak => "SpecialCommandType_LimitBreak".Local("Limit Break"),
        _ => string.Empty,
    };

    internal static string ToStateString(this StateCommandType type, JobRole _) => type switch
    {
        StateCommandType.Auto => "SpecialCommandType_Smart".Local("Auto Target ") + DataCenter.TargetingType.Local(),
        StateCommandType.Manual => "SpecialCommandType_Manual".Local("Manual Target"),
        StateCommandType.Cancel => "SpecialCommandType_Off".Local("Off"),
        _ => string.Empty,
    };
}
