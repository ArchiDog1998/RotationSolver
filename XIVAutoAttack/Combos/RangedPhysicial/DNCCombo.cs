using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class DNCCombo : CustomComboJob<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //∆Ÿ–∫
            Cascade = new BaseAction(15989)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //≈Á»™
            Fountain = new BaseAction(15990)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //ƒÊ∆Ÿ–∫
            ReverseCascade = new BaseAction(15991)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //◊π≈Á»™
            Fountainfall = new BaseAction(15992)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //…»ŒË°§–Ú
            FanDance = new BaseAction(16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //∑Á≥µ
            Windmill = new BaseAction(15993)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //¬‰»–”Í
            Bladeshower = new BaseAction(15994)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //…˝∑Á≥µ
            RisingWindmill = new BaseAction(15995)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //¬‰—™”Í
            Bloodshower = new BaseAction(15996)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //…»ŒË°§∆∆
            FanDance2 = new BaseAction(16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //…»ŒË°§º±
            FanDance3 = new BaseAction(16009)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //…»ŒË°§÷’
            FanDance4 = new BaseAction(25791)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FourfoldFanDance },
            },

            //Ω£ŒË
            SaberDance = new BaseAction(16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //¡˜–«ŒË
            StarfallDance = new BaseAction(25792)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingStarfall },
            },

            //«∞≥Â≤Ω
            EnAvant = new BaseAction(16010),

            //«æﬁ±«˙Ω≈≤Ω
            Emboite = new BaseAction(15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //–°ƒÒΩªµ˛Ã¯
            Entrechat = new BaseAction(16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //¬Ã“∂–°ÃﬂÕ»
            Jete = new BaseAction(16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //Ωπ⁄÷∫º‚◊™
            Pirouette = new BaseAction(16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //±Í◊ºŒË≤Ω
            StandardStep = new BaseAction(15997)
            {
                BuffsProvide = new ushort[] { ObjectStatus.StandardStep },
            },

            //ºº«…ŒË≤Ω
            TechnicalStep = new BaseAction(15998)
            {
                BuffsProvide = new ushort[] { ObjectStatus.TechnicalStep },
            },

            //∑¿ ÿ÷Æ…£∞Õ
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

            //÷Œ¡∆÷Æª™∂˚◊»
            CuringWaltz = new BaseAction(16015, true),

            //±’ ΩŒË◊À
            ClosedPosition = new BaseAction(16006, true)
            {
                ChoiceFriend = ASTCombo.ASTMeleeTarget,
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.ClosedPosition1,
                    ObjectStatus.ClosedPosition2,
                },
            },

            //Ω¯π•÷ÆÃΩ∏Í
            Devilment = new BaseAction(16011, true),

            //∞Ÿª®’˘—ﬁ
            Flourish = new BaseAction(16013)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.SilkenSymmetry,
                    ObjectStatus.SilkenFlow,
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
                }
            },

            //º¥–À±Ì—›
            Improvisation = new BaseAction(16014, true),

            //Ã·¿≠ƒ…
            Tillana = new BaseAction(25790)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingFinish },
            };
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Flourish.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Devilment.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.EnAvant.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.CuringWaltz.ShouldUseAction(out act, Empty: true)) return true;
        if (Actions.Improvisation.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ShieldSamba.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.StandardStep.ShouldUseAction(out act)) return true;
        if (SettingBreak)
        {
            if (Actions.TechnicalStep.ShouldUseAction(out act)) return true;
        }
        if (Actions.Tillana.ShouldUseAction(out act, mustUse: true)) return true;


        if (StepGCD(out act)) return true;
        if (AttackGCD(out act, BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Devilment), lastComboActionID)) return true;

        return false;
    }

    private bool StepGCD(out BaseAction act)
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

        if (Actions.Emboite.ShouldUseAction(out act)) return true;
        if (Actions.Entrechat.ShouldUseAction(out act)) return true;
        if (Actions.Jete.ShouldUseAction(out act)) return true;
        if (Actions.Pirouette.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ClosedPosition.ShouldUseAction(out act)) return true;
        return false;
    }

    private bool AttackGCD(out BaseAction act, bool breaking, uint lastComboActionID)
    {
        //Ω£ŒË
        if ((breaking || JobGauge.Esprit >= 75) && 
            Actions.SaberDance.ShouldUseAction(out act)) return true;

        if (Actions.StarfallDance.ShouldUseAction(out act)) return true;


        //…»ŒË°§º±
        if (Actions.FanDance4.ShouldUseAction(out act)) return true;
        if (Actions.FanDance3.ShouldUseAction(out act)) return true;

        //…»ŒË
        if (breaking || JobGauge.Feathers > 3)
        {
            if (Actions.FanDance2.ShouldUseAction(out act)) return true;
            if (Actions.FanDance.ShouldUseAction(out act)) return true;
        }

        //”√µÙBuff
        if (Actions.Bloodshower.ShouldUseAction(out act)) return true;
        if (Actions.Fountainfall.ShouldUseAction(out act)) return true;

        if (Actions.RisingWindmill.ShouldUseAction(out act)) return true;
        if (Actions.ReverseCascade.ShouldUseAction(out act)) return true;

        //aoe
        if (Actions.Bladeshower.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Windmill.ShouldUseAction(out act)) return true;

        //single
        if (Actions.Fountain.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Cascade.ShouldUseAction(out act)) return true;

        return false;
    }

    //public static class Buffs
    //{
    //    public const ushort FlourishingSymmetry = 2693;

    //    public const ushort FlourishingFlow = 2694;

    //    public const ushort FlourishingFinish = 2698;

    //    public const ushort FlourishingStarfall = 2700;

    //    public const ushort StandardStep = 1818;

    //    public const ushort TechnicalStep = 1819;

    //    public const ushort ThreefoldFanDance = 1820;

    //    public const ushort FourfoldFanDance = 2699;
    //}

}
