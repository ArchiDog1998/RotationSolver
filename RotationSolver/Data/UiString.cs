using System.ComponentModel;

namespace RotationSolver.Data;
internal enum UiString
{
    [Description("The condition set you chose, click to modify.")]
    ConfigWindow_ConditionSetDesc,

    [Description("No rotations loaded! Please see the rotations tab!")]
    ConfigWindow_NoRotation,

    [Description("The duty rotation you chose, click to modify.")]
    ConfigWindow_DutyRotationDesc,

    [Description("Remove")]
    ConfigWindow_List_Remove,

    [Description("Load From folder.")]
    ActionSequencer_Load,

    [Description("Analyses PvE combat information in every frame and finds the best action.")]
    ConfigWindow_About_Punchline,

    [Description("Game")]
    ConfigWindow_Helper_GameVersion,

    [Description("Invalid Rotation! \nPlease update to the latest version or contact to the {0}!")]
    ConfigWindow_Rotation_InvalidRotation,

    [Description("Beta Rotation!")]
    ConfigWindow_Rotation_BetaRotation,

    [Description("Click to switch rotations")]
    ConfigWindow_Helper_SwitchRotation,

    [Description("Search Result")]
    ConfigWindow_Search_Result,

    [Description("Well, you must be a lazy player!")]
    ConfigWindow_About_Clicking100k,

    [Description("You're tiring RSR out, give it a break!")]
    ConfigWindow_About_Clicking500k,

    [Description("This means almost all the information available in one frame in combat, including the status of all players in the party, the status of any hostile targets, skill cooldowns, the MP and HP of characters, the location of characters, casting status of the hostile target, combo, combat duration, player level, etc.\n\nThen, it will highlight the best action on the hot bar, or help you to click on it.")]
    ConfigWindow_About_Description,

    [Description("It is designed for GENERAL COMBAT, not for savage or ultimate. Use it carefully.")]
    ConfigWindow_About_Warning,

    [Description("RSR helped you by clicking actions {0:N0} times.")]
    ConfigWindow_About_ClickingCount,

    [Description("Macro")]
    ConfigWindow_About_Macros,

    [Description("Compatibility")]
    ConfigWindow_About_Compatibility,

    [Description("Supporters")]
    ConfigWindow_About_Supporters,

    [Description("Links")]
    ConfigWindow_About_Links,

    [Description("System Warnings")]
    ConfigWindow_About_Warnings,

    [Description("Warning Message")]
    ConfigWindow_About_Warnings_Warning,

    [Description("Warning Time")]
    ConfigWindow_About_Warnings_Time,

    [Description("Please don't relog without closing the game. Crashes may occur.")]
    ConfigWindow_About_Compatibility_Others,

    [Description("Literally, Rotation Solver helps you to choose the target and then click the action. So any plugin that changes these will affect its decision.\n\nHere is a list of known incompatible plugins:")]
    ConfigWindow_About_Compatibility_Description,

    [Description("Can't properly execute the behavior that RSR wants to do.")]
    ConfigWindow_About_Compatibility_Mistake,

    [Description("Conflicts with RSR decision making")]
    ConfigWindow_About_Compatibility_Mislead,

    [Description("Causes the game to crash.")]
    ConfigWindow_About_Compatibility_Crash,

    [Description("Many thanks to the sponsors.")]
    ConfigWindow_About_ThanksToSupporters,

    [Description("Open Config Folder")]
    ConfigWindow_About_OpenConfigFolder,

    [Description("Description")]
    ConfigWindow_Rotation_Description,

    [Description("Configuration")]
    ConfigWindow_Rotation_Configuration,

    [Description("Rating")]
    ConfigWindow_Rotation_Rating,

    [Description("Information")]
    ConfigWindow_Rotation_Information,

    [Description("Here are some rating methods to analysis this rotation. Most of these methods need your engagement.")]
    ConfigWindow_Rotation_Rating_Description,

