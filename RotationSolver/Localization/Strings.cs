namespace RotationSolver.Localization;

internal class Strings
{
    #region Commands
    public string Commands_Rotation { get; set; } = "Open config window.";
    public string Commands_ChangeSettingsValue { get; set; } = "Modify {0} to {1}";
    public string Commands_ChangeRotationConfig { get; set; } = "Modify {0} to {1}";
    public string Commands_CannotFindRotationConfig { get; set; } = "Failed to find the config in this rotation, please check it.";

    public string Commands_CannotFindConfig { get; set; } = "Failed to find the config, please check it.";

    public string Commands_InsertAction { get; set; } = "Will use it within {0}s";

    public string Commands_InsertActionFailure { get; set; } = "Can not find the action, please check the action name.";


    #endregion

    #region ConfigWindow
    public string ConfigWindow_Header { get; set; } = "Rotation Solver Settings v";
    public string ConfigWindow_EventItem { get; set; } = "Event";

    public string ConfigWindow_HelpItem_AttackAuto { get; set; }
        = "Start the addon in Auto mode. When out of combat or when combat starts, switches the target according to the set condition.";

    public string ConfigWindow_HelpItem_AttackManual { get; set; }
        = "Start the addon in Manual mode. You need to choose the target manually. This will bypass any engage settings that you have set up and will start attacking immediately once something is targeted.";

    public string ConfigWindow_HelpItem_NextAction { get; set; } = "Do the next action";

    public string ConfigWindow_HelpItem_AttackCancel { get; set; }
        = "Stop the addon. Always remember to turn it off when it is not in use!";

    public string ConfigWindow_HelpItem_HealArea { get; set; }
        = "Open a window to use AoE heal.";

    public string ConfigWindow_HelpItem_HealSingle { get; set; }
        = "Open a window to use single heal.";

    public string ConfigWindow_HelpItem_DefenseArea { get; set; }
        = "Open a window to use AoE defense.";

    public string ConfigWindow_HelpItem_DefenseSingle { get; set; }
        = "Open a window to use single defense.";

    public string ConfigWindow_HelpItem_Esuna { get; set; }
        = "Open a window to use Esuna, tank stance actions or True North.";

    public string ConfigWindow_HelpItem_RaiseShirk { get; set; }
        = "Open a window to use Raise or Shirk.";

    public string ConfigWindow_HelpItem_AntiKnockback { get; set; }
        = "Open a window to use knockback immunity actions.";

    public string ConfigWindow_HelpItem_Burst { get; set; }
        = "Open a window to burst.";

    public string ConfigWindow_HelpItem_MoveForward { get; set; }
        = "Open a window to move forward.";

    public string ConfigWindow_HelpItem_MoveBack { get; set; }
        = "Open a window to move back.";
    public string ConfigWindow_HelpItem_Speed { get; set; }
        = "Open a window to speed up.";

    public string ConfigWindow_HelpItem_LimitBreak { get; set; }
    = "Open a window to use limit break.";

    public string ConfigWindow_HelpItem_EndSpecial { get; set; }
        = "To end this special duration before the set time.";
    public string ConfigWindow_Helper_SwitchRotation { get; set; } = "Click to switch rotations";
    public string ConfigWindow_Helper_GameVersion { get; set; } = "Game";
    public string ConfigWindow_Helper_RunCommand { get; set; } = "Click to execute the command";
    public string ConfigWindow_Helper_CopyCommand { get; set; } = "Right-click to copy the command";

    public string ConfigWindow_Events_AddEvent { get; set; } = "AddEvents";
    public string ConfigWindow_Events_Description { get; set; } = "In this window, you can set what macro will be trigger after using an action.";
    public string ConfigWindow_Events_ActionName { get; set; } = "Action Name";
    public string ConfigWindow_Events_MacroIndex { get; set; } = "Macro No.";
    public string ConfigWindow_Events_ShareMacro { get; set; } = "Is Shared";
    public string ConfigWindow_Events_RemoveEvent { get; set; } = "Delete Event";
    public string ConfigWindow_Events_DutyStart { get; set; } = "Duty Start: ";
    public string ConfigWindow_Events_DutyEnd { get; set; } = "Duty End: ";

    public string ConfigWindow_Param_UseOverlayWindow { get; set; } = "Display UI Overlay";
    public string ConfigWindow_Param_UseOverlayWindowDesc { get; set; } = "This top window is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.";

    public string ConfigWindow_Param_ActionAhead { get; set; } = "The time before an oGCD is avaliable to start trying using it";
    public string ConfigWindow_Param_MinLastAbilityAdvanced { get; set; } = "The time ahead of the last oGCD before the next GCD being avaliable to start trying using it (may affect skill weaving)";

    public string ConfigWindow_Param_CountDownAhead { get; set; } = "The starting when abilities will be used before finishing the countdown";
    public string ConfigWindow_Param_SpecialDuration { get; set; } = "The duration of special windows set by commands";
    public string ConfigWindow_Param_AddDotGCDCount { get; set; } = "The number of GCDs in advance for DOT refreshing";
    public string ConfigWindow_Param_MaxPing { get; set; } = "The max ping that RS can get to before skipping to the next action.";
    public string ConfigWindow_Param_AutoOffBetweenArea { get; set; } = "Auto turn off when player is moving between areas.";
    public string ConfigWindow_Param_AutoOffAfterCombat { get; set; } = "Auto turn off RS when combat is over more for more then...";
    public string ConfigWindow_Param_AutoOffCutScene { get; set; } = "Auto turn off during cutscenes.";

    public string ConfigWindow_Param_AutoOffWhenDead { get; set; } = "Auto turn off when dead.";
    public string ConfigWindow_Param_AutoOffWhenDutyCompleted { get; set; } = "Auto turn off when duty completed.";

    public string ConfigWindow_Param_UseWorkTask { get; set; } = "Use work task for acceleration.";
    public string ConfigWindow_Param_ToggleManual { get; set; } = "Make /rotation Manual as a toggle command.";
    public string ConfigWindow_Param_ToggleAuto { get; set; } = "Make /rotation Auto as a toggle command.";

    public string ConfigWindow_Param_WeaponDelay { get; set; } = "The range of random delay for GCD usage.";
    public string ConfigWindow_Param_DeathDelay { get; set; } = "The range of random delay for using revival abilities.";
    public string ConfigWindow_Param_HostileDelay { get; set; } = "The range of random delay for targeting.";
    public string ConfigWindow_Param_InterruptDelay { get; set; } = "The range of random delay for interrupting hostile targets.";
    public string ConfigWindow_Param_WeakenDelay { get; set; } = "The range of random delay for cleansing dispellable debuffs.";

    public string ConfigWindow_Param_HealDelay { get; set; } = "The range of random delay for healing.";

    public string ConfigWindow_Param_CountdownDelay { get; set; } = "The random delay between which auto mode activation on countdown varies.";
    public string ConfigWindow_Param_NotInCombatDelay { get; set; } = "The range of random delay for Not In Combat.";

