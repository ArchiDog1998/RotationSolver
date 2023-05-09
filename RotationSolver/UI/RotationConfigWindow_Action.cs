using RotationSolver.ActionSequencer;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    internal static IAction ActiveAction { get; set; }

    private static void DrawActionTab()
    {
        ImGui.Columns(2);

        DrawActionList();

        ImGui.NextColumn();

        DrawActionSequencerCondition();

        ImGui.Columns(1);
    }

    private static void DrawActionList()
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
                        foreach (var item in pair.OrderBy(t => t.ID))
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

    private static void DrawActionSequencerCondition()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ActionSequencer_ActionSequencerDescription);

        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        ActionSequencerUpdater.DrawHeader();

        if (ActiveAction == null) return;
        if (!RotationUpdater.RightRotationActions.Any(a => a.ID == ActiveAction.ID)) return;

        var set = ActionSequencerUpdater.RightSet;
        if (set == null) return;

        ConditionSet conditionSet;
        if (ActionSequencerUpdater.IsDisableCondition)
        {
            if (!set.DiableConditions.TryGetValue(ActiveAction.ID, out conditionSet))
            {
                conditionSet = set.DiableConditions[ActiveAction.ID] = new ConditionSet();
            }
        }
        else
        {
            if (!set.Conditions.TryGetValue(ActiveAction.ID, out conditionSet))
            {
                conditionSet = set.Conditions[ActiveAction.ID] = new ConditionSet();
            }
        }


        conditionSet?.Draw(rotation, ActiveAction.IsActionSequencer);
    }
}
