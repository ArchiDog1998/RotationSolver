using ImGuiNET;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Linq;
using System.Reflection;

namespace RotationSolver.Helpers
{
    internal class ConditionHelper
    {
        internal static bool CheckBaseAction(ICustomRotation rotation, ActionID id, ref BaseAction action)
        {
            if (id != ActionID.None && (action == null || (ActionID)action.ID != id))
            {
                action = rotation.AllActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == id);
            }
            if (action == null || Service.ClientState.LocalPlayer == null) return false;
            return true;
        }

        internal static void CheckMemberInfo<T>(ICustomRotation rotation, string name, ref T value) where T : MemberInfo
        {
            if (!string.IsNullOrEmpty(name) && (value == null || value.Name != name))
            {
                if (typeof(T).IsAssignableFrom(typeof(PropertyInfo)))
                {
                    value = (T)(MemberInfo)rotation.GetType().GetPropertyInfo(name);
                }
                else if (typeof(T).IsAssignableFrom(typeof(MethodInfo)))
                {
                    value = (T)(MemberInfo)rotation.GetType().GetMethodInfo(name);
                }
            }
        }

        internal static void DrawIntEnum<T>(string name, ref T value, Func<T, string> func) where T : struct, Enum
        {
            var type = (int)(object)value;
            var names = Enum.GetValues<T>().Select(func).ToArray();
            ImGui.SetNextItemWidth(100);

            if (ImGui.Combo(name, ref type, names, names.Length))
            {
                value = (T)(object)type;
            }
        }

        internal static bool DrawDragFloat(string name, ref float value)
        {
            ImGui.SameLine();
            ImGui.SetNextItemWidth(50);
            return ImGui.DragFloat(name, ref value);
        }

        internal static bool DrawDragInt(string name, ref int value)
        {
            ImGui.SameLine();
            ImGui.SetNextItemWidth(50);
            return ImGui.DragInt(name, ref value);
        }

        internal static bool DrawCheckBox(string name, ref int value, string desc = "")
        {
            ImGui.SameLine();

            var @bool = value != 0;

            var result = false;
            if (ImGui.Checkbox(name, ref @bool))
            {
                value = @bool ? 1 : 0;
                result = true;
            }
            if(!string.IsNullOrEmpty(desc) && ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(desc);
            }

            return result;
        }
    }
}
