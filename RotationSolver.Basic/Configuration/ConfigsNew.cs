using Dalamud.Configuration;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration;
internal partial class ConfigsNew : IPluginConfiguration
{
    public int Version { get; set; } = 8;

    public List<ActionEventInfo> Events { get; private set; } = [];

    public string[] OtherLibs = [];

    public string[] GitHubLibs = [];
    public List<TargetingType> TargetingTypes { get; set; } = [];

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();


    [UI("Show RS logo animation")]
    public bool DrawIconAnimation { get; set; } = true;

    [UI("Auto turn off when player is moving between areas.")]
    public bool AutoOffBetweenArea { get; set; } = true;

    [UI("Auto turn off during cutscenes.")]
    public bool AutoOffCutScene { get; set; } = true;

    [UI("Auto turn off when dead.")]
    public bool AutoOffWhenDead { get; set; } = true;

    [UI("Auto turn off when duty completed.")]
    public bool AutoOffWhenDutyCompleted { get; set; } = true;

    [UI("Select only Fate targets in Fate")]
    public bool ChangeTargetForFate { get; set; } = true;

    [UI("Using movement actions towards the object in the center of the screen",
        Description = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.")]
    public bool MoveTowardsScreenCenter { get; set; } = true;

    [UI("Audio notification for when the status changes")]
    public bool SayOutStateChanged { get; set; } = true;

    [UI("Display plugin status on server info")]
    public bool ShowInfoOnDtr { get; set; } = true;

    [UI("Heal party members outside of combat.")]
    public bool HealOutOfCombat { get; set; } = false;

    [UI("Display plugin status on toast")]
    public bool ShowInfoOnToast { get; set; } = true;

    [UI("Raise any player in range (even if they are not in your party)")]
    public bool RaiseAll { get; set; } = false;

    [UI("Lock the movement when casting or when doing some actions.")]
    public bool  PoslockCasting { get; set; } = false;
    public bool  PosPassageOfArms { get; set; } = false;
    public bool PosTenChiJin { get; set; } = true;
    public bool  PosFlameThrower { get; set; } = false;
    public bool  PosImprovisation { get; set; } = false;

    [UI("Raise player while swiftcast is on cooldown")]
    public bool RaisePlayerByCasting { get; set; } = true;

    [UI("Raise players that even have Brink of Death debuff")]
    public bool RaiseBrinkOfDeath { get; set; } = true;

    [UI("Add enemy list to the hostile targets.")]
    public bool AddEnemyListToHostile { get; set; } = true;

    [UI("Only attack the targets in enemy list.")]
    public bool  OnlyAttackInEnemyList { get; set; } = false;

    [UI("Use Tinctures")]
    public bool  UseTinctures { get; set; } = false;

    [UI("Use Heal Potions")]
    public bool  UseHealPotions { get; set; } = false;

    [UI("Draw the offset of melee on the screen")]
    public bool DrawMeleeOffset { get; set; } = true;

    [UI("Show the target of the move action")]
    public bool ShowMoveTarget { get; set; } = true;

    [UI("Show the target's time to kill.")]
    public bool  ShowTargetTimeToKill { get; set; } = false;

    [UI("Show Target")]
    public bool ShowTarget { get; set; } = true;

    [UI("Priority attack targets with attack markers")]
    public bool ChooseAttackMark { get; set; } = true;

    [UI("Allowed use of AoE to attack more mobs.")]
    public bool CanAttackMarkAOE { get; set; } = true;

    [UI("Never attack targets with stop markers")]
    public bool FilterStopMark { get; set; } = true;

    [UI ("Show the hostile target icon")]
    public bool ShowHostilesIcons { get; set; } = true;

    [UI ("Teaching mode")]
    public bool TeachingMode { get; set; } = true;

    [UI("Display UI Overlay", Description = "This top window is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.")]
    public bool UseOverlayWindow { get; set; } = true;

    [UI("Simulate the effect of pressing abilities")]
    public bool KeyBoardNoise { get; set; } = true;

    [UI("Target movement area ability to the farthest possible location", Description = "Move to the furthest position for targeting are movement actions.")]
    public bool MoveAreaActionFarthest { get; set; } = true;

    [UI("Auto mode activation delay on countdown start")]
    public bool StartOnCountdown { get; set; } = true;

    [UI("Automatically turn on manual mode and target enemy when being attacked")]
    public bool  StartOnAttackedBySomeone { get; set; } = false;

    [UI("Don't attack new mobs by AoE", Description = "Never use any AoE action when this may attack the mobs that are not hostile targets.")]
    public bool  NoNewHostiles { get; set; } = false;

    [UI("Use healing abilities when playing a non-healer role")]
    public bool UseHealWhenNotAHealer { get; set; } = true;

    [UI("Target allies for friendly actions.")]
    public bool  SwitchTargetFriendly { get; set; } = false;

    [UI("Use interrupt abilities if possible.")]
    public bool InterruptibleMoreCheck { get; set; } = true;

    [UI("Use work task for acceleration.")]
    public bool UseWorkTask { get; set; } = false;

    [UI("Stops casting when the target is dead.")]
    public bool UseStopCasting { get; set; } = false;

    [UI("Cleanse all dispellable debuffs.")]
    public bool EsunaAll { get; set; } = false;

    [UI("Only attack the target in view.")]
    public bool OnlyAttackInView { get; set; } = false;

    [UI("Only attack the targets in vision cone")]
    public bool OnlyAttackInVisionCone { get; set; } = false;

    [UI("Use single target healing over time actions only on tanks")]
    public bool OnlyHotOnTanks { get; set; } = false;

    [UI("Debug Mode")]
    public bool  InDebug { get; set; } = false;
    public bool AutoUpdateLibs { get; set; } = true;

    [UI("Auto Download Rotations")]
    public bool DownloadRotations { get; set; } = true;

    [UI("Auto Update Rotations")]
    public bool AutoUpdateRotations { get; set; } = true;

    [UI("Make /rotation Manual as a toggle command.")]
    public bool  ToggleManual { get; set; } = false;

    [UI("Make /rotation Auto as a toggle command.")]
    public bool  ToggleAuto { get; set; } = false;

    [UI("Only show these windows if there are enemies in or in duty")]
    public bool OnlyShowWithHostileOrInDuty { get; set; } = true;

    [UI("Show Control Window")]
    public bool  ShowControlWindow { get; set; } = false;
    public bool  IsControlWindowLock { get; set; } = false;

    [UI("Show Next Action Window")]
    public bool ShowNextActionWindow { get; set; } = true;

    [UI("No Inputs")]
    public bool  IsInfoWindowNoInputs { get; set; } = false;

    [UI("No Move")]
    public bool  IsInfoWindowNoMove { get; set; } = false;

    [UI("Show Items' Cooldown")]
    public bool  ShowItemsCooldown { get; set; } = false;

    [UI("Show GCD' Cooldown")]
    public bool  ShowGCDCooldown { get; set; } = false;

    [UI("Show Original Cooldown")]
    public bool UseOriginalCooldown { get; set; } = true;

    [UI("Show tooltips")]
    public bool ShowTooltips { get; set; } = true;

    [UI("Auto load rotations")]
    public bool  AutoLoadCustomRotations { get; set; } = false;

    [UI("Target Fate priority")]
    public bool TargetFatePriority { get; set; } = true;

    [UI("Target Hunt/Relic/Leve priority.")]
    public bool TargetHuntingRelicLevePriority { get; set; } = true;

    [UI("Target quest priority.")]
    public bool TargetQuestPriority { get; set; } = true;

    [UI("Display do action feedback on toast")]
    public bool ShowToastsAboutDoAction { get; set; } = true;

    [UI("Use AoE actions")]
    public bool UseAOEAction { get; set; } = true;

    [UI("Use AoE actions in manual mode")]
    public bool  UseAOEWhenManual { get; set; } = false;

    [UI("Automatically trigger dps burst phase")]
    public bool AutoBurst { get; set; } = true;

    [UI("Automatic Heal")]
    public bool AutoHeal { get; set; } = true;

    [UI("Auto-use abilities")]
    public bool UseAbility { get; set; } = true;

    [UI("Use defensive abilities", Description = "It is recommended to check this option if you are playing Raids or you can plan the heal and defense ability usage by yourself.")]
    public bool UseDefenseAbility { get; set; } = true;

    [UI("Automatically activate tank stance")]
    public bool AutoTankStance { get; set; } = true;

    [UI("Auto provoke non-tank attacking targets", Description = "Automatically use provoke when an enemy is attacking a non-tank member of the party.")]
    public bool AutoProvokeForTank { get; set; } = true;

    [UI("Auto TrueNorth (Melee)")]
    public bool AutoUseTrueNorth { get; set; } = true;

    [UI("Raise player by using swiftcast if avaliable")]
    public bool RaisePlayerBySwift { get; set; } = true;

    [UI("Use movement speed increase abilities when out of combat.")]
    public bool AutoSpeedOutOfCombat { get; set; } = true;

    [UI("Use beneficial ground-targeted actions")]
    public bool UseGroundBeneficialAbility { get; set; } = true;

    [UI("Use beneficial AoE actions when moving.")]
    public bool  UseGroundBeneficialAbilityWhenMoving { get; set; } = false;

    [UI("Target all for friendly actions (include passerby)")]
    public bool  TargetAllForFriendly { get; set; } = false;

    [UI("Show Cooldown Window")]
    public bool  ShowCooldownWindow { get; set; } = false;

    [UI("Record AOE actions")]
    public bool RecordCastingArea { get; set; } = true;

    [UI("Auto turn off RS when combat is over more for more then...")]
    public bool AutoOffAfterCombat { get; set; } = true;

    [UI("Auto Open the treasure chest")]
    public bool  AutoOpenChest { get; set; } = false;

    [UI("Auto close the loot window when auto opened the chest.")]
    public bool AutoCloseChestWindow { get; set; } = true;

    [UI("Show RS state icon")]
    public bool ShowStateIcon { get; set; } = true;

    [UI("Show beneficial AoE locations.")]
    public bool ShowBeneficialPositions { get; set; } = true;

    [UI("Hide all warnings")]
    public bool  HideWarning { get; set; } = false;

    [UI("Healing the members with GCD if there is nothing to do in combat.")]
    public bool HealWhenNothingTodo { get; set; } = true;

    [UI("Use actions that use resources")]
    public bool UseResourcesAction { get; set; } = true;

    [UI("Say hello to all users of Rotation Solver.")]
    public bool SayHelloToAll { get; set; } = true;

    [UI("Say hello to the users of Rotation Solver.", Description = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed in Discord Server with your Hash!")]
    public bool SayHelloToUsers { get; set; } = true;

    [UI("Just say hello once to the same user.")]
    public bool  JustSayHelloOnce { get; set; } = false;

    [UI("Use additional conditions")]
    public bool UseAdditionalConditions { get; set; } = false;

    [UI("Only Heal self When Not a healer")]
    public bool OnlyHealSelfWhenNoHealer { get; set; } = false;

    [UI("Display toggle action feedback on chat")]
    public bool ShowToggledActionInChat { get; set; } = true;

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
    public Vector2 KeyboardNoise { get; set; } = new Vector2(2, 3);

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
    private readonly string _RotationChoice = string.Empty;
    #endregion
}
