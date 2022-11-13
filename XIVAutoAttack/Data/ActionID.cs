
namespace XIVAutoAttack.Data
{
    internal enum ActionID : uint
    {
        #region Astrologian

        /// <summary>
        /// 生辰
        /// </summary>
        Ascend = 3603,

        /// <summary>
        /// 凶星
        /// </summary>
        Malefic = 3596,

        /// <summary>
        /// 烧灼
        /// </summary>
        Combust = 3599,

        /// <summary>
        /// 重力
        /// </summary>
        Gravity = 3615,

        /// <summary>
        /// 吉星
        /// </summary>
        Benefic = 3594,

        /// <summary>
        /// 福星
        /// </summary>
        Benefic2 = 3610,

        /// <summary>
        /// 吉星相位
        /// </summary>
        AspectedBenefic = 3595,

        /// <summary>
        /// 先天禀赋
        /// </summary>
        EssentialDignity = 3614,

        /// <summary>
        /// 星位合图
        /// </summary>
        Synastry = 3612,

        /// <summary>
        /// 天星交错
        /// </summary>
        CelestialIntersection = 16556,

        /// <summary>
        /// 擢升
        /// </summary>
        Exaltation = 25873,

        /// <summary>
        /// 阳星
        /// </summary>
        Helios = 3600,

        /// <summary>
        /// 阳星相位
        /// </summary>
        AspectedHelios = 3601,

        /// <summary>
        /// 天星冲日
        /// </summary>
        CelestialOpposition = 16553,

        /// <summary>
        /// 地星
        /// </summary>
        EarthlyStar = 7439,

        /// <summary>
        /// 命运之轮
        /// </summary>
        CollectiveUnconscious = 3613,

        /// <summary>
        /// 天宫图
        /// </summary>
        Horoscope = 16557,

        /// <summary>
        /// 光速
        /// </summary>
        Lightspeed = 3606,

        /// <summary>
        /// 中间学派
        /// </summary>
        NeutralSect = 16559,

        /// <summary>
        /// 大宇宙
        /// </summary>
        Macrocosmos = 25874,

        /// <summary>
        /// 星力
        /// </summary>
        Astrodyne = 25870,

        /// <summary>
        /// 占卜
        /// </summary>
        Divination = 16552,

        /// <summary>
        /// 抽卡
        /// </summary>
        Draw = 3590,

        /// <summary>
        /// 重抽
        /// </summary>
        Redraw = 3593,

        /// <summary>
        /// 小奥秘卡
        /// </summary>
        MinorArcana = 7443,

        /// <summary>
        /// 出王冠卡
        /// </summary>
        CrownPlay = 25869,

        /// <summary>
        /// 太阳神之衡
        /// </summary>
        Balance = 4401,

        /// <summary>
        /// 放浪神之箭
        /// </summary>
        Arrow = 4402,

        /// <summary>
        /// 战争神之枪
        /// </summary>
        Spear = 4403,

        /// <summary>
        /// 世界树之干
        /// </summary>
        Bole = 4404,

        /// <summary>
        /// 河流神之瓶
        /// </summary>
        Ewer = 4405,

        /// <summary>
        /// 建筑神之塔
        /// </summary>
        Spire = 4406,
        #endregion

        #region BlackMage
        /// <summary>
        /// 闪雷
        /// </summary>
        Thunder = 144,

        /// <summary>
        /// 震雷
        /// </summary>
        Thunder2 = 7447,

        /// <summary>
        /// 暴雷
        /// </summary>
        Thunder3 = 153,

        /// <summary>
        /// 星灵移位
        /// </summary>
        Transpose = 149,

        /// <summary>
        /// 灵极魂
        /// </summary>
        UmbralSoul = 16506,

        /// <summary>
        /// 魔罩
        /// </summary>
        Manaward = 157,

        /// <summary>
        /// 魔泉
        /// </summary>
        Manafont = 158,

        /// <summary>
        /// 激情咏唱
        /// </summary>
        Sharpcast = 3574,

        /// <summary>
        /// 三连咏唱
        /// </summary>
        Triplecast = 7421,

        /// <summary>
        /// 黑魔纹
        /// </summary>
        Leylines = 3573,

        /// <summary>
        /// 魔纹步
        /// </summary>
        BetweenTheLines = 7419,

        /// <summary>
        /// 以太步
        /// </summary>
        AetherialManipulation = 155,

        /// <summary>
        /// 详述
        /// </summary>
        Amplifier = 25796,

        /// <summary>
        /// 核爆
        /// </summary>
        Flare = 162,

