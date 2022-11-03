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

namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo
{
    internal sealed partial class BLMCombo : JobGaugeCombo<BLMGauge>
    {
        internal struct Actions
        {
            internal class ThunderAction : BaseAction
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
                    if (Player.CurrentMp >= 8000 && MPYuPanDouble < 9400 && !HasFire && IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder) && IsOldSpell(1, Actions.Paradox))
                    {
                        // && (!GeneralActions.Swiftcast.IsCoolDown || HaveSwift)
                        return true;
                    }
                    //瞬双3,长双3(2/3-1)
                    if (Player.CurrentMp >= 9600 && !HasFire && IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder))
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
            if (DoubleTranspose && JobGauge.InAstralFire)
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

            if (JobGauge.InUmbralIce && JobGauge.UmbralIceStacks < 3)
            {
                if (!HasFire && MPYuPanDouble >= 7900) return true;
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

            if (DoubleTranspose && JobGauge.InUmbralIce)
            {
                if (!JobGauge.IsParadoxActive && JobGauge.UmbralIceStacks < 3 && MPYuPanDouble >= 7900) return true;
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

            if (!JobGauge.InAstralFire) return false;

            if (!JobGauge.IsParadoxActive)
            {
                if (Player.CurrentMp == 0) return false;

                if (IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder)) return true;
            }

            if (!DoubleTranspose)
            {
                if (IsLastSpell(true, Actions.Xenoglossy, Actions.Thunder) && Actions.Triplecast.ShouldUse(out act)) return true;
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

            //火起手
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
            //当前火状态还能打几个火4
            if (F4RemainingNumber() >= 1) return true;
            //悖论后
            //if (!JobGauge.IsParadoxActive && JobGauge.ElementTimeRemaining >= CalcSpellTime(3000) + CalcSpellTime(2800)) return true;

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

            //标准循环
            if (!DoubleTranspose && (Player.CurrentMp == 0 || !CanUseFire4(out _) && !CanUseDespair(out _))) return true;

            //双星灵
            if (JobGauge.InAstralFire && !CanUseFire4(out _) && !CanUseDespair(out _) && (JobGauge.ElementTimeRemaining < CooldownHelper.CalcSpellTime(3000) - 0.5 || Player.CurrentMp <= 1200) && !JobGauge.IsParadoxActive && (Actions.Manafont.ElapsedAfter(3, false) || !Actions.Manafont.IsCoolDown)) return true;

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

            //在冰
            if (JobGauge.InUmbralIce)
            {
                if (JobGauge.UmbralIceStacks == 3)
                {
                    if (!TargetHasThunder || TargetThunderWillEnd(3)) return true;
                    return false;
                }

                if (TargetHasThunder && !TargetThunderWillEnd(10)) return false;
                //有悖论时不释放
                if (JobGauge.IsParadoxActive) return false;
                //没雷dot
                if (!TargetHasThunder) return true;

                if (IsLastSpell(true, Actions.Xenoglossy)) return false;
                if (HasThunder) return true;
            }
           
            //在火
            if (JobGauge.InAstralFire)
            {
                if (!DoubleTranspose)
                {
                    if (!TargetHasThunder || TargetHasThunder && TargetThunderWillEnd(3)) return true;
                    return false;
                }

                if (TargetHasThunder && !TargetThunderWillEnd(6)) return false;
                //上个技能是异言pass
                if (IsLastSpell(true, Actions.Xenoglossy)) return false;
                //魔泉时
                if (!Actions.Manafont.ElapsedAfter(3, false) && Actions.Manafont.IsCoolDown) return false;

                //if (BenignMp() && HasThunder) return true;  

                if (HasThunder && JobGauge.IsParadoxActive) return true;

                //未来观测卡跳蓝(三连咏唱)
                if (BenignMp()) return true;

                if (IsLastSpell(true, Actions.Despair) && Player.HaveStatus(ObjectStatus.Sharpcast) && HasThunder)
                {
                    return true;
                }
                if (IsLastSpell(true, Actions.Despair) && HasThunder && JobGauge.PolyglotStacks == 1)
                {
                    return true;
                }

                if (!HasThunder && (!TargetHasThunder || TargetHasThunder && TargetThunderWillEnd(3))) return true;

                //if (Player.HaveStatus(ObjectStatus.Sharpcast) && Player.WillStatusEnd(3, false)) return true;
            }

            return false;
        }

        /// <summary>
        /// 异言
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseXenoglossy(out IAction act)
        {
            if (!Actions.Xenoglossy.ShouldUse(out act)) return false;

            if (!DoubleTranspose)
            {
                if (JobGauge.UmbralHearts != 3) return false;
                if (IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return false;
                if (JobGauge.EnochianTimer <= 5000 && IsPolyglotStacksMaxed) return true;
                if (!Actions.Manafont.IsCoolDown && IsLastSpell(true, Actions.Despair)) return true;

                return false;
            }

            //在冰
            if (JobGauge.InUmbralIce)
            {

                if (JobGauge.UmbralHearts == 3) return false;
                if (JobGauge.IsParadoxActive) return false;
                if (IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return false;
                if (IsOldSpell(1,Actions.Thunder3)) return false;

                if (HasFire && !IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return true;
                if (!HasFire && MPYuPanDouble >= 7900) return true;
            }

            //在火
            if (JobGauge.InAstralFire)
            {

                if (IsLastSpell(true, Actions.Xenoglossy) || HaveSwift) return false;
                //起手
                if (iceOpener && inOpener && !JobGauge.IsParadoxActive && Player.CurrentMp <= 1200) return true;
                //魔泉时
                if (!Actions.Manafont.ElapsedAfter(3, false) && Actions.Manafont.IsCoolDown) return false;

                //未来观测卡跳蓝(三连咏唱)
                if (BenignMp()) return true; 
                
                if (IsLastSpell(true, Actions.Despair))
                {
                    //火双n
                    if (HasFire) return true;

                    //看雷云满足条件吗
                    if (TargetHasThunder && !TargetThunderWillEnd((float)GCDTime * 4 / 1000) || !HasThunder)
                    {
                        if (JobGauge.PolyglotStacks == 2 || (JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer < GCDTime * 2)) return true;
                        return false;
                    }

                    //长瞬双
                    if (JobGauge.PolyglotStacks >= 1) return true;
                }

                //if (JobGauge.ElementTimeRemaining < CooldownHelper.CalcSpellTime(2500) + CooldownHelper.CalcSpellTime(3000)) return false;
                if (JobGauge.EnochianTimer <= 5000 && IsPolyglotStacksMaxed) return true;
            }

            act = null;
            return false;
        }
    }
}
