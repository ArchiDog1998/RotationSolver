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

    public int MultiCount { get; set; } = 3;

    public bool IsTargetBoss { get; set; } = true;
    public SortedSet<string> EnabledActions { get; private set; } = new SortedSet<string>();

    public float HealthDifference { get; set; } = 0.2f;
    public float HealthAreaAbility { get; set; } = 0.8f;
    public float HealthAreafSpell { get; set; } = 0.6f;
    public float HealthSingleAbility { get; set; } = 0.7f;
    public float HealthSingleSpell { get; set; } = 0.5f;
    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
