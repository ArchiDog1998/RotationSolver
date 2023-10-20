using ECommons.DalamudServices;

namespace RotationSolver.ActionSequencer;

internal class MajorConditionSet
{
    /// <summary>
    /// Key for action id.
    /// </summary>
    public Dictionary<uint, ConditionSet> Conditions { get; } = new ();

    public Dictionary<uint, ConditionSet> DiabledConditions { get; } = new();

    public string Name;
    public void DrawCondition(uint id, ICustomRotation rotation)
    {
        if (!Conditions.TryGetValue(id, out var conditionSet))
        {
            conditionSet = Conditions[id] = new ConditionSet();
        }

        if (conditionSet == null) return;

        ConditionHelper.DrawCondition(conditionSet.IsTrue(rotation));
        ImGui.SameLine();
        conditionSet?.Draw(rotation);
    }

    public void DrawDisabledCondition(uint id, ICustomRotation rotation)
    {
        if (!DiabledConditions.TryGetValue(id, out var conditionSet))
        {
            conditionSet = DiabledConditions[id] = new ConditionSet();
        }
        if (conditionSet == null) return;

        ConditionHelper.DrawCondition(conditionSet.IsTrue(rotation));
        ImGui.SameLine();
        conditionSet?.Draw(rotation);
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
                Svc.Chat.Print($"Failed to load the conditionSet from {p}");
                return null;
            }
        }).Where(set => set != null && !string.IsNullOrEmpty(set.Name)).ToArray();
    }
}
