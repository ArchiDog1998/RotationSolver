using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial.DNCCombos;
internal abstract class DNCCombo<TCmd> : JobGaugeCombo<DNCGauge, TCmd> where TCmd : Enum
{

    public sealed override uint[] JobIDs => new uint[] { 38 };

    public static readonly BaseAction

        //∆Ÿ–∫
        Cascade = new(15989)
        {
            BuffsProvide = new[] { StatusIDs.SilkenSymmetry }
        },

        //≈Á»™
        Fountain = new(15990)
        {
            BuffsProvide = new[] { StatusIDs.SilkenFlow }
        },

        //ƒÊ∆Ÿ–∫
        ReverseCascade = new(15991)
        {
            BuffsNeed = new[] { StatusIDs.SilkenSymmetry, StatusIDs.SilkenSymmetry2 },
        },

        //◊π≈Á»™
        Fountainfall = new(15992)
        {
            BuffsNeed = new[] { StatusIDs.SilkenFlow, StatusIDs.SilkenFlow2 }
        },

        //…»ŒË°§–Ú
        FanDance = new(16007)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { StatusIDs.ThreefoldFanDance },
        },

        //∑Á≥µ
        Windmill = new(15993)
        {
            BuffsProvide = Cascade.BuffsProvide,
        },

        //¬‰»–”Í
        Bladeshower = new(15994)
        {
            BuffsProvide = Fountain.BuffsProvide,
        },

        //…˝∑Á≥µ
        RisingWindmill = new(15995)
        {
            BuffsNeed = ReverseCascade.BuffsNeed,
        },

        //¬‰—™”Í
        Bloodshower = new(15996)
        {
            BuffsNeed = Fountainfall.BuffsNeed,
        },

        //…»ŒË°§∆∆
        FanDance2 = new(16008)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { StatusIDs.ThreefoldFanDance },
        },

        //…»ŒË°§º±
        FanDance3 = new(16009)
        {
            BuffsNeed = FanDance2.BuffsProvide,
        },

        //…»ŒË°§÷’
        FanDance4 = new(25791)
        {
            BuffsNeed = new[] { StatusIDs.FourfoldFanDance },
        },

        //Ω£ŒË
        SaberDance = new(16005)
        {
            OtherCheck = b => JobGauge.Esprit >= 50,
        },

        //¡˜–«ŒË
        StarfallDance = new(25792)
        {
            BuffsNeed = new[] { StatusIDs.FlourishingStarfall },
        },

        //«∞≥Â≤Ω
        EnAvant = new(16010, shouldEndSpecial: true),

        //«æﬁ±«˙Ω≈≤Ω
        Emboite = new(15999)
        {
            OtherCheck = b => JobGauge.NextStep == 15999,
        },

        //–°ƒÒΩªµ˛Ã¯
        Entrechat = new(16000)
        {
            OtherCheck = b => JobGauge.NextStep == 16000,
        },

        //¬Ã“∂–°ÃﬂÕ»
        Jete = new(16001)
        {
            OtherCheck = b => JobGauge.NextStep == 16001,
        },

        //Ωπ⁄÷∫º‚◊™
        Pirouette = new(16002)
        {
            OtherCheck = b => JobGauge.NextStep == 16002,
        },

        //±Í◊ºŒË≤Ω
        StandardStep = new(15997)
        {
            BuffsProvide = new[]
            {
                    StatusIDs.StandardStep,
                    StatusIDs.TechnicalStep,
            },
        },

        //ºº«…ŒË≤Ω
        TechnicalStep = new(15998)
        {
            BuffsNeed = new[]
            {
                    StatusIDs.StandardFinish,
            },
            BuffsProvide = StandardStep.BuffsProvide,
        },

        //∑¿ ÿ÷Æ…£∞Õ
        ShieldSamba = new(16012, true)
        {
            BuffsProvide = new[]
            {
                    StatusIDs.Troubadour,
                    StatusIDs.Tactician1,
                    StatusIDs.Tactician2,
                    StatusIDs.ShieldSamba,
            },
        },

        //÷Œ¡∆÷Æª™∂˚◊»
        CuringWaltz = new(16015, true),

        //±’ ΩŒË◊À
        ClosedPosition = new(16006, true)
        {
            ChoiceTarget = Targets =>
            {
                Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
                //Remove Weak
                b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { StatusIDs.Weakness, StatusIDs.BrinkofDeath }).Count() == 0 &&
                //Remove other partner.
                b.StatusList.Where(s => s.StatusId == StatusIDs.ClosedPosition2 && s.SourceID != Player.ObjectId).Count() == 0).ToArray();

                var targets = TargetFilter.GetJobCategory(Targets, Role.Ω¸’Ω);
                if (targets.Length > 0) return targets[0];

                targets = TargetFilter.GetJobCategory(Targets, Role.‘∂≥Ã);
                if (targets.Length > 0) return targets[0];

                targets = Targets;
                if (targets.Length > 0) return targets[0];

                return null;
            },
        },

        //Ω¯π•÷ÆÃΩ∏Í
        Devilment = new(16011, true),

        //∞Ÿª®’˘—ﬁ
        Flourish = new(16013)
        {
            BuffsNeed = new[] { StatusIDs.StandardFinish },
            BuffsProvide = new[]
            {
                    StatusIDs.ThreefoldFanDance,
                    StatusIDs.FourfoldFanDance,
            },
            OtherCheck = b => InCombat,
        },

        //º¥–À±Ì—›
        Improvisation = new(16014, true),

        //Ã·¿≠ƒ…
        Tillana = new(25790)
        {
            BuffsNeed = new[] { StatusIDs.FlourishingFinish },
        };

}
