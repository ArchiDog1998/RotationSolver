namespace RotationSolver.Localization;

internal partial class Strings
{
    #region Commands
    public string Commands_Rotation { get; set; } = "Open config window.";
    public string Commands_ChangeSettingsValue { get; set; } = "Modify {0} to {1}";
    public string Commands_ChangeRotationConfig { get; set; } = "Modify {0} to {1}";
    public string Commands_CannotFindRotationConfig { get; set; } = "Failed to find the config in this rotation, please check it.";

    public string Commands_InsertAction { get; set; } = "Will use it within {0}s";

    public string Commands_InsertActionFailure { get; set; } = "Can not find the action, please check the action name.";


    #endregion

    #region ConfigWindow
    public string ConfigWindow_Header { get; set; } = "Rotation Solver Settings v";
    public string ConfigWindow_RotationItem { get; set; } = "Rotation";
    public string ConfigWindow_ParamItem { get; set; } = "Param";
    public string ConfigWindow_EventItem { get; set; } = "Event";
    public string ConfigWindow_ActionItem { get; set; } = "Action";
    public string ConfigWindow_ControlItem { get; set; } = "Control";
    public string ConfigWindow_HelpItem { get; set; } = "Help";
    public string ConfigWindow_RotationDev { get; set; } = "RotationDev";

    public string ConfigWindow_ActionItem_Description { get; set; }
        = "Modify the usage for each action.";

    public string ConfigWindow_HelpItem_Description { get; set; }
        = "In this window, you can see all Rotation Solver built-in commands for combat. ";

    public string ConfigWindow_HelpItem_AttackSmart { get; set; }
        = "Start attacking in smart mode(auto-targeting) when out of combat, otherwise switch the target according to the conditions.";

    public string ConfigWindow_HelpItem_AttackManual { get; set; }
        = "Start attacking in manual mode. You need to choose the target manually.";

    public string ConfigWindow_HelpItem_AttackCancel { get; set; }
        = "Stop attacking. Remember to turn it off when not in use!";

    public string ConfigWindow_HelpItem_HealArea { get; set; }
        = "Open a window to use AoE heal.";

    public string ConfigWindow_HelpItem_HealSingle { get; set; }
        = "Open a window to use single heal.";

    public string ConfigWindow_HelpItem_DefenseArea { get; set; }
        = "Open a window to use AoE defense.";

    public string ConfigWindow_HelpItem_DefenseSingle { get; set; }
        = "Open a window to use single defense.";

    public string ConfigWindow_HelpItem_Esuna { get; set; }
        = "Open a window to use Esuna,tank stance actions or True North.";

    public string ConfigWindow_HelpItem_RaiseShirk { get; set; }
        = "Open a window to use Raise or Shirk.";

    public string ConfigWindow_HelpItem_AntiKnockback { get; set; }
        = "Open a window to use knockback-penalty actions.";

    public string ConfigWindow_HelpItem_Burst { get; set; }
        = "Open a window to burst.";

    public string ConfigWindow_HelpItem_MoveForward { get; set; }
        = "Open a window to move forward.";

    public string ConfigWindow_HelpItem_MoveBack { get; set; }
        = "Open a window to move back.";

    public string ConfigWindow_HelpItem_EndSpecial { get; set; }
        = "Close special window.";
    public string ConfigWindow_Helper_SwitchRotation { get; set; } = "Click to switch authors";
    public string ConfigWindow_Helper_HighEndWarning { get; set; } = "This <{0}> rotation is not allowed to be used in High-end Duty!";
    public string ConfigWindow_Helper_GameVersion { get; set; } = "Game Version";
    public string ConfigWindow_Helper_OpenSource { get; set; } = "Open the source code URL";
    public string ConfigWindow_Helper_RunCommand { get; set; } = "Click to execute the command";
    public string ConfigWindow_Helper_CopyCommand { get; set; } = "Right-click to copy command";
    public string ConfigWindow_Helper_InsertCommand { get; set; } = "Insert \"{0}\" first in {1}s";
    public string ConfigWindow_Rotation_Description { get; set; } = "You can enable the function for each job you want and configure the setting about how to use actions.";

    public string ConfigWindow_RotationDev_Description { get; set; } = "You can get some extra rotation development information here.";

