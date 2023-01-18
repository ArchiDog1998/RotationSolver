using ImGuiNET;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Configuration.RotationConfig;

internal class RotationConfigCombo : RotationConfigBase
{
    public string[] Items { get; set; }
    public RotationConfigCombo(string name, int value, string displayName, string[] items) : base(name, value.ToString(), displayName)
    {
        Items = items;
    }

    public override void Draw(RotationConfigSet set, bool canAddButton)
    {
        var val = set.GetCombo(Name);
        if (ImGui.BeginCombo($"{DisplayName}##{GetHashCode()}_{Name}", Items[val]))
        {
            for (int comboIndex = 0; comboIndex < Items.Length; comboIndex++)
            {
                if (ImGui.Selectable(Items[comboIndex]))
                {
                    set.SetValue(Name, comboIndex.ToString());
                    Service.Configuration.Save();
                }
                if (canAddButton)
                {
                    ImGui.SameLine();
                    //Spacing();
                    //CommandHelp(comboItem.name + comboIndex.ToString());
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_AttackItem_KeyName + ": " + Name);
        }

        //显示可以设置的按键
        if (canAddButton)
        {
            ImGui.SameLine();
            //Spacing();
            //CommandHelp(comboItem.name);
        }
    }
}
