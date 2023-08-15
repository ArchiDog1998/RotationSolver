using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class ConditionSet : ICondition
{
    public bool IsTrue(ICustomRotation combo) => Conditions.Count != 0 && (IsAnd ? Conditions.All(c => c.IsTrue(combo))
                                : Conditions.Any(c => c.IsTrue(combo)));
    public List<ICondition> Conditions { get; set; } = new List<ICondition>();
    public bool IsAnd { get; set; }

    public void Draw(ICustomRotation combo)
    {
        var start = ImGui.GetCursorPos();
        ImGui.BeginGroup();

        AddButton();

        ImGui.SameLine();

        ImGuiHelper.DrawCondition(IsTrue(combo));

        ImGui.SameLine();
        ImGui.SetNextItemWidth(65);
        int isAnd = IsAnd ? 1 : 0;
        if (ImGui.Combo($"##Rule" + GetHashCode().ToString(), ref isAnd, new string[]
        {
                "OR", "AND",
        }, 2))
        {
            IsAnd = isAnd != 0;
        }

        ImGui.Spacing();

        var relay = Conditions;
        if (ImGuiHelper.DrawEditorList(relay, i => i.Draw(combo)))
        {
            Conditions = relay;
        }

        ImGui.EndGroup();

        //ControlWindow.HighLight(ImGui.GetWindowPos() + start, ImGui.GetItemRectSize(), 0.5f);
    }

    private void AddButton()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddButton" + GetHashCode().ToString()))
        {
            ImGui.OpenPopup("Popup" + GetHashCode().ToString());
        }

        if (ImGui.BeginPopup("Popup" + GetHashCode().ToString()))
        {
            AddOneCondition<ConditionSet>(LocalizationManager.RightLang.ActionSequencer_ConditionSet);
            AddOneCondition<ActionCondition>(LocalizationManager.RightLang.ActionSequencer_ActionCondition);
            AddOneCondition<TargetCondition>(LocalizationManager.RightLang.ActionSequencer_TargetCondition);
            AddOneCondition<RotationCondition>(LocalizationManager.RightLang.ActionSequencer_RotationCondition);

            ImGui.EndPopup();
        }
    }

    private void AddOneCondition<T>(string name) where T : ICondition
    {
        if (ImGui.Selectable(name))
        {
            Conditions.Add(Activator.CreateInstance<T>());
            ImGui.CloseCurrentPopup();
        }
    }
}
