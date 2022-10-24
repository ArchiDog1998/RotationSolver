using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class DNCCombo : JobGaugeCombo<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //ÆÙÐº
            Cascade = new (15989)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenSymmetry}
            },

            //ÅçÈª
            Fountain = new (15990)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenFlow }
            },

            //ÄæÆÙÐº
            ReverseCascade = new (15991)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
            },

            //×¹ÅçÈª
            Fountainfall = new (15992)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
            },

            //ÉÈÎè¡¤Ðò
            FanDance = new (16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //·ç³µ
            Windmill = new (15993)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenSymmetry }
            },

            //ÂäÈÐÓê
            Bladeshower = new (15994)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenFlow }
            },

            //Éý·ç³µ
            RisingWindmill = new (15995)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
            },

            //ÂäÑªÓê
            Bloodshower = new (15996)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
            },

            //ÉÈÎè¡¤ÆÆ
            FanDance2 = new (16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //ÉÈÎè¡¤¼±
            FanDance3 = new (16009)
            {
                BuffsNeed = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //ÉÈÎè¡¤ÖÕ
            FanDance4 = new (25791)
            {
                BuffsNeed = new [] { ObjectStatus.FourfoldFanDance },
            },

            //½£Îè
            SaberDance = new (16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //Á÷ÐÇÎè
            StarfallDance = new (25792)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingStarfall },
            },

            //Ç°³å²½
            EnAvant = new (16010, shouldEndSpecial: true),

            //Ç¾Þ±Çú½Å²½
            Emboite = new (15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //Ð¡Äñ½»µþÌø
            Entrechat = new (16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //ÂÌÒ¶Ð¡ÌßÍÈ
            Jete = new (16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //½ð¹ÚÖº¼â×ª
            Pirouette = new (16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //±ê×¼Îè²½
            StandardStep = new (15997)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //¼¼ÇÉÎè²½
            TechnicalStep = new (15998)
            {
                BuffsNeed = new []
                {
                    ObjectStatus.StandardFinish,
                },
                BuffsProvide = new []
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //·ÀÊØÖ®É£°Í
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

            //ÖÎÁÆÖ®»ª¶û×È
            CuringWaltz = new (16015, true),

            //±ÕÊ½Îè×Ë
            ClosedPosition = new (16006, true)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != LocalPlayer.ObjectId && b.CurrentHp != 0 &&
                    //Remove Weak
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                    //Remove other partner.
                    b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != LocalPlayer.ObjectId).Count() == 0).ToArray();

                    var targets = TargetFilter.GetJobCategory(Targets, Role.½üÕ½);
                    if (targets.Length > 0) return targets[0];

                    targets = TargetFilter.GetJobCategory(Targets, Role.Ô¶³Ì);
                    if (targets.Length > 0) return targets[0];

                    targets = Targets;
                    if (targets.Length > 0) return targets[0];

                    return null;
                },
            },

            //½ø¹¥Ö®Ì½¸ê
            Devilment = new (16011, true),

            //°Ù»¨ÕùÑÞ
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

            //¼´ÐË±íÑÝ
            Improvisation = new (16014, true),

            //ÌáÀ­ÄÉ
            Tillana = new (25790)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingFinish },
            };
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.·¶Î§·ÀÓù, $"{Actions.ShieldSamba.Action.Name}"},
        {DescType.·¶Î§ÖÎÁÆ, $"{Actions.CuringWaltz.Action.Name}, {Actions.Improvisation.Action.Name}"},
        {DescType.ÒÆ¶¯, $"{Actions.EnAvant.Action.Name}"},
    };

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //Ó¦¼±»»Îè°é
        if (LocalPlayer.HaveStatus(ObjectStatus.ClosedPosition1))
        {
            foreach (var friend in TargetHelper.PartyMembers)
            {
                if (StatusHelper.FindStatusTimes(friend, ObjectStatus.ClosedPosition2)?.Length > 0)
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

        //³¢ÊÔ±¬·¢
        if (LocalPlayer.HaveStatus(ObjectStatus.TechnicalFinish)
        && Actions.Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //°Ù»¨
        if (Actions.Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //ÉÈÎè¡¤¼±
        if (Actions.FanDance4.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //ÉÈÎè
        if (LocalPlayer.HaveStatus(ObjectStatus.Devilment) || JobGauge.Feathers > 3 || Level < 70)
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
    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Level < Actions.TechnicalStep.Level
            && Actions.Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return base.BreakAbility(abilityRemain, out act);
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
        //½£Îè
        if ((breaking || JobGauge.Esprit >= 80) &&
            Actions.SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //ÌáÄÉÀ­
        if (Actions.Tillana.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = Actions.TechnicalStep.RecastTimeRemain == 0 || Actions.TechnicalStep.RecastTimeRemain > 5;

        if (!LocalPlayer.HaveStatus(ObjectStatus.TechnicalFinish))
        {
            //±ê×¼Îè²½
            if (canstandard && Actions.StandardStep.ShouldUse(out act)) return true;
        }

        //ÓÃµôBuff
        if (Actions.Bloodshower.ShouldUse(out act)) return true;
        if (Actions.Fountainfall.ShouldUse(out act)) return true;

        if (Actions.RisingWindmill.ShouldUse(out act)) return true;
        if (Actions.ReverseCascade.ShouldUse(out act)) return true;


        //±ê×¼Îè²½
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
