using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XIVComboPlugin
{
    [Serializable]
    public class XIVComboConfiguration : IPluginConfiguration
    {
        public int Version { get; set; } = 4;

        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions = new HashSet<CustomComboPreset>();

        [JsonProperty("HiddenActionsV4")]
        public HashSet<CustomComboPreset> HiddenActions = new HashSet<CustomComboPreset>();

        public bool IsEnabled(CustomComboPreset preset) => EnabledActions.Contains(preset);

        public bool IsHidden(CustomComboPreset preset) => HiddenActions.Contains(preset);

        #region Obsolete

        // By omitting the getter, we can prevent any obsolete fields from being serialized after the upgrade

        [Obsolete]  // Version < 3
        [JsonProperty("ComboPresets")]
        public LegacyCustomComboPreset _ComboPresets { set { _ComboPresetsBacker = value; } }
        private LegacyCustomComboPreset _ComboPresetsBacker;

        [Obsolete]  // Version == 3
        [JsonProperty("HiddenActions")]
        public List<bool> _HiddenActions { set { _HiddenActionsBacker = value; } }
        private List<bool> _HiddenActionsBacker = new List<bool>();

        #endregion

        public void Upgrade()
        {
            if (Version < 3)
                UpgradeToVersion3();
            if (Version == 3)
                UpgradeToVersion4();
        }

        private void UpgradeToVersion4()
        {
            PluginLog.Information("Upgrading configuration to version 4");
            foreach (LegacyCustomComboPreset legacyPreset in Enum.GetValues(typeof(LegacyCustomComboPreset)))
            {
                if (_ComboPresetsBacker.HasFlag(legacyPreset))
                {
                    int legacyPresetIndex = (int)Math.Log((long)legacyPreset, 2);
                    CustomComboPreset preset = (CustomComboPreset)legacyPresetIndex;
                    if (Enum.IsDefined(typeof(CustomComboPreset), preset))
                    {
                        EnabledActions.Add(preset);
                        if (_HiddenActionsBacker.Count < legacyPresetIndex && _HiddenActionsBacker[legacyPresetIndex])
                            HiddenActions.Add(preset);
                    }
                }
            }
            _ComboPresetsBacker = 0;
            _HiddenActionsBacker = null;
            Version = 4;
        }

        private void UpgradeToVersion3()
        {
            PluginLog.Information("Upgrading configuration to version 3");
            foreach (var _ in Enum.GetValues(typeof(CustomComboPreset)))
                _HiddenActionsBacker.Add(false);
            Version = 3;
        }
    }
}
