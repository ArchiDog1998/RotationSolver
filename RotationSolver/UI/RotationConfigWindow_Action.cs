using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    internal static IAction ActiveAction { get; set; }

    private void DrawActionTab()
    {
        ImGui.Columns(2);

        DrawActionList();

        ImGui.NextColumn();

        DrawActionSequencerCondition();

        ImGui.Columns(1);
    }

    private void DrawActionList()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_ActionItem_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
        {
            if (RotationUpdater.RightNowRotation != null)
                foreach (var pair in RotationUpdater.AllGroupedActions)
                {
                    if (ImGui.CollapsingHeader(pair.Key))
                    {
                        foreach (var item in pair)
                        {
                            item.Display(ActiveAction == item);
                            ImGui.Separator();
                        }
                    }
                }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }

    private void DrawActionSequencerCondition()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ActionSequencer_ActionSequencerDescription);

        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        ActionSequencerUpdater.DrawHeader();

        if (ActiveAction == null) return;
        if (!RotationUpdater.RightRotationActions.Any(a => a.ID == ActiveAction.ID)) return;

        var set = ActionSequencerUpdater.RightSet;
        if (set == null) return;

        if (!set.Conditions.TryGetValue(ActiveAction.ID, out var conditionSet))
        {
            conditionSet = set.Conditions[ActiveAction.ID] = new Timeline.ConditionSet();
        }

        conditionSet?.Draw(rotation);
    }
}
