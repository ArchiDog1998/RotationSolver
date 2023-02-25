using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Commands;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Linq;

namespace RotationSolver.Rotations.Basic;


internal interface IBLUAction : IBaseAction
{
    bool OnSlot { get; }
    bool RightType { get; }
}
internal abstract class BLU_Base : CustomRotation.CustomRotation
{
    internal enum BLUID : byte
    {
        Tank,
        Healer,
        DPS,
    }

    internal enum BLUAttackType : byte
    {
        Magical,
        Physical,
        Both,
    }

    internal enum BLUActionType : byte
    {
        None,
        Magical,
        Physical,
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlueMage };

    protected static BLUAttackType AttackType { get; set; } = BLUAttackType.Both;

    protected static BLUID BlueId { get; set; } = BLUID.DPS;

    private protected sealed override IBaseAction Raise => AngelWhisper;

    public class BLUAction : BaseAction, IBLUAction
    {
        static readonly StatusID[] NoPhysic = new StatusID[]
        {
            StatusID.IceSpikes,
        };

        static readonly StatusID[] NoMagic = new StatusID[]
        {
            StatusID.RespellingSpray,
            StatusID.Magitek,
        };

        private BLUActionType _type;
        public bool RightType
        {
            get
            {
                if (_type == BLUActionType.None) return true;
                if (AttackType == BLUAttackType.Physical && _type == BLUActionType.Magical) return false;
                if (AttackType == BLUAttackType.Magical && _type == BLUActionType.Physical) return false;

                if (Target.HasStatus(false, NoPhysic) && _type == BLUActionType.Physical) return false;
                if (Target.HasStatus(false, NoMagic) && _type == BLUActionType.Magical) return false;
                return true;
            }
        }

        public unsafe bool OnSlot => ActionUpdater.BluSlots.Any(i => AdjustedID == Service.IconReplacer.OriginalHook(i));

        internal BLUAction(ActionID actionID, BLUActionType type, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false, bool isTimeline = false)
            : base(actionID, isFriendly, shouldEndSpecial, isEot, isTimeline)
        {
            _type = type;
            ActionCheck = t => OnSlot && RightType;
        }

