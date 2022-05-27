//using Dalamud.Game.ClientState.JobGauge.Types;

//namespace XIVComboPlus.Combos
//{
//    internal abstract class BLMCombo : CustomComboJob<BLMGauge>
//    {
//        internal override uint JobID => 25;

//        /// <summary>
//        /// 判断通晓是否满了。
//        /// </summary>
//        protected static bool IsPolyglotStacksMaxed
//        {
//            get
//            {
//                if (Service.ClientState.LocalPlayer.Level < 80)
//                {
//                    return JobGauge.PolyglotStacks == 1;
//                }
//                else
//                {
//                    return JobGauge.PolyglotStacks == 2;
//                }
//            }
//        }
//        private bool HasFire => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Firestarter);
//        private bool CanGoFire => JobGauge.InUmbralIce && LocalPlayer.CurrentMp > 9000 && (JobGauge.UmbralHearts == 3 || Service.ClientState.LocalPlayer.Level < 58);
//        internal struct Actions
//        {

//            public static readonly BaseAction
//                //雷1
//                Thunder = new BaseAction(144u)
//                {
//                    TargetStatus = new ushort[]
//                {
//                    ObjectStatus.Thunder,
//                    ObjectStatus.Thunder2,
//                    ObjectStatus.Thunder3,
//                    ObjectStatus.Thunder4,
//                },
//                    OtherIDsNot = new uint[] { 153u, 144u, 7420u, 7447u } //雷1,3 ID
//                },

//                //雷2
//                Thunder2 = new BaseAction(7447u)
//                {
//                    TargetStatus = Thunder.TargetStatus,
//                    OtherIDsNot = new uint[] { 153u, 144u, 7420u, 7447u } //雷2,4 ID
//                },

//                ////雷3
//                //Thunder3 = new BaseAction(45, 153u, 400)
//                //{
//                //    Debuffs = new ushort[]
//                //    {
//                //        ObjectStatus.Thunder3,
//                //    }
//                //},

//                ////雷4
//                //Thunder4 = new BaseAction(64, 7420u, 400)
//                //{
//                //    Debuffs = new ushort[]
//                //{
//                //    ObjectStatus.Thunder4,
//                //}
//                //},

//                //火1
//                Fire = new BLMAction(141u, true),

//                //火2
//                Fire2 = new BLMAction(147u, true),

//                //火3
//                Fire3 = new BLMAction(152u, true),

//                //火4
//                Fire4 = new BLMAction(3577u, true) { OtherCheck = b => JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 5000 },

//                ////高火2
//                //HighFire2 = new BLMAction(82, 25794u, 1500, true),

//                //冰1
//                Blizzard = new BLMAction(142u, false),

//                //冰2
//                Blizzard2 = new BLMAction(25793u, false),

//                //冰3
//                Blizzard3 = new BLMAction(154u, false),

//                //冰4
//                Blizzard4 = new BLMAction(3576u, false) { OtherCheck = b => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2500 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) },

//                ////高冰2
//                //HighBlizzard2 = new BLMAction(82, 25795u, 800, false),

//                //冻结
//                Freeze = new BLMAction(159u, false) { OtherCheck = b => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2800 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) },

//                //星灵移位
//                Transpose = new BaseAction(149u) { OtherCheck = b => JobGauge.InUmbralIce || JobGauge.InAstralFire },

//                //灵极魂
//                UmbralSoul = new BaseAction(16506u) { OtherCheck = b => JobGauge.InUmbralIce },

//                //魔罩
//                Manaward = new BaseAction(157u),

//                //魔泉
//                Manafont = new BaseAction(158u) { OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp == 0 && JobGauge.InAstralFire },

//                //激情咏唱
//                Sharpcast = new BaseAction(3574u)
//                {
//                    BuffsProvide = new ushort[] { ObjectStatus.Sharpcast }
//                },

//                //三连咏唱
//                Triplecast = new BaseAction(7421u)
//                {
//                    BuffsProvide = GeneralActions.Swiftcast.BuffsProvide,
//                    //OtherCheck = () => JobGauge.InAstralFire && JobGauge.UmbralHearts < 2 && JobGauge.ElementTimeRemaining > 10000,
//                },

//                //黑魔纹
//                Leylines = new BaseAction(3573u)
//                {
//                    BuffsProvide = new ushort[] { ObjectStatus.LeyLines, },
//                },

//                //魔纹步
//                BetweenTheLines = new BaseAction(7419u) { BuffsNeed = new ushort[] { ObjectStatus.LeyLines } },

