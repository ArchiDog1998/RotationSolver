using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class SMNCombo : CustomComboJob<SMNGauge>
{
    internal override uint JobID => 27;
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Actions.Resurrection;
    internal struct Actions
    {
        public static readonly BaseAction
            //宝石兽召唤
            SummonCarbuncle = new BaseAction(25798)
            {
                OtherCheck = b => JobGauge.ReturnSummon == Dalamud.Game.ClientState.JobGauge.Enums.SummonPet.NONE,
            },

            //灼热之光 团辅
            SearingLight = new BaseAction(25801),

            //守护之光 给自己戴套
            RadiantAegis = new BaseAction(25799),

            //医术
            Physick = new BaseAction(16230, true),

            //以太蓄能 
            Aethercharge = new BaseAction(25800),

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
            Gemshine = new BaseAction(25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //毁灭 单体攻击
            Ruin = new BaseAction(163),

            //宝石辉
            PreciousBrilliance = new BaseAction(25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //迸裂 范围伤害
            Outburst = new BaseAction(16511),

            //复生
            Resurrection = new BaseAction(173, true),

            //能量吸收
            EnergyDrain = new BaseAction(16508)
            {
                BuffsProvide = new ushort[] { ObjectStatus.FurtherRuin },
            },

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
                OtherCheck = b => JobGauge.ReturnSummonGlam == Dalamud.Game.ClientState.JobGauge.Enums.PetGlam.CARBUNCLE,
            },

            //死星核爆
            Deathflare = new BaseAction(3582)
            {
                OtherCheck = b => (byte)JobGauge.ReturnSummon == 10,
            },

            //苏生之炎
            Rekindle = new BaseAction(25830, true)
            {
                OtherCheck = b => (byte)JobGauge.ReturnSummon == 20,
            },

            //深红旋风
            CrimsonCyclone = new BaseAction(25835)
            {
                BuffsNeed = new ushort[] { ObjectStatus.IfritsFavor },
            },

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

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        //灼热之光
        if (Actions.SearingLight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //宝石兽召唤
        if (Actions.SummonCarbuncle.ShouldUseAction(out act)) return true;

        //大招
        if (Actions.CrimsonCyclone.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Slipstream.ShouldUseAction(out act, mustUse: true)) return true;

        if (Actions.RuinIV.ShouldUseAction(out act, mustUse:true)) return true;
        if (Actions.EnkindleBahamut.ShouldUseAction(out act, mustUse:true)) return true;

        //召唤
        if (Actions.Aethercharge.ShouldUseAction(out act)) return true;
        if (JobGauge.Attunement == 0)
        {
            if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
            if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
            if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUseAction(out act)) return true;
        if (Actions.Outburst.ShouldUseAction(out act)) return true;

        //单体
        if (Actions.Gemshine.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Deathflare.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUseAction(out act, mustUse: true)) return true;


        //能量吸收
        if (JobGauge.HasAetherflowStacks)
        {
            if (Actions.Painflare.ShouldUseAction(out act)) return true;
            if (Actions.Fester.ShouldUseAction(out act)) return true;
        }
        else if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //守护之光
        if (Actions.RadiantAegis.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //医术
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }
}
