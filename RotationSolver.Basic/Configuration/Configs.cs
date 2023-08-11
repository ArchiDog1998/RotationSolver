using Dalamud.Configuration;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Serializable] public class PluginConfig : IPluginConfiguration
{
    [JsonProperty]
    private Dictionary<Job, JobConfig> _jobsConfig = new();
    public GlobalConfig GlobalConfig { get; private set; } = new();
    public int Version { get; set; } = 7;

    public int GetValue(Job job, JobConfigInt config)
        => GetJobConfig(job).Ints.GetValue(config);

    public float GetValue(Job job, JobConfigFloat config)
        => GetJobConfig(job).Floats.GetValue(config);

    public int GetValue(PluginConfigInt config)
        => GlobalConfig.Ints.GetValue(config);

    public bool GetValue(PluginConfigBool config)
        => GlobalConfig.Bools.GetValue(config);

    public float GetValue(PluginConfigFloat config)
        => GlobalConfig.Floats.GetValue(config);

    public Vector4 GetValue(PluginConfigVector4 config)
        => GlobalConfig.Vectors.GetValue(config);

    public int GetDefault(Job job, JobConfigInt config)
        => GetJobConfig(job).Ints.GetDefault(config);

    public float GetDefault(Job job, JobConfigFloat config)
        => GetJobConfig(job).Floats.GetDefault(config);

    public int GetDefault(PluginConfigInt config)
        => GlobalConfig.Ints.GetDefault(config);

    public bool GetDefault(PluginConfigBool config)
        => GlobalConfig.Bools.GetDefault(config);

    public float GetDefault(PluginConfigFloat config)
        => GlobalConfig.Floats.GetDefault(config);

    public Vector4 GetDefault(PluginConfigVector4 config)
        => GlobalConfig.Vectors.GetDefault(config);

    public void SetValue(Job job, JobConfigInt config, int value)
        => GetJobConfig(job).Ints.SetValue(config, value);

    public void SetValue(Job job, JobConfigFloat config, float value)
        => GetJobConfig(job).Floats.SetValue(config, value);

    public void SetValue(PluginConfigInt config, int value)
        => GlobalConfig.Ints.SetValue(config, value);

    public void SetValue(PluginConfigBool config, bool value)
        => GlobalConfig.Bools.SetValue(config, value);

    public void SetValue(PluginConfigFloat config, float value)
        => GlobalConfig.Floats.SetValue(config, value);

    public void SetValue(PluginConfigVector4 config, Vector4 value)
        => GlobalConfig.Vectors.SetValue(config, value);

    public JobConfig GetJobConfig(Job job)
    {
        if (_jobsConfig.TryGetValue(job, out var config)) return config;
        return _jobsConfig[job] = new JobConfig();
    }

    public void Save()
        => Svc.PluginInterface.SavePluginConfig(this);
}

#region Job Config
[Serializable] public class JobConfig
{
    public string RotationChoice { get; set; }
    public DictionConfig<JobConfigFloat, float> Floats { get; private set; } = new();

    public DictionConfig<JobConfigInt, int> Ints { get; private set; } = new();
    public Dictionary<string, Dictionary<string, string>> RotationsConfigurations { get; private set; } = new ();
}

public enum JobConfigInt : byte
{
    //TODO : Type binding by jobs.
    [Default(2)] HostileType,
    [Default(2)] AddDotGCDCount,
}

public enum JobConfigFloat : byte
{
    [Default(0.55f)] HealthAreaAbilityHot,
    [Default(0.55f)] HealthAreaSpellHot,
    [Default(0.75f)] HealthAreaAbility,
    [Default(0.65f)] HealthAreaSpell,
    [Default(0.6f)] HealthSingleAbilityHot,
    [Default(0.45f)] HealthSingleSpellHot,
    [Default(0.7f)] HealthSingleAbility,
    [Default(0.55f)] HealthSingleSpell,

    [Default(0.15f)] HealthForDyingTanks,
}
#endregion

