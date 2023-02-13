//本插件不得以任何形式在国服中使用。

using System.Collections.Generic;

namespace RotationSolver.Localization;

internal partial class Strings
{
    #region Commands
    public string Commands_Rotation { get; set; } = "Open config window.";
    public string Commands_ChangeAutoBurst { get; set; } = "Modify automatic burst to {0}";
    public string Commands_ChangeRotationConfig { get; set; } = "Modify {0} to {1}";
    public string Commands_CannotFindRotationConfig { get; set; } = "Failed to find the config in this rotation, please check it.";

    public string Commands_InsertAction { get; set; } = "Will use it within {0}s";

    public string Commands_InsertActionFailure { get; set; } = "Can not find the action, please check the action name.";
    public string Commands_SayHelloToAuthor { get; set; } = "This \"{0}\" is probably one of the authors of the \"Rotation Solver\", so say hello to him!";

    #endregion

    #region ConfigWindow
    public string ConfigWindow_Header { get; set; } = "Rotation Solver Settings v";
    public string ConfigWindow_RotationItem { get; set; } = "Rotation";
    public string ConfigWindow_ParamItem { get; set; } = "Param";
    public string ConfigWindow_EventItem { get; set; } = "Event";
    public string ConfigWindow_ActionItem { get; set; } = "Action";
    public string ConfigWindow_HelpItem { get; set; } = "Help";

    public string ConfigWindow_ActionItem_Description { get; set; }
        = "Modify the usage for each action.";

    public string ConfigWindow_HelpItem_Description { get; set; }
        = "In this window, you can see all Rotation Solver built-in commands for combat. ";

    public string Configwindow_HelpItem_AttackSmart { get; set; }
        = "Start attacking in smart mode(auto-targeting) when out of combat, otherwise switch the target according to the conditions.";

    public string Configwindow_HelpItem_AttackManual { get; set; }
        = "Start attacking in manual mode.";

    public string Configwindow_HelpItem_AttackCancel { get; set; }
        = "Stop attacking. Remember to turn it off when not in use!";

    public string Configwindow_HelpItem_HealArea { get; set; }
        = "Open a window to use one or more AoE heal.";

    public string Configwindow_HelpItem_HealSingle { get; set; }
        = "Open a window to use one or more single heal.";

    public string Configwindow_HelpItem_DefenseArea { get; set; }
        = "Open a window to use one or more AoE defense.";

    public string Configwindow_HelpItem_DefenseSingle { get; set; }
        = "Open a window to use one or more single defense.";

    public string Configwindow_HelpItem_EsunaShield { get; set; }
        = "Open a window to use Esuna,tank stance actions or True North.";

    public string Configwindow_HelpItem_RaiseShirk { get; set; }
        = "Open a window to use Raise or Shirk.";

    public string Configwindow_HelpItem_AntiKnockback { get; set; }
        = "Open a window to use knockback-penalty actions.";

    public string Configwindow_HelpItem_Break { get; set; }
        = "Open a window to use break-combo.";

    public string Configwindow_HelpItem_MoveForward { get; set; }
        = "Open a window to move forward.";

    public string Configwindow_HelpItem_MoveBack { get; set; }
        = "Open a window to move back.";

    public string Configwindow_HelpItem_EndSpecial { get; set; }
    = "Close special status";
    public string Configwindow_Helper_SwitchRotation { get; set; } = "Click to switch authors";
    public string Configwindow_Helper_GameVersion { get; set; } = "Game Version";
    public string Configwindow_Helper_OpenSource { get; set; } = "Open the source code URL";
    public string Configwindow_Helper_RunCommand { get; set; } = "Click to execute the command";
    public string Configwindow_Helper_CopyCommand { get; set; } = "Right-click to copy command";
    public string Configwindow_Helper_InsertCommand { get; set; } = "Insert \"{0}\" first in 5s";
    public string Configwindow_Rotation_Description { get; set; } = "You can enable the function for each job you want and configure the setting about how to use actions.";
    public string Configwindow_Rotation_KeyName { get; set; } = "The key name is";
    public string Configwindow_Events_AddEvent { get; set; } = "AddEvents";
    public string Configwindow_Events_Description { get; set; } = "In this window, you can set what macro will be trigger after using an action.";
    public string Configwindow_Events_ActionName { get; set; } = "Action Name";
    public string Configwindow_Events_MacroIndex { get; set; } = "Macro No.";
    public string Configwindow_Events_ShareMacro { get; set; } = "Is Shared";
    public string Configwindow_Events_RemoveEvent { get; set; } = "Delete Event";
    public string Configwindow_Events_DutyStart { get; set; } = "Duty Start: ";
    public string Configwindow_Events_DutyEnd{ get; set; } = "Duty End: ";
    public string Configwindow_Params_Description { get; set; } = "In this window, you can set the parameters about the using way of actions.";
    public string Configwindow_Param_NeverReplaceIcon { get; set; } = "Never Replace Icons";
    public string Configwindow_Param_NeverReplaceIconDesc { get; set; } = "Icon replacement: Repose is automatically displayed as the next skill to be used";
    public string Configwindow_Param_UseOverlayWindow { get; set; } = "Display top overlay";
    public string Configwindow_Param_UseOverlayWindowDesc { get; set; } = "This window is currently used to cue the body position in advance.";
    public string Configwindow_Param_Basic { get; set; } = "Basic";

