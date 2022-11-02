using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
//using static XIVAutoAttack.Helpers.BLMHelper;

namespace XIVAutoAttack.Combos.RangedMagicial
{
    internal sealed class BLMCombo : JobGaugeCombo<BLMGauge>
    {
        internal override uint JobID => 25;

        protected override bool CanHealSingleAbility => false;


        private static int HaveCount = 0;

        private static bool inOpener = false;
        //internal static bool openerFinished = false;
        private static bool iceOpener = false;
        private static bool fireOpener = true;
        private static string MyCommand = "";
        private static DateTime TransposeElapsed = new();
        private static double DespairElapsed => (DateTime.Now - DespairUpdate).TotalMilliseconds;
        private static DateTime DespairUpdate = new();
        private static double Mpyupan = 0;
        private static double MPNextUpInCurrGCD = 0;
        private static double MPUpdateElapsed = 0;
        private static double MPYuPanDouble = 0;
        private static string loopName = "";
        private static uint MpInFire = 0;
        internal static bool IsPolyglotStacksMaxed => Actions.Xenoglossy.EnoughLevel ? JobGauge.PolyglotStacks == 2 : JobGauge.PolyglotStacks == 1;
        internal static bool HasFire => Player.HaveStatus(ObjectStatus.Firestarter);
        internal static bool HasThunder => Player.HaveStatus(ObjectStatus.Thundercloud);
        internal static bool TargetHasThunder => Target.HaveStatus(ObjectStatus.Thunder, ObjectStatus.Thunder2, ObjectStatus.Thunder3, ObjectStatus.Thunder4);

        internal static Watcher.ActionRec[] RecordSpells => Watcher.RecordActions.Where(b => b.action.ActionCategory.Value.RowId == 2).ToArray();
        internal static bool IsOldSpell(int count, IAction action) => RecordSpells[count].action.RowId == action.ID;
        internal static bool TargetThunderWillEnd(float time) => Target.WillStatusEnd(time, false, ObjectStatus.Thunder, ObjectStatus.Thunder2, ObjectStatus.Thunder3, ObjectStatus.Thunder4);
        /// <summary>
        /// 计算魔法的咏唱或GCD时间
        /// </summary>
        /// <param name="GCDTime"></param>
        /// <param name="isSpell"></param>
        /// <param name="isSpellTime"></param>
        /// <returns></returns>
        internal static double CalcSpellTime(double GCDTime, bool isSpell = true, bool isSpellTime = true) => CooldownHelper.CalcSpellTime(GCDTime, isSpell, isSpellTime);
        /// <summary>
        /// 计算咏速是否可以打几个火4
        /// </summary>
        /// <param name="f4Count"></param>
        /// <returns></returns>
        internal static bool CanF4Number(int f4Count, bool hasFire = true) => (hasFire ? CalcSpellTime(2500, isSpell: false) : -550) + CalcSpellTime(2800) * f4Count + CalcSpellTime(3000, isSpellTime: false) <= 15000;
        /// <summary>
        /// 准备进冰阶段预测异言和雷的数量
        /// </summary>
        /// <param name="isInIce"></param>
        /// <returns></returns>      
        internal static int HaveNotSpellSkillCount(bool isInIce = true)
        {

            var count = 0;
            var GCDTime = (float)CalcSpellTime(2500, isSpell: false);
            //当前异言数量
            count += JobGauge.PolyglotStacks;
            //冰阶段可用异言和雷数量
            if (isInIce && !JobGauge.InUmbralIce)
            {
                if (HasThunder && (TargetHasThunder && TargetThunderWillEnd(GCDTime * 3 / 1000) || !TargetHasThunder))
                {
                    count += 1;
                }
                if (JobGauge.EnochianTimer <= GCDTime * 2)
                {
                    count += 1;
                }
                return count;
            }
            else
            {
                return count;
            }


        }
        /// <summary>
        /// 技能序列后在跳蓝条经过时间的位置
        /// </summary>
        /// <param name="time">序列总时间</param>
        /// <returns></returns>
        internal static double MPYuCeDian(double time) => 3 - (ActionUpdater.MPUpdateElapsed + time) % 3;

        /// <summary>
        /// 倒数第二个F4计算那个循环可以良性循环
        /// </summary>
        /// <returns></returns>
        internal static int LiangXing()
        {
            //F4 F4 De Xe Pa  
            var aTime = CalcSpellTime(2800) * 2 + CalcSpellTime(3000) + GCDTime * 2;
            //F4 F4 Xe De Pa
            var bTime = CalcSpellTime(2800) * 2 + GCDTime * 3;
            //F4 Xe F4 De Pa
            var cTime = GCDTime * 3;

            var f3Time = GCDTime + CalcSpellTime(3500) + CalcSpellTime(2800) * 4 + CalcSpellTime(3000);

            //悖论后到星灵前时间
            var tatolTime = GCDTime * 2 - 0.8;
            //悖论到长火3判定点时间 ,星灵到长火3必定可以跳两次蓝
            var tatolTimeF3P = GCDTime + CalcSpellTime(3500, isSpellTime: false);

            //悖论后跳两次蓝所用时间
            var aMpE = 6 - (ActionUpdater.MPUpdateElapsed + aTime) % 3;
            var bMpE = 6 - (ActionUpdater.MPUpdateElapsed + bTime) % 3;
            var cMpE = 6 - (ActionUpdater.MPUpdateElapsed + cTime) % 3;

            //
            if (aMpE < tatolTime) return 1;
            if (bMpE < tatolTime) return 2;
            if (cMpE < tatolTime) return 3;

            return 0;
        }

