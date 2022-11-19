using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SCHCombos.SCHCombo_Default;

namespace XIVAutoAttack.Combos.Healer.SCHCombos;

internal sealed class SCHCombo_Default : SCHCombo_Base<CommandType>
{
    public override string Author => "逆光 & Ana Amari";

    internal enum CommandType : byte
    {
        None,
    }
    public SCHCombo_Default()
    {
        //防止大仙女吞技能
        SummonSeraph.ComboCheck = b => WhisperingDawn.ElapsedAfterGCD(1) || FeyIllumination.ElapsedAfterGCD(1) || FeyBlessing.ElapsedAfterGCD(1);
    }
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围治疗, $"GCD: {Succor}\n                     能力: {SacredSoil}, {SummonSeraph}, {WhisperingDawn}, {FeyBlessing}, {Indomitability}"},
        {DescType.单体治疗, $"GCD: {Adloquium}, {Physick}\n                     能力: {SacredSoil}, {Aetherpact}, {Protraction}, {Excogitation}, {Lustrate}"},
        {DescType.范围防御, $"GCD: {Succor}\n                     能力: {SacredSoil}, {Adloquium}, {SummonSeraph}, {FeyIllumination}, {Expedient}"},
        {DescType.单体防御, $"{Adloquium}"},
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //秘策绑定单盾群盾
        if (nextGCD.IsAnySameAction(true, Succor, Adloquium))
        {
            if (Recitation.ShouldUse(out act)) return true;
        }

        //以太契约
        foreach (var item in TargetUpdater.PartyMembers)
        {
            if (item.GetHealthRatio() < 0.9) continue;
            foreach (var status in item.StatusList)
            {
                if (status.StatusId == 1223 && status.SourceObject != null
                    && status.SourceObject.OwnerId == Service.ClientState.LocalPlayer.ObjectId)
                {
                    act = Aetherpact;
                    return true;
                }
            }
        }

        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //召唤小仙女
        if (SummonEos.ShouldUse(out act)) return true;

        //DoT
        if (Bio.ShouldUse(out act)) return true;


        //AOE
        if (ArtofWar.ShouldUse(out act)) return true;

        //单体
        if (Ruin.ShouldUse(out act)) return true;
        if (Ruin2.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //鼓舞激励之策
        if (Adloquium.ShouldUse(out act)) return true;

        //医术
        if (Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //判断是否有人有线
        var haveLink = TargetUpdater.PartyMembers.Any(p =>
        p.StatusList.Any(
            status => status.StatusId == 1223 && status.SourceObject != null
            && status.SourceObject.OwnerId == Service.ClientState.LocalPlayer.ObjectId)
        );
        //以太契约
        if (Aetherpact.ShouldUse(out act) && FairyGauge >= 70 && !haveLink) return true;

        //生命回生法
        if (Protraction.ShouldUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        //深谋远虑之策
        if (Excogitation.ShouldUse(out act)) return true;

        //生命活性法
        if (Lustrate.ShouldUse(out act)) return true;

        //以太契约
        if (Aetherpact.ShouldUse(out act) && !haveLink) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        //深谋远虑之策
        if (Excogitation.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        //士气高扬之策
        if (Succor.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //慰藉
        if (SummonSeraph.ShouldUse(out act)) return true;
        if (Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //异想的祥光
        if (FeyBlessing.ShouldUse(out act)) return true;

        //仙光的低语
        if (WhisperingDawn.ShouldUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        //不屈不挠之策
        if (Indomitability.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //士气高扬之策
        if (Succor.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //异想的幻光
        if (FeyIllumination.ShouldUse(out act)) return true;

        //疾风怒涛之计
        if (Expedient.ShouldUse(out act)) return true;

        //慰藉
        if (SummonSeraph.ShouldUse(out act)) return true;
        if (Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //连环计
            if (ChainStratagem.ShouldUse(out act)) return true;
        }

        if ((Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3) && Dissipation.IsEnabled) || Aetherflow.WillHaveOneChargeGCD(3))
        {
            //能量吸收
            if (EnergyDrain.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        //转化
        if (Dissipation.ShouldUse(out act)) return true;

        //以太超流
        if (Aetherflow.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}