        public override bool CanUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false, uint gcdCountForAbility = 0)
        {
            act = null;

            if (!OnSlot) return false;
            return base.CanUse(out act, mustUse, emptyOrSkipCombo, skipDisable);
        }
    }

    #region 魔法单体

    /// <summary>
    /// 水炮
    /// </summary>
    public static IBLUAction WaterCannon { get; } = new BLUAction(ActionID.WaterCannon, BLUActionType.Magical);

    /// <summary>
    /// 苦闷之歌
    /// </summary>
    public static IBLUAction SongofTorment { get; } = new BLUAction(ActionID.SongofTorment, BLUActionType.Magical, isEot: true)
    {
        TargetStatus = new[] { StatusID.Bleeding }
    };

    /// <summary>
    /// 吸血
    /// </summary>
    public static IBLUAction BloodDrain { get; } = new BLUAction(ActionID.BloodDrain, BLUActionType.Magical)
    {
        ActionCheck = b => Player.CurrentMp <= 9500,
    };

    /// <summary>
    /// 音爆
    /// </summary>
    public static IBLUAction SonicBoom { get; } = new BLUAction(ActionID.SonicBoom, BLUActionType.Magical);

    /// <summary>
    /// 永恒射线
    /// </summary>
    public static IBLUAction PerpetualRay { get; } = new BLUAction(ActionID.PerpetualRay, BLUActionType.Magical);


    /// <summary>
    /// 逆流
    /// </summary>
    public static IBLUAction Reflux { get; } = new BLUAction(ActionID.Reflux, BLUActionType.Magical);

    /// <summary>
    /// 捕食
    /// </summary>
    public static IBLUAction Devour { get; } = new BLUAction(ActionID.Devour, BLUActionType.Magical, isTimeline: true);

    /// <summary>
    /// 斗灵弹
    /// </summary>
    public static IBLUAction TheRoseofDestruction { get; } = new BLUAction(ActionID.TheRoseofDestruction, BLUActionType.Magical);

    /// <summary>
    /// 马特拉魔术
    /// </summary>
    public static IBLUAction MatraMagic { get; } = new BLUAction(ActionID.MatraMagic, BLUActionType.Magical);

    /// <summary>
    /// 冰雾
    /// </summary>
    public static IBLUAction WhiteDeath { get; } = new BLUAction(ActionID.WhiteDeath, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.TouchofFrost)
    };
    #endregion

    #region 魔法群体

    /// <summary>
    /// 火炎放射
    /// </summary>
    public static IBLUAction FlameThrower { get; } = new BLUAction(ActionID.FlameThrower, BLUActionType.Magical);

    /// <summary>
    /// 水流吐息
    /// </summary>
    public static IBLUAction AquaBreath { get; } = new BLUAction(ActionID.AquaBreath, BLUActionType.Magical, isEot: true);

    /// <summary>
    /// 高压电流
    /// </summary>
    public static IBLUAction HighVoltage { get; } = new BLUAction(ActionID.HighVoltage, BLUActionType.Magical);

    /// <summary>
    /// 怒视
    /// </summary>
    public static IBLUAction Glower { get; } = new BLUAction(ActionID.Glower, BLUActionType.Magical);

    /// <summary>
    /// 平原震裂
    /// </summary>
    public static IBLUAction Plaincracker { get; } = new BLUAction(ActionID.Plaincracker, BLUActionType.Magical);

    /// <summary>
    /// 诡异视线
    /// </summary>
    public static IBLUAction TheLook { get; } = new BLUAction(ActionID.TheLook, BLUActionType.Magical);

    /// <summary>
    /// 寒冰咆哮
    /// </summary>
    public static IBLUAction TheRamVoice { get; } = new BLUAction(ActionID.TheRamVoice, BLUActionType.Magical);

    /// <summary>
    /// 雷电咆哮
    /// </summary>
    public static IBLUAction TheDragonVoice { get; } = new BLUAction(ActionID.TheDragonVoice, BLUActionType.Magical);

    /// <summary>
    /// 喷墨
    /// </summary>
    public static IBLUAction InkJet { get; } = new BLUAction(ActionID.InkJet, BLUActionType.Magical);

    /// <summary>
    /// 火投枪
    /// </summary>
    public static IBLUAction FireAngon { get; } = new BLUAction(ActionID.FireAngon, BLUActionType.Magical);

    /// <summary>
    /// 精神冲击
    /// </summary>
    public static IBLUAction MindBlast { get; } = new BLUAction(ActionID.MindBlast, BLUActionType.Magical);

    /// <summary>
    /// 飞翎雨
    /// </summary>
    public static IBLUAction FeatherRain { get; } = new BLUAction(ActionID.FeatherRain, BLUActionType.Magical);

    /// <summary>
    /// 地火喷发
    /// </summary>
    public static IBLUAction Eruption { get; } = new BLUAction(ActionID.Eruption, BLUActionType.Magical);

    /// <summary>
    /// 山崩
    /// </summary>
    public static IBLUAction MountainBuster { get; } = new BLUAction(ActionID.MountainBusterBLU, BLUActionType.Magical);

    /// <summary>
    /// 轰雷
    /// </summary>
    public static IBLUAction ShockStrike { get; } = new BLUAction(ActionID.ShockStrike, BLUActionType.Magical);

    /// <summary>
    /// 冰雪乱舞
    /// </summary>
    public static IBLUAction GlassDance { get; } = new BLUAction(ActionID.GlassDance, BLUActionType.Magical);

    /// <summary>
    /// 高山气流
    /// </summary>
    public static IBLUAction AlpineDraft { get; } = new BLUAction(ActionID.AlpineDraft, BLUActionType.Magical);

    /// <summary>
    /// 万变水波
    /// </summary>
    public static IBLUAction ProteanWave { get; } = new BLUAction(ActionID.ProteanWave, BLUActionType.Magical);

    /// <summary>
    /// 狂风暴雪
    /// </summary>
    public static IBLUAction Northerlies { get; } = new BLUAction(ActionID.Northerlies, BLUActionType.Magical);

    /// <summary>
    /// 生物电
    /// </summary>
    public static IBLUAction Electrogenesis { get; } = new BLUAction(ActionID.Electrogenesis, BLUActionType.Magical);

    /// <summary>
    /// 魔法锤
    /// </summary>
    public static IBLUAction MagicHammer { get; } = new BLUAction(ActionID.MagicHammer, BLUActionType.Magical, isTimeline: true);

    /// <summary>
    /// 白骑士之旅
    /// </summary>
    public static IBLUAction WhiteKnightsTour { get; } = new BLUAction(ActionID.WhiteKnightsTour, BLUActionType.Magical);

    /// <summary>
    /// 黑骑士之旅
    /// </summary>
    public static IBLUAction BlackKnightsTour { get; } = new BLUAction(ActionID.BlackKnightsTour, BLUActionType.Magical);

    /// <summary>
    /// 穿甲散弹
    /// </summary>
    public static IBLUAction Surpanakha { get; } = new BLUAction(ActionID.Surpanakha, BLUActionType.Magical);

    /// <summary>
    /// 类星体
    /// </summary>
    public static IBLUAction Quasar { get; } = new BLUAction(ActionID.Quasar, BLUActionType.Magical);

    /// <summary>
    /// 哔哩哔哩
    /// </summary>
    public static IBLUAction Tingle { get; } = new BLUAction(ActionID.Tingle, BLUActionType.Magical)
    {
        StatusProvide = new StatusID[] {StatusID.Tingling},
    };

    /// <summary>
    /// 掀地板之术
    /// </summary>
    public static IBLUAction Tatamigaeshi { get; } = new BLUAction(ActionID.Tatamigaeshi, BLUActionType.Magical);

    /// <summary>
    /// 圣光射线
    /// </summary>
    public static IBLUAction SaintlyBeam { get; } = new BLUAction(ActionID.SaintlyBeam, BLUActionType.Magical);

    /// <summary>
    /// 污泥泼洒
    /// </summary>
    public static IBLUAction FeculentFlood { get; } = new BLUAction(ActionID.FeculentFlood, BLUActionType.Magical);

    /// <summary>
    /// 冰焰
    /// </summary>
    public static IBLUAction Blaze { get; } = new BLUAction(ActionID.Blaze, BLUActionType.Magical);

    /// <summary>
    /// 芥末爆弹
    /// </summary>
    public static IBLUAction MustardBomb { get; } = new BLUAction(ActionID.MustardBomb, BLUActionType.Magical);

    /// <summary>
    /// 以太火花
    /// </summary>
    public static IBLUAction AetherialSpark { get; } = new BLUAction(ActionID.AetherialSpark, BLUActionType.Magical, isEot: true);

    /// <summary>
    /// 水力吸引
    /// </summary>
    public static IBLUAction HydroPull { get; } = new BLUAction(ActionID.HydroPull, BLUActionType.Magical);

    /// <summary>
    /// 水脉诅咒
    /// </summary>
    public static IBLUAction MaledictionofWater { get; } = new BLUAction(ActionID.MaledictionofWater, BLUActionType.Magical);

    /// <summary>
    /// 陆行鸟陨石
    /// </summary>
    public static IBLUAction ChocoMeteor { get; } = new BLUAction(ActionID.ChocoMeteor, BLUActionType.Magical);

    /// <summary>
    /// 月下彼岸花
    /// </summary>
    public static IBLUAction Nightbloom { get; } = new BLUAction(ActionID.Nightbloom, BLUActionType.Magical);

    /// <summary>
    /// 玄天武水壁
    /// </summary>
    public static IBLUAction DivineCataract { get; } = new BLUAction(ActionID.DivineCataract, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.AuspiciousTrance)
    };

    /// <summary>
    /// 鬼宿脚(需要buff版本）
    /// </summary>
    public static IBLUAction PhantomFlurry2 { get; } = new BLUAction(ActionID.PhantomFlurry2, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.PhantomFlurry)
    };
    #endregion

    #region 物理单体

    /// <summary>
    /// 终极针
    /// </summary>
    public static IBLUAction FinalSting { get; } = new BLUAction(ActionID.FinalSting, BLUActionType.Physical)
    {
        ActionCheck = b => !Player.HasStatus(true, StatusID.BrushwithDeath),
    };

    /// <summary>
    /// 锋利菜刀
    /// </summary>
    public static IBLUAction SharpenedKnife { get; } = new BLUAction(ActionID.SharpenedKnife, BLUActionType.Physical);

    /// <summary>
    /// 投掷沙丁鱼
    /// </summary>
    public static IBLUAction FlyingSardine { get; } = new BLUAction(ActionID.FlyingSardine, BLUActionType.Physical);

    /// <summary>
    /// 深渊贯穿
    /// </summary>
    public static IBLUAction AbyssalTransfixion { get; } = new BLUAction(ActionID.AbyssalTransfixion, BLUActionType.Physical);

    /// <summary>
    /// 渔叉三段
    /// </summary>
    public static IBLUAction TripleTrident { get; } = new BLUAction(ActionID.TripleTrident, BLUActionType.Physical);

    /// <summary>
    /// 复仇冲击
    /// </summary>
    public static IBLUAction RevengeBlast { get; } = new BLUAction(ActionID.RevengeBlast, BLUActionType.Physical)
    {
        ActionCheck = b => b.GetHealthRatio() < 0.2f,
    };

    #endregion

    #region 物理群体

    /// <summary>
    /// 狂乱
    /// </summary>
    public static IBLUAction FlyingFrenzy { get; } = new BLUAction(ActionID.FlyingFrenzy, BLUActionType.Physical);

    /// <summary>
    /// 钻头炮
    /// </summary>
    public static IBLUAction DrillCannons { get; } = new BLUAction(ActionID.DrillCannons, BLUActionType.Physical);

    /// <summary>
    /// 4星吨
    /// </summary>
    public static IBLUAction Weight4tonze { get; } = new BLUAction(ActionID.Weight4tonze, BLUActionType.Physical);

    /// <summary>
    /// 千针刺
    /// </summary>
    public static IBLUAction Needles1000 { get; } = new BLUAction(ActionID.Needles1000, BLUActionType.Physical);

    /// <summary>
    /// 寒光
    /// </summary>
    public static IBLUAction Kaltstrahl { get; } = new BLUAction(ActionID.Kaltstrahl, BLUActionType.Physical);

    /// <summary>
    /// 正义飞踢
    /// </summary>
    public static IBLUAction JKick { get; } = new BLUAction(ActionID.JKick, BLUActionType.Physical);

    /// <summary>
    /// 生成外设
    /// </summary>
    public static IBLUAction PeripheralSynthesis { get; } = new BLUAction(ActionID.PeripheralSynthesis, BLUActionType.Physical);

    /// <summary>
    /// 如意大旋风
    /// </summary>
    public static IBLUAction BothEnds { get; } = new BLUAction(ActionID.BothEnds, BLUActionType.Physical);
    #endregion

    #region 其他单体
    /// <summary>
    /// 滑舌
    /// </summary>
    public static IBLUAction StickyTongue { get; } = new BLUAction(ActionID.StickyTongue, BLUActionType.None);

    /// <summary>
    /// 导弹
    /// </summary>
    public static IBLUAction Missile { get; } = new BLUAction(ActionID.Missile, BLUActionType.None);

    /// <summary>
    /// 螺旋尾
    /// </summary>
    public static IBLUAction TailScrew { get; } = new BLUAction(ActionID.TailScrew, BLUActionType.None);

    /// <summary>
    /// 死亡宣告
    /// </summary>
    public static IBLUAction Doom { get; } = new BLUAction(ActionID.Doom, BLUActionType.None);

    /// <summary>
    /// 怪音波
    /// </summary>
    public static IBLUAction EerieSoundwave { get; } = new BLUAction(ActionID.EerieSoundwave, BLUActionType.None);

    /// <summary>
    /// 小侦测
    /// </summary>
    public static IBLUAction CondensedLibra { get; } = new BLUAction(ActionID.CondensedLibra, BLUActionType.None);
    #endregion

    #region 其他群体

    /// <summary>
    /// 5级石化
    /// </summary>
    public static IBLUAction Level5Petrify { get; } = new BLUAction(ActionID.Level5Petrify, BLUActionType.None);

    /// <summary>
    /// 橡果炸弹
    /// </summary>
    public static IBLUAction AcornBomb { get; } = new BLUAction(ActionID.AcornBomb, BLUActionType.None);

    /// <summary>
    /// 投弹
    /// </summary>
    public static IBLUAction BombToss { get; } = new BLUAction(ActionID.BombToss, BLUActionType.None);

    /// <summary>
    /// 自爆
    /// </summary>
    public static IBLUAction Selfdestruct { get; } = new BLUAction(ActionID.Selfdestruct, BLUActionType.None)
    {
        ActionCheck = b => !Player.HasStatus(true, StatusID.BrushwithDeath),
    };

    /// <summary>
    /// 拍掌
    /// </summary>
    public static IBLUAction Faze { get; } = new BLUAction(ActionID.Faze, BLUActionType.None);

    /// <summary>
    /// 鼻息
    /// </summary>
    public static IBLUAction Snort { get; } = new BLUAction(ActionID.Snort, BLUActionType.None);

    /// <summary>
    /// 臭气
    /// </summary>
    public static IBLUAction BadBreath { get; } = new BLUAction(ActionID.BadBreath, BLUActionType.None, isTimeline: true);

    /// <summary>
    /// 唧唧咋咋
    /// </summary>
    public static IBLUAction Chirp { get; } = new BLUAction(ActionID.Chirp, BLUActionType.None);

    /// <summary>
    /// 蛙腿
    /// </summary>
    public static IBLUAction FrogLegs { get; } = new BLUAction(ActionID.FrogLegs, BLUActionType.None);

    /// <summary>
    /// 5级即死
    /// </summary>
    public static IBLUAction Level5Death { get; } = new BLUAction(ActionID.Level5Death, BLUActionType.None);

    /// <summary>
    /// 火箭炮
    /// </summary>
    public static IBLUAction Launcher { get; } = new BLUAction(ActionID.Launcher, BLUActionType.None);

    /// <summary>
    /// 超振动
    /// </summary>
    public static IBLUAction Ultravibration { get; } = new BLUAction(ActionID.Ultravibration, BLUActionType.None);

    /// <summary>
    /// 鬼宿脚
    /// </summary>
    public static IBLUAction PhantomFlurry { get; } = new BLUAction(ActionID.PhantomFlurry, BLUActionType.None);
    #endregion

    #region 防御

    /// <summary>
    /// 冰棘屏障
    /// </summary>
    public static IBLUAction IceSpikes { get; } = new BLUAction(ActionID.IceSpikes, BLUActionType.None, true);

    /// <summary>
    /// 水神的面纱
    /// </summary>
    public static IBLUAction VeiloftheWhorl { get; } = new BLUAction(ActionID.VeiloftheWhorl, BLUActionType.None, true);

    /// <summary>
    /// 超硬化
    /// </summary>
    public static IBLUAction Diamondback { get; } = new BLUAction(ActionID.Diamondback, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 哥布防御
    /// </summary>
    public static IBLUAction Gobskin { get; } = new BLUAction(ActionID.Gobskin, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 仙人盾
    /// </summary>
    public static IBLUAction Cactguard { get; } = new BLUAction(ActionID.Cactguard, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 玄结界
    /// </summary>
    public static IBLUAction ChelonianGate { get; } = new BLUAction(ActionID.ChelonianGate, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 龙之力
    /// </summary>
    public static IBLUAction DragonForce { get; } = new BLUAction(ActionID.DragonForce, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };
    #endregion


    #region 辅助
    /// <summary>
    /// 油性分泌物
    /// </summary>
    public static IBLUAction ToadOil { get; } = new BLUAction(ActionID.ToadOil, BLUActionType.None, true);

    /// <summary>
    /// 怒发冲冠
    /// </summary>
    public static IBLUAction Bristle { get; } = new BLUAction(ActionID.Bristle, BLUActionType.Magical, true)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };

    /// <summary>
    /// 破防
    /// </summary>
    public static IBLUAction Offguard { get; } = new BLUAction(ActionID.Offguard, BLUActionType.None, true);

    /// <summary>
    /// 强力守护
    /// </summary>
    public static IBLUAction MightyGuard { get; } = new BLUAction(ActionID.MightyGuard, BLUActionType.None, true)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.MightyGuard,
        },
    };

    /// <summary>
    /// 月之笛
    /// </summary>
    public static IBLUAction MoonFlute { get; } = new BLUAction(ActionID.MoonFlute, BLUActionType.None, true)
    {
        StatusProvide = new StatusID[] { StatusID.WaxingNocturne },
    };

    /// <summary>
    /// 惊奇光
    /// </summary>
    public static IBLUAction PeculiarLight { get; } = new BLUAction(ActionID.PeculiarLight, BLUActionType.Magical);

    /// <summary>
    /// 防御指示
    /// </summary>
    public static IBLUAction Avail { get; } = new BLUAction(ActionID.Avail, BLUActionType.Magical);

    /// <summary>
    /// 口笛
    /// </summary>
    public static IBLUAction Whistle { get; } = new BLUAction(ActionID.Whistle, BLUActionType.Physical, true)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };


    /// <summary>
    /// 彻骨雾寒
    /// </summary>
    public static IBLUAction ColdFog { get; } = new BLUAction(ActionID.ColdFog, BLUActionType.Magical, true);
    #endregion

    #region 治疗
    /// <summary>
    /// 白风
    /// </summary>
    public static IBLUAction WhiteWind { get; } = new BLUAction(ActionID.WhiteWind, BLUActionType.None, true)
    {
        ActionCheck = b => Player.GetHealthRatio() is > 0.3f and < 0.5f,
    };

    /// <summary>
    /// 赞歌
    /// </summary>
    public static IBLUAction Stotram { get; } = new BLUAction(ActionID.Stotram, BLUActionType.Magical, true);


    /// <summary>
    /// 绒绒治疗
    /// </summary>
    public static IBLUAction PomCure { get; } = new BLUAction(ActionID.PomCure, BLUActionType.None, true);

    /// <summary>
    /// 天使低语
    /// </summary>
    public static IBLUAction AngelWhisper { get; } = new BLUAction(ActionID.AngelWhisper, BLUActionType.None, true);

    /// <summary>
    /// 蜕皮  
    /// </summary>
    public static IBLUAction Exuviation { get; } = new BLUAction(ActionID.Exuviation, BLUActionType.None, true);

    /// <summary>
    /// 天使的点心  
    /// </summary>
    public static IBLUAction AngelsSnack { get; } = new BLUAction(ActionID.AngelsSnack, BLUActionType.None, true);
    #endregion

    #region 其他
    /// <summary>
    /// 若隐若现
    /// </summary>
    private static IBLUAction Loom { get; } = new BLUAction(ActionID.Loom, BLUActionType.None, shouldEndSpecial: true);

    /// <summary>
    /// 斗争本能
    /// </summary>
    public static IBLUAction BasicInstinct { get; } = new BLUAction(ActionID.BasicInstinct, BLUActionType.None)
    {
        StatusProvide = new StatusID[] { StatusID.BasicInstinct },
        ActionCheck = b =>
        {
            //TODO: 还需要判断是否为多人本
            return Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56]
                && TargetUpdater.PartyMembers.Count(p => p.GetHealthRatio() > 0) == 1;
        },
    };

    /// <summary>
    /// 以太复制
    /// </summary>
    private static IBLUAction AethericMimicry { get; } = new BLUAction(ActionID.AethericMimicry, BLUActionType.None, true)
    {
        ChoiceTarget = (charas, mustUse) =>
        {
            switch (BlueId)
            {
                case BLUID.DPS:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryDPS))
                    {
                        return charas.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical).FirstOrDefault();
                    }
                    break;

                case BLUID.Tank:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryTank))
                    {
                        return charas.GetJobCategory(JobRole.Tank).FirstOrDefault();
                    }
                    break;

                case BLUID.Healer:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryHealer))
                    {
                        return charas.GetJobCategory(JobRole.Healer).FirstOrDefault();
                    }
                    break;
            }
            return null;
        },
    };

    #endregion

    private protected override bool MoveForwardGCD(out IAction act)
    {
        if (Loom.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }


    private protected override bool EmergencyGCD(out IAction act)
    {
        if (AethericMimicry.CanUse(out act)) return true;
        if (BlueId == BLUID.Healer)
        {
            //有某些非常危险的状态。
            if (RSCommands.SpecialType == SpecialCommandType.EsunaStanceNorth && TargetUpdater.WeakenPeople.Any() || TargetUpdater.DyingPeople.Any())
            {
                if (Exuviation.CanUse(out act, mustUse: true)) return true;
            }
        }
        if (BasicInstinct.CanUse(out _))
        {
            if (MightyGuard.CanUse(out act)) return true;
            act = BasicInstinct;
            return true;
        }

        return base.EmergencyGCD(out act);
    }

    protected static bool AllOnSlot(params IBLUAction[] actions) => actions.All(a => a.OnSlot);
    protected static uint OnSlotCount(params IBLUAction[] actions) => (uint)actions.Count(a => a.OnSlot);

    public override IBaseAction[] AllActions => base.AllActions.Where(a =>
    {
        if (a is not BLUAction b) return false;
        return b.OnSlot;
    }).ToArray();

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("BlueId", 2, "Role", "Tank", "Healer", "DPS")
            .SetCombo("AttackType", 2, "Type", "Magic", "Physic", "Both");
    }

    private protected override void UpdateInfo()
    {
        BlueId = (BLUID)Configs.GetCombo("BlueId");
        AttackType = (BLUAttackType)Configs.GetCombo("AttackType");
        base.UpdateInfo();
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (PomCure.CanUse(out act)) return true;
        }
        if (WhiteWind.CanUse(out act, mustUse: true)) return true;
        return base.HealSingleGCD(out act);
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (AngelsSnack.CanUse(out act)) return true;
            if (Stotram.CanUse(out act)) return true;
        }

        if (WhiteWind.CanUse(out act, mustUse: true)) return true;
        return base.HealAreaGCD(out act);
    }
}
