using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal abstract class BLMCombo : CustomComboJob<BLMGauge>
    {
        protected static bool HaveEnoughMP => LocalPlayer.CurrentMp > 9000;

        /// <summary>
        /// 判断通晓是否满了。
        /// </summary>
        protected static bool IsPolyglotStacksFull
        {
            get
            {
                if(Service.ClientState.LocalPlayer.Level < 80)
                {
                    return JobGauge.PolyglotStacks == 1;
                }
                else
                {
                    return JobGauge.PolyglotStacks == 2;
                }
            }
        }

        internal struct Actions
        {

            public static readonly BaseAction
                //雷1
                Thunder = new BaseAction(144u)
                {
                    Debuffs = new ushort[]
                {
                    ObjectStatus.Thunder,
                    ObjectStatus.Thunder2,
                    ObjectStatus.Thunder3,
                    ObjectStatus.Thunder4,
                },
                    OtherIDs = new uint[] { 153u } //雷3 ID
                },

                //雷2
                Thunder2 = new BaseAction(7447u)
                {
                    Debuffs = new ushort[]
                {
                    ObjectStatus.Thunder,
                    ObjectStatus.Thunder2,
                    ObjectStatus.Thunder3,
                    ObjectStatus.Thunder4,                },
                    OtherIDs = new uint[] { 7420u } //雷4 ID
                },

                ////雷3
                //Thunder3 = new BaseAction(45, 153u, 400)
                //{
                //    Debuffs = new ushort[]
                //    {
                //        ObjectStatus.Thunder3,
                //    }
                //},

                ////雷4
                //Thunder4 = new BaseAction(64, 7420u, 400)
                //{
                //    Debuffs = new ushort[]
                //{
                //    ObjectStatus.Thunder4,
                //}
                //},

                //火1
                Fire = new BLMAction(141u, true),

                //火2
                Fire2 = new BLMAction( 147u, true),

                //火3
                Fire3 = new BLMAction(152u, true),

                //火4
                Fire4 = new BLMAction( 3577u,  true) { OtherCheck = () => JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 5000 },

                ////高火2
                //HighFire2 = new BLMAction(82, 25794u, 1500, true),

                //冰1
                Blizzard = new BLMAction( 142u,  false),

                //冰2
                Blizzard2 = new BLMAction( 25793u,  false),

                //冰3
                Blizzard3 = new BLMAction(154u, false),

                //冰4
                Blizzard4 = new BLMAction( 3576u,  false) { OtherCheck = () => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2500 * (JobGauge.UmbralIceStacks == 3 ? 0.5: 1)},

                ////高冰2
                //HighBlizzard2 = new BLMAction(82, 25795u, 800, false),

                //冻结
                Freeze = new BLMAction(159u, false) { OtherCheck = () => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2800 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) },

                //星灵移位
                Transpose = new BaseAction(149u) { OtherCheck = () => JobGauge.InUmbralIce || JobGauge.InAstralFire },

                //灵极魂
                UmbralSoul = new BaseAction(16506u) { OtherCheck = () => JobGauge.InUmbralIce },

                //魔罩
                Manaward = new BaseAction( 157u),

                //魔泉
                Manafont = new BaseAction( 158u) { OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp == 0},

                //激情咏唱
                Sharpcast = new BaseAction(3574u),

                //三连咏唱
                Triplecast = new BaseAction(7421u)
                {
                    BuffsProvide = new ushort[]
                    {
                        ObjectStatus.Swiftcast1,
                        ObjectStatus.Swiftcast2,
                        ObjectStatus.Triplecast,
                    },
                    OtherCheck = () => JobGauge.InAstralFire && Service.ClientState.LocalPlayer.CurrentMp > 5000 && JobGauge.UmbralHearts < 2,
                },

                //黑魔纹
                Leylines = new BaseAction(3573u)
                {
                    BuffsProvide = new ushort[]{ObjectStatus.LeyLines,}, 
                    BuffsNeed = GeneralActions.Swiftcast.BuffsProvide
                },

                //魔纹步
                BetweenTheLines = new BaseAction(7419u) { BuffsNeed = new ushort[] { ObjectStatus.LeyLines } },

                //详述
                Amplifier = new BaseAction(25796u) { OtherCheck = () => !IsPolyglotStacksFull },

                //核爆
                Flare = new BaseAction(162u) { OtherCheck = () => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 4000 },

                //绝望
                Despair = new BaseAction(16505u) { OtherCheck = () => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 3000 },

                //秽浊
                Foul = new BaseAction(7422u) { OtherCheck = () => JobGauge.PolyglotStacks != 0 },

                //异言
                Xenoglossy = new BaseAction(16507u) { OtherCheck = () => JobGauge.PolyglotStacks != 0 },

                //悖论
                Paradox = new BaseAction(25797u);
        }

        protected bool CanAddAbility(byte level, out uint action)
        {
            if(Actions.Triplecast.TryUseAction(level, out action)) return true;


            if (CanInsertAbility)
            {
                //加个即刻或者黑魔纹
                if (JobGauge.InAstralFire && LocalPlayer.CurrentMp > 800 && JobGauge.UmbralHearts <2)
                {
                    if (GeneralActions.Swiftcast.TryUseAction(level, out action)) return true;
                    //if (Actions.Leylines.TryUseAction(level, out action)) return true;
                }

                //加个通晓
                if (Actions.Amplifier.TryUseAction(level, out action)) return true;

                //加个魔泉
                if (Actions.Manafont.TryUseAction(level, out action)) return true;

                //加个激情
                if (Actions.Sharpcast.TryUseAction(level, out action)) return true;

                //加个混乱
                if (GeneralActions.Addle.TryUseAction(level, out action)) return true;

                //加个魔罩
                if (Actions.Manaward.TryUseAction(level, out action)) return true;

                //加个醒梦
                if (GeneralActions.LucidDreaming.TryUseAction(level, out action)) return true;
            }
            return false;
        }
    }
}