    public string ConfigWindow_Rotation_KeyName { get; set; } = "The key name is";
    public string ConfigWindow_Events_AddEvent { get; set; } = "AddEvents";
    public string ConfigWindow_Events_Description { get; set; } = "In this window, you can set what macro will be trigger after using an action.";
    public string ConfigWindow_Events_ActionName { get; set; } = "Action Name";
    public string ConfigWindow_Events_MacroIndex { get; set; } = "Macro No.";
    public string ConfigWindow_Events_ShareMacro { get; set; } = "Is Shared";
    public string ConfigWindow_Events_RemoveEvent { get; set; } = "Delete Event";
    public string ConfigWindow_Events_DutyStart { get; set; } = "Duty Start: ";
    public string ConfigWindow_Events_DutyEnd{ get; set; } = "Duty End: ";
    public string ConfigWindow_Params_Description { get; set; } = "In this window, you can set the parameters about the using way of actions.";
    public string ConfigWindow_Param_UseOverlayWindow { get; set; } = "Display Top Overlay";
    public string ConfigWindow_Param_UseOverlayWindowDesc { get; set; } = "This top window is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.";
    public string ConfigWindow_Param_Basic { get; set; } = "Basic";

    public string ConfigWindow_Param_ActionAhead { get; set; } = "Set the time advance of using actions";
    public string ConfigWindow_Param_CountDownAhead { get; set; } = "Set the time advance of using casting actions on counting down.";
    public string ConfigWindow_Param_SpecialDuration { get; set; } = "Set the duration of special windows set by commands";
    public string ConfigWindow_Param_AddDotGCDCount { get; set; } = "Set GCD advance of DOT refresh";
    public string ConfigWindow_Param_AutoOffBetweenArea { get; set; } = "Auto turn off when player is between area.";

    public string ConfigWindow_Param_AutoOffCutScene { get; set; } = "Auto turn off during cut scene.";

    public string ConfigWindow_Param_AutoOffWhenDead { get; set; } = "Auto turn off when dead.";

    public string ConfigWindow_Param_UseWorkTask { get; set; } = "Use work task for acceleration.";
    public string ConfigWindow_Param_ToggleManual { get; set; } = "Make Manual Command as toggle.";
    public string ConfigWindow_Param_Delay { get; set; } = "Delay";

    public string ConfigWindow_Param_WeaponDelay { get; set; } = "Set the range of random delay for GCD in second.";
    public string ConfigWindow_Param_DeathDelay { get; set; } = "Set the range of random delay for raising deaths in second.";
    public string ConfigWindow_Param_HostileDelay { get; set; } = "Set the range of random delay for finding hostile targets in second.";
    public string ConfigWindow_Param_InterruptDelay { get; set; } = "Set the range of random delay for interrupting hostile targets in second.";
    public string ConfigWindow_Param_WeakenDelay { get; set; } = "Set the range of random delay for esuna weakens in second.";

    public string ConfigWindow_Param_HealDelay { get; set; } = "Set the range of random delay for healing people in second.";

    public string ConfigWindow_Param_NotInCombatDelay { get; set; } = "Set the range of random delay for Not In Combat in second.";

    public string ConfigWindow_Param_ClickingDelay { get; set; } = "Set the range of random delay for the interval of clicking actions.";
    public string ConfigWindow_Param_StopCastingDelay { get; set; } = "Set the range of random delay for stopping casting when target is no need to cast in second.";
    public string ConfigWindow_Param_Display { get; set; } = "Display";
    public string ConfigWindow_Param_Advanced { get; set; } = "Advanced";
    public string ConfigWindow_Param_PoslockCasting { get; set; } = "Lock the movement when casting.";
    public string ConfigWindow_Param_UseStopCasting { get; set; } = "Use stopping casting when target is dead.";
    public string ConfigWindow_Param_ShowActionFlag { get; set; } = "Show action flag.";
    public string ConfigWindow_Param_InDebug { get; set; } = "Debug Mode";

    public string ConfigWindow_Param_ShowHealthRatio { get; set; } = "Show the health ratio for the check of Boss, Dying, Dot.";

    public string ConfigWindow_Param_HealthRatioBoss { get; set; } = "If target's max health ratio is higher than this, regard it as Boss.";

