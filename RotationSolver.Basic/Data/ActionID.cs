namespace RotationSolver.Basic.Data;

/// <summary>
/// The id of the action
/// </summary>
public enum ActionID : uint
{
    None = 0,

    #region Astrologian
    Ascend = 3603,

    Malefic = 3596,

    Combust = 3599,

    Gravity = 3615,

    Benefic = 3594,

    Benefic2 = 3610,

    AspectedBenefic = 3595,

    EssentialDignity = 3614,

    Synastry = 3612,

    CelestialIntersection = 16556,

    Exaltation = 25873,

    Helios = 3600,

    AspectedHelios = 3601,

    CelestialOpposition = 16553,

    EarthlyStar = 7439,

    CollectiveUnconscious = 3613,

    Horoscope = 16557,

    Lightspeed = 3606,

    NeutralSect = 16559,

    Macrocosmos = 25874,

    Astrodyne = 25870,

    Divination = 16552,

    Draw = 3590,

    Redraw = 3593,

    MinorArcana = 7443,

    CrownPlay = 25869,

    Balance = 4401,

    Arrow = 4402,

    Spear = 4403,

    Bole = 4404,

    Ewer = 4405,

    Spire = 4406,
    #endregion

    #region BlackMage
    Thunder = 144,

    Thunder2 = 7447,

    Thunder3 = 153,

    Thunder4 = 7420,

    Transpose = 149,

    UmbralSoul = 16506,

    Manaward = 157,

    Manafont = 158,

    SharpCast = 3574,

    TripleCast = 7421,

    LeyLines = 3573,

    BetweenTheLines = 7419,

    AetherialManipulation = 155,

    Amplifier = 25796,

    Flare = 162,

    Despair = 16505,

    Foul = 7422,

    Xenoglossy = 16507,

    Scathe = 156,

    Paradox = 25797,

    Fire = 141,

    Fire2 = 147,

    Fire3 = 152,

    Fire4 = 3577,

    Blizzard = 142,

    Blizzard2 = 25793,

    Blizzard3 = 154,

    Blizzard4 = 3576,

    Freeze = 159,
    #endregion

    #region BlueMage
    WaterCannon = 11385,

    FlameThrower = 11402,

    AquaBreath = 11390,

    FlyingFrenzy = 11389,

    DrillCannons = 11398,

    HighVoltage = 11387,

    Loom = 11401,

    FinalSting = 11407,

    SongOfTorment = 11386,

    Glower = 11404,

    PlainCracker = 11391,

    Bristle = 11393,

    WhiteWind = 11406,

    Level5Petrify = 11414,

    SharpenedKnife = 11400,

    IceSpikes = 11418,

    BloodDrain = 11395,

    AcornBomb = 11392,

    BombToss = 11396,

    OffGuard = 11411,

    SelfDestruct = 11408,

    Faze = 11403,

    FlyingSardine = 11423,

    Snort = 11383,

    Weight4Tonze = 11384,

    TheLook = 11399,

    BadBreath = 11388,

    Diamondback = 11424,

    MightyGuard = 11417,

    StickyTongue = 11412,

    ToadOil = 11410,

    TheRamVoice = 11419,

    TheDragonVoice = 11420,

    Missile = 11405,

    Needles1000 = 11397,

    InkJet = 11422,

    FireAngon = 11425,

    MoonFlute = 11415,

    TailScrew = 11413,

    MindBlast = 11394,

    Doom = 11416,

    PeculiarLight = 11421,

    FeatherRain = 11426,

    Eruption = 11427,

    MountainBusterBLU = 11428,

    ShockStrike = 11429,

    GlassDance = 11430,

    VeilOfTheWhorl = 11431,

    AlpineDraft = 18295,

    ProteanWave = 18296,

    Northerlies = 18297,

    Electrogenesis = 18298,

    Kaltstrahl = 18299,

    AbyssalTransfixion = 18300,

    Chirp = 18301,

    EerieSoundwave = 18302,

    PomCure = 18303,

    Gobskin = 18304,

    MagicHammer = 18305,

    Avail = 18306,

    FrogLegs = 18307,

    SonicBoom = 18308,

    Whistle = 18309,

    WhiteKnightsTour = 18310,

    BlackKnightsTour = 18311,

    Level5Death = 18312,

    Launcher = 18313,

    PerpetualRay = 18314,

    CactGuard = 18315,

    RevengeBlast = 18316,

    AngelWhisper = 18317,

    Exuviation = 18318,

    Reflux = 18319,

