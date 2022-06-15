using System;
using System.Linq;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Utility;

namespace XIVAutoAttack.Configuration;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 5;

    public int HostileCount { get; set; } = 3;
    public int PartyCount { get; set; } = 2;
    public int VoiceVolume { get; set; } = 80;
    public SortedSet<string> EnabledActions { get; private set; } = new SortedSet<string>();
    public List<ActionEvents> Events { get; private set; } = new List<ActionEvents>();
    public Dictionary<string, ActionConfiguration> ActionsConfigurations { get; private set; } = new Dictionary<string, ActionConfiguration>();
    public bool AllTargeAsHostile { get; set; } = false;
    public bool AutoBreak { get; set; } = true;
    public bool OnlyGCD { get; set; } = false;
    public bool NeverReplaceIcon { get; set; } = false;
    public bool AutoDefenseForTank { get; set; } = true;
    public bool AutoSayingOut { get; set; } = false;
    public bool UseDtr { get; set; } = true;
    public bool SayingLocation { get; set; } = true;
    public bool TextLocation { get; set; } = true;
    public bool UseToast { get; set; } = true;
    public bool RaiseAll { get; set; } = false;
    public bool CheckForCasting { get; set; } = true;
    public bool UseItem { get; set; } = true;
    public float ObjectMinRadius { get; set; } = 0f;
    public float HealthDifference { get; set; } = 0.25f;
    public float HealthAreaAbility { get; set; } = 0.75f;
    public float HealthAreafSpell { get; set; } = 0.65f;
    public float HealthSingleAbility { get; set; } = 0.7f;
    public float HealthSingleSpell { get; set; } = 0.55f;
    public float HealthForDyingTank { get; set; } = 0.15f;

    public float SpecialDuration { get; set; } = 3;
    public float WeaponInterval { get; set; } = 0.67f;
    public float WeaponFaster { get; set; } = 0.05f;
    public float WeaponDelay { get; set; } = 0;
    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