    public string ConfigWindow_Param_HealthRatioDying { get; set; } = "If target's current health ratio is lower than this, regard it is dying.";
    public string ConfigWindow_Param_HealthRatioDot { get; set; } = "If target's current health ratio is higher than this, regard it can be dot.";
    public string ConfigWindow_Param_PoslockModifier { get; set; } = "Set the modifier key to unlock the movement temporary";
    public string ConfigWindow_Param_PoslockDescription { get; set; } = "LT is for gamepad player";
    public string ConfigWindow_Param_CastingDisplay { get; set; } = "Enhance cast bar with casting status";
    public string ConfigWindow_Param_TeachingMode { get; set; } = "Teaching mode";
    public string ConfigWindow_Param_TeachingModeColor { get; set; } = "Prompt box color of teaching mode";
    public string ConfigWindow_Param_MovingTargetColor { get; set; } = "Prompt box color of moving target";
    public string ConfigWindow_Param_TargetColor { get; set; } = "Target color";
    public string ConfigWindow_Param_SubTargetColor { get; set; } = "Sub-target color";
    public string ConfigWindow_Param_KeyBoardNoise { get; set; } = "Simulate the effect of pressing";
    public string ConfigWindow_Param_KeyBoardNoiseBefore { get; set; } = "Simulate before action cast";
    public string ConfigWindow_Param_KeyBoardNoiseTimes { get; set; } = "Effect times";
    public string ConfigWindow_Param_VoiceVolume { get; set; } = "Voice volume";
    public string ConfigWindow_Param_ToastPositional { get; set; } = "Hint positional anticipation by Toast";
    public string ConfigWindow_Param_SayPositional { get; set; } = "Hint positional anticipation by shouting";
    public string ConfigWindow_Param_PositionalFeedback { get; set; } = "Positional error feedback on flytext";
    public string ConfigWindow_Param_DrawPositional { get; set; } = "Draw Positional on the screen";
    public string ConfigWindow_Param_DrawMeleeRange { get; set; } = "Draw the range of melee on the screen";

    public string ConfigWindow_Param_DrawMeleeOffset { get; set; } = "Draw the offset of melee on the screen";
    public string ConfigWindow_Param_ShowMoveTarget { get; set; } = "Show the target of the move action";
    public string ConfigWindow_Param_ShowTarget { get; set; } = "Show Target";
    public string ConfigWindow_Param_PositionalFeedbackDesc { get; set; } = "Attention: Positional anticipation is experimental, just for reference only.";
    public string ConfigWindow_Param_PositionalErrorText { get; set; } = "Positional error prompt";
    public string ConfigWindow_Params_LocationWrongTextDesc { get; set; } = "How do you want to be scolded if you have a positional error ?!";
    public string ConfigWindow_Param_SayOutStateChanged { get; set; } = "Saying the state changes out";
    public string ConfigWindow_Param_ShowInfoOnDtr { get; set; } = "Display plugin state on server info";

    public string ConfigWindow_Param_ShowInfoOnToast { get; set; } = "Display plugin state on toast";
    public string ConfigWindow_Param_NamePlateIconId { get; set; } = "Player's name plate icon id when state is on. Recommend 61435, 61437";
    public string ConfigWindow_Param_Action { get; set; } = "Action";
    public string ConfigWindow_Param_UseAOEAction { get; set; } = "Use AOE actions";

    public string ConfigWindow_Param_UseAOEWhenManual { get; set; } = "Use AOE actions in manual mode";
    public string ConfigWindow_Param_AutoBurst { get; set; } = "Automatic burst";
    public string ConfigWindow_Param_UseAbility { get; set; } = "Auto-use abilities";
    public string ConfigWindow_Param_NoNewHostiles { get; set; } = "Don't attack new mobs by aoe";
    public string ConfigWindow_Params_NoNewHostilesDesc { get; set; } = "Never use any AOE action when this action may attack the mobs that not is a hostile target.";
    public string ConfigWindow_Param_UseDefenceAbility { get; set; } = "Use defense abilities";
    public string ConfigWindow_Param_UseDefenceAbilityDesc { get; set; } = "It is recommended to check this option if you are playing Raids./nPlan the heal and defense by yourself.???";
    public string ConfigWindow_Param_AutoShield { get; set; } = "Auto tank stance";
    public string ConfigWindow_Param_AutoProvokeForTank { get; set; } = "Auto Provoke (Tank)";
    public string ConfigWindow_Param_AutoProvokeForTankDesc { get; set; } = "When a hostile is hitting the non-Tank member of party, it will automatically use the Provoke.";
    public string ConfigWindow_Param_AutoUseTrueNorth { get; set; } = "Auto TrueNorth (Melee)";
    public string ConfigWindow_Param_RaisePlayerBySwift { get; set; } = "Raise player by swift";
    public string ConfigWindow_Param_UseGroundBeneficialAbility { get; set; } = "Use beneficial ground-targeted actions";
    public string ConfigWindow_Param_RaisePlayerByCasting { get; set; } = "Raise player by casting when swift is in cooldown";
    public string ConfigWindow_Param_UseHealWhenNotAHealer { get; set; } = "Use heal when not-healer";
    public string ConfigWindow_Param_LessMPNoRaise { get; set; } = "Never raise player if MP is less than the set value";
    public string ConfigWindow_Param_UseItem { get; set; } = "Use items";
    public string ConfigWindow_Param_UseItemDesc { get; set; } = "Use poison, WIP";
    public string ConfigWindow_Param_Conditon { get; set; } = "Condition";
    public string ConfigWindow_Param_StartOnCountdown { get; set; } = "Auto turn smart on countdown";
    public string ConfigWindow_Param_EsunaAll { get; set; } = "Esuna All Statuses.";
    public string ConfigWindow_Param_InterruptibleMoreCheck { get; set; } = "Interrupt the action with action type check.";