    public string Configwindow_Param_WeaponFaster { get; set; } = "Set the time advance of use actions";
    public string Configwindow_Param_WeaponInterval { get; set; } = "Set the interval between abilities using";
    public string Configwindow_Param_SpecialDuration { get; set; } = "Set the duration of special windows set by commands";
    public string Configwindow_Param_AddDotGCDCount { get; set; } = "Set GCD advance of DOT refresh";
    public string Configwindow_Param_AutoOffBetweenArea { get; set; } = "Turn off when player is between area.";

    public string Configwindow_Param_UseWorkTask { get; set; } = "Use work task for acceleration.";
    public string Configwindow_Param_Delay { get; set; } = "Delay";

    public string Configwindow_Param_WeaponDelay { get; set; } = "Set the range of random delay for GCD in second.";
    public string Configwindow_Param_DeathDelay { get; set; } = "Set the range of random delay for raising deaths in second.";
    public string Configwindow_Param_HostileDelay { get; set; } = "Set the range of random delay for finding hostile targets in second.";
    public string Configwindow_Param_InterruptDelay { get; set; } = "Set the range of random delay for interrupting hostile targets in second.";
    public string Configwindow_Param_WeakenDelay { get; set; } = "Set the range of random delay for esuna weakens in second.";

    public string Configwindow_Param_HealDelay { get; set; } = "Set the range of random delay for healing people in second.";

    public string Configwindow_Param_NotInCombatDelay { get; set; } = "Set the range of random delay for Not In Combat in second.";
    public string Configwindow_Param_StopCastingDelay { get; set; } = "Set the range of random delay for stoping casting when target is dead in second.";
    public string Configwindow_Param_WorkTaskDelay { get; set; } = "Set the work task delay in millisecond. Smaller, more precise, more resource-intensive";

    public string Configwindow_Param_Display { get; set; } = "Display";
    public string Configwindow_Param_Advanced { get; set; } = "Advanced";
    public string Configwindow_Param_PoslockCasting { get; set; } = "Lock the movement when casting.";
    public string Configwindow_Param_UseStopCasting { get; set; } = "Use stopping casting when target is dead.";
    public string Configwindow_Param_PoslockModifier { get; set; } = "Set the modifier key to unlock the movement temporary";
    public string Configwindow_Param_PoslockDescription { get; set; } = "LT is for gamepad player";
    public string Configwindow_Param_CastingDisplay { get; set; } = "Enhance castbar with casting status";
    public string Configwindow_Param_TeachingMode { get; set; } = "Teaching mode";
    public string Configwindow_Param_TeachingModeColor { get; set; } = "Prompt box color of teaching mode";
    public string Configwindow_Param_MovingTargetColor { get; set; } = "Prompt box color of moving target";
    public string Configwindow_Param_TargetColor { get; set; } = "Target color";
    public string Configwindow_Params_SubTargetColor { get; set; } = "Sub-target color";
    public string Configwindow_Param_KeyBoardNoise { get; set; } = "Simulate the effect of pressing";
    public string Configwindow_Params_VoiceVolume { get; set; } = "Voice volume";
    public string Configwindow_Param_FlytextPositional { get; set; } = "Hint positional anticipation by flytext";
    public string Configwindow_Param_SayPositional { get; set; } = "Hint positional anticipation by shouting";
    public string Configwindow_Param_PositionalFeedback { get; set; } = "Positional error feedback";
    public string Configwindow_Param_ShowMoveTarget { get; set; } = "Show the pointing target of the move skill";
    public string Configwindow_Param_ShowTarget { get; set; } = "Show Target";
    public string Configwindow_Param_PositionalFeedbackDesc { get; set; } = "Attention: Positional anticipation is experimental, just for reference only.";
    public string Configwindow_Param_PositionaErrorText { get; set; } = "Positional error prompt";
    public string Configwindow_Params_LocationWrongTextDesc { get; set; } = "How do you want to be scolded if you have a positional error ?!";
    public string Configwindow_Param_SayOutStateChanged { get; set; } = "Saying the state changes out";
    public string Configwindow_Param_ShowInfoOnDtr { get; set; } = "Display plugin state on dtrbar";

