using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal class SMNCombo : JobGaugeCombo<SMNGauge>
{
    public class SMNAction : BaseAction
    {
        internal override int Cast100 => new BaseAction(Service.IconReplacer.OriginalHook(ID)).Cast100;
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
        public static readonly SMNAction
            //宝石耀
            Gemshine = new (25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //宝石辉
            PreciousBrilliance = new (25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //毁灭 单体攻击
            Ruin = new (163),

            //迸裂 范围伤害
            Outburst = new (16511);

        public static readonly BaseAction
            //宝石兽召唤
            SummonCarbuncle = new (25798)
            {
                OtherCheck = b => !TargetHelper.HavePet,
            },

            //灼热之光 团辅
            SearingLight = new (25801)
            {
                OtherCheck = b => InBattle && !InBahamut && !InPhoenix
            },

            //守护之光 给自己戴套
            RadiantAegis = new (25799),

            //医术
            Physick = new (16230, true),

            //以太蓄能 
            Aethercharge = new (25800)
            {
                OtherCheck = b => InBattle,
            },

            //龙神召唤
            SummonBahamut = new (7427),

            //红宝石召唤
            SummonRuby = new (25802)
            {
                OtherCheck = b => JobGauge.IsIfritReady && !XIVAutoAttackPlugin.movingController.IsMoving,
            },

            //黄宝石召唤
            SummonTopaz = new (25803)
            {
                OtherCheck = b => JobGauge.IsTitanReady,
            },

            //绿宝石召唤
            SummonEmerald = new (25804)
            {
                OtherCheck = b => JobGauge.IsGarudaReady,
            },


            //复生
            Resurrection = new (173, true),

            //能量吸收
            EnergyDrain = new (16508),

            //能量抽取
            EnergySiphon = new (16510),

            //溃烂爆发
            Fester = new (181),

            //痛苦核爆
            Painflare = new (3578),

            //毁绝
            RuinIV = new (7426)
            {
                BuffsNeed = new [] { ObjectStatus.FurtherRuin },
            },

            //龙神迸发
            EnkindleBahamut = new (7429)
            {
                OtherCheck = b => InBahamut || InPhoenix,
            },

            //死星核爆
            Deathflare = new (3582)
            {
                OtherCheck = b => InBahamut,
            },

            //苏生之炎
            Rekindle = new (25830, true)
            {
                OtherCheck = b => InPhoenix,
            },

            //深红旋风
            CrimsonCyclone = new (25835)
            {
                BuffsNeed = new [] { ObjectStatus.IfritsFavor },
            },

            //深红强袭
            CrimsonStrike = new (25885),

            //山崩
            MountainBuster = new (25836)
            {
                BuffsNeed = new [] { ObjectStatus.TitansFavor },
            },

            //螺旋气流
            Slipstream = new (25837)
            {
                BuffsNeed = new [] { ObjectStatus.GarudasFavor },
            };
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.RadiantAegis.Action.Name}"},
        {DescType.单体治疗, $"{Actions.Physick.Action.Name}"},
    };

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //灼热之光
        if (Actions.SearingLight.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;
        return base.MoveGCD(lastComboActionID, out act);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //宝石兽召唤
        if (Actions.SummonCarbuncle.ShouldUse(out act)) return true;

        //大招
        if (!InBahamut && !InPhoenix)
        {
            if (Actions.RuinIV.ShouldUse(out act, mustUse: true)) return true;
            if (Actions.CrimsonStrike.ShouldUse(out act, lastComboActionID, mustUse: true)) return true;
            if (Actions.CrimsonCyclone.ShouldUse(out act, mustUse: true))
            {
                if (Actions.CrimsonCyclone.Target.DistanceToPlayer() < 2)
                {
                    return true;
                }
            }
            if (Actions.Slipstream.ShouldUse(out act, mustUse: true)) return true;
        }


        //召唤
        if (JobGauge.Attunement == 0)
        {
            if (Actions.SummonBahamut.ShouldUse(out act))
            {
                if (Actions.SearingLight.IsCoolDown || Level < Actions.SearingLight.Level)
                    return true;
            }
            else if (Actions.Aethercharge.ShouldUse(out act)) return true;

            if (JobGauge.IsIfritReady && JobGauge.IsGarudaReady && JobGauge.IsTitanReady ? JobGauge.SummonTimerRemaining == 0 : true)
            {
                switch (Config.GetComboByName("SummonOrder"))
                {
                    default:
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 1:
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 2:
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        break;
                    case 3:
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 4:
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 5:
                        //绿 风
                        if (Actions.SummonEmerald.ShouldUse(out act)) return true;
                        //黄 土
                        if (Actions.SummonTopaz.ShouldUse(out act)) return true;
                        //红 火
                        if (Actions.SummonRuby.ShouldUse(out act)) return true;
                        break;
                }
            }
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUse(out act)) return true;
        if (Actions.Outburst.ShouldUse(out act)) return true;

        //单体
        if (Actions.Gemshine.ShouldUse(out act)) return true;
        if (Actions.Ruin.ShouldUse(out act)) return true;
        return false;
    }
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetCombo("SummonOrder", 0, new string[]
        {
            "红-黄-绿", "红-绿-黄", "黄-绿-红", "黄-红-绿", "绿-红-黄", "绿-黄-红",

        }, "三神召唤顺序");
    }
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnkindleBahamut.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.Deathflare.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUse(out act, mustUse: true)) return true;


        //能量吸收
        if (JobGauge.HasAetherflowStacks && InBreak)
        {
            if (Actions.Painflare.ShouldUse(out act)) return true;
            if (Actions.Fester.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.EnergySiphon.ShouldUse(out act)) return true;
            if (Actions.EnergyDrain.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //守护之光
        if (Actions.RadiantAegis.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //医术
        if (Actions.Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //混乱
        if (GeneralActions.Addle.ShouldUse(out act)) return true;
        return false;
    }
}