    Devour = 18320,

    CondensedLibra = 18321,

    AethericMimicry = 18322,

    Surpanakha = 18323,

    Quasar = 18324,

    JKick = 18325,

    TripleTrident = 23264,

    Tingle = 23265,

    Tatamigaeshi = 23266,

    ColdFog = 23267,

    Stotram = 23269,

    SaintlyBeam = 23270,

    FeculentFlood = 23271,

    AngelsSnack = 23272,

    ChelonianGate = 23273,

    TheRoseOfDestruction = 23275,

    BasicInstinct = 23276,

    UltraVibration = 23277,

    Blaze = 23278,

    MustardBomb = 23279,

    DragonForce = 23280,

    AetherialSpark = 23281,

    HydroPull = 23282,

    MaledictionOfWater = 23283,

    ChocoMeteor = 23284,

    MatraMagic = 23285,

    PeripheralSynthesis = 23286,

    BothEnds = 23287,

    PhantomFlurry = 23288,

    NightBloom = 23290,

    WhiteDeath = 23268,

    DivineCataract = 23274,

    PhantomFlurry2 = 23289,
    #endregion

    #region Bard
    HeavyShoot = 97,

    StraitShoot = 98,

    VenomousBite = 100,

    WindBite = 113,

    IronJaws = 3560,

    MagesBallad = 114,

    ArmysPaeon = 116,

    WanderersMinuet = 3559,

    BattleVoice = 118,

    RagingStrikes = 101,

    RadiantFinale = 25785,

    Barrage = 107,

    EmpyrealArrow = 3558,

    PitchPerfect = 7404,

    Bloodletter = 110,

    RainOfDeath = 117,

    QuickNock = 106,

    ShadowBite = 16494,

    WardensPaean = 3561,

    NaturesMinne = 7408,

    Sidewinder = 3562,

    ApexArrow = 16496,

    BlastArrow = 25784,

    Troubadour = 7405,
    #endregion

    #region Dancer
    Cascade = 15989,

    Fountain = 15990,

    ReverseCascade = 15991,

    FountainFall = 15992,

    FanDance = 16007,

    Windmill = 15993,

    BladeShower = 15994,

    RisingWindmill = 15995,

    BloodShower = 15996,

    FanDance2 = 16008,

    FanDance3 = 16009,

    FanDance4 = 25791,

    SaberDance = 16005,

    StarFallDance = 25792,

    EnAvant = 16010,

    Emboite = 15999,

    Entrechat = 16000,

    Jete = 16001,

    Pirouette = 16002,

    StandardStep = 15997,

    TechnicalStep = 15998,

    StandardFinish = 16003,

    TechnicalFinish = 16004,

    ShieldSamba = 16012,

    CuringWaltz = 16015,

    ClosedPosition = 16006,

    Devilment = 16011,

    Flourish = 16013,

    Improvisation = 16014,

    Improvised = 25789,

    Tillana = 25790,
    #endregion

    #region Dragoon
    TrueThrust = 75,

    VorpalThrust = 78,

    RaidenThrust = 16479,

    FullThrust = 84,

    HeavensThrust = 25771,

    Disembowel = 87,

    ChaosThrust = 88,

    ChaoticSpring = 25772,

    WheelingThrust = 3556,

    FangandClaw = 3554,

    PiercingTalon = 90,

    DoomSpike = 86,

    SonicThrust = 7397,

    DraconianFury = 25770,

    CoerthanTorment = 16477,

    SpineShatterDive = 95,

    DragonFireDive = 96,

    Jump = 92,

    HighJump = 16478,

    MirageDive = 7399,

    Geirskogul = 3555,

    Nastrond = 7400,

    StarDiver = 16480,

    WyrmwindThrust = 25773,

    ElusiveJump = 94,
    #endregion

    #region DRK
    HardSlash = 3617,

    SyphonStrike = 3623,

    Unleash = 3621,

    Grit = 3629,

    Unmend = 3624,

    Souleater = 3632,

    FloodOfDarkness = 16466,

    EdgeOfDarkness = 16467,

    BloodWeapon = 3625,

    ShadowWall = 3636,

    DarkMind = 3634,

    LivingDead = 3638,

    SaltedEarth = 3639,

    Plunge = 3640,

    AbyssalDrain = 3641,

    CarveAndSpit = 3643,

    BloodSpiller = 7392,

    Quietus = 7391,

    Delirium = 7390,

    TheBlackestNight = 7393,

    StalwartSoul = 16468,

    DarkMissionary = 16471,

