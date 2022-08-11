using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class DNCCombo : CustomComboJob<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //ÆÙÐº
            Cascade = new BaseAction(15989)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //ÅçÈª
            Fountain = new BaseAction(15990)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //ÄæÆÙÐº
            ReverseCascade = new BaseAction(15991)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //×¹ÅçÈª
            Fountainfall = new BaseAction(15992)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //ÉÈÎè¡¤Ðò
            FanDance = new BaseAction(16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //·ç³µ
            Windmill = new BaseAction(15993)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //ÂäÈÐÓê
            Bladeshower = new BaseAction(15994)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //Éý·ç³µ
            RisingWindmill = new BaseAction(15995)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //ÂäÑªÓê
            Bloodshower = new BaseAction(15996)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //ÉÈÎè¡¤ÆÆ
            FanDance2 = new BaseAction(16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //ÉÈÎè¡¤¼±
            FanDance3 = new BaseAction(16009)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //ÉÈÎè¡¤ÖÕ
            FanDance4 = new BaseAction(25791)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FourfoldFanDance },
            },

            //½£Îè
            SaberDance = new BaseAction(16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //Á÷ÐÇÎè
            StarfallDance = new BaseAction(25792)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingStarfall },
            },

            //Ç°³å²½
            EnAvant = new BaseAction(16010, shouldEndSpecial: true),

            //Ç¾Þ±Çú½Å²½
            Emboite = new BaseAction(15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //Ð¡Äñ½»µþÌø
            Entrechat = new BaseAction(16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //ÂÌÒ¶Ð¡ÌßÍÈ
            Jete = new BaseAction(16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //½ð¹ÚÖº¼â×ª
            Pirouette = new BaseAction(16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //±ê×¼Îè²½
            StandardStep = new BaseAction(15997)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //¼¼ÇÉÎè²½
            TechnicalStep = new BaseAction(15998)
            {
                BuffsNeed = new ushort[]
                {
                    ObjectStatus.StandardFinish,
                },
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //·ÀÊØÖ®É£°Í
            ShieldSamba = new BaseAction(16012, true)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            },

            //ÖÎÁÆÖ®»ª¶û×È
            CuringWaltz = new BaseAction(16015, true),

            //±ÕÊ½Îè×Ë
            ClosedPosition = new BaseAction(16006, true)
            {
                ChoiceFriend = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId && b.CurrentHp != 0 &&
                    //Remove Weak
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                    //Remove other partner.
                    b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != Service.ClientState.LocalPlayer.ObjectId).Count() == 0).ToArray();

                    var targets = TargetHelper.GetJobCategory(Targets, Role.½üÕ½);
                    if (targets.Length > 0) return targets[0];

                    targets = TargetHelper.GetJobCategory(Targets, Role.Ô¶³Ì);
                    if (targets.Length > 0) return targets[0];

                    targets = Targets;
                    if (targets.Length > 0) return targets[0];

                    return null;
                },

                //OtherCheck = b => !JobGauge.IsDancing,
                //BuffsProvide = new ushort[]
                //{
                //    ObjectStatus.ClosedPosition1,
                //    ObjectStatus.ClosedPosition2,
                //},
            },

            //½ø¹¥Ö®Ì½¸ê
            Devilment = new BaseAction(16011, true),

            //°Ù»¨ÕùÑÞ
            Flourish = new BaseAction(16013)
            {
                BuffsNeed = new ushort[] { ObjectStatus.StandardFinish },
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
                }
            },

            //¼´ÐË±íÑÝ
            Improvisation = new BaseAction(16014, true),

            //ÌáÀ­ÄÉ
            Tillana = new BaseAction(25790)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingFinish },
            };
    }

    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.·¶Î§·ÀÓù, $"{Actions.ShieldSamba.Action.Name}"},
        {DescType.·¶Î§ÖÎÁÆ, $"{Actions.CuringWaltz.Action.Name}, {Actions.Improvisation.Action.Name}"},
        {DescType.ÒÆ¶¯, $"{Actions.EnAvant.Action.Name}"},
    };

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.ClosedPosition1))
        {
            foreach (var friend in TargetHelper.PartyMembers)
            {
                if (BaseAction.FindStatusFromSelf(friend, ObjectStatus.ClosedPosition2)?.Length > 0)
                {
                    if (Actions.ClosedPosition.ShouldUseAction(out act) && Actions.ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (Actions.ClosedPosition.ShouldUseAction(out act)) return true;

        //³¢ÊÔ±¬·¢
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.TechnicalFinish)
        && Actions.Devilment.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //°Ù»¨
        if (Actions.Flourish.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //ÉÈÎè¡¤¼±
        if (Actions.FanDance4.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.FanDance3.ShouldUseAction(out act, mustUse: true)) return true;

        //ÉÈÎè
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Devilment) || JobGauge.Feathers > 3 || Service.ClientState.LocalPlayer.Level < 70)
        {
            if (Actions.FanDance2.ShouldUseAction(out act)) return true;
            if (Actions.FanDance.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnAvant.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.CuringWaltz.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.Improvisation.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShieldSamba.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!TargetHelper.InBattle && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.ClosedPosition1) && Actions.ClosedPosition.ShouldUseAction(out act)) return true;

        if (SettingBreak)
        {
            if (Actions.TechnicalStep.ShouldUseAction(out act)) return true;
        }

        if (StepGCD(out act)) return true;
        if (AttackGCD(out act, BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Devilment), lastComboActionID)) return true;

        return false;
    }
    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Service.ClientState.LocalPlayer.Level < Actions.TechnicalStep.Level
            && Actions.Devilment.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return base.BreakAbility(abilityRemain, out act);
    }

    private bool StepGCD(out IAction act)
    {
        act = null;
        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.StandardStep, ObjectStatus.TechnicalStep)) return false;

        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = Actions.StandardStep;
            return true;
        }
        else if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = Actions.TechnicalStep;
            return true;
        }
        else
        {
            if (Actions.Emboite.ShouldUseAction(out act)) return true;
            if (Actions.Entrechat.ShouldUseAction(out act)) return true;
            if (Actions.Jete.ShouldUseAction(out act)) return true;
            if (Actions.Pirouette.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking, uint lastComboActionID)
    {
        //½£Îè
        if ((breaking || JobGauge.Esprit >= 75) &&
            Actions.SaberDance.ShouldUseAction(out act, mustUse: true)) return true;

        //ÌáÄÉÀ­
        if (Actions.Tillana.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.StarfallDance.ShouldUseAction(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = Actions.TechnicalStep.RecastTimeRemain == 0 || Actions.TechnicalStep.RecastTimeRemain > 5;

        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.TechnicalFinish))
        {
            //±ê×¼Îè²½
            if (canstandard && Actions.StandardStep.ShouldUseAction(out act)) return true;
        }

        //ÓÃµôBuff
        if (Actions.Bloodshower.ShouldUseAction(out act)) return true;
        if (Actions.Fountainfall.ShouldUseAction(out act)) return true;

        if (Actions.RisingWindmill.ShouldUseAction(out act)) return true;
        if (Actions.ReverseCascade.ShouldUseAction(out act)) return true;


        //±ê×¼Îè²½
        if (canstandard && Actions.StandardStep.ShouldUseAction(out act)) return true;


        //aoe
        if (Actions.Bladeshower.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Windmill.ShouldUseAction(out act)) return true;

        //single
        if (Actions.Fountain.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Cascade.ShouldUseAction(out act)) return true;

        return false;
    }
}
