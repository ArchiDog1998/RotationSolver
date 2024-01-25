using Dalamud.Configuration;

namespace RotationSolver.Basic.Configuration;
internal class ConfigsNew : IPluginConfiguration
{
    public int Version { get; set; } = 8;

    [UI("Show RS logo animation")]
    public bool DrawIconAnimation = true;

    [UI("Auto turn off when player is moving between areas.")]
    public bool AutoOffBetweenArea = true;

    [UI("Auto turn off during cutscenes.")]
    public bool AutoOffCutScene = true;

    [UI("Auto turn off when dead.")]
    public bool AutoOffWhenDead = true;

    [UI("Auto turn off when duty completed.")]
    public bool AutoOffWhenDutyCompleted = true;

    [UI("Select only Fate targets in Fate")]
    public bool ChangeTargetForFate = true;

    [UI("Using movement actions towards the object in the center of the screen",
        Description = "Using movement actions towards the object in the center of the screen, otherwise toward the facing object.")]
    public bool MoveTowardsScreenCenter = true;

    [UI("Audio notification for when the status changes")]
    public bool SayOutStateChanged = true;

    [UI("Display plugin status on server info")]
    public bool ShowInfoOnDtr = true;

    [UI("Heal party members outside of combat.")]
    public bool HealOutOfCombat = false;

    [UI("Display plugin status on toast")]
    public bool ShowInfoOnToast = true;

    [UI("Raise any player in range (even if they are not in your party)")]
    public bool RaiseAll = false;

    [UI("Lock the movement when casting or when doing some actions.")]
    public bool  PoslockCasting = false;
    public bool  PosPassageOfArms = false;
    public bool PosTenChiJin = true;
    public bool  PosFlameThrower = false;
    public bool  PosImprovisation = false;

    [UI("Raise player while swiftcast is on cooldown")]
    public bool RaisePlayerByCasting =true;

    [UI("Raise players that even have Brink of Death debuff")]
    public bool RaiseBrinkOfDeath = true;

    [UI("Add enemy list to the hostile targets.")]
    public bool AddEnemyListToHostile = true;

    [UI("Only attack the targets in enemy list.")]
    public bool  OnlyAttackInEnemyList = false;

    [UI("Use Tinctures")]
    public bool  UseTinctures = false;

    [UI("Use Heal Potions")]
    public bool  UseHealPotions = false;

    [UI("Draw the offset of melee on the screen")]
    public bool DrawMeleeOffset = true;

    [UI("Show the target of the move action")]
    public bool ShowMoveTarget = true;

    [UI("Show the target's time to kill.")]
    public bool  ShowTargetTimeToKill = false;

    [UI("Show Target")]
    public bool ShowTarget = true;

    [UI("Priority attack targets with attack markers")]
    public bool ChooseAttackMark = true;

    [UI("Allowed use of AoE to attack more mobs.")]
    public bool CanAttackMarkAOE = true;

    [UI("Never attack targets with stop markers")]
    public bool FilterStopMark = true;

    [UI ("Show the hostile target icon")]
    public bool ShowHostilesIcons = true;

    [UI ("Teaching mode")]
    public bool TeachingMode = true;

    [UI("Display UI Overlay", Description = "This top window is used to display some extra information on your game window, such as target's positional, target and sub-target, etc.")]
    public bool UseOverlayWindow = true;

    [UI("Simulate the effect of pressing abilities")]
    public bool KeyBoardNoise = true;

    [UI("Target movement area ability to the farthest possible location", Description = "Move to the furthest position for targeting are movement actions.")]
    public bool MoveAreaActionFarthest = true;

    [UI("Auto mode activation delay on countdown start")]
    public bool StartOnCountdown = true;

    [UI("Automatically turn on manual mode and target enemy when being attacked")]
    public bool  StartOnAttackedBySomeone = false;

    [UI("Don't attack new mobs by AoE", Description = "Never use any AoE action when this may attack the mobs that are not hostile targets.")]
    public bool  NoNewHostiles = false;

    [UI("Use healing abilities when playing a non-healer role")]
    public bool UseHealWhenNotAHealer = true;

    [UI("Target allies for friendly actions.")]
    public bool  SwitchTargetFriendly = false;

    [UI("Use interrupt abilities if possible.")]
    public bool InterruptibleMoreCheck = true;

    [UI("Use work task for acceleration.")]
    public bool UseWorkTask = false;

    [UI("Stops casting when the target is dead.")]
    public bool UseStopCasting = false;

    [UI("Cleanse all dispellable debuffs.")]
    public bool EsunaAll = false;

    [UI("Only attack the target in view.")]
    public bool OnlyAttackInView = false;

    [UI("Only attack the targets in vision cone")]
    public bool OnlyAttackInVisionCone = false;

    [UI("Use single target healing over time actions only on tanks")]
    public bool OnlyHotOnTanks = false;

    [UI("Debug Mode")]
    public bool  InDebug = false;
    public bool AutoUpdateLibs = true;

