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
using static XIVAutoAttack.Combos.RangedMagicial.BLMCombo.BLMCombo;

namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo
{
    internal sealed partial class BLMCombo : JobGaugeCombo<BLMGauge, CommandType>
    {
        internal enum CommandType : byte
        {
            None,
        }

        protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
        {
            //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
        };

        public override uint JobID => 25;
        protected override bool CanHealSingleAbility => false;

        private static bool iceOpener = false;
        private static bool fireOpener = true;

        enum LoopName
        {           
            Standard,           //标准循环          
            StandardEx,         //标准循环改
            NewShortFour,       //新短4
            NewShortFive,       //新短5
            LongSingleF3Three,  //长单3
            LongSingleF3Four,   //长单4
            LongSingleF3Five,   //长单5
            LongSingleF2Four,   //长单4(F2)
            SwiftSingleThree,   //瞬单3
            SwiftSingleFour,    //瞬单4
            LongDoubleTwo,      //长双2
            LongDoubleThree,    //长双3
            SwiftDoubleTwo,     //瞬双2
            SwiftDoubleThree,   //瞬双3
            FireDoubleFour,     //火双4
        }
        /// <summary>
        /// 标准循环
        /// </summary>
        private bool StandardLoop => Config.GetComboByName("UseLoop") == 0;
        /// <summary>
        /// 双星灵循环
        /// </summary>
        private bool DoubleTranspose => Config.GetComboByName("UseLoop") == 1;
        /// <summary>
        /// 进阶压冰循环
        /// </summary>
        private bool FewBlizzard => Config.GetComboByName("UseLoop") == 2;

        public override SortedList<DescType, string> Description => new()
        {
            { DescType.单体治疗, $"{Actions.BetweenTheLines}, {Actions.Leylines}, 这个很特殊！" },
            { DescType.单体防御, $"{Actions.Manaward}" },
            { DescType.移动技能, $"{Actions.AetherialManipulation}，目标为面向夹角小于30°内最远目标。" },
        };

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                        .SetCombo("UseLoop", 1, new string[] { "标准循环", "星灵循环", "压冰循环" }, "循环管理")
                        .SetBool("AutoLeylines", true, "自动上黑魔纹");
        }

        private bool CommandManager(out IAction act)
        {
            act = null;
            return false;
        }

        private protected override void UpdateInfo()
        {
            //跳蓝判定点计算
           
            MPYuCe(2);

            base.UpdateInfo();
        }

        private protected override IAction CountDownAction(float remainTime)
        {
            //战斗前激情
            if (remainTime <= 21 && Actions.Sharpcast.ShouldUse(out var act)) return act;
            return base.CountDownAction(remainTime);
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
            //以太步
            if (Actions.AetherialManipulation.ShouldUse(out act, mustUse: true)) return true;

            return base.MoveAbility(abilityRemain, out act);
        }

        private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {

            act = null;
            return false;
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            //起手
            if (OpenerManager(out act)) return true;

            //AOE
            if (LoopManagerArea(out act)) return true;

            //低级适配
            if (Level < 90 && LoopManagerSingleNOMax(out act)) return true;
            //满级循环
            if (Level == 90 && UseLoopManager(out act)) return true;

            if (IsMoving && InCombat && HaveHostileInRange)
            {
                if (Actions.Xenoglossy.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                if (Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
            if (!HaveHostileInRange && Maintence(out act)) return true;
            //act = null;
            return false;
        }

        private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
        {
            if (!HaveHostileInRange && Maintence(out act)) return true;
            return base.GeneralAbility(abilityRemain, out act);
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {          
            //三连咏唱
            if (CanUseTriplecast(out act)) return true;

            //魔泉
            if (Actions.Manafont.ShouldUse(out act) && IsLastSpell(true, Actions.Despair, Actions.Xenoglossy) && Player.CurrentMp == 0 && JobGauge.InAstralFire) return true;
            
            //星灵移位
            if (CanUseTranspose(abilityRemain, out act)) return true;

            //醒梦
            if (CanUseLucidDreaming(out act)) return true;

            //即刻
            if (CanUseSwiftcast(out act)) return true;

            //激情咏唱
            if (CanUseSharpcast(out act)) return true;

            //黑魔纹
            if (Config.GetBoolByName("AutoLeylines") && Actions.Leylines.ShouldUse(out act))
            {
                if (Player.HaveStatus(ObjectStatus.Triplecast) && Player.FindStatusStack(ObjectStatus.Triplecast) <= 1) return true;

                if (!Player.HaveStatus(ObjectStatus.Triplecast) && JobGauge.InUmbralIce && IsLastSpell(true, Actions.Thunder, Actions.Xenoglossy)) return true;

                if (!Player.HaveStatus(ObjectStatus.Triplecast) && JobGauge.InAstralFire) return true;
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
                //激情咏唱
                if (Actions.Sharpcast.ShouldUse(out act) && HaveHostileInRange) return true;
            }
            if (iceOpener)
            {
                //三连
                if (Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true) && Actions.Triplecast.ChargesCount == 2)
                {
                    if (IsLastAction(true, Actions.Fire3) || IsLastSpell(true, Actions.Xenoglossy)) return true;
                }
            }

            if (fireOpener)
            {
                //三连
                if (IsLastAction(true, Actions.Fire4) && Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true) && Actions.Triplecast.ChargesCount == 2) return true;
            }

            act = null;
            return false;
        }

        /// <summary>
        /// 循环管理
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool UseLoopManager(out IAction act)
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
                if (CanUseBlizzard3(out act)) return true;
            }
            else
            {
                //火3
                if (CanUseFire3(out act)) return true;
                //冰3
                if (Actions.Blizzard3.ShouldUse(out act)) return true;
            }
            act = null;
            return false;
        } 
        
        private bool LoopManagerSingleNOMax(out IAction act)
        {
            if (JobGauge.InUmbralIce)
            {
                if (Actions.Transpose.ShouldUse(out act))
                {
                    if (!Actions.Fire4.EnoughLevel && Player.CurrentMp == 10000) return true;
                }
                //雷
                if (CanUseThunder(out act)) return true;
                //冰阶段
                if (!Actions.Fire3.EnoughLevel && Actions.Blizzard.ShouldUse(out act)) return true;
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
                //火3
                if (CanUseFire3(out act)) return true;
                //雷
                if (CanUseThunder(out act)) return true;
                //火1
                if (!Actions.Paradox.EnoughLevel && Actions.Fire.ShouldUse(out act))
                {
                    if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
                    if (Level < 60) return true;
                }
                //异言
                if (CanUseXenoglossy(out act)) return true;
                //火4
                if (CanUseFire4(out act)) return true;
                //绝望
                if (CanUseDespair(out act)) return true;
                //冰3转冰
                if (CanUseBlizzard3(out act)) return true;
                if (Actions.Transpose.ShouldUse(out act))
                {
                    if (!Actions.Fire3.EnoughLevel && Player.CurrentMp < 1600) return true;
                }
            }
            else
            {
                if (!Actions.Paradox.EnoughLevel && Actions.Fire.ShouldUse(out act))
                {
                    if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
                    if (Level < 60) return true;
                }
                //冰3
                if (Actions.Blizzard3.ShouldUse(out act)) return true;
                if (Actions.Blizzard.ShouldUse(out act)) return true;
                //火3
                if (CanUseFire3(out act)) return true;
            }
            act = null;
            return false;
        }

        private bool LoopManagerArea(out IAction act)
        {
            act = null;
            if (!Actions.Blizzard2.ShouldUse(out _)) return false;

            if (Actions.Foul.ShouldUse(out act) && IsPolyglotStacksMaxed) return true;


            if (Actions.Freeze.ShouldUse(out act) && !IsLastSpell(true, Actions.Freeze))
            {
                if (JobGauge.UmbralIceStacks == 3 && JobGauge.UmbralHearts != 3) return true;
            }
            if (Actions.Thunder2.ShouldUse(out act) && !IsLastSpell(true, Actions.Thunder2))
            {
                if (HasThunder || !TargetHasThunder) return true;
            }

            if (Actions.Fire2.ShouldUse(out act))
            {
                if (Level < 20) return false;
                if (JobGauge.InUmbralIce && !Actions.Freeze.EnoughLevel && Player.CurrentMp == 10000) return true;
                if (JobGauge.InUmbralIce && JobGauge.UmbralHearts == 3) return true;
                if (JobGauge.InAstralFire && !Player.HaveStatus(ObjectStatus.EnhancedFlare)) return true;
                if (JobGauge.InAstralFire && JobGauge.UmbralHearts > 1) return true;
            }
            if (Actions.Flare.ShouldUse(out act))
            {
                if (JobGauge.InAstralFire && Player.HaveStatus(ObjectStatus.EnhancedFlare)) return true;
                return true;
            }
            if (Actions.Blizzard2.ShouldUse(out act) && !IsLastSpell(true, Actions.Blizzard2) && JobGauge.UmbralIceStacks != 3)
            {
                //if (!JobGauge.IsEnochianActive) return true;
                //if (JobGauge.InAstralFire) return true;
                return true;
            }
            act = null;
            return false;
        }
        private bool Maintence(out IAction act)
        {
            if (Actions.UmbralSoul.ShouldUse(out act)) return true;
            if (Actions.Transpose.ShouldUse(out act)) return true;

            return false;
        }
    }
}