    LivingShadow = 16472,

    Oblation = 25754,

    LifeSurge = 83,

    LanceCharge = 85,

    DragonSight = 7398,

    BattleLitany = 3557,

    ShadowBringer = 25757,

    SaltandDarkness = 25755,
    #endregion

    #region GNB
    RoyalGuard = 16142,

    KeenEdge = 16137,

    NoMercy = 16138,

    BrutalShell = 16139,

    Camouflage = 16140,

    DemonSlice = 16141,

    LightningShot = 16143,

    DangerZone = 16144,

    SolidBarrel = 16145,

    BurstStrike = 16162,

    Nebula = 16148,

    DemonSlaughter = 16149,

    Aurora = 16151,

    SuperBolide = 16152,

    SonicBreak = 16153,

    RoughDivide = 16154,

    GnashingFang = 16146,

    BowShock = 16159,

    HeartOfLight = 16160,

    HeartOfStone = 16161,

    FatedCircle = 16163,

    BloodFest = 16164,

    DoubleDown = 25760,

    SavageClaw = 16147,

    WickedTalon = 16150,

    Continuation = 16155,

    JugularRip = 16156,

    AbdomenTear = 16157,

    EyeGouge = 16158,

    Hypervelocity = 25759,
    #endregion

    #region MCH
    SplitShot = 2866,

    HeatedSplitShot = 7411,

    SlugShot = 2868,

    HeatedSlugShot = 7412,

    CleanShot = 2873,

    HeatBlast = 7410,

    SpreadShot = 2870,

    AutoCrossbow = 16497,

    HotShot = 2872,

    AirAnchor = 16500,

    Drill = 16498,

    ChainSaw = 25788,

    BioBlaster = 16499,

    Reassemble = 2876,

    Hypercharge = 17209,

    Wildfire = 2878,

    GaussRound = 2874,

    Ricochet = 2890,

    BarrelStabilizer = 7414,

    RookAutoturret = 2864,

    Tactician = 16889,

    Dismantle = 2887,
    #endregion

    #region MNK
    DragonKick = 74,

    BootShine = 53,

    ArmOfTheDestroyer = 62,

    ShadowOfTheDestroyer = 25767,

    TwinSnakes = 61,

    TrueStrike = 54,

    FourPointFury = 16473,

    Demolish = 66,

    SnapPunch = 56,

    RockBreaker = 70,

    Meditation = 3546,

    SteelPeak = 25761,

    HowlingFist = 25763,

    Brotherhood = 7396,

    RiddleOfFire = 7395,

    Thunderclap = 25762,

    Mantra = 65,

    PerfectBalance = 69,

    ElixirField = 3545,

    FlintStrike = 25882,

    CelestialRevolution = 25765,

    RisingPhoenix = 25768,

    TornadoKick = 3543,

    PhantomRush = 25769,

    FormShift = 4262,

    RiddleOfEarth = 7394,

    RiddleOfWind = 25766,
    #endregion

    #region NIN
    Hide = 2245,

    SpinningEdge = 2240,

    ShadeShift = 2241,

    GustSlash = 2242,

    ThrowingDagger = 2247,

    Mug = 2248,

    TrickAttack = 2258,

    AeolianEdge = 2255,

    DeathBlossom = 2254,

    Ten = 2259,

    Ninjutsu = 2260,

    Chi = 2261,

    Jin = 2263,

    TenChiJin = 7403,

    Shukuchi = 2262,

    Assassinate = 2246,

    Meisui = 16489,

    Kassatsu = 2264,

    HakkeMujinsatsu = 16488,

    ArmorCrush = 3563,

    HellFrogMedium = 7401,

    Bhavacakra = 7402,

    Bunshin = 16493,

    PhantomKamaitachi = 25774,

    FleetingRaiju = 25778,

    ForkedRaiju = 25777,

    Huraijin = 25876,

    DreamWithInADream = 3566,

    FumaShurikenTen = 18873,

    FumaShurikenJin = 18875,

    KatonTen = 18876,

    RaitonChi = 18877,

    DotonChi = 18880,

    SuitonJin = 18881,

    RabbitMedium = 2272,

    FumaShuriken = 2265,

    Katon = 2266,

    Raiton = 2267,

    Hyoton = 2268,

    Huton = 2269,

    Doton = 2270,

    Suiton = 2271,

    GokaMekkyaku = 16491,

    HyoshoRanryu = 16492,
    #endregion

    #region PLD
    IronWill = 28,

    Bulwark = 22,