    [Description("This is the count of using last action checking in this rotation. First is average one, second is maximum one. The less the better.\nLast used action is not a part of information from the game, it is recorded by player or author. \nIt can't accurately describe the current state of combat, which may make this rotation not general. \nFor example, clipping the gcd, death, take some status that grated by some action off manually, etc.")]
    ConfigWindow_Rotation_Rating_CountOfLastUsing,

    [Description("This is the count of using combat time in this rotation. First is average one, second is maximum one. The less the better.\nCombat time is not a part of information from the game, it is recorded by player or author. \nIt can't accurately describe the current state of combat, which may make this rotation not general.\nFor example, engaged by others in the party, different gcd time, etc.")]
    ConfigWindow_Rotation_Rating_CountOfCombatTimeUsing,

    [Description("Status")]
    ConfigWindow_Rotation_Status,

    [Description("Used to customize when RSR uses specific actions automatically, click on an action's icon in the left list. Below, you may set the conditions for when that specific action is used. Each action can have a different set of conditions to override the default rotation behavior.")]
    ConfigWindow_Actions_Description,

    [Description("Show on CD window")]
    ConfigWindow_Actions_ShowOnCDWindow,

    [Description("TTK that this action needs the target be before it is used.")]
    ConfigWindow_Actions_TTK,

    [Description("TTK that this action needs the target be on the timeline before it is used")]
    ConfigWindow_Actions_TTU,

    [Description("How many targets are needed to use this action.")]
    ConfigWindow_Actions_AoeCount,

    [Description("Should this action check the status.")]
    ConfigWindow_Actions_CheckStatus,

    [Description("How many gcds before the dot is reapplied.")]
    ConfigWindow_Actions_GcdCount,

    [Description("The HP ratio to auto heal")]
    ConfigWindow_Actions_HealRatio,

    [Description("Forced Conditions have a higher priority. If Forced Conditions are met, Disabled Condition will be ignored.")]
    ConfigWindow_Actions_ConditionDescription,

    [Description("Forced Condition")]
    ConfigWindow_Actions_ForcedConditionSet,

    [Description("Conditions for forced automatic use of action.")]
    ConfigWindow_Actions_ForcedConditionSet_Description,

    [Description("Disabled Condition")]
    ConfigWindow_Actions_DisabledConditionSet,

    [Description("Conditions for automatic use of action being disabled.")]
    ConfigWindow_Actions_DisabledConditionSet_Description,

    [Description("It looks like this might be your first time here. Rotation Solver Reborn does not come with rotations out of the box, but you can download ones created by the community. You can also create your own rotations! For your convenience, Rotation Solver Reborn comes pre-loaded with links to well-known community rotations, but it still your responsibility to install them.")]
    ConfigWindow_Rotations_FirstTime,

    [Description("Custom rotations are just like plugins and have full access to the game and your computer")]
    ConfigWindow_Rotations_Warning,

    [Description("Please only load rotations from sources that you trust")]
    ConfigWindow_Rotations_Warning2,

    [Description("Update Rotations")]
    ConfigWindow_Rotations_Download,

    [Description("Reset To Defaults")]
    ConfigWindow_Rotations_Reset,

    [Description("Rotation Sources:")]
    ConfigWindow_Rotations_Sources,

    [Description("Links of the rotations online")]
    ConfigWindow_Rotations_Links,

    [Description("Settings")]
    ConfigWindow_Rotations_Settings,

    [Description("Loaded")]
    ConfigWindow_Rotations_Loaded,

    [Description("Github")]
    ConfigWindow_Rotations_GitHub,

    [Description("Libraries")]
    ConfigWindow_Rotations_Libraries,

    [Description("User Name")]
    ConfigWindow_Rotations_UserName,

    [Description("Repository")]
    ConfigWindow_Rotations_Repository,

    [Description("File Name")]
    ConfigWindow_Rotations_FileName,

    [Description("The folder contains the rotation library or the download url for the rotation library.")]
    ConfigWindow_Rotations_Library,

    [Description("In this window, you can set the parameters that can be customised using lists.")]
    ConfigWindow_List_Description,

    [Description("Statuses")]
    ConfigWindow_List_Statuses,

    [Description("Actions")]
    ConfigWindow_List_Actions,

    [Description("Map specific settings")]
    ConfigWindow_List_Territories,

