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

namespace XIVAutoAttack.Combos.RangedMagicial.BLMCombo
{
    internal sealed partial class BLMCombo : JobGaugeCombo<BLMGauge>
    {      
        internal override uint JobID => 25;
        protected override bool CanHealSingleAbility => false;

        private static bool inOpener = false;
        private static bool iceOpener = false;
        private static bool fireOpener = true;
        private static string MyCommand = "";
        private bool DoubleTranspose => Config.GetBoolByName("DoubleTranspose");
        //private bool DoubleTranspose = false;

        internal override SortedList<DescType, string> Description => new()
        {
            { DescType.单体治疗, $"{Actions.BetweenTheLines}, {Actions.Leylines}, 这个很特殊！" },
            { DescType.单体防御, $"{Actions.Manaward}" },
            { DescType.移动技能, $"{Actions.AetherialManipulation}，目标为面向夹角小于30°内最远目标。" },
        };

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                .SetBool("DoubleTranspose", true, "双星灵循环")
                .SetBool("AutoLeylines", true, "自动上黑魔纹");
        }

        internal override string OnCommand(string args)
        {
            MyCommand = args;

            return Mpyupan + "+" + MPYuPanDouble;
        }

        private bool CommandManager(out IAction act)
        {
            act = null;
            return false;
        }

        private protected override void UpdateInfo()
        {
            //跳蓝判定点计算
            if (JobGauge.InAstralFire && Actions.Transpose.IsCoolDown && !Actions.Transpose.ElapsedAfter(0.1f, false))
            {
                MPNextUpInCurrGCD = (3 - (ActionUpdater.MPUpdateElapsed - ActionUpdater.WeaponElapsed)) % 3 - 0.09;
                Mpyupan = ActionUpdater.WeaponElapsed;// - (15000 - JobGauge.ElementTimeRemaining);
            }
            MPYuCe(2);

            base.UpdateInfo();
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
            //起手
            if (OpenerManager(out act)) return true;

            //循环
            if (LoopManagerArea(out act)) return true;
            if (LoopManager(out act)) return true;

            if (IsMoving && InCombat && HaveHostileInRange)
            {
                if (Actions.Xenoglossy.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                if (Actions.Triplecast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
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
            if (CanUseTranspose(abilityRemain, out act)) return true;
            //醒梦
            if (CanUseLucidDreaming(out act)) return true;
            //即刻
            if (CanUseSwiftcast(out act)) return true;

            //激情咏唱
            if (Actions.Sharpcast.ShouldUse(out act, emptyOrSkipCombo: true))
            {
                //if (Player.HaveStatus(ObjectStatus.Triplecast) && Player.FindStatusStack(ObjectStatus.Triplecast) <= 1) return true;

                if (!inOpener) return true;

                if (!inOpener && JobGauge.InUmbralIce && !JobGauge.IsParadoxActive) return true;


            }
            //Service.ChatGui.Print("1++++" + Player.FindStatusStack(ObjectStatus.Triplecast));


            //黑魔纹
            if (Config.GetBoolByName("AutoLeylines") && Actions.Leylines.ShouldUse(out act))
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
                //火1
                if (Actions.Paradox.EnoughLevel && Actions.Fire.ShouldUse(out act))
                {
                    if (JobGauge.InAstralFire)
                    {
                        if (JobGauge.ElementTimeRemaining <= CalcSpellTime(2500) * 2) return true;
                        if (Level < 60) return true;
                    }
                }
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

        private bool LoopManagerArea(out IAction act)
        {
            if (Actions.Blizzard2.ShouldUse(out act) && !IsLastSpell(true, Actions.Blizzard2))
            {
                if (!JobGauge.IsEnochianActive || JobGauge.InAstralFire && Player.CurrentMp < 800) return true;
            }
            if (Actions.Freeze.ShouldUse(out act) && !IsLastSpell(true, Actions.Freeze))
            {
                if (JobGauge.UmbralIceStacks == 3) return true;
            }
            if (Actions.Thunder2.ShouldUse(out act) && !IsLastSpell(true, Actions.Thunder2))
            {
                if (HasThunder || !TargetHasThunder) return true;
            }

            if (Actions.Fire2.ShouldUse(out act))
            {
                if (JobGauge.InUmbralIce && JobGauge.UmbralHearts == 3) return true;
                if (JobGauge.InAstralFire && !Player.HaveStatus(ObjectStatus.EnhancedFlare)) return true;
                if (JobGauge.InAstralFire && JobGauge.UmbralHearts > 1) return true;
            }
            if (Actions.Flare.ShouldUse(out act))
            {
                if (JobGauge.InAstralFire && Player.HaveStatus(ObjectStatus.EnhancedFlare)) return true;
                return true;
            }
            act = null;
            return false;
        }
    }
}