    public string Configwindow_Param_ShowWorkTaskFPS { get; set; } = "Display Task FPS on dtrbar";

    public string Configwindow_Param_ShowInfoOnToast { get; set; } = "Display plugin state changed on toast";
    public string Configwindow_Param_Action { get; set; } = "Action";
    public string Configwindow_Param_UseAOEWhenManual { get; set; } = "Use AOE actions in manual mode";
    public string Configwindow_Param_AutoBurst { get; set; } = "Automatic burst";
    public string Configwindow_Param_UseAbility { get; set; } = "Auto-use abilities";
    public string Configwindow_Param_NoNewHostiles { get; set; } = "Don't attack new mobs by aoe";
    public string Configwindow_Params_NoNewHostilesDesc { get; set; } = "Nerver use any AOE action when this action may attack the mobs that not is a hostile target.";
    public string Configwindow_Param_UseDefenceAbility { get; set; } = "Use defence abilities";
    public string Configwindow_Param_UseDefenceAbilityDesc { get; set; } = "It is recommended to check this option if you are playing Raids./nPlan the heal and defense by yourself.???";
    public string Configwindow_Param_AutoShield { get; set; } = "Auto tank stance";
    public string Configwindow_Param_AutoProvokeForTank { get; set; } = "Auto Provoke (Tank)";
    public string Configwindow_Param_AutoProvokeForTankDesc { get; set; } = "When a hostile is hitting the non-Tank member of party, it will automatically use the Provoke.";
    public string Configwindow_Param_AutoUseTrueNorth { get; set; } = "Auto TrueNorth (Melee)";
    public string Configwindow_Param_RaisePlayerBySwift { get; set; } = "Raise player by swift";
    public string Configwindow_Param_UseGroundBeneficialAbility { get; set; } = "Use beneficaial ground-targeted actions";
    public string Configwindow_Param_RaisePlayerByCasting { get; set; } = "Raise player by casting when swift is in cooldown";
    public string Configwindow_Param_UseHealWhenNotAHealer { get; set; } = "Use heal when not-healer";
    public string Configwindow_Param_LessMPNoRaise { get; set; } = "Nerver raise player if MP is less than the set value";
    public string Configwindow_Param_UseItem { get; set; } = "Use items";
    public string Configwindow_Param_UseItemDesc { get; set; } = "Use poison, WIP";
    public string Configwindow_Param_Conditon { get; set; } = "Condition";
    public string Configwindow_Param_StartOnCountdown { get; set; } = "Turn on auto-rotation on countdown";
    public string Configwindow_Param_EsunaAll { get; set; } = "Esuna All Statuses.";
    public string Configwindow_Param_InterruptibleMoreCheck { get; set; } = "Interrupt the action with action type check.";

    public string Configwindow_Param_HealOutOfCombat { get; set; } = "Heal party members outside of combat.";