    public string ConfigWindow_Param_HealOutOfCombat { get; set; } = "Heal party members outside of combat.";

    public string ConfigWindow_Param_OnlyHotOnTanks { get; set; } = "Only use hot actions on tank rols.";

    public string ConfigWindow_Param_BeneficialAreaOnTarget { get; set; } = "Use Beneficial Area abilities on target.";

    public string ConfigWindow_Param_HealthDifference { get; set; } = "Set the HP standard deviation threshold for using AOE heal (ability & spell)";
    public string ConfigWindow_Param_HealthAreaAbility { get; set; } = "Set the HP threshold for using AOE healing ability";
    public string ConfigWindow_Param_HealthAreaSpell { get; set; } = "Set the HP threshold for using AOE healing spell";
    public string ConfigWindow_Param_HealingOfTimeSubtractArea { get; set; } = "Set the HP threshold reduce with hot effect(AOE)";
    public string ConfigWindow_Param_HealthSingleAbility { get; set; } = "Set the HP threshold for using single healing ability";
    public string ConfigWindow_Param_HealthSingleSpell { get; set; } = "Set the HP threshold for using single healing spell";

    public string ConfigWindow_Param_HealthTankHealerRatio { get; set; } = "Heal healer or tank first if its HP threshold is lower than this.";


    public string ConfigWindow_Param_HealingOfTimeSubtractSingle { get; set; } = "Set the HP threshold reduce with hot effect(single)";
    public string ConfigWindow_Param_HealthForDyingTank { get; set; } = "Set the HP threshold for tank to use invincibility";

    public string ConfigWindow_Param_MeleeRangeOffset { get; set; } = "Melee Range action using offset";
    public string ConfigWindow_Param_Target { get; set; } = "Target";
    public string ConfigWindow_Param_RightNowTargetToHostileType { get; set; } = "Hostile target filtering condition";
    public string ConfigWindow_Param_TargetToHostileType1 { get; set; } = "All targets can attack";
    public string ConfigWindow_Param_TargetToHostileType2 { get; set; } = "Targets have a target or all targets can attack";
    public string ConfigWindow_Param_TargetToHostileType3 { get; set; } = "Targets have a target";
    public string ConfigWindow_Param_NoticeUnexpectedCombat { get; set; } = "NOTICE: You are not turn the auto off between area on. It may start a combat unexpectedly.";
    public string ConfigWindow_Param_AddEnemyListToHostile { get; set; } = "Add Enemies list to the hostile target.";
    public string ConfigWindow_Param_ChooseAttackMark { get; set; } = "Priority attack targets with attack markers";
    public string ConfigWindow_Param_CanAttackMarkAOE { get; set; } = "Forced use of AOE";
    public string ConfigWindow_Param_AttackMarkAOEDesc { get; set; } = "Attention: Checking this option , AA will attack as many hostile targets as possible, while ignoring whether the attack will cover the marked target.";
    public string ConfigWindow_Param_FilterStopMark { get; set; } = "Never attack targets with stop markers";
    public string ConfigWindow_Param_ObjectMinRadius { get; set; } = "Set the minimum target circle threshold possessed by the attack target";
    public string ConfigWindow_Param_MoveTargetAngle { get; set; } = "The size of the sector angle that can be selected as the moveable target";
    public string ConfigWindow_Param_MoveTargetAngleDesc { get; set; } = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are movable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.";
    public string ConfigWindow_Param_ChangeTargetForFate { get; set; } = "Select only Fate targets in Fate";
    public string ConfigWindow_Param_OnlyAttackInView { get; set; } = "Only attack the target in view.";