        /// <summary>
        /// GCD间隔总时间
        /// </summary>
        internal static double GCDTime => CalcSpellTime(2500, isSpell: false);

        internal static bool CanJudgeXe()
        {
            //悖论后
            if (JobGauge.ElementTimeRemaining <= CalcSpellTime(3000) + CalcSpellTime(2800)) return true;

            // 0   1    2    3    4    5
            //800 2400 4000 5600 7200 8800
            if (Player.CurrentMp < 4000) return true;

            return false;
        }

        /// <summary>
        /// 当前火状态还能打几个火4
        /// </summary>
        /// <returns></returns>
        internal static byte F4RemainingNumber()
        {
            if (!JobGauge.InAstralFire) return 0;
            var mpCount = (byte)((Player.CurrentMp - 800) / Actions.Fire4.MPNeed);
            var timeCountDe = (byte)((JobGauge.ElementTimeRemaining - CalcSpellTime(3000, isSpellTime: false)) / CalcSpellTime(2800));
            var timeCountPe = (byte)((JobGauge.ElementTimeRemaining - CalcSpellTime(2500, isSpellTime: false)) / CalcSpellTime(2800));
            if (JobGauge.IsParadoxActive) return Math.Min(mpCount, timeCountPe);
            else return Math.Min(mpCount, timeCountDe);
        }

        //internal static void UpdateInfo()
        //{
        //    //Service.ChatGui.Print("++++" + Mpyupan);

        //    if (JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 14700)
        //    {
        //        MpInFire = Player.CurrentMp;
        //    }
        //    if (JobGauge.InUmbralIce && IsLastAction(true, Actions.Transpose))
        //    {
        //        TransposeElapsed = DateTime.Now;
        //    }

        //    //记录绝望使用时时间 JobGauge.InAstralFire &&  && (DateTime.Now - DespairUpdate).TotalMilliseconds > 10000
        //    if (IsLastSpell(true, Actions.Despair) && (DateTime.Now - DespairUpdate).TotalMilliseconds > GCDTime * 1.5)
        //    {
        //        DespairUpdate = DateTime.Now;
        //    }

        //    //跳蓝判定点计算
        //    if (JobGauge.InAstralFire && Actions.Transpose.IsCoolDown && !Actions.Transpose.ElapsedAfter(0.1f, false))
        //    {
        //        MPNextUpInCurrGCD = ActionUpdater.MPNextUpInCurrGCD * 1000;
        //        Mpyupan = ActionUpdater.WeaponElapsed * 1000;// - (15000 - JobGauge.ElementTimeRemaining);
        //    }
        //    //瞬发计数
        //    if ((DateTime.Now - DespairUpdate).TotalMilliseconds < 100)
        //    {
        //        HaveCount = HaveNotSpellSkillCount();
        //    }
        //    MPYuCe(2);
        //}

        //private static double MPYuCe(int count)
        //{
        //    //双星灵蓝量预测(两瞬发情况下)
        //    MPYuPanDouble = 0;

        //    if (MPNextUpInCurrGCD >= Mpyupan && MPNextUpInCurrGCD <= CalcSpellTime(2500, false))
        //    {
        //        MPYuPanDouble += 3200;
        //    }
        //    if (MPNextUpInCurrGCD > CalcSpellTime(2500, false))
        //    {
        //        MPYuPanDouble += 4700;
        //    }

        //    if (MPNextUpInCurrGCD + 3000 > CalcSpellTime(2500, false) && MPNextUpInCurrGCD + 6000 > CalcSpellTime(2500, false) * 3 - 0.8)
        //    {
        //        MPYuPanDouble += 4700;
        //    }
        //    if (MPNextUpInCurrGCD + 3000 > CalcSpellTime(2500, false) && MPNextUpInCurrGCD + 6000 < CalcSpellTime(2500, false) * 3 - 0.8)
        //    {
        //        MPYuPanDouble += 9400;
        //    }

        //    return MPYuPanDouble;
        //}

        //internal static double LoopFireTime()
        //{
        //    var Mp = 
        //    return 0;
        //}
        internal struct Actions
        {

            public static readonly BaseAction
                //雷1
                Thunder = new(144u, isDot: true),

