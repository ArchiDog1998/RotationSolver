using System;

namespace XIVComboPlugin
{
    public enum CustomComboPreset
    {
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("Jump + Mirage Dive", "Replace (High) Jump with Mirage Dive when Dive Ready", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 44,

        [CustomComboInfo("BOTD Into Stardiver", "Replace Blood of the Dragon with Stardiver when in Life of the Dragon", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 46,

        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 0,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 1,

        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 2,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 3,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 4,

        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 5,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("Requiescat Confiteor", "Replace Requiescat with Confiter while under the effect of Requiescat", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 55,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "Replace Storms Path with its combo chain", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Mythril Tempest with its combo chain", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 10,

        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 11,

        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 12,

        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 13,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 14,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 15,

        [CustomComboInfo("Seigan to Third Eye", "Replace Seigan with Third Eye when not procced", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 51,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 17,

        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 18,

        [CustomComboInfo("Hakke Mujinsatsu Combo", "Replace Hakke Mujinsatsu with its combo chain", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 19,

        [CustomComboInfo("Dream to Assassinate", "Replace Dream Within a Dream with Assassinate when Assassinate Ready", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 45,
        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 20,

        [CustomComboInfo("Wicked Talon Combo", "Replace Wicked Talon with its combo chain", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCombo = 21,

        [CustomComboInfo("Wicked Talon Continuation", "In addition to the Wicked Talon combo chain, put Continuation moves on Wicked Talon when appropriate", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCont = 52,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 22,

        [CustomComboInfo("Fated Circle Feature", "In addition to the Demon Slaughter combo, add Fated Circle when charges are full", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerFatedCircleFeature = 30,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain", MCH.JobID, MCH.HeatedCleanShot, MCH.CleanShot)]
        MachinistMainCombo = 23,

        [CustomComboInfo("Spread Shot Heat", "Replace Spread Shot with Auto Crossbow when overheated", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("Heat Blast when overheated", "Replace Hypercharge with Heat Blast when overheated", MCH.JobID, MCH.Hypercharge)]
        MachinistOverheatFeature = 47,

        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("Enochian Stance Switcher", "Change Enochian to Fire 4 or Blizzard 4 depending on stance", BLM.JobID, BLM.Enochian)]
        BlackEnochianFeature = 25,

        [CustomComboInfo("Umbral Soul/Transpose Switcher", "Change Transpose into Umbral Soul when Umbral Soul is usable", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 26,

        [CustomComboInfo("(Between the) Ley Lines", "Change Ley Lines into BTL when Ley Lines is active", BLM.JobID, BLM.LeyLines)]
        BlackLeyLines = 56,

        #endregion
        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("Draw on Play", "Play turns into Draw when no card is drawn, as well as the usual Play behavior", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 27,

        #endregion
        // ====================================================================================
        #region SUMMONER

        [CustomComboInfo("Demi-summon combiners", "Dreadwyrm Trance, Summon Bahamut, and Firebird Trance are now one button.\nDeathflare, Enkindle Bahamut, and Enkindle Phoenix are now one button", SMN.JobID, SMN.DreadwyrmTrance, SMN.Deathflare)]
        SummonerDemiCombo = 28,

        [CustomComboInfo("Brand of Purgatory Combo", "Replaces Fountain of Fire with Brand of Purgatory when under the affect of Hellish Conduit", SMN.JobID, SMN.Ruin1, SMN.Ruin3)]
        SummonerBoPCombo = 38,

        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when out of Aetherflow stacks", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 39,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Syphon when out of Aetherflow stacks", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 40,
        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("Seraph Fey Blessing/Consolation", "Change Fey Blessing into Consolation when Seraph is out", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 29,

        [CustomComboInfo("ED Aetherflow", "Change Energy Drain into Aetherflow when you have no more Aetherflow stacks", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 37,

        #endregion
        // ====================================================================================
        #region DANCER

        /*
        [CustomComboInfo("AoE GCD procs", "DNC AoE procs turn into their normal abilities when not procced", DNC.JobID, DNC.Bloodshower, DNC.RisingWindmill)]
        DancerAoeGcdFeature = 32,
        */

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 33,

        [CustomComboInfo("Dance Step Combo", "Change Standard Step and Technical Step into each dance step while dancing", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 31,

        [CustomComboInfo("Flourish Proc Saver", "Change Flourish into any available procs before using", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 34,

        [CustomComboInfo("Single Target Multibutton", "Change Cascade into procs and combos as available", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 43,

        [CustomComboInfo("AoE Multibutton", "Change Windmill into procs and combos as available", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 50,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.Solace)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.Misery)]
        WhiteMageRaptureMiseryFeature = 36,

        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("Wanderer's into Pitch Perfect", "Replaces Wanderer's Minuet with Pitch Perfect while in WM", BRD.JobID, BRD.WanderersMinuet)]
        BardWandererPPFeature = 41,

        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 42,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("Monk AoE Combo", "Replaces Rockbreaker with the AoE combo chain, or Rockbreaker when Perfect Balance is active", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 54,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/thunder 2 with Impact when Dualcast or Swiftcast are active", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 48,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 49,

        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 53,

        #endregion
        // ====================================================================================
    }

    public class CustomComboInfoAttribute : Attribute
    {
        internal CustomComboInfoAttribute(string fancyName, string description, byte jobID, params uint[] abilities)
        {
            FancyName = fancyName;
            Description = description;
            JobID = jobID;
            Abilities = abilities;
        }

        public string FancyName { get; }
        public string Description { get; }
        public byte JobID { get; }
        public string JobName => JobIDToName(JobID);
        public uint[] Abilities { get; }

        private static string JobIDToName(byte key)
        {
            switch (key)
            {
                default: return "Unknown";
                case 1: return "Gladiator";
                case 2: return "Pugilist";
                case 3: return "Marauder";
                case 4: return "Lancer";
                case 5: return "Archer";
                case 6: return "Conjurer";
                case 7: return "Thaumaturge";
                case 8: return "Carpenter";
                case 9: return "Blacksmith";
                case 10: return "Armorer";
                case 11: return "Goldsmith";
                case 12: return "Leatherworker";
                case 13: return "Weaver";
                case 14: return "Alchemist";
                case 15: return "Culinarian";
                case 16: return "Miner";
                case 17: return "Botanist";
                case 18: return "Fisher";
                case 19: return "Paladin";
                case 20: return "Monk";
                case 21: return "Warrior";
                case 22: return "Dragoon";
                case 23: return "Bard";
                case 24: return "White Mage";
                case 25: return "Black Mage";
                case 26: return "Arcanist";
                case 27: return "Summoner";
                case 28: return "Scholar";
                case 29: return "Rogue";
                case 30: return "Ninja";
                case 31: return "Machinist";
                case 32: return "Dark Knight";
                case 33: return "Astrologian";
                case 34: return "Samurai";
                case 35: return "Red Mage";
                case 36: return "Blue Mage";
                case 37: return "Gunbreaker";
                case 38: return "Dancer";
            }
        }
    }

    [Flags]
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
}