    public string ConfigWindow_Param_ClickingDelay { get; set; } = "The range of random delay for clicking actions.";
    public string ConfigWindow_Param_StopCastingDelay { get; set; } = "The range of random delay for stopping casting when the target is dead or immune to damage.";
    public string ConfigWindow_Param_ClickMistake { get; set; } = "How likely is it that RS will click the wrong action.";
    public string ConfigWindow_Param_PoslockCasting { get; set; } = "Lock the movement when casting or when doing some actions.";
    public string ConfigWindow_Param_UseStopCasting { get; set; } = "Stops casting when the target is dead.";
    public string ConfigWindow_Param_ShowTooltips { get; set; } = "Show tooltips";
    public string ConfigWindow_Param_InDebug { get; set; } = "Debug Mode";

    public string ConfigWindow_Param_ShowTargetTimeToKill { get; set; } = "Show the target's time to kill.";

    public string ConfigWindow_Param_BossTimeToKill { get; set; } = "If target's time until death is higher than this, regard it as boss.";

    public string ConfigWindow_Param_DyingTimeToKill { get; set; } = "If target's time until death is lower than this, regard it is dying.";
    public string ConfigWindow_Param_PoslockModifier { get; set; } = "The modifier key to unlock the movement temporary";
    public string ConfigWindow_Param_PoslockDescription { get; set; } = "LT is for gamepad player";
    public string ConfigWindow_Param_TeachingMode { get; set; } = "Teaching mode";
    public string ConfigWindow_Param_TeachingModeColor { get; set; } = "Prompt box color of teaching mode";
    public string ConfigWindow_Param_MovingTargetColor { get; set; } = "Prompt box color of moving target";
    public string ConfigWindow_Param_TargetColor { get; set; } = "Target color";
    public string ConfigWindow_Param_SubTargetColor { get; set; } = "Sub-target color";
    public string ConfigWindow_Param_DrawingHeight { get; set; } = "The height of the drawing things.";
    public string ConfigWindow_Param_SampleLength { get; set; } = "Drawing smoothness.";
    public string ConfigWindow_Param_KeyBoardNoise { get; set; } = "Simulate the effect of pressing abilities";
    public string ConfigWindow_Param_KeyBoardNoiseTimes { get; set; } = "Effect times";
    public string ConfigWindow_Param_DrawMeleeOffset { get; set; } = "Draw the offset of melee on the screen";
    public string ConfigWindow_Param_ShowMoveTarget { get; set; } = "Show the target of the move action";
    public string ConfigWindow_Param_ShowTarget { get; set; } = "Show Target";
    public string ConfigWindow_Param_TargetIconSize { get; set; } = "The size of the next ability that will be used icon.";
    public string ConfigWindow_Param_SayOutStateChanged { get; set; } = "Audio notification for when the status changes";
    public string ConfigWindow_Param_ShowInfoOnDtr { get; set; } = "Display plugin status on server info";
    public string ConfigWindow_Param_ShowInfoOnToast { get; set; } = "Display plugin status on toast";
    public string ConfigWindow_Param_ShowToastsAboutDoAction { get; set; } = "Display do action feedback on toast";
    public string ConfigWindow_Param_ShowToggledActionInChat { get; set; } = "Display toggle action feedback on chat";
    public string ConfigWindow_Param_UseAOEAction { get; set; } = "Use AoE actions";

    public string ConfigWindow_Param_UseAOEWhenManual { get; set; } = "Use AoE actions in manual mode";
    public string ConfigWindow_Param_AutoBurst { get; set; } = "Automatically trigger dps burst phase";

    public string ConfigWindow_Param_AutoHeal { get; set; } = "Automatic Heal";
    public string ConfigWindow_Param_UseAbility { get; set; } = "Auto-use abilities";
    public string ConfigWindow_Param_NoNewHostiles { get; set; } = "Don't attack new mobs by AoE";
    public string ConfigWindow_Params_NoNewHostilesDesc { get; set; } = "Never use any AoE action when this may attack the mobs that are not hostile targets.";
    public string ConfigWindow_Param_UseDefenseAbility { get; set; } = "Use defensive abilities";
    public string ConfigWindow_Param_UseDefenseAbilityDesc { get; set; } = "It is recommended to check this option if you are playing Raids or you can plan the heal and defense ability usage by yourself.";
    public string ConfigWindow_Param_AutoShield { get; set; } = "Automatically activate tank stance";
    public string ConfigWindow_Param_AutoProvokeForTank { get; set; } = "Auto provoke non-tank attacking targets";
    public string ConfigWindow_Param_AutoProvokeForTankDesc { get; set; } = "Automatically use provoke when an enemy is attacking a non-tank member of the party.";
    public string ConfigWindow_Param_AutoUseTrueNorth { get; set; } = "Auto TrueNorth (Melee)";
    public string ConfigWindow_Param_RaisePlayerBySwift { get; set; } = "Raise player by using swiftcast if avaliable";
    public string ConfigWindow_Param_UseGroundBeneficialAbility { get; set; } = "Use beneficial ground-targeted actions";
    public string ConfigWindow_Param_AutoSpeedOutOfCombat { get; set; } = "Use movement speed increase abilities when out of combat.";
    public string ConfigWindow_Param_RaisePlayerByCasting { get; set; } = "Raise player while swiftcast is on cooldown";
    public string ConfigWindow_Param_UseHealWhenNotAHealer { get; set; } = "Use healing abilities when playing a non-healer role";
    public string ConfigWindow_Param_LessMPNoRaise { get; set; } = "Never raise player if MP is less than the set value";
    public string ConfigWindow_Param_UseTinctures { get; set; } = "Use Tinctures";
    public string ConfigWindow_Param_UseHealPotions { get; set; } = "Use Heal Potions";
    public string ConfigWindow_Param_StartOnCountdown { get; set; } = "Auto mode activation delay on countdown start";

    public string ConfigWindow_Param_StartOnAttackedBySomeone { get; set; } = "Automatically turn on manual mode and target enemy when being attacked";
    public string ConfigWindow_Param_EsunaAll { get; set; } = "Cleanse all dispellable debuffs.";
    public string ConfigWindow_Param_InterruptibleMoreCheck { get; set; } = "Use interrupt abilities if possible.";

    public string ConfigWindow_Param_HealOutOfCombat { get; set; } = "Heal party members outside of combat.";

    public string ConfigWindow_Param_OnlyHotOnTanks { get; set; } = "Use single target healing over time actions only on tanks";

    public string ConfigWindow_Param_BeneficialAreaOnLocations { get; set; } = "On predefined location";
    public string ConfigWindow_Param_BeneficialAreaOnlyOnLocations { get; set; } = "Only on predefined location";
    public string ConfigWindow_Param_BeneficialAreaOnTarget { get; set; } = "On target";

    public string ConfigWindow_Param_BeneficialAreaOnCalculated { get; set; } = "On the calculated location";



    public string ConfigWindow_Param_HealthDifference { get; set; } = "HP for standard deviation for using AoE heal.";
    public string ConfigWindow_Param_HealthAreaAbility { get; set; } = "HP for AoE healing oGCDs";

    public string ConfigWindow_Param_HealthAreaSpell { get; set; } = "HP for AoE healing GCDs";

    public string ConfigWindow_Param_Normal { get; set; } = "Normal Targets";
    public string ConfigWindow_Param_HOT { get; set; } = "Targets with HOT";

    public string ConfigWindow_Param_HealthSingleAbility { get; set; } = "HP for ST healing oGCDs";

    public string ConfigWindow_Param_HealthSingleSpell { get; set; } = "HP for ST healing GCDs";

