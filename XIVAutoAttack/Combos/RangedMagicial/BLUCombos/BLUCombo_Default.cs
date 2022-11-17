using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.BLUCombos.BLUCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.BLUCombos
{
    internal sealed class BLUCombo_Default : BLUCombo_Base<CommandType>
    {
        public override string Author => "秋水";

        internal enum CommandType : byte
        {
            None,
        }

        protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
        {
            //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
        };

        protected override bool CanHealAreaSpell => base.CanHealAreaSpell && BlueId == BLUID.Healer;
        protected override bool CanHealSingleSpell => base.CanHealSingleSpell && BlueId == BLUID.Healer;

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                .SetCombo("BlueId", 2, "职能", "防护", "治疗", "进攻")
                .SetCombo("AttackType", 2, "攻击方式", "魔法", "物理", "我全都要")
                .SetBool("MoonFluteBreak", false, "D青月笛爆发")
                .SetBool("UseFinalSting", false, "终极针收尾")
                .SetFloat("FinalStingHP", 0, "开始使用终极针的Hp");
        }

        private bool MoonFluteBreak => Config.GetBoolByName("MoonFluteBreak");
        private bool UseFinalSting => Config.GetBoolByName("UseFinalSting");
        private float FinalStingHP => Config.GetFloatByName("FinalStingHP");

        private protected override void UpdateInfo()
        {
            //BlueId = (BLUID)Config.GetComboByName("BlueId");
            //AttackType = (BLUAttackType)Config.GetComboByName("AttackType");
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            if(nextGCD.IsAnySameAction(false, Selfdestruct, FinalSting))
            {
                if (Swiftcast.ShouldUse(out act)) return true;
            }
            return base.EmergencyAbility(abilityRemain, nextGCD, out act);
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            act=null;
            //狂战士副作用期间
            if (Player.HasStatus(true, StatusID.WaningNocturne)) return false;
            //鬼宿脚
            if (IsLastAction(false, PhantomFlurry) || Player.HasStatus(true, StatusID.PhantomFlurry))
            {
                //if (Player.WillStatusEnd(1, true, StatusID.PhantomFlurry) && PhantomFlurry2.ShouldUse(out act, mustUse: true)) return true;
                return false;
            }
            //穿甲散弹
            if (Player.HasStatus(true, StatusID.SurpanakhaFury))
            {
                if (Surpanakha.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
            }

            //爆发
            if (MoonFluteBreak && DBlueBreak(out act)) return true;

            //填充
            if (SingleGCD(out act)) return true;

            

            act = null;
            return false;
        }

        private protected override bool HealAreaGCD(out IAction act)
        {
            if (BlueId == BLUID.Healer)
            {
                //有某些非常危险的状态。
                if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
                {
                    if (Exuviation.ShouldUse(out act, mustUse: true)) return true;
                }
                if (AngelsSnack.ShouldUse(out act)) return true;
                if (Stotram.ShouldUse(out act)) return true;
                if (PomCure.ShouldUse(out act)) return true;
            }
            else
            {
                if (WhiteWind.ShouldUse(out act)) return true; 
            }

            return base.HealAreaGCD(out act);
        }

        /// <summary>
        /// D青爆发
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool DBlueBreak(out IAction act)
        {
            act = null;
            if (BlueId == BLUID.Healer && BlueId == BLUID.Tank) return false;

            if (!AllOnSlot(MoonFlute)) return false;

            if (AllOnSlot(Whistle, Tingle, TripleTrident) && !Nightbloom.IsCoolDown && !TripleTrident.IsCoolDown)
            {
                //口笛
                if (Whistle.ShouldUse(out act)) return true;
                //哔哩哔哩
                if (!Player.HasStatus(true, StatusID.Tingling) && Player.HasStatus(true, StatusID.Harmonized) && Tingle.ShouldUse(out act, mustUse: true)) return true;
                //鱼叉
                if (Player.HasStatus(true, StatusID.WaxingNocturne) && TripleTrident.ShouldUse(out act, mustUse: true)) return true;
            }

            if (CanUseMoonFlute(out act)) return true;

            if (!Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            if (Nightbloom.ShouldUse(out act, mustUse: true)) return true;
            if (Eruption.ShouldUse(out act, mustUse: true)) return true;
            if (MatraMagic.ShouldUse(out act, mustUse: true)) return true;
            if (JKick.ShouldUse(out act, mustUse: true)) return true;
            if (Devour.ShouldUse(out act, mustUse: true)) return true;
            if (ShockStrike.ShouldUse(out act, mustUse: true)) return true;
            if (GlassDance.ShouldUse(out act, mustUse: true)) return true;
            if (MagicHammer.ShouldUse(out act, mustUse: true)) return true;
            if (Surpanakha.CurrentCharges >= 3 && Surpanakha.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
            if (PhantomFlurry.ShouldUse(out act, mustUse: true)) return true;

            if (SonicBoom.ShouldUse(out act)) return true;

            return false;
        }


        /// <summary>
        /// 月笛条件
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseMoonFlute(out IAction act)
        {
            if (!MoonFlute.ShouldUse(out act)) return false;

            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            if (Player.HasStatus(true, StatusID.Harmonized)) return true;

            return false;
        }

        /// <summary>
        /// 终极针组合
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseFinalSting(out IAction act)
        {
            act = null;
            if (!UseFinalSting) return false;
            if (!AllOnSlot(Whistle, MoonFlute, FinalSting)) return false;
            if (!FinalSting.ShouldUse(out _)) return false;

            if ((float)Target.CurrentHp / Target.MaxHp > FinalStingHP) return false;

            if (Whistle.ShouldUse(out act)) return true;
            if (MoonFlute.ShouldUse(out act)) return true;
            if (FinalSting.ShouldUse(out act)) return true;

            return false;
        }

        /// <summary>
        /// 单体GCD填充
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool SingleGCD(out IAction act)
        {
            act = null;
            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            //音爆
            if (SonicBoom.ShouldUse(out act)) return true;

            return false;
        }
    }
}
