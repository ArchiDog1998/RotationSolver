using Dalamud.Configuration;
using ECommons.DalamudServices;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration;
internal partial class ConfigsNew : IPluginConfiguration
{
    public int Version { get; set; } = 8;

    public List<ActionEventInfo> Events { get; private set; } = [];
    public SortedSet<Job> DisabledJobs { get; private set; } = [];

    public string[] OtherLibs { get; private set; } = [];

    public string[] GitHubLibs { get; private set; } = [];
    public List<TargetingType> TargetingTypes { get; set; } = [];

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();

    [ConditionBool, UI("Show RS logo animation")]
    private static readonly bool _drawIconAnimation = true;

    [ConditionBool, UI("Auto turn off when player is moving between areas.")]
    private static readonly bool _autoOffBetweenArea = true;

    [ConditionBool, UI("Auto turn off during cutscenes.")]
    private static readonly bool _autoOffCutScene = true;

    [ConditionBool, UI("Auto turn off when dead.")]
    private static readonly bool _autoOffWhenDead = true;

    [ConditionBool, UI("Auto turn off when duty completed.")]
    private static readonly bool _autoOffWhenDutyCompleted = true;

    [ConditionBool, UI("Select only Fate targets in Fate")]
    private static readonly bool _changeTargetForFate  = true;