    public string ConfigWindow_Param_HealthHealerRatio { get; set; } = "Heal healer first if its HP is lower than this.";

    public string ConfigWindow_Param_HealthTankRatio { get; set; } = "Heal tank first if its HP is lower than this.";

    public string ConfigWindow_Param_DistanceForMoving { get; set; } = "Use gapcloser as a damage ability if the distance to your target is less then this.";
    public string ConfigWindow_Param_HealWhenNothingTodo { get; set; } = "Healing the members with GCD if there is nothing to do in combat.";

    public string ConfigWindow_Param_HealWhenNothingTodoBelow { get; set; } = "When their minimum HP is lower than this.";
    public string ConfigWindow_Param_HealWhenNothingTodoDelay { get; set; } = "The delay of this type of healing.";

    public string ConfigWindow_Param_HealthForDyingTank { get; set; } = "The HP for tank to use invulnerability";
    public string ConfigWindow_Param_HealthForGuard { get; set; } = "The HP for using Guard.";

    public string ConfigWindow_Param_MeleeRangeOffset { get; set; } = "Melee Range action using offset";
    public string ConfigWindow_Param_RightNowTargetToHostileType { get; set; } = "Engage settings";
    public string ConfigWindow_Param_TargetToHostileType1 { get; set; } = "All targets that are in range for any abilities";
    public string ConfigWindow_Param_TargetToHostileType2 { get; set; } = "Previously engaged targets or all targets that are in range\n(engages on countdown timer and resets when out of combat)";
    public string ConfigWindow_Param_TargetToHostileType3 { get; set; } = "Previously engaged targets (engages on countdown timer)";
    public string ConfigWindow_Param_AddEnemyListToHostile { get; set; } = "Add enemy list to the hostile targets.";
    public string ConfigWindow_Param_OnlyAttackInEnemyList { get; set; } = "Only attack the targets in enemy list.";
    public string ConfigWindow_Param_ChooseAttackMark { get; set; } = "Priority attack targets with attack markers";
    public string ConfigWindow_Param_CanAttackMarkAOE { get; set; } = "Allowed use of AoE to attack more mobs.";
    public string ConfigWindow_Param_AttackMarkAOEDesc { get; set; } = "Attention: Checking this option , AA will attack as many hostile targets as possible, while ignoring whether the attack will reach the marked target.";
    public string ConfigWindow_Param_FilterStopMark { get; set; } = "Never attack targets with stop markers";
    public string ConfigWindow_Param_MoveTargetAngle { get; set; } = "The size of the sector angle that can be selected as the moveable target";
    public string ConfigWindow_Param_MoveTargetAngleDesc { get; set; } = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are moveable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.";
    public string ConfigWindow_Param_ChangeTargetForFate { get; set; } = "Select only Fate targets in Fate";
    public string ConfigWindow_Param_OnlyAttackInView { get; set; } = "Only attack the target in view.";

    public string ConfigWindow_Param_MoveTowardsScreen { get; set; } = "Using movement actions towards the object in the center of the screen";
    public string ConfigWindow_Param_MoveTowardsScreenDesc { get; set; } = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.";
    public string ConfigWindow_Param_RaiseAll { get; set; } = "Raise any player in range (even if they are not in your party)";
    public string ConfigWindow_Param_ActionTargetFriendly { get; set; } = "Target all for friendly actions (include passerby)";
    public string ConfigWindow_Param_TargetFriendly { get; set; } = "Target allies for friendly actions.";
    public string ConfigWindow_Param_TargetFatePriority { get; set; } = "Target Fate priority";
    public string ConfigWindow_Param_TargetHuntingRelicLevePriority { get; set; } = "Target Hunt/Relic/Leve priority.";
    public string ConfigWindow_Param_TargetQuestPriority { get; set; } = "Target quest priority.";

    public string ConfigWindow_Param_RaiseBrinkOfDeath { get; set; } = "Raise players that even have Brink of Death debuff";
    public string ConfigWindow_Param_MoveAreaActionFarthest { get; set; } = "Target movement area ability to the farthest possible location";
    public string ConfigWindow_Param_MoveAreaActionFarthestDesc { get; set; } = "Move to the furthest position for targeting are movement actions.";

    public string ConfigWindow_Param_HostileDesc { get; set; } = "You can The logic of hostile target selection to allow flexibility in switching the logic of selecting hostile in battle.";
    public string ConfigWindow_Param_HostileCondition { get; set; } = "Hostile target selection condition";

    public string ConfigWindow_Control_OnlyShowWithHostileOrInDuty { get; set; } = "Only show these windows if there are enemies in or in duty";
    public string ConfigWindow_Control_ShowNextActionWindow { get; set; } = "Show Next Action Window";

    public string ConfigWindow_Control_ShowControlWindow { get; set; } = "Show Control Window";
    public string ConfigWindow_Control_ShowCooldownWindow { get; set; } = "Show Cooldown Window";
    public string ConfigWindow_Control_IsInfoWindowNoInputs { get; set; } = "No Inputs";
    public string ConfigWindow_Control_IsInfoWindowNoMove { get; set; } = "No Move";
    public string ConfigWindow_Control_IsInfoWindowMove { get; set; } = "Move";

    public string ConfigWindow_Control_ShowItemsCooldown { get; set; } = "Show Items' Cooldown";
    public string ConfigWindow_Control_ShowGCDCooldown { get; set; } = "Show GCD' Cooldown";
    public string ConfigWindow_Control_UseOriginalCooldown { get; set; } = "Show Original Cooldown";
    public string ConfigWindow_Control_CooldownFontSize { get; set; } = "Change the cooldown font size.";
    public string ConfigWindow_Control_UnlockBackgroundColor { get; set; } = "Unlocked Control Window's Background";
    public string ConfigWindow_Control_LockBackgroundColor { get; set; } = "Locked Control Window's Background";
    public string ConfigWindow_Control_InfoWindowBg { get; set; } = "Info Window's Background";
    public string ConfigWindow_Control_ControlWindowGCDSize { get; set; } = "GCD icon size";
    public string ConfigWindow_Control_ControlWindow0GCDSize { get; set; } = "oGCD icon size";
    public string ConfigWindow_Control_CooldownWindowIconSize { get; set; } = "Cooldown icon size";
    public string ConfigWindow_Control_ControlWindowNextSizeRatio { get; set; } = "Next Action Size Ratio";
    public string ConfigWindow_Rotation_BetaRotation { get; set; } = "Beta Rotation!";

    public string ConfigWindow_Rotation_DownloadRotations { get; set; } = "Auto Download Rotations";
    public string ConfigWindow_Rotation_AutoUpdateRotations { get; set; } = "Auto Update Rotations";

    public string ConfigWindow_Rotation_InvalidRotation { get; set; } = "Invalid Rotation! \nPlease update to the latest version or contact to the {0}!";

    public string ConfigWindow_List_Description { get; set; } = "In this window, you can set the parameters that can be customised using lists.";
    public string ConfigWindow_List_Hostile { get; set; } = "Hostile";

