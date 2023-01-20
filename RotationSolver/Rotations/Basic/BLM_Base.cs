using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Updaters;

namespace RotationSolver.Rotations.Basic
{
    internal abstract partial class BLMRotation_Base : CustomRotation.CustomRotation
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

            public override bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false)
            {
                if (JobGauge.IsEnochianActive && CastTime - 0.5f > JobGauge.ElementTimeRemaining / 1000f)
                {
                    act = null;
                    return false;
                }
                return base.ShouldUse(out act, mustUse, emptyOrSkipCombo, skipDisable);
            }
        }


        /// <summary>
        /// 闪雷
        /// </summary>
        public static IBaseAction Thunder { get; } = new ThunderAction(ActionID.Thunder, isDot: true);

        /// <summary>
        /// 暴雷
        /// </summary>
        public static IBaseAction Thunder3 { get; } = new ThunderAction(ActionID.Thunder3, isDot: true);

        /// <summary>
        /// 震雷
        /// </summary>
        public static IBaseAction Thunder2 { get; } = new ThunderAction(ActionID.Thunder2, isDot: true);

        /// <summary>
        /// 星灵移位
        /// </summary>
        public static IBaseAction Transpose { get; } = new BaseAction(ActionID.Transpose) { ActionCheck = b => ActionUpdater.AbilityRemain.IsLessThan(JobGauge.ElementTimeRemaining / 1000f) };

        /// <summary>
        /// 灵极魂
        /// </summary>
        public static IBaseAction UmbralSoul { get; } = new BaseAction(ActionID.UmbralSoul) { ActionCheck = b => JobGauge.InUmbralIce && Transpose.ActionCheck(b) };

        /// <summary>
        /// 魔罩
        /// </summary>
        public static IBaseAction Manaward { get; } = new BaseAction(ActionID.Manaward, true, isTimeline: true);

        /// <summary>
        /// 魔泉
        /// </summary>
        public static IBaseAction Manafont { get; } = new BaseAction(ActionID.Manafont);

        /// <summary>
        /// 激情咏唱
        /// </summary>
        public static IBaseAction Sharpcast { get; } = new BaseAction(ActionID.Sharpcast)
        {
            StatusProvide = new[] { StatusID.Sharpcast },
            ActionCheck = b => HaveHostilesInRange,
        };

        /// <summary>
        /// 三连咏唱
        /// </summary>
        public static IBaseAction Triplecast { get; } = new BaseAction(ActionID.Triplecast)
        {
            StatusProvide = Swiftcast.StatusProvide,
        };

        /// <summary>
        /// 黑魔纹
        /// </summary>
        public static IBaseAction Leylines { get; } = new BaseAction(ActionID.Leylines, true, shouldEndSpecial: true)
        {
            StatusProvide = new[] { StatusID.LeyLines, },
        };

        /// <summary>
        /// 魔纹步
        /// </summary>
        public static IBaseAction BetweenTheLines { get; } = new BaseAction(ActionID.BetweenTheLines, true, shouldEndSpecial: true)
        {
            StatusNeed = Leylines.StatusProvide,
        };

        /// <summary>
        /// 以太步
        /// </summary>
        public static IBaseAction AetherialManipulation { get; } = new BaseAction(ActionID.AetherialManipulation, true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        };

        /// <summary>
        /// 详述
        /// </summary>
        public static IBaseAction Amplifier { get; } = new BaseAction(ActionID.Amplifier) { ActionCheck = b => JobGauge.EnochianTimer > 10000 && JobGauge.PolyglotStacks < 2 };

        /// <summary>
        /// 核爆
        /// </summary>
        public static IBaseAction Flare { get; } = new ElementAction(ActionID.Flare) { ActionCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 绝望
        /// </summary>
        public static IBaseAction Despair { get; } = new ElementAction(ActionID.Despair) { ActionCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 秽浊
        /// </summary>
        public static IBaseAction Foul { get; } = new BaseAction(ActionID.Foul) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 异言
        /// </summary>
        public static IBaseAction Xenoglossy { get; } = new BaseAction(ActionID.Xenoglossy) { ActionCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 崩溃
        /// </summary>
        public static IBaseAction Scathe { get; } = new BaseAction(ActionID.Scathe);

        /// <summary>
        /// 悖论
        /// </summary>
        public static IBaseAction Paradox { get; } = new BaseAction(ActionID.Paradox)
        {
            ActionCheck = b => JobGauge.IsParadoxActive,
        };

        /// <summary>
        /// 火1
        /// </summary>
        public static IBaseAction Fire { get; } = new BaseAction(ActionID.Fire);

        /// <summary>
        /// 火2
        /// </summary>
        public static IBaseAction Fire2 { get; } = new BaseAction(ActionID.Fire2);

        /// <summary>
        /// 火3
        /// </summary>
        public static IBaseAction Fire3 { get; } = new Fire3Action(ActionID.Fire3);

        /// <summary>
        /// 火4
        /// </summary>
        public static IBaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
        {
            ActionCheck = b => JobGauge.InAstralFire,
        };

        /// <summary>
        /// 冰1
        /// </summary>
        public static IBaseAction Blizzard { get; } = new BaseAction(ActionID.Blizzard);

        /// <summary>
        /// 冰2
        /// </summary>
        public static IBaseAction Blizzard2 { get; } = new BaseAction(ActionID.Blizzard2);

        /// <summary>
        /// 冰3
        /// </summary>
        public static IBaseAction Blizzard3 { get; } = new BaseAction(ActionID.Blizzard3);

        /// <summary>
        /// 冰4
        /// </summary>
        public static IBaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4)
        {
            ActionCheck = b =>
            {
                if (IsLastGCD(true, Blizzard4)) return false;

                if (JobGauge.UmbralHearts == 3) return false;

                return JobGauge.InUmbralIce;
            }
        };

        /// <summary>
        /// 冻结
        /// </summary>
        public static IBaseAction Freeze { get; } = new ElementAction(ActionID.Freeze)
        {
            ActionCheck = b => JobGauge.InUmbralIce,
        };
    }
}
