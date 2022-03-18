using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboExpandedPlugin.Attributes;

namespace XIVComboExpandedPlugin;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
	public int Version { get; set; } = 5;


	[JsonProperty("EnabledActionsV5")]
	public HashSet<CustomComboPreset> EnabledActions { get; set; } = new HashSet<CustomComboPreset>();


	[JsonProperty("EnabledActionsV4")]
	public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new HashSet<CustomComboPreset>();


	[JsonProperty("Debug")]
	public bool EnableSecretCombos { get; set; }

	public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[4] { 15989u, 16013u, 16007u, 16008u };


	public void Save()
	{
		Service.Interface.SavePluginConfig((IPluginConfiguration)(object)this);
	}

	public bool IsEnabled(CustomComboPreset preset)
	{
		if (EnabledActions.Contains(preset))
		{
			if (!EnableSecretCombos)
			{
				return !IsSecret(preset);
			}
			return true;
		}
		return false;
	}

	public bool IsSecret(CustomComboPreset preset)
	{
		return EnumExtensions.GetAttribute<SecretCustomComboAttribute>((Enum)preset) != null;
	}

	public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
	{
		return EnumExtensions.GetAttribute<ConflictingCombosAttribute>((Enum)preset)?.ConflictingPresets ?? Array.Empty<CustomComboPreset>();
	}

	public CustomComboPreset? GetParent(CustomComboPreset preset)
	{
		return EnumExtensions.GetAttribute<ParentComboAttribute>((Enum)preset)?.ParentPreset;
	}
}
