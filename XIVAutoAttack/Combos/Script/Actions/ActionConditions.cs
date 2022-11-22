using ImGuiNET;
using Lumina.Data.Parsing;
using Lumina.Data.Parsing.Layer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Script.Conditions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Windows;
using XIVAutoAttack.Windows.ComboConfigWindow;

namespace XIVAutoAttack.Combos.Script.Actions;

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
        if (ID != ActionID.None && (_action == null || (ActionID)_action.ID != ID))
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
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

            ImGui.Image(_action.GetTexture().ImGuiHandle,
                new System.Numerics.Vector2(30, 30));

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            var mustUse = MustUse;
            if (ImGui.Checkbox($"必须##必须{GetHashCode()}", ref mustUse))
            {
                MustUse = mustUse;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("跳过AOE判断，跳过提供的Buff判断。");
            }

            ImGui.SameLine();

            var empty = Empty;
            if (ImGui.Checkbox($"用光##用光{GetHashCode()}", ref empty))
            {
                Empty = empty;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("用完所有层数或者跳过连击判断。");
            }

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            if (ImGui.Selectable(_action.Name, this == XIVAutoAttackPlugin._scriptComboWindow.ActiveAction))
            {
                XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
            }
        }
        else if(_method != null)
        {
            if (ImGui.Selectable(_method.GetMemberName()))
            {
                XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
            }

            var desc = _method.GetMemberDescription();
            if(ImGui.IsItemHovered() && !string.IsNullOrEmpty(desc))
            {
                ImGui.SetTooltip(desc);
            }
        }
        else
        {
            if (ImGui.Selectable("返回条件"))
            {
                XIVAutoAttackPlugin._scriptComboWindow.ActiveAction = this;
            }
        }
    }

    string search = string.Empty;
    public void Draw(IScriptCombo combo)
    {
        if (ID != ActionID.None && _action == null)
        {
            _action = combo.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
        }
        if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
        {
            _method = combo.GetType().GetMethodInfo(MethodName);
        }

        ImGui.Text("描述:");

        var desc = Description;

        ImGui.SetNextItemWidth(ImGui.GetColumnWidth(2) - 20);

        if (ImGui.InputTextMultiline($"##{_action?.Name}的描述", ref desc, 1024, new System.Numerics.Vector2(0, 100)))
        {
            Description = desc;
        }

        if (IsAbility)
        {
            ImGui.SetNextItemWidth(100);
            int c = AbilityCount;
            if(ImGui.DragInt("还剩第几个能力技", ref c))
            {
                AbilityCount = c;
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("当还剩下能插几个能力技的时候才能使用这个技能，设为0的时候忽略这个条件。");
            }

            if (IsEmergency)
            {
                if (_actions.Count != IDs.Count)
                {
                    _actions = combo.AllActions.Where(a => IDs.Contains((ActionID)a.ID)).ToList();
                }


                var adj = IsAdjust;
                if(ImGui.Checkbox("是否为调整后", ref adj))
                {
                    IsAdjust = adj;
                }
                ImGui.SameLine();

                ImGui.Text("下一个GCD是：");

                ImGui.SameLine();

                ScriptComboWindow.AddPopup("Emergency" + GetHashCode().ToString(), string.Empty, null, ref search, combo.AllActions, item =>
                {
                    _actions.Add(item);
                    IDs.Add((ActionID)item.ID);
                });

                var relay = _actions;
                if(relay.Count > 0 && ImGui.BeginChild($"##{_action?.Name}的下一个GCD是", new System.Numerics.Vector2(0, 100), true))
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
            _action = owner.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
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
                    _actions = owner.AllActions.Where(a => IDs.Contains((ActionID)a.ID)).ToList();
                }
                if (nextGCD != null && _actions.Count > 0)
                {
                    if (!nextGCD.IsAnySameAction(IsAdjust, _actions.ToArray())) return false;
                }
            }

            return _action.ShouldUse(out act, MustUse, Empty) && otherCheck;
        }
        else if(_method != null)
        {
            var param = new object[] { null };
            if((bool)_method.Invoke(owner, param) && otherCheck)
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
