namespace XIVComboExpandedestPlugin
{

    internal static class ACN
    {
        public const byte JobID = 26;

        public const uint
            Resurrection = 173;
    }
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Ascend = 3603,
            Play = 17055,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            Benefic2 = 3610,
            Benefic = 3594,
            SleeveDraw = 7448,
            MinorArcana = 7443,
            Lady = 7445,
            Lord = 7444;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte 
                Benefic2 = 26,
                MinorArcana = 50,
                SleeveDraw = 70;
        }
    }

    internal static class BLM
    {
        public const byte JobID = 25;

        public const uint
            Enochian = 3575,
            Blizzard4 = 3576,
            Fire = 141,
            Fire3 = 152,
            Fire4 = 3577,
            Blizzard = 142,
            Blizzard2 = 146,
            Blizzard3 = 154,
            Thunder = 144,
            Thunder3 = 153,
            Despair = 16505,
            Flare = 162,
            Freeze = 159,
            Transpose = 149,
            UmbralSoul = 16506,
            LeyLines = 3573,
            BetweenTheLines = 7419;

        public static class Buffs
        {
            public const short
                Thundercloud = 164,
                LeyLines = 737,
                Firestarter = 165;
        }
        public static class Debuffs
        {
            public const short
                Thunder = 161,
                Thunder3 = 163;
        }

        public static class Levels
        {
            public const byte
                Fire3 = 34,
                Freeze = 35,
                Blizzard3 = 40,
                Thunder3 = 45,
                Flare = 50,
                Enochian = 56,
                Blizzard4 = 58,
                Fire4 = 60,
                BetweenTheLines = 62,
                Despair = 72,
                UmbralSoul = 76;
        }
    }

    internal static class BRD
    {
        public const byte JobID = 23;

        public const uint
            WanderersMinuet = 3559,
            PitchPerfect = 7404,
            HeavyShot = 97,
            QuickNock = 106,
            BurstShot = 16495,
            StraightShot = 98,
            VenomousBite = 100,
            Windbite = 113,
            IronJaws = 3560,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            ApexArrow = 16496;

        public static class Buffs
        {
            public const short
                StraightShotReady = 122;
        }

        public static class Debuffs
        {
            public const short
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }


        public static class Levels
        {
            public const byte
                Windbite = 30,
                IronJaws = 56,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                BurstShot = 76;
        }
    }

    internal static class DoM
    {
        public const byte JobID = 99;

        public const uint
            Swiftcast = 7561;

        public static class Buffs
        {
            public const short
                Swiftcast = 167;
        }

        public static class Levels
        {
            public const byte 
                Raise = 12;
        }
    }

    internal static class DNC
    {
        public const byte JobID = 38;

        public const uint
            // Single Target
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            // AoE
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            // Dancing
            StandardStep = 15997,
            TechnicalStep = 15998,
            StandardFinish0 = 16003,
            StandardFinish1 = 16191,
            StandardFinish2 = 16192,
            TechnicalFinish0 = 16004,
            TechnicalFinish1 = 16193,
            TechnicalFinish2 = 16194,
            TechnicalFinish3 = 16195,
            TechnicalFinish4 = 16196,
            // Fans
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            // Other
            EnAvant = 16010,
            Flourish = 16013,
            SaberDance = 16005;

        public static class Buffs
        {
            public const short
                FlourishingCascade = 1814,
                FlourishingFountain = 1815,
                FlourishingWindmill = 1816,
                FlourishingShower = 1817,
                StandardStep = 1818,
                TechnicalStep = 1819,
                FlourishingFanDance = 1820;
        }

        public static class Levels
        {
            public const byte
                Fountain = 2,
                Bladeshower = 25;
        }
    }

    internal static class DRG
    {
        public const byte JobID = 22;

        public const uint
            Jump = 92,
            HighJump = 16478,
            MirageDive = 7399,
            BloodOfTheDragon = 3553,
            Stardiver = 16480,
            CoerthanTorment = 16477,
            DoomSpike = 86,
            SonicThrust = 7397,
            ChaosThrust = 88,
            RaidenThrust = 16479,
            TrueThrust = 75,
            Disembowel = 87,
            FangAndClaw = 3554,
            WheelingThrust = 3556,
            FullThrust = 84,
            VorpalThrust = 78;

        public static class Buffs
        {
            public const short
                SharperFangAndClaw = 802,
                EnhancedWheelingThrust = 803,
                DiveReady = 1243,
                RaidenThrustReady = 1863;
        }

        public static class Levels
        {
            public const byte
                VorpalThrust = 4,
                Disembowel = 18,
                FullThrust = 26,
                ChaosThrust = 50,
                FangAndClaw = 56,
                WheelingThrust = 58,
                SonicThrust = 62,
                CoerthanTorment = 72,
                HighJump = 74,
                RaidenThrust = 76,
                Stardiver = 80;
        }
    }

    internal static class DRK
    {
        public const byte JobID = 32;

        public const uint
            Souleater = 3632,
            HardSlash = 3617,
            SyphonStrike = 3623,
            StalwartSoul = 16468,
            Unleash = 3621,
            Quietus = 7391,
            Bloodspiller = 7392,
            EdgeOfDarkness = 16467,
            EdgeOfShadow = 16470,
            FloodOfDarkness = 16466,
            FloodOfShadow = 16469;

        public static class Buffs
        {
            public const short
                Delirium = 1972,
                BloodWeapon = 742;
        }

        public static class Levels
        {
            public const byte
                SyphonStrike = 2,
                Souleater = 26,
                FloodOfDarkness = 30,
                EdgeOfDarkness = 40,
                Bloodpiller = 62,
                Quietus = 64,
                Delirium = 68,
                StalwartSoul = 72,
                Shadow = 74;
        }
    }

    internal static class GNB
    {
        public const byte JobID = 37;

        public const uint
            NoMercy = 16138,
            Bloodfest = 16164,
            SolidBarrel = 16145,
            BurstStrike = 16162,
            SonicBreak = 16153,
            BowShock = 16159,
            KeenEdge = 16137,
            BrutalShell = 16139,
            WickedTalon = 16150,
            GnashingFang = 16146,
            SavageClaw = 16147,
            DemonSlaughter = 16149,
            DemonSlice = 16141,
            Continuation = 16155,
            JugularRip = 16156,
            AbdomenTear = 16157,
            EyeGouge = 16158,
            FatedCircle = 16163;

        public static class Buffs
        {
            public const short
                NoMercy = 1831,
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844;
        }

        public static class Debuffs
        {
            public const short
                BowShock = 1838;
        }

        public static class Levels
        {
            public const byte
                BrutalShell = 4,
                SolidBarrel = 26,
                DemonSlaughter = 40,
                SonicBreak = 54,
                BowShock = 62,
                Continuation = 70,
                FatedCircle = 72,
                Bloodfest = 76;
        }
    }

    internal static class PLD
    {
        public const byte JobID = 19;

        public const uint
            GoringBlade = 3538,
            FastBlade = 9,
            RiotBlade = 15,
            RoyalAuthority = 3539,
            RageOfHalone = 21,
            Prominence = 16457,
            TotalEclipse = 7381,
            Requiescat = 7383,
            HolySpirit = 7384,
            HolyCircle = 16458,
            Atonement = 16460,
            Confiteor = 16459;

        public static class Buffs
        {
            public const short
                Requiescat = 1368,
                SwordOath = 1902;
        }

        public static class Levels
        {
            public const byte
                RiotBlade = 4,
                RageOfHalone = 26,
                Prominence = 40,
                GoringBlade = 54,
                RoyalAuthority = 60,
                HolyCircle = 72,
                Atonement = 76,
                Confiteor = 80;
        }
    }

    internal static class MCH
    {
        public const byte JobID = 31;

        public const uint
            GaussRound = 2874,
            Ricochet = 2890,
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            HeatedSlugshot = 7412,
            Hypercharge = 17209,
            HeatBlast = 7410,
            SpreadShot = 2870,
            AutoCrossbow = 16497,
            RookAutoturret = 2864,
            RookOverdrive = 7415,
            AutomatonQueen = 16501,
            QueenOverdrive = 16502;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                SlugShot = 2,
                CleanShot = 26,
                Hypercharge = 30,
                HeatBlast = 35,
                RookOverdrive = 40,
                Ricochet = 50,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                HeatedSlugshot = 60,
                HeatedCleanShot = 64,
                QueenOverdrive = 80;
        }

    }

    internal static class MNK
    {
        public const byte JobID = 20;

        public const uint
            Bootshine = 53,
            DragonKick = 74,
            SnapPunch = 56,
            TwinSnakes = 61,
            Demolish = 66,
            ArmOfTheDestroyer = 62,
            FourPointFury = 16473,
            Rockbreaker = 70;

        public static class Buffs
        {
            public const short
                TwinSnakes = 101,
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110,
                LeadenFist = 1861,
                FormlessFist = 2513;
        }
        
        public static class Debuffs
        {
            public const short
                Demolish = 246;
        }

        public static class Levels
        {
            public const byte
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                DragonKick = 50;
        }
    }

    internal static class NIN
    {
        public const byte JobID = 30;

        public const uint
            Mug = 2248,
            Hide = 2245,
            ArmorCrush = 3563,
            SpinningEdge = 2240,
            GustSlash = 2242,
            AeolianEdge = 2255,
            HakkeMujinsatsu = 16488,
            DeathBlossom = 2254,
            DreamWithinADream = 3566,
            Assassinate = 2246,
            TenChiJin = 7403,
            Kassatsu = 2264,
            Meisui = 16489,
            TrickAttack = 2258,
            Chi = 2261,
            Jin = 18807;

        public static class Buffs
        {
            public const short
                AssassinateReady = 1955,
                Hidden = 614,
                Kassatsu = 497,
                Suiton = 507;
        }

        public static class Levels
        {
            public const byte
                GustSlash = 4,
                AeolianEdge = 26,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54,
                Meisui = 72,
                EnhancedKassatsu = 76;
        }
    }

    internal static class RDM
    {
        public const byte JobID = 35;

        public const uint
            Verthunder = 7505,
            Veraero = 7507,
            Veraero2 = 16525,
            Verthunder2 = 16524,
            Impact = 16526,
            Redoublement = 7516,
            EnchantedRedoublement = 7529,
            Zwerchhau = 7512,
            EnchantedZwerchhau = 7528,
            Riposte = 7504,
            EnchantedRiposte = 7527,
            Scatter = 7509,
            Vercure = 7514,
            Verstone = 7511,
            Verfire = 7510,
            Jolt = 7503,
            Jolt2 = 7524,
            Verraise = 7523,
            Verholy = 7526,
            Verflare = 7525,
            Scorch = 16530;

        public static class Buffs
        {
            public const short
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249;
        }

        public static class Levels
        {
            public const byte
                Jolt = 2,
                Verthunder = 4,
                Veraero = 10,
                Verraise = 64,
                Zwerchhau = 35,
                Redoublement = 50,
                Vercure = 54,
                Jolt2 = 62,
                Impact = 66,
                Verflare = 68,
                Verholy = 70,
                Scorch = 80;
        }
    }

    internal static class SAM
    {
        public const byte JobID = 34;

        public const uint
            Yukikaze = 7480,
            Hakaze = 7477,
            Gekko = 7481,
            Jinpu = 7478,
            Kasha = 7482,
            Shifu = 7479,
            Mangetsu = 7484,
            Fuga = 7483,
            Oka = 7485,
            Seigan = 7501,
            ThirdEye = 7498,
            Meditate = 7497,
            MeikyoShisui = 7499,
            Iaijutsu = 7867,
            Tsubame = 16483,
            Shoha = 16487,
            Higanbana = 7489,
            Tenka = 7488,
            Midare = 7487,
            TrashHiganbana = 16484;

        public static class Buffs
        {
            public const short
                MeikyoShisui = 1233,
                EyesOpen = 1252,
                Jinpu = 1298,
                Shifu = 1299;
        }

        public static class Levels
        {
            public const byte
                Jinpu = 4,
                Shifu = 18,
                Gekko = 30,
                Mangetsu = 35,
                Kasha = 40,
                Oka = 45,
                Yukikaze = 50,
                Tsubame = 76;
        }
    }

    internal static class SCH
    {
        public const byte JobID = 28;

        public const uint
            FeyBless = 16543,
            Consolation = 16546,
            EnergyDrain = 167,
            Aetherflow = 166;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }

    internal static class SMN
    {
        public const byte JobID = 27;

        public const uint
            Deathflare = 3582,
            EnkindlePhoenix = 16516,
            EnkindleBahamut = 7429,
            DreadwyrmTrance = 3581,
            SummonBahamut = 7427,
            FirebirdTranceLow = 16513,
            FirebirdTranceHigh = 16549,
            Ruin1 = 163,
            Ruin3 = 3579,
            BrandOfPurgatory = 16515,
            FountainOfFire = 16514,
            Fester = 181,
            EnergyDrain = 16508,
            Painflare = 3578,
            EnergySyphon = 16510,
            RuinIV = 7426,
            EgiAssault = 16509,
            EgiAssault2 = 16512,
            Enkindle = 184,
            Inferno = 16803,
            Bio = 164,
            Bio2 = 178,
            Bio3 = 7424,
            Miasma = 168,
            Miasma3 = 7425;

        public static class Buffs
        {
            public const short
                HellishConduit = 1867,
                FurtherRuin = 1212;
        }

        public static class Debuffs
        {
            public const short
                Bio = 179,
                Bio2 = 189,
                Bio3 = 1214,
                Miasma = 180,
                Miasma3 = 1215;
        }

        public static class Levels
        {
            public const byte
                Miasma = 6,
                Bio2 = 26,
                Painflare = 52,
                Ruin3 = 54,
                DoTUpgrade = 66,
                EnhancedEgiAssault = 74,
                EnhancedFirebirdTrance = 80;
        }

        public static class CDs
        {
            // public const byte placeholder = 0;
        }
    }

    internal static class WAR
    {
        public const byte JobID = 21;

        public const uint
            StormsPath = 42,
            HeavySwing = 31,
            Infuriate = 52,
            Maim = 37,
            StormsEye = 45,
            MythrilTempest = 16462,
            Overpower = 41,
            FellCleave = 3549,
            Decimate = 3550,
            InnerBeast = 49,
            SteelCyclone = 51,
            RawIntuition = 3551,
            InnerChaos = 16465,
            ChaoticCyclone = 16463,
            NascentFlash = 16464;

        public static class Buffs
        {
            public const short 
                InnerRelease = 1177,
                NascentChaos = 1897;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                StormsPath = 26,
                MythrilTempest = 40,
                StormsEye = 50,
                FellCleave = 54,
                Decimate = 60,
                MythrilTempestTrait = 74,
                NascentFlash = 76,
                InnerChaos = 80;
        }
    }

    internal static class WHM
    {
        public const byte JobID = 24;

        public const uint
            AfflatusSolace = 16531,
            AfflatusRapture = 16534,
            AfflatusMisery = 16535,
            Cure2 = 135,
            Cure = 120,
            Raise = 125,
            Medica = 124;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Cure2 = 30,
                AfflatusRapture = 76;
        }
    }
}
