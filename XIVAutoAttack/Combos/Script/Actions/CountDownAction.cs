using ImGuiNET;
using System.Linq;
using System.Reflection;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Localization;
using XIVAutoAttack.Windows.ComboConfigWindow;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class CountDownAction
    {
        private BaseAction _action { get; set; }

        public ActionID ID { get; set; } = ActionID.None;

        MethodInfo _method;
        public string MethodName { get; set; } = string.Empty;

        public bool MustUse { get; set; }

        public bool Empty { get; set; }

        public float Time { get; set; }

        public CountDownAction()
        {

        }

        public CountDownAction(BaseAction act)
        {
            _action = act;
            ID = (ActionID)act.ID;
        }

        public CountDownAction(MethodInfo method)
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

            if (_action != null)
            {
                ImGui.Image(_action.GetTexture().ImGuiHandle, size);

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                var time = Time;
                ImGui.SetNextItemWidth(50);
                if (ImGui.DragFloat($"s##CountDown{GetHashCode()}", ref time))
                {
                    Time = time;
                }

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

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
                ComboConfigWindow.Spacing();

                ImGui.Text(_action.Name);
            }
            else if (_method != null)
            {
                ImGui.Image(IconSet.GetTexture(1).ImGuiHandle, size);

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                var time = Time;
                ImGui.SetNextItemWidth(50);
                if (ImGui.DragFloat($"s##CountDown{GetHashCode()}", ref time))
                {
                    Time = time;
                }

                ImGui.SameLine();
                ComboConfigWindow.Spacing();

                ImGui.Text(_method.GetMemberName());

                var desc = _method.GetMemberDescription();
                if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(desc))
                {
                    ImGui.SetTooltip(desc);
                }
            }
        }

        public bool ShouldUse(IScriptCombo owner, out IAction act)
        {
            act = null;
            if (ID != ActionID.None && _action == null)
            {
                _action = owner.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == ID);
            }
            if (!string.IsNullOrEmpty(MethodName) && (_method == null || _method.Name != MethodName))
            {
                _method = owner.GetType().GetMethodInfo(MethodName);
            }

            return _action?.ShouldUse(out act, MustUse, Empty) ?? false;
        }
    }
}
