using RotationSolver.Basic.Configuration.Condition;

namespace RotationSolver.Updaters;

internal class ActionSequencerUpdater
{
    static string? _actionSequencerFolder;

    public static void UpdateActionSequencerAction()
    {
        if (DataCenter.ConditionSets == null) return;
        var customRotation = DataCenter.RightNowRotation;
        if (customRotation == null) return;

        var allActions = RotationUpdater.RightRotationActions;

        var set = DataCenter.RightSet;
        if (set == null) return;

        DataCenter.DisabledActionSequencer = new HashSet<uint>(set.DisableConditionDict
            .Where(pair => pair.Value.IsTrue(customRotation) ?? false)
            .Select(pair => pair.Key));

        bool find = false;
        var conditions = set.ConditionDict;
        if (conditions != null)
        {
            foreach (var conditionPair in conditions)
            {
                var nextAct = allActions.FirstOrDefault(a => a.ID == conditionPair.Key);
                if (nextAct == null) continue;

                if (!(conditionPair.Value.IsTrue(customRotation) ?? false)) continue;

                DataCenter.ActionSequencerAction = nextAct;
                find = true;
                break;
            }
        }

        if (!find)
        {
            DataCenter.ActionSequencerAction = null;
        }
    }

    public static void Enable(string folder)
    {
        _actionSequencerFolder = folder;
        if (!Directory.Exists(_actionSequencerFolder)) Directory.CreateDirectory(_actionSequencerFolder);

        LoadFiles();
    }

    public static void SaveFiles()
    {
        if (_actionSequencerFolder == null) return;
        try
        {
            if (!Directory.Exists(_actionSequencerFolder))
            {
                Directory.CreateDirectory(_actionSequencerFolder);
            }
        }
        catch
        {

        }
        foreach (var set in DataCenter.ConditionSets)
        {
            set.Save(_actionSequencerFolder);
        }
    }

    public static void LoadFiles()
    {
        if (_actionSequencerFolder == null) return;

        DataCenter.ConditionSets = MajorConditionSet.Read(_actionSequencerFolder);
    }

    public static void AddNew()
    {
        if (!DataCenter.ConditionSets.Any(c => c.IsUnnamed))
        {
            DataCenter.ConditionSets = [.. DataCenter.ConditionSets, new MajorConditionSet()];
        }
    }

    public static void Delete(string name)
    {
        DataCenter.ConditionSets = DataCenter.ConditionSets.Where(c => c.Name != name).ToArray();
        File.Delete(_actionSequencerFolder + $"\\{name}.json");
    }
}
