using ImGuiNET;
using RotationSolver.Basic.Actions;
using RotationSolver.Localization;
using RotationSolver.UI;
using RotationSolver.Updaters;
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
            foreach (var pair in RotationUpdater.RightNowRotation.AllActions.GroupBy(a =>
            {
                if(a is IBaseAction act)
                {
                    string result;

                    if (act.IsFriendly)
                    {
                        result = LocalizationManager.RightLang.Action_Friendly;
                        if (act.IsEot)
                        {
                            result += "Hot";
                        }
                    }
                    else
                    {
                        result = LocalizationManager.RightLang.Action_Attack;

                        if (act.IsEot)
                        {
                            result += "Dot";
                        }
                    }
                    result += "-" + (act.IsRealGCD ? "GCD" : LocalizationManager.RightLang.Timeline_Ability);
                    return result;
                }
                else if(a is IBaseItem)
                {
                    return "Item";
                }
                return string.Empty;

            }).OrderBy(g => g.Key))
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

        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        TimeLineUpdater.DrawHeader();

        if (ActiveAction == null) return;
        if (!RotationUpdater.RightRotationBaseActions.Any(a => a.ID == ActiveAction.ID)) return;

        var set = TimeLineUpdater.RightSet;
        if (set == null) return;

        if (!set.Conditions.TryGetValue(ActiveAction.ID, out var conditionSet))
        {
            conditionSet = set.Conditions[ActiveAction.ID] = new Timeline.ConditionSet();
        }

        conditionSet?.Draw(rotation);
    }
}