    [Description("Status name or id")]
    ConfigWindow_List_StatusNameOrId,

    [Description("Invulnerability")]
    ConfigWindow_List_Invincibility,

    [Description("Priority")]
    ConfigWindow_List_Priority,

    [Description("Dispellable debuffs")]
    ConfigWindow_List_DangerousStatus,

    [Description("No Casting debuffs")]
    ConfigWindow_List_NoCastingStatus,

    [Description("Ignores target if it has one of these statuses")]
    ConfigWindow_List_InvincibilityDesc,

    [Description("Attacks the target first if it has one of these statuses")]
    ConfigWindow_List_PriorityDesc,

    [Description("Dispellable debuffs list")]
    ConfigWindow_List_DangerousStatusDesc,

    [Description("Do no action if you have one of these debuffs")]
    ConfigWindow_List_NoCastingStatusDesc,

    [Description("Copy to Clipboard")]
    ConfigWindow_Actions_Copy,

    [Description("From Clipboard")]
    ActionSequencer_FromClipboard,

    [Description("Open the timeline link")]
    Timeline_OpenLink,

    [Description("Add Status")]
    ConfigWindow_List_AddStatus,

    [Description("Action name or id")]
    ConfigWindow_List_ActionNameOrId,

    [Description("Tank Buster")]
    ConfigWindow_List_HostileCastingTank,

    [Description("AoE")]
    ConfigWindow_List_HostileCastingArea,

    [Description("Knockback")]
    ConfigWindow_List_HostileCastingKnockback,

    [Description("Use tank personal damage mitigation abilities if the target is casting any of these actions")]
    ConfigWindow_List_HostileCastingTankDesc,

    [Description("Use AoE damage mitigation abilities if the target is casting any of these actions")]
    ConfigWindow_List_HostileCastingAreaDesc,

    [Description("Use knockback prevention abilities if the target is casting any of these actions")]
    ConfigWindow_List_HostileCastingKnockbackDesc,

    [Description("Add Action")]
    ConfigWindow_List_AddAction,

    [Description("Don't target")]
    ConfigWindow_List_NoHostile,

    [Description("Don't provoke")]
    ConfigWindow_List_NoProvoke,

    [Description("Beneficial AoE locations")]
    ConfigWindow_List_BeneficialPositions,

    [Description("Enemies that will never be targeted.")]
    ConfigWindow_List_NoHostileDesc,

    [Description("The name of the enemy that you don't want to be targeted")]
    ConfigWindow_List_NoHostilesName,

    [Description("Enemies that will never be provoked.")]
    ConfigWindow_List_NoProvokeDesc,

    [Description("The name of the enemy that you don't want to be provoked")]
    ConfigWindow_List_NoProvokeName,

    [Description("Add beneficial AoE location")]
    ConfigWindow_List_AddPosition,

    [Description("Time")]
    ConfigWindow_Timeline_Time,

    [Description("Name")]
    ConfigWindow_Timeline_Name,

    [Description("Actions")]
    ConfigWindow_Timeline_Actions,

    [Description("Ability")]
    ActionAbility,

    [Description("Friendly")]
    ActionFriendly,

    [Description("Attack")]
    ActionAttack,

    [Description("Normal Targets")]
    NormalTargets,

    [Description("Targets with Heal-over-Time")]
    HotTargets,

    [Description("HP for AoE healing oGCDs")]
    HpAoe0Gcd,

    [Description("HP for AoE healing GCDs")]
    HpAoeGcd,

    [Description("HP for ST healing oGCDs")]
    HpSingle0Gcd,

    [Description("HP for ST healing GCDs")]
    HpSingleGcd,

    [Description("No Move")]
    InfoWindowNoMove,

    [Description("Move")]
    InfoWindowMove,

    [Description("Search... ")]
    ConfigWindow_Searching,

    [Description("Timer")]
    ConfigWindow_Basic_Timer,

    [Description("Auto Switch")]
    ConfigWindow_Basic_AutoSwitch,

    [Description("Named Conditions")]
    ConfigWindow_Basic_NamedConditions,

