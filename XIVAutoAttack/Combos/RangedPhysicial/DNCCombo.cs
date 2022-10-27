using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal sealed class DNCCombo : JobGaugeCombo<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //∆Ÿ–∫
            Cascade = new (15989)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenSymmetry}
            },

            //≈Á»™
            Fountain = new (15990)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenFlow }
            },

            //ƒÊ∆Ÿ–∫
            ReverseCascade = new (15991)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
            },

            //◊π≈Á»™
            Fountainfall = new (15992)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
            },

            //…»ŒË°§–Ú
            FanDance = new (16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //∑Á≥µ
            Windmill = new (15993)
            {
                BuffsProvide = Cascade.BuffsProvide,
            },

            //¬‰»–”Í
            Bladeshower = new (15994)
            {
                BuffsProvide = Fountain.BuffsProvide,
            },

            //…˝∑Á≥µ
            RisingWindmill = new (15995)
            {
                BuffsNeed = ReverseCascade.BuffsNeed,
            },

            //¬‰—™”Í
            Bloodshower = new (15996)
            {
                BuffsNeed = Fountainfall.BuffsNeed,
            },

            //…»ŒË°§∆∆
            FanDance2 = new (16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //…»ŒË°§º±
            FanDance3 = new (16009)
            {
                BuffsNeed = FanDance2.BuffsProvide,
            },

            //…»ŒË°§÷’
            FanDance4 = new (25791)
            {
                BuffsNeed = new [] { ObjectStatus.FourfoldFanDance },
            },

            //Ω£ŒË
            SaberDance = new (16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //¡˜–«ŒË
            StarfallDance = new (25792)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingStarfall },
            },

            //«∞≥Â≤Ω
            EnAvant = new (16010, shouldEndSpecial: true),

            //«æﬁ±«˙Ω≈≤Ω
            Emboite = new (15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //–°ƒÒΩªµ˛Ã¯
            Entrechat = new (16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //¬Ã“∂–°ÃﬂÕ»
            Jete = new (16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //Ωπ⁄÷∫º‚◊™
            Pirouette = new (16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //±Í◊ºŒË≤Ω
            StandardStep = new (15997)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //ºº«…ŒË≤Ω
            TechnicalStep = new (15998)
            {
                BuffsNeed = new []
                {
                    ObjectStatus.StandardFinish,
                },
                BuffsProvide = StandardStep.BuffsProvide,
            },

            //∑¿ ÿ÷Æ…£∞Õ
            ShieldSamba = new (16012, true)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            },

            //÷Œ¡∆÷Æª™∂˚◊»
            CuringWaltz = new (16015, true),

            //±’ ΩŒË◊À
            ClosedPosition = new (16006, true)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != LocalPlayer.ObjectId && b.CurrentHp != 0 &&
                    //Remove Weak
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                    //Remove other partner.
                    b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != LocalPlayer.ObjectId).Count() == 0).ToArray();

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
            Devilment = new (16011, true),

            //∞Ÿª®’˘—ﬁ
            Flourish = new (16013)
            {
                BuffsNeed = new [] { ObjectStatus.StandardFinish },
                BuffsProvide = new []
                {
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
                },
                OtherCheck = b => InBattle,
            },

            //º¥–À±Ì—›
            Improvisation = new (16014, true),

            //Ã·¿≠ƒ…
            Tillana = new (25790)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingFinish },
            };
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.∑∂Œß∑¿”˘, $"{Actions.ShieldSamba.Action.Name}"},
        {DescType.∑∂Œß÷Œ¡∆, $"{Actions.CuringWaltz.Action.Name}, {Actions.Improvisation.Action.Name}"},
        {DescType.“∆∂Ø, $"{Actions.EnAvant.Action.Name}"},
    };

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && !Actions.TechnicalStep.EnoughLevel
    && Actions.Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //”¶º±ªªŒË∞È
        if (LocalPlayer.HaveStatus(ObjectStatus.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HaveStatus(ObjectStatus.ClosedPosition2))
                {
                    if (Actions.ClosedPosition.ShouldUse(out act) && Actions.ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (Actions.ClosedPosition.ShouldUse(out act)) return true;

        //≥¢ ‘±¨∑¢
        if (LocalPlayer.HaveStatus(ObjectStatus.TechnicalFinish)
        && Actions.Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //∞Ÿª®
        if (Actions.Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //…»ŒË°§º±
        if (Actions.FanDance4.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //…»ŒË
        if (LocalPlayer.HaveStatus(ObjectStatus.Devilment) || JobGauge.Feathers > 3 || !Actions.TechnicalStep.EnoughLevel)
        {
            if (Actions.FanDance2.ShouldUse(out act)) return true;
            if (Actions.FanDance.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!InBattle && !LocalPlayer.HaveStatus(ObjectStatus.ClosedPosition1) 
            && Actions.ClosedPosition.ShouldUse(out act)) return true;

        if (SettingBreak)
        {
            if (Actions.TechnicalStep.ShouldUse(out act)) return true;
        }

        if (StepGCD(out act)) return true;
        if (AttackGCD(out act, LocalPlayer.HaveStatus(ObjectStatus.Devilment), lastComboActionID)) return true;

        return false;
    }

    private bool StepGCD(out IAction act)
    {
        act = null;
        if (!LocalPlayer.HaveStatus(ObjectStatus.StandardStep, ObjectStatus.TechnicalStep)) return false;

        if (LocalPlayer.HaveStatus(ObjectStatus.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = Actions.StandardStep;
            return true;
        }
        else if (LocalPlayer.HaveStatus(ObjectStatus.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = Actions.TechnicalStep;
            return true;
        }
        else
        {
            if (Actions.Emboite.ShouldUse(out act)) return true;
            if (Actions.Entrechat.ShouldUse(out act)) return true;
            if (Actions.Jete.ShouldUse(out act)) return true;
            if (Actions.Pirouette.ShouldUse(out act)) return true;
        }

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking, uint lastComboActionID)
    {

        //Ω£ŒË
        if ((breaking || JobGauge.Esprit >= 80) &&
            Actions.SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //Ã·ƒ…¿≠
        if (Actions.Tillana.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = !Actions.TechnicalStep.IsCoolDown || Actions.TechnicalStep.WillHaveOneChargeGCD(2);

        if (!LocalPlayer.HaveStatus(ObjectStatus.TechnicalFinish))
        {
            //±Í◊ºŒË≤Ω
            if (canstandard && Actions.StandardStep.ShouldUse(out act)) return true;
        }

        //”√µÙBuff
        if (Actions.Bloodshower.ShouldUse(out act)) return true;
        if (Actions.Fountainfall.ShouldUse(out act)) return true;

        if (Actions.RisingWindmill.ShouldUse(out act)) return true;
        if (Actions.ReverseCascade.ShouldUse(out act)) return true;


        //±Í◊ºŒË≤Ω
        if (canstandard && Actions.StandardStep.ShouldUse(out act)) return true;


        //aoe
        if (Actions.Bladeshower.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Windmill.ShouldUse(out act)) return true;

        //single
        if (Actions.Fountain.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Cascade.ShouldUse(out act)) return true;

        return false;
    }
}
