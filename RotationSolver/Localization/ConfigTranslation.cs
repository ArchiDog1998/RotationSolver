using RotationSolver.Basic.Configuration;
using RotationSolver.UI;

namespace RotationSolver.Localization;

internal static class ConfigTranslation
{
    public static string ToDescription(this RotationConfigWindowTab tab) => tab switch
    {
        RotationConfigWindowTab.Actions => LocalizationManager._rightLang.ConfigWindow_Tab_Actions,
        RotationConfigWindowTab.Rotations => LocalizationManager._rightLang.ConfigWindow_Tab_Rotations,
        RotationConfigWindowTab.List => LocalizationManager._rightLang.ConfigWindow_Tab_List,
        RotationConfigWindowTab.Basic => LocalizationManager._rightLang.ConfigWindow_Tab_Basic,
        RotationConfigWindowTab.UI => LocalizationManager._rightLang.ConfigWindow_Tab_UI,
        RotationConfigWindowTab.Auto => LocalizationManager._rightLang.ConfigWindow_Tab_Auto,
        RotationConfigWindowTab.Target => LocalizationManager._rightLang.ConfigWindow_Tab_Target,
        RotationConfigWindowTab.Extra => LocalizationManager._rightLang.ConfigWindow_Tab_Extra,
        _ => string.Empty,
    };

    public static string ToName(this JobConfigInt config) => config switch
    {
        JobConfigInt.AddDotGCDCount => LocalizationManager._rightLang.ConfigWindow_Param_AddDotGCDCount,
        JobConfigInt.HostileType => LocalizationManager._rightLang.ConfigWindow_Param_RightNowTargetToHostileType,
        _ => string.Empty,
    };

    public static string ToName(this JobConfigFloat config) => config switch
    {
        JobConfigFloat.HealthForDyingTanks => LocalizationManager._rightLang.ConfigWindow_Param_HealthForDyingTank,
        JobConfigFloat.HealthForAutoDefense => LocalizationManager._rightLang.ConfigWindow_Auto_HealthForAutoDefense,

        JobConfigFloat.ActionAhead => LocalizationManager._rightLang.ConfigWindow_Param_ActionAhead,

        _ => string.Empty,
    };

    public static string ToName(this PluginConfigInt config) => config switch
    {
        PluginConfigInt.PoslockModifier => LocalizationManager._rightLang.ConfigWindow_Param_PoslockModifier,
        PluginConfigInt.BeneficialAreaStrategy => LocalizationManager._rightLang.ConfigWindow_Auto_BeneficialAreaStrategy,

        // UI
        PluginConfigInt.KeyBoardNoiseMin => LocalizationManager._rightLang.ConfigWindow_Param_KeyBoardNoiseTimes,
        PluginConfigInt.LessMPNoRaise => LocalizationManager._rightLang.ConfigWindow_Param_LessMPNoRaise,
        _ => string.Empty,
    };