                //雷2
                Thunder2 = new(7447u, isDot: true)
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
                    BuffsProvide = new[] { ObjectStatus.Sharpcast }
                },

                //三连咏唱
                Triplecast = new(7421u)
                {
                    BuffsProvide = GeneralActions.Swiftcast.BuffsProvide,
                },

                //黑魔纹
                Leylines = new(3573u, shouldEndSpecial: true)
                {
                    BuffsProvide = new[] { ObjectStatus.LeyLines, },
                },

                //魔纹步
                BetweenTheLines = new(7419u, shouldEndSpecial: true)
                {
                    BuffsNeed = new[] { ObjectStatus.LeyLines },
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
        };

        internal override SortedList<DescType, string> Description => new()
        {
            { DescType.单体治疗, $"{Actions.BetweenTheLines}, {Actions.Leylines}, 这个很特殊！" },
            { DescType.单体防御, $"{Actions.Manaward}" },
            { DescType.移动技能, $"{Actions.AetherialManipulation}，目标为面向夹角小于30°内最远目标。" },
        };

        //private protected override ActionConfiguration CreateConfiguration()
        //{
        //    return base.CreateConfiguration()
        //        .SetBool("AutoLeylines", true, "自动上黑魔纹")
        //        .SetBool("StartFire", false, "火起手")
        //        .SetFloat("TimeToAdd", 5.4f, 3, 8, "火阶段还剩几秒时补时间", 0.01f);
        //}

        internal override string OnCommand(string args)
        {
            MyCommand = args;

            return Mpyupan + "+" + MPYuPanDouble + "+" + DespairElapsed + "+" + HaveCount;
        }

        private bool CommandManager(out IAction act)
        {
            act = null;
            return false;
        }

        private protected override void UpdateInfo()
        {
            //Service.ChatGui.Print("++++" + Mpyupan);

            if (JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 14700)
            {
                MpInFire = Player.CurrentMp;
            }
            if (JobGauge.InUmbralIce && IsLastAction(true, Actions.Transpose))
            {
                TransposeElapsed = DateTime.Now;
            }

            //记录绝望使用时时间 JobGauge.InAstralFire &&  && (DateTime.Now - DespairUpdate).TotalMilliseconds > 10000
            if (IsLastSpell(true, Actions.Despair) && (DateTime.Now - DespairUpdate).TotalMilliseconds > GCDTime * 1.5)
            {
                DespairUpdate = DateTime.Now;
            }

            //跳蓝判定点计算
            if (JobGauge.InAstralFire && Actions.Transpose.IsCoolDown && !Actions.Transpose.ElapsedAfter(0.1f, false))
            {
                MPNextUpInCurrGCD = ActionUpdater.MPNextUpInCurrGCD * 1000;
                Mpyupan = ActionUpdater.WeaponElapsed * 1000;// - (15000 - JobGauge.ElementTimeRemaining);
            }
            //瞬发计数
            if ((DateTime.Now - DespairUpdate).TotalMilliseconds < 100)
            {
                HaveCount = HaveNotSpellSkillCount();
            }
            MPYuCe(2);

            base.UpdateInfo();
        }

        private double MPYuCe(int count)
        {
            //双星灵蓝量预测(两瞬发情况下)
            MPYuPanDouble = 0;

            if (MPNextUpInCurrGCD >= Mpyupan && MPNextUpInCurrGCD <= CalcSpellTime(2500, false))
            {
                MPYuPanDouble += 3200;
            }
            if (MPNextUpInCurrGCD > CalcSpellTime(2500, false))
            {
                MPYuPanDouble += 4700;
            }

            if (MPNextUpInCurrGCD + 3000 > CalcSpellTime(2500, false) && MPNextUpInCurrGCD + 6000 > CalcSpellTime(2500, false) * 3 - 0.8)
            {
                MPYuPanDouble += 4700;
            }
            if (MPNextUpInCurrGCD + 3000 > CalcSpellTime(2500, false) && MPNextUpInCurrGCD + 6000 < CalcSpellTime(2500, false) * 3 - 0.8)
            {
                MPYuPanDouble += 9400;
            }

            return MPYuPanDouble;
        }

        private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
        {
            if (Actions.BetweenTheLines.ShouldUse(out act)) return true;
            if (Actions.Leylines.ShouldUse(out act, mustUse: true)) return true;

            return base.HealSingleAbility(abilityRemain, out act);
        }

        private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
        {
            //加个魔罩
            if (Actions.Manaward.ShouldUse(out act)) return true;

            return base.DefenceSingleAbility(abilityRemain, out act);
        }

        private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
        {
            //混乱
            if (GeneralActions.Addle.ShouldUse(out act)) return true;
            return false;
        }

        private protected override bool MoveAbility(byte abilityRemain, out IAction act)
        {
            if (Actions.AetherialManipulation.ShouldUse(out act, mustUse: true)) return true;

            return base.MoveAbility(abilityRemain, out act);
        }