    public string ConfigWindow_List_Invincibility { get; set; } = "Invulnerability";
    public string ConfigWindow_List_Priority { get; set; } = "Priority";
    public string ConfigWindow_List_InvincibilityDesc { get; set; } = "Ignores target if it has one of this statuses";
    public string ConfigWindow_List_PriorityDesc { get; set; } = "Attacks the target first if it has one of this statuses";
    public string ConfigWindow_List_DangerousStatus { get; set; } = "Dispellable debuffs";

    public string ConfigWindow_List_DangerousStatusDesc { get; set; } = "Esuna dispellable debuffs list";

    public string ConfigWindow_List_HostileCastingTank { get; set; } = "Tank Buster";

    public string ConfigWindow_List_HostileCastingTankDesc { get; set; } = "Use tank personal damage mitigation abilities if the target is casting any of these actions";

    public string ConfigWindow_List_HostileCastingArea { get; set; } = "AoE";

    public string ConfigWindow_List_HostileCastingAreaDesc { get; set; } = "Use AoE damage mitigation abilities if the target is casting any of these actions";

    public string ConfigWindow_List_NoHostile { get; set; } = "Don't target";
    public string ConfigWindow_List_NoProvoke { get; set; } = "Don't provoke";
    public string ConfigWindow_List_NoHostileDesc { get; set; } = "Enemies that will never be targeted.";
    public string ConfigWindow_List_NoProvokeDesc { get; set; } = "Enemies that will never be provoked.";

    #endregion

    #region Action Sequencer
    public string ActionSequencer_Load { get; set; } = "Load From folder.";
    public string ActionSequencer_TimeOffset { get; set; } = "Time Offset";

    public string ActionSequencer_AOECount { get; set; } = "AOE Count";
    public string ActionSequencer_Charges { get; set; } = "Charges";
    public string ActionSequencer_ConditionSet { get; set; } = "Condition Set";
    public string ActionSequencer_ActionCondition { get; set; } = "Action Condition";
    public string ActionSequencer_TargetCondition { get; set; } = "Target Condition";
    public string ActionSequencer_RotationCondition { get; set; } = "Rotation Condition";
    public string ActionSequencer_NamedCondition { get; set; } = "Named Condition";
    public string ActionSequencer_TerritoryCondition { get; set; } = "Territory Condition";
    public string ActionSequencer_FromClipboard { get; set; } = "From Clipboard";
    public string ActionSequencer_TraitCondition { get; set; } = "Trait Condition";
    public string ActionSequencer_ActionTarget { get; set; } = "{0}'s target";
    public string ActionSequencer_Target { get; set; } = "Target";
    public string ActionSequencer_HostileTarget { get; set; } = "Hostile Target";
    public string ActionSequencer_Player { get; set; } = "Player";
    public string ActionSequencer_Original { get; set; } = "Original";
    public string ActionSequencer_Adjusted { get; set; } = "Adjusted";
    public string ActionSequencer_StatusSelf { get; set; } = "From Self";
    public string ActionSequencer_StatusAll { get; set; } = "From All";
    public string ActionSequencer_Delay_Description { get; set; } = "Delay its turning to true.";
    public string ActionSequencer_NotDescription { get; set; } = "Click to make it reverse.\nIs reversed : {0}";
    #endregion

    #region Actions
    public string Action_Friendly { get; set; } = "Support";
    public string Action_Ability { get; set; } = "0GCD";
    public string Action_Attack { get; set; } = "Attack";
    #endregion

    #region ComboConditionType
    public string ComboConditionType_Bool { get; set; } = "Boolean";
    public string ComboConditionType_Byte { get; set; } = "Byte";
    public string ComboConditionType_Float { get; set; } = "Float";
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
    public string SpecialCommandType_Speed { get; set; } = "Speed";
    public string SpecialCommandType_LimitBreak { get; set; } = "Limit Break";
    public string SpecialCommandType_Smart { get; set; } = "Auto Target ";
    public string SpecialCommandType_Manual { get; set; } = "Manual Target";
    public string SpecialCommandType_Cancel { get; set; } = "Cancel";
    public string SpecialCommandType_Off { get; set; } = "Off";
    #endregion

    #region ActionConditionType
    public string ActionConditionType_Elapsed { get; set; } = "Elapsed";
    public string ActionConditionType_ElapsedGCD { get; set; } = "Elapsed GCD";
    public string ActionConditionType_Remain { get; set; } = "Remain Time";
    public string ActionConditionType_RemainGCD { get; set; } = "Remain GCD";
    public string ActionConditionType_ShouldUse { get; set; } = "Can Use";
    public string ActionConditionType_EnoughLevel { get; set; } = "Enough Level";
    public string ActionConditionType_IsCoolDown { get; set; } = "Is CoolDown";
    public string ActionConditionType_CurrentCharges { get; set; } = "Current Charges";
    public string ActionConditionType_MaxCharges { get; set; } = "Max Charges";
    #endregion

    #region TargetConditionType
    public string TargetConditionType_HasStatus { get; set; } = "Has Status";
    public string TargetConditionType_IsDying { get; set; } = "Is Dying";
    public string TargetConditionType_IsBossFromTTK { get; set; } = "Is Boss From TTK";
    public string TargetConditionType_IsBossFromIcon { get; set; } = "Is Boss From Icon";
    public string TargetConditionType_InCombat { get; set; } = "In Combat";
    public string TargetConditionType_Distance { get; set; } = "Distance";
    public string TargetConditionType_StatusEnd { get; set; } = "Status End";
    public string TargetConditionType_TimeToKill { get; set; } = "Time To Kill";
    public string TargetConditionType_StatusEndGCD { get; set; } = "Status End GCD";
    public string TargetConditionType_CastingAction { get; set; } = "Casting Action";
    public string TargetConditionType_CastingActionTimeUntil { get; set; } = "Casting Action Time Until";
    public string TargetConditionType_HP { get; set; } = "HP";
    public string TargetConditionType_HPRatio { get; set; } = "HP%";
    public string TargetConditionType_MP { get; set; } = "MP";
    public string TargetConditionType_TargetName { get; set; } = "Target Name";
    public string TargetConditionType_ObjectEffect { get; set; } = "Object Effect";
    public string TargetConditionType_Vfx { get; set; } = "Vfx";
    public string TargetConditionType_IsNull { get; set; } = "Is Null";

    #endregion

    #region COnfigUnitType
    public string ConfigUnitType_Seconds { get; set; } = "Time Unit, in seconds.";
    public string ConfigUnitType_Degree { get; set; } = "Angle Unit, in degrees.";
    public string ConfigUnitType_Pixels { get; set; } = "Display Unit, in pixels.";
    public string ConfigUnitType_Yalms { get; set; } = "Distance Unit, in yalms.";
    public string ConfigUnitType_Ratio { get; set; } = "Ratio Unit, as percentage.";

    #endregion

    #region TerritoryConditionType
    public string TerritoryConditionType_TerritoryContentType { get; set; } = "Territory Content Type";
    public string TerritoryConditionType_TerritoryName { get; set; } = "Territory Name";
    public string TerritoryConditionType_DutyName { get; set; } = "Duty Name";
    public string TerritoryConditionType_MapEffect { get; set; } = "Map Effect";
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
    public string DescType_SpeedAbility { get; set; } = "Speed Ability";

    #endregion

