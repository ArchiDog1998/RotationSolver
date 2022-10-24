using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Combos.Healer;

internal class WHMCombo : JobGaugeCombo<WHMGauge>
{
    internal override uint JobID => 24;
    private protected override BaseAction Raise => Actions.Raise;
    internal struct Actions
    {
        public static readonly BaseAction
            //复活
            Raise = new (125, true),

            //飞石 平A
            Stone = new (119),

            //疾风 Dot
            Aero = new (121, isDot:true)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Aero,
                    ObjectStatus.Aero2,
                    ObjectStatus.Dia,
                }
            },
            //苦难之心
            AfflatusMisery = new (16535)
            {
                OtherCheck = b => JobGauge.BloodLily == 3,
            },
            //神圣
            Holy = new (139),

            //治疗
            Cure = new (120, true),
            //救疗
            Cure2 = new (135, true) { OtherIDsNot = new [] { 135u } },
            //神名
            Tetragrammaton = new (3570, true),
            //安慰之心 800
            AfflatusSolace = new (16531, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },
            //再生
            Regen = new (137, true)
            {
                TargetStatus = new []
                {
                    ObjectStatus.Regen1,
                    ObjectStatus.Regen2,
                    ObjectStatus.Regen3,
                }
            },
            //水流幕
            Aquaveil = new (25861, true),
            //神祝祷
            DivineBenison = new (7432, true),
            //天赐
            Benediction = new (140, true)
            {
                OtherCheck = b => TargetHelper.PartyMembersMinHP < 0.15f,
            },

            //医治 群奶最基础的。300
            Medica = new (124, true),
            //愈疗 600
            Cure3 = new (131, true),
            //医济 群奶加Hot。
            Medica2 = new (133, true) { BuffsProvide = new [] { ObjectStatus.Medica2, ObjectStatus.TrueMedica2 } },
            //庇护所
            Asylum = new (3569, true),

            //法令
            Assize = new (3571, true),

            //狂喜之心 400
            AfflatusRapture = new (16534, true)
            {
                OtherCheck = b => JobGauge.Lily > 0,
            },
            //礼仪之铃
            LiturgyoftheBell = new (25862, true),

            //神速咏唱
            PresenseOfMind = new (136, true),
            //无中生有
            ThinAir = new (7430, true),

            //全大赦
            PlenaryIndulgence = new (7433, true),
            //节制
            Temperance = new (16536, true);
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.范围治疗, $"GCD: {Actions.AfflatusRapture.Action.Name}, {Actions.Medica2.Action.Name}, {Actions.Cure3.Action.Name}, {Actions.Medica.Action.Name}\n                     能力: {Actions.Asylum.Action.Name}, {Actions.Assize.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.AfflatusSolace.Action.Name}, {Actions.Regen.Action.Name}, {Actions.Cure2.Action.Name}, {Actions.Cure.Action.Name}\n                     能力: {Actions.Tetragrammaton.Action.Name}"},
        {DescType.范围防御, $"{Actions.Temperance.Action.Name}, {Actions.LiturgyoftheBell.Action.Name}"},
        {DescType.单体防御, $"{Actions.DivineBenison.Action.Name}, {Actions.Aquaveil.Action.Name}"},
    };
    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //狂喜之心
        if (Actions.AfflatusRapture.ShouldUse(out act)) return true;
        //加Hot
        if (Actions.Medica2.ShouldUse(out act, lastComboActionID)) return true;

        float cure3 = GetBestHeal(Actions.Cure3.Action, 600);
        float medica = GetBestHeal(Actions.Medica.Action, 300);

        //愈疗
        if (cure3 > medica && Actions.Cure3.ShouldUse(out act)) return true;
        if (Actions.Medica.ShouldUse(out act)) return true;

        return false;
    }

    [Obsolete]
    /// <summary>
    /// 返回总共能大约回复的血量，非常大概。
    /// </summary>
    /// <param name="action"></param>
    /// <param name="strength"></param>
    /// <returns></returns>
    internal static float GetBestHeal(Action action, uint strength)
    {
        float healRange = strength * 0.000352f;

        //能够放到技能的队员。
        var canGet = TargetFilter.GetObjectInRadius(TargetHelper.PartyMembers, Math.Max(action.Range, 0.1f));

        float bestHeal = 0;
        foreach (var member in canGet)
        {
            float thisHeal = 0;
            Vector3 centerPt = member.Position;
            foreach (var ran in TargetHelper.PartyMembers)
            {
                //如果不在范围内，那算了。
                if (Vector3.Distance(centerPt, ran.Position) > action.EffectRange)
                {
                    continue;
                }

                thisHeal += Math.Min(1 - ran.CurrentHp / ran.MaxHp, healRange);
            }

            bestHeal = Math.Max(thisHeal, healRange);
        }
        return bestHeal;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //加个神祝祷
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

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
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
        if (nextGCD is BaseAction action && action.MPNeed > 500 && Actions.ThinAir.ShouldUse(out act)) return true;


        //天赐救人啊！
        if (Actions.Benediction.ShouldUse(out act)) return true;

        if (nextGCD.IsAnySameAction(true, Actions.Medica , Actions.Medica2,
            Actions.Cure3, Actions.AfflatusRapture))
        {
            //加个全大赦
            if (Actions.PlenaryIndulgence.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //庇护所
        if (!IsMoving && Actions.Asylum.ShouldUse(out act)) return true;

        //加个法令
        if (Actions.Assize.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //神名
        if (Actions.Tetragrammaton.ShouldUse(out act)) return true;

        //庇护所
        if (!IsMoving && Actions.Asylum.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //安慰之心
        if (Actions.AfflatusSolace.ShouldUse(out act)) return true;

        //再生
        if (Actions.Regen.ShouldUse(out act)) return true;

        //救疗
        if (Actions.Cure2.ShouldUse(out act, lastComboActionID)) return true;

        //治疗
        if (Actions.Cure.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //苦难之心
        if (Actions.AfflatusMisery.ShouldUse(out act, mustUse: true)) return true;

        //群体输出
        if (Actions.Holy.ShouldUse(out act)) return true;

        //单体输出
        if (Actions.Aero.ShouldUse(out act, mustUse: IsMoving && HaveHostileInRange)) return true;
        if (Actions.Stone.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}
