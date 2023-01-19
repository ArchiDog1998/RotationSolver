using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Windows;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.Script.Conditions;

namespace RotationSolver.Rotations.Script.Actions;

/// <summary>
/// 最右边那栏用来渲染的
/// </summary>
internal class ActionConditions : IDraw
{
    public bool IsAbility { get; set; } = false;
    public bool IsEmergency { get; set; } = false;

    public int AbilityCount { get; set; }
    private BaseAction _action { get; set; }
    public ActionID ID { get; set; } = ActionID.None;

    MethodInfo _method;
    public string MethodName { get; set; } = string.Empty;
    private List<BaseAction> _actions { get; set; } = new List<BaseAction>();
    public List<ActionID> IDs { get; set; } = new List<ActionID>();

    public bool IsAdjust { get; set; }

    public ConditionSet Set { get; set; } = new ConditionSet();

    public bool MustUse { get; set; }
    public bool Empty { get; set; }

    public string Description { get; set; } = string.Empty;

    public ActionConditions()
    {

    }

    public ActionConditions(BaseAction act)
    {
        _action = act;
        ID = (ActionID)act.ID;
    }

    public ActionConditions(MethodInfo method)
    {
        _method = method;
        MethodName = method.Name;
    }

    public void DrawHeader(IScriptCombo combo)
    {
        float iconSize = 30;
        var size = new System.Numerics.Vector2(iconSize, iconSize);

        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = combo.GetType().GetMethodInfo(MethodName);
        }

        var tag = ShouldUse(combo, 0, null, out _);
        ScriptComboWindow.DrawCondition(tag);
        ImGui.SameLine();


        if (_action != null)
        {
            ImGui.Image(_action.GetTexture().ImGuiHandle, size);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            var mustUse = MustUse;
            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_MustUse}##MustUse{GetHashCode()}", ref mustUse))
            {
                MustUse = mustUse;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_MustUseDesc);
            }

            ImGui.SameLine();

            var empty = Empty;
            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Scriptwindow_Empty}##Empty{GetHashCode()}", ref empty))
            {
                Empty = empty;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_EmptyDesc);
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            //if (ImGui.Selectable(_action.Name, this == RotationSolverPlugin._scriptComboWindow.ActiveAction))
            //{
            //    RotationSolverPlugin._scriptComboWindow.ActiveAction = this;
            //}
        }
        else if (_method != null)
        {
            ImGui.Image(IconSet.GetTexture(1).ImGuiHandle, size);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            //if (ImGui.Selectable(_method.GetMemberName()))
            //{
            //    RotationSolverPlugin._scriptComboWindow.ActiveAction = this;
            //}

            var desc = _method.GetMemberDescription();
            if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(desc))
            {
                ImGui.SetTooltip(desc);
            }
        }
        else
        {
            //if (ImGui.Selectable(LocalizationManager.RightLang.Scriptwindow_Return))
            //{
            //    RotationSolverPlugin._scriptComboWindow.ActiveAction = this;
            //}
        }
    }

    string search = string.Empty;
    public void Draw(IScriptCombo combo)
    {
        if (ID != ActionID.None && _action == null)
        {
            _action = combo.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = combo.GetType().GetMethodInfo(MethodName);
        }

        ImGui.Text($"{LocalizationManager.RightLang.Scriptwindow_ActionConditionsDescription}: ");

        var desc = Description;

        ImGui.SetNextItemWidth(ImGui.GetColumnWidth(2) - 20);

        if (ImGui.InputTextMultiline($"##DescriptionOf{_action?.Name}", ref desc, 1024, new System.Numerics.Vector2(0, 100)))
        {
            Description = desc;
        }

        if (IsAbility)
        {
            ImGui.SetNextItemWidth(100);
            int c = AbilityCount;
            if (ImGui.DragInt(LocalizationManager.RightLang.Scriptwindow_AbilityRemain, ref c))
            {
                AbilityCount = c;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_AbilityRemainDesc);
            }

            if (IsEmergency)
            {
                if (_actions.Count != IDs.Count)
                {
                    _actions = combo.AllActions.Where(a => IDs.Contains((ActionID)a.ID)).OfType<BaseAction>().ToList();
                }


                var adj = IsAdjust;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Scriptwindow_AdjustID, ref adj))
                {
                    IsAdjust = adj;
                }
                ImGui.SameLine();

                ImGui.Text($"{LocalizationManager.RightLang.Scriptwindow_NextGCD}: ");

                ImGui.SameLine();

                ScriptComboWindow.AddPopup("Emergency" + GetHashCode().ToString(), string.Empty, null, ref search, combo.AllActions.OfType<BaseAction>(), item =>
                {
                    _actions.Add(item);
                    IDs.Add((ActionID)item.ID);
                });

                var relay = _actions;
                if (relay.Count > 0 && ImGui.BeginChild($"##{_action?.Name}NextGCD", new System.Numerics.Vector2(0, 100), true))
                {
                    if (ScriptComboWindow.DrawEditorList(relay, i =>
                    {
                        ImGui.Image(i.GetTexture().ImGuiHandle, new System.Numerics.Vector2(30, 30));

                        ImGui.SameLine();

                        ImGui.Text(i.Name);
                    }))
                    {
                        _actions = relay;
                        IDs = _actions.Select(i => (ActionID)i.ID).ToList();
                    }

                    ImGui.EndChild();
                }
            }
        }

        Set.Draw(combo);
    }

    public bool? ShouldUse(IScriptCombo owner, byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (ID != ActionID.None && _action == null)
        {
            _action = owner.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = owner.GetType().GetMethodInfo(MethodName);
        }

        act = _action;

        var otherCheck = Set.IsTrue(owner);

        if (_action != null)
        {
            if (AbilityCount != 0 && abilityRemain != 0)
            {
                if (abilityRemain != AbilityCount) return false;
            }

            if (IsEmergency)
            {
                if (_actions.Count != IDs.Count)
                {
                    _actions = owner.AllActions.Where(a => IDs.Contains((ActionID)a.ID)).OfType<BaseAction>().ToList();
                }
                if (nextGCD != null && _actions.Count > 0)
                {
                    if (!nextGCD.IsAnySameAction(IsAdjust, _actions.ToArray())) return false;
                }
            }

            return _action.ShouldUse(out act, MustUse, Empty) && otherCheck;
        }
        else if (_method != null)
        {
            var param = new object[] { null };
            if ((bool)_method.Invoke(owner, param) && otherCheck)
            {
                act = (IAction)param[0];
                return true;
            }
            return false;
        }
        else
        {
            return otherCheck ? null : false;
        }
    }
}
