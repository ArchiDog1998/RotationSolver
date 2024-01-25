using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Localization;

internal static class EnumTranslations
{
    internal static string ToName(this TargetConditionType type) => type switch
    {
        TargetConditionType.HasStatus => LocalizationManager._rightLang.TargetConditionType_HasStatus,
        TargetConditionType.IsDying => LocalizationManager._rightLang.TargetConditionType_IsDying,
        TargetConditionType.IsBossFromTTK => LocalizationManager._rightLang.TargetConditionType_IsBossFromTTK,
        TargetConditionType.IsBossFromIcon => LocalizationManager._rightLang.TargetConditionType_IsBossFromIcon,
        TargetConditionType.InCombat => LocalizationManager._rightLang.TargetConditionType_InCombat,
        TargetConditionType.Distance => LocalizationManager._rightLang.TargetConditionType_Distance,
        TargetConditionType.StatusEnd => LocalizationManager._rightLang.TargetConditionType_StatusEnd,
        TargetConditionType.StatusEndGCD => LocalizationManager._rightLang.TargetConditionType_StatusEndGCD,
        TargetConditionType.CastingAction => LocalizationManager._rightLang.TargetConditionType_CastingAction,
        TargetConditionType.CastingActionTime => LocalizationManager._rightLang.TargetConditionType_CastingActionTimeUntil,
        TargetConditionType.TimeToKill => LocalizationManager._rightLang.TargetConditionType_TimeToKill,
        TargetConditionType.HP => LocalizationManager._rightLang.TargetConditionType_HP,
        TargetConditionType.HPRatio => LocalizationManager._rightLang.TargetConditionType_HPRatio,
        TargetConditionType.MP => LocalizationManager._rightLang.TargetConditionType_MP,
        TargetConditionType.TargetName => LocalizationManager._rightLang.TargetConditionType_TargetName,
        TargetConditionType.ObjectEffect => LocalizationManager._rightLang.TargetConditionType_ObjectEffect,
        TargetConditionType.Vfx => LocalizationManager._rightLang.TargetConditionType_Vfx,
        TargetConditionType.IsNull => LocalizationManager._rightLang.TargetConditionType_IsNull,
        _ => string.Empty,
    };

    internal static string ToName(this ComboConditionType type) => type switch
    {
        ComboConditionType.Bool => LocalizationManager._rightLang.ComboConditionType_Bool,
        ComboConditionType.Integer => LocalizationManager._rightLang.ComboConditionType_Byte,
        ComboConditionType.Float => LocalizationManager._rightLang.ComboConditionType_Float,
        ComboConditionType.Last => LocalizationManager._rightLang.ComboConditionType_Last,
        _ => string.Empty,
    };

    internal static string ToName(this TerritoryConditionType type) => type switch
    {
        TerritoryConditionType.TerritoryContentType => LocalizationManager._rightLang.TerritoryConditionType_TerritoryContentType,
        TerritoryConditionType.TerritoryName => LocalizationManager._rightLang.TerritoryConditionType_TerritoryName,
        TerritoryConditionType.DutyName => LocalizationManager._rightLang.TerritoryConditionType_DutyName,
        TerritoryConditionType.MapEffect => LocalizationManager._rightLang.TerritoryConditionType_MapEffect,
        _ => string.Empty,
    };

    internal static string ToName(this ActionConditionType type) => type switch
    {
        ActionConditionType.Elapsed => LocalizationManager._rightLang.ActionConditionType_Elapsed,
        ActionConditionType.ElapsedGCD => LocalizationManager._rightLang.ActionConditionType_ElapsedGCD,
        ActionConditionType.Remain => LocalizationManager._rightLang.ActionConditionType_Remain,
        ActionConditionType.RemainGCD => LocalizationManager._rightLang.ActionConditionType_RemainGCD,
        ActionConditionType.CanUse => LocalizationManager._rightLang.ActionConditionType_ShouldUse,
        ActionConditionType.EnoughLevel => LocalizationManager._rightLang.ActionConditionType_EnoughLevel,
        ActionConditionType.IsCoolDown => LocalizationManager._rightLang.ActionConditionType_IsCoolDown,
        ActionConditionType.CurrentCharges => LocalizationManager._rightLang.ActionConditionType_CurrentCharges,
        ActionConditionType.MaxCharges => LocalizationManager._rightLang.ActionConditionType_MaxCharges,
        _ => string.Empty,
    };

    public static string ToName(this DescType type) => type switch
    {
        DescType.BurstActions => LocalizationManager._rightLang.DescType_BurstActions,

        DescType.MoveForwardGCD => LocalizationManager._rightLang.DescType_MoveForwardGCD,
        DescType.HealSingleGCD => LocalizationManager._rightLang.DescType_HealSingleGCD,
        DescType.HealAreaGCD => LocalizationManager._rightLang.DescType_HealAreaGCD,
        DescType.DefenseSingleGCD => LocalizationManager._rightLang.DescType_DefenseSingleGCD,
        DescType.DefenseAreaGCD => LocalizationManager._rightLang.DescType_DefenseAreaGCD,

        DescType.MoveForwardAbility => LocalizationManager._rightLang.DescType_MoveForwardAbility,
        DescType.MoveBackAbility => LocalizationManager._rightLang.DescType_MoveBackAbility,
        DescType.HealSingleAbility => LocalizationManager._rightLang.DescType_HealSingleAbility,
        DescType.HealAreaAbility => LocalizationManager._rightLang.DescType_HealAreaAbility,
        DescType.DefenseSingleAbility => LocalizationManager._rightLang.DescType_DefenseSingleAbility,
        DescType.DefenseAreaAbility => LocalizationManager._rightLang.DescType_DefenseAreaAbility,

        DescType.SpeedAbility => LocalizationManager._rightLang.DescType_SpeedAbility,

        _ => string.Empty,
    };

