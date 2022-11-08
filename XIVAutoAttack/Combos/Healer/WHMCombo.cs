using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.WHMCombo;

namespace XIVAutoAttack.Combos.Healer;

internal sealed class WHMCombo : JobGaugeCombo<WHMGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint JobID => 24;
    private protected override BaseAction Raise => Actions.Raise;
    internal struct Actions
    {
        public static readonly BaseAction
        #region 治疗
            //治疗
            Cure = new(120, true),

            //医治
            Medica = new(124, true),

            //复活
            Raise = new(125, true),

            //救疗
            Cure2 = new(135, true),

            //医济
            Medica2 = new(133, true)
            {
                BuffsProvide = new[] { ObjectStatus.Medica2, ObjectStatus.TrueMedica2 }
            },

            //再生
            Regen = new(137, true)
            {
                TargetStatus = new[]
                {
                    ObjectStatus.Regen1,
                    ObjectStatus.Regen2,
                    ObjectStatus.Regen3,
                }
            },

            //愈疗
            Cure3 = new(131, true),

            //天赐祝福
            Benediction = new(140, true)
            {
                OtherCheck = b => TargetUpdater.PartyMembersMinHP < 0.15f,
            },

            //庇护所
            Asylum = new(3569, true)
            {
                OtherCheck = b => !IsMoving
            },

            //安慰之心
            AfflatusSolace = new(16531, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },

            //神名
            Tetragrammaton = new(3570, true),

            //神祝祷
            DivineBenison = new(7432, true),

            //狂喜之心
            AfflatusRapture = new(16534, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },

            //水流幕
            Aquaveil = new(25861, true),

            //礼仪之铃
            LiturgyoftheBell = new(25862, true),
        #endregion
        #region 输出
            //飞石 
            Stone = new(119),//坚石127 垒石3568 崩石7431 闪耀16533 闪灼25859

            //疾风 Dot
            Aero = new(121, isDot: true)//烈风132 天辉16532
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Aero,
                    ObjectStatus.Aero2,
                    ObjectStatus.Dia,
                }
            },

            //神圣
            Holy = new(139),//豪圣 25860

            //法令
            Assize = new(3571, true),

            //苦难之心
            AfflatusMisery = new(16535)
            {
                OtherCheck = b => JobGauge.BloodLily == 3,
            },
        #endregion
        #region buff
            //神速咏唱
            PresenseOfMind = new(136, true),

            //无中生有
            ThinAir = new(7430, true),

            //全大赦
            PlenaryIndulgence = new(7433, true),

            //节制
            Temperance = new(16536, true);
        #endregion
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseLilyWhenFull", true, "蓝花集满时自动释放蓝花");
    }
    public override SortedList<DescType, string> Description => new()
    {
        {DescType.范围治疗, $"GCD: {Actions.AfflatusRapture}, {Actions.Medica2}, {Actions.Cure3}, {Actions.Medica}\n                     能力: {Actions.Asylum}, {Actions.Assize}"},
        {DescType.单体治疗, $"GCD: {Actions.AfflatusSolace}, {Actions.Regen}, {Actions.Cure2}, {Actions.Cure}\n                     能力: {Actions.Tetragrammaton}"},
        {DescType.范围防御, $"{Actions.Temperance}, {Actions.LiturgyoftheBell}"},
        {DescType.单体防御, $"{Actions.DivineBenison}, {Actions.Aquaveil}"},
    };
    private protected override bool GeneralGCD(out IAction act)
    {
        //苦难之心
        if (Actions.AfflatusMisery.ShouldUse(out act, mustUse: true)) return true;

        //泄蓝花 狂喜之心
        bool liliesNearlyFull = JobGauge.Lily == 2 && JobGauge.LilyTimer >= 17000;
        bool liliesFullNoBlood = JobGauge.Lily == 3 && JobGauge.BloodLily < 3;
        if(Config.GetBoolByName("UseLilyWhenFull") && (liliesNearlyFull || liliesFullNoBlood))
        {
            if (Actions.AfflatusRapture.ShouldUse(out act)) return true;
        }

        //群体输出
        if (Actions.Holy.ShouldUse(out act)) return true;

        //单体输出
        if (Actions.Aero.ShouldUse(out act, mustUse: IsMoving && HaveHostileInRange)) return true;
        if (Actions.Stone.ShouldUse(out act)) return true;

        
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //加个神速咏唱
        if (Actions.PresenseOfMind.ShouldUse(out act)) return true;

        //加个法令
        if (Actions.Assize.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //加个无中生有
        if (nextGCD is BaseAction action && action.MPNeed >= 1000 &&
            Actions.ThinAir.ShouldUse(out act)) return true;

        //加个全大赦
        if (nextGCD.IsAnySameAction(true, Actions.Medica, Actions.Medica2,
            Actions.Cure3, Actions.AfflatusRapture))
        {
            if (Actions.PlenaryIndulgence.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //安慰之心
        if (Actions.AfflatusSolace.ShouldUse(out act)) return true;

        //再生
        if (Actions.Regen.ShouldUse(out act)) return true;

        //救疗
        if (Actions.Cure2.ShouldUse(out act)) return true;

        //治疗
        if (Actions.Cure.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //神祝祷
        if (Actions.DivineBenison.ShouldUse(out act)) return true;

        //神名
        if (Actions.Tetragrammaton.ShouldUse(out act)) return true;

        //庇护所
        if (Actions.Asylum.ShouldUse(out act)) return true;

        //天赐
        if (Actions.Benediction.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        //狂喜之心
        if (Actions.AfflatusRapture.ShouldUse(out act)) return true;

        //医济
        if (Actions.Medica2.ShouldUse(out act)) return true;

        //医治
        if (Actions.Medica.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //庇护所
        if (Actions.Asylum.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神祝祷
        if (Actions.DivineBenison.ShouldUse(out act)) return true;

        //水流幕
        if (Actions.Aquaveil.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //节制
        if (Actions.Temperance.ShouldUse(out act)) return true;

        //礼仪之铃
        if (Actions.LiturgyoftheBell.ShouldUse(out act)) return true;
        return false;
    }
}