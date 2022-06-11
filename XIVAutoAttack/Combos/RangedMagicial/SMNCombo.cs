using Dalamud.Game.ClientState.JobGauge.Types;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal class SMNCombo : CustomComboJob<SMNGauge>
{
    public class SMNAction : BaseAction
    {
        internal override int Cast100 => InBahamut || InPhoenix || !JobGauge.IsIfritAttuned ? 0 : base.Cast100;
        public SMNAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {

        }
    }
    internal override uint JobID => 27;
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Actions.Resurrection;

    private static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Actions.Deathflare.ID;
    private static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Actions.Rekindle.ID;
    private static bool InBreak => InBahamut || InPhoenix || Service.ClientState.LocalPlayer.Level < Actions.SummonBahamut.Level;
    internal struct Actions
    {
        public static readonly BaseAction
            //±¦Ê¯ÊÞÕÙ»½
            SummonCarbuncle = new BaseAction(25798)
            {
                OtherCheck = b => !TargetHelper.HavePet,
            },

            //×ÆÈÈÖ®¹â ÍÅ¸¨
            SearingLight = new BaseAction(25801)
            {
                OtherCheck = b => TargetHelper.InBattle && !InBahamut && !InPhoenix &&
                JobGauge.ReturnSummon == Dalamud.Game.ClientState.JobGauge.Enums.SummonPet.NONE,
            },

            //ÊØ»¤Ö®¹â ¸ø×Ô¼º´÷Ì×
            RadiantAegis = new BaseAction(25799),

            //Ò½Êõ
            Physick = new BaseAction(16230, true),

            //ÒÔÌ«ÐîÄÜ 
            Aethercharge = new BaseAction(25800)
            {
                OtherCheck = b => TargetHelper.InBattle,
            },

            //ÁúÉñÕÙ»½
            SummonBahamut = new BaseAction(7427),

            //ºì±¦Ê¯ÕÙ»½
            SummonRuby = new BaseAction(25802)
            {
                OtherCheck = b => JobGauge.IsIfritReady && !TargetHelper.IsMoving,
            },

            //»Æ±¦Ê¯ÕÙ»½
            SummonTopaz = new BaseAction(25803)
            {
                OtherCheck = b => JobGauge.IsTitanReady,
            },

            //ÂÌ±¦Ê¯ÕÙ»½
            SummonEmerald = new BaseAction(25804)
            {
                OtherCheck = b => JobGauge.IsGarudaReady,
            },

            //±¦Ê¯Ò«
            Gemshine = new SMNAction(25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //±¦Ê¯»Ô
            PreciousBrilliance = new SMNAction(25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //»ÙÃð µ¥Ìå¹¥»÷
            Ruin = new SMNAction(163),

            //±ÅÁÑ ·¶Î§ÉËº¦
            Outburst = new SMNAction(16511),

            //¸´Éú
            Resurrection = new BaseAction(173, true),

            //ÄÜÁ¿ÎüÊÕ
            EnergyDrain = new BaseAction(16508),

            //ÄÜÁ¿³éÈ¡
            EnergySiphon = new BaseAction(16510),

            //À£ÀÃ±¬·¢
            Fester = new BaseAction(181),

            //Í´¿àºË±¬
            Painflare = new BaseAction(3578),

            //»Ù¾ø
            RuinIV = new BaseAction(7426)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FurtherRuin },
            },

            //ÁúÉñ±Å·¢
            EnkindleBahamut = new BaseAction(7429)
            {
                OtherCheck = b => InBahamut || InPhoenix,
            },

            //ËÀÐÇºË±¬
            Deathflare = new BaseAction(3582)
            {
                OtherCheck = b => InBahamut,
            },

            //ËÕÉúÖ®Ñ×
            Rekindle = new BaseAction(25830, true)
            {
                OtherCheck = b => InPhoenix,
            },

            //ÉîºìÐý·ç
            CrimsonCyclone = new BaseAction(25835)
            {
                BuffsNeed = new ushort[] { ObjectStatus.IfritsFavor },
            },

            //ÉîºìÇ¿Ï®
            CrimsonStrike = new BaseAction(25885),

            //É½±À
            MountainBuster = new BaseAction(25836)
            {
                BuffsNeed = new ushort[] { ObjectStatus.TitansFavor },
            },

            //ÂÝÐýÆøÁ÷
            Slipstream = new BaseAction(25837)
            {
                BuffsNeed = new ushort[] { ObjectStatus.GarudasFavor },
            };
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //×ÆÈÈÖ®¹â
        if (Actions.SearingLight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //±¦Ê¯ÊÞÕÙ»½
        if (Actions.SummonCarbuncle.ShouldUseAction(out act)) return true;

        //´óÕÐ
        if (!InBahamut && !InPhoenix)
        {
            if (Actions.RuinIV.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.CrimsonStrike.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
            if (Actions.CrimsonCyclone.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.Slipstream.ShouldUseAction(out act, mustUse: true)) return true;
        }


        //ÕÙ»½
        if (JobGauge.Attunement == 0)
        {
            if (Actions.SummonBahamut.ShouldUseAction(out act))
            {
                if (Actions.SearingLight.IsCoolDown || Service.ClientState.LocalPlayer.Level < Actions.SearingLight.Level)
                    return true;
            }
            else if (Actions.Aethercharge.ShouldUseAction(out act)) return true;

            if (JobGauge.IsIfritReady && JobGauge.IsGarudaReady && JobGauge.IsTitanReady ? JobGauge.SummonTimerRemaining == 0 : true)
            {
                switch (config.GetComboByName("SummonOrder"))
                {
                    default:
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        break;
                    case 1:
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        break;
                    case 2:
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        break;
                    case 3:
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        break;
                    case 4:
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        break;
                    case 5:
                        //ÂÌ ·ç
                        if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                        //»Æ ÍÁ
                        if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
                        //ºì »ð
                        if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
                        break;
                }
            }
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUseAction(out act)) return true;
        if (Actions.Outburst.ShouldUseAction(out act)) return true;

        //µ¥Ìå
        if (Actions.Gemshine.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetCombo("SummonOrder", 0, new string[]
        {
            "ºì-»Æ-ÂÌ", "ºì-ÂÌ-»Æ", "»Æ-ÂÌ-ºì", "»Æ-ºì-ÂÌ", "ÂÌ-ºì-»Æ", "ÂÌ-»Æ-ºì",

        }, "ÈýÉñÕÙ»½Ë³Ðò");
    }
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnkindleBahamut.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Deathflare.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUseAction(out act, mustUse: true)) return true;


        //ÄÜÁ¿ÎüÊÕ
        if (JobGauge.HasAetherflowStacks && InBreak)
        {
            if (Actions.Painflare.ShouldUseAction(out act)) return true;
            if (Actions.Fester.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.EnergySiphon.ShouldUseAction(out act)) return true;
            if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //ÊØ»¤Ö®¹â
        if (Actions.RadiantAegis.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //Ò½Êõ
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }


}
