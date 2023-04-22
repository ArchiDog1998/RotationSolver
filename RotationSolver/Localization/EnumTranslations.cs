using Dalamud.Game.ClientState.Keys;
using RotationSolver.Commands;
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
        TargetConditionType.CastingActionTimeUntil => LocalizationManager.RightLang.TargetConditionType_CastingActionTimeUntil,
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

    internal static string ToName(this ActionConditionType type) => type switch
    {
        ActionConditionType.Elapsed => LocalizationManager.RightLang.ActionConditionType_Elapsed,
        ActionConditionType.ElapsedGCD => LocalizationManager.RightLang.ActionConditionType_ElapsedGCD,
        ActionConditionType.Remain => LocalizationManager.RightLang.ActionConditionType_Remain,
        ActionConditionType.RemainGCD => LocalizationManager.RightLang.ActionConditionType_RemainGCD,
        ActionConditionType.ShouldUse => LocalizationManager.RightLang.ActionConditionType_ShouldUse,
        ActionConditionType.EnoughLevel => LocalizationManager.RightLang.ActionConditionType_EnoughLevel,
        ActionConditionType.IsCoolDown => LocalizationManager.RightLang.ActionConditionType_IsCoolDown,
        ActionConditionType.CurrentCharges => LocalizationManager.RightLang.ActionConditionType_CurrentCharges,
        ActionConditionType.MaxCharges => LocalizationManager.RightLang.ActionConditionType_MaxCharges,
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
        DescType.BurstActions => LocalizationManager.RightLang.DescType_BurstActions,

        DescType.MoveForwardGCD => LocalizationManager.RightLang.DescType_MoveForwardGCD,
        DescType.HealSingleGCD => LocalizationManager.RightLang.DescType_HealSingleGCD,
        DescType.HealAreaGCD => LocalizationManager.RightLang.DescType_HealAreaGCD,
        DescType.DefenseSingleGCD => LocalizationManager.RightLang.DescType_DefenseSingleGCD,
        DescType.DefenseAreaGCD => LocalizationManager.RightLang.DescType_DefenseAreaGCD,

        DescType.MoveForwardAbility => LocalizationManager.RightLang.DescType_MoveForwardAbility,
        DescType.MoveBackAbility => LocalizationManager.RightLang.DescType_MoveBackAbility,
        DescType.HealSingleAbility => LocalizationManager.RightLang.DescType_HealSingleAbility,
        DescType.HealAreaAbility => LocalizationManager.RightLang.DescType_HealAreaAbility,
        DescType.DefenseSingleAbility => LocalizationManager.RightLang.DescType_DefenseSingleAbility,
        DescType.DefenseAreaAbility => LocalizationManager.RightLang.DescType_DefenseAreaAbility,

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
        JobRole.RangedMagical => LocalizationManager.RightLang.JobRole_RangedMagical,
        JobRole.DiscipleOfTheLand => LocalizationManager.RightLang.JobRole_DiscipleOfTheLand,
        JobRole.DiscipleOfTheHand => LocalizationManager.RightLang.JobRole_DiscipleOfTheHand,
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
        SpecialCommandType.EsunaStanceNorth => role switch
        {
            JobRole.Tank => LocalizationManager.RightLang.SpecialCommandType_TankStance,
            JobRole.Healer => CustomRotation.Esuna.Name,
            JobRole.Melee => CustomRotation.TrueNorth.Name,
            _ => nameof(SpecialCommandType.EsunaStanceNorth),
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
        SpecialCommandType.HealArea => LocalizationManager.RightLang.ConfigWindow_HelpItem_HealArea,
        SpecialCommandType.HealSingle => LocalizationManager.RightLang.ConfigWindow_HelpItem_HealSingle,
        SpecialCommandType.DefenseArea => LocalizationManager.RightLang.ConfigWindow_HelpItem_DefenseArea,
        SpecialCommandType.DefenseSingle => LocalizationManager.RightLang.ConfigWindow_HelpItem_DefenseSingle,
        SpecialCommandType.EsunaStanceNorth => LocalizationManager.RightLang.ConfigWindow_HelpItem_Esuna,
        SpecialCommandType.RaiseShirk => LocalizationManager.RightLang.ConfigWindow_HelpItem_RaiseShirk,
        SpecialCommandType.MoveForward => LocalizationManager.RightLang.ConfigWindow_HelpItem_MoveForward,
        SpecialCommandType.MoveBack => LocalizationManager.RightLang.ConfigWindow_HelpItem_MoveBack,
        SpecialCommandType.AntiKnockback => LocalizationManager.RightLang.ConfigWindow_HelpItem_AntiKnockback,
        SpecialCommandType.Burst => LocalizationManager.RightLang.ConfigWindow_HelpItem_Burst,
        SpecialCommandType.EndSpecial => LocalizationManager.RightLang.ConfigWindow_HelpItem_EndSpecial,
        _ => string.Empty,
    };

    internal static string ToHelp(this StateCommandType type) => type switch
    {
        StateCommandType.Smart => LocalizationManager.RightLang.ConfigWindow_HelpItem_AttackSmart,
        StateCommandType.Manual => LocalizationManager.RightLang.ConfigWindow_HelpItem_AttackManual,
        StateCommandType.Cancel => LocalizationManager.RightLang.ConfigWindow_HelpItem_AttackCancel,
        _ => string.Empty,
    };
}
