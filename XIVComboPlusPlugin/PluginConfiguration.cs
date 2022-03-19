using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboPlus.Attributes;
using XIVComboPlus.Combos;

namespace XIVComboPlus;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 5;


    [JsonProperty("EnabledActionsV5")]
    public HashSet<string> EnabledActions { get; } = new HashSet<string>();

    [JsonProperty("Debug")]
    public bool EnableSecretCombos { get; set; }

    public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[4] { 15989u, 16013u, 16007u, 16008u };


    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }

    public bool IsEnabled(string preset)
    {
        return EnabledActions.Contains(preset);
    }
}