    public Dictionary<string, string> MemberInfoName { get; set; } = new Dictionary<string, string>()
    {
        #region Rotation
        { nameof(CustomRotation.IsMoving), "Is Moving or Jumping"},
        { nameof(CustomRotation.HasSwift), "Has Swift"},
        { nameof(CustomRotation.HasTankStance), "Has tank stance"},
        { nameof(CustomRotation.InCombat), "In Combat"},
        { nameof(CustomRotation.NotInCombatDelay), "Not In Combat Delay"},
        { nameof(CustomRotation.CurrentMp), "Player's MP"},
        { nameof(CustomRotation.HasCompanion), "Has companion"},
        { nameof(CustomRotation.RatioOfMembersIn2minsBurst), "Raito of members that in 2 mins burst."},
        { nameof(CustomRotation.IsFullParty), "Is Full Party"},
        { nameof(CustomRotation.PartyMembersMinHP), "Min HP in party members."},
        { nameof(CustomRotation.PartyMembersAverHP), "Average HP in party members."},
        { nameof(CustomRotation.HasHostilesInRange), "Has hostiles in Range"},
        { nameof(CustomRotation.HasHostilesInMaxRange), "Has hostiles in 25 yalms"},
        { nameof(CustomRotation.NumberOfHostilesInRange), "The number of hostiles in Range"},
        { nameof(CustomRotation.NumberOfHostilesInMaxRange), "The number of hostiles in max Range"},
        { nameof(CustomRotation.NumberOfAllHostilesInRange), "The number of all hostiles in Range"},
        { nameof(CustomRotation.NumberOfAllHostilesInMaxRange), "The number of all hostiles in max Range"},
        { nameof(CustomRotation.IsBurst), "Is burst"},
        { nameof(CustomRotation.IsHealArea), "Is Heal Area"},
        { nameof(CustomRotation.IsHealSingle), "Is Heal Single"},
        { nameof(CustomRotation.IsDefenseArea), "Is Defense Area"},
        { nameof(CustomRotation.IsDefenseSingle), "Is Defense Single"},
        { nameof(CustomRotation.IsEsunaStanceNorth), "Is Esuna Stance North"},
        { nameof(CustomRotation.IsRaiseShirk), "Is Raise Shirk"},
        { nameof(CustomRotation.IsMoveForward), "Is Move Forward"},
        { nameof(CustomRotation.IsMoveBack), "Is Move Back"},
        { nameof(CustomRotation.IsAntiKnockback), "Is Anti Knockback"},
        { nameof(CustomRotation.IsSpeed), "Is Speed"},
        { nameof(CustomRotation.IsLimitBreak), "Is Limit Break"},

        { nameof(CustomRotation.CanHealAreaAbility), "Can heal area ability"},
        { nameof(CustomRotation.CanHealAreaSpell), "Can heal area spell"},
        { nameof(CustomRotation.CanHealSingleAbility), "Can heal single ability"},
        { nameof(CustomRotation.CanHealSingleSpell), "Can heal single area"},

        { nameof(CustomRotation.AutoState), "The state of auto. True for on."},
        { nameof(CustomRotation.IsManual), "The state of manual. True for manual."},

        { nameof(CustomRotation.IsInHighEndDuty), "Is in the high-end duty"},
        { nameof(CustomRotation.IsInDuty), "Is player in duty"},
        { nameof(CustomRotation.Ping), "Your ping"},
        { nameof(CustomRotation.NextAbilityToNextGCD), "Time from next ability to next GCD"},

        { nameof(CustomRotation.IsLastGCD), "Just used GCD"},
        { nameof(CustomRotation.IsLastAbility), "Just used Ability"},
        { nameof(CustomRotation.IsLastAction), "Just used Action"},

        { nameof(CustomRotation.CombatTime), "Combat time"},
        { nameof(CustomRotation.StopMovingTime), "Stop moving time"},
        { nameof(CustomRotation.CountDownAhead), "Count Down ahead"},
        { nameof(CustomRotation.HealthAreaAbility), "Health of Area Ability"},
        { nameof(CustomRotation.HealthAreaSpell), "Health of Area spell"},
        { nameof(CustomRotation.HealthAreaAbilityHot), "Health of Area Ability Hot"},
        { nameof(CustomRotation.HealthAreaSpellHot), "Health of Area spell Hot"},
        { nameof(CustomRotation.HealthSingleAbility), "Health of single ability"},
        { nameof(CustomRotation.HealthSingleSpell), "Health of single spell"},
        { nameof(CustomRotation.HealthSingleAbilityHot), "Health of single ability Hot"},
        { nameof(CustomRotation.HealthSingleSpellHot), "Health of single spell Hot"},
        { nameof(CustomRotation.HealthForDyingTanksDefault), "Health of dying tank"},

        { nameof(CustomRotation.IsValid), "Is this rotation valid"},
        { nameof(CustomRotation.ShowStatus), "Show the status"},
        { nameof(CustomRotation.AverageTimeToKill), "Average time to kill"},
        { nameof(CustomRotation.LimitBreakLevel), "Limit Break Level"},

        { nameof(CustomRotation.MobsTime), "Mobs Time"},
        #endregion

        #region AST
        #endregion

        #region BLM
        { nameof(BLM_Base.UmbralIceStacks), "Umbral Ice Stacks" },
        { nameof(BLM_Base.AstralFireStacks), "Astral Fire Stacks" },
        { nameof(BLM_Base.PolyglotStacks), "Polyglot Stacks" },
        { nameof(BLM_Base.UmbralHearts), "Umbral Hearts" },
        { nameof(BLM_Base.IsParadoxActive), "Is Paradox Active" },
        { nameof(BLM_Base.InUmbralIce), "In Umbral Ice" },
        { nameof(BLM_Base.InAstralFire), "In Astral Fire" },
        { nameof(BLM_Base.IsEnochianActive), "Is Enochian Active" },
        { nameof(BLM_Base.IsPolyglotStacksMaxed), "IsPolyglot Stacks Maxed" },
        { nameof(BLM_Base.EnochianTime), "Enochian Time" },
        #endregion

        #region BRD
        { nameof(BRD_Base.Repertoire), "Repertoire" },
        { nameof(BRD_Base.SoulVoice), "Soul Voice" },
        { nameof(BRD_Base.SongTime), "Soul Time" },
        #endregion

        #region DNC
        { nameof(DNC_Base.IsDancing), "Is Dancing"},
        { nameof(DNC_Base.Esprit), "Esprit"},
        { nameof(DNC_Base.Feathers), "Feathers"},
        { nameof(DNC_Base.CompletedSteps), "Completed Steps"},
        #endregion

        #region DRG
        { nameof(DRG_Base.EyeCount), "Eye Count"},
        { nameof(DRG_Base.FocusCount), "Focus Count"},
        { nameof(DRG_Base.LOTDTime), "LOTD Time"},
        #endregion

        #region DRK
        { nameof(DRK_Base.Blood), "Blood"},
        { nameof(DRK_Base.HasDarkArts), "Has Dark Arts"},
        { nameof(DRK_Base.DarkSideTime), "Dark Side Time"},
        { nameof(DRK_Base.ShadowTime), "Shadow Side Time"},
        #endregion

        #region GNB
        { nameof(GNB_Base.Ammo), "Ammo"},
        { nameof(GNB_Base.AmmoComboStep), "Ammo Combo Step"},
        { nameof(GNB_Base.MaxAmmo), "Max Ammo"},
        #endregion    

        #region MCH
        { nameof(MCH_Base.IsOverheated), "Is Overheated"},
        { nameof(MCH_Base.Heat), "Heat"},
        { nameof(MCH_Base.Battery), "Battery"},
        { nameof(MCH_Base.OverheatTime), "Overheat Time Remaining"},
        #endregion

        #region MNK
        { nameof(MNK_Base.Chakra), "Chakra"},
        { nameof(MNK_Base.HasSolar), "Has Solar"},
        { nameof(MNK_Base.HasLunar), "Has Lunar"},
        #endregion

        #region NIN
        { nameof(NIN_Base.Ninki), "Ninki"},
        { nameof(NIN_Base.HutonTime), "Huton Time"},
        #endregion

        #region PLD
        { nameof(PLD_Base.HasDivineMight), "Has Divine Might"},
        { nameof(PLD_Base.HasFightOrFlight), "Has Fight Or Flight"},
        { nameof(PLD_Base.OathGauge), "OathGauge"},
        #endregion

        #region RDM
        { nameof(RDM_Base.WhiteMana), "White Mana"},
        { nameof(RDM_Base.BlackMana), "Black Mana"},
        { nameof(RDM_Base.ManaStacks), "Mana Stacks"},
        { nameof(RDM_Base.IsWhiteManaLargerThanBlackMana), "Is White Mana Larger Than Black Mana"},
        #endregion

        #region RPR
        { nameof(RPR_Base.HasEnshrouded), "Has Enshrouded"},
        { nameof(RPR_Base.HasSoulReaver), "Has Soul Reaver"},
        { nameof(RPR_Base.Soul), "Soul"},
        { nameof(RPR_Base.Shroud), "Shroud"},
        { nameof(RPR_Base.LemureShroud), "Lemure Shroud"},
        { nameof(RPR_Base.VoidShroud), "Void Shroud"},
        #endregion

        #region SAM
        { nameof(SAM_Base.HasMoon), "Has Moon"},
        { nameof(SAM_Base.HasFlower), "Has Flower"},
        { nameof(SAM_Base.IsMoonTimeLessThanFlower), "Is MoonTime Less Than Flower"},
        { nameof(SAM_Base.HasSetsu), "Has Setsu"},
        { nameof(SAM_Base.HasGetsu), "Has Getsu"},
        { nameof(SAM_Base.HasKa), "Has Ka"},
        { nameof(SAM_Base.Kenki), "Kenki"},
        { nameof(SAM_Base.MeditationStacks), "Meditation Stacks"},
        { nameof(SAM_Base.SenCount), "Sen Count"},
        #endregion

        #region SCH
        { nameof(SCH_Base.FairyGauge), "Fairy Gauge"},
        { nameof(SCH_Base.HasAetherflow), "Has Aetherflow"},
        { nameof(SCH_Base.SeraphTime), "Seraph Time"},
        #endregion

        #region SGE
        { nameof(SGE_Base.HasEukrasia), "Has Eukrasia"},
        { nameof(SGE_Base.Addersgall), "Addersgall"},
        { nameof(SGE_Base.Addersting), "Addersting"},
        { nameof(SGE_Base.AddersgallTime), "Addersgall Time"},
        #endregion

        #region SMN
        { nameof(SMN_Base.InBahamut), "In Bahamut"},
        { nameof(SMN_Base.InPhoenix), "In Phoenix"},
        { nameof(SMN_Base.HasAetherflowStacks), "Has Aetherflow Stacks"},
        { nameof(SMN_Base.Attunement), "Attunement"},
        { nameof(SMN_Base.IsIfritReady), "Is Ifrit Ready"},
        { nameof(SMN_Base.IsTitanReady), "Is Titan Ready"},
        { nameof(SMN_Base.IsGarudaReady), "Is Garuda Ready"},
        { nameof(SMN_Base.InIfrit), "In Ifrit"},
        { nameof(SMN_Base.InTitan), "In Titan"},
        { nameof(SMN_Base.InGaruda), "In Garuda"},
        { nameof(SMN_Base.SummonTime), "Summon Time"},
        { nameof(SMN_Base.AttunmentTime), "Attunement Time"},
        #endregion

        #region WAR
        { nameof(WAR_Base.BeastGauge), "Beast Gauge"},
        #endregion

        #region WHM
        { nameof(WHM_Base.Lily), "Lily"},
        { nameof(WHM_Base.BloodLily), "Blood Lily"},
        { nameof(WHM_Base.LilyTime), "Lily Time"},
        #endregion
    };