    [Description("Others")]
    ConfigWindow_Basic_Others,

    [Description("The ping time.\nIn RSR, it means the time from sending the action request to receiving the using success message from the server.")]
    ConfigWindow_Basic_Ping,

    [Description("The Animation lock time from individual actions. Here is 0.6s for example.")]
    ConfigWindow_Basic_AnimationLockTime,

    [Description("The clicking duration, RSR will try to click at this moment.")]
    ConfigWindow_Basic_ClickingDuration,

    [Description("The ideal click time.")]
    ConfigWindow_Basic_IdealClickingTime,

    [Description("The real click time.")]
    ConfigWindow_Basic_RealClickingTime,

    [Description("Auto turn off conditions")]
    ConfigWindow_Basic_SwitchCancelConditionSet,

    [Description("Auto turn manual conditions")]
    ConfigWindow_Basic_SwitchManualConditionSet,

    [Description("Auto turn auto conditions")]
    ConfigWindow_Basic_SwitchAutoConditionSet,

    [Description("Condition Name")]
    ConfigWindow_Condition_ConditionName,

    [Description("Information")]
    ConfigWindow_UI_Information,

    [Description("Overlay")]
    ConfigWindow_UI_Overlay,

    [Description("Windows")]
    ConfigWindow_UI_Windows,

    [Description("Change the way that RSR automatically uses actions.")]
    ConfigWindow_Auto_Description,

    [Description("Action Usage")]
    ConfigWindow_Auto_ActionUsage,

    [Description("Which actions RSR can use.")]
    ConfigWindow_Auto_ActionUsage_Description,

    [Description("Action Condition")]
    ConfigWindow_Auto_ActionCondition,

    [Description("Healing Condition")]
    ConfigWindow_Auto_HealingCondition,

    [Description("How RSR should use healing abilities")]
    ConfigWindow_Auto_HealingCondition_Description,

    [Description("State Condition")]
    ConfigWindow_Auto_StateCondition,

    [Description("Heal Area Forced Condition")]
    ConfigWindow_Auto_HealAreaConditionSet,

    [Description("Heal Single Forced Condition")]
    ConfigWindow_Auto_HealSingleConditionSet,

    [Description("Defense Area Forced Condition")]
    ConfigWindow_Auto_DefenseAreaConditionSet,

    [Description("Defense Single Forced Condition")]
    ConfigWindow_Auto_DefenseSingleConditionSet,

    [Description("Dispel Stance Positional Forced Condition")]
    ConfigWindow_Auto_DispelStancePositionalConditionSet,

    [Description("Raise Shirk Forced Condition")]
    ConfigWindow_Auto_RaiseShirkConditionSet,

    [Description("Move Forward Forced Condition")]
    ConfigWindow_Auto_MoveForwardConditionSet,

    [Description("Move Back Forced Condition")]
    ConfigWindow_Auto_MoveBackConditionSet,

    [Description("Anti Knockback Forced Condition")]
    ConfigWindow_Auto_AntiKnockbackConditionSet,

    [Description("Speed Forced Condition")]
    ConfigWindow_Auto_SpeedConditionSet,

    [Description("Limit Break Forced Condition")]
    ConfigWindow_Auto_LimitBreakConditionSet,

    [Description("This will change the way that RSR uses actions.")]
    ConfigWindow_Auto_ActionCondition_Description,

    [Description("Configuration")]
    ConfigWindow_Target_Config,

    [Description("Hostile")]
    ConfigWindow_List_Hostile,

    [Description("Enemy targeting logic. Adding more options cycles them when using /rotation Auto.")]
    ConfigWindow_Param_HostileDesc,

    [Description("Move Up")]
    ConfigWindow_Actions_MoveUp,

    [Description("Move Down")]
    ConfigWindow_Actions_MoveDown,

    [Description("Hostile target selection condition")]
    ConfigWindow_Param_HostileCondition,

    [Description("RSR focuses on the rotation itself. These are side features. If there are some other plugins can do that, these features will be deleted.")]
    ConfigWindow_Extra_Description,

    [Description("Event")]
    ConfigWindow_EventItem,

