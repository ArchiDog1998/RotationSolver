using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.Gui;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial.MCHCombos;

internal abstract class MCHCombo<TCmd> : JobGaugeCombo<MCHGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 31 };

    public static readonly BaseAction
        //分裂弹
        SplitShot = new(2866),

        //独头弹
        SlugShot = new(2868)
        {
            OtherIDsCombo = new[] { 7411u },
        },

        //狙击弹
        CleanShot = new(2873)
        {
            OtherIDsCombo = new[] { 7412u },
        },

        //热冲击
        HeatBlast = new(7410),

        //散射
        SpreadShot = new(2870),

        //自动弩
        AutoCrossbow = new(16497),

        //热弹
        HotShot = new(2872)
        {
            //OtherCheck = AirAnchor.OtherCheck,
        },

        //空气锚
        AirAnchor = new(16500)
        {
            //OtherCheck = b => !JobGauge.IsOverheated || JobGauge.IsOverheated && RemainAfterGCD(JobGauge.OverheatTimeRemaining, 0),
        },

        //钻头
        Drill = new(16498)
        {
            OtherCheck = AirAnchor.OtherCheck,
        },

        //回转飞锯
        ChainSaw = new(25788)
        {
            OtherCheck = AirAnchor.OtherCheck,
        },

        //毒菌冲击
        Bioblaster = new(16499, isEot: true)
        {
            OtherCheck = AirAnchor.OtherCheck,
        },

        //整备
        Reassemble = new(2876)
        {
            BuffsProvide = new ushort[] { StatusIDs.Reassemble },
            OtherCheck = b => HaveHostileInRange,
        },

        //超荷
        Hypercharge = new(17209)
        {
            OtherCheck = b => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
        },

        //野火
        Wildfire = new(2878)
        {
            OtherCheck = b => JobGauge.Heat >= 50,
        },

        //虹吸弹
        GaussRound = new(2874),

        //弹射
        Ricochet = new(2890),
        aaa = new(7557),

        //枪管加热
        BarrelStabilizer = new(7414)
        {
            OtherCheck = b => JobGauge.Heat <= 50 && !IsLastWeaponSkill(false, ChainSaw),
        },

        //车式浮空炮塔
        RookAutoturret = new(2864)
        {
            OtherCheck = b => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
        },

        //策动
        Tactician = new(16889, true)
        {
            BuffsProvide = new[]
            {
                    StatusIDs.Troubadour,
                    StatusIDs.Tactician1,
                    StatusIDs.Tactician2,
                    StatusIDs.ShieldSamba,
            },
        };
}
