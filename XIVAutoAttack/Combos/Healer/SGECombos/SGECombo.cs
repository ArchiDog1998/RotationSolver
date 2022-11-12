using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer.SGECombos;

internal abstract class SGECombo<TCmd> : JobGaugeCombo<SGEGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 40 };
    private sealed protected override BaseAction Raise => Egeiro;

    public static readonly BaseAction
        //复苏
        Egeiro = new(24287),

        //注药
        Dosis = new(24283),

        //均衡注药
        EukrasianDosis = new(24283, isEot: true)
        {
            TargetStatus = new ushort[]
            {
                StatusIDs.EukrasianDosis,
                StatusIDs.EukrasianDosis2,
                StatusIDs.EukrasianDosis3
            },
        },

        //发炎
        Phlegma = new(24289),
        //发炎2
        Phlegma2 = new(24307),
        //发炎3
        Phlegma3 = new(24313),

        //诊断
        Diagnosis = new(24284, true),

        //心关
        Kardia = new(24285, true)
        {
            BuffsProvide = new ushort[] { StatusIDs.Kardia },
            ChoiceTarget = Targets =>
            {
                var targets = TargetFilter.GetJobCategory(Targets, Role.防护);
                targets = targets.Length == 0 ? Targets : targets;

                if (targets.Length == 0) return null;

                foreach (var tar in targets)
                {
                    if (tar.TargetObject?.TargetObject?.ObjectId == tar.ObjectId)
                    {
                        return tar;
                    }
                }

                return targets[0];
            },
            OtherCheck = b =>
            {
                foreach (var status in b.StatusList)
                {
                    if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId
                        && status.StatusId == StatusIDs.Kardion)
                    {
                        return false;
                    }
                }
                return true;
            },
        },

        //预后
        Prognosis = new(24286, true, shouldEndSpecial: true),

        //自生
        Physis = new(24288, true),

        //自生2
        Physis2 = new(24302, true),

        //均衡
        Eukrasia = new(24290)
        {
            OtherCheck = b => !JobGauge.Eukrasia,
        },

        //拯救
        Soteria = new(24294, true)
        {
            ChoiceTarget = Targets =>
            {
                foreach (var friend in Targets)
                {
                    if (friend.HaveStatus(StatusIDs.Kardion))
                    {
                        return friend;
                    }
                }
                return null;
            },
            OtherCheck = b => b.GetHealthRatio() < 0.7,
        },

        //神翼
        Icarus = new(24295, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //灵橡清汁
        Druochole = new(24296, true)
        {
            OtherCheck = b => JobGauge.Addersgall > 0 && HealHelper.SingleHeal(b, 600, 0.9, 0.85),
        },

        //失衡
        Dyskrasia = new(24297),

        //坚角清汁
        Kerachole = new(24298, true)
        {
            OtherCheck = b => JobGauge.Addersgall > 0,
        },

        //寄生清汁
        Ixochole = new(24299, true)
        {
            OtherCheck = b => JobGauge.Addersgall > 0,
        },

        //活化
        Zoe = new(24300),

        //白牛清汁
        Taurochole = new(24303, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
            OtherCheck = b => JobGauge.Addersgall > 0,
        },

        //箭毒
        Toxikon = new(24304),

        //输血
        Haima = new(24305, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //均衡诊断
        EukrasianDiagnosis = new(24291, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //均衡预后
        EukrasianPrognosis = new(24292, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //根素
        Rhizomata = new(24309),

        //整体论
        Holos = new(24310, true),

        //泛输血
        Panhaima = new(24311, true),

        //混合
        Krasis = new(24317, true),

        //魂灵风息
        Pneuma = new(24318),

        //消化
        Pepsis = new(24301, true)
        {
            OtherCheck = b =>
            {
                foreach (var chara in TargetUpdater.PartyMembers)
                {
                    if (chara.StatusList.Select(s => s.StatusId).Intersect(new uint[]
                    {
                            StatusIDs.EukrasianDiagnosis,
                            StatusIDs.EukrasianPrognosis,
                    }).Any()
                    && b.WillStatusEndGCD(2, 0, true, StatusIDs.EukrasianDiagnosis, StatusIDs.EukrasianPrognosis)
                    && chara.GetHealthRatio() < 0.9) return true;
                }

                return false;
            },
        };
}
