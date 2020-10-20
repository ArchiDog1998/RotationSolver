using System;

namespace XIVComboPlugin
{
    //CURRENT HIGHEST FLAG IS 56
    [Flags]
    public enum CustomComboPreset : long
    {
        None = 0,

        /*
        DragoonCoerthanTormentCombo     = 1L << 0,
        DragoonChaosThrustCombo         = 1L << 1,
        DragoonFullThrustCombo          = 1L << 2,
        DarkSouleaterCombo              = 1L << 3,
        DarkStalwartSoulCombo           = 1L << 4,
        PaladinGoringBladeCombo         = 1L << 5,
        PaladinRoyalAuthorityCombo      = 1L << 6,
        PaladinProminenceCombo          = 1L << 7,
        WarriorStormsPathCombo          = 1L << 8,
        WarriorStormsEyeCombo           = 1L << 9,
        WarriorMythrilTempestCombo      = 1L << 10,
        SamuraiYukikazeCombo            = 1L << 11,
        SamuraiGekkoCombo               = 1L << 12,
        SamuraiKashaCombo               = 1L << 13,
        SamuraiMangetsuCombo            = 1L << 14,
        SamuraiOkaCombo                 = 1L << 15,
        // 16
        NinjaArmorCrushCombo            = 1L << 17,
        NinjaAeolianEdgeCombo           = 1L << 18,
        NinjaHakkeMujinsatsuCombo       = 1L << 19,
        GunbreakerSolidBarrelCombo      = 1L << 20,
        GunbreakerGnashingFangCombo     = 1L << 21,
        GunbreakerDemonSlaughterCombo   = 1L << 22,
        MachinistMainCombo              = 1L << 23,
        MachinistSpreadShotFeature      = 1L << 24,
        BlackEnochianFeature            = 1L << 25,
        BlackManaFeature                = 1L << 26,
        AstrologianCardsOnDrawFeature   = 1L << 27,
        SummonerDemiCombo               = 1L << 28,
        ScholarSeraphConsolationFeature = 1L << 29,
        // 30
        // 31
        DancerAoeGcdFeature             = 1L << 32,
        DancerFanDanceCombo             = 1L << 33,
        // 34
        WhiteMageSolaceMiseryFeature    = 1L << 35,
        WhiteMageRaptureMiseryFeature   = 1L << 36,
        ScholarEnergyDrainFeature       = 1L << 37,
        SummonerBoPCombo                = 1L << 38,
        SummonerEDFesterCombo           = 1L << 39,
        SummonerESPainflareCombo        = 1L << 40,
        BardWandererPPFeature           = 1L << 41,
        BardStraightShotUpgradeFeature  = 1L << 42,
        // 43
        DragoonJumpFeature              = 1L << 44,
        NinjaAssassinateFeature         = 1L << 45,
        DragoonBOTDFeature              = 1L << 46,
        MachinistOverheatFeature        = 1L << 47,
        RedMageAoECombo                 = 1L << 48,
        RedMageMeleeCombo               = 1L << 49,
        // 50
        SamuraiThirdEyeFeature          = 1L << 51,
        GunbreakerGnashingFangCont      = 1L << 52,
        RedMageVerprocCombo             = 1L << 53,
        MnkAoECombo                     = 1L << 54,
        PaladinRequiescatCombo          = 1L << 55,
        BlackLeyLines                   = 1L << 56,
        
        // Custom
        GunbreakerFatedCircleFeature    = 1L << 30,
        DancerDanceStepCombo            = 1L << 31,
        DancerFlourishFeature           = 1L << 34,
        DancerSingleTargetMultibutton   = 1L << 43,
        DancerAoeMultibutton            = 1L << 50,
        */

        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("Jump + Mirage Dive", "Replace (High) Jump with Mirage Dive when Dive Ready", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 1L << 44,

        [CustomComboInfo("BOTD Into Stardiver", "Replace Blood of the Dragon with Stardiver when in Life of the Dragon", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 1L << 46,

        [CustomComboInfo("Coerthan Torment Combo", "Replace Coerthan Torment with its combo chain", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 1L << 0,

        [CustomComboInfo("Chaos Thrust Combo", "Replace Chaos Thrust with its combo chain", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 1L << 1,

        [CustomComboInfo("Full Thrust Combo", "Replace Full Thrust with its combo chain", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 1L << 2,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("Souleater Combo", "Replace Souleater with its combo chain", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 1L << 3,

        [CustomComboInfo("Stalwart Soul Combo", "Replace Stalwart Soul with its combo chain", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 1L << 4,

        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 1L << 5,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 1L << 6,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 1L << 7,

        [CustomComboInfo("Requiescat Confiteor", "Replace Requiescat with Confiter while under the effect of Requiescat", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 1L << 55,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "Replace Storms Path with its combo chain", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 1L << 8,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 1L << 9,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Mythril Tempest with its combo chain", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 1L << 10,

        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("Yukikaze Combo", "Replace Yukikaze with its combo chain", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 1L << 11,

        [CustomComboInfo("Gekko Combo", "Replace Gekko with its combo chain", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 1L << 12,

        [CustomComboInfo("Kasha Combo", "Replace Kasha with its combo chain", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 1L << 13,

        [CustomComboInfo("Mangetsu Combo", "Replace Mangetsu with its combo chain", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 1L << 14,

        [CustomComboInfo("Oka Combo", "Replace Oka with its combo chain", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 1L << 15,

        [CustomComboInfo("Seigan to Third Eye", "Replace Seigan with Third Eye when not procced", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 1L << 51,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("Armor Crush Combo", "Replace Armor Crush with its combo chain", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 1L << 17,

        [CustomComboInfo("Aeolian Edge Combo", "Replace Aeolian Edge with its combo chain", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 1L << 18,

        [CustomComboInfo("Hakke Mujinsatsu Combo", "Replace Hakke Mujinsatsu with its combo chain", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 1L << 19,

        [CustomComboInfo("Dream to Assassinate", "Replace Dream Within a Dream with Assassinate when Assassinate Ready", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 1L << 45,
        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Solid Barrel Combo", "Replace Solid Barrel with its combo chain", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 1L << 20,

        [CustomComboInfo("Wicked Talon Combo", "Replace Wicked Talon with its combo chain", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCombo = 1L << 21,

        [CustomComboInfo("Wicked Talon Continuation", "In addition to the Wicked Talon combo chain, put Continuation moves on Wicked Talon when appropriate", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCont = 1L << 52,

        [CustomComboInfo("Demon Slaughter Combo", "Replace Demon Slaughter with its combo chain", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 1L << 22,

        [CustomComboInfo("Fated Circle Feature", "In addition to the Demon Slaughter combo, add Fated Circle when charges are full", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerFatedCircleFeature = 1L << 30,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain", MCH.JobID, MCH.HeatedCleanShot, MCH.CleanShot)]
        MachinistMainCombo = 1L << 23,

        [CustomComboInfo("Spread Shot Heat", "Replace Spread Shot with Auto Crossbow when overheated", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 1L << 24,

        [CustomComboInfo("Heat Blast when overheated", "Replace Hypercharge with Heat Blast when overheated", MCH.JobID, MCH.Hypercharge)]
        MachinistOverheatFeature = 1L << 47,

        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("Enochian Stance Switcher", "Change Enochian to Fire 4 or Blizzard 4 depending on stance", BLM.JobID, BLM.Enochian)]
        BlackEnochianFeature = 1L << 25,

        [CustomComboInfo("Umbral Soul/Transpose Switcher", "Change Transpose into Umbral Soul when Umbral Soul is usable", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 1L << 26,

        [CustomComboInfo("(Between the) Ley Lines", "Change Ley Lines into BTL when Ley Lines is active", BLM.JobID, BLM.LeyLines)]
        BlackLeyLines = 1L << 56,

        #endregion
        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("Draw on Play", "Play turns into Draw when no card is drawn, as well as the usual Play behavior", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 1L << 27,

        #endregion
        // ====================================================================================
        #region SUMMONER

        [CustomComboInfo("Demi-summon combiners", "Dreadwyrm Trance, Summon Bahamut, and Firebird Trance are now one button.\nDeathflare, Enkindle Bahamut, and Enkindle Phoenix are now one button", SMN.JobID, SMN.DreadwyrmTrance, SMN.Deathflare)]
        SummonerDemiCombo = 1L << 28,

        [CustomComboInfo("Brand of Purgatory Combo", "Replaces Fountain of Fire with Brand of Purgatory when under the affect of Hellish Conduit", SMN.JobID, SMN.Ruin1, SMN.Ruin3)]
        SummonerBoPCombo = 1L << 38,

        [CustomComboInfo("ED Fester", "Change Fester into Energy Drain when out of Aetherflow stacks", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 1L << 39,

        [CustomComboInfo("ES Painflare", "Change Painflare into Energy Syphon when out of Aetherflow stacks", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 1L << 40,
        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("Seraph Fey Blessing/Consolation", "Change Fey Blessing into Consolation when Seraph is out", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 1L << 29,

        [CustomComboInfo("ED Aetherflow", "Change Energy Drain into Aetherflow when you have no more Aetherflow stacks", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 1L << 37,

        #endregion
        // ====================================================================================
        #region DANCER

        /*
        [CustomComboInfo("AoE GCD procs", "DNC AoE procs turn into their normal abilities when not procced", DNC.JobID, DNC.Bloodshower, DNC.RisingWindmill)]
        DancerAoeGcdFeature = 1L << 32,
        */

        [CustomComboInfo("Fan Dance Combos", "Change Fan Dance and Fan Dance 2 into Fan Dance 3 while flourishing", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 1L << 33,

        [CustomComboInfo("Dance Step Combo", "Change Standard Step and Technical Step into each dance step while dancing", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 1L << 31,

        [CustomComboInfo("Flourish Proc Saver", "Change Flourish into any available procs before using", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 1L << 34,

        [CustomComboInfo("Single Target Multibutton", "Change Cascade into procs and combos as available", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 1L << 43,

        [CustomComboInfo("AoE Multibutton", "Change Windmill into procs and combos as available", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 1L << 50,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.Solace)]
        WhiteMageSolaceMiseryFeature = 1L << 35,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.Misery)]
        WhiteMageRaptureMiseryFeature = 1L << 36,

        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("Wanderer's into Pitch Perfect", "Replaces Wanderer's Minuet with Pitch Perfect while in WM", BRD.JobID, BRD.WanderersMinuet)]
        BardWandererPPFeature = 1L << 41,

        [CustomComboInfo("Heavy Shot into Straight Shot", "Replaces Heavy Shot/Burst Shot with Straight Shot/Refulgent Arrow when procced", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 1L << 42,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("Monk AoE Combo", "Replaces Rockbreaker with the AoE combo chain, or Rockbreaker when Perfect Balance is active", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 1L << 54,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("Red Mage AoE Combo", "Replaces Veraero/thunder 2 with Impact when Dualcast or Swiftcast are active", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 1L << 48,

        [CustomComboInfo("Redoublement combo", "Replaces Redoublement with its combo chain, following enchantment rules", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 1L << 49,

        [CustomComboInfo("Verproc into Jolt", "Replaces Verstone/Verfire with Jolt/Scorch when no proc is available.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 1L << 53

        #endregion
        // ====================================================================================
    }

    public class CustomComboInfoAttribute : Attribute
    {
        internal CustomComboInfoAttribute(string fancyName, string description, byte classJob, params uint[] abilities)
        {
            FancyName = fancyName;
            Description = description;
            ClassJob = classJob;
            Abilities = abilities;
        }

        public string FancyName { get; }
        public string Description { get; }
        public byte ClassJob { get; }
        public uint[] Abilities { get; }
    }
}