    public static string ToName(this TargetingType role) => role switch
    {
        TargetingType.Big => LocalizationManager._rightLang.TargetingType_Big,
        TargetingType.Small => LocalizationManager._rightLang.TargetingType_Small,
        TargetingType.HighHP => LocalizationManager._rightLang.TargetingType_HighHP,
        TargetingType.LowHP => LocalizationManager._rightLang.TargetingType_LowHP,
        TargetingType.HighMaxHP => LocalizationManager._rightLang.TargetingType_HighMaxHP,
        TargetingType.LowMaxHP => LocalizationManager._rightLang.TargetingType_LowMaxHP,
        _ => string.Empty,
    };

    internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.EndSpecial => type.ToSpecialString(role),
        _ => LocalizationManager._rightLang.SpecialCommandType_Start + type.ToSpecialString(role),
    };

    internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
    {
        StateCommandType.Cancel => LocalizationManager._rightLang.SpecialCommandType_Cancel,
        _ => type.ToStateString(role),
    };

    internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
    {
        SpecialCommandType.HealArea => LocalizationManager._rightLang.SpecialCommandType_HealArea,
        SpecialCommandType.HealSingle => LocalizationManager._rightLang.SpecialCommandType_HealSingle,
        SpecialCommandType.DefenseArea => LocalizationManager._rightLang.SpecialCommandType_DefenseArea,
        SpecialCommandType.DefenseSingle => LocalizationManager._rightLang.SpecialCommandType_DefenseSingle,
        SpecialCommandType.EsunaStanceNorth => role switch
        {
            JobRole.Tank => LocalizationManager._rightLang.SpecialCommandType_TankStance,
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
        SpecialCommandType.MoveForward => LocalizationManager._rightLang.SpecialCommandType_MoveForward,
        SpecialCommandType.MoveBack => LocalizationManager._rightLang.SpecialCommandType_MoveBack,
        SpecialCommandType.AntiKnockback => LocalizationManager._rightLang.SpecialCommandType_AntiKnockback,
        SpecialCommandType.Burst => LocalizationManager._rightLang.SpecialCommandType_Burst,
        SpecialCommandType.EndSpecial => LocalizationManager._rightLang.SpecialCommandType_EndSpecial,
        SpecialCommandType.Speed => LocalizationManager._rightLang.SpecialCommandType_Speed,
        SpecialCommandType.LimitBreak => LocalizationManager._rightLang.SpecialCommandType_LimitBreak,
        _ => string.Empty,
    };

    internal static string ToStateString(this StateCommandType type, JobRole _) => type switch
    {
        StateCommandType.Auto => LocalizationManager._rightLang.SpecialCommandType_Smart + DataCenter.TargetingType.ToName(),
        StateCommandType.Manual => LocalizationManager._rightLang.SpecialCommandType_Manual,
        StateCommandType.Cancel => LocalizationManager._rightLang.SpecialCommandType_Off,
        _ => string.Empty,
    };

    internal static string ToHelp(this SpecialCommandType type) => type switch
    {
        SpecialCommandType.HealArea => LocalizationManager._rightLang.ConfigWindow_HelpItem_HealArea,
        SpecialCommandType.HealSingle => LocalizationManager._rightLang.ConfigWindow_HelpItem_HealSingle,
        SpecialCommandType.DefenseArea => LocalizationManager._rightLang.ConfigWindow_HelpItem_DefenseArea,
        SpecialCommandType.DefenseSingle => LocalizationManager._rightLang.ConfigWindow_HelpItem_DefenseSingle,
        SpecialCommandType.EsunaStanceNorth => LocalizationManager._rightLang.ConfigWindow_HelpItem_Esuna,
        SpecialCommandType.RaiseShirk => LocalizationManager._rightLang.ConfigWindow_HelpItem_RaiseShirk,
        SpecialCommandType.MoveForward => LocalizationManager._rightLang.ConfigWindow_HelpItem_MoveForward,
        SpecialCommandType.MoveBack => LocalizationManager._rightLang.ConfigWindow_HelpItem_MoveBack,
        SpecialCommandType.AntiKnockback => LocalizationManager._rightLang.ConfigWindow_HelpItem_AntiKnockback,
        SpecialCommandType.Burst => LocalizationManager._rightLang.ConfigWindow_HelpItem_Burst,
        SpecialCommandType.EndSpecial => LocalizationManager._rightLang.ConfigWindow_HelpItem_EndSpecial,
        SpecialCommandType.Speed => LocalizationManager._rightLang.ConfigWindow_HelpItem_Speed,
        SpecialCommandType.LimitBreak => LocalizationManager._rightLang.ConfigWindow_HelpItem_LimitBreak,
        _ => string.Empty,
    };

    internal static string ToHelp(this StateCommandType type) => type switch
    {
        StateCommandType.Auto => LocalizationManager._rightLang.ConfigWindow_HelpItem_AttackAuto,
        StateCommandType.Manual => LocalizationManager._rightLang.ConfigWindow_HelpItem_AttackManual,
        StateCommandType.Cancel => LocalizationManager._rightLang.ConfigWindow_HelpItem_AttackCancel,
        _ => string.Empty,
    };
}
