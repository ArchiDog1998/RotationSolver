using ImGuiNET;
using Lumina.Data.Parsing;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
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
                    ImGuiHelper.Spacing();
                    RSCommands.DisplayCommandHelp(OtherCommandType.Rotations, Name + " " +comboIndex.ToString());

                    ImGui.SameLine();
                    ImGuiHelper.Spacing();
                    RSCommands.DisplayCommandHelp(OtherCommandType.Rotations, Name + " " + Items[comboIndex]);

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
            ImGuiHelper.Spacing();
            RSCommands.DisplayCommandHelp(OtherCommandType.Rotations, Name);
        }
    }

    public override string GetDisplayValue(ClassJobID job, string rotationName)
    {
        var indexStr = base.GetDisplayValue(job, rotationName);
        if (!int.TryParse(indexStr, out var index)) return Items[0];
        return Items[index];
    }

    public override bool DoCommand(IRotationConfigSet set, string str)
    {
        if (!base.DoCommand(set, str)) return false;

        string numStr = str.Substring(Name.Length).Trim();
        var length = Items.Length;

        int nextId = (set.GetCombo(Name) + 1) % length;
        if (int.TryParse(numStr, out int num))
        {
            nextId = num % length;
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                if (Items[i] == str)
                {
                    nextId = i;
                }
            }
        }

        set.SetValue(Name, nextId.ToString());
        return true;
    }
}
