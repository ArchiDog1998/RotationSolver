using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos
{
    internal struct ObjectStatus
    {
        public const ushort
            Thunder = 161,

            Thunder2 = 162,

            Thunder3 = 163,

            Thunder4 = 1210,

            //即刻咏唱
            Swiftcast1 = 1325,
            Swiftcast2 = 1987,
            Swiftcast3 = 167,
            Dualcast = 1249,
            //三连咏唱
            Triplecast = 1211,

            //激情咏唱
            Sharpcast = 867,

            //雷云
            Thundercloud = 164,

            //黑魔纹
            LeyLines = 737,

            //火苗
            Firestarter = 165,

            //复活
            Raise = 148,

            //止步
            Bind1 = 13,
            Bind2 = 1345,

            //赤火炎预备
            VerfireReady = 1234,

            //赤飞石预备
            VerstoneReady = 1235,

            //促进
            Acceleration = 1238,

            //疾风
            Aero = 143,

            //烈风
            Aero2 = 144,

            //天辉
            Dia = 1871,

            //医济的Hot
            Medica2 = 150,
            TrueMedica2 = 2792,

            //再生
            Regen1 = 158,
            Regen2 = 897,
            Regen3 = 1330,

            //死亡宣告，可以被解除的！
            Doom = 910,

            //神圣领域
            HallowedGround = 82,

            //死斗
            Holmgang = 409,

            //行尸走肉
            WillDead = 810,

            //死而不僵
            WalkingDead = 811,

            //超火流行
            Superbolide = 1836,

            //直线射击预备
            StraightShotReady = 122,

            //毒咬箭Dot
            VenomousBite = 124,

            //风蚀箭Dot
            Windbite = 129,

            //毒咬箭 Plus Dot
            CausticBite = 1200,

            //风蚀箭 Plus Dot
            Stormbite = 1201,

            //影噬箭准备
            ShadowbiteReady = 3002,

            //爆破箭准备
            BlastArrowReady = 2692,

            //猛者强击
            RagingStrikes = 125,

            //战斗之声
            BattleVoice = 141,

            //铁壁
            Rampart1 = 71,
            Rampart2 = 1191,
            Rampart3 = 1978,

            //复仇
            Vengeance = 89,

            //守护 盾姿
            Defiance = 91,

            //原初的直觉
            RawIntuition = 735,

            //原初的血气
            Bloodwhetting = 2678,

            //原初的解放
            InnerRelease = 1177,

            //原初的混沌
            NascentChaos = 1897,

            //战场风暴
            SurgingTempest = 2677,

            //蛮荒崩裂预备
            PrimalRendReady = 2624,

            //烧灼Dot
            Combust = 838,
            Combust2 = 843,
            Combust3 = 1881,
            Combust4 = 2041,

            //吉星相位
            AspectedBenefic = 835,

            //阳星相位
            AspectedHelios = 836,

            //光速
            LightSpeed = 841,

            //可以重抽
            ClarifyingDraw = 2713,

            //六个Buff
            TheBalance = 1882,
            TheBole = 1883,
            TheArrow = 1884,
            TheSpear = 1885,
            TheEwer = 1886,
            TheSpire = 1887,

            //地星主宰
            EarthlyDominance = 1224,
            //巨星主宰
            GiantDominance = 1248,

            //行吟
            Troubadour = 1934,
            //策动
            Tactician1 = 1951,
            Tactician2 = 2177,
            //防守之桑巴
            ShieldSamba = 1826,

            //死亡烙印
            DeathsDesign = 2586,

            //妖异之镰
            SoulReaver = 2587,

            //绞决效果提高
            EnhancedGibbet = 2588,
            EnhancedVoidReaping = 2590,

            //夜游魂
            Enshrouded = 2593,

            //死亡祭祀
            CircleofSacrifice = 2972,
            //死亡祭品
            ImmortalSacrifice = 2592,

            //播魂种
            Soulsow = 2594,

            //回退准备
            Threshold = 2595,

            //龙牙龙爪预备状态
            SharperFangandClaw = 802,

            //龙尾大回旋预备状态
            EnhancedWheelingThrust = 803,

            //龙枪状态
            PowerSurge = 2720,

            //龙剑状态
            LifeSurge = 2175,

            //猛枪
            LanceCharge = 1864,

            //幻象冲预备状态
            DiveReady = 1243,

            //魔猿形
            OpoOpoForm = 107,

            //盗龙形
            RaptorForm = 108,

            //猛豹形
            CoerlForm = 109,

            //连击效果提高
            LeadenFist = 1861,

            //功力
            DisciplinedFist = 3001,

            //破碎拳
            Demolish = 246,

            //震脚
            PerfectBalance = 110,

            //无相身形
            FormlessFist = 2513,

            //对称投掷
            SilkenSymmetry = 2693,

            //非对称投掷
            SilkenFlow = 2694,

            //扇舞·急
            ThreefoldFanDance = 1820,

            //扇舞·终
            FourfoldFanDance = 2699,

            //流星舞预备
            FlourishingStarfall = 2700,

            //标准舞步
            StandardStep = 1818,

            //标准舞步结束
            StandardFinish = 1821,

            //技巧舞步结束
            TechnicalFinish = 1822,

            //技巧舞步
            TechnicalStep = 1819,

            //闭式舞姿
            ClosedPosition1 = 1823,
            ClosedPosition2 = 2026,

            //进攻之探戈
            Devilment = 1825,

            //提拉纳预备
            FlourishingFinish = 2698,

            //要死了
            Weakness = 43,
            BrinkofDeath = 44,
            //关心
            Kardia = 2604,
            //关心
            Kardion = 2605,

            //均衡注药
            EukrasianDosis = 2614,
            EukrasianDosis2 = 2615,
            EukrasianDosis3 = 2616,

            //均衡诊断
            EukrasianDiagnosis = 2607,
            EukrasianPrognosis = 2609,

            //鼓舞
            Galvanize = 297,

            //钢铁信念 盾姿
            IronWill = 79,

            //预警
            Sentinel = 74,

            //沥血剑
            GoringBlade = 725,

            //英勇之剑
            BladeofValor = 2721,

            //赎罪剑
            SwordOath = 1902,

            //安魂祈祷
            Requiescat = 1368,

            //深恶痛绝
            Grit = 743,

            //王室亲卫
            RoyalGuard = 1833,

            //毁绝预备
            FurtherRuin = 2701,

            //深红旋风预备
            IfritsFavor = 2724,

            //螺旋气流预备
            GarudasFavor = 2725,

            //山崩预备
            TitansFavor = 2853,

            //毒菌
            Bio = 179,
            Bio2 = 189,
            Biolysis = 1895,

            //暗影墙
            ShadowWall = 747,

            //弃明投暗
            DarkMind = 746,

            //腐秽大地
            SaltedEarth = 749,

            //血乱
            Delirium = 1972,

            //残影镰鼬预备
            PhantomKamaitachiReady = 2723,

            //月影雷兽预备
            RaijuReady = 2690,

            //忍术
            Ninjutsu = 496,

            //生杀予夺
            Kassatsu = 497,

            //土遁之术
            Doton = 501,
            //水遁
            Suiton = 507,
            //隐遁
            Hidden = 614,

            //天地人
            TenChiJin = 1186,

            //极光
            Aurora = 1835,

            //伪装
            Camouflage = 1832,

            //星云
            Nebula = 1834,

            //石之心
            HeartofStone = 1840,

            //撕喉预备
            ReadyToRip = 1842,

            //裂膛预备
            ReadyToTear = 1843,

            //穿目预备
            ReadyToGouge = 1844,

            //超高速预备
            ReadyToBlast = 2686,

            //明镜止水
            MeikyoShisui = 1233,

            //奥义斩浪预备
            OgiNamikiriReady = 2959,

            //彼岸花
            Higanbana = 1228,

            //不能使用能力技
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

            //m慢
            Slow = 9,
            Slow2 = 10,
            Slow3 = 193,
            Slow4 = 561,
            Slow5 = 1346,


            //打不中！
            Blind = 15,
            Blind2 = 564,
            Blind3 = 1345,

            //麻痹
            Paralysis = 17,
            Paralysis2 = 482,

            //噩梦
            Nightmare = 423,

            //耐心
            Patience = 850,

            //捕鱼人之技
            AnglersArt = 2778,

            //钓组
            Snagging = 761,

            //撒饵
            Chum = 763,

            //战斗连祷
            BattleLitany = 786,
            //风月
            Moon = 1298,
            //风花
            Flower = 1299,

            //真理之剑预备状态
            ReadyForBladeofFaith = 3019,

            //盾阵
            Sheltron = 728,

            //圣盾阵
            HolySheltron = 2674,

            //野火
            Wildfire = 1946,

            //整备
            Reassemble = 851;
    }
}