    public string HighEndWarning { get; set; } = "Please separately keybind damage reduction / shield cooldowns in case RS fails at a crucial moment in {0}!";
    public string TextToTalkWarning { get; set; } = "TextToTalk addon was not detected, please install it to make Rotation Solver give audio notifications!";
    public string AvariceWarning { get; set; } = "Avarice addon was not detected, please install it if you want to get the positional indicators for Rotation Solver!";

    public string ClickingMistakeMessage { get; set; } = "OOOps! RS clicked the wrong action ({0})!";

    public string ConfigWindow_About_Punchline { get; set; } = "Analyses PvE combat information every frame and finds the best action.";
    public string ConfigWindow_About_Description { get; set; } = "This means almost all the information available in one frame in combat, including the status of all players in the party, the status of any hostile targets, skill cooldowns, the MP and HP of characters, the location of characters, casting status of the hostile target, combo, combat duration, player level, etc.\n\nThen, it will highlight the best action on the hot bar, or help you to click on it.";

    public string ConfigWindow_About_Warning { get; set; } = "It is designed for GENERAL COMBAT, not for savage or ultimate. Use it carefully.";

    public string ConfigWindow_About_Macros { get; set; } = "Macros";
    public string ConfigWindow_About_Links { get; set; } = "Links";
    public string ConfigWindow_About_Compatibility { get; set; } = "Compatibility";
    public string ConfigWindow_About_Supporters { get; set; } = "Supporters";
    public string ConfigWindow_About_Compatibility_Description { get; set; } = "Literally, Rotation Solver helps you to choose the target and then click the action. So any plugin that changes these will affect its decision.\n\nHere is a list of known incompatible plugins:";
    public string ConfigWindow_About_Compatibility_Others { get; set; } = "Please don't relog without closing the game. Crashes may occur.";

    public string ConfigWindow_About_Compatibility_Mistake { get; set; } = "Can't properly execute the behavior that RS wants to do.";
    public string ConfigWindow_About_Compatibility_Mislead { get; set; } = "Misleading RS to make the right decision.";
    public string ConfigWindow_About_Compatibility_Crash { get; set; } = "Causes the game to crash.";

    public string ConfigWindow_Rotation_Description { get; set; } = "Description";
    public string ConfigWindow_Rotation_Status { get; set; } = "Status";
    public string ConfigWindow_Rotation_Configuration { get; set; } = "Configuration";
    public string ConfigWindow_Rotation_Rating { get; set; } = "Rating";
    public string ConfigWindow_Rotation_Information { get; set; } = "Information";
    public string ConfigWindow_Rotation_Rating_Description { get; set; } = "Here are some rating methods to analysis this rotation. Most of these methods need your engagement.";
    public string ConfigWindow_Rotation_Rating_CountOfLastUsing { get; set; } = "This is the count of using last action checking in this rotation. First is average one, second is maximum one. The less the better.\nLast used action is not a part of information from the game, it is recorded by player or author. \nIt can't accurately describe the current state of combat, which may make this rotation not general. \nFor example, clipping the gcd, death, take some status that grated by some action off manually, etc.";
    public string ConfigWindow_Rotation_Rating_CountOfCombatTimeUsing { get; set; } = "This is the count of using combat time in this rotation. First is average one, second is maximum one. The less the better.\nCombat time is not a part of information from the game, it is recorded by player or author. \nIt can't accurately describe the current state of combat, which may make this rotation not general.\nFor example, engaged by others in the party, different gcd time, etc.";