        private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            if (nextGCD == Actions.Thunder && JobGauge.InUmbralIce)
            {
                if (Actions.Sharpcast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }


            act = null;
            return false;
        }

        private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
        {
            //if (IsLastSpell(true, Actions.Despair) && Actions.Xenoglossy.ShouldUse(out act)) return true;
            //Service.ChatGui.Print("" + IsLastSpell(true, Actions.Despair));
            //Service.ChatGui.Print("++++" + (IsLastSpell(true, Actions.Despair) && HasThunder));
            //if (IsLastSpell(true, Actions.Blizzard3) && ActionUpdater.MPUpInCurrGCD < 3)
            //{
            //Service.ChatGui.Print("++++" + HasThunder);
            //}

            //Service.ChatGui.Print("" + Actions.Fire.RecastTimeElapsed + Actions.Fire.ElapsedAfter((float)ActionUpdater.MPUpdateElapsed, false));
            //if (Actions.Paradox.RecastTimeElapsed - ActionUpdater.MPUpdateElapsed > 0)
            //    Actions.Paradox.ElapsedAfter((float)ActionUpdater.MPUpdateElapsed, false);
            //if (Actions.Triplecast.ShouldUse(out act, mustUse: true) && fireOpener && !inOpener && IsLastAbility(Actions.Leylines.ID) && IsLastSpell(Actions.Despair.ID)) return true;

            if (OpenerManager(out act)) return true;
            //if (IsLastSpell(true, Actions.Despair))
            //{
            //    Service.ChatGui.Print("4" + IsLastSpell(true, Actions.Despair));
            //    if (Actions.Xenoglossy.ShouldUse(out act)) return true;
            //}
            //循环
            //if (LoopManager(out act)) return true;

            //if (AAA(out act)) return true;
            if (JobGauge.InUmbralIce)
            {
                //雷
                if (CanUseThunder(out act)) return true;
                //冰阶段
                if (JobGauge.UmbralIceStacks == 3 && Actions.Blizzard4.ShouldUse(out act)) return true;
                //异言
                if (CanUseXenoglossy(out act)) return true;
                //悖论
                if (CanUseParadox(out act)) return true;
                //火3
                if (CanUseFire3(out act)) return true;
            }
            else if (JobGauge.InAstralFire)
            {
                //悖论
                if (CanUseParadox(out act)) return true;
                //火3
                if (CanUseFire3(out act)) return true;
                //雷
                if (CanUseThunder(out act)) return true;
                //异言
                if (CanUseXenoglossy(out act)) return true;
                //火4
                if (CanUseFire4(out act)) return true;
                //绝望
                if (CanUseDespair(out act)) return true;
                //冰3转冰
                if (JobGauge.InAstralFire && !CanUseFire4(out _) && !CanUseDespair(out _) && (JobGauge.ElementTimeRemaining < CooldownHelper.CalcSpellTime(3000) - 0.5 || Player.CurrentMp <= 1200) && !JobGauge.IsParadoxActive && (Actions.Manafont.ElapsedAfter(3, false) || !Actions.Manafont.IsCoolDown) && Actions.Blizzard3.ShouldUse(out act)) return true;
            }
            else
            {
                //火3
                if (CanUseFire3(out act)) return true;
            }

            //act = null;
            return false;
        }

        private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
        {


            return base.GeneralAbility(abilityRemain, out act);
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            if (fireOpener && inOpener && Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true) && Actions.Leylines.IsCoolDown) return true;

            //三连咏唱
            if (CanUseTriplecast(out act)) return true;
            //魔泉
            if (Actions.Manafont.ShouldUse(out act) && IsLastSpell(true, Actions.Despair, Actions.Xenoglossy) && Player.CurrentMp == 0 && JobGauge.InAstralFire) return true;
            //星灵移位
            //if (CanUseTranspose(abilityRemain, out act)) return true;
            //醒梦
            if (CanUseLucidDreaming(out act)) return true;
            //即刻
            if (CanUseSwiftcast(out act)) return true;

            //激情咏唱
            if (Actions.Sharpcast.ShouldUse(out act, emptyOrSkipCombo: true))
            {
                //if (Player.HaveStatus(ObjectStatus.Triplecast) && Player.FindStatusStack(ObjectStatus.Triplecast) <= 1) return true;

                if (!inOpener && JobGauge.InUmbralIce && !JobGauge.IsParadoxActive) return true;

                if (!inOpener && JobGauge.InAstralFire) return true;
            }
            //Service.ChatGui.Print("1++++" + Player.FindStatusStack(ObjectStatus.Triplecast));


            //黑魔纹
            if (Actions.Leylines.ShouldUse(out act))
            {
                if (Player.HaveStatus(ObjectStatus.Triplecast) && Player.FindStatusStack(ObjectStatus.Triplecast) <= 1) return true;

                if (!inOpener && JobGauge.InAstralFire) return true;
                //return true;
            }
            //详述
            if (Actions.Amplifier.ShouldUse(out act)) return true;


