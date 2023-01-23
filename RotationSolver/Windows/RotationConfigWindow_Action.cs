using ImGuiNET;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    internal static IAction ActiveAction { get; set; }

    private void DrawActionTab()
    {

        ImGui.Columns(2);

        DrawActionList();

        ImGui.NextColumn();

        DrawTimelineCondition();

        ImGui.Columns(1);
    }

    private void DrawActionList()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_ActionItem_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
        {
            foreach (var pair in IconReplacer.RightRotationBaseActions.GroupBy(a => a.CateName).OrderBy(g => g.Key))
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

    private void DrawTimelineCondition()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.Timeline_TimelineDescription);

        var rotation = IconReplacer.RightNowRotation;
        if (rotation == null) return;

        TimeLineUpdater.DrawHeader();

        if (ActiveAction == null) return;
        if (!IconReplacer.RightRotationBaseActions.Any(a => a.ID == ActiveAction.ID)) return;

        var set = TimeLineUpdater.RightSet;
        if (set == null) return;

        if (!set.Conditions.TryGetValue((ActionID)ActiveAction.ID, out var conditionSet))
        {
            conditionSet = set.Conditions[(ActionID)ActiveAction.ID] = new Timeline.ConditionSet();
        }

        conditionSet?.Draw(rotation);
    }
}
