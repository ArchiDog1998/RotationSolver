using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedMagicial.SMNCombos.SMNCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.SMNCombos;

internal sealed class SMNCombo_Default : SMNCombo_Base<CommandType>
{
    public override string GameVersion => "6.28";

    public override string Author => "逆光";

    internal enum CommandType : byte
    {
        None,
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("addSwiftcast", 0, "将即刻咏唱加入循环", "关（留着复活乱死的笨比）", "给风神", "给火神", "智能（我全都要）")
            .SetCombo("SummonOrder", 0, "三神召唤顺序", "土神优先1：土-风-火", "土神优先2：土-火-风", "风神优先1：风-土-火")
            .SetFloat("CrimsonCycloneRange", 2, "多远距离内可以使用火神突进", min: 0, max: 25, speed: 1);
    }

    public SMNCombo_Default()
    {
        RuinIV.ComboCheck = b => !Player.HasStatus(true, StatusID.Swiftcast);
    }

    protected override bool CanHealSingleSpell => false;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseSingle, $"{RadiantAegis}"},
        {DescType.HealSingle, $"{Physick}"},
    };

    private protected override bool MoveGCD(out IAction act)
    {
        //火神突进
        if (CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //宝石兽召唤
        if (SummonCarbuncle.ShouldUse(out act)) return true;

        //风神读条
        if (Slipstream.ShouldUse(out act, mustUse: true)) return true;
        //火神冲锋
        if (CrimsonStrike.ShouldUse(out act, mustUse: true)) return true;
        float CrimsonCycloneRange = Config.GetFloatByName("CrimsonCycloneRange");
        if (CrimsonCyclone.Target.DistanceToPlayer() <= CrimsonCycloneRange && CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;

        //AOE
        if (PreciousBrilliance.ShouldUse(out act)) return true;
        //单体
        if (Gemshine.ShouldUse(out act)) return true;

        //龙神不死鸟
        if (SummonBahamut.ShouldUse(out act)) return true;
        if (!SummonBahamut.EnoughLevel && HaveHostilesInRange && Aethercharge.ShouldUse(out act)) return true;

        //召唤蛮神
        switch (Config.GetComboByName("SummonOrder"))
        {
            default:
                //土
                if (SummonTopaz.ShouldUse(out act)) return true;
                //风
                if (SummonEmerald.ShouldUse(out act)) return true;
                //火
                if (SummonRuby.ShouldUse(out act)) return true;
                break;

            case 1:
                //土
                if (SummonTopaz.ShouldUse(out act)) return true;
                //火
                if (SummonRuby.ShouldUse(out act)) return true;
                //风
                if (SummonEmerald.ShouldUse(out act)) return true;
                break;

            case 2:
                //风
                if (SummonEmerald.ShouldUse(out act)) return true;
                //土
                if (SummonTopaz.ShouldUse(out act)) return true;
                //火
                if (SummonRuby.ShouldUse(out act)) return true;
                break;
        }

        //迸裂三灾
        if (Outburst.ShouldUse(out act)) return true;

        //毁4
        if ((IsMoving && ((Player.HasStatus(true, StatusID.GarudasFavor) && !InGaruda) || (InIfrit && !IsLastGCD(true, CrimsonCyclone)))) ||
            (SummonTimerRemaining == 0 && AttunmentTimerRemaining == 0))
        {
            if (RuinIV.ShouldUse(out act, mustUse: true)) return true;
        }
        //毁123
        if (Ruin.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //灼热之光
            if (InBahamut && SearingLight.ShouldUse(out act)) return true;
        }

        //龙神不死鸟迸发
        if (((InBahamut && SummonTimeEndAfter(7)) || InPhoenix || (IsTargetBoss && IsTargetDying)) && EnkindleBahamut.ShouldUse(out act, mustUse: true)) return true;
        //死星核爆
        if ((SummonTimeEndAfter(7) || (IsTargetBoss && IsTargetDying)) && Deathflare.ShouldUse(out act, mustUse: true)) return true;
        //苏生之炎
        if (Rekindle.ShouldUse(out act, mustUse: true)) return true;
        //山崩
        if (MountainBuster.ShouldUse(out act, mustUse: true)) return true;

        //痛苦核爆
        if (((!SearingLight.WillHaveOneCharge(90) && ((InBahamut && SummonTimeEndAfter(7)) || !InBahamut || !EnergyDrain.IsCoolDown)) ||
            !SearingLight.EnoughLevel ||
            (IsTargetBoss && IsTargetDying)) && Painflare.ShouldUse(out act)) return true;
        //溃烂爆发
        if (((!SearingLight.WillHaveOneCharge(90) && ((InBahamut && SummonTimeEndAfter(7)) || !InBahamut || !EnergyDrain.IsCoolDown)) ||
            !SearingLight.EnoughLevel ||
            (IsTargetBoss && IsTargetDying)) && Fester.ShouldUse(out act)) return true;

        //能量抽取
        if (EnergySiphon.ShouldUse(out act)) return true;
        //能量吸收
        if (EnergyDrain.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //即刻进循环
        switch (Config.GetComboByName("addSwiftcast"))
        {
            default:
                break;
            case 1:
                if (InGaruda && Player.HasStatus(true, StatusID.GarudasFavor) && nextGCD.IsAnySameAction(true, Slipstream))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;
            case 2:
                if (InIfrit && nextGCD.IsAnySameAction(true, Gemshine, PreciousBrilliance))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;

            case 3:
                if ((InGaruda && Player.HasStatus(true, StatusID.GarudasFavor) && nextGCD.IsAnySameAction(true, Slipstream)) || (InIfrit && nextGCD.IsAnySameAction(true, Gemshine, PreciousBrilliance)))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        //1.5s预读毁3
        if (remainTime <= 1.5f && Ruin.ShouldUse(out _)) return Ruin;
        return base.CountDownAction(remainTime);
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
        //守护之光
        if (RadiantAegis.ShouldUse(out act)) return true;

        //混乱
        if (Addle.ShouldUse(out act)) return true;
        return false;
    }
}
