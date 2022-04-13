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
        /// <summary>
        /// 判断通晓是否满了。
        /// </summary>
        protected static bool IsPolyglotStacksMaxed
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
        private bool HasFire => BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter));

        internal struct Actions
        {

            public static readonly BaseAction
                //雷1
                Thunder = new BaseAction(144u)
                {
                    TargetStatus = new ushort[]
                {
                    ObjectStatus.Thunder,
                    ObjectStatus.Thunder2,
                    ObjectStatus.Thunder3,
                    ObjectStatus.Thunder4,
                },
                    OtherIDsNot = new uint[] { 153u, 144u, 7420u, 7447u } //雷1,3 ID
                },

                //雷2
                Thunder2 = new BaseAction(7447u)
                {
                    TargetStatus = Thunder.TargetStatus,
                    OtherIDsNot = new uint[] { 153u, 144u, 7420u, 7447u } //雷2,4 ID
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
                Manafont = new BaseAction( 158u) { OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp == 0 && JobGauge.InAstralFire},

                //激情咏唱
                Sharpcast = new BaseAction(3574u),

                //三连咏唱
                Triplecast = new BaseAction(7421u)
                {
                    BuffsProvide = GeneralActions.Swiftcast.BuffsProvide,
                    OtherCheck = () => JobGauge.InAstralFire && JobGauge.UmbralHearts < 2 && JobGauge.ElementTimeRemaining > 10000,
                },

                //黑魔纹
                Leylines = new BaseAction(3573u)
                {
                    BuffsProvide = new ushort[]{ObjectStatus.LeyLines,}, 
                },

                //魔纹步
                BetweenTheLines = new BaseAction(7419u) { BuffsNeed = new ushort[] { ObjectStatus.LeyLines } },

                //以太步
                AetherialManipulation = new BaseAction(155),

                //详述
                Amplifier = new BaseAction(25796u) { OtherCheck = () => !IsPolyglotStacksMaxed && JobGauge.EnochianTimer > 10000},

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

            //加个魔泉
            if (Actions.Manafont.TryUseAction(level, out action))
            {
                return true;
            }

            if (CanInsertAbility)
            {
                //加个醒梦
                if (JobGauge.InUmbralIce && GeneralActions.LucidDreaming.TryUseAction(level, out action)) return true;

                //加个即刻
                if (JobGauge.InAstralFire && LocalPlayer.CurrentMp >= 800 && JobGauge.UmbralHearts < 2)
                {
                    if (GeneralActions.Swiftcast.TryUseAction(level, out action)) return true;
                }

                //加个通晓
                if (Actions.Amplifier.TryUseAction(level, out action)) return true;

                //加个激情
                if (Actions.Sharpcast.TryUseAction(level, out action, mustUse:true)) return true;

                //加个混乱
                if (GeneralActions.Addle.TryUseAction(level, out action)) return true;

                //加个魔罩
                if (Actions.Manaward.TryUseAction(level, out action)) return true;
            }
            return false;
        }

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            uint act;
            if (IsMoving && HaveTargetAngle)
            {
                if(JobGauge.ElementTimeRemaining <= 3000)
                {
                    if (Actions.Transpose.TryUseAction(level, out act)) return act;
                }
                if (Actions.Flare.TryUseAction(level, out act)) return act;
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return act;
                if (Actions.Triplecast.TryUseAction(level, out act, mustUse: true)) return act;
                if (GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return act;
            }


            if (MantainceState(level, lastComboMove, out act)) return act;
            if (CanAddAbility(level, out act)) return act;
            if (AttackAndExchange(level, out act)) return act;
            return GeneralActions.Addle.ActionID;
        }

        private bool AttackAndExchange(byte level, out uint act)
        {
            //如果通晓满了，就放掉。
            if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer < 10000)
            {
                if (Actions.Foul.TryUseAction(level, out act)) return true;
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
            }

            if (JobGauge.InUmbralIce)
            {
                //如果没有火苗且单体有悖论，那打掉！
                if (!HasFire &&
                    JobGauge.IsParadoxActive && Actions.Blizzard.TryUseAction(level, out act)) return true;


                if (Actions.Freeze.TryUseAction(level, out act)) return true;
                if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

                //给我攻击！
                if (JobGauge.PolyglotStacks > 0)
                {
                    if (Actions.Foul.TryUseAction(level, out act)) return true;
                    if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                    if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
                }

                if (Actions.Blizzard4.TryUseAction(level, out act)) return true;
                if (Actions.Blizzard.TryUseAction(level, out act)) return true;
            }
            else if (JobGauge.InAstralFire)
            {
                //如果蓝不够了，赶紧一个绝望。
                if (level >= 58 && JobGauge.UmbralHearts < 2)
                {
                    if (Actions.Flare.TryUseAction(level, out act)) return true;
                }
                if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
                {
                    if (Actions.Despair.TryUseAction(level, out act)) return true;
                }

                //试试看火2
                if (Actions.Fire2.TryUseAction(level, out act)) return true;

                //再试试看核爆
                if (Actions.Flare.TryUseAction(level, out act)) return true;


                //如果MP够打一发伤害。
                if (Service.ClientState.LocalPlayer.CurrentMp >= AttackAstralFire(level, out act))
                {
                    return true;
                }
                //否则，转入冰状态。
                else
                {
                    if (AddUmbralIceStacks(level, out act)) return true;
                }
            }

            act = 0;
            return false;
        }

        /// <summary>
        /// In AstralFire, maintain the time.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private uint AttackAstralFire(byte level, out uint act)
        {
            uint addition = level < Actions.Despair.Level ? 0u : 800u;

            if (Actions.Fire4.TryUseAction(level, out act)) return Actions.Fire4.MPNeed + addition;
            if (Actions.Paradox.TryUseAction(level, out act)) return Actions.Paradox.MPNeed + addition;
            //如果有火苗了，那就来火3
            if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)))
            {
                act = Actions.Fire3.ActionID;
                return addition;
            }
            if (Actions.Fire.TryUseAction(level, out act)) return Actions.Fire.MPNeed + addition;
            return uint.MaxValue;
        }

        /// <summary>
        /// 保证冰火都是最大档数，保证有雷，如果条件允许，赶紧转火。
        /// </summary>
        /// <param name="level"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool MantainceState(byte level, uint lastAct, out uint act)
        {
            if (JobGauge.InUmbralIce)
            {

                if (LocalPlayer.CurrentMp > 9000 && (JobGauge.UmbralHearts == 3 || level < 58))
                {
                    //如果有冰悖论，赶紧打出去！
                    if (!HasFire && JobGauge.IsParadoxActive && Actions.Blizzard.TryUseAction(level, out act)) return true;

                    if (AddAstralFireStacks(level, lastAct, out act)) return true;
                }

                if (AddUmbralIceStacks(level, out act)) return true;
                if (AddUmbralHeartsSingle(level, out act)) return true;
                if (AddThunderSingle(level, lastAct, out act)) return true;
            }
            else if (JobGauge.InAstralFire)
            {
                //如果没蓝了，就直接冰状态。
                if (Service.ClientState.LocalPlayer.CurrentMp == 0 && XIVComboPlusPlugin.LastAction != Actions.Manafont.ActionID)
                {
                    if (AddUmbralIceStacks(level, out act)) return true;
                }

                if (AddAstralFireStacks(level, lastAct, out act)) return true;
                if (AddThunderSingle(level, lastAct, out act)) return true;
            }
            else
            {
                //没状态，就加个冰状态。
                if (AddUmbralIceStacks(level, out act)) return true;
            }

            return false;
        }

        private bool AddUmbralIceStacks(byte level, out uint act)
        {
            //如果冰满了，就别加了。
            act = 0;
            if (JobGauge.UmbralIceStacks > 2 && JobGauge.ElementTimeRemaining > 4000) return false;

            //试试看冰2
            if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

            //如果在火状态，切有火苗的话
            //if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)) && (JobGauge.PolyglotStacks > 0 || 
            //    Service.ClientState.LocalPlayer.CurrentMp > 800))
            //{
            //    if (JobGauge.InAstralFire)
            //    {
            //        //就变成冰状态！
            //        if (CanInsertAbility && Actions.Transpose.TryUseAction(level, out act)) return true;

            //        //创造内插的状态！
            //        if (JobGauge.PolyglotStacks > 0)
            //        {
            //            if (Actions.Foul.TryUseAction(level, out act)) return true;
            //            if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
            //            if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
            //        }

            //        //加个能力技？
            //        if (CanAddAbility(level, out act)) return true;

            //        //试试看冰3
            //        if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

            //    }
            //}
            //else
            {
                //加个能力技？
                if (CanAddAbility(level, out act)) return true;

                //如果有冰悖论，那就上啊！
                if (JobGauge.UmbralIceStacks > 1 && JobGauge.IsParadoxActive && Actions.Blizzard.TryUseAction(level, out act)) return true;

                //试试看冰3
                if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

                //试试看冰1
                if (Actions.Blizzard.TryUseAction(level, out act)) return true;
            }



            return false;
        }

        private bool AddAstralFireStacks(byte level, uint lastaction, out uint act)
        {
            //如果火满了，就别加了。
            act = 0;
            if (JobGauge.AstralFireStacks > 2 && JobGauge.ElementTimeRemaining > 5400) return false;

            if (Service.ClientState.LocalPlayer.CurrentMp < 5000 && lastaction != Actions.Manafont.ActionID)
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }

            //试试看火2
            if (Actions.Fire2.TryUseAction(level, out act)) return true;

            //如果在冰状态，且有火苗的话。
            if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)) && JobGauge.InUmbralIce)
            {
                //就变成火状态！
                if (CanInsertAbility && Actions.Transpose.TryUseAction(level, out act)) return true;

                //创造内插的状态！
                if (JobGauge.PolyglotStacks > 0)
                {
                    if (Actions.Foul.TryUseAction(level, out act)) return true;
                    if (Actions.Xenoglossy.TryUseAction(level, out act)) return true;
                    if (Actions.Foul.TryUseAction(level, out act, mustUse: true)) return true;
                }
            }

            //加个能力技？
            if (CanAddAbility(level, out act)) return true;

            //试试看火3
            if ((JobGauge.InUmbralIce || JobGauge.AstralFireStacks == 1) && Actions.Fire3.TryUseAction(level, out act)) return true;

            //如果时间够火1，并且如果是90级有悖论，那只打悖论出来。
            if (JobGauge.ElementTimeRemaining > 2500 && ((level == 90 && JobGauge.IsParadoxActive) || level < 90))
            {
                if (Actions.Fire.TryUseAction(level, out act)) return true;
            }
            else
            {
                if ((lastaction != Actions.Fire.ActionID || lastaction != 25797) && AddUmbralIceStacks(level, out act)) return true;
            }

            //(level == 90 && JobGauge.IsParadoxActive) || level < 90

            return false;
        }

        private bool AddThunderSingle(byte level, uint lastAct, out uint act)
        {
            //试试看雷2
            if (Actions.Thunder2.TryUseAction(level, out act, lastAct)) return true;

            //加个能力技？
            if (CanAddAbility(level, out act)) return true;

            //试试看雷1
            if (Actions.Thunder.TryUseAction(level, out act, lastAct)) return true;

            return false;
        }

        private bool AddUmbralHeartsSingle(byte level, out uint act)
        {
            //如果满了，或者等级太低，没有冰心，就别加了。
            act = 0;
            if (JobGauge.UmbralHearts == 3 || level < 58) return false;

            //冻结
            if (Actions.Freeze.TryUseAction(level, out act)) return true;

            //加个能力技？
            if (CanAddAbility(level, out act)) return true;

            //冰4
            if (Actions.Blizzard4.TryUseAction(level, out act)) return true;

            return false;
        }
    }
}
