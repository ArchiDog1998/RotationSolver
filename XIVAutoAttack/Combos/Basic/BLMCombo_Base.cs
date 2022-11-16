using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic
{
    internal abstract partial class BLMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
    {
        private static BLMGauge JobGauge => Service.JobGauges.Get<BLMGauge>();

        /// <summary>
        /// 冰状态层数
        /// </summary>
        protected static byte UmbralIceStacks => JobGauge.UmbralIceStacks;

        /// <summary>
        /// 火状态层数
        /// </summary>
        protected static byte AstralFireStacks => JobGauge.AstralFireStacks;

        /// <summary>
        /// 通晓层数
        /// </summary>
        protected static byte PolyglotStacks => JobGauge.PolyglotStacks;

        /// <summary>
        /// 灵极心层数
        /// </summary>
        protected static byte UmbralHearts => JobGauge.UmbralHearts;

        /// <summary>
        /// 是否有悖论
        /// </summary>
        protected static bool IsParadoxActive => JobGauge.IsParadoxActive;

        /// <summary>
        /// 在冰状态
        /// </summary>
        protected static bool InUmbralIce => JobGauge.InUmbralIce;

        /// <summary>
        /// 在火状态
        /// </summary>
        protected static bool InAstralFire => JobGauge.InAstralFire;

        /// <summary>
        /// 是否有天语状态
        /// </summary>
        protected static bool IsEnochianActive => JobGauge.IsEnochianActive;      

        /// <summary>
        /// 下一个通晓还剩多少时间好
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected static bool EnchinaEndAfter(float time)
        {
            return EndAfter(JobGauge.EnochianTimer / 1000f, time);
        }

        /// <summary>
        /// 下一个通晓还剩多少时间好
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected static bool EnchinaEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
        {
            return EndAfterGCD(JobGauge.EnochianTimer / 1000f, gctCount, abilityCount);
        }

        /// <summary>
        /// 天语剩余时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected static bool ElementTimeEndAfter(float time)
        {
            return EndAfter(JobGauge.ElementTimeRemaining / 1000f, time);
        }

        /// <summary>
        /// 天语剩余时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected static bool ElementTimeEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
        {
            return EndAfterGCD(JobGauge.ElementTimeRemaining / 1000f, gctCount, abilityCount);
        }

        public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlackMage, ClassJobID.Thaumaturge };

        /// <summary>
        /// 有火苗
        /// </summary>
        protected static bool HasFire => Player.HasStatus(true, StatusID.Firestarter);
        /// <summary>
        /// 有雷云
        /// </summary>
        protected static bool HasThunder => Player.HasStatus(true, StatusID.Thundercloud);
        /// <summary>
        /// 通晓是否已经达到当前等级下的最大层数
        /// </summary>
        protected static bool IsPolyglotStacksMaxed => Xenoglossy.EnoughLevel ? JobGauge.PolyglotStacks == 2 : JobGauge.PolyglotStacks == 1;

        /// <summary>
        /// 当前火状态还能打几个火4
        /// </summary>
        /// <returns></returns>
        protected static byte F4RemainingNumber()
        {
            if (!InAstralFire) return 0;
            var mpCount = (byte)((Player.CurrentMp - 800) / Fire4.MPNeed);
            var timeCountDe = (byte)((JobGauge.ElementTimeRemaining - CooldownHelper.CalcSpellTime(3000)) / CooldownHelper.CalcSpellTime(2800));
            var timeCountPe = (byte)((JobGauge.ElementTimeRemaining - CooldownHelper.CalcSpellTime(2500)) / CooldownHelper.CalcSpellTime(2800));
            if (IsParadoxActive) return Math.Min(mpCount, timeCountPe);
            else return Math.Min(mpCount, timeCountDe);
        }

        public class ThunderAction : BaseAction
        {
            internal override uint MPNeed => HasThunder ? 0 : base.MPNeed;

            internal ThunderAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
                : base(actionID, isFriendly, shouldEndSpecial, isDot)
            {
            }
        }

        public class Fire3Action : ElementAction
        {
            internal override uint MPNeed => HasFire ? 0 : base.MPNeed;

            internal Fire3Action(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
                : base(actionID, isFriendly, shouldEndSpecial, isDot)
            {
            }
        }

        public class ElementAction : BaseAction
        {
            internal ElementAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false) : base(actionID, isFriendly, shouldEndSpecial, isEot)
            {
            }

            public override bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false)
            {
                if (JobGauge.IsEnochianActive && CastTime - 0.5f > JobGauge.ElementTimeRemaining / 1000f)
                {
                    act = null;
                    return false;
                }
                return base.ShouldUse(out act, mustUse, emptyOrSkipCombo);
            }
        }


        /// <summary>
        /// 闪雷
        /// </summary>
        public static BaseAction Thunder { get; } = new ThunderAction(ActionID.Thunder, isDot: true);

        /// <summary>
        /// 暴雷
        /// </summary>
        public static BaseAction Thunder3 { get; } = new ThunderAction(ActionID.Thunder3, isDot: true);

        /// <summary>
        /// 震雷
        /// </summary>
        public static BaseAction Thunder2 { get; } = new ThunderAction(ActionID.Thunder2, isDot: true);

        /// <summary>
        /// 星灵移位
        /// </summary>
        public static BaseAction Transpose { get; } = new(ActionID.Transpose) { ActionCheck = b => ActionUpdater.AbilityRemain.IsLessThan(JobGauge.ElementTimeRemaining / 1000f) };

        /// <summary>
        /// 灵极魂
        /// </summary>
        public static BaseAction UmbralSoul { get; } = new(ActionID.UmbralSoul) { ActionCheck = b => JobGauge.InUmbralIce && Transpose.ActionCheck(b) };

        /// <summary>
        /// 魔罩
        /// </summary>
        public static BaseAction Manaward { get; } = new(ActionID.Manaward, true);

        /// <summary>
        /// 魔泉
        /// </summary>
        public static BaseAction Manafont { get; } = new(ActionID.Manafont);

        /// <summary>
        /// 激情咏唱
        /// </summary>
        public static BaseAction Sharpcast { get; } = new(ActionID.Sharpcast)
        {
            BuffsProvide = new[] { StatusID.Sharpcast },
            ActionCheck = b => HaveHostilesInRange,
        };

        /// <summary>
        /// 三连咏唱
        /// </summary>
        public static BaseAction Triplecast { get; } = new(ActionID.Triplecast)
        {
            BuffsProvide = Swiftcast.BuffsProvide,
        };

        /// <summary>
        /// 黑魔纹
        /// </summary>
        public static BaseAction Leylines { get; } = new(ActionID.Leylines, true, shouldEndSpecial: true)
        {
            BuffsProvide = new[] { StatusID.LeyLines, },
        };

        /// <summary>
        /// 魔纹步
        /// </summary>
        public static BaseAction BetweenTheLines { get; } = new(ActionID.BetweenTheLines, true, shouldEndSpecial: true)
        {
            BuffsNeed = Leylines.BuffsProvide,
        };

        /// <summary>
        /// 以太步
        /// </summary>
        public static BaseAction AetherialManipulation { get; } = new(ActionID.AetherialManipulation, true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        };

        /// <summary>
        /// 详述
        /// </summary>
        public static BaseAction Amplifier { get; } = new(ActionID.Amplifier) { ActionCheck = b => JobGauge.EnochianTimer > 10000 && JobGauge.PolyglotStacks < 2 };

        /// <summary>
        /// 核爆
        /// </summary>
        public static BaseAction Flare { get; } = new ElementAction(ActionID.Flare) { ActionCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 绝望
        /// </summary>
        public static BaseAction Despair { get; } = new ElementAction(ActionID.Despair) { ActionCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 秽浊
        /// </summary>
        public static BaseAction Foul { get; } = new(ActionID.Foul) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 异言
        /// </summary>
        public static BaseAction Xenoglossy { get; } = new(ActionID.Xenoglossy) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 崩溃
        /// </summary>
        public static BaseAction Scathe { get; } = new(ActionID.Scathe);

        /// <summary>
        /// 悖论
        /// </summary>
        public static BaseAction Paradox { get; } = new(ActionID.Paradox)
        {
            ActionCheck = b => JobGauge.IsParadoxActive,
        };

        /// <summary>
        /// 火1
        /// </summary>
        public static BaseAction Fire { get; } = new (ActionID.Fire);

        /// <summary>
        /// 火2
        /// </summary>
        public static BaseAction Fire2 { get; } = new (ActionID.Fire2);

        /// <summary>
        /// 火3
        /// </summary>
        public static BaseAction Fire3 { get; } = new Fire3Action(ActionID.Fire3);

        /// <summary>
        /// 火4
        /// </summary>
        public static BaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
        {
            ActionCheck = b => JobGauge.InAstralFire,
        };

        /// <summary>
        /// 冰1
        /// </summary>
        public static BaseAction Blizzard { get; } = new (ActionID.Blizzard);

        /// <summary>
        /// 冰2
        /// </summary>
        public static BaseAction Blizzard2 { get; } = new (ActionID.Blizzard2);

        /// <summary>
        /// 冰3
        /// </summary>
        public static BaseAction Blizzard3 { get; } = new (ActionID.Blizzard3);

        /// <summary>
        /// 冰4
        /// </summary>
        public static BaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4)
        {
            ActionCheck = b =>
            {
                if (IsLastSpell(true, Blizzard4)) return false;

                if (JobGauge.UmbralHearts == 3) return false;

                return JobGauge.InUmbralIce;
            }
        };

        /// <summary>
        /// 冻结
        /// </summary>
        public static BaseAction Freeze { get; } = new ElementAction(ActionID.Freeze)
        {
            ActionCheck = b => JobGauge.InUmbralIce,
        };
    }
}