            act = null;
            return false;
        }

        /// <summary>
        /// 起手管理
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool OpenerManager(out IAction act)
        {
            if (!InCombat)
            {
                inOpener = true;
                //激情咏唱
                if (Actions.Sharpcast.ShouldUse(out act) && HaveHostileInRange) return true;
            }
            if (InCombat && inOpener)
            {
                if (!JobGauge.IsEnochianActive)
                {
                    inOpener = false;
                }
                if (IsLastAction(true, Actions.Despair))
                {
                    inOpener = false;
                }
            }
            if (iceOpener && inOpener)
            {
                //三连
                if (Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true))
                {
                    if (IsLastAction(true, Actions.Fire3) || IsLastSpell(true, Actions.Xenoglossy)) return true;
                }
            }

            if (fireOpener && inOpener)
            {
                //三连
                if (IsLastAction(true, Actions.Fire4) && Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;


            }

            act = null;
            return false;
        }

        /// <summary>
        /// 循环管理
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool LoopManager(out IAction act)
        {
            if (JobGauge.InUmbralIce)
            {
                //雷
                if (CanUseThunder(out act)) return true;
                //冰阶段
                if (JobGauge.UmbralIceStacks == 3 && Actions.Blizzard4.ShouldUse(out act)) return true;
                //异言
                if (CanUseXenoglossy(out act)) return true;
                //悖论
                if (CanUseParadox(out act)) return true;
                //火3
                if (CanUseFire3(out act)) return true;
            }
            else if (JobGauge.InAstralFire)
            {
                //悖论
                if (CanUseParadox(out act)) return true;
                //火3
                if (CanUseFire3(out act)) return true;
                //雷
                if (CanUseThunder(out act)) return true;
                //异言
                if (CanUseXenoglossy(out act)) return true;
                //火4
                if (CanUseFire4(out act)) return true;
                //绝望
                if (CanUseDespair(out act)) return true;
                //冰3转冰
                //if (JobGauge.InAstralFire && !CanUseFire4(out _) && !CanUseDespair(out _) && (JobGauge.ElementTimeRemaining < CooldownHelper.CalcSpellTime(3000) - 0.5 || Player.CurrentMp <= 1200) && !JobGauge.IsParadoxActive && (Actions.Manafont.ElapsedAfter(3, false) || !Actions.Manafont.IsCoolDown) && Actions.Blizzard3.ShouldUse(out act)) return true;
            }
            else
            {
                //火3
                if (CanUseFire3(out act)) return true;
            }
            act = null;
            return false;
        }
        //private bool GoIce(out IAction act , bool isGCD = true)
        //{
        //    act = null;
        //    if (!JobGauge.InAstralFire) return false;
        //    //使用魔泉后不转冰
        //    if (!Actions.Manafont.ElapsedAfter(3, false) && Actions.Manafont.IsCoolDown) return false;

        //    //绝望后星灵转冰
        //    if (IsLastSpell(true, Actions.Despair))
        //    {
        //        if (HasFire)
        //        {
        //            if (HaveCount >= 2)
        //            {
        //                //星灵移位
        //                if (!isGCD && Actions.Transpose.ShouldUse(out act)) return true;
        //                //异言
        //                if (isGCD && Actions.Xenoglossy.ShouldUse(out act)) return true;
        //            }
        //        }
        //    }

        //    //冰3转冰
        //    if (!CanUseFire4(out _) && !CanUseDespair(out _) && (JobGauge.ElementTimeRemaining < CalcSpellTime(3000, isSpellTime: false) || Player.CurrentMp < 800) && !JobGauge.IsParadoxActive )
        //    {
        //        if (isGCD && Actions.Blizzard3.ShouldUse(out act)) return true;
        //    }


        //    return false;
        //}
        //private bool GoFire(out IAction act)
        //{
        //    act = null;
        //    if (!JobGauge.InUmbralIce) return false; 


        //    //标准循环-改
        //    if (JobGauge.UmbralIceStacks == 3 && JobGauge.UmbralHearts == 3 && !JobGauge.IsParadoxActive && Player.CurrentMp >= 9600)
        //    {
        //        if (HasFire && Actions.Transpose.ShouldUse(out act)) return true;
        //        if (Actions.Fire3.ShouldUse(out act)) return true;
        //    }
        //    //双星灵
        //    if (JobGauge.ElementTimeRemaining <= 15 - CalcSpellTime(2500, false) * 2)
        //    {

        //    }

        //    return false;
        //}