    public string ConfigWindow_Actions_Description { get; set; } = "To customize when Rotation Solver uses specific actions automatically, click on an action's icon in the left list. Below, you may set the conditions for when that specific action is used. Each action can have a different set of conditions to override the default rotation behavior.";

    public string ConfigWindow_Actions_ForcedConditionSet { get; set; } = "Forced Condition";
    public string ConfigWindow_Actions_ForcedConditionSet_Description { get; set; } = "Conditions for forced automatic use of action.";
    public string ConfigWindow_Actions_DisabledConditionSet { get; set; } = "Disabled Condition";
    public string ConfigWindow_Actions_DisabledConditionSet_Description { get; set; } = "Conditions for automatic use of action being disabled.";
    public string ConfigWindow_Actions_ShowOnCDWindow { get; set; } = "Show on CD window";
    public string ConfigWindow_Actions_IsInMistake { get; set; } = "Can be used by mistake";

    public string ConfigWindow_Configs_JobConfigTip { get; set; } = "This config is job specific";

    public string ConfigWindow_Rotations_Settings { get; set; } = "Settings";
    public string ConfigWindow_Rotations_Loaded { get; set; } = "Loaded";
    public string ConfigWindow_Rotations_GitHub { get; set; } = "GitHub";
    public string ConfigWindow_Rotations_Libraries { get; set; } = "Libraries";
    public string ConfigWindow_Rotations_AutoLoadCustomRotations { get; set; } = "Auto load rotations";

    public string ConfigWindow_Rotations_UserName { get; set; } = "User Name";
    public string ConfigWindow_Rotations_Repository { get; set; } = "Repository";

    public string ConfigWindow_Rotations_FileName { get; set; } = "File Name";

    public string ConfigWindow_Rotations_Library { get; set; } = "The folder contains rotation libs or the download url about rotation lib.";

    public string ConfigWindow_List_Statuses { get; set; } = "Statuses";
    public string ConfigWindow_List_Actions { get; set; } = "Actions";
    public string ConfigWindow_List_Territories { get; set; } = "Map specific settings";
    public string ConfigWindow_List_StatusNameOrId { get; set; } = "Status name or id";
    public string ConfigWindow_Actions_MemberName { get; set; } = "Member Name";
    public string ConfigWindow_List_AddStatus { get; set; } = "Add Status";
    public string ConfigWindow_List_Remove { get; set; } = "Remove";

    public string ConfigWindow_List_ActionNameOrId { get; set; } = "Action name or id";
    public string ConfigWindow_List_AddAction { get; set; } = "Add Action";

    public string ConfigWindow_List_BeneficialPositions { get; set; } = "Beneficial AoE locations";
    public string ConfigWindow_List_NoHostilesName { get; set; } = "The name of the enemy that you don't want to be targeted";
    public string ConfigWindow_List_NoProvokeName { get; set; } = "The name of the enemy that you don't want to be provoked";

    public string ConfigWindow_Basic_AutoSwitch { get; set; } = "Auto Switch";
    public string ConfigWindow_Basic_NamedConditions { get; set; } = "Named Conditions";
    public string ConfigWindow_Basic_Others { get; set; } = "Others";

    public string ConfigWindow_Basic_Timer { get; set; } = "Timer";
    public string ConfigWindow_UI_Windows { get; set; } = "Windows";
    public string ConfigWindow_UI_Overlay { get; set; } = "Overlay";
    public string ConfigWindow_UI_Information { get; set; } = "Information";
    public string ConfigWindow_Auto_ActionUsage { get; set; } = "Action Usage";
    public string ConfigWindow_Auto_ActionUsage_Description { get; set; } = "Which actions Rotation Solver can use.";
    public string ConfigWindow_Extra_Others { get; set; } = "Others";
    public string ConfigWindow_Extra_Description { get; set; } = "Rotation Solver focuses on the rotation itself. These are side features. If there are some other plugins can do that, these features will be deleted.";
    public string ConfigWindow_Auto_Description { get; set; } = "Change the way that RS atomatically uses actions.";
    public string ConfigWindow_Auto_ActionCondition { get; set; } = "Action Condition";
    public string ConfigWindow_Auto_StateCondition { get; set; } = "State Condition";
    public string ConfigWindow_Auto_ActionCondition_Description { get; set; } = "This will change the way that Rotation Solver uses actions.";
    public string ConfigWindow_Target_Config { get; set; } = "Configuration";
    public string ConfigWindow_Search_Result { get; set; } = "Search Result";
    public string ConfigWindow_UI_DrawIconAnimation { get; set; } = "Show RS logo animation";

    public string ConfigWindow_UI_ShowBeneficialPosition { get; set; } = "Show beneficial AoE locations.";
    public string ConfigWindow_UI_BeneficialPositionColor { get; set; } = "The color of beneficial AoE positions";
    public string ConfigWindow_UI_HoveredBeneficialPositionColor { get; set; } = "The color of the hovered beneficial position";

    public string ConfigWindow_Tab_Actions { get; set; } = "The abilities and custom conditions that your selected job has.";
    public string ConfigWindow_Tab_Rotations { get; set; } = "All rotations that RS has loaded.";
    public string ConfigWindow_Tab_List { get; set; } = "Some things that have to be lists.";
    public string ConfigWindow_Tab_Basic { get; set; } = "Basic settings.";
    public string ConfigWindow_Tab_UI { get; set; } = "Settings about the user interface.";
    public string ConfigWindow_Tab_Auto { get; set; } = "About some general actions usage and conditions.";
    public string ConfigWindow_Tab_Target { get; set; } = "The way to find the targets, hostiles or friends.";
    public string ConfigWindow_Tab_Extra { get; set; } = "Some features that shouldn't be included in RS but help you.";
    public string ConfigWindow_List_AddPosition { get; set; } = "Add beneficial AoE location";
    public string ConfigWindow_Actions_MoveUp { get; set; } = "Move Up";
    public string ConfigWindow_Actions_MoveDown { get; set; } = "Move Down";
    public string ConfigWindow_Actions_Copy { get; set; } = "Copy to Clipboard";
    public string ConfigWindow_NotInJob { get; set; } = "This option is unavailable while using your current job\n \nRoles or jobs needed:\n{0}";