    public static string ToName(this PluginConfigBool config) => config switch
    {
        // basic
        PluginConfigBool.AutoOffBetweenArea => LocalizationManager._rightLang.ConfigWindow_Param_AutoOffBetweenArea,
        PluginConfigBool.AutoOffCutScene => LocalizationManager._rightLang.ConfigWindow_Param_AutoOffCutScene,
        PluginConfigBool.AutoOffWhenDead => LocalizationManager._rightLang.ConfigWindow_Param_AutoOffWhenDead,
        PluginConfigBool.AutoOffWhenDutyCompleted => LocalizationManager._rightLang.ConfigWindow_Param_AutoOffWhenDutyCompleted,
        PluginConfigBool.StartOnCountdown => LocalizationManager._rightLang.ConfigWindow_Param_StartOnCountdown,
        PluginConfigBool.StartOnAttackedBySomeone => LocalizationManager._rightLang.ConfigWindow_Param_StartOnAttackedBySomeone,
        PluginConfigBool.UseWorkTask => LocalizationManager._rightLang.ConfigWindow_Param_UseWorkTask,
        PluginConfigBool.ToggleManual => LocalizationManager._rightLang.ConfigWindow_Param_ToggleManual,
        PluginConfigBool.ToggleAuto => LocalizationManager._rightLang.ConfigWindow_Param_ToggleAuto,
        PluginConfigBool.UseStopCasting => LocalizationManager._rightLang.ConfigWindow_Param_UseStopCasting,
        PluginConfigBool.SayHelloToUsers => LocalizationManager._rightLang.ConfigWindow_Basic_SayHelloToUsers,
        PluginConfigBool.SayHelloToAll => LocalizationManager._rightLang.ConfigWindow_Basic_SayHelloToAll,
        PluginConfigBool.JustSayHelloOnce => LocalizationManager._rightLang.ConfigWindow_Basic_JustSayHelloOnce,
        PluginConfigBool.UseAdditionalConditions => LocalizationManager._rightLang.ConfigWindow_Basic_UseAdditionalConditions,

        // UI
        PluginConfigBool.HideWarning => LocalizationManager._rightLang.ConfigWindow_UI_HideWarning,

        PluginConfigBool.DrawIconAnimation => LocalizationManager._rightLang.ConfigWindow_UI_DrawIconAnimation,
        PluginConfigBool.UseOverlayWindow => LocalizationManager._rightLang.ConfigWindow_Param_UseOverlayWindow,
        PluginConfigBool.TeachingMode => LocalizationManager._rightLang.ConfigWindow_Param_TeachingMode,
        PluginConfigBool.ShowMoveTarget => LocalizationManager._rightLang.ConfigWindow_Param_ShowMoveTarget,
        PluginConfigBool.ShowTarget => LocalizationManager._rightLang.ConfigWindow_Param_ShowTarget,
        PluginConfigBool.DrawMeleeOffset => LocalizationManager._rightLang.ConfigWindow_Param_DrawMeleeOffset,
        PluginConfigBool.KeyBoardNoise => LocalizationManager._rightLang.ConfigWindow_Param_KeyBoardNoise,
        PluginConfigBool.ShowInfoOnDtr => LocalizationManager._rightLang.ConfigWindow_Param_ShowInfoOnDtr,
        PluginConfigBool.ShowInfoOnToast => LocalizationManager._rightLang.ConfigWindow_Param_ShowInfoOnToast,
        PluginConfigBool.ShowToastsAboutDoAction => LocalizationManager._rightLang.ConfigWindow_Param_ShowToastsAboutDoAction,
        PluginConfigBool.ShowToggledActionInChat => LocalizationManager._rightLang.ConfigWindow_Param_ShowToggledActionInChat,
        PluginConfigBool.OnlyShowWithHostileOrInDuty => LocalizationManager._rightLang.ConfigWindow_Control_OnlyShowWithHostileOrInDuty,
        PluginConfigBool.ShowNextActionWindow => LocalizationManager._rightLang.ConfigWindow_Control_ShowNextActionWindow,
        PluginConfigBool.ShowCooldownWindow => LocalizationManager._rightLang.ConfigWindow_Control_ShowCooldownWindow,
        PluginConfigBool.IsInfoWindowNoInputs => LocalizationManager._rightLang.ConfigWindow_Control_IsInfoWindowNoInputs,
        PluginConfigBool.IsInfoWindowNoMove => LocalizationManager._rightLang.ConfigWindow_Control_IsInfoWindowNoMove,
        PluginConfigBool.ShowItemsCooldown => LocalizationManager._rightLang.ConfigWindow_Control_ShowItemsCooldown,
        PluginConfigBool.ShowGCDCooldown => LocalizationManager._rightLang.ConfigWindow_Control_ShowGCDCooldown,
        PluginConfigBool.UseOriginalCooldown => LocalizationManager._rightLang.ConfigWindow_Control_UseOriginalCooldown,
        PluginConfigBool.ShowControlWindow => LocalizationManager._rightLang.ConfigWindow_Control_ShowControlWindow,
        PluginConfigBool.IsControlWindowLock => LocalizationManager._rightLang.ConfigWindow_Control_IsInfoWindowNoMove,
        PluginConfigBool.ShowBeneficialPositions => LocalizationManager._rightLang.ConfigWindow_UI_ShowBeneficialPosition,
        PluginConfigBool.ShowHostilesIcons => LocalizationManager._rightLang.ConfigWindow_UI_ShowHostiles,

        // auto
        PluginConfigBool.UseAOEAction => LocalizationManager._rightLang.ConfigWindow_Param_UseAOEAction,
        PluginConfigBool.UseAOEWhenManual => LocalizationManager._rightLang.ConfigWindow_Param_UseAOEWhenManual,
        PluginConfigBool.NoNewHostiles => LocalizationManager._rightLang.ConfigWindow_Param_NoNewHostiles,
        PluginConfigBool.AutoBurst => LocalizationManager._rightLang.ConfigWindow_Param_AutoBurst,
        PluginConfigBool.AutoHeal => LocalizationManager._rightLang.ConfigWindow_Param_AutoHeal,
        PluginConfigBool.UseTinctures => LocalizationManager._rightLang.ConfigWindow_Param_UseTinctures,
        PluginConfigBool.UseHealPotions => LocalizationManager._rightLang.ConfigWindow_Param_UseHealPotions,
        PluginConfigBool.UseAbility => LocalizationManager._rightLang.ConfigWindow_Param_UseAbility,
        PluginConfigBool.UseDefenseAbility => LocalizationManager._rightLang.ConfigWindow_Param_UseDefenseAbility,
        PluginConfigBool.AutoTankStance => LocalizationManager._rightLang.ConfigWindow_Param_AutoShield,
        PluginConfigBool.AutoProvokeForTank => LocalizationManager._rightLang.ConfigWindow_Param_AutoProvokeForTank,
        PluginConfigBool.AutoUseTrueNorth => LocalizationManager._rightLang.ConfigWindow_Param_AutoUseTrueNorth,
        PluginConfigBool.RaisePlayerBySwift => LocalizationManager._rightLang.ConfigWindow_Param_RaisePlayerBySwift,
        PluginConfigBool.AutoSpeedOutOfCombat => LocalizationManager._rightLang.ConfigWindow_Param_AutoSpeedOutOfCombat,
        PluginConfigBool.UseGroundBeneficialAbility => LocalizationManager._rightLang.ConfigWindow_Param_UseGroundBeneficialAbility,
        PluginConfigBool.UseGroundBeneficialAbilityWhenMoving => LocalizationManager._rightLang.ConfigWindow_Auto_UseGroundBeneficialAbilityWhenMoving,
        PluginConfigBool.RaisePlayerByCasting => LocalizationManager._rightLang.ConfigWindow_Param_RaisePlayerByCasting,
        PluginConfigBool.UseHealWhenNotAHealer => LocalizationManager._rightLang.ConfigWindow_Param_UseHealWhenNotAHealer,
        PluginConfigBool.InterruptibleMoreCheck => LocalizationManager._rightLang.ConfigWindow_Param_InterruptibleMoreCheck,
        PluginConfigBool.EsunaAll => LocalizationManager._rightLang.ConfigWindow_Param_EsunaAll,
        PluginConfigBool.HealOutOfCombat => LocalizationManager._rightLang.ConfigWindow_Param_HealOutOfCombat,
        PluginConfigBool.OnlyHotOnTanks => LocalizationManager._rightLang.ConfigWindow_Param_OnlyHotOnTanks,
        PluginConfigBool.RecordCastingArea => "Record AOE actions",
        PluginConfigBool.HealWhenNothingTodo => LocalizationManager._rightLang.ConfigWindow_Param_HealWhenNothingTodo,
        PluginConfigBool.UseResourcesAction => LocalizationManager._rightLang.ConfigWindow_Auto_UseResourcesAction,
        PluginConfigBool.OnlyHealSelfWhenNoHealer => LocalizationManager._rightLang.ConfigWindow_Auto_OnlyHealSelfWhenNoHealer,

        // target
        PluginConfigBool.AddEnemyListToHostile => LocalizationManager._rightLang.ConfigWindow_Param_AddEnemyListToHostile,
        PluginConfigBool.OnlyAttackInEnemyList => LocalizationManager._rightLang.ConfigWindow_Param_OnlyAttackInEnemyList,
        PluginConfigBool.ChooseAttackMark => LocalizationManager._rightLang.ConfigWindow_Param_ChooseAttackMark,
        PluginConfigBool.CanAttackMarkAOE => LocalizationManager._rightLang.ConfigWindow_Param_CanAttackMarkAOE,
        PluginConfigBool.FilterStopMark => LocalizationManager._rightLang.ConfigWindow_Param_FilterStopMark,
        PluginConfigBool.ChangeTargetForFate => LocalizationManager._rightLang.ConfigWindow_Param_ChangeTargetForFate,
        PluginConfigBool.OnlyAttackInView => LocalizationManager._rightLang.ConfigWindow_Param_OnlyAttackInView,
        PluginConfigBool.MoveTowardsScreenCenter => LocalizationManager._rightLang.ConfigWindow_Param_MoveTowardsScreen,
        PluginConfigBool.MoveAreaActionFarthest => LocalizationManager._rightLang.ConfigWindow_Param_MoveAreaActionFarthest,
        PluginConfigBool.TargetAllForFriendly => LocalizationManager._rightLang.ConfigWindow_Param_ActionTargetFriendly,
        PluginConfigBool.RaiseAll => LocalizationManager._rightLang.ConfigWindow_Param_RaiseAll,
        PluginConfigBool.RaiseBrinkOfDeath => LocalizationManager._rightLang.ConfigWindow_Param_RaiseBrinkOfDeath,
        PluginConfigBool.SwitchTargetFriendly => LocalizationManager._rightLang.ConfigWindow_Param_TargetFriendly,
        PluginConfigBool.TargetFatePriority => LocalizationManager._rightLang.ConfigWindow_Param_TargetFatePriority,
        PluginConfigBool.TargetHuntingRelicLevePriority => LocalizationManager._rightLang.ConfigWindow_Param_TargetHuntingRelicLevePriority,
        PluginConfigBool.TargetQuestPriority => LocalizationManager._rightLang.ConfigWindow_Param_TargetQuestPriority,
        PluginConfigBool.ShowTargetTimeToKill => LocalizationManager._rightLang.ConfigWindow_Param_ShowTargetTimeToKill,
        PluginConfigBool.ShowStateIcon => LocalizationManager._rightLang.ConfigWindow_UI_ShowStateIcon,
        PluginConfigBool.OnlyAttackInVisionCone => LocalizationManager._rightLang.ConfigWindow_Target_OnlyAttackInVisionCone,


        // extra
        PluginConfigBool.SayOutStateChanged => LocalizationManager._rightLang.ConfigWindow_Param_SayOutStateChanged,
        PluginConfigBool.PoslockCasting => LocalizationManager._rightLang.ConfigWindow_Param_PoslockCasting,
        PluginConfigBool.ShowTooltips => LocalizationManager._rightLang.ConfigWindow_Param_ShowTooltips,
        PluginConfigBool.InDebug => LocalizationManager._rightLang.ConfigWindow_Param_InDebug,
        PluginConfigBool.AutoOpenChest => "Auto Open the treasure chest",
        PluginConfigBool.AutoCloseChestWindow => "Auto close the loot window when auto opened the chest.",

        //Rotations
        PluginConfigBool.DownloadRotations => LocalizationManager._rightLang.ConfigWindow_Rotation_DownloadRotations,
        PluginConfigBool.AutoUpdateRotations => LocalizationManager._rightLang.ConfigWindow_Rotation_AutoUpdateRotations,
        PluginConfigBool.AutoLoadCustomRotations => LocalizationManager._rightLang.ConfigWindow_Rotations_AutoLoadCustomRotations,
        PluginConfigBool.AutoOffAfterCombat => LocalizationManager._rightLang.ConfigWindow_Param_AutoOffAfterCombat,

        _ => string.Empty,
    };