        /// <summary>
        /// 星灵移位
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseTranspose(byte abilityRemain, out IAction act)
        {
            if (!Actions.Transpose.ShouldUse(out act, mustUse: true)) return false;

            //星灵转火
            if (JobGauge.InUmbralIce)
            {
                if (!JobGauge.IsParadoxActive && JobGauge.UmbralHearts != 3)
                {
                    //瞬双2,长双2(2-1)
                    if (Player.CurrentMp >= 8000 && !HasFire && IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder) && IsOldSpell(1, Actions.Paradox))
                    {
                        // && (!GeneralActions.Swiftcast.IsCoolDown || HaveSwift)
                        return true;
                    }
                    //瞬双3,长双3(2/3-1)
                    if (Player.CurrentMp >= 9600 && !HasFire)
                    {
                        return true;
                    }

                    //if (IsLastSpell(true, Actions.Paradox)) return false;

                    //火双3
                    if (Player.CurrentMp >= 5600 && HasFire && CanF4Number(3) && !CanF4Number(4) && !CanF4Number(5))
                    {
                        return true;
                    }
                    //火双4
                    if (Player.CurrentMp >= 7200 && HasFire && CanF4Number(4) && !CanF4Number(5))
                    {
                        return true;
                    }
                    //火双5
                    if (Player.CurrentMp >= 8800 && HasFire && CanF4Number(5))
                    {
                        return true;
                    }
                    //

                }

                //标准循环改
                if (Player.CurrentMp >= 9600 && JobGauge.UmbralHearts == 3 && !JobGauge.IsParadoxActive)
                {
                    if (HasFire && IsLastSpell(true, Actions.Paradox))
                    {
                        return true;
                    }
                }
            }