    FastBlade = 9,

    RiotBlade = 15,

    GoringBlade = 3538,

    RageOfHalone = 21,

    RoyalAuthority = 3539,

    ShieldLob = 24,

    ShieldBash = 16,

    FightOrFlight = 20,

    TotalEclipse = 7381,

    Prominence = 16457,

    Sentinel = 17,

    CircleOfScorn = 23,

    SpiritsWithin = 29,

    HallowedGround = 30,

    DivineVeil = 3540,

    Clemency = 3541,

    Intervention = 7382,

    Intervene = 16461,

    Atonement = 16460,

    Expiacion = 25747,

    BladeOfValor = 25750,

    BladeOfTruth = 25749,

    BladeOfFaith = 25748,

    Requiescat = 7383,

    Confiteor = 16459,

    HolyCircle = 16458,

    HolySpirit = 7384,

    PassageOfArms = 7385,

    Cover = 27,

    Sheltron = 3542,
    #endregion

    #region RDM
    Verraise = 7523,

    Jolt = 7503,

    Riposte = 7504,

    Verthunder = 7505,

    CorpsACorps = 7506,

    Veraero = 7507,

    Scatter = 7509,

    Verthunder2 = 16524,

    Veraero2 = 16525,

    Verfire = 7510,

    Verstone = 7511,

    Zwerchhau = 7512,

    Engagement = 16527,

    Fleche = 7517,

    Redoublement = 7516,

    Acceleration = 7518,

    Moulinet = 7513,

    Vercure = 7514,

    ContreSixte = 7519,

    Embolden = 7520,

    Reprise = 16529,

    MagickBarrier = 25857,

    Verflare = 7525,

    Verholy = 7526,

    Scorch = 16530,

    Resolution = 25858,

    Manafication = 7521,
    #endregion

    #region RPR
    Slice = 24373,

    WaxingSlice = 24374,

    InfernalSlice = 24375,

    ShadowOfDeath = 24378,

    SoulSlice = 24380,

    SpinningScythe = 24376,

    NightmareScythe = 24377,

    WhorlOfDeath = 24379,

    SoulScythe = 24381,

    Gibbet = 24382,

    Gallows = 24383,

    Guillotine = 24384,

    BloodStalk = 24389,

    GrimSwathe = 24392,

    Gluttony = 24393,

    ArcaneCircle = 24405,

    PlentifulHarvest = 24385,

    Enshroud = 24394,

    Communio = 24398,

    LemuresSlice = 24399,

    LemuresScythe = 24400,

    HellsIngress = 24401,

    HellsEgress = 24402,

    VoidReaping = 24395,

    CrossReaping = 24396,

    GrimReaping = 24397,

    Harpe = 24386,

    SoulSow = 24387,

    HarvestMoon = 24388,

    ArcaneCrest = 24404,
    #endregion

    #region SAM
    Hakaze = 7477,

    Jinpu = 7478,

    ThirdEye = 7498,

    Enpi = 7486,

    Shifu = 7479,

    Fuga = 7483,

    Fuko = 25780,

    Gekko = 7481,

    Hagakure = 7495,

    Higanbana = 7489,

    TenkaGoken = 7488,

    MidareSetsugekka = 7487,

    Mangetsu = 7484,

    Kasha = 7482,

    Oka = 7485,

    MeikyoShisui = 7499,

    Yukikaze = 7480,

    HissatsuGyoten = 7492,

    HissatsuYaten = 7493,

    HissatsuShinten = 7490,

    HissatsuKyuten = 7491,

    Ikishoten = 16482,

    HissatsuGuren = 7496,

    HissatsuSenei = 16481,

    TsubameGaeshi = 16483,

    KaeshiGoken = 16485,

    KaeshiSetsugekka = 16486,

    Shoha = 16487,

    Shoha2 = 25779,

    OgiNamikiri = 25781,

    KaeshiNamikiri = 25782,
    #endregion

    #region SCH
    Physick = 190,

    Adloquium = 185,

    Resurrection = 173,

    Succor = 186,

    Lustrate = 189,

    SacredSoil = 188,

    Indomitability = 3583,

    Excogitation = 7434,

    Consolation = 16546,

    Protraction = 25867,

    Bio = 17864,

    Ruin = 17869,

    Ruin2 = 17870,

    EnergyDrain = 167,

    ArtOfWar = 16539,

    SummonSeraph = 16545,

    SummonEos = 17215,

    WhisperingDawn = 16537,

    FeyIllumination = 16538,

    Dissipation = 3587,

