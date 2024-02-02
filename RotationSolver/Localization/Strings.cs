namespace RotationSolver.Localization;

[Obsolete]
internal class Strings
{
    #region Commands
    public string Commands_CannotFindConfig { get; set; } = "Failed to find the config, please check it.";

    #endregion

    #region ConfigWindow
    public string ConfigWindow_Header { get; set; } = "Rotation Solver Settings v";
    public string ConfigWindow_EventItem { get; set; } = "Event";

    public string ConfigWindow_HelpItem_NextAction { get; set; } = "Do the next action";


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




    public string ConfigWindow_Param_CountDownAhead { get; set; } = "The starting when abilities will be used before finishing the countdown";
    public string ConfigWindow_Param_AddDotGCDCount { get; set; } = "The number of GCDs in advance for DOT refreshing";


    public string ConfigWindow_Param_WeakenDelay { get; set; } = "The range of random delay for cleansing dispellable debuffs.";

    public string ConfigWindow_Param_HealDelay { get; set; } = "The range of random delay for healing.";

    public string ConfigWindow_Param_CountdownDelay { get; set; } = "The random delay between which auto mode activation on countdown varies.";


    public string ConfigWindow_Param_DyingTimeToKill { get; set; } = "If target's time until death is lower than this, regard it is dying.";



    public string ConfigWindow_Param_LessMPNoRaise { get; set; } = "Never raise player if MP is less than the set value";
    public string ConfigWindow_Param_InterruptibleMoreCheck { get; set; } = "Use interrupt abilities if possible.";


    public string ConfigWindow_Param_BeneficialAreaOnLocations { get; set; } = "On predefined location";
    public string ConfigWindow_Param_BeneficialAreaOnlyOnLocations { get; set; } = "Only on predefined location";
    public string ConfigWindow_Param_BeneficialAreaOnTarget { get; set; } = "On target";

    public string ConfigWindow_Param_BeneficialAreaOnCalculated { get; set; } = "On the calculated location";

    public string ConfigWindow_Param_RightNowTargetToHostileType { get; set; } = "Engage settings";
    public string ConfigWindow_Param_AddEnemyListToHostile { get; set; } = "Add enemy list to the hostile targets.";
    public string ConfigWindow_Param_AttackMarkAOEDesc { get; set; } = "Attention: Checking this option , AA will attack as many hostile targets as possible, while ignoring whether the attack will reach the marked target.";
    public string ConfigWindow_Param_MoveTargetAngle { get; set; } = "The size of the sector angle that can be selected as the moveable target";
    public string ConfigWindow_Param_MoveTargetAngleDesc { get; set; } = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are moveable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.";



    public string ConfigWindow_Param_RaiseBrinkOfDeath { get; set; } = "Raise players that even have Brink of Death debuff";

    public string ConfigWindow_Param_HostileDesc { get; set; } = "You can The logic of hostile target selection to allow flexibility in switching the logic of selecting hostile in battle.";
    public string ConfigWindow_Param_HostileCondition { get; set; } = "Hostile target selection condition";
    public string ConfigWindow_Control_CooldownWindowIconSize { get; set; } = "Cooldown icon size";
    public string ConfigWindow_Rotation_BetaRotation { get; set; } = "Beta Rotation!";

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

    #region COnfigUnitType
    public string ConfigUnitType_Seconds { get; set; } = "Time Unit, in seconds.";
    public string ConfigUnitType_Degree { get; set; } = "Angle Unit, in degrees.";
    public string ConfigUnitType_Pixels { get; set; } = "Display Unit, in pixels.";
    public string ConfigUnitType_Yalms { get; set; } = "Distance Unit, in yalms.";
    public string ConfigUnitType_Ratio { get; set; } = "Ratio Unit, as percentage.";

    #endregion

    public Dictionary<string, string> MemberInfoName { get; set; } = new Dictionary<string, string>()
    {
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

    [Obsolete]
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

    public string ConfigWindow_Rotations_Settings { get; set; } = "Settings";
    public string ConfigWindow_Rotations_Loaded { get; set; } = "Loaded";
    public string ConfigWindow_Rotations_GitHub { get; set; } = "GitHub";
    public string ConfigWindow_Rotations_Libraries { get; set; } = "Libraries";

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
    public string ConfigWindow_Extra_Others { get; set; } = "Others";
    public string ConfigWindow_Extra_Description { get; set; } = "Rotation Solver focuses on the rotation itself. These are side features. If there are some other plugins can do that, these features will be deleted.";
    public string ConfigWindow_Auto_ActionCondition { get; set; } = "Action Condition";
    public string ConfigWindow_Auto_StateCondition { get; set; } = "State Condition";
    public string ConfigWindow_Auto_ActionCondition_Description { get; set; } = "This will change the way that Rotation Solver uses actions.";
    public string ConfigWindow_Target_Config { get; set; } = "Configuration";
    public string ConfigWindow_Search_Result { get; set; } = "Search Result";
    public string ConfigWindow_List_AddPosition { get; set; } = "Add beneficial AoE location";
    public string ConfigWindow_Actions_MoveUp { get; set; } = "Move Up";
    public string ConfigWindow_Actions_MoveDown { get; set; } = "Move Down";
    public string ConfigWindow_Actions_Copy { get; set; } = "Copy to Clipboard";

    public string ConfigWindow_About_OpenConfigFolder { get; set; } = "Open Config Folder";
    public string ConfigWindow_Basic_AnimationLockTime { get; set; } = "The Animation lock time from individual actions. Here is 0.6s for example.";
    public string ConfigWindow_Basic_Ping { get; set; } = "The ping time.\nIn RS, it means the time from sending the action request to receiving the using success message from the server.";
    public string ConfigWindow_Basic_IdealClickingTime { get; set; } = "The ideal click time.";
    public string ConfigWindow_Basic_RealClickingTime { get; set; } = "The real click time.";
    public string ConfigWindow_Basic_ClickingDuration { get; set; } = "The clicking duration, RS will try to click at this moment.";
    public string ConfigWindow_About_ClickingCount { get; set; } = "Rotation Solver helped you by clicking actions {0:N0} times.";
    public string ConfigWindow_About_SayHelloCount { get; set; } = "You have said hello to other users {0:N0} times!";


    public string ConfigWindow_Auto_AutoDefenseNumber { get; set; } = "The number hostile targets attacking you. If it's larger than this RS uses personal mitigation abilities.";

    public string ConfigWindow_Auto_ProvokeDelay { get; set; } = "The delay of provoke.";


    public string ConfigWindow_Actions_AOECount { get; set; } = "How many targets are needed to use this action.";
    public string ConfigWindow_Actions_TTK { get; set; } = "TTK that this action needs the target be.";
    public string ConfigWindow_Actions_HealRatio { get; set; } = "The HP ratio to auto heal";
    public string ConfigWindow_Actions_ConditionDescription { get; set; } = "Forced Conditions have a higher priority. If Forced Conditions are met, Disabled Condition will be ignored.";

    public string ConfigWindow_Basic_SayHelloToUsers { get; set; } = "Say hello to the users of Rotation Solver.";


    public string ConfigWindow_About_Clicking100k { get; set; } = "Well, you must be a lazy player!";
    public string ConfigWindow_About_Clicking500k { get; set; } = "You're tiring RS out, give it a break!";

    public string ConfigWindow_About_ThanksToSupporters { get; set; } = "Many thanks to the sponsors.";
    public string ConfigWindow_Rotations_Download { get; set; } = "Download Rotations";
    public string ConfigWindow_Rotations_Links { get; set; } = "Links of the rotations online";
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