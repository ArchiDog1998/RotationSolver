using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee.DRGCombos;

internal abstract class DRGCombo<TCmd> : JobGaugeCombo<DRGGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 22, 4 };
    private static bool safeMove = false;

    public static readonly BaseAction
        //¾«×¼´Ì
        TrueThrust = new(75),

        //¹áÍ¨´Ì
        VorpalThrust = new(78) { OtherIDsCombo = new[] { 16479u } },

        //Ö±´Ì
        FullThrust = new(84),

        //²Ôñ·´Ì
        HeavensThrust = new(25771),

        //¿ªÌÅÇ¹
        Disembowel = new(87) { OtherIDsCombo = new[] { 16479u } },

        //Ó£»¨Å­·Å
        ChaosThrust = new(ActionIDs.ChaosThrust),

        //Ó£»¨Å­·Å
        ChaoticSpring = new(25772),

        //ÁúÑÀÁú×¦
        FangandClaw = new(ActionIDs.FangandClaw)
        {
            BuffsNeed = new ushort[] { StatusIDs.SharperFangandClaw },
        },

        //ÁúÎ²´ó»ØÐý
        WheelingThrust = new(ActionIDs.WheelingThrust)
        {
            BuffsNeed = new ushort[] { StatusIDs.EnhancedWheelingThrust },
        },

        //ÁúÑÛÀ×µç
        RaidenThrust = new(16479),

        //¹á´©¼â
        PiercingTalon = new(90),

        //ËÀÌìÇ¹
        DoomSpike = new(86),

        //ÒôËÙ´Ì
        SonicThrust = new(7397) { OtherIDsCombo = new[] { 25770u } },

        //É½¾³¿áÐÌ
        CoerthanTorment = new(16477),

        //ÆÆËé³å
        SpineshatterDive = new(95)
        {
            OtherCheck = b =>
            {
                if (safeMove && b.DistanceToPlayer() > 2) return false;
                if (IsLastAction(true, SpineshatterDive)) return false;

                return true;
            }
        },

        //ÁúÑ×³å
        DragonfireDive = new(96)
        {
            OtherCheck = b => !safeMove || b.DistanceToPlayer() < 2,
        },

        //ÌøÔ¾
        Jump = new(92)
        {
            BuffsProvide = new ushort[] { StatusIDs.DiveReady },
            OtherCheck = b => (!safeMove || b.DistanceToPlayer() < 2) && Player.HaveStatus(StatusIDs.PowerSurge),
        },
        //¸ßÌø
        HighJump = new(16478)
        {
            OtherCheck = Jump.OtherCheck,
        },
        //»ÃÏó³å
        MirageDive = new(7399)
        {
            BuffsNeed = new[] { StatusIDs.DiveReady },

            OtherCheck = b => !Geirskogul.WillHaveOneChargeGCD(4)
        },

        //ÎäÉñÇ¹
        Geirskogul = new(3555)
        {
            OtherCheck = b => Jump.IsCoolDown || HighJump.IsCoolDown,
        },

        //ËÀÕßÖ®°¶
        Nastrond = new(7400)
        {
            OtherCheck = b => JobGauge.IsLOTDActive,
        },

        //×¹ÐÇ³å
        Stardiver = new(16480)
        {
            OtherCheck = b => JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000,
        },

        //ÌìÁúµã¾¦
        WyrmwindThrust = new(25773)
        {
            OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && !IsLastAction(true, Stardiver),
        },

        //Áú½£
        LifeSurge = new(83)
        {
            BuffsProvide = new[] { StatusIDs.LifeSurge },

            OtherCheck = b => !IsLastAbility(true, LifeSurge),
        },

        //ÃÍÇ¹
        LanceCharge = new(85),

        //¾ÞÁúÊÓÏß
        DragonSight = new(7398)
        {
            ChoiceTarget = Targets =>
            {
                Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { StatusIDs.Weakness, StatusIDs.BrinkofDeath }).Count() == 0).ToArray();

                var targets = TargetFilter.GetJobCategory(Targets, Role.½üÕ½);
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                targets = TargetFilter.GetJobCategory(Targets, Role.Ô¶³Ì);
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                targets = Targets;
                if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                return Player;
            },

            BuffsNeed = new[] { StatusIDs.PowerSurge },

        },

        //Õ½¶·Á¬µ»
        BattleLitany = new(3557)
        {
            BuffsNeed = new[] { StatusIDs.PowerSurge },
        };

}
