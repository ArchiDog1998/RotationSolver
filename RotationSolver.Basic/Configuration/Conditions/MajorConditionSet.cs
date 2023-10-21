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

    public ConditionSet HealAreaConditionSet { get; set; } = new ConditionSet();
    public ConditionSet HealSingleConditionSet { get; set; } = new ConditionSet();
    public ConditionSet DefenseAreaConditionSet { get; set; } = new ConditionSet();
    public ConditionSet DefenseSingleConditionSet { get; set; } = new ConditionSet();
    public ConditionSet EsunaStanceNorthConditionSet { get; set; } = new ConditionSet();
    public ConditionSet RaiseShirkConditionSet { get; set; } = new ConditionSet();
    public ConditionSet MoveForwardConditionSet { get; set; } = new ConditionSet();
    public ConditionSet MoveBackConditionSet { get; set; } = new ConditionSet();
    public ConditionSet AntiKnockbackConditionSet { get; set; } = new ConditionSet();
    public ConditionSet BurstConditionSet { get; set; } = new ConditionSet();
    public ConditionSet SpeedConditionSet { get; set; } = new ConditionSet();

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
                return JsonConvert.DeserializeObject<MajorConditionSet>(str, new IConditionConverter());
            }
            catch
            {
                Svc.Chat.Print($"Failed to load the conditionSet from {p}");
                return null;
            }
        }).Where(set => set != null && !string.IsNullOrEmpty(set.Name)).ToArray();
    }
}