            //星灵转冰
            if (JobGauge.InAstralFire && abilityRemain <= 2)
            {
                if (Actions.Manafont.ElapsedAfter(3, false) || !Actions.Manafont.IsCoolDown)
                {
                    if (IsLastSpell(true, Actions.Despair))
                    {
                        return true;
                    }
                    if (Player.CurrentMp < 1200 && IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder) && !CanUseDespair(out _))
                    {
                        return true;
                    }
                }
            }

            act = null;
            return false;
        }

        /// <summary>
        /// 醒梦
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseLucidDreaming(out IAction act)
        {
            if (!GeneralActions.LucidDreaming.ShouldUse(out act)) return false;

            if (JobGauge.InUmbralIce)
            {
                //GCD经过时间 - 冰阶段经过时间 = 星灵在这个gcd使用的时间点
                //知道跳蓝在这个gcd判定的时间点
                //两者比较得到在哪跳蓝,或计算得到具体在哪跳的蓝
                //if () return true;
                if (Actions.Transpose.IsCoolDown && !HasFire && MPYuPanDouble >= 7900) return true;
                //if (IsLastAbility(true, Actions.Transpose) && HasFire && TargetHasThunder && !TargetThunderWillEnd(7)) return true;
            }

            if (fireOpener && inOpener && Actions.Leylines.IsCoolDown) return true;
            act = null;
            return false;
        }

        /// <summary>
        /// 即刻
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseSwiftcast(out IAction act)
        {
            if (!GeneralActions.Swiftcast.ShouldUse(out act)) return false;

            if (JobGauge.InUmbralIce)
            {
                if (Player.HaveStatus(ObjectStatus.LucidDreaming) && !JobGauge.IsParadoxActive && JobGauge.UmbralIceStacks < 3 && MPYuPanDouble >= 7900) return true;
            }

            act = null;
            return false;
        }

        /// <summary>
        /// 三连
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseTriplecast(out IAction act)
        {
            if (!Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true)) return false;

            if (JobGauge.InAstralFire && !JobGauge.IsParadoxActive)
            {
                if (Player.CurrentMp == 0) return false;

                if (IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder)) return true;
            }

            act = null;
            return false;
        }

        /// <summary>
        /// 悖论
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseParadox(out IAction act)
        {
            if (!Actions.Paradox.ShouldUse(out act)) return false;

            //在冰
            if (JobGauge.InUmbralIce)
            {
                //星灵进冰
                if (Actions.Transpose.IsCoolDown && JobGauge.UmbralIceStacks >= 1) return true;

                //冰3进冰,冰4后
                if (JobGauge.UmbralIceStacks == 3 && JobGauge.UmbralHearts == 3) return true;
            }

            //在火
            if (JobGauge.InAstralFire)
            {
                //if (JobGauge.UmbralHearts == 0) return true;
                if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
            }

            act = null;
            return false;
        }

        /// <summary>
        /// 火3
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseFire3(out IAction act)
        {
            if (!Actions.Fire3.ShouldUse(out act)) return false;
            if (IsLastSpell(true, Actions.Fire3)) return false;

            //冰阶段进火
            if (JobGauge.InUmbralIce)
            {
                //标准循环
                if (!HasFire && !HaveSwift && JobGauge.UmbralHearts == 3 && !JobGauge.IsParadoxActive) return true;

                if (JobGauge.UmbralHearts == 3 || HasFire || JobGauge.IsParadoxActive) return false;

                //瞬单条件
                if (HaveSwift && !Player.HaveStatus(ObjectStatus.LucidDreaming))
                {
                    //瞬单3
                    if (Player.CurrentMp >= 5600 && CanF4Number(3) && !CanF4Number(4)) return true;
                    //瞬单4
                    if (Player.CurrentMp >= 7200 && CanF4Number(4)) return true;
                }

                //长单3,4,5
                if (!HaveSwift) return true;
            }

            if (JobGauge.InAstralFire)
            {
                //进火后火3
                if (IsLastAction(true, Actions.Transpose) || JobGauge.AstralFireStacks < 3) return true;
            }

            if (fireOpener && !JobGauge.IsEnochianActive && !JobGauge.InUmbralIce) return true;

            return false;
        }

        /// <summary>
        /// 火4
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseFire4(out IAction act)
        {
            if (!Actions.Fire4.ShouldUse(out act)) return false;
            if (Player.CurrentMp < 2400) return false;

            //能瞬发时判断
            if (HaveSwift && JobGauge.ElementTimeRemaining > CalcSpellTime(2500)) return true;
            //悖论前
            if (JobGauge.IsParadoxActive && JobGauge.ElementTimeRemaining >= CalcSpellTime(2500) + CalcSpellTime(2800)) return true;
            //悖论后
            if (!JobGauge.IsParadoxActive && JobGauge.ElementTimeRemaining >= CalcSpellTime(3000) + CalcSpellTime(2800)) return true;

            return false;
        }

        /// <summary>
        /// 绝望
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseDespair(out IAction act)
        {
            if (!Actions.Despair.ShouldUse(out act)) return false;
            //有悖论不放
            if (JobGauge.IsParadoxActive) return false;
            //能放火4时不放
            if (CanUseFire4(out _)) return false;
            //能瞬发时
            //if (HaveSwift) return true;
            //正常判断,绝望收尾
            if (JobGauge.ElementTimeRemaining >= Actions.Despair.CastTime - 0.4) return true;

            return false;
        }

        /// <summary>
        /// 冰3
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseBlizzard3(out IAction act)
        {
            if (!Actions.Blizzard3.ShouldUse(out act)) return false;
            if (IsLastSpell(true, Actions.Blizzard3)) return false;

            if (iceOpener && !JobGauge.IsEnochianActive) return true;


            return false;
        }

        /// <summary>
        /// 雷3
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseThunder(out IAction act)
        {
            //Service.ChatGui.Print("1++++" + IsLastSpell(true, Actions.Despair));
            if (!Actions.Thunder.ShouldUse(out act)) return false;

            if (IsLastSpell(true, Actions.Thunder)) return false;

            if (TargetHasThunder && !TargetThunderWillEnd(10)) return false;
            //在冰
            if (JobGauge.InUmbralIce)
            {
                if (JobGauge.UmbralIceStacks == 3)
                {
                    if (TargetHasThunder && !TargetThunderWillEnd(5)) return false;
                    return true;
                }
                //有悖论时不释放
                if (JobGauge.IsParadoxActive) return false;
                //没雷dot
                if (!TargetHasThunder) return true;

                if (IsLastSpell(true, Actions.Xenoglossy) && MPYuPanDouble < 9400) return false;
                if (HasFire && !IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return true;
                if (!HasFire) return true;
            }

            if (TargetHasThunder && !TargetThunderWillEnd(4)) return false;

            //在火
            if (JobGauge.InAstralFire)
            {
                //Service.ChatGui.Print("2++++" + IsLastSpell(true, Actions.Despair));
                //上个技能是异言pass
                if (IsLastSpell(true, Actions.Xenoglossy)) return false;

                if (!inOpener && !HasFire && !HaveSwift && !JobGauge.IsParadoxActive)
                {
                    if (Player.CurrentMp >= 2400 && Player.CurrentMp <= 4800 && HasThunder) return true;
                }

                //魔泉时
                if (!Actions.Manafont.ElapsedAfter(3, false) && Actions.Manafont.IsCoolDown) return false;

                if (IsLastSpell(true, Actions.Despair) && HasThunder && Player.HaveStatus(ObjectStatus.Sharpcast))
                {
                    //Service.ChatGui.Print("1++++" + IsLastSpell(true, Actions.Despair));
                    return true;
                }

                if (JobGauge.ElementTimeRemaining < 6 || CanUseDespair(out _)) return false;

                if (!HasThunder && (!TargetHasThunder || TargetHasThunder && TargetThunderWillEnd(3))) return true;

                if (HasThunder && JobGauge.IsParadoxActive) return true;

                if (Player.HaveStatus(ObjectStatus.Sharpcast) && Player.WillStatusEnd(3, false)) return true;
            }

            return false;
        }

        private bool AAA(out IAction act)
        {
            if (IsLastSpell(true, Actions.Despair))
            {
                //Service.ChatGui.Print("+++" + Actions.Xenoglossy.MPNeed);
                if (Actions.Xenoglossy.ShouldUse(out act)) return true;
            }
            act = null;
            return false;
        }

        /// <summary>
        /// 异言
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private static bool CanUseXenoglossy(out IAction act)
        {
            //Service.ChatGui.Print("1" + IsLastSpell(true, Actions.Despair));
            //if (IsLastSpell(true, Actions.Despair))
            //{
            //    Service.ChatGui.Print("4" + IsLastSpell(true, Actions.Despair));
            //    if (Actions.Xenoglossy.ShouldUse(out act)) return true;
            //}
            if (!Actions.Xenoglossy.ShouldUse(out act)) return false;
            //Service.ChatGui.Print("3" + IsLastSpell(true, Actions.Despair));

            //在冰
            if (JobGauge.InUmbralIce)
            {
                //if (Player.CurrentMp >= 9600) return false;
                if (JobGauge.UmbralHearts == 3) return false;
                if (JobGauge.IsParadoxActive) return false;

                if (IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy) && MPYuPanDouble < 9400) return false;

                if (HasFire && !IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return true;
                if (!HasFire) return true;
            }

            //在火
            if (JobGauge.InAstralFire)
            {
                //Service.ChatGui.Print("1" + IsLastSpell(true, Actions.Despair));
                //if (Player.CurrentMp == 0)
                //{
                //    Service.ChatGui.Print("5" + IsLastSpell(true, Actions.Despair));
                ////    //火双n
                ////    if (HasFire) return true;
                ////    if (!HasFire) return true;
                ////    //
                ////    if (!HasFire && (TargetHasThunder && TargetThunderWillEnd((float)(CalcSpellTime(2500, isSpell: false) * 2 / 1000)) || !TargetHasThunder || HasThunder || JobGauge.PolyglotStacks == 2 || (JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer < CalcSpellTime(2500, isSpell: false) * 2))) return true;

                ////    //瞬单3,4
                ////    if (!HasFire && !HasThunder && TargetHasThunder && !TargetThunderWillEnd((float)(CalcSpellTime(2500, isSpell: false) * 2 / 1000)) && (!GeneralActions.Swiftcast.IsCoolDown || Player.FindStatusStack(ObjectStatus.Triplecast) >= 1) && JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer > CalcSpellTime(2500, isSpell: false) * 2) return true;
                //}
                //Service.ChatGui.Print("2" + IsLastSpell(true, Actions.Despair));
                if (IsLastSpell(true, Actions.Xenoglossy) || HaveSwift) return false;
                //起手
                if (iceOpener && inOpener && !JobGauge.IsParadoxActive && Player.CurrentMp <= 1200) return true;
                //魔泉时
                if (!Actions.Manafont.ElapsedAfter(3, false) && Actions.Manafont.IsCoolDown) return false;
                //Service.ChatGui.Print("2" + IsLastSpell(true, Actions.Despair));

                if (IsLastSpell(true, Actions.Despair))
                {
                    //Service.ChatGui.Print("3" + IsLastSpell(true, Actions.Despair));
                    //火双n
                    if (HasFire) return true;

                    if (!HasFire && (TargetHasThunder && TargetThunderWillEnd((float)(CalcSpellTime(2500, isSpell: false) * 2 / 1000)) || !TargetHasThunder || HasThunder || JobGauge.PolyglotStacks == 2 || JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer < CalcSpellTime(2500, isSpell: false) * 2)) return true;

                    //瞬单3,4
                    if (!HasFire && !HasThunder && TargetHasThunder && !TargetThunderWillEnd((float)(CalcSpellTime(2500, isSpell: false) * 2 / 1000)) && (!GeneralActions.Swiftcast.IsCoolDown || Player.FindStatusStack(ObjectStatus.Triplecast) >= 1) && JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer > CalcSpellTime(2500, isSpell: false) * 2) return true;
                }

                if (JobGauge.ElementTimeRemaining < CooldownHelper.CalcSpellTime(2500) + CooldownHelper.CalcSpellTime(3000)) return false;
                if (JobGauge.EnochianTimer <= 5000 && IsPolyglotStacksMaxed) return true;
                //未来观测卡跳蓝
                //if (!inOpener && !HaveSwift && !HasFire && !JobGauge.IsParadoxActive && CanJudgeXe() && Actions.Triplecast.ChargesCount >= 1)
                //{
                //    //双星灵时悖论后到星灵前时间
                //    var tatolTime = GCDTime * 2 - 0.8;
                //    if (tatolTime > 6 - (ActionUpdater.MPUpdateElapsed + GCDTime * 3) % 3) return true;
                //    if (tatolTime > 6 - (ActionUpdater.MPUpdateElapsed + GCDTime * 2) % 3) return true;
                //    if (tatolTime > 6 - (ActionUpdater.MPUpdateElapsed + GCDTime * 1) % 3) return true;
                //}
            }

            act = null;
            return false;
        }
    }
}
