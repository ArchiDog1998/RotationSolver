namespace RotationSolver.Timeline;

public class ConditionHelper
{
    public static bool CheckBaseAction(ICustomRotation rotation, ActionID id, ref BaseAction action)
    {
        if (id != ActionID.None && (action == null || (ActionID)action.ID != id))
        {
            action = rotation.AllBaseActions.OfType<BaseAction>().FirstOrDefault(a => (ActionID)a.ID == id);
        }
        if (action == null || Service.Player == null) return false;
        return true;
    }

    public static void CheckMemberInfo<T>(ICustomRotation rotation, string name, ref T value) where T : MemberInfo
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

    public static void DrawIntEnum<T>(string name, ref T value, Func<T, string> function) where T : struct, Enum
    {
        var type = (int)(object)value;
        var names = Enum.GetValues<T>().Select(function).ToArray();
        //ImGui.SetNextItemWidth(100);
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        if (ImGui.Combo(name, ref type, names, names.Length))
        {
            value = (T)(object)type;
        }
    }

    public static bool DrawDragFloat(string name, ref float value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragFloat(name, ref value);
    }

    public static bool DrawDragInt(string name, ref int value)
    {
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        return ImGui.DragInt(name, ref value);
    }

    public static bool DrawCheckBox(string name, ref int value, string desc = "")
    {
        ImGui.SameLine();

        var @bool = value != 0;

        var result = false;
        if (ImGui.Checkbox(name, ref @bool))
        {
            value = @bool ? 1 : 0;
            result = true;
        }
        if (!string.IsNullOrEmpty(desc) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(desc);
        }

        return result;
    }
}
