using Dalamud.Configuration;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PluginConfig : IPluginConfiguration
{
    public static PluginConfig Create()
    {
        Svc.Log.Warning("You created a new configuration!");
        var result = new PluginConfig();
        result.SetValue(Job.WAR, JobConfigInt.HostileType, 0);
        result.SetValue(Job.DRK, JobConfigInt.HostileType, 0);
        result.SetValue(Job.PLD, JobConfigInt.HostileType, 0);
        result.SetValue(Job.GNB, JobConfigInt.HostileType, 0);
        return result;
    }

    [JsonProperty]
    private readonly Dictionary<Job, JobConfig> _jobsConfig = new();
    public GlobalConfig GlobalConfig { get; private set; } = new();
    public int Version { get; set; } = 7;

    public int GetValue(Job job, JobConfigInt config)
        => GetJobConfig(job).Ints.GetValue(config);

    public float GetValue(Job job, JobConfigFloat config)
        => GetJobConfig(job).Floats.GetValue(config);

    public int GetValue(PluginConfigInt config)
        => GlobalConfig.Ints.GetValue(config);

    public bool GetValue(PluginConfigBool config)
    {
        if (config != PluginConfigBool.UseAdditionalConditions
            && GetBoolRaw(PluginConfigBool.UseAdditionalConditions))
        {
            var rotation = DataCenter.RightNowRotation;
            var set = DataCenter.RightSet;
            if (rotation != null && set != null)
            {
                if (GetEnableBoolRaw(config) && set.GetEnableCondition(config).IsTrue(rotation)) return true;
                if (GetDisableBoolRaw(config) && set.GetDisableCondition(config).IsTrue(rotation)) return false;
            }
        }

        return GetBoolRaw(config);
    }

    public bool GetBoolRaw(PluginConfigBool config)
        => GlobalConfig.Bools.GetValue(config);

    public bool GetDisableBoolRaw(PluginConfigBool config)
        => GlobalConfig.ForcedDisableBools.GetValue(config);
    public bool GetEnableBoolRaw(PluginConfigBool config)
        => GlobalConfig.ForcedEnableBools.GetValue(config);

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

    public bool GetBoolRawDefault(PluginConfigBool config)
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
    public void SetBoolRaw(PluginConfigBool config, bool value)
        => GlobalConfig.Bools.SetValue(config, value);
    public void SetDisableBoolRaw(PluginConfigBool config, bool value)
        => GlobalConfig.ForcedDisableBools.SetValue(config, value);
    public void SetEnableBoolRaw(PluginConfigBool config, bool value)
        => GlobalConfig.ForcedEnableBools.SetValue(config, value);
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
        Svc.Log.Information("Saved configurations.");
#endif
        File.WriteAllText(Svc.PluginInterface.ConfigFile.FullName,
            JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.None,
            }));
    }
}

#region Job Config
[Serializable]
public class JobConfig
{
    public string RotationChoice { get; set; }
    public string PvPRotationChoice { get; set; }
    public DictionConfig<JobConfigFloat, float> Floats { get; set; } = new();

    public DictionConfig<JobConfigInt, int> Ints { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> RotationsConfigurations { get; set; } = new();
}

public enum JobConfigInt : byte
{
    [Default(2)] HostileType,
    [Default(2, 0, 3)] AddDotGCDCount,
}

public enum JobConfigFloat : byte
{
    [Default(0.55f), Unit(ConfigUnitType.Percent)] HealthAreaAbilityHot,
    [Default(0.55f), Unit(ConfigUnitType.Percent)] HealthAreaSpellHot,
    [Default(0.75f), Unit(ConfigUnitType.Percent)] HealthAreaAbility,
    [Default(0.65f), Unit(ConfigUnitType.Percent)] HealthAreaSpell,
    [Default(0.6f), Unit(ConfigUnitType.Percent)] HealthSingleAbilityHot,
    [Default(0.45f), Unit(ConfigUnitType.Percent)] HealthSingleSpellHot,
    [Default(0.7f), Unit(ConfigUnitType.Percent)] HealthSingleAbility,
    [Default(0.55f), Unit(ConfigUnitType.Percent)] HealthSingleSpell,

