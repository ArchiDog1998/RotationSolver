using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.SMNCombo;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal sealed class SMNCombo : JobGaugeCombo<SMNGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.None };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public class SMNAction : BaseAction
    {
        public SMNAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {

        }
    }
    public override uint[] JobIDs => new uint[] { 27, 26 };
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Resurrection;

    private static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Deathflare.ID;
    private static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Rekindle.ID;
    private static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;

    public static readonly SMNAction
        //宝石耀
        Gemshine = new(25883)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //宝石辉
        PreciousBrilliance = new(25884)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //毁灭 单体攻击
        Ruin = new(163),

        //迸裂 范围伤害
        Outburst = new(16511);

    public static readonly BaseAction
        //宝石兽召唤
        SummonCarbuncle = new(25798)
        {
            OtherCheck = b => !TargetUpdater.HavePet,
        },

        //灼热之光 团辅
        SearingLight = new(25801)
        {
            OtherCheck = b => InCombat && !InBahamut && !InPhoenix
        },

        //守护之光 给自己戴套
        RadiantAegis = new(25799),

        //医术
        Physick = new(16230, true),

        //以太蓄能 
        Aethercharge = new(25800)
        {
            OtherCheck = b => InCombat,
        },

        //龙神召唤
        SummonBahamut = new(7427),

        //红宝石召唤
        SummonRuby = new(25802)
        {
            OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
        },

        //黄宝石召唤
        SummonTopaz = new(25803)
        {
            OtherCheck = b => JobGauge.IsTitanReady,
        },

        //绿宝石召唤
        SummonEmerald = new(25804)
        {
            OtherCheck = b => JobGauge.IsGarudaReady,
        },


        //复生
        Resurrection = new(173, true),

        //能量吸收
        EnergyDrain = new(16508),

        //能量抽取
        EnergySiphon = new(16510),

        //溃烂爆发
        Fester = new(181),

        //痛苦核爆
        Painflare = new(3578),

        //毁绝
        RuinIV = new(7426)
        {
            BuffsNeed = new[] { ObjectStatus.FurtherRuin },
        },

        //龙神迸发
        EnkindleBahamut = new(7429)
        {
            OtherCheck = b => InBahamut || InPhoenix,
        },

        //死星核爆
        Deathflare = new(3582)
        {
            OtherCheck = b => InBahamut,
        },

        //苏生之炎
        Rekindle = new(25830, true)
        {
            OtherCheck = b => InPhoenix,
        },

        //深红旋风
        CrimsonCyclone = new(25835)
        {
            BuffsNeed = new[] { ObjectStatus.IfritsFavor },
        },

        //深红强袭
        CrimsonStrike = new(25885),

        //山崩
        MountainBuster = new(25836)
        {
            BuffsNeed = new[] { ObjectStatus.TitansFavor },
        },

        //螺旋气流
        Slipstream = new(25837)
        {
            BuffsNeed = new[] { ObjectStatus.GarudasFavor },
        };
    public override SortedList<DescType, string> Description => new()
    {
        {DescType.单体防御, $"{RadiantAegis}"},
        {DescType.单体治疗, $"{Physick}"},
    };

    private protected override bool MoveGCD(out IAction act)
    {
        if (CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //宝石兽召唤
        if (SummonCarbuncle.ShouldUse(out act)) return true;

        //大招
        if (!InBahamut && !InPhoenix)
        {
            if (RuinIV.ShouldUse(out act, mustUse: true)) return true;
            if (CrimsonStrike.ShouldUse(out act, mustUse: true)) return true;
            if (CrimsonCyclone.ShouldUse(out act, mustUse: true))
            {
                if (CrimsonCyclone.Target.DistanceToPlayer() < 2)
                {
                    return true;
                }
            }
            if (Slipstream.ShouldUse(out act, mustUse: true)) return true;
        }


        //召唤
        if (JobGauge.Attunement == 0)
        {
            if (SummonBahamut.ShouldUse(out act))
            {
                if (SearingLight.IsCoolDown || !SearingLight.EnoughLevel)
                    return true;
            }
            else if (Aethercharge.ShouldUse(out act)) return true;

            if (JobGauge.IsIfritReady && JobGauge.IsGarudaReady && JobGauge.IsTitanReady ? JobGauge.SummonTimerRemaining == 0 : true)
            {
                switch (Config.GetComboByName("SummonOrder"))
                {
                    default:
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 1:
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 2:
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        break;
                    case 3:
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 4:
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 5:
                        //绿 风
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //黄 土
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //红 火
                        if (SummonRuby.ShouldUse(out act)) return true;
                        break;
                }
            }
        }

        //AOE
        if (PreciousBrilliance.ShouldUse(out act)) return true;
        if (Outburst.ShouldUse(out act)) return true;

        //单体
        if (Gemshine.ShouldUse(out act)) return true;
        if (Ruin.ShouldUse(out act)) return true;
        return false;
    }
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetCombo("SummonOrder", 0, new string[]
        {
            "红-黄-绿", "红-绿-黄", "黄-绿-红", "黄-红-绿", "绿-红-黄", "绿-黄-红",

        }, "三神召唤顺序");
    }
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //灼热之光
            if (SearingLight.ShouldUse(out act, mustUse: true)) return true;
        }

        if (EnkindleBahamut.ShouldUse(out act, mustUse: true)) return true;
        if (Deathflare.ShouldUse(out act, mustUse: true)) return true;
        if (Rekindle.ShouldUse(out act, mustUse: true)) return true;
        if (MountainBuster.ShouldUse(out act, mustUse: true)) return true;


        //能量吸收
        if (JobGauge.HasAetherflowStacks && InBreak)
        {
            if (Painflare.ShouldUse(out act)) return true;
            if (Fester.ShouldUse(out act)) return true;
        }
        else
        {
            if (EnergySiphon.ShouldUse(out act)) return true;
            if (EnergyDrain.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //守护之光
        if (RadiantAegis.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //医术
        if (Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //混乱
        if (GeneralActions.Addle.ShouldUse(out act)) return true;
        return false;
    }
}