//                //以太步
//                AetherialManipulation = new BaseAction(155),

//                //详述
//                Amplifier = new BaseAction(25796u) { OtherCheck = b => !IsPolyglotStacksMaxed && JobGauge.EnochianTimer > 10000 },

//                //核爆
//                Flare = new BaseAction(162u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 4000 },

//                //绝望
//                Despair = new BaseAction(16505u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 3000 },

//                //秽浊
//                Foul = new BaseAction(7422u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 },

//                //异言
//                Xenoglossy = new BaseAction(16507u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 },

//                //悖论
//                Paradox = new BaseAction(25797u);
//        }

//        private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
//        {
//            //加个魔泉
//            if (abilityRemain == 1)
//            {
//                if (Actions.Manafont.ShouldUseAction(out act)) return true;

//                if (HasFire)
//                {
//                    if (JobGauge.InAstralFire && LocalPlayer.CurrentMp < 800)
//                    {
//                        if (Actions.Transpose.ShouldUseAction(out act)) return true;
//                    }
//                    else if (JobGauge.InUmbralIce && (LocalPlayer.CurrentMp > 9000 || (JobGauge.PolyglotStacks == 0 && LocalPlayer.CurrentMp > 7200)))
//                    {
//                        if (Actions.Transpose.ShouldUseAction(out act)) return true;
//                    }
//                }
//            }

//            if (IsMoving)
//            {
//                if (JobGauge.InAstralFire && (LocalPlayer.CurrentMp < 5000 || JobGauge.ElementTimeRemaining < 5000))
//                {
//                    if (Actions.Transpose.ShouldUseAction(out act)) return true;
//                }
//                if (Actions.Triplecast.ShouldUseAction(out act, mustUse: true)) return true;
//                if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
//            }

//            //加个醒梦
//            if (JobGauge.InUmbralIce && HasFire && GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;


//            if (Actions.Triplecast.ShouldUseAction(out act)) return true;


//            //加个即刻
//            if (JobGauge.InAstralFire && LocalPlayer.CurrentMp >= 800 && JobGauge.UmbralHearts < 2)
//            {
//                if (GeneralActions.Swiftcast.ShouldUseAction(out act)) return true;
//            }

//            //加个通晓
//            if (Actions.Amplifier.ShouldUseAction(out act)) return true;

//            //加个激情
//            if (Actions.Sharpcast.ShouldUseAction(out act, Empty: true)) return true;



//            return false;
//        }

//        private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
//        {
//            if (IsMoving && HaveTargetAngle)
//            {
//                if (AddPolyglotAttach(level, out act)) return true;

//                if (Actions.Triplecast.ShouldUseAction(level, out act, mustUse: true)) return true;
//                if (GeneralActions.Swiftcast.ShouldUseAction(level, out act, mustUse: true)) return true;
//            }

//            //冰状态
//            if (JobGauge.InUmbralIce)
//            {
//                //转火状态
//                if (CanGoFire)
//                {
//                    //火状态
//                    if (HasFire && !Actions.Manafont.CoolDown.IsCooldown)
//                    {
//                        if (AddPolyglotAttach(level, out act)) return true;
//                    }
//                    else
//                    {
//                        //把冰悖论放掉
//                        if (JobGauge.IsParadoxActive && Actions.Blizzard.ShouldUseAction(level, out act)) return true;
//                    }


//                    //进入火状态
//                    //试试看火2,3
//                    if (Actions.Fire2.ShouldUseAction(level, out act)) return true;
//                    if (Actions.Fire3.ShouldUseAction(level, out act)) return true;
//                }

//                //常规攻击
//                if (HasFire)
//                {
//                    //加冰心
//                    if (AddUmbralHearts(level, out act)) return true;

//                    //创造内插的状态！
//                    if (AddPolyglotAttach(level, out act)) return true;

//                    //把冰悖论放掉
//                    if (JobGauge.IsParadoxActive && Actions.Blizzard.ShouldUseAction(level, out act)) return true;

//                    //试试看冰2,3
//                    if (Actions.Blizzard2.ShouldUseAction(level, out act)) return true;
//                    if (Actions.Blizzard4.ShouldUseAction(level, out act)) return true;
//                }
//                else
//                {
//                    //如果通晓满了，就放掉。
//                    if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer < 10000)
//                    {
//                        if (AddPolyglotAttach(level, out act)) return true;
//                    }
//                    //补雷补心
//                    if (AddThunder(level, lastComboActionID, out act)) return true;
//                    if (AddUmbralHearts(level, out act)) return true;