    Aetherpact = 7437,

    FeyBlessing = 16543,

    Aetherflow = 166,

    Recitation = 16542,

    ChainStratagem = 7436,

    DeploymentTactics = 3585,

    EmergencyTactics = 3586,

    Expedient = 25868,
    #endregion

    #region SGE
    Egeiro = 24287,

    Dosis = 24283,

    EukrasianDosis = 24283,

    Phlegma = 24289,

    Phlegma2 = 24307,

    Phlegma3 = 24313,

    Diagnosis = 24284,

    Kardia = 24285,

    Prognosis = 24286,

    Physis = 24288,

    Physis2 = 24302,

    Eukrasia = 24290,

    Soteria = 24294,

    Icarus = 24295,

    Druochole = 24296,

    Dyskrasia = 24297,

    Kerachole = 24298,

    Ixochole = 24299,

    Zoe = 24300,

    Taurochole = 24303,

    Toxikon = 24304,

    Haima = 24305,

    EukrasianDiagnosis = 24291,

    EukrasianPrognosis = 24292,

    Rhizomata = 24309,

    Holos = 24310,

    Panhaima = 24311,

    Krasis = 24317,

    Pneuma = 24318,

    Pepsis = 24301,
    #endregion

    #region SMN
    Gemshine = 25883,

    PreciousBrilliance = 25884,

    RuinSMN = 163,

    Outburst = 16511,

    SummonCarbuncle = 25798,

    SearingLight = 25801,

    RadiantAegis = 25799,

    PhysickSMN = 16230,

    AetherCharge = 25800,

    SummonBahamut = 7427,

    SummonRuby = 25802,

    SummonTopaz = 25803,

    SummonEmerald = 25804,

    ResurrectionSMN = 173,

    EnergyDrainSMN = 16508,

    EnergySiphon = 16510,

    FountainOfFire = 16514,

    Fester = 181,

    PainFlare = 3578,

    RuinIV = 7426,

    AstralImpulse = 25820,

    AstralFlow = 25822,

    EnkindleBahamut = 7429,

    DreadwyrmTrance = 3581,

    DeathFlare = 3582,

    Rekindle = 25830,

    CrimsonCyclone = 25835,

    CrimsonStrike = 25885,

    MountainBuster = 25836,

    Slipstream = 25837,
    #endregion

    #region WAR
    Defiance = 48,

    HeavySwing = 31,

    Maim = 37,

    StormsPath = 42,

    StormsEye = 45,

    Tomahawk = 46,

    Onslaught = 7386,

    Upheaval = 7387,

    Overpower = 41,

    MythrilTempest = 16462,

    Orogeny = 25752,

    InnerBeast = 49,

    InnerRelease = 7389,

    SteelCyclone = 51,

    Infuriate = 52,

    Berserk = 38,

    ThrillOfBattle = 40,

    Equilibrium = 3552,

    NascentFlash = 16464,

    BloodWhetting = 25751,

    Vengeance = 44,

    RawIntuition = 3551,

    ShakeItOff = 7388,

    Holmgang = 43,

    PrimalRend = 25753,
    #endregion

    #region WHM
    Cure = 120,

    Medica = 124,

    Raise1 = 125,

    Cure2 = 135,

    Medica2 = 133,

    Regen = 137,

    Cure3 = 131,

    Benediction = 140,

    Asylum = 3569,

    AfflatusSolace = 16531,

    Tetragrammaton = 3570,

    DivineBenison = 7432,

    AfflatusRapture = 16534,

    Aquaveil = 25861,

    LiturgyOfTheBell = 25862,

    Stone = 119,

    Aero = 121,

    Holy = 139,

    Assize = 3571,

    AfflatusMisery = 16535,

    PresenseOfMind = 136,

    ThinAir = 7430,

    PlenaryIndulgence = 7433,

    Temperance = 16536,
    #endregion

    #region General
    Addle = 7560,

    SwiftCast = 7561,

    Esuna = 7568,

    Rescue = 7571,

    Repose = 16560,

    LucidDreaming = 7562,

    SecondWind = 7541,

    ArmsLength = 7548,

    Rampart = 7531,

    Provoke = 7533,

    Reprisal = 7535,

    Shirk = 7537,

    Bloodbath = 7542,

    Feint = 7549,

    Interject = 7538,

    LowBlow = 7540,

    LegSweep = 7863,

    HeadGraze = 7551,

    SureCast = 7559,

    TrueNorth = 7546,

    Peloton = 7557,
    #endregion
}
