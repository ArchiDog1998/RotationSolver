using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using RotationSolver.ActionSequencer;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI;
using System.Diagnostics;
using System.Windows.Forms;

namespace RotationSolver.Updaters;

internal class ActionSequencerUpdater
{
    static string _actionSequencerFolder;

    static IEnumerable<MajorConditionSet> _conditionSet;
    public static MajorConditionSet RightSet => _conditionSet?
        .ElementAtOrDefault(Service.Config.GetValue(PluginConfigInt.ActionSequencerIndex));

    public static string[] ConditionSetsName => _conditionSet?.Select(s => s.Name).ToArray() ?? Array.Empty<string>();

    public static void UpdateActionSequencerAction()
    {
        if (_conditionSet == null) return;
        var customRotation = RotationUpdater.RightNowRotation;
        if (customRotation == null) return;

        var allActions = RotationUpdater.RightRotationActions;

        var set = RightSet;
        if (set == null) return;

        DataCenter.DisabledActionSequencer = new HashSet<uint>(set.DiabledConditions.Where(pair => pair.Value.IsTrue(customRotation))
             .Select(pair => pair.Key));

        bool find = false;
        foreach (var conditionPair in set.Conditions)
        {
            var nextAct = allActions.FirstOrDefault(a => a.ID == conditionPair.Key);
            if (nextAct == null) continue;

            if (!conditionPair.Value.IsTrue(customRotation)) continue;

            DataCenter.ActionSequencerAction = nextAct;
            find = true;
            break;
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

        _conditionSet = MajorConditionSet.Read(_actionSequencerFolder);
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
        foreach (var set in _conditionSet)
        {
            set.Save(_actionSequencerFolder);
        }
    }

    public static void LoadFiles()
    {
        _conditionSet = MajorConditionSet.Read(_actionSequencerFolder);
    }

    private static void AddNew()
    {
        const string conditionName = "Unnamed";
        if (!_conditionSet.Any(c => c.Name == conditionName))
        {
            _conditionSet = _conditionSet.Union(new[] { new MajorConditionSet(conditionName) });
        }
    }

    private static void Delete(string name)
    {
        _conditionSet = _conditionSet.Where(c => c.Name != name);
        File.Delete(_actionSequencerFolder + $"\\{name}.json");
    }

    public static void DrawHeader(float width)
    {
        var set = RightSet;
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
