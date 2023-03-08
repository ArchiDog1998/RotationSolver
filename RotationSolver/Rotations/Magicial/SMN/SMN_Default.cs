using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;

namespace RotationSolver.Rotations.RangedMagicial.SMN;

[DefaultRotation]
[RotationDesc(ActionID.SearingLight)]
internal sealed class SMN_Default : SMN_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("addSwiftcast", 0, "Use Swiftcast", "No", "Wind", "Fire", "All")
            .SetCombo("SummonOrder", 0, "Order", "Soil-Wind-Fire", "Soil-Fire-Wind", "Wind-Soil-Fire")
            .SetBool("addCrimsonCyclone", true, "Auto CrimsonCyclon");
    }

    public SMN_Default()
    {
        RuinIV.RotationCheck = b => !Player.HasStatus(true, StatusID.Swiftcast) && !InBahamut && !InPhoenix;
        SearingLight.RotationCheck = b => !Player.HasStatus(false, StatusID.SearingLight);
    }

    protected override bool CanHealSingleSpell => false;

    [RotationDesc(ActionID.CrimsonCyclone)]
    protected override bool MoveForwardGCD(out IAction act)
    {
        //火神突进
        if (CrimsonCyclone.CanUse(out act, mustUse: true)) return true;
        return base.MoveForwardGCD(out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //宝石兽召唤
        if (SummonCarbuncle.CanUse(out act)) return true;

        //风神读条
        if (Slipstream.CanUse(out act, mustUse: true)) return true;
        //火神冲锋
        if (CrimsonStrike.CanUse(out act, mustUse: true)) return true;

        //AOE
        if (PreciousBrilliance.CanUse(out act)) return true;
        //单体
        if (Gemshine.CanUse(out act)) return true;

        if (!IsMoving && Configs.GetBool("addCrimsonCyclone") && CrimsonCyclone.CanUse(out act, mustUse: true)) return true;

        //龙神不死鸟
        if ((Player.HasStatus(false, StatusID.SearingLight) || SearingLight.IsCoolingDown) && SummonBahamut.CanUse(out act)) return true;
        if (!SummonBahamut.EnoughLevel && HasHostilesInRange && Aethercharge.CanUse(out act)) return true;

        //毁4
        if (IsMoving && (Player.HasStatus(true, StatusID.GarudasFavor) || InIfrit) && RuinIV.CanUse(out act, mustUse: true)) return true;

        //召唤蛮神
        switch (Configs.GetCombo("SummonOrder"))
        {
            default:
                //土
                if (SummonTopaz.CanUse(out act)) return true;
                //风
                if (SummonEmerald.CanUse(out act)) return true;
                //火
                if (SummonRuby.CanUse(out act)) return true;
                break;

            case 1:
                //土
                if (SummonTopaz.CanUse(out act)) return true;
                //火
                if (SummonRuby.CanUse(out act)) return true;
                //风
                if (SummonEmerald.CanUse(out act)) return true;
                break;

            case 2:
                //风
                if (SummonEmerald.CanUse(out act)) return true;
                //土
                if (SummonTopaz.CanUse(out act)) return true;
                //火
                if (SummonRuby.CanUse(out act)) return true;
                break;
        }
        if (SummonTimerRemaining == 0 && AttunmentTimerRemaining == 0 && RuinIV.CanUse(out act, mustUse: true)) return true;
        //迸裂三灾
        if (Outburst.CanUse(out act)) return true;

        //毁123
        if (Ruin.CanUse(out act)) return true;
        return false;
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            //灼热之光
            if (SearingLight.CanUse(out act)) return true;
        }

        //龙神不死鸟迸发
        if ((InBahamut && SummonBahamut.ElapsedAfterGCD(3) || InPhoenix || IsTargetBoss && IsTargetDying) && EnkindleBahamut.CanUse(out act, mustUse: true)) return true;
        //死星核爆
        if ((SummonBahamut.ElapsedAfterGCD(3) || IsTargetBoss && IsTargetDying) && Deathflare.CanUse(out act, mustUse: true)) return true;
        //苏生之炎
        if (Rekindle.CanUse(out act, mustUse: true)) return true;
        //山崩
        if (MountainBuster.CanUse(out act, mustUse: true)) return true;

        //痛苦核爆
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedAfterGCD(3) || !EnergyDrain.IsCoolingDown) ||
            !SearingLight.EnoughLevel || IsTargetBoss && IsTargetDying) && Painflare.CanUse(out act)) return true;
        //溃烂爆发
        if ((Player.HasStatus(false, StatusID.SearingLight) && InBahamut && (SummonBahamut.ElapsedAfterGCD(3) || !EnergyDrain.IsCoolingDown) ||
            !SearingLight.EnoughLevel || IsTargetBoss && IsTargetDying) && Fester.CanUse(out act)) return true;

        //能量抽取
        if (EnergySiphon.CanUse(out act)) return true;
        //能量吸收
        if (EnergyDrain.CanUse(out act)) return true;

        return false;
    }
    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //即刻进循环
        switch (Configs.GetCombo("addSwiftcast"))
        {
            default:
                break;
            case 1:
                if (nextGCD.IsTheSameTo(true, Slipstream) || Attunement == 0 && Player.HasStatus(true, StatusID.GarudasFavor))
                {
                    if (Swiftcast.CanUse(out act, mustUse: true)) return true;
                }
                break;
            case 2:
                if (InIfrit && (nextGCD.IsTheSameTo(true, Gemshine, PreciousBrilliance) || IsMoving))
                {
                    if (Swiftcast.CanUse(out act, mustUse: true)) return true;
                }
                break;

            case 3:
                if (nextGCD.IsTheSameTo(true, Slipstream) || Attunement == 0 && Player.HasStatus(true, StatusID.GarudasFavor) ||
                   InIfrit && (nextGCD.IsTheSameTo(true, Gemshine, PreciousBrilliance) || IsMoving))
                {
                    if (Swiftcast.CanUse(out act, mustUse: true)) return true;
                }
                break;
        }
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime <= 30 && SummonCarbuncle.CanUse(out _)) return SummonCarbuncle;
        //1.5s预读毁3
        if (remainTime <= Ruin.CastTime + Service.Configuration.CountDownAhead
            && Ruin.CanUse(out _)) return Ruin;
        return base.CountDownAction(remainTime);
    }
}
