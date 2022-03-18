using System;

namespace XIVComboExpandedPlugin.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class ConflictingCombosAttribute : Attribute
{
	public CustomComboPreset[] ConflictingPresets { get; }

	internal ConflictingCombosAttribute(params CustomComboPreset[] conflictingPresets)
	{
		ConflictingPresets = conflictingPresets;
	}
}
