using ImGuiNET;
using Newtonsoft.Json;
using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonAOZNotebook;

namespace RotationSolver.Timeline;

internal class MajorConditionSet
{
    public Dictionary<ActionID, ConditionSet> Conditions { get; } = new Dictionary<ActionID, ConditionSet>();

    public string Name;

    public MajorConditionSet(string name)
    {
        Name = name;
    }

    public void Save(string folder)
    {
        if (!Directory.Exists(folder)) return;
        var path = Path.Combine(folder, Name + ".json");

        var str = JsonConvert.SerializeObject(this);
        File.WriteAllText(path, str);
    }

    public static IEnumerable<MajorConditionSet> Read(string folder)
    {
        if (!Directory.Exists(folder)) return new MajorConditionSet[0];

        return Directory.EnumerateFiles(folder, "*.json").Select(p =>
        {
            var str = File.ReadAllText(p);

            return JsonConvert.DeserializeObject<MajorConditionSet>(str, new IConditionConverter());
        }).Where(set => set != null);
    }
}