        /// <summary>
        /// 绝望
        /// </summary>
        Despair = 16505,

        /// <summary>
        /// 秽浊
        /// </summary>
        Foul = 7422,

        /// <summary>
        /// 异言
        /// </summary>
        Xenoglossy = 16507,

        /// <summary>
        /// 崩溃
        /// </summary>
        Scathe = 156,

        /// <summary>
        /// 悖论
        /// </summary>
        Paradox = 25797,

        /// <summary>
        /// 火炎
        /// </summary>
        Fire = 141,

        /// <summary>
        /// 烈炎
        /// </summary>
        Fire2 = 147,

        /// <summary>
        /// 爆炎
        /// </summary>
        Fire3 = 152,

        /// <summary>
        /// 炽炎
        /// </summary>
        Fire4 = 3577,

        /// <summary>
        /// 冰结
        /// </summary>
        Blizzard = 142,

        /// <summary>
        /// 冰冻
        /// </summary>
        Blizzard2 = 25793,

        /// <summary>
        /// 冰封
        /// </summary>
        Blizzard3 = 154,

        /// <summary>
        /// 冰澈
        /// </summary>
        Blizzard4 = 3576,

        /// <summary>
        /// 冻结
        /// </summary>
        Freeze = 159,
        #endregion

        #region BlueMage
        /// <summary>
        /// 水炮
        /// </summary>
        WaterCannon = 11385,
        #endregion

        #region Bard
        /// <summary>
        /// 强力射击
        /// </summary>
        HeavyShoot = 97,

        /// <summary>
        /// 直线射击
        /// </summary>
        StraitShoot = 98,

        /// <summary>
        /// 毒咬箭
        /// </summary>
        VenomousBite = 100,

        /// <summary>
        /// 风蚀箭
        /// </summary>
        Windbite = 113,

        /// <summary>
        /// 伶牙俐齿
        /// </summary>
        IronJaws = 3560,

        /// <summary>
        /// 贤者的叙事谣
        /// </summary>
        MagesBallad = 114,

        /// <summary>
        /// 军神的赞美歌
        /// </summary>
        ArmysPaeon = 116,

        /// <summary>
        /// 放浪神的小步舞曲
        /// </summary>
        WanderersMinuet = 3559,

        /// <summary>
        /// 战斗之声
        /// </summary>
        BattleVoice = 118,

        /// <summary>
        /// 猛者强击
        /// </summary>
        RagingStrikes = 101,

        /// <summary>
        /// 光明神的最终乐章
        /// </summary>
        RadiantFinale = 25785,

        /// <summary>
        /// 纷乱箭
        /// </summary>
        Barrage = 107,

        /// <summary>
        /// 九天连箭
        /// </summary>
        EmpyrealArrow = 3558,

        /// <summary>
        /// 完美音调
        /// </summary>
        PitchPerfect = 7404,

        /// <summary>
        /// 失血箭
        /// </summary>
        Bloodletter = 110,

        /// <summary>
        /// 死亡箭雨
        /// </summary>
        RainofDeath = 117,


        #endregion
        /// <summary>
        /// 崩拳
        /// </summary>
        SnapPunch = 56,
            /// <summary>
            /// 破碎拳
            /// </summary>
            Demolish = 66,
            /// <summary>
            /// 旋风刃
            /// </summary>
            AeolianEdge = 2255,
            /// <summary>
            /// 攻其不备
            /// </summary>
            TrickAttack = 2258,
            /// <summary>
            /// 水遁之术
            /// </summary>
            Suiton = 2271,
            /// <summary>
            /// 龙牙龙爪
            /// </summary>
            FangandClaw = 3554,
            /// <summary>
            /// 强甲破点突
            /// </summary>
            ArmorCrush = 3563,
            /// <summary>
            /// 龙尾大回旋
            /// </summary>
            WheelingThrust = 3556,
            /// <summary>
            /// 樱花怒放
            /// </summary>
            ChaosThrust = 88,
            /// <summary>
            /// 樱花怒放
            /// </summary>
            ChaoticSpring = 25772,
            /// <summary>
            /// 展开战术
            /// </summary>
            DeploymentTactics = 3585,
            /// <summary>
            /// 绞决
            /// </summary>
            Gibbet = 24382,
            /// <summary>
            /// 缢杀
            /// </summary>
            Gallows = 24383,
            /// <summary>
            /// 月光
            /// </summary>
            Gekko = 7481,
            /// <summary>
            /// 花车
            /// </summary>
            Kasha = 7482,
    }
}