    public string Configwindow_Param_HealthDifference { get; set; } = "Set the HP standard deviation threshold for using AOE heal (ability & spell)";
    public string Configwindow_Param_HealthAreaAbility { get; set; } = "Set the HP threshold for using AOE healing ability";
    public string Configwindow_Param_HealthAreaSpell { get; set; } = "Set the HP threshold for using AOE healing spell";
    public string Configwindow_Param_HealingOfTimeSubtractArea { get; set; } = "Set the HP threshold reduce with hot effect(AOE)";
    public string Configwindow_Param_HealthSingleAbility { get; set; } = "Set the HP threshold for using single healing ability";
    public string Configwindow_Param_HealthSingleSpell { get; set; } = "Set the HP threshold for using single healing spell";
    public string Configwindow_Param_HealingOfTimeSubtractSingle { get; set; } = "Set the HP threshold reduce with hot effect(single)";
    public string Configwindow_Param_HealthForDyingTank { get; set; } = "Set the HP threshold for tank to use invincibility";
    public string Configwindow_Param_Target { get; set; } = "Target";
    public string Configwindow_Param_RightNowTargetToHostileType { get; set; } = "Hostile target filtering condition";
    public string Configwindow_Param_TargetToHostileType1 { get; set; } = "All targets can attack";
    public string Configwindow_Param_TargetToHostileType2 { get; set; } = "Targets have a target or all targets can attack";
    public string Configwindow_Param_TargetToHostileType3 { get; set; } = "Targets have a target";
    public string Configwindow_Param_NoticeUnexpectedCombat { get; set; } = "NOTICE: You are not turn the auto off between area on. It may start a combat unexpectedly.";
    public string Configwindow_Param_AddEnemyListToHostile { get; set; } = "Add Enemies list to the hostile target.";
    public string Configwindow_Param_ChooseAttackMark { get; set; } = "Priority attack targets with attack markers";
    public string Configwindow_Param_CanAttackMarkAOE { get; set; } = "Forced use of AOE";
    public string Configwindow_Param_AttackMarkAOEDesc { get; set; } = "Attention: Checking this option , AA will attack as many hostile targets as possible, while ignoring whether the attack will cover the marked target.";
    public string Configwindow_Param_FilterStopMark { get; set; } = "Never attack targets with stop markers";
    public string Configwindow_Param_ObjectMinRadius { get; set; } = "Set the minimum target circle threshold possessed by the attack target";
    public string Configwindow_Param_MoveTargetAngle { get; set; } = "The size of the sector angle that can be selected as the moveable target";
    public string Configwindow_Param_MoveTargetAngleDesc { get; set; } = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are movable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.";
    public string Configwindow_Param_ChangeTargetForFate { get; set; } = "Select only Fate targets in Fate";
    public string Configwindow_Param_MoveTowardsScreen { get; set; } = "Using movement actions towards the object in the center of the screen";
    public string Configwindow_Param_MoveTowardsScreenDesc { get; set; } = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.";
    public string Configwindow_Param_RaiseAll { get; set; } = "Raise all (include passerby)";
    public string Configwindow_Param_RaiseBrinkofDeath { get; set; } = "Raise player even has Brink of Death";
    public string Configwindow_Param_MoveAreaActionFarthest { get; set; } = "Moving Area Ability to farthest";
    public string Configwindow_Param_MoveAreaActionFarthestDesc { get; set; } = "Move to the furthest position from character's face direction.";
    public string Configwindow_Param_Hostile { get; set; } = "Hostile";
    public string Configwindow_Param_HostileDesc { get; set; } = "You can set the logic of hostile target selection to allow flexibility in switching the logic of selecting hostile in battle.";
    public string Configwindow_Param_AddHostileCondition { get; set; } = "Add selection condition";
    public string Configwindow_Param_HostileCondition { get; set; } = "Hostile target selection condition";
    public string Configwindow_Param_ConditionUp { get; set; } = "Up";
    public string Configwindow_Param_ConditionDown { get; set; } = "Down";
    public string Configwindow_Param_ConditionDelete { get; set; } = "Delete";
    #endregion

