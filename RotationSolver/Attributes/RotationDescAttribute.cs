using Dalamud.Interface.Colors;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal class RotationDescAttribute : Attribute
{
	public string Description { get; private set; } = string.Empty;
    public DescType Type { get; private set; } = DescType.None;
	public IEnumerable<ActionID> Actions { get; private set; } = Enumerable.Empty<ActionID>();
	private uint IconID => Type switch
	{
		DescType.BurstActions => 62583,

		DescType.HealAreaGCD => 62582,
		DescType.HealAreaAbility => 62582,
		DescType.HealSingleGCD => 62582,
		DescType.HealSingleAbility => 62582,

		DescType.DefenseAreaGCD => 62581,
		DescType.DefenceAreaAbility => 62581,
		DescType.DefenseSingleGCD => 62581,
		DescType.DefenceSingleAbility => 62581,

		DescType.MoveForwardGCD => 104,
		DescType.MoveForwardAbility => 104,
		DescType.MoveBackAbility => 104,

        _ => 62144,
	};

	private bool IsOnCommand
	{
		get
		{
			var command = RSCommands.SpecialType;
			switch(Type)
			{
				case DescType.BurstActions:
                    return command == SpecialCommandType.Burst;

				case DescType.HealAreaAbility:
				case DescType.HealAreaGCD:
                    return command == SpecialCommandType.HealArea;

                case DescType.HealSingleAbility:
				case DescType.HealSingleGCD:
					return command == SpecialCommandType.HealSingle;

				case DescType.DefenseAreaGCD:
				case DescType.DefenceAreaAbility:
					return command == SpecialCommandType.DefenseArea;

				case DescType.DefenseSingleGCD:
				case DescType.DefenceSingleAbility:
					return command == SpecialCommandType.DefenseSingle;

                case DescType.MoveForwardGCD:
                case DescType.MoveForwardAbility:
                    return command == SpecialCommandType.MoveForward;

                case DescType.MoveBackAbility:
                    return command == SpecialCommandType.MoveBack;

                default: 
					return false;
			}
		}
	}

    public RotationDescAttribute(DescType descType)
	{
        Type = descType;
    }
	public RotationDescAttribute(params ActionID[] actions)
		:this(string.Empty, actions)
	{
    }

	public RotationDescAttribute(string desc, params ActionID[] actions)
	{
        Description = desc;
        Actions = actions;
    }

    private RotationDescAttribute()
	{

	}

	static readonly System.Numerics.Vector2 PIC_SIZE = new System.Numerics.Vector2(24, 24);
	const float ATTR_INDENT = 170;

    public void Display(ICustomRotation rotation)
	{
		var acts = rotation.AllActions;

		var allActions = Actions.Select(i => acts.FirstOrDefault(a => a.ID == (uint)i))
			.Where(i => i != null);

		bool hasDesc = !string.IsNullOrEmpty(Description);

		if (!hasDesc && !allActions.Any()) return;
        ImGui.Separator();

        ImGui.Image(IconSet.GetTexture(IconID).ImGuiHandle, PIC_SIZE);
		ImGui.SameLine();

		var isOnCommand = IsOnCommand;
		if (isOnCommand) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
        ImGui.Text(" " + Type.ToName());
		if (isOnCommand) ImGui.PopStyleColor();

        ImGui.SameLine();
        ImGui.Indent(ATTR_INDENT);

        if (hasDesc)
		{
			ImGui.Text(Description);
		}

		bool notStart = false;
		foreach (var item in allActions)
		{
            if (item == null) continue;

            if (notStart)
			{
				ImGui.SameLine();
				ImGui.Text(" ");
				ImGui.SameLine();
            }

            ImGui.Image(item.GetTexture().ImGuiHandle, PIC_SIZE);
			notStart = true;
        }
        ImGui.Unindent(ATTR_INDENT);
	}
	public static IEnumerable<RotationDescAttribute[]> Merge(IEnumerable<RotationDescAttribute> rotationDescAttributes)
		=> from r in rotationDescAttributes
		   where r is RotationDescAttribute
           group r by r.Type into gr
           orderby gr.Key
           select gr.ToArray();

	public static RotationDescAttribute MergeToOne(IEnumerable<RotationDescAttribute> rotationDescAttributes)
	{
		var result = new RotationDescAttribute();
		foreach (var attr in rotationDescAttributes)
		{
			if(attr == null) continue;
			if(!string.IsNullOrEmpty(attr.Description))
			{
                result.Description = attr.Description;
			}
			if(attr.Type != DescType.None)
			{
                result.Type = attr.Type;
			}
            result.Actions = result.Actions.Union(attr.Actions);
        }

		if (result.Type == DescType.None) return null;
		return result;
	}
}
