using System;
using System.Linq;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboPlus.Combos;

namespace XIVComboPlus;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 5;

    public int HostileCount { get; set; } = 3;
    public int PartyCount { get; set; } = 2;
    public SortedSet<string> EnabledActions { get; private set; } = new SortedSet<string>();
    public List<ActionEvents> Events { get; private set; } = new List<ActionEvents>();
    public bool AllTargeAsHostile { get; set; } = false;
    public bool AutoBreak { get; set; } = true;
    public bool OnlyGCD { get; set; } = false;
    public bool NeverReplaceIcon { get; set; } = false;
    public float HealthDifference { get; set; } = 0.2f;
    public float HealthAreaAbility { get; set; } = 0.8f;
    public float HealthAreafSpell { get; set; } = 0.6f;
    public float HealthSingleAbility { get; set; } = 0.75f;
    public float HealthSingleSpell { get; set; } = 0.5f;
    public float SpecialDuration { get; set; } = 5;
    public float WeaponInterval { get; set; } = 0.67f;
    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