#region Global
[Serializable] public class GlobalConfig
{
    public DictionConfig<PluginConfigInt, int> Ints { get; private set; } = new();
    public DictionConfig<PluginConfigBool, bool> Bools { get; private set; } = new();
    public DictionConfig<PluginConfigFloat, float> Floats { get; private set; } = new();
    public DictionConfig<PluginConfigVector4, Vector4> Vectors { get; private set; } = new(new()
    {
        { PluginConfigVector4.TeachingModeColor, new (0f, 1f, 0.8f, 1f)},
        { PluginConfigVector4.MovingTargetColor, new (0f, 1f, 0.8f, 0.6f)},
        { PluginConfigVector4.TargetColor, new (1f, 0.2f, 0f, 0.8f)},
        { PluginConfigVector4.SubTargetColor, new (1f, 0.9f, 0f, 0.8f)},
        { PluginConfigVector4.ControlWindowLockBg, new (0, 0, 0, 0.6f)},
        { PluginConfigVector4.ControlWindowUnlockBg, new (0, 0, 0, 0.9f)},
        { PluginConfigVector4.InfoWindowBg, new (0, 0, 0, 0.4f)},
    });

    public SortedSet<string> DisabledCombos { get; private set; } = new ();
    public SortedSet<uint> DisabledActions { get; private set; } = new ();
    public SortedSet<uint> NotInCoolDownActions { get; private set; } = new ();
    public SortedSet<uint> DisabledItems { get; private set; } = new ();
    public SortedSet<uint> NotInCoolDownItems { get; private set; } = new ();
    public List<ActionEventInfo> Events { get; private set; } = new ();

    public string[] OtherLibs = Array.Empty<string>();
    public List<TargetingType> TargetingTypes { get; set; } = new List<TargetingType>();

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();
}

public enum PluginConfigInt : byte
{
    [Default(0)] ActionSequencerIndex,
    [Default(0)] PoslockModifier,
    [Default(0)] LessMPNoRaise,
    [Default(2)] KeyBoardNoiseMin,
    [Default(3)] KeyBoardNoiseMax,

    [Default(0)] TargetingIndex,

    [Obsolete]
    [Default(15)] CooldownActionOneLine,
}

public enum PluginConfigBool : byte
{
    [Default(true)] DrawIconAnimation,
    [Default(true)] AutoOffBetweenArea,
    [Default(true)] AutoOffCutScene,
    [Default(true)] AutoOffWhenDead,
    [Default(false)] PreventActionsIfOutOfCombat,
    [Default(false)] PreventActionsIfDutyRing,
    [Default(true)] ChangeTargetForFate,
    [Default(true)] MoveTowardsScreenCenter,
    [Default(true)] SayOutStateChanged,
    [Default(true)] ShowInfoOnDtr,
    [Default(false)] HealOutOfCombat,
    [Default(true)] ShowInfoOnToast,
    [Default(false)] RaiseAll,

    [Default(false)] PoslockCasting,
    [Default(false)] PosPassageOfArms,
    [Default(true)] PosTenChiJin,
    [Default(false)] PosFlameThrower,
    [Default(false)] PosImprovisation,

    [Default(true)] RaisePlayerByCasting,
    [Default(true)] RaiseBrinkOfDeath,
    [Default(true)] AddEnemyListToHostile,
    [Default(false)] UseTinctures,
    [Default(false)] UseHealPotions,

    [Default(true)] DrawPositional,
    [Default(true)] DrawMeleeOffset,

    [Default(true)] ShowMoveTarget,
    [Default(false)] ShowHealthRatio,
    [Default(true)] ShowTarget,
    [Default(true)] ChooseAttackMark,
    [Default(false)] CanAttackMarkAOE,
    [Default(true)] FilterStopMark,

    [Default(true)] TeachingMode,
    [Default(true)] UseOverlayWindow,
    [Default(true)] KeyBoardNoise,

    [Default(true)] MoveAreaActionFarthest,
    [Default(true)] StartOnCountdown,
    [Default(false)] StartOnAttackedBySomeone,
    [Default(false)] NoNewHostiles,

    [Default(true)] UseHealWhenNotAHealer,
    [Default(false)] TargetFriendly,

    [Default(true)] InterruptibleMoreCheck,

    [Default(true)] UseWorkTask,
    [Default(false)] UseStopCasting,
    [Default(false)] EsunaAll,
    [Default(false)] OnlyAttackInView,
    [Default(false)] OnlyHotOnTanks,
    [Default(false)] BeneficialAreaOnTarget,

    [Default(false)] InDebug,
    [Default(true)] AutoUpdateLibs,
    [Default(true)] DownloadRotations,
    [Default(true)] AutoUpdateRotations,

    [Default(false)] ToggleManual,
    [Default(true)] OnlyShowWithHostileOrInDuty,
    [Default(false)] ShowControlWindow,
    [Default(false)] IsControlWindowLock,
    [Default(true)] ShowNextActionWindow,
    [Default(false)] IsInfoWindowNoInputs,
    [Default(false)] IsInfoWindowNoMove,
    [Default(false)] ShowItemsCooldown,
    [Default(false)] ShowGCDCooldown,
    [Default(true)] UseOriginalCooldown,