    [Default(0.15f), Unit(ConfigUnitType.Percent)] HealthForDyingTanks,
    [Default(1f), Unit(ConfigUnitType.Percent)] HealthForAutoDefense,

    [Default(0.08f, 0f, 0.5f), Unit(ConfigUnitType.Seconds)] ActionAhead,
}
#endregion

#region Global
[Serializable]
public class GlobalConfig
{
    public DictionConfig<PluginConfigInt, int> Ints { get; private set; } = new();
    public DictionConfig<PluginConfigBool, bool> Bools { get; private set; } = new();
    public DictionConfig<PluginConfigBool, bool> ForcedEnableBools { get; private set; } = new(defaultGetter: cmd => false);
    public DictionConfig<PluginConfigBool, bool> ForcedDisableBools { get; private set; } = new(defaultGetter: cmd => false);

    public DictionConfig<PluginConfigFloat, float> Floats { get; private set; } = new();
    public DictionConfig<PluginConfigVector4, Vector4> Vectors { get; private set; } = new(new()
    {
        { PluginConfigVector4.TeachingModeColor, new (0f, 1f, 0.8f, 1f)},
        { PluginConfigVector4.TTKTextColor, new (0f, 1f, 0.8f, 1f)},
        { PluginConfigVector4.MovingTargetColor, new (0f, 1f, 0.8f, 0.6f)},
        { PluginConfigVector4.BeneficialPositionColor, new (0.5f, 0.9f, 0.1f, 0.7f)},
        { PluginConfigVector4.HoveredBeneficialPositionColor, new (1f, 0.5f, 0f, 0.8f)},

        { PluginConfigVector4.TargetColor, new (1f, 0.2f, 0f, 0.8f)},
        { PluginConfigVector4.SubTargetColor, new (1f, 0.9f, 0f, 0.8f)},
        { PluginConfigVector4.ControlWindowLockBg, new (0, 0, 0, 0.55f)},
        { PluginConfigVector4.ControlWindowUnlockBg, new (0, 0, 0, 0.75f)},
        { PluginConfigVector4.InfoWindowBg, new (0, 0, 0, 0.4f)},
    });

    public SortedSet<Job> DisabledJobs { get; private set; } = new();
    public SortedSet<uint> DisabledActions { get; private set; } = new();
    public SortedSet<uint> NotInCoolDownActions { get; private set; } = new();
    public SortedSet<uint> NotInMistakeActions { get; private set; } = new();
    public SortedSet<uint> DisabledItems { get; private set; } = new();
    public SortedSet<uint> NotInCoolDownItems { get; private set; } = new();
    public List<ActionEventInfo> Events { get; private set; } = new();

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
    [Default(2, 1, 8)] AutoDefenseNumber,
}

public enum PluginConfigBool : byte
{
    [Default(true)] DrawIconAnimation,
    [Default(true)] AutoOffBetweenArea,
    [Default(true)] AutoOffCutScene,
    [Default(true)] AutoOffWhenDead,
    [Default(true)] AutoOffWhenDutyCompleted,
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
    [Default(false)] OnlyAttackInEnemyList,
    [Default(false)] UseTinctures,
    [Default(false)] UseHealPotions,

    [Default(true)] DrawMeleeOffset,

    [Default(true)] ShowMoveTarget,
    [Default(false)] ShowTargetTimeToKill,
    [Default(true)] ShowTarget,
    [Default(true)] ChooseAttackMark,
    [Default(true)] CanAttackMarkAOE,
    [Default(true)] FilterStopMark,
    [Default(true)] ShowHostilesIcons,

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

    [Default(false)] UseWorkTask,
    [Default(false)] UseStopCasting,
    [Default(false)] EsunaAll,
    [Default(false)] OnlyAttackInView,
    [Default(false)] OnlyAttackInVisionCone,
    [Default(false)] OnlyHotOnTanks,

    [Default(false)] InDebug,
    [Default(true)] AutoUpdateLibs,
    [Default(true)] DownloadRotations,
    [Default(true)] AutoUpdateRotations,

    [Default(false)] ToggleManual,
    [Default(false)] ToggleAuto,
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
    [Default(false)] UseGroundBeneficialAbilityWhenMoving,
    [Default(false)] TargetAllForFriendly,
    [Default(false)] ShowCooldownWindow,

