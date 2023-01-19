using ImGuiNET;
using RotationSolver.Localization;

namespace RotationSolver.Configuration.RotationConfig;

internal class RotationConfigBoolean : RotationConfigBase
{
    public RotationConfigBoolean(string name, bool value, string displayName) : base(name, value.ToString(), displayName)
    {
    }

    public override void Draw(RotationConfigSet set, bool canAddButton)
    {
        bool val = set.GetBool(Name);
        if (ImGui.Checkbox($"{DisplayName}##{GetHashCode()}_{DisplayName}", ref val))
        {
            set.SetValue(Name, val.ToString());
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_KeyName + ": " + Name);
        }

        //显示可以设置的案件
        if (canAddButton)
        {
            ImGui.SameLine();
            //Spacing();
            //CommandHelp(boolean.name);
        }
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;
        set.SetValue(Name, (!set.GetBool(Name)).ToString());
        return true;
    }
}
