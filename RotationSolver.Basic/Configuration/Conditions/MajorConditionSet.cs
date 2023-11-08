using ECommons.DalamudServices;
using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration.Conditions;

internal class MajorConditionSet
{
    const string conditionName = "Unnamed";

    [JsonIgnore]
    public bool IsUnnamed => Name == conditionName;

    /// <summary>
    /// Key for action id.
    /// </summary>
    public Dictionary<Job, Dictionary<uint, ConditionSet>> Conditions { get; } = new();

    [JsonIgnore]
    public Dictionary<uint, ConditionSet> ConditionDict
    {
        get
        {
            if (!Conditions.TryGetValue(DataCenter.Job, out var dict))
            {
                dict = Conditions[DataCenter.Job] = new();
            }
            return dict;
        }
    }

    public Dictionary<Job, Dictionary<uint, ConditionSet>> DisabledConditions { get; } = new();

    [JsonIgnore]
    public Dictionary<uint, ConditionSet> DisableConditionDict
    {
        get
        {
            if (!DisabledConditions.TryGetValue(DataCenter.Job, out var dict))
            {
                dict = DisabledConditions[DataCenter.Job] = new();
            }
            return dict;
        }
    }

    public Dictionary<PluginConfigBool, ConditionSet> ForceEnableConditions { get; private set; }
        = new();

    public Dictionary<PluginConfigBool, ConditionSet> ForceDisableConditions { get; private set; }
        = new();

    public ConditionSet HealAreaConditionSet { get; set; } = new();
    public ConditionSet HealSingleConditionSet { get; set; } = new();
    public ConditionSet DefenseAreaConditionSet { get; set; } = new();
    public ConditionSet DefenseSingleConditionSet { get; set; } = new();
    public ConditionSet EsunaStanceNorthConditionSet { get; set; } = new();
    public ConditionSet RaiseShirkConditionSet { get; set; } = new();
    public ConditionSet MoveForwardConditionSet { get; set; } = new();
    public ConditionSet MoveBackConditionSet { get; set; } = new();
    public ConditionSet AntiKnockbackConditionSet { get; set; } = new();
    public ConditionSet SpeedConditionSet { get; set; } = new();
    public ConditionSet SwitchAutoConditionSet { get; set; } = new();
    public ConditionSet SwitchManualConditionSet { get; set; } = new();
    public ConditionSet SwitchCancelConditionSet { get; set; } = new();

    public (string Name, ConditionSet Condition)[] NamedConditions { get; set; }
        = Array.Empty<(string, ConditionSet)>();

    public string Name;

    public ConditionSet GetCondition(uint id)
    {
        if (!ConditionDict.TryGetValue(id, out var conditionSet))
        {
            conditionSet = ConditionDict[id] = new ConditionSet();
        }
        return conditionSet;
    }

    public ConditionSet GetDisabledCondition(uint id)
    {
        if (!DisableConditionDict.TryGetValue(id, out var conditionSet))
        {
            conditionSet = DisableConditionDict[id] = new ConditionSet();
        }
        return conditionSet;
    }

    public ConditionSet GetEnableCondition(PluginConfigBool config)
    {
        if (!ForceEnableConditions.TryGetValue(config, out var conditionSet))
        {
            conditionSet = ForceEnableConditions[config] = new ConditionSet();
        }
        return conditionSet;
    }

    public ConditionSet GetDisableCondition(PluginConfigBool config)
    {
        if (!ForceDisableConditions.TryGetValue(config, out var conditionSet))
        {
            conditionSet = ForceDisableConditions[config] = new ConditionSet();
        }
        return conditionSet;
    }

    public MajorConditionSet(string name = conditionName)
    {
        Name = name;
    }

    public void Save(string folder)
    {
        if (!Directory.Exists(folder)) return;
        var path = Path.Combine(folder, Name + ".json");

        var str = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, str);
    }

    public static MajorConditionSet[] Read(string folder)
    {
        if (!Directory.Exists(folder)) return Array.Empty<MajorConditionSet>();

        return Directory.EnumerateFiles(folder, "*.json").Select(p =>
        {
            var str = File.ReadAllText(p);

            try
            {
                return JsonConvert.DeserializeObject<MajorConditionSet>(str, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        args.ErrorContext.Handled = true;
                    },
                    Converters = new List<JsonConverter>() { new IConditionConverter() }
                });
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, $"Failed to load the ConditionSet from {p}");
                Svc.Chat.Print($"Failed to load the ConditionSet from {p}");
                return null;
            }
        }).Where(set => set != null && !string.IsNullOrEmpty(set.Name)).ToArray();
    }
}
