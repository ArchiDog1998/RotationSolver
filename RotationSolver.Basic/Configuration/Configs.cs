using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PluginConfig : IPluginConfiguration
{
    public static PluginConfig Create()
    {
        PluginLog.Warning("You created a new configuration!");
        var result = new PluginConfig();
        result.SetValue(Job.WAR, JobConfigInt.HostileType, 0);
        result.SetValue(Job.DRK, JobConfigInt.HostileType, 0);
        result.SetValue(Job.PLD, JobConfigInt.HostileType, 0);
        result.SetValue(Job.GNB, JobConfigInt.HostileType, 0);
        return result;
    }

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
    {
        var attr = config.GetAttribute<DefaultAttribute>();
        if (attr != null)
        {
            var min = attr.Min; var max = attr.Max;
            if (min != null && max != null)
            {
                value = Math.Min(Math.Max(value, (int)min), (int)max);
            }
        }
        GetJobConfig(job).Ints.SetValue(config, value);
    }

    public void SetValue(Job job, JobConfigFloat config, float value)
    {
        var attr = config.GetAttribute<DefaultAttribute>();
        if (attr != null)
        {
            var min = attr.Min; var max = attr.Max;
            if (min != null && max != null)
            {
                value = MathF.Min(MathF.Max(value, (float)min), (float)max);
            }
        }
        GetJobConfig(job).Floats.SetValue(config, value);
    }

    public void SetValue(PluginConfigInt config, int value)
    {
        var attr = config.GetAttribute<DefaultAttribute>();
        if (attr != null)
        {
            var min = attr.Min; var max = attr.Max;
            if (min != null && max != null)
            {
                value = Math.Min(Math.Max(value, (int)min), (int)max);
            }
        }
        GlobalConfig.Ints.SetValue(config, value);
    }
    public void SetValue(PluginConfigBool config, bool value)
        => GlobalConfig.Bools.SetValue(config, value);

    public void SetValue(PluginConfigFloat config, float value)
    {
        var attr = config.GetAttribute<DefaultAttribute>();
        if (attr != null)
        {
            var min = attr.Min; var max = attr.Max;
            if (min != null && max != null)
            {
                value = MathF.Min(MathF.Max(value, (float)min), (float)max);
            }
        }
        GlobalConfig.Floats.SetValue(config, value);
    }

    public void SetValue(PluginConfigVector4 config, Vector4 value)
        => GlobalConfig.Vectors.SetValue(config, value);

    public JobConfig GetJobConfig(Job job)
    {
        if (_jobsConfig.TryGetValue(job, out var config)) return config;
        return _jobsConfig[job] = new JobConfig();
    }

    public void Save()
    {
#if DEBUG
        PluginLog.Information("Saved configurations.");
#endif
        File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName,
            JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
            {
                 TypeNameHandling = TypeNameHandling.None,
            }));
    }
}

#region Job Config
[Serializable] public class JobConfig
{
    public string RotationChoice { get; set; }
    public DictionConfig<JobConfigFloat, float> Floats { get; set; } = new();

    public DictionConfig<JobConfigInt, int> Ints { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> RotationsConfigurations { get; set; } = new ();
}

public enum JobConfigInt : byte
{
    [Default(2)] HostileType,
    [Default(2, 0, 3)] AddDotGCDCount,
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
        { PluginConfigVector4.BeneficialPositionColor, new (0.5f, 0.9f, 0.1f, 0.7f)},
        { PluginConfigVector4.HoveredBeneficialPositionColor, new (1f, 0.5f, 0f, 0.8f)},

        { PluginConfigVector4.TargetColor, new (1f, 0.2f, 0f, 0.8f)},
        { PluginConfigVector4.SubTargetColor, new (1f, 0.9f, 0f, 0.8f)},
        { PluginConfigVector4.ControlWindowLockBg, new (0, 0, 0, 0.6f)},
        { PluginConfigVector4.ControlWindowUnlockBg, new (0, 0, 0, 0.75f)},
        { PluginConfigVector4.InfoWindowBg, new (0, 0, 0, 0.4f)},
    });

    public SortedSet<Job> DisabledJobs { get; private set; } = new ();
    public SortedSet<uint> DisabledActions { get; private set; } = new ();
    public SortedSet<uint> NotInCoolDownActions { get; private set; } = new ();
    public SortedSet<uint> DisabledItems { get; private set; } = new ();
    public SortedSet<uint> NotInCoolDownItems { get; private set; } = new ();
    public List<ActionEventInfo> Events { get; private set; } = new ();

    public string[] OtherLibs = Array.Empty<string>();

    public string[] GitHubLibs = Array.Empty<string>();
    public List<TargetingType> TargetingTypes { get; set; } = new List<TargetingType>();

    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();
}

public enum PluginConfigInt : byte
{
    [Default(0)] ActionSequencerIndex,
    [Default(0)] PoslockModifier,
    [Default(0, 0, 10000)] LessMPNoRaise,

    [Default(2, 0, 5)] KeyBoardNoiseMin,
    [Default(3)] KeyBoardNoiseMax,

    [Default(0)] TargetingIndex,
    [Default(0)] BeneficialAreaStrategy,
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

    [Default(true)] DrawMeleeOffset,

    [Default(true)] ShowMoveTarget,
    [Default(false)] ShowTargetDeadTime,
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
    [Default(false)] SwitchTargetFriendly,

    [Default(true)] InterruptibleMoreCheck,

    [Default(true)] UseWorkTask,
    [Default(false)] UseStopCasting,
    [Default(false)] EsunaAll,
    [Default(false)] OnlyAttackInView,
    [Default(false)] OnlyHotOnTanks,

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

