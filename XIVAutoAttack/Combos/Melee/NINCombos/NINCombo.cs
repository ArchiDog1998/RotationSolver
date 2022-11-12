using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Melee.NINCombos;

internal abstract class NINCombo<TCmd> : JobGaugeCombo<NINGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 30, 29 };
    public class NinAction : BaseAction
    {
        internal BaseAction[] Ninjutsus { get; }
        internal NinAction(uint actionID, params BaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    public static readonly BaseAction

        //隐遁
        Hide = new(2245),

        //双刃旋
        SpinningEdge = new(2240),

        //残影
        ShadeShift = new(2241),

        //绝风
        GustSlash = new(2242),

        //飞刀
        ThrowingDagger = new(2247),

        //夺取
        Mug = new(2248)
        {
            OtherCheck = b => JobGauge.Ninki <= 50,
        },

        //攻其不备
        TrickAttack = new(ActionIDs.TrickAttack)
        {
            BuffsNeed = new ushort[] { StatusIDs.Suiton, StatusIDs.Hidden },
        },

        //旋风刃
        AeolianEdge = new(ActionIDs.AeolianEdge),

        //血雨飞花
        DeathBlossom = new(2254),

        //天之印
        Ten = new(2259),

        //地之印
        Chi = new(2261),

        //人之印
        Jin = new(2263),

        //天地人
        TenChiJin = new(7403)
        {
            BuffsProvide = new[] { StatusIDs.Kassatsu, StatusIDs.TenChiJin },
            OtherCheck = b => JobGauge.HutonTimer > 0,
        },

        //缩地
        Shukuchi = new(2262, true),

        //断绝
        Assassinate = new(2246),

        //命水
        Meisui = new(16489)
        {
            BuffsNeed = new[] { StatusIDs.Suiton },
            OtherCheck = b => JobGauge.Ninki <= 50,
        },

        //生杀予夺
        Kassatsu = new(2264, isFriendly: true)
        {
            BuffsProvide = new[] { StatusIDs.Kassatsu, StatusIDs.TenChiJin },
        },

        //八卦无刃杀
        HakkeMujinsatsu = new(16488),

        //强甲破点突
        ArmorCrush = new(ActionIDs.ArmorCrush)
        {
            OtherCheck = b => RemainAfter(JobGauge.HutonTimer / 1000f, 29) && JobGauge.HutonTimer > 0,
        },

        //通灵之术·大虾蟆
        HellfrogMedium = new(7401),

        //六道轮回
        Bhavacakra = new(7402),

        //分身之术
        Bunshin = new(16493),

        //残影镰鼬
        PhantomKamaitachi = new(25774)
        {
            BuffsNeed = new[] { StatusIDs.PhantomKamaitachiReady },
        },

        //月影雷兽牙
        FleetingRaiju = new(25778)
        {
            BuffsNeed = new[] { StatusIDs.RaijuReady },
        },

        //月影雷兽爪
        ForkedRaiju = new(25777)
        {
            BuffsNeed = new[] { StatusIDs.RaijuReady },
        },

        //风来刃
        Huraijin = new(25876)
        {
            OtherCheck = b => JobGauge.HutonTimer == 0,
        },

        //梦幻三段
        DreamWithinaDream = new(3566),

        //风魔手里剑天
        FumaShurikenTen = new(18873),

        //风魔手里剑人
        FumaShurikenJin = new(18875),

        //火遁之术天
        KatonTen = new(18876),

        //雷遁之术地
        RaitonChi = new(18877),

        //土遁之术地
        DotonChi = new(18880),

        //水遁之术人
        SuitonJin = new(18881);

    public static readonly NinAction

        //通灵之术
        RabbitMedium = new(2272),

        //风魔手里剑
        FumaShuriken = new(2265, Ten),

        //火遁之术
        Katon = new(2266, Chi, Ten),

        //雷遁之术
        Raiton = new(2267, Ten, Chi),

        //冰遁之术
        Hyoton = new(2268, Ten, Jin),

        //风遁之术
        Huton = new(2269, Jin, Chi, Ten)
        {
            OtherCheck = b => JobGauge.HutonTimer == 0,
        },

        //土遁之术
        Doton = new(2270, Jin, Ten, Chi)
        {
            BuffsProvide = new[] { StatusIDs.Doton },
        },

        //水遁之术
        Suiton = new(ActionIDs.Suiton, Ten, Chi, Jin)
        {
            BuffsProvide = new[] { StatusIDs.Suiton },
            OtherCheck = b => TrickAttack.WillHaveOneChargeGCD(1, 1),
        },

        //劫火灭却之术
        GokaMekkyaku = new(16491, Chi, Ten),

        //冰晶乱流之术
        HyoshoRanryu = new(16492, Ten, Jin);

}
