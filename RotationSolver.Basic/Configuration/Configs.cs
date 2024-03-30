using Dalamud.Configuration;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration.Timeline;

namespace RotationSolver.Basic.Configuration;

internal partial class Configs : IPluginConfiguration
{
    [JsonIgnore]
    public const string 
        BasicTimer = "BasicTimer",
        BasicAutoSwitch = "BasicAutoSwitch",
        BasicParams = "BasicParams",
        UiInformation = "UiInformation",
        UiOverlay = "UiOverlay",
        UiWindows = "UiWindows",
        AutoActionUsage = "AutoActionUsage",
        AutoActionCondition = "AutoActionCondition",
        TargetConfig = "TargetConfig",
        Extra = "Extra",
        Rotations = "Rotations",
        List = "List",
        TimelineFilter = "Timeline",
        Debug = "Debug";

    public int Version { get; set; } = 8;

    public List<ActionEventInfo> Events { get; private set; } = [];
    public SortedSet<Job> DisabledJobs { get; private set; } = [];

    public string[] OtherLibs { get; set; } = [];

    public string[] GitHubLibs { get; set; } = [];
    public List<TargetingType> TargetingTypes { get; set; } = [];

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();

    [ConditionBool, UI("Show RS logo animation.",
        Filter = UiWindows)]
    private static readonly bool _drawIconAnimation = true;

    [ConditionBool, UI("Turns auto mode  off when player is switching between different maps.",
        Filter =BasicAutoSwitch)]
    private static readonly bool _autoOffBetweenArea = true;

    [ConditionBool, UI("Turns auto mode off during cutscenes.",
        Filter =BasicAutoSwitch)]
    private static readonly bool _autoOffCutScene = true;

    [ConditionBool, UI("Turns auto mode off when you die.",
        Filter =BasicAutoSwitch)]
    private static readonly bool _autoOffWhenDead = true;

    [ConditionBool, UI("Turns auto mode off when the duty in progress is completed.",
        Filter =BasicAutoSwitch)]
    private static readonly bool _autoOffWhenDutyCompleted = true;

    [ConditionBool, UI("Consider only fate targets in the current fate area",
        Filter = TargetConfig, Section = 1)]
    private static readonly bool _changeTargetForFate  = true;

    [ConditionBool, UI("Use movement actions towards the closest object from the center of the screen",
        Description = "Use movement actions towards the closest object from the center of the screen, otherwise toward your current facing direction.",
        Filter = TargetConfig, Section = 2)]
    private static readonly bool _moveTowardsScreenCenter = true;

    [ConditionBool, UI("Audio notification for addon mode changes",
        Filter =UiInformation)]
    private static readonly bool _sayOutStateChanged = true;

    [ConditionBool, UI("Display plugin status in server info bar",
        Filter =UiInformation)]
    private static readonly bool _showInfoOnDtr = true;

    [ConditionBool, UI("Heals players when not in combat.",
        Filter = AutoActionCondition, Section =1)]
    private static readonly bool _healOutOfCombat = false;

    [ConditionBool, UI("Displays plugin status with toast notifications",
        Filter =UiInformation)]
    private static readonly bool _showInfoOnToast = true;

    [ConditionBool, UI("Lock the movement when casting or when doing some other actions.", Filter = Extra)]
    private static readonly bool _poslockCasting = true;

    [UI("", Action = ActionID.PassageOfArmsPvE, Parent = nameof(PoslockCasting))]
    public bool  PosPassageOfArms { get; set; } = false;

    [UI("", Action = ActionID.TenChiJinPvE, Parent = nameof(PoslockCasting))]
    public bool PosTenChiJin { get; set; } = true;

    [UI("", Action = ActionID.FlamethrowerPvE, Parent = nameof(PoslockCasting))]
    public bool  PosFlameThrower { get; set; } = false;

    [UI("", Action = ActionID.ImprovisationPvE, Parent = nameof(PoslockCasting))]
    public bool  PosImprovisation { get; set; } = false;