    [Description("Others")]
    ConfigWindow_Extra_Others,

    [Description("Add Events")]
    ConfigWindow_Events_AddEvent,

    [Description("In this window, you can set what macro will be trigger after using an action.")]
    ConfigWindow_Events_Description,

    [Description("Duty Start: ")]
    ConfigWindow_Events_DutyStart,

    [Description("Duty End: ")]
    ConfigWindow_Events_DutyEnd,

    [Description("Delete Event")]
    ConfigWindow_Events_RemoveEvent,

    [Description("Click to make it reverse.\nIs reversed : {0}")]
    ActionSequencer_NotDescription,

    [Description("Member Name")]
    ConfigWindow_Actions_MemberName,

    [Description("Rotation is null, please login or switch the job!")]
    ConfigWindow_Condition_RotationNullWarning,

    [Description("Delay its turning to true.")]
    ActionSequencer_Delay_Description,

    [Description("Delay its turning.")]
    ActionSequencer_Offset_Description,

    [Description("Enough Level")]
    ActionConditionType_EnoughLevel,

    [Description("Time Offset")]
    ActionSequencer_TimeOffset,

    [Description("Charges")]
    ActionSequencer_Charges,

    [Description("Original")]
    ActionSequencer_Original,

    [Description("Adjusted")]
    ActionSequencer_Adjusted,

    [Description("{0}'s target")]
    ActionSequencer_ActionTarget,

    [Description("From All")]
    ActionSequencer_StatusAll,

    [Description("From Self")]
    ActionSequencer_StatusSelf,

    [Description("You'd better not use it. Because this target isn't the action's target. Try to pick it from action.")]
    ConfigWindow_Condition_TargetWarning,

    [Description("Territory Name")]
    ConfigWindow_Condition_TerritoryName,

    [Description("Duty Name")]
    ConfigWindow_Condition_DutyName,

    [Description("Please separately keybind damage reduction / shield cooldowns in case RSR fails at a crucial moment in {0}!")]
    HighEndWarning,

    [Description("Avarice addon was not detected, please install it if you want to get the positional indicators for RSRs overlay!")]
    AvariceWarning,

    [Description("TextToTalk addon was not detected, please install it to make RSR give audio notifications!")]
    TextToTalkWarning,

    [Description("Use Forced Enable Condition")]
    ForcedEnableCondition,

    [Description("The conditions of forced to make it true.")]
    ForcedEnableConditionDesc,

    [Description("Use Forced Disable Condition")]
    ForcedDisableCondition,

    [Description("The conditions of forced to make it false.")]
    ForcedDisableConditionDesc,

    [Description("Click to execute the command")]
    ConfigWindow_Helper_RunCommand,

    [Description("Right-click to copy the command")]
    ConfigWindow_Helper_CopyCommand,

    [Description("Macro No.")]
    ConfigWindow_Events_MacroIndex,

    [Description("Is Shared")]
    ConfigWindow_Events_ShareMacro,

    [Description("Action Name")]
    ConfigWindow_Events_ActionName,

    [Description("Modify {0} to {1}")]
    CommandsChangeSettingsValue,

    [Description("Failed to find the config in this rotation, please check it.")]
    CommandsCannotFindConfig,

    [Description("Will use it within {0}s")]
    CommandsInsertAction,

    [Description("Can not find the action, please check the action name.")]
    CommandsInsertActionFailure,

    [Description("Failed to get both value and config from string. Please make sure you provide both a config option and value")]
    CommandsMissingArgument,

    [Description("Start ")]
    SpecialCommandType_Start,

    [Description("Cancel")]
    SpecialCommandType_Cancel,

    [Description("Heal Area")]
    SpecialCommandType_HealArea,

    [Description("Heal Single")]
    SpecialCommandType_HealSingle,

    [Description("Defense Area")]
    SpecialCommandType_DefenseArea,

    [Description("Defense Single")]
    SpecialCommandType_DefenseSingle,

    [Description("Tank Stance")]
    SpecialCommandType_TankStance,

    [Description("Dispel")]
    SpecialCommandType_Dispel,