    [Default(true)] RecordCastingArea,

    [Default(true)] AutoOffAfterCombat,
    [Default(false)] AutoOpenChest,
    [Default(true)] AutoCloseChestWindow,

    [Default(true)] ShowStateIcon,
    [Default(true)] ShowBeneficialPositions,
    [Default(false)] HideWarning,

    [Default(true)] HealWhenNothingTodo,
    [Default(true)] UseResourcesAction,
    [Default(true)] SayHelloToAll,
    [Default(true)] SayHelloToUsers,
    [Default(false)] JustSayHelloOnce,

    [Default(false)] UseAdditionalConditions,
    [Default(false)] OnlyHealSelfWhenNoHealer,

    [Default(true)] ShowToggledActionInChat,

}

public enum PluginConfigFloat : byte
{
    [Default(30f, 0f, 600f), Unit(ConfigUnitType.Seconds)] AutoOffAfterCombatTime,
    [Default(3f, 0f, 8f), Unit(ConfigUnitType.Yalms)] DrawingHeight,
    [Default(0.2f, 0.005f, 0.05f), Unit(ConfigUnitType.Yalms)] SampleLength,
    [Default(45f, 0f, 90f), Unit(ConfigUnitType.Degree)] AngleOfVisionCone,


    [Default(0.25f, 0f, 0.5f), Unit(ConfigUnitType.Percent)] HealthDifference,
    [Default(1f, 0f, 5f), Unit(ConfigUnitType.Yalms)] MeleeRangeOffset,
    [Default(0.1f, 0f, 0.4f), Unit(ConfigUnitType.Seconds)] MinLastAbilityAdvanced,
    [Default(0.8f, 0f, 1f), Unit(ConfigUnitType.Percent)] HealWhenNothingTodoBelow,
    [Default(0.6f, 0f, 1f), Unit(ConfigUnitType.Pixels)] TargetIconSize,

    [Default(0f, 0f, 1f), Unit(ConfigUnitType.Percent)] MistakeRatio,

    [Default(0.4f, 0f, 1f), Unit(ConfigUnitType.Percent)] HealthTankRatio,
    [Default(0.4f, 0f, 1f), Unit(ConfigUnitType.Percent)] HealthHealerRatio,

    [Default(3f, 1f, 20f), Unit(ConfigUnitType.Seconds)] SpecialDuration,

    [Default(0.06f, 0f, 0.5f), Unit(ConfigUnitType.Seconds)] ActionAheadForLast0GCD,

    [Default(0f, 0f, 1f), Unit(ConfigUnitType.Seconds)] WeaponDelayMin,
    [Default(0f)] WeaponDelayMax,

    [Default(1f, 0f, 3f), Unit(ConfigUnitType.Seconds)] DeathDelayMin,
    [Default(1.5f)] DeathDelayMax,

    [Default(0.5f, 0f, 3f), Unit(ConfigUnitType.Seconds)] WeakenDelayMin,
    [Default(1f)] WeakenDelayMax,

    [Default(0f, 0f, 3f), Unit(ConfigUnitType.Seconds)] HostileDelayMin,
    [Default(0f)] HostileDelayMax,

    [Default(0f, 0f, 3f), Unit(ConfigUnitType.Seconds)] HealDelayMin,
    [Default(0f)] HealDelayMax,

    [Default(0.5f, 0f, 3f), Unit(ConfigUnitType.Seconds)] StopCastingDelayMin,
    [Default(1f)] StopCastingDelayMax,

    [Default(0.5f, 0f, 3f), Unit(ConfigUnitType.Seconds)] InterruptDelayMin,
    [Default(1f)] InterruptDelayMax,

    [Default(3f, 0f, 10f), Unit(ConfigUnitType.Seconds)] NotInCombatDelayMin,
    [Default(4f)] NotInCombatDelayMax,

    [Default(0.1f, 0.05f, 0.25f), Unit(ConfigUnitType.Seconds)] ClickingDelayMin,
    [Default(0.15f)] ClickingDelayMax,

    [Default(0.5f, 0f, 10f), Unit(ConfigUnitType.Seconds)] ProvokeDelayMin,
    [Default(1f)] ProvokeDelayMax,

