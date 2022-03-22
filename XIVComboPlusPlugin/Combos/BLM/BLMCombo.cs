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

        protected struct Actions
        {

            public static readonly BaseAction
                //雷1
                Thunder = new BaseAction(6, 144u, 200)
                {
                    Debuffs = new ushort[]
                {
                    ObjectStatus.Thunder,
                    ObjectStatus.Thunder3,
                },
                    OtherIDs = new uint[] { 153u } //雷3 ID
                },

                //雷2
                Thunder2 = new BaseAction(26, 7447u, 400)
                {
                    Debuffs = new ushort[]
                {
                    ObjectStatus.Thunder2,
                    ObjectStatus.Thunder4,
                },
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
                Fire = new BLMAction(2, 141u, 800, true),

                //火2
                Fire2 = new BLMAction(18, 147u, 1500, true),

                //火3
                Fire3 = new BLMAction(35, 152u, 2000, true),

                //火4
                Fire4 = new BLMAction(60, 3577u, 800, true) { OtherCheck = () => JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 5000 },

                ////高火2
                //HighFire2 = new BLMAction(82, 25794u, 1500, true),

                //冰1
                Blizzard = new BLMAction(1, 142u, 400, false),

                //冰2
                Blizzard2 = new BLMAction(12, 25793u, 800, false),

                //冰3
                Blizzard3 = new BLMAction(35, 154u, 800, false),

                //冰4
                Blizzard4 = new BLMAction(58, 3576u, 800, false) { OtherCheck = () => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2500 * (JobGauge.UmbralIceStacks == 3 ? 0.5: 1)},

                ////高冰2
                //HighBlizzard2 = new BLMAction(82, 25795u, 800, false),

                //冻结
                Freeze = new BLMAction(40, 159u, 1000, false) { OtherCheck = () => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2800 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) },

                //星灵移位
                Transpose = new BaseAction(4, 149u, ability: true) { OtherCheck = () => JobGauge.InUmbralIce || JobGauge.InAstralFire },

                //灵极魂
                UmbralSoul = new BaseAction(76, 16506u, ability: true) { OtherCheck = () => JobGauge.InUmbralIce },

                //魔罩
                Manaward = new BaseAction(30, 157u, ability: true),

                //魔泉
                Manafont = new BaseAction(30, 158u, ability: true) { OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp == 0},

                //激情咏唱
                Sharpcast = new BaseAction(54, 3574u, ability: true),

                //三连咏唱
                Triplecast = new BaseAction(66, 7421u, ability: true)
                {
                    BuffsProvide = new ushort[]
                    {
                        ObjectStatus.Swiftcast1,
                        ObjectStatus.Swiftcast2,
                        ObjectStatus.Triplecast,
                    },
                    OtherCheck = () => JobGauge.InAstralFire && Service.ClientState.LocalPlayer.CurrentMp > 5000,
                },

                //黑魔纹
                Leylines = new BaseAction(52, 3573u, ability: true)
                {
                    BuffsProvide = new ushort[]
                {
                    ObjectStatus.LeyLines,
                }
                },

                //魔纹步
                BetweenTheLines = new BaseAction(62, 7419u, ability: true) { BuffNeed = ObjectStatus.LeyLines },

                //详述
                Amplifier = new BaseAction(86, 25796u, ability: true) { OtherCheck = () => !IsPolyglotStacksFull },

                //核爆
                Flare = new BaseAction(50, 162u, 800) { OtherCheck = () => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 4000 },

                //绝望
                Despair = new BaseAction(72, 16505u, 800) { OtherCheck = () => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 3000 },

                //秽浊
                Foul = new BaseAction(70, 7422u) { OtherCheck = () => JobGauge.PolyglotStacks != 0 },

                //异言
                Xenoglossy = new BaseAction(80, 16507u) { OtherCheck = () => JobGauge.PolyglotStacks != 0 },

                //悖论
                Paradox = new BaseAction(90, 25797u, 1600);
        }

        protected bool CanAddAbility(byte level, out uint action)
        {
            if(Actions.Triplecast.TryUseAction(level, out action)) return true;


            if (CanInsertAbility)
            {
                //加个即刻或者黑魔纹
                if (JobGauge.InAstralFire && LocalPlayer.CurrentMp > 800)
                {
                    if (GeneralActions.Swiftcast.TryUseAction(level, out action)) return true;
                    if (Actions.Leylines.TryUseAction(level, out action)) return true;
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
