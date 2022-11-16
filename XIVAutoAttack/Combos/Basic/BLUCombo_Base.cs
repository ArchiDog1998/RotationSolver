using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic
{
    internal enum BLUID : byte
    {
        Tank,
        Healer,
        DPS,
    }

    internal abstract class BLUCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
    {
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

        protected static BLUAttackType AttackType { get; set; } = BLUAttackType.Magical;

        protected static BLUID BlueId { get; set; } = BLUID.DPS;

        public class BLUAction : BaseAction
        {
            private BLUActionType Type;

            public unsafe bool OnSlot
            {
                get
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (ID == ActionManager.Instance()->GetActiveBlueMageActionInSlot(i)) return true;
                    }
                    return false;
                }
            }

            internal BLUAction(ActionID actionID, BLUActionType type, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false)
                : base(actionID, isFriendly, shouldEndSpecial, isEot)
            {
                Type = type;
            }

            public sealed override bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false)
            {
                act = null;

                if (!OnSlot) return false;

                //排除其他类型的魔法。
                if(AttackType == BLUAttackType.Physical && Type == BLUActionType.Magical) return false;
                if(AttackType == BLUAttackType.Magical && Type == BLUActionType.Physical) return false;

                return base.ShouldUse(out act, mustUse, emptyOrSkipCombo);
            }
        }

        #region 魔法单体

        /// <summary>
        /// 水炮
        /// </summary>
        public static BLUAction WaterCannon { get; } = new(ActionID.WaterCannon, BLUActionType.Magical);

        /// <summary>
        /// 苦闷之歌
        /// </summary>
        public static BLUAction SongofTorment { get; } = new(ActionID.SongofTorment, BLUActionType.Magical, isEot: true);

        /// <summary>
        /// 吸血
        /// </summary>
        public static BLUAction BloodDrain { get; } = new (ActionID.BloodDrain, BLUActionType.Magical)
        {
            OtherCheck = b => Player.CurrentMp <= 9500,
        };

        /// <summary>
        /// 音爆
        /// </summary>
        public static BLUAction SonicBoom { get; } = new(ActionID.SonicBoom, BLUActionType.Magical);

        /// <summary>
        /// 永恒射线
        /// </summary>
        public static BLUAction PerpetualRay { get; } = new(ActionID.PerpetualRay, BLUActionType.Magical);


        /// <summary>
        /// 逆流
        /// </summary>
        public static BLUAction Reflux { get; } = new(ActionID.Reflux, BLUActionType.Magical);

        /// <summary>
        /// 捕食
        /// </summary>
        public static BLUAction Devour { get; } = new(ActionID.Devour, BLUActionType.Magical);

        /// <summary>
        /// 斗灵弹
        /// </summary>
        public static BLUAction TheRoseofDestruction { get; } = new(ActionID.TheRoseofDestruction, BLUActionType.Magical);

        /// <summary>
        /// 马特拉魔术
        /// </summary>
        public static BLUAction MatraMagic { get; } = new(ActionID.MatraMagic, BLUActionType.Magical);

        /// <summary>
        /// 冰雾
        /// </summary>
        public static BLUAction WhiteDeath { get; } = new(ActionID.WhiteDeath, BLUActionType.Magical);
        #endregion

        #region 魔法群体

        /// <summary>
        /// 火炎放射
        /// </summary>
        public static BLUAction FlameThrower { get; } = new(ActionID.FlameThrower, BLUActionType.Magical);

        /// <summary>
        /// 水流吐息
        /// </summary>
        public static BLUAction AquaBreath { get; } = new(ActionID.AquaBreath, BLUActionType.Magical, isEot: true);

        /// <summary>
        /// 高压电流
        /// </summary>
        public static BLUAction HighVoltage { get; } = new(ActionID.HighVoltage, BLUActionType.Magical);

        /// <summary>
        /// 怒视
        /// </summary>
        public static BLUAction Glower { get; } = new(ActionID.Glower, BLUActionType.Magical);

        /// <summary>
        /// 平原震裂
        /// </summary>
        public static BLUAction Plaincracker { get; } = new(ActionID.Plaincracker, BLUActionType.Magical);

        /// <summary>
        /// 诡异视线
        /// </summary>
        public static BLUAction TheLook { get; } = new(ActionID.TheLook, BLUActionType.Magical);

        /// <summary>
        /// 寒冰咆哮
        /// </summary>
        public static BLUAction TheRamVoice { get; } = new(ActionID.TheRamVoice, BLUActionType.Magical);

        /// <summary>
        /// 雷电咆哮
        /// </summary>
        public static BLUAction TheDragonVoice { get; } = new(ActionID.TheDragonVoice, BLUActionType.Magical);

        /// <summary>
        /// 喷墨
        /// </summary>
        public static BLUAction InkJet { get; } = new(ActionID.InkJet, BLUActionType.Magical);

        /// <summary>
        /// 火投枪
        /// </summary>
        public static BLUAction FireAngon { get; } = new(ActionID.FireAngon, BLUActionType.Magical);

        /// <summary>
        /// 精神冲击
        /// </summary>
        public static BLUAction MindBlast { get; } = new(ActionID.MindBlast, BLUActionType.Magical);

        /// <summary>
        /// 飞翎雨
        /// </summary>
        public static BLUAction FeatherRain { get; } = new(ActionID.FeatherRain, BLUActionType.Magical);

        /// <summary>
        /// 地火喷发
        /// </summary>
        public static BLUAction Eruption { get; } = new(ActionID.Eruption, BLUActionType.Magical);

        /// <summary>
        /// 山崩
        /// </summary>
        public static BLUAction MountainBuster { get; } = new(ActionID.MountainBusterBLU, BLUActionType.Magical);

        /// <summary>
        /// 轰雷
        /// </summary>
        public static BLUAction ShockStrike { get; } = new(ActionID.ShockStrike, BLUActionType.Magical);

        /// <summary>
        /// 冰雪乱舞
        /// </summary>
        public static BLUAction GlassDance { get; } = new(ActionID.GlassDance, BLUActionType.Magical);

        /// <summary>
        /// 高山气流
        /// </summary>
        public static BLUAction AlpineDraft { get; } = new(ActionID.AlpineDraft, BLUActionType.Magical);

        /// <summary>
        /// 万变水波
        /// </summary>
        public static BLUAction ProteanWave { get; } = new(ActionID.ProteanWave, BLUActionType.Magical);

        /// <summary>
        /// 狂风暴雪
        /// </summary>
        public static BLUAction Northerlies { get; } = new(ActionID.Northerlies, BLUActionType.Magical);

        /// <summary>
        /// 生物电
        /// </summary>
        public static BLUAction Electrogenesis { get; } = new(ActionID.Electrogenesis, BLUActionType.Magical);

        /// <summary>
        /// 魔法锤
        /// </summary>
        public static BLUAction MagicHammer { get; } = new(ActionID.MagicHammer, BLUActionType.Magical);

        /// <summary>
        /// 白骑士之旅
        /// </summary>
        public static BLUAction WhiteKnightsTour { get; } = new(ActionID.WhiteKnightsTour, BLUActionType.Magical);

        /// <summary>
        /// 黑骑士之旅
        /// </summary>
        public static BLUAction BlackKnightsTour { get; } = new(ActionID.BlackKnightsTour, BLUActionType.Magical);

        /// <summary>
        /// 穿甲散弹
        /// </summary>
        public static BLUAction Surpanakha { get; } = new(ActionID.Surpanakha, BLUActionType.Magical);

        /// <summary>
        /// 类星体
        /// </summary>
        public static BLUAction Quasar { get; } = new(ActionID.Quasar, BLUActionType.Magical);

        /// <summary>
        /// 哔哩哔哩
        /// </summary>
        public static BLUAction Tingle { get; } = new(ActionID.Tingle, BLUActionType.Magical);

        /// <summary>
        /// 掀地板之术
        /// </summary>
        public static BLUAction Tatamigaeshi { get; } = new(ActionID.Tatamigaeshi, BLUActionType.Magical);

        /// <summary>
        /// 赞歌
        /// </summary>
        public static BLUAction Stotram { get; } = new(ActionID.Stotram, BLUActionType.Magical);

        /// <summary>
        /// 圣光射线
        /// </summary>
        public static BLUAction SaintlyBeam { get; } = new(ActionID.SaintlyBeam, BLUActionType.Magical);

        /// <summary>
        /// 污泥泼洒
        /// </summary>
        public static BLUAction FeculentFlood { get; } = new(ActionID.FeculentFlood, BLUActionType.Magical);

        /// <summary>
        /// 冰焰
        /// </summary>
        public static BLUAction Blaze { get; } = new(ActionID.Blaze, BLUActionType.Magical);

        /// <summary>
        /// 芥末爆弹
        /// </summary>
        public static BLUAction MustardBomb { get; } = new(ActionID.MustardBomb, BLUActionType.Magical);

        /// <summary>
        /// 以太火花
        /// </summary>
        public static BLUAction AetherialSpark { get; } = new(ActionID.AetherialSpark, BLUActionType.Magical, isEot:true);

        /// <summary>
        /// 水力吸引
        /// </summary>
        public static BLUAction HydroPull { get; } = new(ActionID.HydroPull, BLUActionType.Magical);

        /// <summary>
        /// 水脉诅咒
        /// </summary>
        public static BLUAction MaledictionofWater { get; } = new(ActionID.MaledictionofWater, BLUActionType.Magical);

        /// <summary>
        /// 陆行鸟陨石
        /// </summary>
        public static BLUAction ChocoMeteor { get; } = new(ActionID.ChocoMeteor, BLUActionType.Magical);

        /// <summary>
        /// 月下彼岸花
        /// </summary>
        public static BLUAction Nightbloom { get; } = new(ActionID.Nightbloom, BLUActionType.Magical);

        /// <summary>
        /// 玄天武水壁
        /// </summary>
        public static BLUAction DivineCataract { get; } = new(ActionID.DivineCataract, BLUActionType.Magical);

        /// <summary>
        /// 鬼宿脚(需要buff版本）
        /// </summary>
        public static BLUAction PhantomFlurry2 { get; } = new(ActionID.PhantomFlurry2, BLUActionType.Magical);
        #endregion

        #region 物理单体

        /// <summary>
        /// 终极针
        /// </summary>
        public static BLUAction FinalSting { get; } = new(ActionID.FinalSting, BLUActionType.Physical)
        {
            OtherCheck = b => !Player.HasStatus(true, StatusID.BrushwithDeath),
        };

        /// <summary>
        /// 锋利菜刀
        /// </summary>
        public static BLUAction SharpenedKnife { get; } = new(ActionID.SharpenedKnife, BLUActionType.Physical);

        /// <summary>
        /// 投掷沙丁鱼
        /// </summary>
        public static BLUAction FlyingSardine { get; } = new(ActionID.FlyingSardine, BLUActionType.Physical);

        /// <summary>
        /// 深渊贯穿
        /// </summary>
        public static BLUAction AbyssalTransfixion { get; } = new(ActionID.AbyssalTransfixion, BLUActionType.Physical);

        /// <summary>
        /// 渔叉三段
        /// </summary>
        public static BLUAction TripleTrident { get; } = new(ActionID.TripleTrident, BLUActionType.Physical);

        /// <summary>
        /// 复仇冲击
        /// </summary>
        public static BLUAction RevengeBlast { get; } = new(ActionID.RevengeBlast, BLUActionType.Physical)
        {
            OtherCheck = b => b.GetHealthRatio() < 0.2f,
        };

        #endregion

        #region 物理群体

        /// <summary>
        /// 狂乱
        /// </summary>
        public static BLUAction FlyingFrenzy { get; } = new(ActionID.FlyingFrenzy, BLUActionType.Physical);

        /// <summary>
        /// 钻头炮
        /// </summary>
        public static BLUAction DrillCannons { get; } = new(ActionID.DrillCannons, BLUActionType.Physical);

        /// <summary>
        /// 4星吨
        /// </summary>
        public static BLUAction Weight4tonze { get; } = new(ActionID.Weight4tonze, BLUActionType.Physical);

        /// <summary>
        /// 千针刺
        /// </summary>
        public static BLUAction Needles1000 { get; } = new(ActionID.Needles1000, BLUActionType.Physical);

        /// <summary>
        /// 寒光
        /// </summary>
        public static BLUAction Kaltstrahl { get; } = new(ActionID.Kaltstrahl, BLUActionType.Physical);

        /// <summary>
        /// 正义飞踢
        /// </summary>
        public static BLUAction JKick { get; } = new(ActionID.JKick, BLUActionType.Physical);

        /// <summary>
        /// 生成外设
        /// </summary>
        public static BLUAction PeripheralSynthesis { get; } = new(ActionID.PeripheralSynthesis, BLUActionType.Physical);

        /// <summary>
        /// 如意大旋风
        /// </summary>
        public static BLUAction BothEnds { get; } = new(ActionID.BothEnds, BLUActionType.Physical);
        #endregion

        #region 其他单体
        /// <summary>
        /// 滑舌
        /// </summary>
        public static BLUAction StickyTongue { get; } = new(ActionID.StickyTongue, BLUActionType.None);

        /// <summary>
        /// 导弹
        /// </summary>
        public static BLUAction Missile { get; } = new(ActionID.Missile, BLUActionType.None);

        /// <summary>
        /// 螺旋尾
        /// </summary>
        public static BLUAction TailScrew { get; } = new(ActionID.TailScrew, BLUActionType.None);

        /// <summary>
        /// 死亡宣告
        /// </summary>
        public static BLUAction Doom { get; } = new(ActionID.Doom, BLUActionType.None);

        /// <summary>
        /// 怪音波
        /// </summary>
        public static BLUAction EerieSoundwave { get; } = new(ActionID.EerieSoundwave, BLUActionType.None);

        /// <summary>
        /// 小侦测
        /// </summary>
        public static BLUAction CondensedLibra { get; } = new(ActionID.CondensedLibra, BLUActionType.None);
        #endregion

        #region 其他群体

        /// <summary>
        /// 5级石化
        /// </summary>
        public static BLUAction Level5Petrify { get; } = new(ActionID.Level5Petrify, BLUActionType.None);

        /// <summary>
        /// 橡果炸弹
        /// </summary>
        public static BLUAction AcornBomb { get; } = new(ActionID.AcornBomb, BLUActionType.None);

        /// <summary>
        /// 投弹
        /// </summary>
        public static BLUAction BombToss { get; } = new(ActionID.BombToss, BLUActionType.None);

        /// <summary>
        /// 自爆
        /// </summary>
        public static BLUAction Selfdestruct { get; } = new(ActionID.Selfdestruct, BLUActionType.None)
        {
            OtherCheck = b => !Player.HasStatus(true, StatusID.BrushwithDeath),
        };

        /// <summary>
        /// 拍掌
        /// </summary>
        public static BLUAction Faze { get; } = new(ActionID.Faze, BLUActionType.None);

        /// <summary>
        /// 鼻息
        /// </summary>
        public static BLUAction Snort { get; } = new(ActionID.Snort, BLUActionType.None);

        /// <summary>
        /// 臭气
        /// </summary>
        public static BLUAction BadBreath { get; } = new(ActionID.BadBreath, BLUActionType.None);

        /// <summary>
        /// 唧唧咋咋
        /// </summary>
        public static BLUAction Chirp { get; } = new(ActionID.Chirp, BLUActionType.None);

        /// <summary>
        /// 蛙腿
        /// </summary>
        public static BLUAction FrogLegs { get; } = new(ActionID.FrogLegs, BLUActionType.None);

        /// <summary>
        /// 5级即死
        /// </summary>
        public static BLUAction Level5Death { get; } = new(ActionID.Level5Death, BLUActionType.None);

        /// <summary>
        /// 火箭炮
        /// </summary>
        public static BLUAction Launcher { get; } = new(ActionID.Launcher, BLUActionType.None);

        /// <summary>
        /// 超振动
        /// </summary>
        public static BLUAction Ultravibration { get; } = new(ActionID.Ultravibration, BLUActionType.None);

        /// <summary>
        /// 鬼宿脚
        /// </summary>
        public static BLUAction PhantomFlurry { get; } = new(ActionID.PhantomFlurry, BLUActionType.None);
        #endregion

        #region 防御

        /// <summary>
        /// 冰棘屏障
        /// </summary>
        public static BLUAction IceSpikes { get; } = new(ActionID.IceSpikes, BLUActionType.None, true);

        /// <summary>
        /// 水神的面纱
        /// </summary>
        public static BLUAction VeiloftheWhorl { get; } = new(ActionID.VeiloftheWhorl, BLUActionType.None, true);

        /// <summary>
        /// 超硬化
        /// </summary>
        public static BLUAction Diamondback { get; } = new(ActionID.Diamondback, BLUActionType.None, true);

        /// <summary>
        /// 哥布防御
        /// </summary>
        public static BLUAction Gobskin { get; } = new(ActionID.Gobskin, BLUActionType.None, true);

        /// <summary>
        /// 仙人盾
        /// </summary>
        public static BLUAction Cactguard { get; } = new(ActionID.Cactguard, BLUActionType.None, true);

        /// <summary>
        /// 玄结界
        /// </summary>
        public static BLUAction ChelonianGate { get; } = new(ActionID.ChelonianGate, BLUActionType.None, true);

        /// <summary>
        /// 龙之力
        /// </summary>
        public static BLUAction DragonForce { get; } = new(ActionID.DragonForce, BLUActionType.None, true);
        #endregion


        #region 辅助
        /// <summary>
        /// 油性分泌物
        /// </summary>
        public static BLUAction ToadOil { get; } = new(ActionID.ToadOil, BLUActionType.None, true);

        /// <summary>
        /// 怒发冲冠
        /// </summary>
        public static BLUAction Bristle { get; } = new(ActionID.Bristle, BLUActionType.Magical, true)
        {
            BuffsProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
        };

        /// <summary>
        /// 破防
        /// </summary>
        public static BLUAction Offguard { get; } = new(ActionID.Offguard, BLUActionType.None, true);

        /// <summary>
        /// 强力守护
        /// </summary>
        public static BLUAction MightyGuard { get; } = new(ActionID.MightyGuard, BLUActionType.None, true);

        /// <summary>
        /// 月之笛
        /// </summary>
        public static BLUAction MoonFlute { get; } = new(ActionID.MoonFlute, BLUActionType.None, true)
        {
            BuffsProvide = new StatusID[] { StatusID.WaxingNocturne },
        };

        /// <summary>
        /// 惊奇光
        /// </summary>
        public static BLUAction PeculiarLight { get; } = new(ActionID.PeculiarLight, BLUActionType.Magical);

        /// <summary>
        /// 防御指示
        /// </summary>
        public static BLUAction Avail { get; } = new(ActionID.Avail, BLUActionType.Magical);

        /// <summary>
        /// 口笛
        /// </summary>
        public static BLUAction Whistle { get; } = new(ActionID.Whistle, BLUActionType.Physical, true)
        {
            BuffsProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
        };


        /// <summary>
        /// 彻骨雾寒
        /// </summary>
        public static BLUAction ColdFog { get; } = new(ActionID.ColdFog, BLUActionType.Magical, true);
        #endregion

        #region 治疗
        /// <summary>
        /// 白风
        /// </summary>
        public static BLUAction WhiteWind { get; } = new(ActionID.WhiteWind, BLUActionType.None, true);

        /// <summary>
        /// 绒绒治疗
        /// </summary>
        public static BLUAction PomCure { get; } = new(ActionID.PomCure, BLUActionType.None, true);

        /// <summary>
        /// 天使低语
        /// </summary>
        public static BLUAction AngelWhisper { get; } = new(ActionID.AngelWhisper, BLUActionType.None, true);

        /// <summary>
        /// 蜕皮  
        /// </summary>
        public static BLUAction Exuviation { get; } = new(ActionID.Exuviation, BLUActionType.None, true);

        /// <summary>
        /// 天使的点心  
        /// </summary>
        public static BLUAction AngelsSnack { get; } = new(ActionID.AngelsSnack, BLUActionType.None, true);
        #endregion

        #region 其他
        /// <summary>
        /// 若隐若现
        /// </summary>
        public static BLUAction Loom { get; } = new(ActionID.Loom, BLUActionType.None);

        /// <summary>
        /// 以太复制
        /// </summary>
        private static BLUAction AetherialMimicry { get; } = new(ActionID.AetherialMimicry, BLUActionType.None, true)
        {
            ChoiceTarget = charas =>
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

        /// <summary>
        /// 斗争本能
        /// </summary>
        public static BLUAction BasicInstinct { get; } = new(ActionID.BasicInstinct, BLUActionType.None);
        #endregion


        private protected override bool EmergercyGCD(out IAction act)
        {
            if (AetherialMimicry.ShouldUse(out act)) return true;
            return base.EmergercyGCD(out act);
        }

        protected static bool AllOnSlot(params BLUAction[] actions) => actions.All(a => a.OnSlot);
    }
}
