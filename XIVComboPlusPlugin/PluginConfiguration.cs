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

    public int MultiCount { get; set; } = 2;

    public bool IsTargetBoss { get; set; } = true;
    public SortedSet<string> EnabledActions { get; private set; } = new SortedSet<string>();

    public bool EnableSecretCombos { get; set; } = false;


    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
