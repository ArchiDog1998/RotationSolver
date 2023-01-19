using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.RangedMagicial.SMNCombos;

internal sealed class SMN_Default : SMNRotation_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("addSwiftcast", 0, "将即刻咏唱加入循环", "关（留着复活乱死的笨比）", "给风神", "给火神", "智能（我全都要）")
            .SetCombo("SummonOrder", 0, "三神召唤顺序", "土神优先1：土-风-火", "土神优先2：土-火-风", "风神优先1：风-土-火")
            .SetBool("addCrimsonCyclone", true, "自动释放火神冲锋（不在移动中）");
    }

    public SMN_Default()
    {
        RuinIV.ComboCheck = b => !Player.HasStatus(true, StatusID.Swiftcast) && !InBahamut && !InPhoenix;
        SearingLight.ComboCheck = b => !Player.HasStatus(false, StatusID.SearingLight);
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

        //AOE
        if (PreciousBrilliance.ShouldUse(out act)) return true;
        //单体
        if (Gemshine.ShouldUse(out act)) return true;

        if (!IsMoving && Configs.GetBool("addCrimsonCyclone") && CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;

        //龙神不死鸟
        if ((Player.HasStatus(false, StatusID.SearingLight) || SearingLight.IsCoolDown) && SummonBahamut.ShouldUse(out act)) return true;
        if (!SummonBahamut.EnoughLevel && HaveHostilesInRange && Aethercharge.ShouldUse(out act)) return true;

        //毁4
        if (IsMoving && (Player.HasStatus(true, StatusID.GarudasFavor) || InIfrit) && RuinIV.ShouldUse(out act, mustUse: true)) return true;

        //召唤蛮神
        switch (Configs.GetCombo("SummonOrder"))
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
        if (SummonTimerRemaining == 0 && AttunmentTimerRemaining == 0 && RuinIV.ShouldUse(out act, mustUse: true)) return true;
        //迸裂三灾
        if (Outburst.ShouldUse(out act)) return true;

        //毁123
        if (Ruin.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //灼热之光
            if (SearingLight.ShouldUse(out act)) return true;
        }

        //龙神不死鸟迸发
        if ((InBahamut && SummonBahamut.ElapsedAfterGCD(3) || InPhoenix || IsTargetBoss && IsTargetDying) && EnkindleBahamut.ShouldUse(out act, mustUse: true)) return true;
        //死星核爆
        if ((SummonBahamut.ElapsedAfterGCD(3) || IsTargetBoss && IsTargetDying) && Deathflare.ShouldUse(out act, mustUse: true)) return true;
        //苏生之炎
        if (Rekindle.ShouldUse(out act, mustUse: true)) return true;
        //山崩
        if (MountainBuster.ShouldUse(out act, mustUse: true)) return true;

        //痛苦核爆
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedAfterGCD(3) || !EnergyDrain.IsCoolDown) ||
            !SearingLight.EnoughLevel || IsTargetBoss && IsTargetDying) && Painflare.ShouldUse(out act)) return true;
        //溃烂爆发
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedAfterGCD(3) || !EnergyDrain.IsCoolDown) ||
            !SearingLight.EnoughLevel || IsTargetBoss && IsTargetDying) && Fester.ShouldUse(out act)) return true;

        //能量抽取
        if (EnergySiphon.ShouldUse(out act)) return true;
        //能量吸收
        if (EnergyDrain.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //即刻进循环
        switch (Configs.GetCombo("addSwiftcast"))
        {
            default:
                break;
            case 1:
                if (nextGCD.IsAnySameAction(true, Slipstream) || Attunement == 0 && Player.HasStatus(true, StatusID.GarudasFavor))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;
            case 2:
                if (InIfrit && (nextGCD.IsAnySameAction(true, Gemshine, PreciousBrilliance) || IsMoving))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;

            case 3:
                if (nextGCD.IsAnySameAction(true, Slipstream) || Attunement == 0 && Player.HasStatus(true, StatusID.GarudasFavor) ||
                   InIfrit && (nextGCD.IsAnySameAction(true, Gemshine, PreciousBrilliance) || IsMoving))
                {
                    if (Swiftcast.ShouldUse(out act, mustUse: true)) return true;
                }
                break;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 30 && SummonCarbuncle.ShouldUse(out _)) return SummonCarbuncle;
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