    [Default(true)] UseAOEAction,
    [Default(false)] UseAOEWhenManual,
    [Default(false)] PreventActions,
    [Default(false)] PreventActionsDuty,
    [Default(true)] AutoBurst,
    [Default(true)] AutoHeal,
    [Default(true)] UseAbility,
    [Default(true)] UseDefenseAbility,
    [Default(true)] AutoTankStance,
    [Default(true)] AutoProvokeForTank,
    [Default(true)] AutoUseTrueNorth,
    [Default(true)] RaisePlayerBySwift,
    [Default(true)] AutoSpeedOutOfCombat,
    [Default(true)] UseGroundBeneficialAbility,
    [Default(false)] TargetAllForFriendly,
    [Default(false)] ShowCooldownWindow,

    [Default(true)] RecordCastingArea,

    [Default(false)] AutoOpenChest,
    [Default(true)] AutoCloseChestWindow,
    [Default(true)] AutoOffAfterCombat,

    [Default(true)] ShowBeneficialPositions,
    [Default(false)] HideWarning,
}

public enum PluginConfigFloat : byte
{
    [Default(8f, 0f, 60f)] AutoOffAfterCombatTime,
    [Default(3f, 0f, 8f)] DrawingHeight,
    [Default(0.2f, 0.005f, 0.05f)] SampleLength,
    [Default(0.1f)] KeyBoardNoiseTimeMin,
    [Default(0.2f)] KeyBoardNoiseTimeMax,

    [Default(0.25f, 0f, 0.5f)] HealthDifference,
    [Default(1f, 0f, 5f)] MeleeRangeOffset,
    [Default(0.1f, 0f, 0.4f)] MinLastAbilityAdvanced,
    [Default(0.8f, 0f, 1f)] HealWhenNothingTodoBelow,
    [Default(0.6f, 0f, 1f)] TargetIconSize,

    [Default(0f, 0f, 1f)] MistakeRatio,

    [Default(0.4f, 0f, 1f)] HealthTankRatio,
    [Default(0.4f, 0f, 1f)] HealthHealerRatio,

    [Default(3f, 1f, 20f)] SpecialDuration,

    [Default(0.08f, 0f, 0.5f)] ActionAhead,
    [Default(0.06f)] ActionAheadForLast0GCD,

    [Default(0f, 0f, 1f)] WeaponDelayMin,
    [Default(0f)] WeaponDelayMax,

    [Default(1f, 0f, 3f)] DeathDelayMin,
    [Default(1.5f)] DeathDelayMax,

    [Default(0.5f, 0f, 3f)] WeakenDelayMin,
    [Default(1f)] WeakenDelayMax,

    [Default(0f, 0f, 3f)] HostileDelayMin,
    [Default(0f)] HostileDelayMax,

    [Default(0f, 0f, 3f)] HealDelayMin,
    [Default(0f)] HealDelayMax,

    [Default(0.5f, 0f, 3f)] StopCastingDelayMin,
    [Default(1f)] StopCastingDelayMax,

    [Default(0.5f, 0f, 3f)] InterruptDelayMin,
    [Default(1f)] InterruptDelayMax,

    [Default(3f, 0f, 10f)] NotInCombatDelayMin,
    [Default(4f)] NotInCombatDelayMax,

    [Default(0.1f, 0.05f, 0.25f)] ClickingDelayMin,
    [Default(0.15f)] ClickingDelayMax,

    [Default(0.5f, 0f, 3f)] CountdownDelayMin,
    [Default(1f)] CountdownDelayMax,
    [Default(0.6f, 0.5f, 0.7f)] CountDownAhead,

    [Default(24f, 0f, 90f)] MoveTargetAngle,
    [Default(60f, 10f, 1800f)] DeadTimeBoss,
    [Default(10f, 0f, 60f)] DeadTimeDying,

    [Default(16f, 9.6f, 96f)] CooldownFontSize,

    [Default(40f, 0f, 80f)] ControlWindowGCDSize,
    [Default(30f, 0f, 80f)] ControlWindow0GCDSize,
    [Default(30f, 0f, 80f)] CooldownWindowIconSize,
    [Default(1.5f, 0f, 10f)] ControlWindowNextSizeRatio,
    [Default(8f)] ControlProgressHeight,
    [Default(1.2f, 0f, 30f)] DistanceForMoving,
    [Default(0.2f, 0.01f, 0.5f)] MaxPing,
}

public enum PluginConfigVector4 : byte
{
    TeachingModeColor,
    MovingTargetColor,
    TargetColor,
    SubTargetColor,
    BeneficialPositionColor,
    HoveredBeneficialPositionColor,
    ControlWindowLockBg,
    ControlWindowUnlockBg,
    InfoWindowBg,
}
#endregion

[AttributeUsage(AttributeTargets.Field)] public class DefaultAttribute : Attribute
{
    public object Default { get; set; }
    public object Min { get; set; }
    public object Max { get; set; }

    public DefaultAttribute(object @default, object min = null, object max = null)
    {
        Default = @default;
        Min = min;
        Max = max;
    }
}

[Serializable] public class DictionConfig<TConfig, TValue> where TConfig : struct, Enum
{
    [JsonProperty]
    private Dictionary<TConfig, TValue> configs = new ();

    private readonly SortedList<TConfig, TValue> _defaults;

    public DictionConfig(SortedList<TConfig, TValue> @default = null)
    {
        _defaults = @default;
    }

    public TValue GetValue(TConfig command)
        => configs.TryGetValue(command, out var value) ? value
        : GetDefault(command);

    public TValue GetDefault(TConfig command)
        => _defaults?.TryGetValue(command, out var value) ?? false ? value
        : (TValue)command.GetAttribute<DefaultAttribute>()?.Default ?? default;

    public void SetValue(TConfig command, TValue value)
        => configs[command] = value;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
