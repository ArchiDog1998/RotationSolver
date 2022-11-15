using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;


namespace XIVAutoAttack.Combos.Basic
{
    internal abstract partial class BLMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
    {
        protected static BLMGauge JobGauge => Service.JobGauges.Get<BLMGauge>();

        public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlackMage, ClassJobID.Thaumaturge };

        protected static bool HasFire => Player.HaveStatus(true, StatusID.Firestarter);
        protected static bool HasThunder => Player.HaveStatus(true, StatusID.Thundercloud);

        public class ThunderAction : BaseAction
        {
            internal override uint MPNeed => HasThunder ? 0 : base.MPNeed;

            internal ThunderAction(ActionID actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
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
        public static BaseAction Transpose { get; } = new(ActionID.Transpose) { OtherCheck = b => JobGauge.InUmbralIce || JobGauge.InAstralFire };

        /// <summary>
        /// 灵极魂
        /// </summary>
        public static BaseAction UmbralSoul { get; } = new(ActionID.UmbralSoul) { OtherCheck = b => JobGauge.InUmbralIce };

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
            OtherCheck = b => HaveHostilesInRange,
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
        public static BaseAction Amplifier { get; } = new(ActionID.Amplifier) { OtherCheck = b => JobGauge.EnochianTimer > 10000 && JobGauge.PolyglotStacks < 2 };

        /// <summary>
        /// 核爆
        /// </summary>
        public static BaseAction Flare { get; } = new ElementAction(ActionID.Flare) { OtherCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 绝望
        /// </summary>
        public static BaseAction Despair { get; } = new ElementAction(ActionID.Despair) { OtherCheck = b => JobGauge.InAstralFire };

        /// <summary>
        /// 秽浊
        /// </summary>
        public static BaseAction Foul { get; } = new(ActionID.Foul) { OtherCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 异言
        /// </summary>
        public static BaseAction Xenoglossy { get; } = new(ActionID.Xenoglossy) { OtherCheck = b => JobGauge.PolyglotStacks != 0 };

        /// <summary>
        /// 崩溃
        /// </summary>
        public static BaseAction Scathe { get; } = new(ActionID.Scathe);

        /// <summary>
        /// 悖论
        /// </summary>
        public static BaseAction Paradox { get; } = new(ActionID.Paradox)
        {
            OtherCheck = b => JobGauge.IsParadoxActive,
        };

        /// <summary>
        /// 火1
        /// </summary>
        public static BaseAction Fire { get; } = new ElementAction(ActionID.Fire);

        /// <summary>
        /// 火2
        /// </summary>
        public static BaseAction Fire2 { get; } = new ElementAction(ActionID.Fire2);

        /// <summary>
        /// 火3
        /// </summary>
        public static BaseAction Fire3 { get; } = new ElementAction(ActionID.Fire3);

        /// <summary>
        /// 火4
        /// </summary>
        public static BaseAction Fire4 { get; } = new ElementAction(ActionID.Fire4)
        {
            OtherCheck = b => JobGauge.InAstralFire,
        };

        /// <summary>
        /// 冰1
        /// </summary>
        public static BaseAction Blizzard { get; } = new ElementAction(ActionID.Blizzard);

        /// <summary>
        /// 冰2
        /// </summary>
        public static BaseAction Blizzard2 { get; } = new ElementAction(ActionID.Blizzard2);

        /// <summary>
        /// 冰3
        /// </summary>
        public static BaseAction Blizzard3 { get; } = new ElementAction(ActionID.Blizzard3);

        /// <summary>
        /// 冰4
        /// </summary>
        public static BaseAction Blizzard4 { get; } = new ElementAction(ActionID.Blizzard4)
        {
            OtherCheck = b =>
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
            OtherCheck = b => JobGauge.InUmbralIce,
        };
    }
}
