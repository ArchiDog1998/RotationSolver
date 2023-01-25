using Dalamud.Game.ClientState.Keys;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Rotations.Basic;
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
        ComboConditionType.Bool => LocalizationManager.RightLang.ComboConditionType_Bool,
        ComboConditionType.Byte => LocalizationManager.RightLang.ComboConditionType_Byte,
        ComboConditionType.Time => LocalizationManager.RightLang.ComboConditionType_Time,
        ComboConditionType.TimeGCD => LocalizationManager.RightLang.ComboConditionType_GCD,
        ComboConditionType.Last => LocalizationManager.RightLang.ComboConditionType_Last,
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
        TargetingType.Big => LocalizationManager.RightLang.TargetingType_Big,
        TargetingType.Small => LocalizationManager.RightLang.TargetingType_Small,
        TargetingType.HighHP => LocalizationManager.RightLang.TargetingType_HighHP,
        TargetingType.LowHP => LocalizationManager.RightLang.TargetingType_LowHP,
        TargetingType.HighMaxHP => LocalizationManager.RightLang.TargetingType_HighMaxHP,
        TargetingType.LowMaxHP => LocalizationManager.RightLang.TargetingType_LowMaxHP,
        _ => string.Empty,
    };

    internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.EndSpecial => type.ToSpecialString(role),
        _ => LocalizationManager.RightLang.SpecialCommandType_Start + type.ToSpecialString(role),
    };

    internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Cancel => LocalizationManager.RightLang.SpecialCommandType_Cancel,
        _ => type.ToStateString(role),
    };

    internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => LocalizationManager.RightLang.SpecialCommandType_HealArea,
        SpecialCommandType.HealSingle => LocalizationManager.RightLang.SpecialCommandType_HealSingle,
        SpecialCommandType.DefenseArea => LocalizationManager.RightLang.SpecialCommandType_DefenseArea,
        SpecialCommandType.DefenseSingle => LocalizationManager.RightLang.SpecialCommandType_DefenseSingle,
        SpecialCommandType.EsunaShieldNorth => role switch
        {
            JobRole.Tank => LocalizationManager.RightLang.SpecialCommandType_Shield,
            JobRole.Healer => CustomRotation.Esuna.Name,
            JobRole.Melee => CustomRotation.TrueNorth.Name,
            _ => nameof(SpecialCommandType.EsunaShieldNorth),
        },
        SpecialCommandType.RaiseShirk => role switch
        {
            JobRole.Tank => CustomRotation.Shirk.Name,
            JobRole.Healer => WHM_Base.Raise1.Name,
            _ => nameof(SpecialCommandType.RaiseShirk),
        },
        SpecialCommandType.MoveForward => LocalizationManager.RightLang.SpecialCommandType_MoveForward,
        SpecialCommandType.MoveBack => LocalizationManager.RightLang.SpecialCommandType_MoveBack,
        SpecialCommandType.AntiKnockback => LocalizationManager.RightLang.SpecialCommandType_AntiKnockback,
        SpecialCommandType.Burst => LocalizationManager.RightLang.SpecialCommandType_Burst,
        SpecialCommandType.EndSpecial => LocalizationManager.RightLang.SpecialCommandType_EndSpecial,
        _ => string.Empty,
    };

    internal static string ToStateString(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Smart => LocalizationManager.RightLang.SpecialCommandType_Smart + RSCommands.TargetingType.ToName(),
        StateCommandType.Manual => LocalizationManager.RightLang.SpecialCommandType_Manual,
        StateCommandType.Cancel => LocalizationManager.RightLang.SpecialCommandType_Off,
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
