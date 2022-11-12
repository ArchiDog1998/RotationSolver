using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;


namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombos
{
    internal abstract partial class BLMCombo<TCmd> : JobGaugeCombo<BLMGauge, TCmd> where TCmd : Enum
    {
        public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlackMage, ClassJobID.Thaumaturge };

        protected static bool HasFire => Player.HaveStatusFromSelf(StatusID.Firestarter);
        protected static bool HasThunder => Player.HaveStatusFromSelf(StatusID.Thundercloud);

        public class ThunderAction : BaseAction
        {
            internal override uint MPNeed => HasThunder ? 0 : base.MPNeed;

            internal ThunderAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false, bool isDot = false)
                : base(actionID, isFriendly, shouldEndSpecial, isDot)
            {
            }
        }
        public static readonly BaseAction
            //雷1
            Thunder = new ThunderAction(144u, isDot: true),
            Thunder3 = new ThunderAction(153u, isDot: true),

            //雷2
            Thunder2 = new ThunderAction(7447u, isDot: true)
            {
                TargetStatus = Thunder.TargetStatus,
            },

            //星灵移位
            Transpose = new(149u) { OtherCheck = b => JobGauge.InUmbralIce || JobGauge.InAstralFire },

            //灵极魂
            UmbralSoul = new(16506u) { OtherCheck = b => JobGauge.InUmbralIce },

            //魔罩
            Manaward = new(157u),

            //魔泉
            Manafont = new(158u),

            //激情咏唱
            Sharpcast = new(3574u)
            {
                BuffsProvide = new[] { StatusID.Sharpcast },
                OtherCheck = b => HaveHostileInRange,
            },

            //三连咏唱
            Triplecast = new(7421u)
            {
                BuffsProvide = Swiftcast.BuffsProvide,
            },

            //黑魔纹
            Leylines = new(3573u, shouldEndSpecial: true)
            {
                BuffsProvide = new[] { StatusID.LeyLines, },
            },

            //魔纹步
            BetweenTheLines = new(7419u, shouldEndSpecial: true)
            {
                BuffsNeed = Leylines.BuffsProvide,
            },

            //以太步
            AetherialManipulation = new(155)
            {
                ChoiceTarget = TargetFilter.FindTargetForMoving,
            },

            //详述
            Amplifier = new(25796u) { OtherCheck = b => JobGauge.EnochianTimer > 10000 },

            //核爆
            Flare = new(162u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 4000 },

            //绝望
            Despair = new(16505u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 },

            //秽浊
            Foul = new(7422u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 },

            //异言
            Xenoglossy = new(16507u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 },

            //崩溃
            Scathe = new(156u),

            //悖论
            Paradox = new(25797u)
            {
                OtherCheck = b => JobGauge.IsParadoxActive,
            },

            //火1
            Fire = new(141u, true),

            //火2
            Fire2 = new(147u, true),

            //火3
            Fire3 = new(152u, true),

            //火4
            Fire4 = new(3577u, true)
            {
                OtherCheck = b => JobGauge.InAstralFire,
            },


            //冰1
            Blizzard = new(142u, false),

            //冰2
            Blizzard2 = new(25793u, false),

            //冰3
            Blizzard3 = new(154u, false),

            //冰4
            Blizzard4 = new(3576u, false)
            {
                OtherCheck = b =>
                {
                    if (IsLastSpell(true, Blizzard4)) return false;

                    if (JobGauge.UmbralHearts == 3) return false;

                    return JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2500 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1);
                }
            },

            //冻结
            Freeze = new(159u, false) { OtherCheck = b => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2800 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) };

    }
}