    [Default(0.5f, 0f, 5f), Unit(ConfigUnitType.Seconds)] HealWhenNothingTodoMin,
    [Default(1f)] HealWhenNothingTodoMax,

    [Default(0.5f, 0f, 3f), Unit(ConfigUnitType.Seconds)] CountdownDelayMin,
    [Default(1f), Unit(ConfigUnitType.Seconds)] CountdownDelayMax,
    [Default(0.4f, 0f, 0.7f), Unit(ConfigUnitType.Seconds)] CountDownAhead,

    [Default(24f, 0f, 90f), Unit(ConfigUnitType.Degree)] MoveTargetAngle,
    [Default(90f, 10f, 1800f), Unit(ConfigUnitType.Seconds)] BossTimeToKill,
    [Default(10f, 0f, 60f), Unit(ConfigUnitType.Seconds)] DyingTimeToKill,

    [Default(16f, 9.6f, 96f), Unit(ConfigUnitType.Pixels)] CooldownFontSize,

    [Default(40f, 0f, 80f), Unit(ConfigUnitType.Pixels)] ControlWindowGCDSize,
    [Default(30f, 0f, 80f), Unit(ConfigUnitType.Pixels)] ControlWindow0GCDSize,
    [Default(30f, 0f, 80f), Unit(ConfigUnitType.Pixels)] CooldownWindowIconSize,
    [Default(1.5f, 0f, 10f), Unit(ConfigUnitType.Percent)] ControlWindowNextSizeRatio,
    [Default(8f), Unit(ConfigUnitType.Pixels)] ControlProgressHeight,
    [Default(1.2f, 0f, 30f), Unit(ConfigUnitType.Yalms)] DistanceForMoving,
    [Default(0.2f, 0.01f, 0.5f), Unit(ConfigUnitType.Seconds)] MaxPing,

    [Default(8f, 0f, 30f), Unit(ConfigUnitType.Seconds)] AutoHealTimeToKill,
    [Default(0.5f, 0f, 10f), Unit(ConfigUnitType.Pixels)] HostileIconHeight,
    [Default(1f, 0.1f, 10f), Unit(ConfigUnitType.Percent)] HostileIconSize,

    [Default(1f, 0f, 3f), Unit(ConfigUnitType.Pixels)] StateIconHeight,
    [Default(1f, 0.2f, 10f), Unit(ConfigUnitType.Percent)] StateIconSize,

    [Default(0.02f, 0f, 1f), Unit(ConfigUnitType.Seconds)] MinUpdatingTime,
    [Default(0.15f), Unit(ConfigUnitType.Percent)] HealthForGuard,
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
    TTKTextColor,
}
#endregion

[AttributeUsage(AttributeTargets.Field)]
public class DefaultAttribute : Attribute
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

public enum ConfigUnitType : byte
{
    None,
    Seconds,
    Degree,
    Yalms,
    Percent,
    Pixels,
}

[AttributeUsage(AttributeTargets.Field)]
public class UnitAttribute : Attribute
{
    public ConfigUnitType UnitType { get; set; }

    public UnitAttribute(ConfigUnitType unitType)
    {
        UnitType = unitType;
    }
}

[Serializable]
public class DictionConfig<TConfig, TValue> where TConfig : struct, Enum
{
    [JsonProperty]
    private readonly Dictionary<TConfig, TValue> configs = new();

    private readonly SortedList<TConfig, TValue> _defaults;

    private readonly Func<TConfig, TValue> _defaultGetter;

    public DictionConfig(SortedList<TConfig, TValue> @default = null, Func<TConfig, TValue> defaultGetter = null)
    {
        _defaults = @default;
        _defaultGetter = defaultGetter ?? ((command) =>
            {
                return (TValue)command.GetAttribute<DefaultAttribute>()?.Default ?? default;
            });
    }

    public TValue GetValue(TConfig command)
        => configs.TryGetValue(command, out var value) ? value
        : GetDefault(command);

    public TValue GetDefault(TConfig command)
        => _defaults?.TryGetValue(command, out var value) ?? false ? value
        : Get_Default(command);

    private TValue Get_Default(TConfig command)
    {
        try
        {
            return _defaultGetter(command);
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to load the default value.");
            return default;
        }
    }

    public void SetValue(TConfig command, TValue value)
        => configs[command] = value;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
