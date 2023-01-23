using Dalamud.Game.ClientState.Keys;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Timeline;

namespace RotationSolver.Localization;

internal static class EnumTranslations
{
    internal static string ToName(this TargetConditionType type) => type switch
    {
        TargetConditionType.HaveStatus => LocalizationManager.RightLang.TargetConditionType_HaveStatus,
        TargetConditionType.IsDying => LocalizationManager.RightLang.TargetConditionType_IsDying,
        TargetConditionType.IsBoss => LocalizationManager.RightLang.TargetConditionType_IsBoss,
        TargetConditionType.Distance => LocalizationManager.RightLang.TargetConditionType_Distance,
        TargetConditionType.StatusEnd => LocalizationManager.RightLang.TargetConditionType_StatusEnd,
        TargetConditionType.StatusEndGCD => LocalizationManager.RightLang.TargetConditionType_StatusEndGCD,
        TargetConditionType.CastingAction => LocalizationManager.RightLang.TargetConditionType_CastingAction,
        _ => string.Empty,
    };

    internal static string ToName(this ComboConditionType type) => type switch
    {
        ComboConditionType.Bool => "Boolean",
        ComboConditionType.Byte => "Byte",
        ComboConditionType.Time => "Time",
        ComboConditionType.TimeGCD => "GCD",
        ComboConditionType.Last => "Action",
        _ => string.Empty,
    };

    internal static string ToName(this ActionConditonType type) => type switch
    {
        ActionConditonType.Elapsed => LocalizationManager.RightLang.ActionConditionType_Elapsed,
        ActionConditonType.ElapsedGCD => LocalizationManager.RightLang.ActionConditionType_ElapsedGCD,
        ActionConditonType.Remain => LocalizationManager.RightLang.ActionConditionType_Remain,
        ActionConditonType.RemainGCD => LocalizationManager.RightLang.ActionConditionType_RemainGCD,
        ActionConditonType.ShouldUse => LocalizationManager.RightLang.ActionConditionType_ShouldUse,
        ActionConditonType.EnoughLevel => LocalizationManager.RightLang.ActionConditionType_EnoughLevel,
        ActionConditonType.IsCoolDown => LocalizationManager.RightLang.ActionConditionType_IsCoolDown,
        ActionConditonType.CurrentCharges => LocalizationManager.RightLang.ActionConditionType_CurrentCharges,
        ActionConditonType.MaxCharges => LocalizationManager.RightLang.ActionConditionType_MaxCharges,
        _ => string.Empty,
    };

    public static string ToName(this VirtualKey k) => k switch
    {
        VirtualKey.SHIFT => "SHIFT",
        VirtualKey.CONTROL => "CTRL",
        VirtualKey.MENU => "ALT",
        _ => k.ToString(),
    };

    public static string ToName(this EnemyPositional value) => value switch
    {
        EnemyPositional.None => LocalizationManager.RightLang.EnemyLocation_None,
        EnemyPositional.Rear => LocalizationManager.RightLang.EnemyLocation_Rear,
        EnemyPositional.Flank => LocalizationManager.RightLang.EnemyLocation_Flank,
        EnemyPositional.Front => LocalizationManager.RightLang.EnemyLocation_Front,
        _ => string.Empty,
    };

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

    public static string ToName(this JobRole role) => role switch
    {
        JobRole.None => LocalizationManager.RightLang.JobRole_None,
        JobRole.Tank => LocalizationManager.RightLang.JobRole_Tank,
        JobRole.Melee => LocalizationManager.RightLang.JobRole_Melee,
        JobRole.Ranged => LocalizationManager.RightLang.JobRole_Ranged,
        JobRole.Healer => LocalizationManager.RightLang.JobRole_Healer,
        JobRole.RangedPhysical => LocalizationManager.RightLang.JobRole_RangedPhysical,
        JobRole.RangedMagicial => LocalizationManager.RightLang.JobRole_RangedMagicial,
        JobRole.DiscipleoftheLand => LocalizationManager.RightLang.JobRole_DiscipleoftheLand,
        JobRole.DiscipleoftheHand => LocalizationManager.RightLang.JobRole_DiscipleoftheHand,
        _ => string.Empty,
    };

    public static string ToName(this TargetingType role) => role switch
    {
        TargetingType.Big => "Big",
        TargetingType.Small => "Small",
        TargetingType.HighHP => "High HP",
        TargetingType.LowHP => "Low HP",
        TargetingType.HighMaxHP => "High Max HP",
        TargetingType.LowMaxHP => "Low Max HP",
        _ => string.Empty,
    };

    internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => "Start Heal Area",
        SpecialCommandType.HealSingle => "Start Heal Single",
        SpecialCommandType.DefenseArea => "Start Defense Area",
        SpecialCommandType.DefenseSingle => "Start Defense Single",
        SpecialCommandType.EsunaShieldNorth => "Start " + role switch
        {
            JobRole.Tank => "Shield",
            JobRole.Healer => "Esuna",
            JobRole.Melee => "TrueNorth",
            _ => nameof(SpecialCommandType.EsunaShieldNorth),
        },
        SpecialCommandType.RaiseShirk => "Start " + role switch
        {
            JobRole.Tank => "Shirk",
            JobRole.Healer => "Raise",
            _ => nameof(SpecialCommandType.RaiseShirk),
        },
        SpecialCommandType.MoveForward => "Start Move Forward",
        SpecialCommandType.MoveBack => "Start Move Back",
        SpecialCommandType.AntiKnockback => "Start AntiKnockback",
        SpecialCommandType.Burst => "Start Break",
        SpecialCommandType.EndSpecial => "End Special",
        _ => string.Empty,
    };

    internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Smart => "Smart " + RSCommands.TargetingType.ToName(),
        StateCommandType.Manual => "Manual",
        StateCommandType.Cancel => "Cancel",
        _ => string.Empty,
    };

    internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => "Heal Area",
        SpecialCommandType.HealSingle => "Heal Single",
        SpecialCommandType.DefenseArea => "Defense Area",
        SpecialCommandType.DefenseSingle => "Defense Single",
        SpecialCommandType.EsunaShieldNorth => role switch
        {
            JobRole.Tank => "Shield",
            JobRole.Healer => "Esuna",
            JobRole.Melee => "TrueNorth",
            _ => string.Empty,
        },
        SpecialCommandType.RaiseShirk => role switch
        {
            JobRole.Tank => "Shirk",
            JobRole.Healer => "Raise",
            _ => string.Empty,
        },
        SpecialCommandType.MoveForward => "Move Forward",
        SpecialCommandType.MoveBack => "Move Back",
        SpecialCommandType.AntiKnockback => "AntiKnockback",
        SpecialCommandType.Burst => "Break",
        SpecialCommandType.EndSpecial => "End Special",
        _ => string.Empty,
    };

    internal static string ToStateString(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Smart => "Smart " + RSCommands.TargetingType.ToName(),
        StateCommandType.Manual => "Manual",
        StateCommandType.Cancel => "Off",
        _ => string.Empty,
    };

    internal static string ToHelp(this SpecialCommandType type) => type switch
    {
        SpecialCommandType.HealArea => LocalizationManager.RightLang.Configwindow_HelpItem_HealArea,
        SpecialCommandType.HealSingle => LocalizationManager.RightLang.Configwindow_HelpItem_HealSingle,
        SpecialCommandType.DefenseArea => LocalizationManager.RightLang.Configwindow_HelpItem_DefenseArea,
        SpecialCommandType.DefenseSingle => LocalizationManager.RightLang.Configwindow_HelpItem_DefenseSingle,
        SpecialCommandType.EsunaShieldNorth => LocalizationManager.RightLang.Configwindow_HelpItem_EsunaShield,
        SpecialCommandType.RaiseShirk => LocalizationManager.RightLang.Configwindow_HelpItem_RaiseShirk,
        SpecialCommandType.MoveForward => LocalizationManager.RightLang.Configwindow_HelpItem_MoveForward,
        SpecialCommandType.MoveBack => LocalizationManager.RightLang.Configwindow_HelpItem_MoveBack,
        SpecialCommandType.AntiKnockback => LocalizationManager.RightLang.Configwindow_HelpItem_AntiKnockback,
        SpecialCommandType.Burst => LocalizationManager.RightLang.Configwindow_HelpItem_Break,
        SpecialCommandType.EndSpecial => LocalizationManager.RightLang.Configwindow_HelpItem_EndSpecial,
        _ => string.Empty,
    };

    internal static string ToHelp(this StateCommandType type) => type switch
    {
        StateCommandType.Smart => LocalizationManager.RightLang.Configwindow_HelpItem_AttackSmart,
        StateCommandType.Manual => LocalizationManager.RightLang.Configwindow_HelpItem_AttackManual,
        StateCommandType.Cancel => LocalizationManager.RightLang.Configwindow_HelpItem_AttackCancel,
        _ => string.Empty,
    };
}
