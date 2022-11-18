namespace XIVAutoAttack.Data
{
    internal enum StatusID : ushort
    {
        /// <summary>
        /// 雷1
        /// </summary>
        Thunder = 161,

        /// <summary>
        /// 雷2
        /// </summary>
        Thunder2 = 162,

        /// <summary>
        /// 雷3
        /// </summary>
        Thunder3 = 163,

        /// <summary>
        /// 雷4
        /// </summary>
        Thunder4 = 1210,

        /// <summary>
        /// 昏乱
        /// </summary>
        Addle = 1203,


        /// <summary>
        /// 昏乱
        /// </summary>
        Feint = 1195,

        #region WHM
        /// <summary>
        /// 医济
        /// </summary>
        Medica2 = 150,

        /// <summary>
        /// 医济
        /// </summary>
        TrueMedica2 = 2792,

        /// <summary>
        /// 再生
        /// </summary>
        Regen1 = 158,

        /// <summary>
        /// 再生
        /// </summary>
        Regen2 = 897,

        /// <summary>
        /// 再生
        /// </summary>
        Regen3 = 1330,

        /// <summary>
        /// 神速咏唱
        /// </summary>
        PresenceOfMind = 157,

        /// <summary>
        /// 无中生有
        /// </summary>
        ThinAir = 1217,

        /// <summary>
        /// 神祝祷
        /// </summary>
        DivineBenison = 1218,

        /// <summary>
        /// 疾风
        /// </summary>
        Aero = 143,

        /// <summary>
        /// 烈风
        /// </summary>
        Aero2 = 144,

        /// <summary>
        /// 天辉
        /// </summary>
        Dia = 1871,
        #endregion
        ///// <summary>
        ///// 即刻咏唱
        ///// </summary>
        //Swiftcast1 = 1325,

        ///// <summary>
        ///// 即刻咏唱
        ///// </summary>
        //Swiftcast2 = 1987,

        /// <summary>
        /// 即刻咏唱
        /// </summary>
        Swiftcast = 167,

        /// <summary>
        /// 连续咏唱
        /// </summary>
        Dualcast = 1249,

        /// <summary>
        /// 三连咏唱
        /// </summary>
        Triplecast = 1211,

        /// <summary>
        /// 激情咏唱
        /// </summary>
        Sharpcast = 867,

        /// <summary>
        /// 雷云
        /// </summary>
        Thundercloud = 164,

        /// <summary>
        /// 核爆效果提高
        /// </summary>
        EnhancedFlare = 2960,

        /// <summary>
        /// 黑魔纹
        /// </summary>
        LeyLines = 737,

        /// <summary>
        /// 火苗
        /// </summary>
        Firestarter = 165,

        /// <summary>
        /// 复活
        /// </summary>
        Raise = 148,

        /// <summary>
        /// 止步
        /// </summary>
        Bind1 = 13,
        Bind2 = 1345,

        /// <summary>
        /// 赤火炎预备
        /// </summary>
        VerfireReady = 1234,

        /// <summary>
        /// 赤飞石预备
        /// </summary>
        VerstoneReady = 1235,

        /// <summary>
        /// 促进
        /// </summary>
        Acceleration = 1238,

        /// <summary>
        /// 死亡宣告，可以被解除的！
        /// </summary>
        Doom = 910,

        /// <summary>
        /// 神圣领域
        /// </summary>
        HallowedGround = 82,

        /// <summary>
        /// 死斗
        /// </summary>
        Holmgang = 409,

        /// <summary>
        /// 行尸走肉
        /// </summary>
        WillDead = 810,

        /// <summary>
        /// 死而不僵
        /// </summary>
        WalkingDead = 811,

        /// <summary>
        /// 超火流行
        /// </summary>
        Superbolide = 1836,

        /// <summary>
        /// 直线射击预备
        /// </summary>
        StraightShotReady = 122,

        /// <summary>
        /// 毒咬箭Dot
        /// </summary>
        VenomousBite = 124,

        /// <summary>
        /// 风蚀箭Dot
        /// </summary>
        Windbite = 129,

        /// <summary>
        /// 毒咬箭 Plus Dot
        /// </summary>
        CausticBite = 1200,

        /// <summary>
        /// 风蚀箭 Plus Dot
        /// </summary>
        Stormbite = 1201,

        /// <summary>
        /// 影噬箭准备
        /// </summary>
        ShadowbiteReady = 3002,

        /// <summary>
        /// 爆破箭准备
        /// </summary>
        BlastArrowReady = 2692,

        /// <summary>
        /// 猛者强击
        /// </summary>
        RagingStrikes = 125,

        /// <summary>
        /// 战斗之声
        /// </summary>
        BattleVoice = 141,

        /// <summary>
        /// 光明神的最终乐章
        /// </summary>
        RadiantFinale = 2722,

        /// <summary>
        /// 军神的契约
        /// </summary>
        ArmyEthos = 1933,

        /// <summary>
        /// 铁壁
        /// </summary>
        Rampart1 = 71,
        Rampart2 = 1191,
        Rampart3 = 1978,

        /// <summary>
        /// 复仇
        /// </summary>
        Vengeance = 89,

        /// <summary>
        /// 守护 盾姿
        /// </summary>
        Defiance = 91,

        /// <summary>
        /// 原初的直觉
        /// </summary>
        RawIntuition = 735,

        /// <summary>
        /// 原初的血气
        /// </summary>
        Bloodwhetting = 2678,

        /// <summary>
        /// 原初的解放
        /// </summary>
        InnerRelease = 1177,

        /// <summary>
        /// 原初的混沌
        /// </summary>
        NascentChaos = 1897,

        /// <summary>
        /// 战场风暴
        /// </summary>
        SurgingTempest = 2677,

        /// <summary>
        /// 蛮荒崩裂预备
        /// </summary>
        PrimalRendReady = 2624,

        /// <summary>
        /// 烧灼Dot
        /// </summary>
        Combust = 838,
        Combust2 = 843,
        Combust3 = 1881,
        Combust4 = 2041,

        /// <summary>
        /// 吉星相位
        /// </summary>
        AspectedBenefic = 835,

        /// <summary>
        /// 阳星相位
        /// </summary>
        AspectedHelios = 836,

        /// <summary>
        /// 天星交错
        /// </summary>
        Intersection = 1889,

        /// <summary>
        /// 天宫图
        /// </summary>
        Horoscope = 1890,

        /// <summary>
        /// 阳星天宫图
        /// </summary>
        HoroscopeHelios = 1891,

        /// <summary>
        /// 光速
        /// </summary>
        LightSpeed = 841,

        /// <summary>
        /// 可以重抽
        /// </summary>
        ClarifyingDraw = 2713,

        /// <summary>
        /// 六个Buff
        /// </summary>
        TheBalance = 1882,
        TheBole = 1883,
        TheArrow = 1884,
        TheSpear = 1885,
        TheEwer = 1886,
        TheSpire = 1887,

        /// <summary>
        /// 地星主宰
        /// </summary>
        EarthlyDominance = 1224,

        /// <summary>
        /// 巨星主宰
        /// </summary>
        GiantDominance = 1248,

        /// <summary>
        /// 行吟
        /// </summary>
        Troubadour = 1934,

        /// <summary>
        /// 策动
        /// </summary>
        Tactician1 = 1951,

        /// <summary>
        /// 策动
        /// </summary>
        Tactician2 = 2177,

        /// <summary>
        /// 防守之桑巴
        /// </summary>
        ShieldSamba = 1826,

        /// <summary>
        /// 死亡烙印
        /// </summary>
        DeathsDesign = 2586,

        //妖异之镰
        SoulReaver = 2587,

        /// <summary>
        /// 绞决效果提高
        /// </summary>
        EnhancedGibbet = 2588,
        EnhancedGallows = 2589,
        EnhancedVoidReaping = 2590,
        EnhancedCrossReaping = 2591,

        /// <summary>
        /// 夜游魂
        /// </summary>
        Enshrouded = 2593,

        /// <summary>
        /// 祭祀环
        /// </summary>
        CircleofSacrifice = 2972,

        /// <summary>
        /// 死亡祭品
        /// </summary>
        ImmortalSacrifice = 2592,

        /// <summary>
        /// 死亡祭祀
        /// </summary>
        BloodsownCircle = 2972,

        /// <summary>
        /// 神秘环
        /// </summary>
        ArcaneCircle = 2599,

        /// <summary>
        /// 播魂种
        /// </summary>
        Soulsow = 2594,

        /// <summary>
        /// 回退准备
        /// </summary>
        Threshold = 2595,

        /// <summary>
        /// 勾刃效果提高
        /// </summary>
        EnhancedHarpe = 2845,

        /// <summary>
        /// 龙牙龙爪预备状态
        /// </summary>
        SharperFangandClaw = 802,

        /// <summary>
        /// 龙尾大回旋预备状态
        /// </summary>
        EnhancedWheelingThrust = 803,

        /// <summary>
        /// 龙枪状态
        /// </summary>
        PowerSurge = 2720,

        /// <summary>
        /// 龙剑状态
        /// </summary>
        LifeSurge = 2175,

        /// <summary>
        /// 猛枪
        /// </summary>
        LanceCharge = 1864,

        /// <summary>
        /// 幻象冲预备状态
        /// </summary>
        DiveReady = 1243,

        /// <summary>
        /// 巨龙右眼
        /// </summary>
        RightEye = 1910,

        /// <summary>
        /// 魔猿形
        /// </summary>
        OpoOpoForm = 107,

        /// <summary>
        /// 盗龙形
        /// </summary>
        RaptorForm = 108,

        /// <summary>
        /// 猛豹形
        /// </summary>
        CoerlForm = 109,

        /// <summary>
        /// 连击效果提高
        /// </summary>
        LeadenFist = 1861,

        /// <summary>
        /// 功力
        /// </summary>
        DisciplinedFist = 3001,

        /// <summary>
        /// 破碎拳
        /// </summary>
        Demolish = 246,

        /// <summary>
        /// 震脚
        /// </summary>
        PerfectBalance = 110,

        /// <summary>
        /// 无相身形
        /// </summary>
        FormlessFist = 2513,

        /// <summary>
        /// 对称投掷
        /// </summary>
        SilkenSymmetry = 2693,
        SilkenSymmetry2 = 3017,

        /// <summary>
        /// 非对称投掷
        /// </summary>
        SilkenFlow = 2694,
        SilkenFlow2 = 3018,

        /// <summary>
        /// 扇舞·急
        /// </summary>
        ThreefoldFanDance = 1820,

        /// <summary>
        /// 扇舞·终
        /// </summary>
        FourfoldFanDance = 2699,

        /// <summary>
        /// 流星舞预备
        /// </summary>
        FlourishingStarfall = 2700,

        /// <summary>
        /// 标准舞步
        /// </summary>
        StandardStep = 1818,

        /// <summary>
        /// 标准舞步结束
        /// </summary>
        StandardFinish = 1821,

        /// <summary>
        /// 技巧舞步结束
        /// </summary>
        TechnicalFinish = 1822,

        /// <summary>
        /// 技巧舞步
        /// </summary>
        TechnicalStep = 1819,

        /// <summary>
        /// 闭式舞姿
        /// </summary>
        ClosedPosition1 = 1823,

        /// <summary>
        /// 闭式舞姿
        /// </summary>
        ClosedPosition2 = 2026,

        /// <summary>
        /// 进攻之探戈
        /// </summary>
        Devilment = 1825,

        /// <summary>
        /// 提拉纳预备
        /// </summary>
        FlourishingFinish = 2698,

        /// <summary>
        /// 虚弱
        /// </summary>
        Weakness = 43,

        /// <summary>
        /// 濒死
        /// </summary>
        BrinkofDeath = 44,

        /// <summary>
        /// 心关
        /// </summary>
        Kardia = 2604,

        /// <summary>
        /// 关心
        /// </summary>
        Kardion = 2605,

        /// <summary>
        /// 均衡注药1
        /// </summary>
        EukrasianDosis = 2614,

        /// <summary>
        /// 均衡注药2
        /// </summary>
        EukrasianDosis2 = 2615,

        /// <summary>
        /// 均衡注药3
        /// </summary>
        EukrasianDosis3 = 2616,

        /// <summary>
        /// 均衡诊断
        /// </summary>
        EukrasianDiagnosis = 2607,

        /// <summary>
        /// 均衡诊断
        /// </summary>
        EukrasianPrognosis = 2609,

        /// <summary>
        /// 钢铁信念 盾姿
        /// </summary>
        IronWill = 79,

        /// <summary>
        /// 预警
        /// </summary>
        Sentinel = 74,

        /// <summary>
        /// 沥血剑
        /// </summary>
        GoringBlade = 725,

        /// <summary>
        /// 英勇之剑
        /// </summary>
        BladeofValor = 2721,

        /// <summary>
        /// 赎罪剑
        /// </summary>
        SwordOath = 1902,

        /// <summary>
        /// 安魂祈祷
        /// </summary>
        Requiescat = 1368,

        /// <summary>
        /// 战逃反应
        /// </summary>
        FightOrFlight = 76,

        /// <summary>
        /// 深恶痛绝
        /// </summary>
        Grit = 743,

        /// <summary>
        /// 王室亲卫
        /// </summary>
        RoyalGuard = 1833,

        /// <summary>
        /// 毁绝预备
        /// </summary>
        FurtherRuin = 2701,

        /// <summary>
        /// 深红旋风预备
        /// </summary>
        IfritsFavor = 2724,

        /// <summary>
        /// 螺旋气流预备
        /// </summary>
        GarudasFavor = 2725,

        /// <summary>
        /// 山崩预备
        /// </summary>
        TitansFavor = 2853,

        #region SCH
        /// <summary>
        /// 鼓舞
        /// </summary>
        Galvanize = 297,

        /// <summary>
        /// 转化
        /// </summary>
        Dissipation = 791,

        /// <summary>
        /// 秘策
        /// </summary>
        Recitation = 1896,

        /// <summary>
        /// 毒菌1
        /// </summary>
        Bio = 179,

        /// <summary>
        /// 毒菌2
        /// </summary>
        Bio2 = 189,

        /// <summary>
        /// 毒菌3
        /// </summary>
        Biolysis = 1895,

        /// <summary>
        /// 连环计
        /// </summary>
        ChainStratagem = 1221,
        #endregion

        /// <summary>
        /// 暗影墙
        /// </summary>
        ShadowWall = 747,

        /// <summary>
        /// 弃明投暗
        /// </summary>
        DarkMind = 746,

        /// <summary>
        /// 腐秽大地
        /// </summary>
        SaltedEarth = 749,

        /// <summary>
        /// 血乱
        /// </summary>
        Delirium = 1972,

        /// <summary>
        /// 嗜血
        /// </summary>
        BloodWeapon = 742,

        /// <summary>
        /// 残影镰鼬预备
        /// </summary>
        PhantomKamaitachiReady = 2723,

        /// <summary>
        /// 月影雷兽预备
        /// </summary>
        RaijuReady = 2690,

        /// <summary>
        /// 忍术
        /// </summary>
        Ninjutsu = 496,

        /// <summary>
        /// 生杀予夺
        /// </summary>
        Kassatsu = 497,

        /// <summary>
        /// 土遁之术
        /// </summary>
        Doton = 501,

        /// <summary>
        /// 水遁
        /// </summary>
        Suiton = 507,

        /// <summary>
        /// 隐遁
        /// </summary>
        Hidden = 614,

        /// <summary>
        /// 天地人
        /// </summary>
        TenChiJin = 1186,

        /// <summary>
        /// 极光
        /// </summary>
        Aurora = 1835,

        /// <summary>
        /// 伪装
        /// </summary>
        Camouflage = 1832,

        /// <summary>
        /// 星云
        /// </summary>
        Nebula = 1834,

        /// <summary>
        /// 石之心
        /// </summary>
        HeartofStone = 1840,

        /// <summary>
        /// 无情
        /// </summary>
        NoMercy = 1831,

        /// <summary>
        /// 撕喉预备
        /// </summary>
        ReadyToRip = 1842,

        /// <summary>
        /// 裂膛预备
        /// </summary>
        ReadyToTear = 1843,

        /// <summary>
        /// 穿目预备
        /// </summary>
        ReadyToGouge = 1844,

        /// <summary>
        /// 超高速预备
        /// </summary>
        ReadyToBlast = 2686,

        /// <summary>
        /// 明镜止水
        /// </summary>
        MeikyoShisui = 1233,

        /// <summary>
        /// 燕飞效果提高
        /// </summary>
        Enhanced_Enpi = 1236,

        /// <summary>
        /// 奥义斩浪预备
        /// </summary>
        OgiNamikiriReady = 2959,

        /// <summary>
        /// 彼岸花
        /// </summary>
        Higanbana = 1228,

        /// <summary>
        /// 不能使用能力技
        /// </summary>
        Amnesia = 5,

        //不能干活
        Stun = 2,
        Stun2 = 1343,
        Sleep = 3,
        Sleep2 = 926,
        Sleep3 = 1348,
        Pacification = 6,
        Pacification2 = 620,
        Silence = 7,
        Silence2 = 1347,

        /// <summary>
        /// m慢
        /// </summary>
        Slow = 9,
        Slow2 = 10,
        Slow3 = 193,
        Slow4 = 561,
        Slow5 = 1346,


        /// <summary>
        /// 打不中！
        /// </summary>
        Blind = 15,
        Blind2 = 564,
        Blind3 = 1345,

        /// <summary>
        /// 麻痹
        /// </summary>
        Paralysis = 17,
        Paralysis2 = 482,

        /// <summary>
        /// 噩梦
        /// </summary>
        Nightmare = 423,

        /// <summary>
        /// 耐心
        /// </summary>
        Patience = 850,

        /// <summary>
        /// 战斗连祷
        /// </summary>
        BattleLitany = 786,

        /// <summary>
        /// 风月
        /// </summary>
        Moon = 1298,

        /// <summary>
        /// 风花
        /// </summary>
        Flower = 1299,

        /// <summary>
        /// 真理之剑预备状态
        /// </summary>
        ReadyForBladeofFaith = 3019,

        /// <summary>
        /// 盾阵
        /// </summary>
        Sheltron = 728,

        /// <summary>
        /// 圣盾阵
        /// </summary>
        HolySheltron = 2674,

        /// <summary>
        /// 野火
        /// </summary>
        Wildfire = 1946,

        /// <summary>
        /// 整备
        /// </summary>
        Reassemble = 851,

        /// <summary>
        /// 真北
        /// </summary>
        TrueNorth = 1250,

        /// <summary>
        /// 金刚极意
        /// </summary>
        RiddleofEarth = 1179,

        /// <summary>
        /// 醒梦
        /// </summary>
        LucidDreaming = 1204,

        /// <summary>
        /// 速行
        /// </summary>
        Peloton = 1199,

        /// <summary>

        /// 即兴表演
        /// </summary>
        Improvisation = 1827,

        /// <summary>
        /// 享受即兴表演
        /// </summary>
        _Improvisation = 2695,


        /// <summary>
        /// 即兴表演结束
        /// </summary>
        Improvised_Finish = 2697,

        /// <summary>
        /// 舞动的热情
        /// </summary>
        Rising_Rhythm = 2696,


        /// <summary>
        /// 魔元化
        /// </summary>
        Manafication = 1971,


        /// <summary>
        /// 鼓励
        /// </summary>
        Embolden = 2282,

        /// 以太复制：防护
        /// </summary>
        AethericMimicryTank = 2124,

        /// <summary>
        /// 以太复制：进攻
        /// </summary>
        AethericMimicryDPS = 2125,

        /// <summary>
        /// 以太复制：治疗
        /// </summary>
        AethericMimicryHealer = 2126,

        /// <summary>
        /// 狂战士化
        /// </summary>
        WaxingNocturne = 1718,

        /// <summary>
        /// 狂战士化的副作用
        /// </summary>
        WaningNocturne = 1727,

        /// <summary>
        /// 意志薄弱
        /// </summary>
        BrushwithDeath = 2127,

        /// <summary>
        /// 蓄力
        /// </summary>
        Boost = 1716,

        /// <summary>
        /// 攻击准备
        /// </summary>
        Harmonized = 2118,

        /// <summary>
        /// 斗争本能
        /// </summary>
        BasicInstinct = 2498,

        /// <summary>
        /// 哔哩哔哩
        /// </summary>
        Tingling = 2492,

        /// <summary>
        /// 鬼宿脚
        /// </summary>
        PhantomFlurry = 2502,

        /// <summary>
        /// 穿甲散弹强化
        /// </summary>
        SurpanakhaFury = 2130,

        /// <summary>
        /// 出血
        /// </summary>
        Bleeding = 1714,

        /// <summary>
        /// 冻结
        /// </summary>
        DeepFreeze = 1731,

        /// <summary>
        /// 冰雾
        /// </summary>
        TouchofFrost = 2994,

        /// <summary>
        /// 玄天武水壁
        /// </summary>
        AuspiciousTrance = 2497,

    }
}
