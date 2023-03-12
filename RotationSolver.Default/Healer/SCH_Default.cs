using RotationSolver.Actions;
using RotationSolver.Attributes;
using RotationSolver.Basic;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;

namespace RotationSolver.Default.Healer;

[RotationDesc(ActionID.ChainStratagem)]
internal sealed class SCH_Default : SCH_Base
{
    public override string GameVersion => "6.28";

    public override string RotationName => "Default";

    public SCH_Default()
    {
        SummonSeraph.RotationCheck = b => WhisperingDawn.ElapsedAfterGCD(1) || FeyIllumination.ElapsedAfterGCD(1) || FeyBlessing.ElapsedAfterGCD(1);
    }
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "Aut use GCD to heal")
                                            .SetBool("prevDUN", false, "Recitation in 15 seconds.")
                                            .SetBool("GiveT", false, "Give Recitation to Tank");
    }

    protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //秘策绑定单盾群盾
        if (nextGCD.IsTheSameTo(true, Succor, Adloquium))
        {
            if (Recitation.CanUse(out act)) return true;
        }

        //Remove Aetherpact
        foreach (var item in PartyMembers)
        {
            if (item.GetHealthRatio() < 0.9) continue;
            if (item.HasStatus(true, StatusID.Aetherpact))
            {
                act = Aetherpact;
                return true;
            }
        }

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //召唤小仙女
        if (SummonEos.CanUse(out act)) return true;

        //DoT
        if (Bio.CanUse(out act)) return true;

        //AOE
        if (ArtofWar.CanUse(out act)) return true;

        //Single
        if (Ruin.CanUse(out act)) return true;
        if (Ruin2.CanUse(out act)) return true;

        //Add dot.
        if (Bio.CanUse(out act, true)) return true;

        return false;
    }

    [RotationDesc(ActionID.Adloquium, ActionID.Physick)]
    protected override bool HealSingleGCD(out IAction act)
    {
        //鼓舞激励之策
        if (Adloquium.CanUse(out act)) return true;

        //医术
        if (Physick.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Aetherpact, ActionID.Protraction, ActionID.SacredSoil, ActionID.Excogitation, ActionID.Lustrate, ActionID.Aetherpact)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //判断是否有人有线
        var haveLink = PartyMembers.Any(p => p.HasStatus(true, StatusID.Aetherpact));

        //以太契约
        if (Aetherpact.CanUse(out act) && FairyGauge >= 70 && !haveLink) return true;

        //生命回生法
        if (Protraction.CanUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.CanUse(out act)) return true;

        //深谋远虑之策
        if (Excogitation.CanUse(out act)) return true;

        //生命活性法
        if (Lustrate.CanUse(out act)) return true;

        //以太契约
        if (Aetherpact.CanUse(out act) && !haveLink) return true;

        return false;
    }

    [RotationDesc(ActionID.Excogitation)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Excogitation.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Succor)]
    protected override bool HealAreaGCD(out IAction act)
    {
        //士气高扬之策
        if (Succor.CanUse(out act)) return true;

        return false;
    }


    [RotationDesc(ActionID.SummonSeraph, ActionID.Consolation, ActionID.WhisperingDawn, ActionID.SacredSoil, ActionID.Indomitability)]
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //慰藉
        if (SummonSeraph.CanUse(out act)) return true;
        if (Consolation.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //异想的祥光
        if (FeyBlessing.CanUse(out act)) return true;

        //仙光的低语
        if (WhisperingDawn.CanUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.CanUse(out act)) return true;

        //不屈不挠之策
        if (Indomitability.CanUse(out act)) return true;

        act = null;
        return false;
    }

    [RotationDesc(ActionID.Succor)]
    protected override bool DefenseAreaGCD(out IAction act)
    {
        if (Succor.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.FeyIllumination, ActionID.Expedient, ActionID.SummonSeraph, ActionID.Consolation, ActionID.SacredSoil)]
    protected override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //异想的幻光
        if (FeyIllumination.CanUse(out act)) return true;

        //疾风怒涛之计
        if (Expedient.CanUse(out act)) return true;

        //慰藉
        if (SummonSeraph.CanUse(out act)) return true;
        if (Consolation.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //野战治疗阵
        if (SacredSoil.CanUse(out act)) return true;

        return false;
    }


    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        if (InBurst)
        {
            //连环计
            if (ChainStratagem.CanUse(out act)) return true;
        }

        if (Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3) && Dissipation.IsEnabled || Aetherflow.WillHaveOneChargeGCD(3))
        {
            //能量吸收
            if (EnergyDrain.CanUse(out act, emptyOrSkipCombo: true)) return true;
        }

        //转化
        if (Dissipation.CanUse(out act)) return true;

        //以太超流
        if (Aetherflow.CanUse(out act)) return true;

        act = null;
        return false;
    }

    //15秒秘策单盾扩散
    protected override IAction CountDownAction(float remainTime)
    {
        if (remainTime < Ruin.CastTime + Service.Config.CountDownAhead
            && Ruin.CanUse(out var act)) return act;

        if (Configs.GetBool("prevDUN") && remainTime <= 15 && !DeploymentTactics.IsCoolingDown && PartyMembers.Count() > 1)
        {

            if (!Recitation.IsCoolingDown) return Recitation;
            if (!PartyMembers.Any((n) => n.HasStatus(true, StatusID.Galvanize)))
            {
                //如果还没上激励就给t一个激励
                if (Configs.GetBool("GiveT"))
                {
                    return Adloquium;
                }
            }
            else
            {
                return DeploymentTactics;
            }
        }
        return base.CountDownAction(remainTime);
    }
}