    [Description("Positional")]
    SpecialCommandType_Positional,

    [Description("Shirk")]
    SpecialCommandType_Shirk,

    [Description("Raise")]
    SpecialCommandType_Raise,

    [Description("Move Forward")]
    SpecialCommandType_MoveForward,

    [Description("Move Back")]
    SpecialCommandType_MoveBack,

    [Description("Anti Knockback")]
    SpecialCommandType_AntiKnockback,

    [Description("Burst")]
    SpecialCommandType_Burst,

    [Description("End Special")]
    SpecialCommandType_EndSpecial,

    [Description("Speed")]
    SpecialCommandType_Speed,

    [Description("Limit Break")]
    SpecialCommandType_LimitBreak,

    [Description("No Casting")]
    SpecialCommandType_NoCasting,

    [Description("Auto Target ")]
    SpecialCommandType_Smart,

    [Description("Manual Target")]
    SpecialCommandType_Manual,

    [Description("Off")]
    SpecialCommandType_Off,

    [Description("Open config window.")]
    Commands_Rotation,

    [Description("Rotation Solver Reborn Settings v")]
    ConfigWindowHeader,

    [Description("This config is job specific")]
    JobConfigTip,

    [Description("This option is unavailable while using your current job\n \nRoles or jobs needed:\n{0}")]
    NotInJob,

    [Description("Raid Time")]
    TimelineRaidTime,

    [Description("Execute")]
    TimelineExecute,

    [Description("Position")]
    TimelinePosition,

    [Description("Duration")]
    TimelineDuration,

    [Description("Target Count")]
    TimelineTargetCount,

    [Description("Target Getter")]
    TimelineTargetGetter,

    [Description("Effect Duration")]
    TimelineEffectDuration,

    [Description("Scale")]
    TimelineScale,

    [Description("Showing Text")]
    TimelineShowText,

    [Description("Ground")]
    TimelineGround,

    [Description("Actor")]
    TimelineActor,

    [Description("Rotation")]
    TimelineRotation,

    [Description("Object Getter")]
    TimelineObjectGetter,

    [Description("Need a target")]
    TimelineNeedATarget,

    [Description("Target by target")]
    TimelineTargetByTarget,

    [Description("Position Offset")]
    TimelinePositionOffset,

    [Description("Corner")]
    TimelineCorner,

    [Description("Padding")]
    TimelinePadding,

    [Description("Color")]
    TimelineColor,

    [Description("Background Color")]
    TimelineBackgroundColor,

    [Description("Place On Target")]
    TimelinePlaceOnTarget,

    [Description("Add a timeline item.")]
    AddTimelineButton,

    [Description("Add a condition item.")]
    AddTimelineCondition,

    [Description("Add a drawing timeline item.")]
    AddDrawingTimelineButton,

    [Description("Click to toggle the timeline item condition.")]
    TimelineItemCondition,

    [Description("The time before this action.")]
    TimelineItemTime,

    [Description("The duration of this action.")]
    TimelineItemDuration,

    [Description("Welcome to Rotation Solver Reborn!")]
    WelcomeWindow_Header,

    [Description("Here's what you missed since the last time you were here")]
    WelcomeWindow_WelcomeBack,

    [Description("It looks like you might be new here! Let's get you started!")]
    WelcomeWindow_Welcome,

    [Description("Rotation Solver Reborn does not come with rotations out of the box, but for your convenience a link to a set of rotations maintained by the Combat Reborn team is included by default.")]
    WelcomeWindow_FirstTime,

    [Description("Would you like to install the default rotations now?")]
    WelcomeWindow_FirstTime2,

    [Description("Some other settings you may want to consider:")]
    WelcomeWindow_FirstTime3,

    [Description("Click here to install")]
    WelcomeWindow_SaveAndInstall,

    [Description("Recent Changes:")]
    WelcomeWindow_Changelog,

    [Description("Do you want your rotations to update and reload automatically upon login?")]
    WelcomeWindow_LoadAtStartup,

    [Description("Do you want to automatically reload local rotations when they are updated? (Developer Mode)")]
    WelcomeWindow_AutoReload,
}
