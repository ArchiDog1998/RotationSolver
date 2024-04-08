using Dalamud.Configuration;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration.Timeline;
using RotationSolver.Data;
using XIVConfigUI;

namespace RotationSolver.Basic.Configuration;

internal partial class Configs : IPluginConfiguration
{
    public int Version { get; set; } = 8;

    public List<ActionEventInfo> Events { get; private set; } = [];
    public SortedSet<Job> DisabledJobs { get; private set; } = [];

    public string[] OtherLibs { get; set; } = [];

    public string[] GitHubLibs { get; set; } = [];
    public List<TargetingType> TargetingTypes { get; set; } = [];

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();

    [UI("Show RS logo animation.", (int)UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean DrawIconAnimation { get; private set; } = new(true, nameof(DrawIconAnimation));

    [UI("Turns auto mode  off when player is switching between different maps.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch)]
    public ConditionBoolean AutoOffBetweenArea { get; private set; } = new(true, nameof(AutoOffBetweenArea));

    [UI("Turns auto mode off during cutscenes.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch)]
    public ConditionBoolean AutoOffCutScene { get; private set; } = new(true, nameof(AutoOffCutScene));

    [UI("Turns auto mode off when you die.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch)]
    public ConditionBoolean AutoOffWhenDead { get; private set; } = new(true, nameof(AutoOffWhenDead));

    [UI("Turns auto mode off when the duty in progress is completed.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch)]
    public ConditionBoolean AutoOffWhenDutyCompleted { get; private set; } = new(true, nameof(AutoOffWhenDutyCompleted));

    [UI("Consider only fate targets in the current fate area",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    public ConditionBoolean ChangeTargetForFate  { get; private set; } = new(true, nameof(ChangeTargetForFate));

    [UI("Use movement actions towards the closest object from the center of the screen",(int)UiString.ConfigWindow_Target_Config,
        Description = "Use movement actions towards the closest object from the center of the screen, otherwise toward your current facing direction.",
         Section = 2)]
    public ConditionBoolean MoveTowardsScreenCenter { get; private set; } = new(true, nameof(MoveTowardsScreenCenter));

    [UI("Audio notification for addon mode changes",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean SayOutStateChanged { get; private set; } = new(true, nameof(SayOutStateChanged));

    [UI("Display plugin status in server info bar",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean ShowInfoOnDtr { get; private set; } = new(true, nameof(ShowInfoOnDtr));

    [UI("Heals players when not in combat.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section =1)]
    public ConditionBoolean HealOutOfCombat { get; private set; } = new(false, nameof(HealOutOfCombat));

    [UI("Displays plugin status with toast notifications",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean ShowInfoOnToast { get; private set; } = new(true, nameof(ShowInfoOnToast));

    [UI("Lock the movement when casting or when doing some other actions.", (int)UiString.ConfigWindow_Extra_Others)]
    public ConditionBoolean PoslockCasting { get; private set; } = new(true, nameof(PoslockCasting));

    [UI("", Action = (uint)ActionID.PassageOfArmsPvE, Parent = nameof(PoslockCasting))]
    public bool PosPassageOfArms { get; set; } = false;

    [UI("", Action = (uint)ActionID.TenChiJinPvE, Parent = nameof(PoslockCasting))]
    public bool PosTenChiJin { get; set; } = true;

    [UI("", Action = (uint)ActionID.FlamethrowerPvE, Parent = nameof(PoslockCasting))]
    public bool PosFlameThrower { get; set; } = false;

    [UI("", Action = (uint)ActionID.ImprovisationPvE, Parent = nameof(PoslockCasting))]
    public bool PosImprovisation { get; set; } = false;

    [UI("Replace the action icon.", (int)UiString.ConfigWindow_Extra_Others)]
    public ConditionBoolean ReplaceIcon { get; private set; } = new(false, nameof(ReplaceIcon));

    [UI("", Action = (uint)ActionID.LegGrazePvE, Parent = nameof(ReplaceIcon))]
    public bool ReplaceLegGraze { get; set; } = true;

    [UI("", Action = (uint)ActionID.FootGrazePvE, Parent = nameof(ReplaceIcon))]
    public bool ReplaceFootGraze { get; set; } = true;

    [UI("", Action = (uint)ActionID.ReposePvE, Parent = nameof(ReplaceIcon))]
    public bool ReplaceRepose { get; set; } = true;

    [UI("", Action = (uint)ActionID.SleepPvE, Parent = nameof(ReplaceIcon))]
    public bool ReplaceSleep { get; set; } = true;

    [JobFilter(PvE = JobFilterType.Raise, PvP = JobFilterType.NoJob)]
    [UI("Raise player while swiftcast is on cooldown",
        (int)UiString.ConfigWindow_Auto_ActionUsage, Section = 2)]
    public ConditionBoolean RaisePlayerByCasting { get; private set; } = new(true, nameof(RaisePlayerByCasting));

    [JobFilter(PvE = JobFilterType.Raise, PvP = JobFilterType.NoJob)]
    [UI("Raise any player in range (even if they are not in your party)",
        (int)UiString.ConfigWindow_Auto_ActionUsage, Section = 2)]
    public ConditionBoolean RaiseAll { get; private set; } = new(false, nameof(RaiseAll));

    [JobFilter(PvE = JobFilterType.Raise, PvP = JobFilterType.NoJob)]
    [UI("Raise players that have the Brink of Death debuff",
        (int)UiString.ConfigWindow_Auto_ActionUsage, Section = 2)]
    public ConditionBoolean RaiseBrinkOfDeath { get; private set; } = new(true, nameof(RaiseBrinkOfDeath));

    [JobFilter(PvE = JobFilterType.Raise, PvP = JobFilterType.NoJob)]
    [UI("Random delay for considering a ressurection type action.",
        (int)UiString.ConfigWindow_Auto_ActionUsage, Section = 2)]
    [Range(0, 10, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 RaiseDelay { get; set; } = new(1, 2);

    [UI("Add enemy list to the hostile targets.",
        (int)UiString.ConfigWindow_Target_Config)]
    public ConditionBoolean AddEnemyListToHostile { get; private set; } = new(true, nameof(AddEnemyListToHostile));

    [UI("Only attack targets in the enemy list.",
        Parent = nameof(AddEnemyListToHostile))]
    public ConditionBoolean OnlyAttackInEnemyList { get; private set; } = new(false, nameof(OnlyAttackInEnemyList));

    [UI("Use Tinctures.", (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public ConditionBoolean UseTinctures { get; private set; } = new(false, nameof(UseTinctures));

    [UI("Use HP Potions.", (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public ConditionBoolean UseHpPotions { get; private set; } = new(false, nameof(UseHpPotions));

    [UI("Use MP Potions.", (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public ConditionBoolean UseMpPotions { get; private set; } = new(false, nameof(UseMpPotions));

    [UI("Draw the melee buffer area on the screen.", (int)UiString.ConfigWindow_UI_Overlay,
        Description = "Shows the area where no actions will be used between ranged and melee type actions.")]
    public ConditionBoolean DrawMeleeOffset { get; private set; } = new(true, nameof(DrawMeleeOffset));

    [UI("Shows the target of the movement action.",
        (int)UiString.ConfigWindow_UI_Overlay)]
    public ConditionBoolean ShowMoveTarget { get; private set; } = new(true, nameof(ShowMoveTarget));

    [UI("Shows target related drawing.", (int)UiString.ConfigWindow_UI_Overlay,
        Description = "Shows the next ability under the target and AoE attacks effect area")]
    public ConditionBoolean ShowTarget { get; private set; } = new(true, nameof(ShowTarget));

    [UI("Show the target's estimated time to kill.",
        Parent = nameof(ShowTarget))]
    public ConditionBoolean ShowTargetTimeToKill { get; private set; } = new(false, nameof(ShowTargetTimeToKill));

    [UI("Priority attacks targets with attack markers.",
        (int)UiString.ConfigWindow_Target_Config)]
    public ConditionBoolean ChooseAttackMark { get; private set; } = new(true, nameof(ChooseAttackMark));

    [UI("Allows use of AoE abilities to attack as many targets as possibly.",
        Parent = nameof(ChooseAttackMark))]
    public ConditionBoolean CanAttackMarkAoe { get; private set; } = new(true, nameof(CanAttackMarkAoe));

    [UI("Never attack targets with stop markers.",
        (int)UiString.ConfigWindow_Target_Config)]
    public ConditionBoolean FilterStopMark { get; private set; } = new(true, nameof(FilterStopMark));

    [UI ("Shows the hostile targets icon.",
        (int)UiString.ConfigWindow_UI_Overlay)]
    public ConditionBoolean ShowHostilesIcons { get; private set; } = new(true, nameof(ShowHostilesIcons));

    [UI ("Teaching mode.", (int)UiString.ConfigWindow_UI_Overlay,
        Description = "Shows the next suggested ability that should be used")]
    
    public ConditionBoolean TeachingMode { get; private set; } = new(true, nameof(TeachingMode));

    [UI("Display UI Overlay.", (int)UiString.ConfigWindow_UI_Overlay,
        Description = "This overlay is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.")]
    public ConditionBoolean UseOverlayWindow { get; private set; } = new(true, nameof(UseOverlayWindow));

    [UI("Simulates the effect of pressing abilities.",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean KeyBoardNoise { get; private set; } = new(true, nameof(KeyBoardNoise));

    [UI("Targets movement area ability to the farthest possible location.", (int)UiString.ConfigWindow_Target_Config,
        Description = "Moves to the furthest possible position that can be targeted with movement actions.", Section = 2)]
    public ConditionBoolean MoveAreaActionFarthest { get; private set; } = new(true, nameof(MoveAreaActionFarthest));

    [UI("Auto mode activation delay on countdown start.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch, Section = 1)]
    public ConditionBoolean StartOnCountdown { get; private set; } = new(true, nameof(StartOnCountdown));

    [UI("Automatically turns on manual mode and targets enemy when being attacked.",
        (int)UiString.ConfigWindow_Basic_AutoSwitch, Section =1)]
    public ConditionBoolean StartOnAttackedBySomeone { get; private set; } = new(false, nameof(StartOnAttackedBySomeone));

    [UI("Don't attack new mobs by AoE.", Description = "Avoids usage of AoE abilities when new enemies would be aggroed.",
        Parent =nameof(UseAoeAction))]
    public ConditionBoolean NoNewHostiles { get; private set; } = new(false, nameof(NoNewHostiles));

    [JobFilter(PvE = JobFilterType.NoHealer, PvP = JobFilterType.NoJob)]
    [UI("Use healing abilities when playing a non-healer role.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    public ConditionBoolean UseHealWhenNotAHealer { get; private set; } = new(true, nameof(UseHealWhenNotAHealer));

    [UI("Target allies for friendly actions.",
        (int)UiString.ConfigWindow_Target_Config, Section = 3)]
    public ConditionBoolean SwitchTargetFriendly { get; private set; } = new(false, nameof(SwitchTargetFriendly));

    [JobFilter(PvE = JobFilterType.Interrupt, PvP = JobFilterType.NoJob)]
    [UI("Use interrupt abilities if possible.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    public ConditionBoolean InterruptibleMoreCheck { get; private set; } = new(true, nameof(InterruptibleMoreCheck));

    [UI("Use work task for acceleration.",
        (int)UiString.ConfigWindow_Basic_Others)]
    public ConditionBoolean UseWorkTask { get; private set; } = new(false, nameof(UseWorkTask));

    [UI("Stops casting when the target is dead.", (int)UiString.ConfigWindow_Extra_Others)]
    public ConditionBoolean UseStopCasting { get; private set; } = new(false, nameof(UseStopCasting));

    [JobFilter(PvE = JobFilterType.Dispel, PvP = JobFilterType.NoJob)]
    [UI("Cleanse all dispellable debuffs.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    public ConditionBoolean DispelAll { get; private set; } = new(false, nameof(DispelAll));

    [UI("Only attacks targets within camera view.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    public ConditionBoolean OnlyAttackInView { get; private set; } = new(false, nameof(OnlyAttackInView));

    [UI("Only attack the targets within the character's vision cone.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    public ConditionBoolean OnlyAttackInVisionCone { get; private set; } = new(false, nameof(OnlyAttackInVisionCone));

    [JobFilter(PvE = JobFilterType.Healer, PvP = JobFilterType.Healer)]
    [UI("Use single target healing over time actions only on tanks.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    public ConditionBoolean OnlyHotOnTanks { get; private set; } = new(false, nameof(OnlyHotOnTanks));

    [UI("Debug Mode.", -1)]
    public ConditionBoolean InDebug { get; private set; } = new(false, nameof(InDebug));

    [UI("Auto Download Rotations.", (int)UiString.ConfigWindow_Rotations_Settings)]
    public ConditionBoolean DownloadRotations { get; private set; } = new(true, nameof(DownloadRotations));

    [UI("Auto Update Rotations.", Parent = nameof(DownloadRotations))]
    public ConditionBoolean AutoUpdateRotations { get; private set; } = new(true, nameof(AutoUpdateRotations));

    [UI("Make /rotation Manual as a toggle command.",
        (int)UiString.ConfigWindow_Basic_Others)]
    public ConditionBoolean ToggleManual { get; private set; } = new(false, nameof(ToggleManual));

    [UI("Make /rotation Auto as a toggle command.",
        (int)UiString.ConfigWindow_Basic_Others)]
    public ConditionBoolean ToggleAuto { get; private set; } = new(false, nameof(ToggleAuto));

    [UI("Only show these windows if there are enemies in or in duty.",
        (int)UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean OnlyShowWithHostileOrInDuty { get; private set; } = new(true, nameof(OnlyShowWithHostileOrInDuty));

    [UI("Show Control Window.",
        (int)UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean ShowControlWindow { get; private set; } = new(false, nameof(ShowControlWindow));

    [UI("Lock Control Window.",
        (int)UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean IsControlWindowLock { get; private set; } = new(false, nameof(IsControlWindowLock));

    [UI("Show Next Action Window.", (byte) UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean ShowNextActionWindow { get; private set; } = new(true, nameof(ShowNextActionWindow));

    [UI("No Inputs.", Parent = nameof(ShowNextActionWindow))]
    public ConditionBoolean IsInfoWindowNoInputs { get; private set; } = new(false, nameof(IsInfoWindowNoInputs));

    [UI("No Move.", Parent = nameof(ShowNextActionWindow))]
    public ConditionBoolean IsInfoWindowNoMove { get; private set; } = new(false, nameof(IsInfoWindowNoMove));

    [UI("Show Items Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    public ConditionBoolean ShowItemsCooldown { get; private set; } = new(false, nameof(ShowItemsCooldown));

    [UI("Show GCD Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    public ConditionBoolean ShowGcdCooldown { get; private set; } = new(false, nameof(ShowGcdCooldown));

    [UI("Show Original Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    public ConditionBoolean UseOriginalCooldown { get; private set; } = new(true, nameof(UseOriginalCooldown));

    [UI("Show tooltips.",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean ShowTooltips { get; private set; } = new(true, nameof(ShowTooltips));

    [UI("Auto load custom rotation files.",
        (int)UiString.ConfigWindow_Rotations_Settings)]
    public ConditionBoolean AutoLoadCustomRotations { get; private set; } = new(false, nameof(AutoLoadCustomRotations));

    [UI("Prioritize fate targets.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    public ConditionBoolean TargetFatePriority { get; private set; } = new(true, nameof(TargetFatePriority));

    [UI("Prioritize Hunt/Relic/Leve targets.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    public ConditionBoolean TargetHuntingRelicLevePriority { get; private set; } = new(true, nameof(TargetHuntingRelicLevePriority));

    [UI("Prioritize Quest targets.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]

    public ConditionBoolean TargetQuestPriority { get; private set; } = new(true, nameof(TargetQuestPriority));

    [UI("Display manually triggered actions feedback on toast",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean ShowToastsAboutDoAction { get; private set; } = new(true, nameof(ShowToastsAboutDoAction));

    [UI("Use AoE actions.", (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public ConditionBoolean UseAoeAction { get; private set; } = new(true, nameof(UseAoeAction));

    [UI("Use AoE actions in manual mode.", Parent = nameof(UseAoeAction))]
    public ConditionBoolean UseAoeWhenManual { get; private set; } = new(false, nameof(UseAoeWhenManual));

    [UI("Automatically trigger dps burst phase.", (int)UiString.ConfigWindow_Auto_ActionCondition)]
    public ConditionBoolean AutoBurst { get; private set; } = new(true, nameof(AutoBurst));

    [UI("Automatic Healing.", (int)UiString.ConfigWindow_Auto_ActionCondition)]
    public ConditionBoolean AutoHeal { get; private set; } = new(true, nameof(AutoHeal));

    [UI("Automatic offensive ability use.", (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public ConditionBoolean UseAbility { get; private set; } = new(true, nameof(UseAbility));

    [UI("Automatically use defensive abilities.", Description = "It is recommended to uncheck this option if you are playing high end content or if you can plan healing and defensive ability usage by yourself.",
        Parent = nameof(UseAbility))]
    public ConditionBoolean UseDefenseAbility { get; private set; } = new(true, nameof(UseDefenseAbility));

    [JobFilter(PvE = JobFilterType.Tank)]
    [UI("Automatically activate tank stance.", Parent =nameof(UseAbility))]
    public ConditionBoolean AutoTankStance { get; private set; } = new(true, nameof(AutoTankStance));

    [JobFilter(PvE = JobFilterType.Tank)]

    [UI("Auto provoke non-tank attacking targets.", Description = "Automatically use provoke when an enemy is attacking a non-tank member of the party.", Parent = nameof(UseAbility))]
    public ConditionBoolean AutoProvokeForTank { get; private set; } = new(true, nameof(AutoProvokeForTank));

    [JobFilter(PvE = JobFilterType.Melee)]
    [UI("Auto TrueNorth (Melee).", Parent = nameof(UseAbility))]
    public ConditionBoolean AutoUseTrueNorth { get; private set; } = new(true, nameof(AutoUseTrueNorth));

    [JobFilter(PvE = JobFilterType.Healer)]
    [UI("Raise player by using swiftcast if available",
        Parent = nameof(UseAbility))]
    public ConditionBoolean RaisePlayerBySwift { get; private set; } = new(true, nameof(RaisePlayerBySwift));

    [UI("Use movement speed increase abilities when out of combat.", Parent = nameof(UseAbility))]
    public ConditionBoolean AutoSpeedOutOfCombat { get; private set; } = new(true, nameof(AutoSpeedOutOfCombat));

    [JobFilter(PvE = JobFilterType.Healer)]
    [UI("Use beneficial ground-targeted actions.", Parent = nameof(UseAbility))]
    public ConditionBoolean UseGroundBeneficialAbility { get; private set; } = new(true, nameof(UseGroundBeneficialAbility));

    [UI("Use Anti-Knockback abilities", Parent = nameof(UseAbility))]
    public ConditionBoolean UseKnockback { get; private set; } = new(true, nameof(UseKnockback));

    [UI("Use beneficial AoE actions while moving.", Parent = nameof(UseGroundBeneficialAbility))]
    public ConditionBoolean UseGroundBeneficialAbilityWhenMoving { get; private set; } = new(false, nameof(UseGroundBeneficialAbilityWhenMoving));

    [UI("Consider all players for friendly actions (include passerby).",
        (int)UiString.ConfigWindow_Target_Config, Section = 3)]
    public ConditionBoolean TargetAllForFriendly { get; private set; } = new(false, nameof(TargetAllForFriendly));

    [UI("Show Cooldown Window.", (int)UiString.ConfigWindow_UI_Windows)]
    public ConditionBoolean ShowCooldownWindow { get; private set; } = new(false, nameof(ShowCooldownWindow));

    [UI("Record AoE actions.", (int)UiString.ConfigWindow_List_HostileCastingArea)]
    public ConditionBoolean RecordCastingArea { get; private set; } = new(true, nameof(RecordCastingArea));

    [UI("Record Knockback actions.", (int)UiString.ConfigWindow_List_HostileCastingKnockback)]
    public ConditionBoolean RecordKnockback { get; private set; } = new(true, nameof(RecordKnockback));

    [UI("Auto turn off RS when combat is over more for more then...",
        (int)UiString.ConfigWindow_Basic_AutoSwitch)]
    public ConditionBoolean AutoOffAfterCombat { get; private set; } = new(true, nameof(AutoOffAfterCombat));

    [UI("Auto open treasure chests",
        (int)UiString.ConfigWindow_Extra_Others)]
    public ConditionBoolean AutoOpenChest { get; private set; } = new(false, nameof(AutoOpenChest));

    [UI("Auto close the loot window when auto opened the chest.",
        Parent = nameof(AutoOpenChest))]
    public ConditionBoolean AutoCloseChestWindow { get; private set; } = new(true, nameof(AutoCloseChestWindow));

    [UI("Show RS state icon.", (int)UiString.ConfigWindow_UI_Overlay)]
    public ConditionBoolean ShowStateIcon { get; private set; } = new(true, nameof(ShowStateIcon));

    [UI("Show beneficial AoE locations.", (int)UiString.ConfigWindow_UI_Overlay)]
    public ConditionBoolean ShowBeneficialPositions { get; private set; } = new(true, nameof(ShowBeneficialPositions));

    [UI("Hide all warnings.", (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean HideWarning { get; private set; } = new(false, nameof(HideWarning));

    [UI("Heal party members using GCD healing if there is nothing to do while in combat.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    public ConditionBoolean HealWhenNothingTodo { get; private set; } = new(true, nameof(HealWhenNothingTodo));

    [UI("Say hello to all other users of Rotation Solver.",
        (int)UiString.ConfigWindow_Basic_Others)]
    public ConditionBoolean SayHelloToAll { get; private set; } = new(true, nameof(SayHelloToAll));

    [UI("Say hello to the users of Rotation Solver.", Description = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed with your Hash!",
        Parent =nameof(SayHelloToAll))]
    public ConditionBoolean SayHelloToUsers { get; private set; } = new(true, nameof(SayHelloToUsers));

    [UI("Say hello only one time to the same user.",
        Parent = nameof(SayHelloToAll))]
    public ConditionBoolean JustSayHelloOnce { get; private set; } = new(false, nameof(JustSayHelloOnce));

    [JobFilter(PvP = JobFilterType.NoHealer, PvE = JobFilterType.NoHealer)]
    [UI("Only Heal self when not a healer.", 
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    public ConditionBoolean OnlyHealSelfWhenNoHealer { get; private set; } = new(false, nameof(OnlyHealSelfWhenNoHealer));

    [UI("Display toggle action feedback on chat.",
        (int)UiString.ConfigWindow_UI_Information)]
    public ConditionBoolean ShowToggledActionInChat { get; private set; } = new(true, nameof(ShowToggledActionInChat));

    [UI("Show the timeline assigned drawings in game.",
    (int)UiString.TimelineRaidTime)]
    public ConditionBoolean ShowTimelineDrawing { get; private set; } = new(true, nameof(ShowTimelineDrawing));

    [UI("Enable auto movement using timeline assignments.",
        (int)UiString.TimelineRaidTime)]
    public ConditionBoolean EnableTimelineMovement { get; private set; } = new(true, nameof(EnableTimelineMovement));

    [UI("Skips ping checking. Please use it along with NoClippy",
        (int)UiString.ConfigWindow_Basic_Timer, Section = 2)]
    public ConditionBoolean NoPingCheck { get; private set; } = new(false, nameof(NoPingCheck));

    [UI("Use additional conditions", (int)UiString.ConfigWindow_Basic_Others)]
    public bool UseAdditionalConditions { get; set; } = false;

    #region Float
    [UI("Auto turn off RS when combat is over more for more then...",
        Parent =nameof(AutoOffAfterCombat))]
    [Range(0, 600, ConfigUnitType.Seconds)]
    public float AutoOffAfterCombatTime { get; set; } = 30;

    [UI("Drawing smoothness.", Parent = nameof(UseOverlayWindow))]
    [Range(0.005f, 0.05f, ConfigUnitType.Yalms, 0.001f)]
    public float SampleLength { get; set; } = 1;

    [UI("Use tasks for making the overlay window faster.", Parent = nameof(UseOverlayWindow))]
    public ConditionBoolean UseTasksForOverlay { get; private set; } = new(false, nameof(UseTasksForOverlay));

    [UI("The angle of your vision cone", Parent = nameof(OnlyAttackInVisionCone))]
    [Range(0, 90, ConfigUnitType.Degree, 0.02f)]
    public float AngleOfVisionCone { get; set; } = 45;

    [UI("HP for standard deviation for using AoE heal.", Parent = nameof(UseHealWhenNotAHealer))]
    [Range(0, 0.5f, ConfigUnitType.Percent, 0.02f)]
    public float HealthDifference { get; set; } = 0.25f;

    [JobFilter(PvE = JobFilterType.Melee, PvP = JobFilterType.NoJob)]
    [UI("Maximum melee range action usage before using offset setting", 
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    [Range(0, 5, ConfigUnitType.Yalms, 0.02f)]
    public float MeleeRangeOffset { get; set; } = 1;

    [UI("The time ahead of the last oGCD before the next GCD being available to start trying using it (may affect skill weaving)",
        (int)UiString.ConfigWindow_Basic_Timer)]
    [Range(0, 0.4f, ConfigUnitType.Seconds, 0.002f)]
    public float MinLastAbilityAdvanced { get; set; } = 0.1f;

    [UI("Heal party members that are lower then the selected percentage", Parent = nameof(HealWhenNothingTodo))]
    [Range(0, 1, ConfigUnitType.Percent, 0.002f)]
    public float HealWhenNothingTodoBelow { get; set; } = 0.8f;

    [UI("The size of the next ability that will be used icon.",
        Parent =nameof(ShowTarget))]
    [Range(0, 1, ConfigUnitType.Percent, 0.002f)]
    public float TargetIconSize { get; set; } = 0.3f;

    [UI("How likely is it that RS will click the wrong action.",
        (int)UiString.ConfigWindow_Basic_Others)]
    [Range(0, 1, ConfigUnitType.Percent, 0.002f)]
    public float MistakeRatio { get; set; } = 0;

    [JobFilter(PvE = JobFilterType.Healer, PvP = JobFilterType.Healer)]
    [UI("Prioritize tank healing if its HP is lower than the selected percentage.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    [Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    public float HealthTankRatio { get; set; } = 0.4f;

    [JobFilter(PvE = JobFilterType.Healer, PvP = JobFilterType.Healer)]
    [UI("Prioritize healer healing if its HP is lower than the selected percentage", 
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 1)]
    [Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    public float HealthHealerRatio { get; set; } = 0.4f;

    [UI("The duration of special windows set by commands.",
        (int)UiString.ConfigWindow_Basic_Timer, Section = 1)]
    [Range(1, 20, ConfigUnitType.Seconds, 1f)]
    public float SpecialDuration { get; set; } = 3;

    [UI("Random delay for finding a target.", (int)UiString.ConfigWindow_Target_Config)]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 TargetDelay { get; set; } = new(1, 2);

    [UI("This is the clipping time.\nGCD is over. However, RS forgets to click the next action.",
        (int)UiString.ConfigWindow_Basic_Timer)]
    [Range(0, 1, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 WeaponDelay { get; set; } = new(0, 0);

    [UI("Random delay for stopping casting when the target is dead or immune to damage.",
        Parent = nameof(UseStopCasting))]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 StopCastingDelay { get; set; } = new(0.5f, 1);

    [JobFilter(PvE = JobFilterType.Interrupt, PvP = JobFilterType.NoJob)]
    [UI("Random delay for interrupting hostile targets.",
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 InterruptDelay { get; set; } = new(0.5f, 1);

    [UI("Provoke delay.", Parent = nameof(AutoProvokeForTank))]
    [Range(0, 10, ConfigUnitType.Seconds, 0.05f)]
    public Vector2 ProvokeDelay { get; set; } = new(0.5f, 1);

    [UI("Random delay for Not In Combat.",
        (int)UiString.ConfigWindow_Basic_Others)]
    [Range(0, 10, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 NotInCombatDelay { get; set; } = new(3, 4);

    [UI("Random delay for clicking actions.",
        (int)UiString.ConfigWindow_Basic_Timer)]
    [Range(0.05f, 0.25f, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 ClickingDelay { get; set; } = new(0.1f, 0.15f);

    [UI("Delay of this type of healing.", Parent = nameof(HealWhenNothingTodo))]
    [Range(0, 5,  ConfigUnitType.Seconds, 0.05f)]
    public Vector2 HealWhenNothingTodoDelay { get; set; } = new(0.5f, 1);

    [UI("Random delay between for auto mode activation on countdown.",
        Parent =nameof(StartOnCountdown))]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 CountdownDelay { get; set; } = new(0.5f, 1);

    [UI("The random delay between for the heal delay.",
    Parent = nameof(AutoHeal))]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 HealDelay { get; set; } = new(0.5f, 1);

    [UI("The random delay between for the auto anti-knockback.", Parent = nameof(UseKnockback))]
    [Range(0, 5, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 AntiKnockbackDelay { get; set; } = new(2, 3);

    [UI("The random delay between for the auto defense single.", Parent = nameof(UseDefenseAbility))]
    [Range(0, 5, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 DefenseSingleDelay { get; set; } = new(2, 3);

    [UI("The random delay between for the auto defense area.", Parent = nameof(UseDefenseAbility))]
    [Range(0, 5, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 DefenseAreaDelay { get; set; } = new(2, 3);

    [JobFilter(PvP = JobFilterType.NoJob)]
    [UI("Remaining countdown duration when abilities will start being used before finishing the countdown.", (int)UiString.ConfigWindow_Basic_Timer, Section = 1)]
    [Range(0, 0.7f, ConfigUnitType.Seconds, 0.002f)]
    public float CountDownAhead { get; set; } = 0.4f;

    [UI("The angle that targets can be selected as the move ability targets.", (int)UiString.ConfigWindow_Target_Config,
        Description = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are moveable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.",
         Section = 2)]
    [Range(0, 90, ConfigUnitType.Degree, 0.02f)]
    public float MoveTargetAngle { get; set; } = 24;

    [UI("If target's time until death is higher than this, regard it as boss.",
        (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    [Range(10, 1800, ConfigUnitType.Seconds, 0.02f)]
    public float BossTimeToKill { get; set; } = 90;


    [UI("If target's time until death is lower than this, regard it is dying.",
                (int)UiString.ConfigWindow_Target_Config, Section = 1)]
    [Range(0, 60, ConfigUnitType.Seconds, 0.02f)]
    public float DyingTimeToKill { get; set; } = 10;

    [UI("Cooldown font size.", Parent = nameof(ShowCooldownWindow))]
    [Range(9.6f, 96, ConfigUnitType.Pixels, 0.1f)]
    public float CooldownFontSize { get; set; } = 16;

    [UI("Cooldown window icon size", Parent = nameof(ShowCooldownWindow))]
    [Range(0, 80, ConfigUnitType.Pixels, 0.2f)]
    public float CooldownWindowIconSize { get; set; } = 30;

    [UI("Next Action Size Ratio", Parent = nameof(ShowControlWindow))]
    [Range(0, 10, ConfigUnitType.Percent, 0.02f)]
    public float ControlWindowNextSizeRatio { get; set; } = 1.5f;

    [UI("GCD icon size", Parent = nameof(ShowControlWindow))]
    [Range(0, 80, ConfigUnitType.Pixels, 0.2f)]
    public float ControlWindowGCDSize { get; set; } = 40;

    [UI("oGCD icon size", Parent = nameof(ShowControlWindow))]
    [Range(0, 80, ConfigUnitType.Pixels, 0.2f)]
    public float ControlWindow0GCDSize { get; set; } = 30;

    [UI("Control Progress Height")]
    [Range( 2, 30, ConfigUnitType.Yalms)]
    public float ControlProgressHeight { get; set; } = 8;

    [UI("Use gapcloser as a damage ability if the distance to your target is less then this.",
        (int)UiString.ConfigWindow_Target_Config, Section = 2)]
    [Range(0, 30, ConfigUnitType.Yalms, 1f)]
    public float DistanceForMoving { get; set; } = 1.2f;

    [UI("Stop healing when time to kill is lower then...", Parent = nameof(UseHealWhenNotAHealer))]
    [Range(0, 30, ConfigUnitType.Seconds, 0.02f)]
    public float AutoHealTimeToKill { get; set; } = 8f;

    [UI("Hostile Icon height from position", Parent =nameof(ShowHostilesIcons))]
    [Range(0, 10, ConfigUnitType.Pixels, 0.002f)]
    public float HostileIconHeight { get; set; } = 0.5f;

    [UI("Hostile Icon size", Parent = nameof(ShowHostilesIcons))]
    [Range(0.1f, 5, ConfigUnitType.Percent, 0.002f)]
    public float HostileIconSize { get; set; } = 0.5f;

    [UI("State icon height", Parent =nameof(ShowStateIcon))]
    [Range(0, 3, ConfigUnitType.Pixels, 0.002f)]
    public float StateIconHeight { get; set; } = 1;

    [UI("State icon size", Parent = nameof(ShowStateIcon))]
    [Range(0.1f, 5, ConfigUnitType.Percent, 0.002f)]
    public float StateIconSize { get; set; } = 0.5f;

    [UI("The minimum time between updating RS information.",
        (int)UiString.ConfigWindow_Basic_Timer)]
    [Range(0, 1, ConfigUnitType.Seconds, 0.002f)]
    public float MinUpdatingTime { get; set; } = 0.02f;

    [JobFilter(PvE = JobFilterType.NoJob)]
    [UI("The HP for using Guard.", 
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    [Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    public float HealthForGuard { get; set; } = 0.15f;

    [UI("Prompt box color of teaching mode", Parent =nameof(TeachingMode))]
    public Vector4 TeachingModeColor { get; set; } = new(0f, 1f, 0.8f, 1f);

    [UI("Target color", Parent =nameof(TargetColor))]
    public Vector4 TargetColor { get; set; } = new(1f, 0.2f, 0f, 0.8f);

    [UI("The color of beneficial AoE positions", Parent =nameof(ShowBeneficialPositions))]
    public Vector4 BeneficialPositionColor { get; set; } = new(0.5f, 0.9f, 0.1f, 0.7f);

    [UI("The color of the hovered beneficial position", Parent = nameof(ShowBeneficialPositions))]
    public Vector4 HoveredBeneficialPositionColor { get; set; } = new(1f, 0.5f, 0f, 0.8f);

    [UI("Locked Control Window's Background", Parent = nameof(ShowControlWindow))]
    public Vector4 ControlWindowLockBg { get; set; } = new (0, 0, 0, 0.55f);

    [UI("Unlocked Control Window's Background", Parent =nameof(ShowControlWindow))]
    public Vector4 ControlWindowUnlockBg { get; set; } = new(0, 0, 0, 0.75f);

    [UI("Info Window's Background", (int)UiString.ConfigWindow_UI_Windows)]
    public Vector4 InfoWindowBg { get; set; } = new(0, 0, 0, 0.4f);

    [UI("The text color of the time to kill indicator.", Parent =nameof(ShowTargetTimeToKill))]
    public Vector4 TTKTextColor { get; set; } = new(0f, 1f, 0.8f, 1f);
    #endregion

    #region Integer

    public int ActionSequencerIndex { get; set; }

    [UI("The modifier key to unlock movement temporary", Description = "RB is for gamepad player", Parent = nameof(PoslockCasting))]
    public ConsoleModifiers PoslockModifier { get; set; }

    [JobFilter(PvE = JobFilterType.Raise)]
    [Range(0, 10000, ConfigUnitType.None, 200)]
    [UI("Never ressurect players if MP is less than the set value",
        (int)UiString.ConfigWindow_Auto_ActionUsage)]
    public int LessMPNoRaise { get; set; }

    [Range(0, 5, ConfigUnitType.None, 1)]
    [UI("Effect times", Parent =nameof(KeyBoardNoise))]
    public Vector2Int KeyboardNoise { get; set; } = new (2, 3);

    [Range(0, 10, ConfigUnitType.None)]
    public int TargetingIndex { get; set; }

    [UI("Beneficial AoE strategy", Parent = nameof(UseGroundBeneficialAbility))]
    public BeneficialAreaStrategy BeneficialAreaStrategy { get; set; } = BeneficialAreaStrategy.OnCalculated;

    [JobFilter(PvE = JobFilterType.Tank)]
    [UI("Number of hostiles", Parent = nameof(UseDefenseAbility))]
    [Range(1, 8, ConfigUnitType.None, 0.05f)]
    public int AutoDefenseNumber { get; set; } = 2;
    #endregion

    #region Jobs
    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthAreaAbilityHot = 0.55f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthAreaSpellHot = 0.55f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthAreaAbility = 0.75f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthAreaSpell = 0.65f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthSingleAbilityHot = 0.65f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthSingleSpellHot = 0.45f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthSingleAbility = 0.7f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthSingleSpell = 0.55f;

    [JobFilter(PvE = JobFilterType.Tank, PvP = JobFilterType.NoJob)]
    [JobConfig, Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    [UI("The HP%% for tanks to use invulnerability", 
        (int)UiString.ConfigWindow_Auto_ActionCondition, Section = 3)]
    private readonly float _healthForDyingTanks = 0.15f;

    [JobFilter(PvE = JobFilterType.Tank)]
    [JobConfig, Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    [UI("HP%% for personal defense abilities usage for tanks", Parent = nameof(UseDefenseAbility))]
    private readonly float _healthForAutoDefense = 1;

    [JobConfig, Range(0, 0.5f, ConfigUnitType.Seconds)]
    [UI("Action Ahead", (int)UiString.ConfigWindow_Basic_Timer, Description = "This plugin helps you to use the right action during the combat. Here is a guide about the different options.")]
    private readonly float _actionAhead = 0.08f;

    [JobFilter(PvP = JobFilterType.NoJob)]
    [JobConfig, UI("Engage settings", (int)UiString.ConfigWindow_Target_Config)]
    private readonly TargetHostileType _hostileType = TargetHostileType.AllTargetsWhenSolo;

    [JobConfig]
    private readonly string _PvPRotationChoice = string.Empty;

    [JobConfig]
    private readonly string _rotationChoice = string.Empty;
    #endregion

    [JobConfig]
    private readonly Dictionary<uint, ActionConfig> _rotationActionConfig = [];

    [JobConfig]
    private readonly Dictionary<uint, ItemConfig> _rotationItemConfig = [];

    [JobChoiceConfig]
    private readonly Dictionary<string, string> _rotationConfigurations = [];

    public Dictionary<uint, string> DutyRotationChoice { get; set; } = [];

    [JobConfig]
    private readonly Dictionary<uint, Dictionary<float, List<BaseTimelineItem>>> _timeline = [];

    public void Save()
    {
#if DEBUG
        Svc.Log.Information("Saved configurations.");

        Dictionary<uint, Dictionary<float, List<BaseTimelineItem>>> dict = [];
        foreach((var job, var timelineSet) in this._timelineDict)
        {
            foreach ((var id, var timeline) in timelineSet)
            {
                var refineTimeline = timeline.Select(i => (i.Key, i.Value.Where(j => j is DrawingTimeline).ToList())).ToDictionary();

                var count = refineTimeline.Sum(i => i.Value.Count);

                if (count == 0) continue;

                if (dict.TryGetValue(id, out var lastTimeline))
                {
                    if (lastTimeline.Sum(i => i.Value.Count) >= count)
                    {
                        continue;
                    }
                }
                dict[id] = refineTimeline;
            }
        }
        
        foreach ((var id, var timeline) in dict)
        {
            var dirInfo = Svc.PluginInterface.AssemblyLocation.Directory;
            dirInfo = dirInfo?.Parent!.Parent!.Parent!.Parent!;
            var dir = dirInfo.FullName + @"\Resources\Timelines";
            if (!Directory.Exists(dir))
            {
                Svc.Log.Error("Failed to save the resources: " + dir);
                continue;
            }
            File.WriteAllText(dir + @$"\{id}.json", JsonConvert.SerializeObject(timeline, Formatting.Indented));
        }
#endif
        File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName,
            JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}