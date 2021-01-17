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

        public bool IsEnabled(CustomComboPreset preset) => EnabledActions.Contains(preset);

        #region Obsolete
#pragma warning disable IDE1006 // Naming Styles

        // By omitting the getter, we can prevent any obsolete fields from being serialized after the upgrade

        [JsonProperty("ComboPresets")]
        [Obsolete("This was removed in favor of EnabledActions in version 4")]
        public LegacyCustomComboPreset _ComboPresets { set { _ComboPresetsBacker = value; } }

        [JsonIgnore]
        [Obsolete("This was added to prevent serialization of another obsolete property")]
        private LegacyCustomComboPreset _ComboPresetsBacker;

        [JsonProperty("HiddenActions")]
        [Obsolete("This was removed in favor of HiddenActions in version 4")]
        public List<bool> _HiddenActions { set { _HiddenActionsBacker = value; } }

        [JsonIgnore]
        [Obsolete("This was added to prevent serialization of another obsolete property")]
        private List<bool> _HiddenActionsBacker = new List<bool>();

#pragma warning restore IDE1006 // Naming Styles
        #endregion

        public void Upgrade()
        {
            if (Version < 3)
                UpgradeToVersion3();
            if (Version == 3)
                UpgradeToVersion4();
        }

        private void UpgradeToVersion3()
        {
            PluginLog.Information("Upgrading configuration to version 3");
            foreach (var _ in Enum.GetValues(typeof(CustomComboPreset)))
#pragma warning disable CS0618 // Type or member is obsolete
                _HiddenActionsBacker.Add(false);
#pragma warning restore CS0618 // Type or member is obsolete
            Version = 3;
        }

        private void UpgradeToVersion4()
        {
#pragma warning disable CS0612,CS0618 // Type or member is obsolete
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
                    }
                }
            }
            _ComboPresetsBacker = 0;
            _HiddenActionsBacker = null;
            Version = 4;
#pragma warning restore CS0612,CS0618 // Type or member is obsolete
        }
    }
}