    public static string ToName(this PluginConfigFloat config) => config switch
    {
        // basic
        PluginConfigFloat.MinLastAbilityAdvanced => LocalizationManager._rightLang.ConfigWindow_Param_MinLastAbilityAdvanced,
        PluginConfigFloat.CountDownAhead => LocalizationManager._rightLang.ConfigWindow_Param_CountDownAhead,
        PluginConfigFloat.SpecialDuration => LocalizationManager._rightLang.ConfigWindow_Param_SpecialDuration,
        PluginConfigFloat.MaxPing => LocalizationManager._rightLang.ConfigWindow_Param_MaxPing,
        PluginConfigFloat.WeaponDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_WeaponDelay,
        PluginConfigFloat.HostileDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_HostileDelay,
        PluginConfigFloat.InterruptDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_InterruptDelay,
        PluginConfigFloat.DeathDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_DeathDelay,
        PluginConfigFloat.WeakenDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_WeakenDelay,
        PluginConfigFloat.HealDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_HealDelay,
        PluginConfigFloat.CountdownDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_CountdownDelay,
        PluginConfigFloat.NotInCombatDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_NotInCombatDelay,
        PluginConfigFloat.ClickingDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_ClickingDelay,
        PluginConfigFloat.StopCastingDelayMin => LocalizationManager._rightLang.ConfigWindow_Param_StopCastingDelay,
        PluginConfigFloat.MistakeRatio => LocalizationManager._rightLang.ConfigWindow_Param_ClickMistake,
        PluginConfigFloat.MinUpdatingTime => LocalizationManager._rightLang.ConfigWindow_Basic_MinUpdatingTime,

        // UI
        PluginConfigFloat.TargetIconSize => LocalizationManager._rightLang.ConfigWindow_Param_TargetIconSize,
        PluginConfigFloat.DrawingHeight => LocalizationManager._rightLang.ConfigWindow_Param_DrawingHeight,
        PluginConfigFloat.SampleLength => LocalizationManager._rightLang.ConfigWindow_Param_SampleLength,
        PluginConfigFloat.CooldownFontSize => LocalizationManager._rightLang.ConfigWindow_Control_CooldownFontSize,
        PluginConfigFloat.CooldownWindowIconSize => LocalizationManager._rightLang.ConfigWindow_Control_CooldownWindowIconSize,
        PluginConfigFloat.ControlWindowGCDSize => LocalizationManager._rightLang.ConfigWindow_Control_ControlWindowGCDSize,
        PluginConfigFloat.ControlWindow0GCDSize => LocalizationManager._rightLang.ConfigWindow_Control_ControlWindow0GCDSize,
        PluginConfigFloat.ControlWindowNextSizeRatio => LocalizationManager._rightLang.ConfigWindow_Control_ControlWindowNextSizeRatio,
        PluginConfigFloat.HostileIconHeight => LocalizationManager._rightLang.ConfigWindow_UI_HostileIconHeight,
        PluginConfigFloat.HostileIconSize => LocalizationManager._rightLang.ConfigWindow_UI_HostileIconSize,
        PluginConfigFloat.StateIconHeight => LocalizationManager._rightLang.ConfigWindow_UI_StateIconHeight,
        PluginConfigFloat.StateIconSize => LocalizationManager._rightLang.ConfigWindow_UI_StateIconSize,

        // auto
        PluginConfigFloat.HealWhenNothingTodoBelow => LocalizationManager._rightLang.ConfigWindow_Param_HealWhenNothingTodoBelow,
        PluginConfigFloat.HealWhenNothingTodoMin => LocalizationManager._rightLang.ConfigWindow_Param_HealWhenNothingTodoDelay,
        PluginConfigFloat.DistanceForMoving => LocalizationManager._rightLang.ConfigWindow_Param_DistanceForMoving,
        PluginConfigFloat.MeleeRangeOffset => LocalizationManager._rightLang.ConfigWindow_Param_MeleeRangeOffset,
        PluginConfigFloat.HealthDifference => LocalizationManager._rightLang.ConfigWindow_Param_HealthDifference,
        PluginConfigFloat.HealthHealerRatio => LocalizationManager._rightLang.ConfigWindow_Param_HealthHealerRatio,
        PluginConfigFloat.HealthTankRatio => LocalizationManager._rightLang.ConfigWindow_Param_HealthTankRatio,
        PluginConfigFloat.MoveTargetAngle => LocalizationManager._rightLang.ConfigWindow_Param_MoveTargetAngle,
        PluginConfigFloat.AutoHealTimeToKill => LocalizationManager._rightLang.ConfigWindow_Auto_AutoHealTimeToKill,
        PluginConfigFloat.ProvokeDelayMin => LocalizationManager._rightLang.ConfigWindow_Auto_ProvokeDelay,
        PluginConfigFloat.HealthForGuard => LocalizationManager._rightLang.ConfigWindow_Param_HealthForGuard,
        // target
        PluginConfigFloat.BossTimeToKill => LocalizationManager._rightLang.ConfigWindow_Param_BossTimeToKill,
        PluginConfigFloat.DyingTimeToKill => LocalizationManager._rightLang.ConfigWindow_Param_DyingTimeToKill,
        PluginConfigFloat.AngleOfVisionCone => LocalizationManager._rightLang.ConfigWindow_Target_VisionCone,

        _ => string.Empty,
    };