    [UI("Auto Download Rotations")]
    public bool DownloadRotations = true;

    [UI("Auto Update Rotations")]
    public bool AutoUpdateRotations = true;

    [UI("Make /rotation Manual as a toggle command.")]
    public bool  ToggleManual = false;

    [UI("Make /rotation Auto as a toggle command.")]
    public bool  ToggleAuto = false;

    [UI("Only show these windows if there are enemies in or in duty")]
    public bool OnlyShowWithHostileOrInDuty= true;

    [UI("Show Control Window")]
    public bool  ShowControlWindow = false;
    public bool  IsControlWindowLock = false;

    [UI("Show Next Action Window")]
    public bool ShowNextActionWindow = true;

    [UI("No Inputs")]
    public bool  IsInfoWindowNoInputs = false;

    [UI("No Move")]
    public bool  IsInfoWindowNoMove = false;

    [UI("Show Items' Cooldown")]
    public bool  ShowItemsCooldown = false;

    [UI("Show GCD' Cooldown")]
    public bool  ShowGCDCooldown = false;

    [UI("Show Original Cooldown")]
    public bool UseOriginalCooldown = true;

    [UI("Show tooltips")]
    public bool ShowTooltips = true;

    [UI("Auto load rotations")]
    public bool  AutoLoadCustomRotations = false;

    [UI("Target Fate priority")]
    public bool TargetFatePriority = true;

    [UI("Target Hunt/Relic/Leve priority.")]
    public bool TargetHuntingRelicLevePriority = true;

    [UI("Target quest priority.")]
    public bool TargetQuestPriority = true;

    [UI("Display do action feedback on toast")]
    public bool ShowToastsAboutDoAction = true;

    [UI("Use AoE actions")]
    public bool UseAOEAction = true;

    [UI("Use AoE actions in manual mode")]
    public bool  UseAOEWhenManual = false;

    [UI("Automatically trigger dps burst phase")]
    public bool AutoBurst = true;

    [UI("Automatic Heal")]
    public bool AutoHeal = true;

    [UI("Auto-use abilities")]
    public bool UseAbility = true;

    [UI("Use defensive abilities", Description = "It is recommended to check this option if you are playing Raids or you can plan the heal and defense ability usage by yourself.")]
    public bool UseDefenseAbility = true;

    [UI("Automatically activate tank stance")]
    public bool AutoTankStance = true;

    [UI("Auto provoke non-tank attacking targets", Description = "Automatically use provoke when an enemy is attacking a non-tank member of the party.")]
    public bool AutoProvokeForTank = true;

    [UI("Auto TrueNorth (Melee)")]
    public bool AutoUseTrueNorth = true;

    [UI("Raise player by using swiftcast if avaliable")]
    public bool RaisePlayerBySwift = true;

    [UI("Use movement speed increase abilities when out of combat.")]
    public bool AutoSpeedOutOfCombat = true;

    [UI("Use beneficial ground-targeted actions")]
    public bool UseGroundBeneficialAbility = true;

    [UI("Use beneficial AoE actions when moving.")]
    public bool  UseGroundBeneficialAbilityWhenMoving = false;

    [UI("Target all for friendly actions (include passerby)")]
    public bool  TargetAllForFriendly = false;

    [UI("Show Cooldown Window")]
    public bool  ShowCooldownWindow = false;

    [UI("Record AOE actions")]
    public bool RecordCastingArea = true;

    [UI("Auto turn off RS when combat is over more for more then...")]
    public bool AutoOffAfterCombat = true;

    [UI("Auto Open the treasure chest")]
    public bool  AutoOpenChest = false;

    [UI("Auto close the loot window when auto opened the chest.")]
    public bool AutoCloseChestWindow = true;

    [UI("Show RS state icon")]
    public bool ShowStateIcon = true;

    [UI("Show beneficial AoE locations.")]
    public bool ShowBeneficialPositions = true;

    [UI("Hide all warnings")]
    public bool  HideWarning = false;

    [UI("Healing the members with GCD if there is nothing to do in combat.")]
    public bool HealWhenNothingTodo = true;

    [UI("Use actions that use resources")]
    public bool UseResourcesAction = true;

    [UI("Say hello to all users of Rotation Solver.")]
    public bool SayHelloToAll = true;

    [UI("Say hello to the users of Rotation Solver.", Description = "It can only be disabled for users, not authors and contributors.\nIf you want to be greeted by other users, please DM ArchiTed in Discord Server with your Hash!")]
    public bool SayHelloToUsers = true;

    [UI("Just say hello once to the same user.")]
    public bool  JustSayHelloOnce = false;

    [UI("Use additional conditions")]
    public bool UseAdditionalConditions = false;

    [UI("Only Heal self When Not a healer")]
    public bool OnlyHealSelfWhenNoHealer = false;

    [UI("Display toggle action feedback on chat")]
    public bool ShowToggledActionInChat = true;
}