    #region ScriptWindow
    public string Timeline_DragdropDescription { get; set; } = "Drag&drop to move，Ctrl+Alt+RightClick to delete.";
    public string Timeline_SearchBar { get; set; } = "Search Bar";
    public string Timeline_MustUse { get; set; } = "MustUse";
    public string Timeline_MustUseDesc { get; set; } = "Skip AOE and Buff.";
    public string Timeline_Empty { get; set; } = "UseUp";
    public string Timeline_EmptyDesc { get; set; } = "UseUp or Skip Combo";
    public string Timeline_TimelineDescription { get; set; } = "Add some condition to automatic use this action.";
    public string Timeline_Can { get; set; } = "Can";
    public string Timeline_Cannot { get; set; } = "Cannot";
    public string Timeline_Is { get; set; } = "Is";
    public string Timeline_Isnot { get; set; } = "Isnot";
    public string Timeline_Have { get; set; } = "Have";
    public string Timeline_Havenot { get; set; } = "Havenot";
    public string Timeline_Ability { get; set; } = "Ability";
    public string Timeline_Charges { get; set; } = "Charges";
    public string Timeline_ConditionSet { get; set; } = "ConditionSet";
    public string Timeline_ActionCondition { get; set; } = "ActionCondition";
    public string Timeline_TargetCondition { get; set; } = "TargetCondition";
    public string Timeline_RotationCondition { get; set; } = "RotationCondition";
    public string Timeline_ActionTarget { get; set; } = "{0}'s target";
    public string Timeline_Target { get; set; } = "Target";
    public string Timeline_Player { get; set; } = "Player";
    public string Timeline_StatusSelf { get; set; } = "StatusSelf";
    public string Timeline_StatusSelfDesc { get; set; } = "StatusSelf";
    #endregion

    #region Actions
    public string Action_Friendly { get; set; } = "Support";
    public string Action_Attack { get; set; } = "Attack";
    #endregion

    #region ComboConditionType
    public string ComboConditionType_Bool { get; set; } = "Boolean";
    public string ComboConditionType_Byte { get; set; } = "Byte";
    public string ComboConditionType_Time { get; set; } = "Time";
    public string ComboConditionType_GCD { get; set; } = "GCD";
    public string ComboConditionType_Last { get; set; } = "Last";
    #endregion

    #region TargetingType
    public string TargetingType_Big { get; set; } = "Big";
    public string TargetingType_Small { get; set; } = "Small";
    public string TargetingType_HighHP { get; set; } = "High HP";
    public string TargetingType_LowHP { get; set; } = "Low HP";
    public string TargetingType_HighMaxHP { get; set; } = "High Max HP";
    public string TargetingType_LowMaxHP { get; set; } = "Low Max HP";
    #endregion

    #region SpecialCommandTypeSayout
    public string SpecialCommandType_Start { get; set; } = "Start ";

    public string SpecialCommandType_HealArea { get; set; } = "Heal Area";
    public string SpecialCommandType_HealSingle { get; set; } = "Heal Single";
    public string SpecialCommandType_DefenseArea { get; set; } = "Defense Area";
    public string SpecialCommandType_DefenseSingle { get; set; } = "Defense Single";
    public string SpecialCommandType_Shield { get; set; } = "Shield";
    public string SpecialCommandType_MoveForward { get; set; } = "Move Forward";
    public string SpecialCommandType_MoveBack { get; set; } = "Move Back";
    public string SpecialCommandType_AntiKnockback { get; set; } = "Anti-Knockback";
    public string SpecialCommandType_Burst { get; set; } = "Burst";
    public string SpecialCommandType_EndSpecial { get; set; } = "End Special";
    public string SpecialCommandType_Smart { get; set; } = "Smart ";
    public string SpecialCommandType_Manual { get; set; } = "Manual";
    public string SpecialCommandType_Cancel { get; set; } = "Cancel";
    public string SpecialCommandType_Off { get; set; } = "Off";
    #endregion

    #region ActionConditionType
    public string ActionConditionType_Elapsed { get; set; } = "Elapsed";
    public string ActionConditionType_ElapsedGCD { get; set; } = "ElapsedGCD ";
    public string ActionConditionType_Remain { get; set; } = "RemainTime";
    public string ActionConditionType_RemainGCD { get; set; } = "RemainGCD";
    public string ActionConditionType_ShouldUse { get; set; } = "ShouldUse";
    public string ActionConditionType_EnoughLevel { get; set; } = "EnoughLevel";
    public string ActionConditionType_IsCoolDown { get; set; } = "IsCoolDown";
    public string ActionConditionType_CurrentCharges { get; set; } = "CurrentCharges";
    public string ActionConditionType_MaxCharges { get; set; } = "MaxCharges";
    #endregion

    #region TargetConditionType
    public string TargetConditionType_HaveStatus { get; set; } = "Have Status";
    public string TargetConditionType_IsDying { get; set; } = "Is Dying";
    public string TargetConditionType_IsBoss { get; set; } = "Is Boss";
    public string TargetConditionType_Distance { get; set; } = "Distance";
    public string TargetConditionType_StatusEnd { get; set; } = "Status End";
    public string TargetConditionType_StatusEndGCD { get; set; } = "Status End GCD";
    public string TargetConditionType_CastingAction { get; set; } = "Casting Action";

