using ImGuiNET;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
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
    public DescType DescType { get; private set; } = DescType.None;
	public IEnumerable<ActionID> Actions { get; private set; } = Enumerable.Empty<ActionID>();

	public RotationDescAttribute(DescType descType)
	{
        DescType = descType;
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

	public void Display(ICustomRotation rotation)
	{
		ImGui.Columns(2);
		ImGui.SetColumnWidth(0, 100);
		ImGui.Text(DescType.ToName());

		ImGui.NextColumn();

		ImGui.TextWrapped(Description);
		var acts = rotation.AllActions;
		foreach (var item in Actions)
		{
			var a = acts.FirstOrDefault(a => a.ID == (uint)item);
			if (a == null) continue;
			ImGui.Image(a.GetTexture().ImGuiHandle, new System.Numerics.Vector2(24, 24));
			if (ImGui.IsItemHovered())
			{
				ImGui.SetTooltip(a.Name);
			}
			ImGui.SameLine();
		}


        ImGui.Columns();
	}

	public static RotationDescAttribute Merge(IEnumerable<RotationDescAttribute> rotationDescAttributes)
	{
		var result = new RotationDescAttribute();
		foreach (var attr in rotationDescAttributes)
		{
			if(!string.IsNullOrEmpty(attr.Description))
			{
                result.Description = attr.Description;
			}
			if(attr.DescType != DescType.None)
			{
                result.DescType = attr.DescType;
			}
            result.Actions = result.Actions.Union(attr.Actions);
        }

		if (result.DescType == DescType.None) return null;
		return result;
	}
}