    public string ConfigWindow_Param_MoveTowardsScreen { get; set; } = "Using movement actions towards the object in the center of the screen";
    public string ConfigWindow_Param_MoveTowardsScreenDesc { get; set; } = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.";
    public string ConfigWindow_Param_RaiseAll { get; set; } = "Raise all (include passerby)";
    public string ConfigWindow_Param_TargetFriendly { get; set; } = "Target all for friendly actions(include passerby)";
    public string ConfigWindow_Param_RaiseBrinkOfDeath { get; set; } = "Raise player even has Brink of Death";
    public string ConfigWindow_Param_MoveAreaActionFarthest { get; set; } = "Moving Area Ability to farthest";
    public string ConfigWindow_Param_MoveAreaActionFarthestDesc { get; set; } = "Move to the furthest position from character's face direction.";
    public string ConfigWindow_Param_Hostile { get; set; } = "Hostile";
    public string ConfigWindow_Param_HostileDesc { get; set; } = "You can set the logic of hostile target selection to allow flexibility in switching the logic of selecting hostile in battle.";
    public string ConfigWindow_Param_AddHostileCondition { get; set; } = "Add selection condition";
    public string ConfigWindow_Param_HostileCondition { get; set; } = "Hostile target selection condition";
    public string ConfigWindow_Param_ConditionUp { get; set; } = "Up";
    public string ConfigWindow_Param_ConditionDown { get; set; } = "Down";
    public string ConfigWindow_Param_ConditionDelete { get; set; } = "Delete";
    public string ConfigWindow_Param_ResetToDefault { get; set; } = "Press left ctrl + shift and press right mouse button to reset this value";

    public string ConfigWindow_Action_ShowOnCDWindow { get; set; } = "Show on CD window";

    public string ConfigWindow_Control_OnlyShowWithHostileOrInDuty { get; set; } = "Only shown if there are enemies in or in duty";
    public string ConfigWindow_Control_ShowNextActionWindow { get; set; } = "Show Next Action Window";

    public string ConfigWindow_Control_ShowControlWindow { get; set; } = "Show Control Window";
    public string ConfigWindow_Control_UseKeyboardCommand { get; set; } = "Use Keyboard Command";
    public string ConfigWindow_Control_UseGamepadCommand { get; set; } = "Use GamePad Command";
    public string ConfigWindow_Control_ShowCooldownWindow { get; set; } = "Show Cooldown Window";
    public string ConfigWindow_Control_IsInfoWindowNoInputs { get; set; } = "No Inputs";
    public string ConfigWindow_Control_IsInfoWindowNoMove { get; set; } = "No Move";

    public string ConfigWindow_Control_ShowItemsCooldown { get; set; } = "Show Items' Cooldown";
    public string ConfigWindow_Control_ShowGCDCooldown { get; set; } = "Show GCD' Cooldown";
    public string ConfigWindow_Control_UseOriginalCooldown { get; set; } = "Show Original Cooldown";
    public string ConfigWindow_Control_CooldownActionOneLine { get; set; } = "The count of cooldown actions in one line.";
    public string ConfigWindow_Control_CooldownFontSize { get; set; } = "Change the cooldown font size.";
    public string ConfigWindow_Control_BackgroundColor { get; set; } = "Control Window's Background";
    public string ConfigWindow_Control_InfoWindowBg { get; set; } = "Info Window's Background";
    public string ConfigWindow_Control_ControlWindowGCDSize { get; set; } = "GCD icon size";
    public string ConfigWindow_Control_ControlWindow0GCDSize { get; set; } = "0GCD icon size";
    public string ConfigWindow_Control_CooldownWindowIconSize { get; set; } = "Cooldown icon size";
    public string ConfigWindow_Control_ControlWindowNextSizeRatio { get; set; } = "Next Action Size Ratio";
    public string ConfigWindow_Control_ResetButtonOrKeyCommand { get; set; } = "Right click to reset the gamepad button or key board key.\nHold Left Ctrl and middle click to clear the key setting.";
    public string ConfigWindow_Control_NeedToEnable { get; set; } = " (Need to enable)";
    public string ConfigWindow_Control_ClickToUse { get; set; } = "Click to use it!";
    public string ConfigWindow_Rotation_BetaRotation { get; set; } = "Beta Rotation!";
    public string ConfigWindow_Rotation_DownloadRotationsButton { get; set; } = "Download Rotations";