//                    //那填充一下
//                    if (AddPolyglotAttach(level, out act)) return true;
//                }
//            }
//            //火状态
//            else if (JobGauge.InAstralFire)
//            {
//                //如果需要续时间,提高档数
//                if (JobGauge.ElementTimeRemaining < 5400 || JobGauge.AstralFireStacks == 2)
//                {
//                    if (Actions.Fire.ShouldUseAction(level, out act)) return true;
//                }
//                if (JobGauge.AstralFireStacks == 1)
//                {
//                    if (Actions.Fire3.ShouldUseAction(level, out act)) return true;
//                    if (Actions.Fire.ShouldUseAction(level, out act)) return true;
//                }
//                //补雷，如果需要
//                if (AddThunder(level, lastComboActionID, out act)) return true;

//                //如果通晓满了，就放掉。
//                if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer < 10000)
//                {
//                    if (AddPolyglotAttach(level, out act)) return true;
//                }

//                //如果蓝不够了，赶紧一个绝望。
//                if (level >= 58 && JobGauge.UmbralHearts < 2)
//                {
//                    if (Actions.Flare.ShouldUseAction(level, out act)) return true;
//                }
//                if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
//                {
//                    if (Actions.Despair.ShouldUseAction(level, out act)) return true;
//                }

//                //试试看火2
//                if (Actions.Fire2.ShouldUseAction(level, out act)) return true;

//                //再试试看核爆
//                if (Actions.Flare.ShouldUseAction(level, out act)) return true;


//                //如果MP够打一发伤害。
//                if (Service.ClientState.LocalPlayer.CurrentMp >= AttackAstralFire(level, out act))
//                {
//                    if (Actions.Triplecast.ShouldUseAction(level, out BaseAction action)) act = action;
//                    return true;
//                }
//                //否则，转入冰状态。
//                else
//                {
//                    if (HasFire && AddPolyglotAttach(level, out act)) return true;
//                }
//            }

//            //进入冰状态
//            //试试看冰2,3,1给个冰状态
//            if (Actions.Blizzard2.ShouldUseAction(level, out act)) return true;
//            if (Actions.Blizzard3.ShouldUseAction(level, out act)) return true;
//            if (Actions.Blizzard.ShouldUseAction(level, out act)) return true;

//            return false;
//        }

//        /// <summary>
//        /// In AstralFire, maintain the time.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <param name="act"></param>
//        /// <returns></returns>
//        private uint AttackAstralFire(byte level, out BaseAction act)
//        {
//            uint addition = level < Actions.Despair.Level ? 0u : 800u;

//            if (Actions.Fire4.TryUseAction(level, out act)) return Actions.Fire4.MPNeed + addition;
//            if (Actions.Paradox.TryUseAction(level, out act)) return Actions.Paradox.MPNeed + addition;
//            //如果有火苗了，那就来火3
//            if (HasFire)
//            {
//                act = Actions.Fire3;
//                return addition;
//            }
//            if (Actions.Fire.TryUseAction(level, out act)) return Actions.Fire.MPNeed + addition;
//            return uint.MaxValue;
//        }

//        private bool AddThunder(byte level, uint lastAct, out BaseAction act)
//        {
//            //试试看雷2
//            if (Actions.Thunder2.TryUseAction(level, out act, lastAct)) return true;

//            //试试看雷1
//            if (Actions.Thunder.TryUseAction(level, out act, lastAct)) return true;

//            return false;
//        }

//        private bool AddUmbralHearts(byte level, out BaseAction act)
//        {
//            //如果满了，或者等级太低，没有冰心，就别加了。
//            act = null;
//            if (JobGauge.UmbralHearts == 3 || level < 58) return false;

//            //冻结
//            if (Actions.Freeze.TryUseAction(level, out act)) return true;

//            //冰4
//            if (Actions.Blizzard4.TryUseAction(level, out act)) return true;

//            return false;
//        }

//        private bool AddPolyglotAttach(out BaseAction act)
//        {
//            if (JobGauge.PolyglotStacks > 0)
//            {
//                if (Actions.Foul.ShouldUseAction(out act)) return true;
//                if (Actions.Xenoglossy.ShouldUseAction(out act)) return true;
//                if (Actions.Foul.ShouldUseAction(out act, mustUse: true)) return true;
//            }
//            act = null;
//            return false;
//        }
//    }
//}
