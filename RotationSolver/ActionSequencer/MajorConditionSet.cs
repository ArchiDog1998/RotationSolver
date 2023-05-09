namespace RotationSolver.ActionSequencer;

internal class MajorConditionSet
{
    /// <summary>
    /// Key for action id.
    /// </summary>
    public Dictionary<uint, ConditionSet> Conditions { get; } = new Dictionary<uint, ConditionSet>();

    public Dictionary<uint, ConditionSet> DiableConditions { get; } = new Dictionary<uint, ConditionSet>();

    public string Name;

    public MajorConditionSet()
    {

    }

    public MajorConditionSet(string name)
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
                Service.ChatGui.Print($"Failed to load the conditionSet from {p}");
                return null;
            }
        }).Where(set => set != null && !string.IsNullOrEmpty(set.Name)).ToArray();
    }
}