    public static string ToName(this PluginConfigVector4 config) => config switch
    {
        PluginConfigVector4.TeachingModeColor => LocalizationManager._rightLang.ConfigWindow_Param_TeachingModeColor,
        PluginConfigVector4.MovingTargetColor => LocalizationManager._rightLang.ConfigWindow_Param_MovingTargetColor,
        PluginConfigVector4.TargetColor => LocalizationManager._rightLang.ConfigWindow_Param_TargetColor,
        PluginConfigVector4.SubTargetColor => LocalizationManager._rightLang.ConfigWindow_Param_SubTargetColor,
        PluginConfigVector4.InfoWindowBg => LocalizationManager._rightLang.ConfigWindow_Control_InfoWindowBg,
        PluginConfigVector4.ControlWindowLockBg => LocalizationManager._rightLang.ConfigWindow_Control_LockBackgroundColor,
        PluginConfigVector4.ControlWindowUnlockBg => LocalizationManager._rightLang.ConfigWindow_Control_UnlockBackgroundColor,
        PluginConfigVector4.BeneficialPositionColor => LocalizationManager._rightLang.ConfigWindow_UI_BeneficialPositionColor,
        PluginConfigVector4.HoveredBeneficialPositionColor => LocalizationManager._rightLang.ConfigWindow_UI_HoveredBeneficialPositionColor,
        PluginConfigVector4.TTKTextColor => LocalizationManager._rightLang.ConfigWindow_UI_TTKTextColor,

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
        PluginConfigInt.PoslockModifier => LocalizationManager._rightLang.ConfigWindow_Param_PoslockDescription,
        PluginConfigInt.AutoDefenseNumber => LocalizationManager._rightLang.ConfigWindow_Auto_AutoDefenseNumber,

        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigBool config) => config switch
    {
        PluginConfigBool.UseOverlayWindow => LocalizationManager._rightLang.ConfigWindow_Param_UseOverlayWindowDesc,
        PluginConfigBool.NoNewHostiles => LocalizationManager._rightLang.ConfigWindow_Params_NoNewHostilesDesc,
        PluginConfigBool.UseDefenseAbility => LocalizationManager._rightLang.ConfigWindow_Param_UseDefenseAbilityDesc,
        PluginConfigBool.AutoProvokeForTank => LocalizationManager._rightLang.ConfigWindow_Param_AutoProvokeForTankDesc,
        PluginConfigBool.CanAttackMarkAOE => LocalizationManager._rightLang.ConfigWindow_Param_AttackMarkAOEDesc,
        PluginConfigBool.MoveTowardsScreenCenter => LocalizationManager._rightLang.ConfigWindow_Param_MoveTowardsScreenDesc,
        PluginConfigBool.MoveAreaActionFarthest => LocalizationManager._rightLang.ConfigWindow_Param_MoveAreaActionFarthestDesc,

        PluginConfigBool.SayHelloToUsers => LocalizationManager._rightLang.ConfigWindow_Basic_SayHelloToUsersDesc,

        PluginConfigBool.AutoOpenChest => "Because of the feature in pandora, there is an issue the treasure chest cannot be opened in some cases, I find the code from roll for loot. Once Pandora fixed that, this feature will be deleted.",
        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigFloat config) => config switch
    {
        PluginConfigFloat.MoveTargetAngle => LocalizationManager._rightLang.ConfigWindow_Param_MoveTargetAngleDesc,

        _ => string.Empty,
    };

    public static string ToDescription(this PluginConfigVector4 config) => config switch
    {
        _ => string.Empty,
    };

    public static string ToCommand(this JobConfigInt config) => ToCommandStr(config, "1");
    public static string ToCommand(this JobConfigFloat config) => ToCommandStr(config, "0");
    public static string ToCommand(this PluginConfigInt config) => ToCommandStr(config, "1");
    public static string ToCommand(this PluginConfigBool config) => ToCommandStr(config);
    public static string ToCommand(this PluginConfigFloat config) => ToCommandStr(config, "0");
    private static string ToCommandStr(object obj, string extra = "")
    {
        var result = Service.COMMAND + " " + OtherCommandType.Settings.ToString() + " " + obj.ToString();
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }

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
        //PluginConfigFloat.ActionAhead => new LinkDescription[]
        //{
        //    //new LinkDescription()
        //    //{
        //    //    Url = $"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Images/HowAndWhenToClick.svg",
        //    //    Description = "This plugin helps you to use the right action during the combat. Here is a guide about the different options.",
        //    //},
        //},
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