    #endregion

    #region DescType
    public string DescType_Description { get; set; } = "Loop Description";
    public string DescType_BreakingAction { get; set; } = "Burst Skills";
    public string DescType_HealArea { get; set; } = "Range Healing";
    public string DescType_HealSingle { get; set; } = "Mono Healing";
    public string DescType_DefenseArea { get; set; } = "Range Defense";
    public string DescType_DefenseSingle { get; set; } = "Mono Defense";
    public string DescType_MoveAction { get; set; } = "Move Skills";

    #endregion

    #region JobRole
    public string JobRole_None { get; set; } = "Gathering&Production";
    public string JobRole_Tank { get; set; } = "Tank";
    public string JobRole_Melee { get; set; } = "Melee";
    public string JobRole_Ranged { get; set; } = "Ranged";
    public string JobRole_Healer { get; set; } = "Healer";
    public string JobRole_RangedPhysical { get; set; } = "Ranged Physical";
    public string JobRole_RangedMagicial { get; set; } = "Ranged Magicial";
    public string JobRole_DiscipleoftheLand { get; set; } = "Disciple of the Land";
    public string JobRole_DiscipleoftheHand { get; set; } = "Disciple of the Hand";

    #endregion

    #region EnemyLocation
    public string EnemyLocation_None { get; set; } = "None";
    public string EnemyLocation_Rear { get; set; } = "Rear";
    public string EnemyLocation_Flank { get; set; } = "Flank";
    public string EnemyLocation_Front { get; set; } = "Front";

    #endregion
    public Dictionary<string, string> MemberInfoName { get; set; } = new Dictionary<string, string>()
    {
        #region Rotation
        { "IsMoving", "IsMoving"},
        { "HaveHostilesInRange", "Have Hostiles InRange"},
        { "IsFullParty", "Is Full Party"},
        { "SettingBreak", "Breaking"},
        { "Level", "Level"},
        { "InCombat", "In Combat"},
        { "IsLastGCD", "Just used GCD"},
        { "IsLastAbility", "Just used Ability"},
        { "IsLastAction", "Just used Action"},
        { "IsTargetDying", "Target is dying"},
        { "IsTargetBoss", "Target is Boss"},
        { "HaveSwift", "Have Swift"},
        { "HaveShield", "Have defensive stance"},
        #endregion

        #region AST
        { "PlayCard", "Play"},
        #endregion

        #region BLM
        { "UmbralIceStacks", "Umbral Ice Stacks"},
        { "AstralFireStacks", "Astral Fire Stacks"},
        { "PolyglotStacks", "Polyglot Stacks"},
        { "UmbralHearts", "Umbral Heart Stacks"},
        { "IsParadoxActive", "Is Paradox Active ?"},
        { "InUmbralIce", "In Umbral Ice"},
        { "InAstralFire", "In Astral Fire"},
        { "IsEnochianActive", "Is Enochian Active?"},
        { "EnchinaEndAfter", "Enchina End After (s)"},
        { "EnchinaEndAfterGCD", "Enchina End After (GCDs)"},
        { "ElementTimeEndAfter", "Element Time End After (s)"},
        { "ElementTimeEndAfterGCD", "Element Time End After (GCDs)"},
        { "HasFire", "Has Firestarter"},
        { "HasThunder", "Has Thunder"},
        { "IsPolyglotStacksMaxed", "Whether Polyglot already has the maximum number of charge stacks at the current level"}, //这玩意儿太长了！
        #endregion

        #region BRD
        { "SoulVoice", "Soul Voice"},
        { "SongEndAfter", "Song End After (s)"},
        { "SongEndAfterGCD", "Song End After (GCDs)"},
        { "Repertoire", "Song Gauge Stacks"},
        #endregion

        #region DNC
        { "IsDancing", "Is Dancing"},
        { "Esprit", "Esprit"},
        { "Feathers", "Feathers"},
        { "CompletedSteps", "CompletedSteps"},
        { "FinishStepGCD", "FinishStepGCD"},
        { "ExcutionStepGCD", "Excution Step GCD"},
        #endregion

        #region DRG
        #endregion

        #region DRK
        { "Blood", "Blood"},
        { "HasDarkArts", "Has Dark Arts"},
        { "DarkSideEndAfter", "DarkSideEndAfter"},
        { "DarkSideEndAfterGCD", "DarkSideEndAfterGCD"},
        #endregion

        #region GNB
        { "Ammo", "Ammo"},
        { "AmmoComboStep", "Ammo Combo Step"},
        #endregion    

        #region MCH
        { "IsOverheated", "Is Over heated"},
        { "Heat", "Heat"},
        { "Battery", "Battery"},
        { "OverheatedEndAfter", "Over heated End After (s)"},
        { "OverheatedEndAfterGCD", "Over heated End After(GCDs)"},
        #endregion

        #region MNK
        { "Chakra", "Chakra"},
        #endregion        
    };