    public string ConfigWindow_Searching { get; set; } = "Search...";
    public string ConfigWindow_UI_HideWarning { get; set; } = "Hide all warnings";
    public string ConfigWindow_Auto_BeneficialAreaStrategy { get; set; } = "Beneficial AoE strategy";
    public string ConfigWindow_About_OpenConfigFolder { get; set; } = "Open Config Folder";
    public string ConfigWindow_Basic_AnimationLockTime { get; set; } = "The Animation lock time from individual actions. Here is 0.6s for example.";
    public string ConfigWindow_Basic_Ping { get; set; } = "The ping time.\nIn RS, it means the time from sending the action request to receiving the using success message from the server.";
    public string ConfigWindow_Basic_IdealClickingTime { get; set; } = "The ideal click time.";
    public string ConfigWindow_Basic_RealClickingTime { get; set; } = "The real click time.";
    public string ConfigWindow_Basic_ClickingDuration { get; set; } = "The clicking duration, RS will try to click at this moment.";
    public string ConfigWindow_Basic_WeaponDelay { get; set; } = "This is the clipping time.\nGCD is over. However, RS forgets to click the next action.";
    public string ConfigWindow_About_ClickingCount { get; set; } = "Rotation Solver helped you by clicking actions {0:N0} times.";
    public string ConfigWindow_About_SayHelloCount { get; set; } = "You have said hello to other users {0:N0} times!";
    public string ConfigWindow_Auto_AutoHealTimeToKill { get; set; } = "Stop healing when time to kill is lower then...";
    public string ConfigWindow_UI_ShowHostiles { get; set; } = "Show the hostile target icon";
    public string ConfigWindow_UI_HostileIconHeight { get; set; } = "Hostile Icon height from position";
    public string ConfigWindow_UI_HostileIconSize { get; set; } = "Hostile Icon size";

    public string ConfigWindow_UI_StateIconHeight { get; set; } = "State icon height";
    public string ConfigWindow_UI_StateIconSize { get; set; } = "State icon size";
    public string ConfigWindow_UI_ShowStateIcon { get; set; } = "Show RS state icon";
    public string ConfigWindow_Auto_UseGroundBeneficialAbilityWhenMoving { get; set; } = "Use beneficial AoE actions when moving.";
    public string ConfigWindow_Auto_AutoDefenseNumber { get; set; } = "The number hostile targets attacking you. If it's larger than this RS uses personal mitigation abilities.";

    public string ConfigWindow_Target_OnlyAttackInVisionCone { get; set; } = "Only attack the targets in vision cone";
    public string ConfigWindow_Target_VisionCone { get; set; } = "The angle of your vision cone";

    public string ConfigWindow_Auto_ProvokeDelay { get; set; } = "The delay of provoke.";

    public string ConfigWindow_UI_TTKTextColor { get; set; } = "The text color of the time to kill indicator.";
    public string ConfigWindow_Basic_MinUpdatingTime { get; set; } = "The minimum time between updating RS information.";

    public string ConfigWindow_Actions_AOECount { get; set; } = "How many targets are needed to use this action.";
    public string ConfigWindow_Actions_TTK { get; set; } = "TTK that this action needs the target be.";
    public string ConfigWindow_Actions_HealRatio { get; set; } = "The HP ratio to auto heal";
    public string ConfigWindow_Actions_ConditionDescription { get; set; } = "Forced Conditions have a higher priority. If Forced Conditions are met, Disabled Condition will be ignored.";

    public string ConfigWindow_Auto_UseResourcesAction { get; set; } = "Use actions that use resources";
    public string ConfigWindow_Auto_OnlyHealSelfWhenNoHealer { get; set; } = "Only Heal self When Not a healer";

    public string ConfigWindow_Auto_HealthForAutoDefense { get; set; } = "HP Ratio about defense single of Tanks";
    public string ConfigWindow_Basic_SayHelloToUsers { get; set; } = "Say hello to the users of Rotation Solver.";
    public string ConfigWindow_Basic_SayHelloToAll { get; set; } = "Say hello to all users of Rotation Solver.";
    public string ConfigWindow_Basic_SayHelloToUsersDesc { get; set; } = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed in Discord Server with your Hash!";
    public string ConfigWindow_Basic_JustSayHelloOnce { get; set; } = "Just say hello once to the same user.";

    public string ConfigWindow_About_Clicking100k { get; set; } = "Well, you must be a lazy player!";
    public string ConfigWindow_About_Clicking500k { get; set; } = "You're tiring RS out, give it a break!";

    public string ConfigWindow_About_ThanksToSupporters { get; set; } = "Many thanks to the sponsors.";
    public string ConfigWindow_Rotations_Download { get; set; } = "Download Rotations";
    public string ConfigWindow_Rotations_Links { get; set; } = "Links of the rotations online";
    public string ConfigWindow_Options_ForcedEnableCondition { get; set; } = "Use Forced Enable Condition";
    public string ConfigWindow_Options_ForcedEnableConditionDesc { get; set; } = "The conditions of forced to make it true.";

    public string ConfigWindow_Options_ForcedDisableCondition { get; set; } = "Use Forced Disable Condition";
    public string ConfigWindow_Options_ForcedDisableConditionDesc { get; set; } = "The conditions of forced to make it false.";

    public string ConfigWindow_Auto_HealAreaConditionSet { get; set; } = "Heal Area Forced Condition";
    public string ConfigWindow_Auto_HealSingleConditionSet { get; set; } = "Heal Single Forced Condition";
    public string ConfigWindow_Auto_DefenseAreaConditionSet { get; set; } = "Defense Area Forced Condition";
    public string ConfigWindow_Auto_DefenseSingleConditionSet { get; set; } = "Defense Single Forced Condition";
    public string ConfigWindow_Auto_EsunaStanceNorthConditionSet { get; set; } = "Esuna Stance North Forced Condition";
    public string ConfigWindow_Auto_RaiseShirkConditionSet { get; set; } = "Raise Shirk Forced Condition";
    public string ConfigWindow_Auto_MoveForwardConditionSet { get; set; } = "Move Forward Forced Condition";
    public string ConfigWindow_Auto_MoveBackConditionSet { get; set; } = "Move Back Forced Condition";
    public string ConfigWindow_Auto_AntiKnockbackConditionSet { get; set; } = "Anti Knockback Forced Condition";
    public string ConfigWindow_Auto_SpeedConditionSet { get; set; } = "Speed Forced Condition";
    public string ConfigWindow_Auto_LimitBreakConditionSet { get; set; } = "Limit Break Condition";
    public string ConfigWindow_ConditionSetDesc { get; set; } = "The Condition set you chose, click to modify.";
    public string ConfigWindow_Basic_UseAdditionalConditions { get; set; } = "Use additional conditions";
    public string ConfigWindow_Basic_SwitchCancelConditionSet { get; set; } = "Auto turn off conditions";
    public string ConfigWindow_Basic_SwitchManualConditionSet { get; set; } = "Auto turn manual conditions";
    public string ConfigWindow_Basic_SwitchAutoConditionSet { get; set; } = "Auto turn auto conditions";

    public string ConfigWindow_Condition_RotationNullWarning { get; set; } = "Rotation is null, please login or switch the job!";

    public string ConfigWindow_Condition_NoItemsWarning { get; set; } = "There are no items!";

    public string ConfigWindow_Condition_ConditionName { get; set; } = "Condition Name";

    public string ConfigWindow_Condition_TerritoryName { get; set; } = "Territory Name";

    public string ConfigWindow_Condition_DutyName { get; set; } = "Duty Name";
    public string ConfigWindow_Condition_TargetWarning { get; set; } = "You'd better not use it. Because this target isn't the action's target. Try to pick it from action.";
}