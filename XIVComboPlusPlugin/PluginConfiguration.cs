using System;
using System.Linq;
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


    //[JsonProperty("EnabledActionsV5")]
    public SortedSet<string> EnabledActions { get; private set; } = new SortedSet<string>();

    // [JsonProperty("Debug")]
    public bool EnableSecretCombos { get; set; } = false;

    //public uint[] DancerDanceCompatActionIDs { get; set; } = new uint[4] { 15989u, 16013u, 16007u, 16008u };


    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
