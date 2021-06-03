using Dalamud.Configuration;
using Dalamud.Game.Text;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XIVComboExpandedestPlugin
{
    [Serializable]
    public class XIVComboExpandedestConfiguration : IPluginConfiguration
    {
        public int Version { get; set; } = 4;

        [JsonProperty("EnabledActionsV4")]
        public HashSet<CustomComboPreset> EnabledActions = new();

        [JsonProperty("Debug")]
        public bool ShowSecrets = false;

        public int DanceAction1, DanceAction2, DanceAction3, DanceAction4;

        public int gauge1;

        public bool IsEnabled(CustomComboPreset preset) => EnabledActions.Contains(preset);

        public bool IsSecret(CustomComboPreset preset) => preset.GetAttribute<SecretCustomComboAttribute>() != default;

        #region Obsolete
#pragma warning disable IDE1006 // Naming Styles

        [Flags]
        [Obsolete]
        public enum LegacyCustomComboPreset : long
        {
            None = 0,
            DragoonJumpFeature = 1L << 44,
            DragoonBOTDFeature = 1L << 46,
            DragoonCoerthanTormentCombo = 1L << 0,
            DragoonChaosThrustCombo = 1L << 1,
            DragoonFullThrustCombo = 1L << 2,
            DarkSouleaterCombo = 1L << 3,
            DarkStalwartSoulCombo = 1L << 4,
            PaladinGoringBladeCombo = 1L << 5,
            PaladinRoyalAuthorityCombo = 1L << 6,
            PaladinProminenceCombo = 1L << 7,
            PaladinRequiescatCombo = 1L << 55,
            WarriorStormsPathCombo = 1L << 8,
            WarriorStormsEyeCombo = 1L << 9,
            WarriorMythrilTempestCombo = 1L << 10,
            SamuraiYukikazeCombo = 1L << 11,
            SamuraiGekkoCombo = 1L << 12,
            SamuraiKashaCombo = 1L << 13,
            SamuraiMangetsuCombo = 1L << 14,
            SamuraiOkaCombo = 1L << 15,
            SamuraiThirdEyeFeature = 1L << 51,
            NinjaArmorCrushCombo = 1L << 17,
            NinjaAeolianEdgeCombo = 1L << 18,
            NinjaHakkeMujinsatsuCombo = 1L << 19,
            NinjaAssassinateFeature = 1L << 45,
            GunbreakerSolidBarrelCombo = 1L << 20,
            GunbreakerGnashingFangCombo = 1L << 21,
            GunbreakerGnashingFangCont = 1L << 52,
            GunbreakerDemonSlaughterCombo = 1L << 22,
            MachinistMainCombo = 1L << 23,
            MachinistSpreadShotFeature = 1L << 24,
            MachinistOverheatFeature = 1L << 47,
            BlackEnochianFeature = 1L << 25,
            BlackManaFeature = 1L << 26,
            BlackLeyLines = 1L << 56,
            AstrologianCardsOnDrawFeature = 1L << 27,
            SummonerDemiCombo = 1L << 28,
            SummonerBoPCombo = 1L << 38,
            SummonerEDFesterCombo = 1L << 39,
            SummonerESPainflareCombo = 1L << 40,
            ScholarSeraphConsolationFeature = 1L << 29,
            ScholarEnergyDrainFeature = 1L << 37,
            DancerAoeGcdFeature = 1L << 32,
            DancerFanDanceCombo = 1L << 33,
            WhiteMageSolaceMiseryFeature = 1L << 35,
            WhiteMageRaptureMiseryFeature = 1L << 36,
            BardWandererPPFeature = 1L << 41,
            BardStraightShotUpgradeFeature = 1L << 42,
            MnkAoECombo = 1L << 54,
            RedMageAoECombo = 1L << 48,
            RedMageMeleeCombo = 1L << 49,
            RedMageVerprocCombo = 1L << 53,
        }

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