    [ConditionBool, UI("Raise player while swiftcast is on cooldown",
        Filter = AutoActionUsage, Section = 2,
        PvEFilter = JobFilterType.Raise, PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _raisePlayerByCasting = true;

    [ConditionBool, UI("Raise any player in range (even if they are not in your party)",
        Filter = AutoActionUsage, Section = 2,
        PvEFilter = JobFilterType.Raise, PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _raiseAll = false;

    [ConditionBool, UI("Raise players that have the Brink of Death debuff",
        Filter = AutoActionUsage, Section = 2,
        PvEFilter = JobFilterType.Raise, PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _raiseBrinkOfDeath = true;

    [UI("Random delay for considering a ressurection type action.",
        Filter = AutoActionUsage, Section = 2,
        PvEFilter = JobFilterType.Raise, PvPFilter = JobFilterType.NoJob)]
    [Range(0, 10, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 RaiseDelay { get; set; } = new(1, 2);

    [ConditionBool, UI("Add enemy list to the hostile targets.",
        Filter = TargetConfig)]
    private static readonly bool _addEnemyListToHostile = true;

    [ConditionBool, UI("Only attack targets in the enemy list.",
        Parent = nameof(AddEnemyListToHostile))]
    private static readonly bool _onlyAttackInEnemyList = false;

    [ConditionBool, UI("Use Tinctures.", Filter = AutoActionUsage)]
    private static readonly bool _useTinctures = false;

    [ConditionBool, UI("Use HP Potions.", Filter = AutoActionUsage)]
    private static readonly bool _useHpPotions = false;

    [ConditionBool, UI("Use MP Potions.", Filter = AutoActionUsage)]
    private static readonly bool _useMpPotions = false;

    [ConditionBool, UI("Draw the melee buffer area on the screen.",
        Description = "Shows the area where no actions will be used between ranged and melee type actions.",
        Filter = UiOverlay)]
    private static readonly bool _drawMeleeOffset = true;

    [ConditionBool, UI("Shows the target of the movement action.",
        Filter =UiOverlay)]
    private static readonly bool _showMoveTarget = true;

    [ConditionBool, UI("Shows target related drawing.",
        Description = "Shows the next ability under the target and AoE attacks effect area",
        Filter = UiOverlay)]
    private static readonly bool _showTarget = true;

    [ConditionBool, UI("Show the target's estimated time to kill.",
        Parent = nameof(ShowTarget))]
    private static readonly bool _showTargetTimeToKill = false;

    [ConditionBool, UI("Priority attacks targets with attack markers.",
        Filter = TargetConfig)]
    private static readonly bool _chooseAttackMark = true;

    [ConditionBool, UI("Allows use of AoE abilities to attack as many targets as possibly.",
        Parent = nameof(ChooseAttackMark))]
    private static readonly bool _canAttackMarkAOE = true;

    [ConditionBool, UI("Never attack targets with stop markers.",
        Filter = TargetConfig)]
    private static readonly bool _filterStopMark = true;

    [ConditionBool, UI ("Shows the hostile targets icon.",
        Filter = UiOverlay)]
    private static readonly bool _showHostilesIcons = true;

    [ConditionBool, UI ("Teaching mode.",
        Description = "Shows the next suggested ability that should be used",
        Filter =UiOverlay)]
    
    private static readonly bool _teachingMode = true;

    [ConditionBool, UI("Display UI Overlay.", Description = "This overlay is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.",
        Filter = UiOverlay)]
    private static readonly bool _useOverlayWindow = true;

    [ConditionBool, UI("Simulates the effect of pressing abilities.",
        Filter =UiInformation)]
    private static readonly bool _keyBoardNoise = true;

    [ConditionBool, UI("Targets movement area ability to the farthest possible location.", Description = "Moves to the furthest possible position that can be targeted with movement actions.",
        Filter = TargetConfig, Section = 2)]
    private static readonly bool _moveAreaActionFarthest = true;

    [ConditionBool, UI("Auto mode activation delay on countdown start.",
        Filter =BasicAutoSwitch, Section = 1)]
    private static readonly bool _startOnCountdown = true;

    [ConditionBool, UI("Automatically turns on manual mode and targets enemy when being attacked.",
        Filter =BasicAutoSwitch, Section =1)]
    private static readonly bool _startOnAttackedBySomeone = false;

    [ConditionBool, UI("Don't attack new mobs by AoE.", Description = "Avoids usage of AoE abilities when new enemies would be aggroed.",
        Parent =nameof(UseAoeAction))]
    private static readonly bool _noNewHostiles = false;

    [ConditionBool, UI("Use healing abilities when playing a non-healer role.",
        Filter = AutoActionCondition, Section = 1,
        PvEFilter = JobFilterType.NoHealer, PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _useHealWhenNotAHealer = true;

    [ConditionBool, UI("Target allies for friendly actions.",
        Filter = TargetConfig, Section = 3)]
    private static readonly bool _switchTargetFriendly = false;

    [ConditionBool, UI("Use interrupt abilities if possible.",
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.Interrupt,
        PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _interruptibleMoreCheck = true;

    [ConditionBool, UI("Use work task for acceleration.",
        Filter = BasicParams)]
    private static readonly bool _useWorkTask = false;

    [ConditionBool, UI("Stops casting when the target is dead.", Filter = Extra)]
    private static readonly bool _useStopCasting = false;

    [ConditionBool, UI("Cleanse all dispellable debuffs.",
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.Dispel, PvPFilter = JobFilterType.NoJob)]
    private static readonly bool _dispelAll = false;

    [ConditionBool, UI("Only attacks targets within camera view.",
        Filter = TargetConfig, Section = 1)]
    private static readonly bool _onlyAttackInView = false;

    [ConditionBool, UI("Only attack the targets within the character's vision cone.",
        Filter = TargetConfig, Section = 1)]
    private static readonly bool _onlyAttackInVisionCone = false;

    [ConditionBool, UI("Use single target healing over time actions only on tanks.",
        Filter = AutoActionCondition, Section = 1,
        PvEFilter = JobFilterType.Healer, PvPFilter = JobFilterType.Healer)]
    private static readonly bool _onlyHotOnTanks = false;

    [ConditionBool, UI("Debug Mode.", Filter = Debug)]
    private static readonly bool _inDebug = false;
    public bool AutoUpdateLibs { get; set; } = true;

    [ConditionBool, UI("Auto Download Rotations.", Filter = Rotations)]
    private static readonly bool _downloadRotations = true;

    [ConditionBool, UI("Auto Update Rotations.", Parent = nameof(DownloadRotations))]
    private static readonly bool _autoUpdateRotations = true;

    [ConditionBool, UI("Make /rotation Manual as a toggle command.",
        Filter = BasicParams)]
    private static readonly bool _toggleManual = false;

    [ConditionBool, UI("Make /rotation Auto as a toggle command.",
        Filter =BasicParams)]
    private static readonly bool _toggleAuto = false;

    [ConditionBool, UI("Only show these windows if there are enemies in or in duty.",
        Filter =UiWindows)]
    private static readonly bool _onlyShowWithHostileOrInDuty = true;

    [ConditionBool, UI("Show Control Window.",
        Filter =UiWindows)]
    private static readonly bool _showControlWindow = false;

    [ConditionBool, UI("Lock Control Window.",
        Filter = UiWindows)]
    private static readonly bool _isControlWindowLock = false;

    [ConditionBool, UI("Show Next Action Window.", Filter  = UiWindows)]
    private static readonly bool _showNextActionWindow = true;

    [ConditionBool, UI("No Inputs.", Parent = nameof(ShowNextActionWindow))]
    private static readonly bool _isInfoWindowNoInputs = false;

    [ConditionBool, UI("No Move.", Parent = nameof(ShowNextActionWindow))]
    private static readonly bool _isInfoWindowNoMove = false;

    [ConditionBool, UI("Show Items Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    private static readonly bool _showItemsCooldown = false;

    [ConditionBool, UI("Show GCD Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    private static readonly bool _showGCDCooldown = false;

    [ConditionBool, UI("Show Original Cooldown.",
        Parent = nameof(ShowCooldownWindow))]
    private static readonly bool _useOriginalCooldown = true;

    [ConditionBool, UI("Show tooltips.",
        Filter = UiInformation)]
    private static readonly bool _showTooltips = true;

    [ConditionBool, UI("Auto load custom rotation files.",
        Filter = Rotations)]
    private static readonly bool _autoLoadCustomRotations = false;

    [ConditionBool, UI("Prioritize fate targets.",
        Filter = TargetConfig, Section = 1)]
    private static readonly bool _targetFatePriority = true;

    [ConditionBool, UI("Prioritize Hunt/Relic/Leve targets.",
        Filter = TargetConfig, Section = 1)]
    private static readonly bool _targetHuntingRelicLevePriority = true;

    [ConditionBool, UI("Prioritize Quest targets.",
        Filter = TargetConfig, Section = 1)]

    private static readonly bool _targetQuestPriority = true;

    [ConditionBool, UI("Display manually triggered actions feedback on toast",
        Filter =UiInformation)]
    private static readonly bool _showToastsAboutDoAction = true;

    [ConditionBool, UI("Use AoE actions.", Filter = AutoActionUsage)]
    private static readonly bool _useAOEAction = true;

    [ConditionBool, UI("Use AoE actions in manual mode.", Parent = nameof(UseAoeAction))]
    private static readonly bool _useAOEWhenManual = false;

    [ConditionBool, UI("Automatically trigger dps burst phase.", Filter = AutoActionCondition)]
    private static readonly bool _autoBurst = true;

    [ConditionBool, UI("Automatic Healing.", Filter = AutoActionCondition)]
    private static readonly bool _autoHeal = true;

    [ConditionBool, UI("Automatic offensive ability use.", Filter = AutoActionUsage)]
    private static readonly bool _useAbility = true;

    [ConditionBool, UI("Automatically use defensive abilities.", Description = "It is recommended to uncheck this option if you are playing high end content or if you can plan healing and defensive ability usage by yourself.",
        Parent = nameof(UseAbility))]
    private static readonly bool _useDefenseAbility = true;

    [ConditionBool, UI("Automatically activate tank stance.", Parent =nameof(UseAbility),
        PvEFilter = JobFilterType.Tank)]
    private static readonly bool _autoTankStance = true;

    [ConditionBool, UI("Auto provoke non-tank attacking targets.", Description = "Automatically use provoke when an enemy is attacking a non-tank member of the party.",
        Parent = nameof(UseAbility), PvEFilter = JobFilterType.Tank)]
    private static readonly bool _autoProvokeForTank = true;

    [ConditionBool, UI("Auto TrueNorth (Melee).",
        Parent = nameof(UseAbility),
        PvEFilter = JobFilterType.Melee)]
    private static readonly bool _autoUseTrueNorth = true;

    [ConditionBool, UI("Raise player by using swiftcast if avaliable",
        Parent = nameof(UseAbility),
        PvEFilter = JobFilterType.Healer)]
    private static readonly bool _raisePlayerBySwift = true;

    [ConditionBool, UI("Use movement speed increase abilities when out of combat.", Parent = nameof(UseAbility))]
    private static readonly bool _autoSpeedOutOfCombat = true;

    [ConditionBool, UI("Use beneficial ground-targeted actions.", Parent = nameof(UseAbility),
        PvEFilter = JobFilterType.Healer)]
    private static readonly bool _useGroundBeneficialAbility = true;

    [ConditionBool, UI("Use beneficial AoE actions while moving.", Parent = nameof(UseGroundBeneficialAbility))]
    private static readonly bool _useGroundBeneficialAbilityWhenMoving = false;

    [ConditionBool, UI("Consider all players for friendly actions (include passerby).",
        Filter = TargetConfig, Section = 3)]
    private static readonly bool _targetAllForFriendly = false;

    [ConditionBool, UI("Show Cooldown Window.", Filter = UiWindows)]
    private static readonly bool _showCooldownWindow = false;

    [ConditionBool, UI("Record AoE actions.", Filter = List)]
    private static readonly bool _recordCastingArea = true;

    [ConditionBool, UI("Auto turn off RS when combat is over more for more then...",
        Filter =BasicAutoSwitch)]
    private static readonly bool _autoOffAfterCombat = true;

    [ConditionBool, UI("Auto open treasure chests",
        Filter = Extra)]
    private static readonly bool _autoOpenChest = false;

    [ConditionBool, UI("Auto close the loot window when auto opened the chest.",
        Parent = nameof(AutoOpenChest))]
    private static readonly bool _autoCloseChestWindow = true;

    [ConditionBool, UI("Show RS state icon.", Filter = UiOverlay)]
    private static readonly bool _showStateIcon = true;

    [ConditionBool, UI("Show beneficial AoE locations.", Filter = UiOverlay)]
    private static readonly bool _showBeneficialPositions = true;

    [ConditionBool, UI("Hide all warnings.",
        Filter = UiInformation)]
    private static readonly bool _hideWarning = false;

    [ConditionBool, UI("Heal party members using GCD healing if there is nothing to do while in combat.",
        Filter = AutoActionCondition, Section = 1)]
    private static readonly bool _healWhenNothingTodo = true;

    [ConditionBool, UI("Say hello to all other users of Rotation Solver.",
        Filter = BasicParams)]
    private static readonly bool _sayHelloToAll = true;

    [ConditionBool, UI("Say hello to the users of Rotation Solver.", Description = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed in Discord Server with your Hash!",
        Parent =nameof(SayHelloToAll))]
    private static readonly bool _sayHelloToUsers = true;

    [ConditionBool, UI("Say hello only one time to the same user.",
        Parent = nameof(SayHelloToAll))]
    private static readonly bool _justSayHelloOnce = false;

    [ConditionBool, UI("Only Heal self when not a healer.", 
        Filter = AutoActionCondition, Section = 1,
        PvPFilter = JobFilterType.NoHealer, PvEFilter = JobFilterType.NoHealer)]
    private static readonly bool _onlyHealSelfWhenNoHealer = false;

    [ConditionBool, UI("Display toggle action feedback on chat.",
        Filter =UiInformation)]
    private static readonly bool _showToggledActionInChat = true;

    [ConditionBool, UI("Show the timeline assigned drawings in game.",
    Filter = TimelineFilter)]
    private static readonly bool _showTimelineDrawing = true;

    [ConditionBool, UI("Enable auto movement using timeline assignments.",
        Filter = TimelineFilter)]
    private static readonly bool _enableTimelineMovement = true;

    [ConditionBool, UI("Skips ping checking. Please use it along with NoClippy",
        Filter = BasicTimer, Section = 2)]
    private static readonly bool _noPingCheck = false;

    [UI("Use additional conditions", Filter = BasicParams)]
    public bool UseAdditionalConditions { get; set; } = false;

    #region Float
    [UI("Auto turn off RS when combat is over more for more then...",
        Parent =nameof(AutoOffAfterCombat))]
    [Range(0, 600, ConfigUnitType.Seconds)]
    public float AutoOffAfterCombatTime { get; set; } = 30;

    [UI("Drawing smoothness.", Parent = nameof(UseOverlayWindow))]
    [Range(0.005f, 0.05f, ConfigUnitType.Yalms, 0.001f)]
    public float SampleLength { get; set; } = 1;

    [ConditionBool, UI("Use tasks for making the overlay window faster.", Parent = nameof(UseOverlayWindow))]
    private static readonly bool _useTasksForOverlay = false;

    [UI("The angle of your vision cone", Parent = nameof(OnlyAttackInVisionCone))]
    [Range(0, 90, ConfigUnitType.Degree, 0.02f)]
    public float AngleOfVisionCone { get; set; } = 45;

    [UI("HP for standard deviation for using AoE heal.", Parent = nameof(UseHealWhenNotAHealer))]
    [Range(0, 0.5f, ConfigUnitType.Percent, 0.02f)]
    public float HealthDifference { get; set; } = 0.25f;

    [UI("Maximum melee range action usage before using offset setting", 
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.Melee, PvPFilter = JobFilterType.NoJob)]
    [Range(0, 5, ConfigUnitType.Yalms, 0.02f)]
    public float MeleeRangeOffset { get; set; } = 1;

    [UI("The time ahead of the last oGCD before the next GCD being avaliable to start trying using it (may affect skill weaving)",
        Filter = BasicTimer)]
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
        Filter = BasicParams)]
    [Range(0, 1, ConfigUnitType.Percent, 0.002f)]
    public float MistakeRatio { get; set; } = 0;

    [UI("Prioritize tank healing if its HP is lower than the selected percentage.",
        Filter = AutoActionCondition, Section = 1,
        PvEFilter = JobFilterType.Healer, PvPFilter = JobFilterType.Healer)]
    [Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    public float HealthTankRatio { get; set; } = 0.4f;

    [UI("Prioritize healer healing if its HP is lower than the selected percentage", 
        Filter = AutoActionCondition, Section = 1,
        PvEFilter = JobFilterType.Healer, PvPFilter = JobFilterType.Healer)]
    [Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    public float HealthHealerRatio { get; set; } = 0.4f;

    [UI("The duration of special windows set by commands.",
        Filter =BasicTimer, Section = 1)]
    [Range(1, 20, ConfigUnitType.Seconds, 1f)]
    public float SpecialDuration { get; set; } = 3;

    [UI("The time before an oGCD is avaliable to start trying using it.",
        Filter = BasicTimer)]
    [Range(0, 0.5f, ConfigUnitType.Seconds, 0.002f)]
    public float ActionAheadForLast0GCD { get; set; } = 0.06f;

    [UI("Random delay for finding a target.", Filter =TargetConfig)]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 TargetDelay { get; set; } = new(1, 2);

    [UI("This is the clipping time.\nGCD is over. However, RS forgets to click the next action.",
        Filter = BasicTimer)]
    [Range(0, 1, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 WeaponDelay { get; set; } = new(0, 0);

    [UI("Random delay for stopping casting when the target is dead or immune to damage.",
        Parent = nameof(UseStopCasting))]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 StopCastingDelay { get; set; } = new(0.5f, 1);

    [UI("Random delay for interrupting hostile targets.",
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.Interrupt, PvPFilter = JobFilterType.NoJob)]
    [Range(0, 3, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 InterruptDelay { get; set; } = new(0.5f, 1);

    [UI("Provoke delay.", Parent = nameof(AutoProvokeForTank))]
    [Range(0, 10, ConfigUnitType.Seconds, 0.05f)]
    public Vector2 ProvokeDelay { get; set; } = new(0.5f, 1);

    [UI("Random delay for Not In Combat.",
        Filter =BasicParams)]
    [Range(0, 10, ConfigUnitType.Seconds, 0.002f)]
    public Vector2 NotInCombatDelay { get; set; } = new(3, 4);

    [UI("Random delay for clicking actions.",
        Filter =BasicTimer)]
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

    [UI("Remaining countdown duration when abilities will start being used before finishing the countdown.",
        Filter =BasicTimer, Section = 1, PvPFilter = JobFilterType.NoJob)]
    [Range(0, 0.7f, ConfigUnitType.Seconds, 0.002f)]
    public float CountDownAhead { get; set; } = 0.4f;

    [UI("The angle that targets can be selected as the move ability targets.", 
        Description = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are moveable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.",
        Filter = TargetConfig, Section = 2)]
    [Range(0, 90, ConfigUnitType.Degree, 0.02f)]
    public float MoveTargetAngle { get; set; } = 24;

    [UI("If target's time until death is higher than this, regard it as boss.",
        Filter = TargetConfig, Section = 1)]
    [Range(10, 1800, ConfigUnitType.Seconds, 0.02f)]
    public float BossTimeToKill { get; set; } = 90;


    [UI("If target's time until death is lower than this, regard it is dying.",
                Filter = TargetConfig, Section = 1)]
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
        Filter = TargetConfig, Section = 2)]
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
        Filter = BasicTimer)]
    [Range(0, 1, ConfigUnitType.Seconds, 0.002f)]
    public float MinUpdatingTime { get; set; } = 0.02f;

    [UI("The HP for using Guard.", 
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.NoJob)]
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

    [UI("Info Window's Background", Filter =UiWindows)]
    public Vector4 InfoWindowBg { get; set; } = new(0, 0, 0, 0.4f);

    [UI("The text color of the time to kill indicator.", Parent =nameof(ShowTargetTimeToKill))]
    public Vector4 TTKTextColor { get; set; } = new(0f, 1f, 0.8f, 1f);
    #endregion

    #region Integer

    public int ActionSequencerIndex { get; set; }

    [UI("The modifier key to unlock movement temporary", Description = "RB is for gamepad player", Parent = nameof(PoslockCasting))]
    public ConsoleModifiers PoslockModifier { get; set; }

    [Range(0, 10000, ConfigUnitType.None, 200)]
    [UI("Never ressurect players if MP is less than the set value",
        Filter = AutoActionUsage,
        PvEFilter = JobFilterType.Raise)]
    public int LessMPNoRaise { get; set; }

    [Range(0, 5, ConfigUnitType.None, 1)]
    [UI("Effect times", Parent =nameof(KeyBoardNoise))]
    public Vector2Int KeyboardNoise { get; set; } = new (2, 3);

    [Range(0, 10, ConfigUnitType.None)]
    public int TargetingIndex { get; set; }

    [UI("Beneficial AoE strategy", Parent = nameof(UseGroundBeneficialAbility))]
    public BeneficialAreaStrategy BeneficialAreaStrategy { get; set; } = BeneficialAreaStrategy.OnCalculated;

    [UI("Number of hostiles", Parent = nameof(UseDefenseAbility),
        PvEFilter = JobFilterType.Tank)]
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

    [JobConfig, Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    [UI("The HP%% for tanks to use invulnerability", 
        Filter = AutoActionCondition, Section = 3,
        PvEFilter = JobFilterType.Tank, PvPFilter = JobFilterType.NoJob)]
    private readonly float _healthForDyingTanks = 0.15f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent, 0.02f)]
    [UI("HP%% for personal defense abilities usage for tanks", Parent = nameof(UseDefenseAbility),
        PvEFilter = JobFilterType.Tank)]
    private readonly float _healthForAutoDefense = 1;

    [LinkDescription($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Images/HowAndWhenToClick.svg",
        "This plugin helps you to use the right action during the combat. Here is a guide about the different options.")]
    [JobConfig, Range(0, 0.5f, ConfigUnitType.Seconds)]
    [UI("Action Ahead", Filter = BasicTimer)]
    private readonly float _actionAhead = 0.08f;

    [JobConfig, UI("Engage settings", Filter = TargetConfig, PvPFilter = JobFilterType.NoJob)]
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
            File.WriteAllText(@$"E:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\Resources\Timelines\{id}.json", JsonConvert.SerializeObject(timeline, Formatting.Indented));
        }
#endif
        File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName,
            JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}