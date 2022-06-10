using Dalamud.Game.ClientState.JobGauge.Types;

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
            //宝石兽召唤
            SummonCarbuncle = new BaseAction(25798)
            {
                OtherCheck = b => !TargetHelper.HavePet,
            },

            //灼热之光 团辅
            SearingLight = new BaseAction(25801)
            {
                OtherCheck = b => TargetHelper.InBattle && !InBahamut && !InPhoenix &&
                JobGauge.ReturnSummon == Dalamud.Game.ClientState.JobGauge.Enums.SummonPet.NONE,
            },

            //守护之光 给自己戴套
            RadiantAegis = new BaseAction(25799),

            //医术
            Physick = new BaseAction(16230, true),

            //以太蓄能 
            Aethercharge = new BaseAction(25800)
            {
                OtherCheck = b => TargetHelper.InBattle,
            },

            //龙神召唤
            SummonBahamut = new BaseAction(7427),

            //红宝石召唤
            SummonRuby = new BaseAction(25802)
            {
                OtherCheck = b => JobGauge.IsIfritReady,
            },

            //黄宝石召唤
            SummonTopaz = new BaseAction(25803)
            {
                OtherCheck = b => JobGauge.IsTitanReady,
            },

            //绿宝石召唤
            SummonEmerald = new BaseAction(25804)
            {
                OtherCheck = b => JobGauge.IsGarudaReady,
            },

            //宝石耀
            Gemshine = new SMNAction(25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //宝石辉
            PreciousBrilliance = new SMNAction(25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //毁灭 单体攻击
            Ruin = new SMNAction(163),

            //迸裂 范围伤害
            Outburst = new SMNAction(16511),

            //复生
            Resurrection = new BaseAction(173, true),

            //能量吸收
            EnergyDrain = new BaseAction(16508),

            //能量抽取
            EnergySiphon = new BaseAction(16510),

            //溃烂爆发
            Fester = new BaseAction(181),

            //痛苦核爆
            Painflare = new BaseAction(3578),

            //毁绝
            RuinIV = new BaseAction(7426)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FurtherRuin },
            },

            //龙神迸发
            EnkindleBahamut = new BaseAction(7429)
            {
                OtherCheck = b => InBahamut || InPhoenix,
            },

            //死星核爆
            Deathflare = new BaseAction(3582)
            {
                OtherCheck = b => InBahamut,
            },

            //苏生之炎
            Rekindle = new BaseAction(25830, true)
            {
                OtherCheck = b => InPhoenix,
            },

            //深红旋风
            CrimsonCyclone = new BaseAction(25835)
            {
                BuffsNeed = new ushort[] { ObjectStatus.IfritsFavor },
            },

            //深红强袭
            CrimsonStrike = new BaseAction(25885),

            //山崩
            MountainBuster = new BaseAction(25836)
            {
                BuffsNeed = new ushort[] { ObjectStatus.TitansFavor },
            },

            //螺旋气流
            Slipstream = new BaseAction(25837)
            {
                BuffsNeed = new ushort[] { ObjectStatus.GarudasFavor },
            };
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //灼热之光
        if (Actions.SearingLight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //宝石兽召唤
        if (Actions.SummonCarbuncle.ShouldUseAction(out act)) return true;

        //大招
        if (!InBahamut && !InPhoenix)
        {
            if (Actions.RuinIV.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.CrimsonStrike.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
            if (Actions.CrimsonCyclone.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.Slipstream.ShouldUseAction(out act, mustUse: true)) return true;
        }


        //召唤
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
                //火
                if (!TargetHelper.IsMoving && Actions.SummonRuby.ShouldUseAction(out act)) return true;

                //风
                if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                //土
                if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
            }
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUseAction(out act)) return true;
        if (Actions.Outburst.ShouldUseAction(out act)) return true;

        //单体
        if (Actions.Gemshine.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnkindleBahamut.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Deathflare.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUseAction(out act, mustUse: true)) return true;


        //能量吸收
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
        //守护之光
        if (Actions.RadiantAegis.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //医术
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }

}