    [Default(true)] ShowTooltips,
    [Default(false)] AutoLoadCustomRotations,

    [Default(true)] TargetFatePriority,
    [Default(true)] TargetHuntingRelicLevePriority,
    [Default(true)] TargetQuestPriority,
    [Default(true)] ShowToastsAboutDoAction,
}

public enum PluginConfigFloat : byte
{
    [Default(8f)] AutoOffAfterCombat,
    [Default(3f)] DrawingHeight,
    [Default(0.2f)] SampleLength,
    [Default(0.1f)] KeyBoardNoiseTimeMin,
    [Default(0.2f)] KeyBoardNoiseTimeMax,

    [Default(0.25f)] HealthDifference,
    [Default(1f)] MeleeRangeOffset,
    [Default(0.1f)] MinLastAbilityAdvanced,
    [Default(0.8f)] HealWhenNothingTodoBelow,
    [Default(0.6f)] TargetIconSize,

    [Default(0f)] MistakeRatio,

    [Default(0.4f)] HealthTankRatio,
    [Default(0.4f)] HealthHealerRatio,

    [Default(3f)] SpecialDuration,

    [Default(0.08f)] ActionAhead,
    [Default(0.06f)] ActionAheadForLast0GCD,

    [Default(0f)] WeaponDelayMin,
    [Default(0f)] WeaponDelayMax,

    [Default(1f)] DeathDelayMin,
    [Default(1.5f)] DeathDelayMax,

    [Default(0.5f)] WeakenDelayMin,
    [Default(1f)] WeakenDelayMax,

    [Default(0f)] HostileDelayMin,
    [Default(0f)] HostileDelayMax,

    [Default(0f)] HealDelayMin,
    [Default(0f)] HealDelayMax,

    [Default(0.5f)] StopCastingDelayMin,
    [Default(1f)] StopCastingDelayMax,

    [Default(0.5f)] InterruptDelayMin,
    [Default(1f)] InterruptDelayMax,

    [Default(3f)] NotInCombatDelayMin,
    [Default(4f)] NotInCombatDelayMax,

    [Default(0.1f)] ClickingDelayMin,
    [Default(0.15f)] ClickingDelayMax,

    [Default(0.5f)] CountdownDelayMin,
    [Default(1f)] CountdownDelayMax,

    [Default(0.6f)] CountDownAhead,

    [Default(24f)] MoveTargetAngle,
    [Default(1.85f)] HealthRatioBoss,
    [Default(0.8f)] HealthRatioDying,
    [Default(1.2f)] HealthRatHealthRatioDotioBoss,

    [Default(16f)] CooldownFontSize,

    [Default(40f)] ControlWindowGCDSize,
    [Default(30f)] ControlWindow0GCDSize,
    [Default(30f)] CooldownWindowIconSize,
    [Default(1.5f)] ControlWindowNextSizeRatio,
    [Default(8f)] ControlProgressHeight,
    [Default(1.2f)] DistanceForMoving,
    [Default(0.2f)] MaxPing,
}

public enum PluginConfigVector4 : byte
{
    TeachingModeColor,
    MovingTargetColor,
    TargetColor,
    SubTargetColor,
    ControlWindowLockBg,
    ControlWindowUnlockBg,
    InfoWindowBg,
}
#endregion

[AttributeUsage(AttributeTargets.Field)] public class DefaultAttribute : Attribute
{
    public object Default { get; set; }

    public DefaultAttribute(object @default)
    {
        Default = @default;
    }
}

[Serializable] public class DictionConfig<TConfig, Tvalue> where TConfig : struct, Enum
{
    [JsonProperty]
    private Dictionary<TConfig, Tvalue> configs = new ();

    private readonly SortedList<TConfig, Tvalue> _defaults;

    public DictionConfig(SortedList<TConfig, Tvalue> @default = null)
    {
        _defaults = @default;
    }

    public Tvalue GetValue(TConfig command)
        => configs.TryGetValue(command, out var value) ? value
        : GetDefault(command);

    public Tvalue GetDefault(TConfig command)
        => _defaults?.TryGetValue(command, out var value) ?? false ? value
        : (Tvalue)command.GetAttribute<DefaultAttribute>()?.Default ?? default;

    public void SetValue(TConfig command, Tvalue value)
        => configs[command] = value;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