    public string ConfigWindow_Rotation_DownloadRotations { get; set; } = "Download Rotations.";
    public string ConfigWindow_Rotation_AutoUpdateRotations { get; set; } = "Auto Update Rotations.";

    public string ConfigWindow_Rotation_InvalidRotation { get; set; } = "Invalid Rotation! \nPlease update to the latest version or contact to the {0}!";
    public string ConfigWindow_Rotation_ResetToDefault { get; set; } = "Click to reset the rotation configuration to default!";
    #endregion

    #region ScriptWindow
    public string ActionSequencer_DragdropDescription { get; set; } = "Drag&drop to move，Ctrl+Alt+RightClick to delete.";
    public string ActionSequencer_SearchBar { get; set; } = "Search Bar";
    public string ActionSequencer_MustUse { get; set; } = "MustUse";
    public string ActionSequencer_MustUseDesc { get; set; } = "Skip AOE and Buff.";
    public string ActionSequencer_Empty { get; set; } = "UseUp";
    public string ActionSequencer_EmptyDesc { get; set; } = "UseUp or Skip Combo";
    public string ActionSequencer_ActionSequencerDescription { get; set; } = "Add some condition to automatic use this action.";
    public string ActionSequencer_Can { get; set; } = "Can";
    public string ActionSequencer_Cannot { get; set; } = "Cannot";
    public string ActionSequencer_Is { get; set; } = "Is";
    public string ActionSequencer_Isnot { get; set; } = "Isnot";
    public string ActionSequencer_Have { get; set; } = "Have";
    public string ActionSequencer_Havenot { get; set; } = "Havenot";
    public string ActionSequencer_Ability { get; set; } = "Ability";
    public string ActionSequencer_Charges { get; set; } = "Charges";
    public string ActionSequencer_ConditionSet { get; set; } = "ConditionSet";
    public string ActionSequencer_ActionCondition { get; set; } = "ActionCondition";
    public string ActionSequencer_TargetCondition { get; set; } = "TargetCondition";
    public string ActionSequencer_RotationCondition { get; set; } = "RotationCondition";
    public string ActionSequencer_ActionTarget { get; set; } = "{0}'s target";
    public string ActionSequencer_Target { get; set; } = "Target";
    public string ActionSequencer_Player { get; set; } = "Player";
    public string ActionSequencer_StatusSelf { get; set; } = "StatusSelf";
    public string ActionSequencer_StatusSelfDesc { get; set; } = "StatusSelf";
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
    public string SpecialCommandType_TankStance { get; set; } = "Tank Stance";
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
    public string TargetConditionType_CastingActionTimeUntil { get; set; } = "Casting Action Time Until";

    #endregion

    #region DescType
    public string DescType_BurstActions { get; set; } = "Burst Actions";
    public string DescType_MoveForwardGCD { get; set; } = "Move Forward GCD";
    public string DescType_HealAreaGCD { get; set; } = "Area Healing GCD";
    public string DescType_HealSingleGCD { get; set; } = "Single Healing GCD";
    public string DescType_DefenseAreaGCD { get; set; } = "Area Defense GCD";
    public string DescType_DefenseSingleGCD { get; set; } = "Single Defense GCD";

    public string DescType_HealAreaAbility { get; set; } = "Area Healing Ability";
    public string DescType_HealSingleAbility { get; set; } = "Single Healing Ability";
    public string DescType_DefenseAreaAbility { get; set; } = "Area Defense Ability";
    public string DescType_DefenseSingleAbility { get; set; } = "Single Defense Ability";
    public string DescType_MoveForwardAbility { get; set; } = "Move Forward Ability";
    public string DescType_MoveBackAbility { get; set; } = "Move Back Ability";

    #endregion

    #region JobRole
    public string JobRole_None { get; set; } = "Gathering&Production";
    public string JobRole_Tank { get; set; } = "Tank";
    public string JobRole_Melee { get; set; } = "Melee";
    public string JobRole_Ranged { get; set; } = "Ranged";
    public string JobRole_Healer { get; set; } = "Healer";
    public string JobRole_RangedPhysical { get; set; } = "Ranged";
    public string JobRole_RangedMagical { get; set; } = "Magical";
    public string JobRole_DiscipleOfTheLand { get; set; } = "Disciple of the Land";
    public string JobRole_DiscipleOfTheHand { get; set; } = "Disciple of the Hand";

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
    public string HighEndBan { get; set; } = "{0} can not be used in High-end Duty!";
}
