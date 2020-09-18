namespace XIVComboPlugin
{
    public static class AST
    {
        public const uint
            Play = 17055,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }

    public static class BLM
    {
        public const uint
            Enochian = 3575,
            Blizzard4 = 3576,
            Fire4 = 3577,
            Transpose = 149,
            UmbralSoul = 16506,
            LeyLines = 3573,
            BetweenTheLines = 7419;

        public static class Buffs
        {
            public const short
                LeyLines = 737;
        }

        public static class Levels
        {
            public const byte
                Blizzard4 = 58,
                Fire4 = 60,
                BetweenTheLines = 62,
                UmbralSoul = 76;
        }
    }

    public static class BRD
    {
        public const uint
            WanderersMinuet = 3559,
            PitchPerfect = 7404,
            HeavyShot = 97,
            BurstShot = 16495,
            StraightShot = 98,
            RefulgentArrow = 7409;

        public static class Buffs
        {
            public const short
                StraightShotReady = 122;
        }

        public static class Levels
        {
            public const byte
                RefulgentArrow = 70,
                BurstShot = 76;
        }
    }

    public static class DoM
    {
        // public const uint placeholder = 0;

        public static class Buffs
        {
            public const short
                Swiftcast = 167;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }

    public static class DNC
    {
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

    public static class DRG
    {
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

    public static class DRK
    {
        public const uint
            Souleater = 3632,
            HardSlash = 3617,
            SyphonStrike = 3623,
            StalwartSoul = 16468,
            Unleash = 3621;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                SyphonStrike = 2,
                Souleater = 26,
                StalwartSoul = 72;
        }
    }

    public static class GNB
    {
        public const uint
            SolidBarrel = 16145,
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
                ReadyToRip = 1842,
                ReadyToTear = 1843,
                ReadyToGouge = 1844;
        }

        public static class Levels
        {
            public const byte
                BrutalShell = 4,
                SolidBarrel = 26,
                DemonSlaughter = 40,
                Continuation = 70,
                FatedCircle = 72;
        }
    }

    public static class PLD
    {
        public const uint
            GoringBlade = 3538,
            FastBlade = 9,
            RiotBlade = 15,
            RoyalAuthority = 3539,
            RageOfHalone = 21,
            Prominence = 16457,
            TotalEclipse = 7381,
            Requiescat = 7383,
            Confiteor = 16459;

        public static class Buffs
        {
            public const short
                Requiescat = 1368;
        }

        public static class Levels
        {
            public const byte
                RiotBlade = 4,
                RageOfHalone = 26,
                Prominence = 40,
                GoringBlade = 54,
                RoyalAuthority = 60,
                Confiteor = 80;
        }
    }

    public static class MCH
    {
        public const uint
            CleanShot = 2873,
            HeatedCleanShot = 7413,
            SplitShot = 2866,
            HeatedSplitShot = 7411,
            SlugShot = 2868,
            HeatedSlugshot = 7412,
            Hypercharge = 17209,
            HeatBlast = 7410,
            SpreadShot = 2870,
            AutoCrossbow = 16497;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                SlugShot = 2,
                CleanShot = 26,
                HeatBlast = 35,
                AutoCrossbow = 52,
                HeatedSplitShot = 54,
                HeatedSlugshot = 60,
                HeatedCleanShot = 64;
        }
    }

    public static class MNK
    {
        public const uint
            ArmOfTheDestroyer = 62,
            FourPointFury = 16473,
            Rockbreaker = 70;

        public static class Buffs
        {
            public const short
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }

    public static class NIN
    {
        public const uint
            ArmorCrush = 3563,
            SpinningEdge = 2240,
            GustSlash = 2242,
            AeolianEdge = 2255,
            HakkeMujinsatsu = 16488,
            DeathBlossom = 2254,
            DreamWithinADream = 3566,
            Assassinate = 2246;

        public static class Buffs
        {
            public const short
                AssassinateReady = 1955;
        }

        public static class Levels
        {
            public const byte
                GustSlash = 4,
                AeolianEdge = 26,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54;
        }
    }

    public static class RDM
    {
        public const uint
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
            Verstone = 7511,
            Verfire = 7510,
            Jolt = 7503,
            Jolt2 = 7524,
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
                Zwerchhau = 35,
                Redoublement = 50,
                Jolt2 = 62,
                Impact = 66,
                Scorch = 80;
        }
    }

    public static class SAM
    {
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
            ThirdEye = 7498;

        public static class Buffs
        {
            public const short
                MeikyoShisui = 1233,
                EyesOpen = 1252;
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
                Yukikaze = 50;
        }
    }

    public static class SCH
    {
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

    public static class SMN
    {
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
            EnergySyphon = 16510;

        public static class Buffs
        {
            public const short
                HellishConduit = 1867;
        }

        public static class Levels
        {
            public const byte
                Painflare = 52,
                Ruin3 = 54,
                EnhancedFirebirdTrance = 80;
        }
    }

    public static class WAR
    {
        public const uint
            StormsPath = 42,
            HeavySwing = 31,
            Maim = 37,
            StormsEye = 45,
            MythrilTempest = 16462,
            Overpower = 41;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Maim = 4,
                StormsPath = 26,
                MythrilTempest = 40,
                StormsEye = 50;
        }
    }

    public static class WHM
    {
        public const uint
            Solace = 16531,
            Rapture = 16534,
            Misery = 16535;

        public static class Buffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            // public const byte placeholder = 0;
        }
    }
}
