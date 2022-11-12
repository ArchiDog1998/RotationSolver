using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.SMNCombos.SMNCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.SMNCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedMagicial/SMNCombos/SMNCombo_Default.cs")]
internal sealed class SMNCombo_Default : SMNCombo_Base<CommandType>
{
    public override string Author => "秋水";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    protected override bool CanHealSingleSpell => false;

    public override SortedList<DescType, string> DescriptionDict => new()
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
        if (Addle.ShouldUse(out act)) return true;
        return false;
    }
}
