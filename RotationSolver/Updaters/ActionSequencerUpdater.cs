using Dalamud.Interface.Colors;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.Updaters;

internal class ActionSequencerUpdater
{
    static string _actionSequencerFolder;

   
    public static string[] ConditionSetsName => DataCenter.ConditionSets?.Select(s => s.Name).ToArray() ?? Array.Empty<string>();

    public static void UpdateActionSequencerAction()
    {
        if (DataCenter.ConditionSets == null) return;
        var customRotation = DataCenter.RightNowRotation;
        if (customRotation == null) return;

        var allActions = RotationUpdater.RightRotationActions;

        var set = DataCenter.RightSet;
        if (set == null) return;

        DataCenter.DisabledActionSequencer = new HashSet<uint>(set.DisableConditionDict
            .Where(pair => pair.Value.IsTrue(customRotation))
            .Select(pair => pair.Key));

        bool find = false;
        var conditions = set.ConditionDict;
        if (conditions != null)
        {
            foreach (var conditionPair in conditions)
            {
                var nextAct = allActions.FirstOrDefault(a => a.ID == conditionPair.Key);
                if (nextAct == null) continue;

                if (!conditionPair.Value.IsTrue(customRotation)) continue;

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
        try
        {
            Directory.Delete(_actionSequencerFolder);
            Directory.CreateDirectory(_actionSequencerFolder);
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
        DataCenter.ConditionSets = MajorConditionSet.Read(_actionSequencerFolder);
    }

    private static void AddNew()
    {
        if (!DataCenter.ConditionSets.Any(c => c.IsUnnamed))
        {
            DataCenter.ConditionSets = DataCenter.ConditionSets.Append(new MajorConditionSet());
        }
    }

    private static void Delete(string name)
    {
        DataCenter.ConditionSets = DataCenter.ConditionSets.Where(c => c.Name != name);
        File.Delete(_actionSequencerFolder + $"\\{name}.json");
    }

    public static void DrawHeader(float width)
    {
        var set = DataCenter.RightSet;
        bool hasSet = set != null;

        if (hasSet)
        {
            ImGuiHelper.SetNextWidthWithName(set.Name);
            ImGui.InputText("##MajorConditionSet", ref set.Name, 100);

            ImGui.SameLine();
        }

        var combos = ConditionSetsName;
        ImGui.SetNextItemWidth(width);

        if(ImGui.BeginCombo("##MajorConditionCombo", ""))
        {
            for (int i = 0; i < combos.Length; i++)
            {
                void DeleteFile()
                {
                    Delete(combos[i]);
                }

                var key = "Condition Set At " + i.ToString();

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, DeleteFile, new string[] { "Delete" }));


                if (ImGui.Selectable(combos[i]))
                {
                    Service.Config.SetValue(PluginConfigInt.ActionSequencerIndex, i);
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false,
    (DeleteFile, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
            }

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Selectable(FontAwesomeIcon.Plus.ToIconString()))
            {
                AddNew();
            }
            ImGui.PopFont();
            ImGui.PopStyleColor();
            ImGui.EndCombo();
        }

        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesomeIcon.FileDownload, "##LoadTheConditions"))
        {
            LoadFiles();
        }
        ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_Load);

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_ConditionDescription);
    }
}
