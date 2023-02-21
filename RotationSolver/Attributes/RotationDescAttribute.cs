using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
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

	public bool Display(ICustomRotation rotation)
	{
		var acts = rotation.AllActions;

		var allActions = Actions.Select(i => acts.FirstOrDefault(a => a.ID == (uint)i))
			.Where(i => i != null);

		bool hasDesc = !string.IsNullOrEmpty(Description);

		if (!hasDesc && !allActions.Any()) return false;

        ImGui.Columns(2, this.GetHashCode().ToString(), false);
		ImGui.SetColumnWidth(0, 150);
        ImGui.Text(DescType.ToName());

		ImGui.NextColumn();

		if (hasDesc)
		{
			ImGui.Text(Description);
			ImGui.NewLine();
		}

		bool notStart = false;
		foreach (var item in allActions)
		{
            if (item == null) continue;

            if (notStart)
			{
				ImGui.SameLine();
                ImGuiHelper.Spacing();
            }

            ImGui.Image(item.GetTexture().ImGuiHandle, new System.Numerics.Vector2(24, 24));
			notStart = true;
        }

        ImGui.Columns(1);
		return true;
	}
	public static IEnumerable<RotationDescAttribute[]> Merge(IEnumerable<RotationDescAttribute> rotationDescAttributes)
		=> from r in rotationDescAttributes
		   where r is RotationDescAttribute
           group r by r.DescType into gr
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