    [ConditionBool, UI("Using movement actions towards the object in the center of the screen",
        Description = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.")]
    private static readonly bool _moveTowardsScreenCenter = true;

    [ConditionBool, UI("Audio notification for when the status changes")]
    private static readonly bool _sayOutStateChanged = true;

    [ConditionBool, UI("Display plugin status on server info")]
    private static readonly bool _showInfoOnDtr = true;

    [ConditionBool, UI("Heal party members outside of combat.")]
    private static readonly bool _healOutOfCombat = false;

    [ConditionBool, UI("Display plugin status on toast")]
    private static readonly bool _showInfoOnToast = true;

    [ConditionBool, UI("Raise any player in range (even if they are not in your party)")]
    private static readonly bool _raiseAll = false;

    [ConditionBool, UI("Lock the movement when casting or when doing some actions.")]
    private static readonly bool _poslockCasting = false;
    public bool  PosPassageOfArms { get; set; } = false;
    public bool PosTenChiJin { get; set; } = true;
    public bool  PosFlameThrower { get; set; } = false;
    public bool  PosImprovisation { get; set; } = false;

    [ConditionBool, UI("Raise player while swiftcast is on cooldown")]
    private static readonly bool _raisePlayerByCasting = true;

    [ConditionBool, UI("Raise players that even have Brink of Death debuff")]
    private static readonly bool _raiseBrinkOfDeath = true;

    [ConditionBool, UI("Add enemy list to the hostile targets.")]
    private static readonly bool _addEnemyListToHostile = true;

    [ConditionBool, UI("Only attack the targets in enemy list.")]
    private static readonly bool _onlyAttackInEnemyList = false;

    [ConditionBool, UI("Use Tinctures")]
    private static readonly bool _useTinctures = false;

    [ConditionBool, UI("Use HP Potions")]
    private static readonly bool _useHpPotions = false;

    [ConditionBool, UI("Use MP Potions")]
    private static readonly bool _useMpPotions = false;

    [ConditionBool, UI("Draw the offset of melee on the screen")]
    private static readonly bool _drawMeleeOffset = true;

    [ConditionBool, UI("Show the target of the move action")]
    private static readonly bool _showMoveTarget = true;

    [ConditionBool, UI("Show the target's time to kill.")]
    private static readonly bool _showTargetTimeToKill = false;

    [ConditionBool, UI("Show Target")]
    private static readonly bool _showTarget = true;

    [ConditionBool, UI("Priority attack targets with attack markers")]
    private static readonly bool _chooseAttackMark = true;

    [ConditionBool, UI("Allowed use of AoE to attack more mobs.")]
    private static readonly bool _canAttackMarkAOE = true;

    [ConditionBool, UI("Never attack targets with stop markers")]
    private static readonly bool _filterStopMark = true;

    [ConditionBool, UI ("Show the hostile target icon")]
    private static readonly bool _showHostilesIcons = true;

    [ConditionBool, UI ("Teaching mode")]
    private static readonly bool _teachingMode = true;

    [ConditionBool, UI("Display UI Overlay", Description = "This top window is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.")]
    private static readonly bool _useOverlayWindow = true;

    [ConditionBool, UI("Simulate the effect of pressing abilities")]
    private static readonly bool _keyBoardNoise = true;

    [ConditionBool, UI("Target movement area ability to the farthest possible location", Description = "Move to the furthest position for targeting are movement actions.")]
    private static readonly bool _moveAreaActionFarthest = true;

    [ConditionBool, UI("Auto mode activation delay on countdown start")]
    private static readonly bool _startOnCountdown = true;

    [ConditionBool, UI("Automatically turn on manual mode and target enemy when being attacked")]
    private static readonly bool _startOnAttackedBySomeone = false;

    [ConditionBool, UI("Don't attack new mobs by AoE", Description = "Never use any AoE action when this may attack the mobs that are not hostile targets.")]
    private static readonly bool _noNewHostiles = false;

    [ConditionBool, UI("Use healing abilities when playing a non-healer role")]
    private static readonly bool _useHealWhenNotAHealer = true;

    [ConditionBool, UI("Target allies for friendly actions.")]
    private static readonly bool _switchTargetFriendly = false;

    [ConditionBool, UI("Use interrupt abilities if possible.")]
    private static readonly bool _interruptibleMoreCheck = true;

    [ConditionBool, UI("Use work task for acceleration.")]
    private static readonly bool _useWorkTask = false;

    [ConditionBool, UI("Stops casting when the target is dead.")]
    private static readonly bool _useStopCasting = false;

    [ConditionBool, UI("Cleanse all dispellable debuffs.")]
    private static readonly bool _esunaAll = false;

    [ConditionBool, UI("Only attack the target in view.")]
    private static readonly bool _onlyAttackInView = false;

    [ConditionBool, UI("Only attack the targets in vision cone")]
    private static readonly bool _onlyAttackInVisionCone = false;

    [ConditionBool, UI("Use single target healing over time actions only on tanks")]
    private static readonly bool _onlyHotOnTanks = false;

    [ConditionBool, UI("Debug Mode")]
    private static readonly bool _inDebug = false;
    public bool AutoUpdateLibs { get; set; } = true;

    [ConditionBool, UI("Auto Download Rotations")]
    private static readonly bool _downloadRotations = true;

    [ConditionBool, UI("Auto Update Rotations")]
    private static readonly bool _autoUpdateRotations = true;

    [ConditionBool, UI("Make /rotation Manual as a toggle command.")]
    private static readonly bool _toggleManual = false;

    [ConditionBool, UI("Make /rotation Auto as a toggle command.")]
    private static readonly bool _toggleAuto = false;

    [ConditionBool, UI("Only show these windows if there are enemies in or in duty")]
    private static readonly bool _onlyShowWithHostileOrInDuty = true;

    [ConditionBool, UI("Show Control Window")]
    private static readonly bool _showControlWindow = false;
    public bool  IsControlWindowLock { get; set; } = false;

    [ConditionBool, UI("Show Next Action Window")]
    private static readonly bool _showNextActionWindow = true;

    [ConditionBool, UI("No Inputs")]
    private static readonly bool _isInfoWindowNoInputs = false;

    [ConditionBool, UI("No Move")]
    private static readonly bool _isInfoWindowNoMove = false;

    [ConditionBool, UI("Show Items' Cooldown")]
    private static readonly bool _showItemsCooldown = false;

    [ConditionBool, UI("Show GCD' Cooldown")]
    private static readonly bool _showGCDCooldown = false;

    [ConditionBool, UI("Show Original Cooldown")]
    private static readonly bool _useOriginalCooldown = true;

    [ConditionBool, UI("Show tooltips")]
    private static readonly bool _showTooltips = true;

    [ConditionBool, UI("Auto load rotations")]
    private static readonly bool _autoLoadCustomRotations = false;

    [ConditionBool, UI("Target Fate priority")]
    private static readonly bool _targetFatePriority = true;

    [ConditionBool, UI("Target Hunt/Relic/Leve priority.")]
    private static readonly bool _targetHuntingRelicLevePriority = true;

    [ConditionBool, UI("Target quest priority.")]
    private static readonly bool _targetQuestPriority = true;

    [ConditionBool, UI("Display do action feedback on toast")]
    private static readonly bool _showToastsAboutDoAction = true;

    [ConditionBool, UI("Use AoE actions")]
    private static readonly bool _useAOEAction = true;

    [ConditionBool, UI("Use AoE actions in manual mode")]
    private static readonly bool _useAOEWhenManual = false;

    [ConditionBool, UI("Automatically trigger dps burst phase")]
    private static readonly bool _autoBurst = true;

    [ConditionBool, UI("Automatic Heal", Searchable = "AutoHealCheckBox")]
    private static readonly bool _autoHeal = true;

    [ConditionBool, UI("Auto-use abilities")]
    private static readonly bool _useAbility = true;

    [ConditionBool, UI("Use defensive abilities", Description = "It is recommended to check this option if you are playing Raids or you can plan the heal and defense ability usage by yourself.")]
    private static readonly bool _useDefenseAbility = true;

    [ConditionBool, UI("Automatically activate tank stance")]
    private static readonly bool _autoTankStance = true;

    [ConditionBool, UI("Auto provoke non-tank attacking targets", Description = "Automatically use provoke when an enemy is attacking a non-tank member of the party.")]
    private static readonly bool _autoProvokeForTank = true;

    [ConditionBool, UI("Auto TrueNorth (Melee)")]
    private static readonly bool _autoUseTrueNorth = true;

    [ConditionBool, UI("Raise player by using swiftcast if avaliable")]
    private static readonly bool _raisePlayerBySwift = true;

    [ConditionBool, UI("Use movement speed increase abilities when out of combat.")]
    private static readonly bool _autoSpeedOutOfCombat = true;

    [ConditionBool, UI("Use beneficial ground-targeted actions")]
    private static readonly bool _useGroundBeneficialAbility = true;

    [ConditionBool, UI("Use beneficial AoE actions when moving.")]
    private static readonly bool _useGroundBeneficialAbilityWhenMoving = false;

    [ConditionBool, UI("Target all for friendly actions (include passerby)")]
    private static readonly bool _targetAllForFriendly = false;

    [ConditionBool, UI("Show Cooldown Window")]
    private static readonly bool _showCooldownWindow = false;

    [ConditionBool, UI("Record AOE actions")]
    private static readonly bool _recordCastingArea = true;

    [ConditionBool, UI("Auto turn off RS when combat is over more for more then...")]
    private static readonly bool _autoOffAfterCombat = true;

    [ConditionBool, UI("Auto Open the treasure chest")]
    private static readonly bool _autoOpenChest = false;

    [ConditionBool, UI("Auto close the loot window when auto opened the chest.")]
    private static readonly bool _autoCloseChestWindow = true;

    [ConditionBool, UI("Show RS state icon")]
    private static readonly bool _showStateIcon = true;

    [ConditionBool, UI("Show beneficial AoE locations.")]
    private static readonly bool _showBeneficialPositions = true;

    [ConditionBool, UI("Hide all warnings")]
    private static readonly bool _sideWarning = false;

    [ConditionBool, UI("Healing the members with GCD if there is nothing to do in combat.")]
    private static readonly bool _healWhenNothingTodo = true;

    [ConditionBool, UI("Use actions that use resources")]
    private static readonly bool _useResourcesAction = true;

    [ConditionBool, UI("Say hello to all users of Rotation Solver.")]
    private static readonly bool _sayHelloToAll = true;

    [ConditionBool, UI("Say hello to the users of Rotation Solver.", Description = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed in Discord Server with your Hash!")]
    private static readonly bool _sayHelloToUsers = true;

    [ConditionBool, UI("Just say hello once to the same user.")]
    private static readonly bool _justSayHelloOnce = false;

    [ConditionBool, UI("Only Heal self When Not a healer")]
    private static readonly bool _onlyHealSelfWhenNoHealer = false;

    [ConditionBool, UI("Display toggle action feedback on chat")]
    private static readonly bool _showToggledActionInChat = true;

    [UI("Use additional conditions")]
    public bool UseAdditionalConditions { get; set; } = false;

    #region Float
    [UI("Auto turn off RS when combat is over more for more then...")]
    [Range(0, 600, ConfigUnitType.Seconds)]
    public float AutoOffAfterCombatTime { get; set; } = 30;

    [UI("The height of the drawing things.")]
    [Range(0, 8, ConfigUnitType.Yalms)]
    public float DrawingHeight { get; set; } = 3;

    [UI("Drawing smoothness.")]
    [Range(0.005f, 0.05f, ConfigUnitType.Yalms)]
    public float SampleLength { get; set; } = 0.2f;

    [UI("The angle of your vision cone")]
    [Range(0, 90, ConfigUnitType.Degree)]
    public float AngleOfVisionCone { get; set; } = 45;

    [UI("HP for standard deviation for using AoE heal.")]
    [Range(0, 0.5f, ConfigUnitType.Percent)]
    public float HealthDifference { get; set; } = 0.25f;

    [UI("Melee Range action using offset")]
    [Range(0, 5, ConfigUnitType.Yalms)]
    public float MeleeRangeOffset { get; set; } = 1;

    [UI("The time ahead of the last oGCD before the next GCD being avaliable to start trying using it (may affect skill weaving)")]
    [Range(0, 0.4f, ConfigUnitType.Seconds)]
    public float MinLastAbilityAdvanced { get; set; } = 0.1f;

    [UI("When their minimum HP is lower than this.")]
    [Range(0, 1, ConfigUnitType.Percent)]
    public float HealWhenNothingTodoBelow { get; set; } = 0.8f;

    [UI("The size of the next ability that will be used icon.")]
    [Range(0, 1, ConfigUnitType.Pixels)]
    public float TargetIconSize { get; set; } = 0.6f;

    [UI("How likely is it that RS will click the wrong action.")]
    [Range(0, 1, ConfigUnitType.Percent)]
    public float MistakeRatio { get; set; } = 0;

    [UI("Heal tank first if its HP is lower than this.")]
    [Range(0, 1, ConfigUnitType.Percent)]
    public float HealthTankRatio { get; set; } = 0.4f;

    [UI("Heal healer first if its HP is lower than this.")]
    [Range(0, 1, ConfigUnitType.Percent)]
    public float HealthHealerRatio { get; set; } = 0.4f;

    [UI("The duration of special windows set by commands")]
    [Range(1, 20, ConfigUnitType.Seconds)]
    public float SpecialDuration { get; set; } = 3;

    [UI("The time before an oGCD is avaliable to start trying using it")]
    [Range(0, 0.5f, ConfigUnitType.Seconds)]
    public float ActionAheadForLast0GCD { get; set; } = 0.06f;

    [UI("This is the delay time.")]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 TargetDelay { get; set; } = new(0, 0);

    [UI("This is the clipping time.\nGCD is over. However, RS forgets to click the next action.")]
    [Range(0, 1, ConfigUnitType.Seconds)]
    public Vector2 WeaponDelay { get; set; } = new(0, 0);

    [UI("The range of random delay for stopping casting when the target is dead or immune to damage.")]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 StopCastingDelay { get; set; } = new(0.5f, 1);

    [UI("The range of random delay for interrupting hostile targets.")]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 InterruptDelay { get; set; } = new(0.5f, 1);

    [UI("The delay of provoke.")]
    [Range(0, 10, ConfigUnitType.Seconds)]
    public Vector2 ProvokeDelay { get; set; } = new(0.5f, 1);

    [UI("The range of random delay for Not In Combat.")]
    [Range(0, 10, ConfigUnitType.Seconds)]
    public Vector2 NotInCombatDelay { get; set; } = new(3, 4);

    [UI("The range of random delay for clicking actions.")]
    [Range(0.05f, 0.25f, ConfigUnitType.Seconds)]
    public Vector2 ClickingDelay { get; set; } = new(0.1f, 0.15f);

    [UI("The delay of this type of healing.")]
    [Range(0, 5,  ConfigUnitType.Seconds)]
    public Vector2 HealWhenNothingTodoDelay { get; set; } = new(0.5f, 1);

    [UI("The random delay between which auto mode activation on countdown varies.")]
    [Range(0, 3, ConfigUnitType.Seconds)]
    public Vector2 CountdownDelay { get; set; } = new(0.5f, 1);

    [UI("The starting when abilities will be used before finishing the countdown")]
    [Range(0, 0.7f, ConfigUnitType.Seconds)]
    public float CountDownAhead { get; set; } = 0.4f;

    [UI("The size of the sector angle that can be selected as the moveable target", 
        Description = "If the selection mode is based on character facing, i.e., targets within the character's viewpoint are moveable targets. \nIf the selection mode is screen-centered, i.e., targets within a sector drawn upward from the character's point are movable targets.")]
    [Range(0, 90, ConfigUnitType.Degree)]
    public float MoveTargetAngle { get; set; } = 24;

    [UI("If target's time until death is higher than this, regard it as boss.")]
    [Range(10, 1800, ConfigUnitType.Seconds)]
    public float BossTimeToKill { get; set; } = 90;


    [UI("If target's time until death is lower than this, regard it is dying.")]
    [Range(0, 60, ConfigUnitType.Seconds)]
    public float DyingTimeToKill { get; set; } = 10;

    [UI("Change the cooldown font size.")]
    [Range(9.6f, 96, ConfigUnitType.Pixels)]
    public float CooldownFontSize { get; set; } = 16;

    [UI("GCD icon size")]
    [Range(0, 80, ConfigUnitType.Pixels)]
    public float ControlWindowGCDSize { get; set; } = 40;

    [UI("oGCD icon size")]
    [Range(0, 80, ConfigUnitType.Pixels)]
    public float ControlWindow0GCDSize { get; set; } = 30;

    [UI("Cooldown window icon size")]
    [Range(0, 80, ConfigUnitType.Pixels)]
    public float CooldownWindowIconSize { get; set; } = 30;

    [UI("Next Action Size Ratio")]
    [Range(0, 10, ConfigUnitType.Percent)]
    public float ControlWindowNextSizeRatio { get; set; } = 1.5f;

    [UI("Control Progress Height")]
    [Range( 2, 30, ConfigUnitType.Yalms)]
    public float ControlProgressHeight { get; set; } = 8;

    [UI("Use gapcloser as a damage ability if the distance to your target is less then this.")]
    [Range(0, 30, ConfigUnitType.Yalms)]
    public float DistanceForMoving { get; set; } = 1.2f;

    [UI("The max ping that RS can get to before skipping to the next action.")]
    [Range(0.01f, 0.5f, ConfigUnitType.Seconds)]
    public float MaxPing { get; set; } = 0.2f;

    [UI("Stop healing when time to kill is lower then...")]
    [Range(0, 30, ConfigUnitType.Seconds)]
    public float AutoHealTimeToKill { get; set; } = 8f;

    [UI("Hostile Icon height from position")]
    [Range(0, 10, ConfigUnitType.Pixels)]
    public float HostileIconHeight { get; set; } = 0.5f;

    [UI("Hostile Icon size")]
    [Range(0.1f, 10, ConfigUnitType.Percent)]
    public float HostileIconSize { get; set; } = 1;

    [UI("State icon height")]
    [Range(0, 3, ConfigUnitType.Pixels)]
    public float StateIconHeight { get; set; } = 1;

    [UI("State icon size")]
    [Range(0.2f, 10, ConfigUnitType.Percent)]
    public float StateIconSize { get; set; } = 1;

    [UI("The minimum time between updating RS information.")]
    [Range(0, 1, ConfigUnitType.Seconds)]
    public float MinUpdatingTime { get; set; } = 0.02f;

    [UI("The HP for using Guard.")]
    [Range(0, 1, ConfigUnitType.Percent)]
    public float HealthForGuard { get; set; } = 0.15f;

    [UI("Prompt box color of teaching mode")]
    public Vector4 TeachingModeColor { get; set; } = new(0f, 1f, 0.8f, 1f);

    [UI("Prompt box color of moving target")]
    public Vector4 MovingTargetColor { get; set; } = new(0f, 1f, 0.8f, 0.6f);

    [UI("Target color")]
    public Vector4 TargetColor { get; set; } = new(1f, 0.2f, 0f, 0.8f);

    [UI("Sub-target color")]
    public Vector4 SubTargetColor { get; set; } = new(1f, 0.9f, 0f, 0.8f);

    [UI("The color of beneficial AoE positions")]
    public Vector4 BeneficialPositionColor { get; set; } = new(0.5f, 0.9f, 0.1f, 0.7f);

    [UI("The color of the hovered beneficial position")]
    public Vector4 HoveredBeneficialPositionColor { get; set; } = new(1f, 0.5f, 0f, 0.8f);

    [UI("Locked Control Window's Background")]
    public Vector4 ControlWindowLockBg { get; set; } = new (0, 0, 0, 0.55f);

    [UI("Unlocked Control Window's Background")]
    public Vector4 ControlWindowUnlockBg { get; set; } = new(0, 0, 0, 0.75f);

    [UI("Info Window's Background")]
    public Vector4 InfoWindowBg { get; set; } = new(0, 0, 0, 0.4f);

    [UI("The text color of the time to kill indicator.")]
    public Vector4 TTKTextColor { get; set; } = new(0f, 1f, 0.8f, 1f);
    #endregion

    #region Integer

    public int ActionSequencerIndex { get; set; }
    public int PoslockModifier { get; set; }

    [Range(0, 10000, ConfigUnitType.None)]
    public int LessMPNoRaise { get; set; }

    [Range(0, 5, ConfigUnitType.None)]
    public Vector2Int KeyboardNoise { get; set; } = new (2, 3);

    [Range(0, 10, ConfigUnitType.None)]
    public int TargetingIndex { get; set; }

    [Range(0, 10, ConfigUnitType.None)]
    public int BeneficialAreaStrategy { get; set; }

    [Range(1, 8, ConfigUnitType.None)]
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

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthForDyingTanks = 0.15f;

    [JobConfig, Range(0, 1, ConfigUnitType.Percent)]
    private readonly float _healthForAutoDefense = 1;

    [JobConfig, Range(0, 0.5f, ConfigUnitType.Seconds)]
    private readonly float _actionAhead = 0.08f;

    [JobConfig]
    private readonly TargetHostileType _hostileType;

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
    private readonly Dictionary<string, string> _rotationsConfigurations = [];

    public void Save()
    {
#if DEBUG
        Svc.Log.Information("Saved configurations.");
#endif
        File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName,
            JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }));
    }
}
