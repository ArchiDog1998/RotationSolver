using System;

namespace XIVComboExpandedestPlugin
{
    public enum CustomComboPreset
    {
        // Last enum used: 72
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

        [CustomComboInfo("Delirium Feature", "Replace Souleater and Stalwart Soul with Bloodspiller and Quietus when Delirium is active", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 57,
        
        [CustomComboInfo("Gauge Overcap Feature", "Replace Single-target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate gauge", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DRKOvercapFeature = 71,

        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("Goring Blade Combo", "Replace Goring Blade with its combo chain", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 5,

        [CustomComboInfo("Royal Authority Combo", "Replace Royal Authority/Rage of Halone with its combo chain", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("Atonement Feature", "Replace Royal Authority with Atonement when under the effect of Sword Oath", PLD.JobID, PLD.RoyalAuthority)]
        PaladinAtonementFeature = 59,

        [CustomComboInfo("Prominence Combo", "Replace Prominence with its combo chain", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("Requiescat Confiteor", "Replace Requiescat with Confiter while under the effect of Requiescat", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 55,

        [CustomComboInfo("Requiescat Feature", "Replace Royal Authority combo with Holy Spirit and Prominence combo with Holy Circle while Requiescat is active. Requires said combos to be activated.", PLD.JobID, PLD.RoyalAuthority, PLD.Prominence)]
        PaladinRequiescatFeature = 63,

        [CustomComboInfo("Confiteor Feature", "Replace Holy Spirit/Circle with Confiteor while MP is under 4000 and Requiescat is up.", PLD.JobID, PLD.HolySpirit, PLD.HolyCircle)]
        PaladinConfiteorFeature = 64,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("Storms Path Combo", "Replace Storms Path with its combo chain", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("Storms Eye Combo", "Replace Storms Eye with its combo chain", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("Mythril Tempest Combo", "Replace Mythril Tempest with its combo chain", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 10,

        [CustomComboInfo("Gauge Overcap Feature", "Replace Single-target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate gauge", WAR.JobID, WAR.MythrilTempest, WAR.StormsEye, WAR.StormsPath)]
        WarriorGaugeOvercapFeature = 67,

        [CustomComboInfo("Infuriate Gauge Overcap Feature", "Make Infuriate Decimate if Infuriate would overcap you", WAR.JobID, WAR.Infuriate)]
        WarriorInfuriateOvercapFeature = 68,

        [CustomComboInfo("Inner Release Feature", "Replace Single-target and AoE combo with Fell Cleave/Decimate during Inner Release to prevent wasted GCDs", WAR.JobID, WAR.MythrilTempest, WAR.StormsPath)]
        WarriorInnerReleaseFeature = 69,

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

        [CustomComboInfo("Jinpu/Shifu Feature", "Replace Seigan and Meditate with Jinpu/Shifu respectively during Meikyo", SAM.JobID, SAM.Seigan, SAM.Meditate)]
        SamuraiJinpuShifuFeature = 72,

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

        [CustomComboInfo("Gauge Overcap Feature", "Replace Single-target or AoE combo with gauge spender if you are about to overcap and are before a step of a combo that would generate gauge", GNB.JobID, GNB.DemonSlaughter, GNB.SolidBarrel)]
        GunbreakerGaugeOvercapFeature = 30,

        [CustomComboInfo("Bloodfest Gauge Overcap Feature", "Replace Bloodfest with Burst Strike if it would cause you to overcap.", GNB.JobID, GNB.Bloodfest)]
        GunbreakerBloodfestOvercapFeature = 70,

        [CustomComboInfo("Sonic Break Feature", "Replace No Mercy with Sonic Break while No Mercy is active.", GNB.JobID, GNB.NoMercy)]
        GunbreakerSonicBreakFeature = 66,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("(Heated) Shot Combo", "Replace either form of Clean Shot with its combo chain", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistMainCombo = 23,

        [CustomComboInfo("Spread Shot Heat", "Replace Spread Shot with Auto Crossbow when overheated", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("Hypercharge Feature", "Replace Heat Blast and Auto Crossbow with Hypercharge when not overheated", MCH.JobID, MCH.HeatBlast, MCH.AutoCrossbow)]
        MachinistOverheatFeature = 47,

        [CustomComboInfo("Overdrive Feature", "Replace Rook Autoturret and Automaton Queen with Overdrive while active", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 58,

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

        [CustomComboInfo("Benefic 2 to Benefic Level Sync", "Changes Benefic 2 to Benefic when below level 26 in synced content.", AST.JobID, AST.Benefic2)]
        AstrologianBeneficFeature = 62,

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

        [CustomComboInfo("Solace into Misery", "Replaces Afflatus Solace with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusSolace)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("Rapture into Misery", "Replaces Afflatus Rapture with Afflatus Misery when Misery is ready to be used", WHM.JobID, WHM.AfflatusRapture)]
        WhiteMageRaptureMiseryFeature = 36,

        [CustomComboInfo("Cure 2 to Cure Level Sync", "Changes Cure 2 to Cure when below level 30 in synced content.", WHM.JobID, WHM.Cure2)]
        WhiteMageCureFeature = 60,

        [CustomComboInfo("Afflatus Feature", "Changes Cure 2 into Afflatus Solace, and Medica into Afflatus Rapture, when lilies are up.", WHM.JobID, WHM.Cure2, WHM.Medica)]
        WhiteMageAfflatusFeature = 61,

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

        [CustomComboInfo("Monk Bootshine Feature", "Replaces Dragon Kick with Bootshine if both a form and Leaden Fist are up.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineFeature = 65,

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

    internal class CustomComboInfoAttribute : Attribute
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
            return key switch
            {
                1 => "Gladiator",
                2 => "Pugilist",
                3 => "Marauder",
                4 => "Lancer",
                5 => "Archer",
                6 => "Conjurer",
                7 => "Thaumaturge",
                8 => "Carpenter",
                9 => "Blacksmith",
                10 => "Armorer",
                11 => "Goldsmith",
                12 => "Leatherworker",
                13 => "Weaver",
                14 => "Alchemist",
                15 => "Culinarian",
                16 => "Miner",
                17 => "Botanist",
                18 => "Fisher",
                19 => "Paladin",
                20 => "Monk",
                21 => "Warrior",
                22 => "Dragoon",
                23 => "Bard",
                24 => "White Mage",
                25 => "Black Mage",
                26 => "Arcanist",
                27 => "Summoner",
                28 => "Scholar",
                29 => "Rogue",
                30 => "Ninja",
                31 => "Machinist",
                32 => "Dark Knight",
                33 => "Astrologian",
                34 => "Samurai",
                35 => "Red Mage",
                36 => "Blue Mage",
                37 => "Gunbreaker",
                38 => "Dancer",
                _ => "Unknown",
            };
        }
    }
}