    public Dictionary<string, string> MemberInfoDesc { get; set; } = new Dictionary<string, string>()
    {
        #region Rotation
        { "IsMoving", "Player Is Moving"},
        { "HaveHostilesInRange", "Have Hostiles In Range(Melee <3m,Ranged<25m)"},
        { "IsFullParty", "Is Full Party"},
        { "SettingBreak", "In break"},
        { "Level", "Player level"},
        { "InCombat", "In Combat"},
        { "IsLastGCD", "Just used GCD"},
        { "IsLastAbility", "Just used ability"},
        { "IsLastAction", "Just used Action"},
        { "IsTargetDying", "Target is Dying"},
        { "IsTargetBoss", "Target is Boss"},
        { "HaveSwift", "Have Swift"},
        { "HaveShield", "Have defensive stance"},
        #endregion

        #region AST
        { "PlayCard", "Play"},
        #endregion

        #region BLM
        { "UmbralIceStacks", "Umbral Ice Stacks"},
        { "AstralFireStacks", "Astral Fire Stacks"},
        { "PolyglotStacks", "Polyglot Stacks"},
        { "UmbralHearts", "Umbral Heart Stacks"},
        { "IsParadoxActive", "Is Paradox Active?"},
        { "InUmbralIce", "In Umbral Ice"},
        { "InAstralFire", "In Astral Fire"},
        { "IsEnochianActive", "Is Enochian Active?"},
        { "EnchinaEndAfter", "Enchina End After (s)"},
        { "EnchinaEndAfterGCD", "Enchina End After (GCDs)"},
        { "ElementTimeEndAfter", "Element remaining time"},
        { "ElementTimeEndAfterGCD", "Element remaining time"},
        { "HasFire", "Has Firestarter"},
        { "HasThunder", "Has Thunder"},
        { "IsPolyglotStacksMaxed", "Whether Polyglot already has the maximum number of charge stacks at the current level"},
        #endregion

        #region BRD
        { "SoulVoice", "SoulVoice"},
        { "SongEndAfter", "Song End After (s)"},
        { "SongEndAfterGCD", "Song End After (GCDs)"},
        { "Repertoire", "Song Gauge Stacks"},
        #endregion

        #region DNC
        { "IsDancing", "Is Dancing"},
        { "Esprit", "Esprit"},
        { "Feathers", "Feathers"},
        { "CompletedSteps", "Completed Steps"},
        { "FinishStepGCD", "Finish Step GCD"},
        { "ExcutionStepGCD", "Excution Step GCD"},
        #endregion

        #region DRG
        #endregion

        #region DRK
        { "Blood", "Blood"},
        { "HasDarkArts", "Has Dark Arts"},
        { "DarkSideEndAfter", "DarkSide End After (s)"},
        { "DarkSideEndAfterGCD", "DarkSide End After (GCDs)"},
        #endregion

        #region GNB
        { "Ammo", "Ammo"},
        { "AmmoComboStep", "Ammo Combo Step"},
        #endregion

        #region MCH
        { "IsOverheated", "Is Over heated"},
        { "Heat", "Heat"},
        { "Battery", "Battery"},
        { "OverheatedEndAfter", "OverheatedEndAfter"},
        { "OverheatedEndAfterGCD", "OverheatedEndAfterGCD"},
        #endregion

        #region MNK
        { "Chakra", "Chakra"},
        #endregion        
    };

    public string HighEndWarning { get; set; } = "You'd better not use Rotation Solver in {0}!";
}
