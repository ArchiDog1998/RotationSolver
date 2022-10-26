using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.RangedMagicial;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer;

internal sealed class SCHCombo : JobGaugeCombo<SCHGauge>
{
    internal override uint JobID => 28;

    private protected override BaseAction Raise => SMNCombo.Actions.Resurrection;
    protected override bool CanHealSingleSpell => base.CanHealSingleAbility && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaAbility  && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealSingleAbility => base.CanHealSingleSpell;

    protected override bool CanHealAreaAbility => base.CanHealAreaSpell;
    internal struct Actions
    {
        public static readonly BaseAction
            //毁灭
            Ruin = new (17869),

            //毁坏
            Ruin2 = new (17870),

            //毁坏
            Broil = new (3584),

            //朝日召唤
            SummonEos = new (17215)
            {
                OtherCheck = b => !TargetUpdater.HavePet,
            },

            //毒菌
            Bio = new (17864, isDot: true)
            {
                TargetStatus = new ushort[] { ObjectStatus.Bio, ObjectStatus.Bio2, ObjectStatus.Biolysis },
            },

            //医术
            Physick = new (190, true),

            //破阵法
            ArtofWar = new (16539),

            //鼓舞激励之策
            Adloquium = new (185, true)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.EukrasianDiagnosis,
                    ObjectStatus.EukrasianPrognosis,
                    ObjectStatus.Galvanize,
                },
            },

            //士气高扬之策
            Succor = new (186, true)
            {
                BuffsProvide = new [] { ObjectStatus.Galvanize },
            },

            //仙光的低语
            WhisperingDawn = new (16537),


            //异想的幻光
            FeyIllumination = new (16538),


            //异想的祥光
            FeyBlessing = new (16543)
            {
                OtherCheck = b => JobGauge.SeraphTimer == 0,
            },

            //以太超流
            Aetherflow = new (166)
            {
                OtherCheck = b => InBattle,
            },

            //能量吸收
            EnergyDrain = new (167),

            //生命活性法
            Lustrate = new (189, true),

            //深谋远虑之策
            Excogitation = new (7434, true),

            //不屈不挠之策
            Indomitability = new (3583, true),

            //野战治疗阵
            SacredSoil = new (188, true)
            {
                OtherCheck = b => JobGauge.Aetherflow > 0 && !IsMoving,
            },

            //以太契约
            Aetherpact = new (7437, true)
            {
                OtherCheck = b => JobGauge.FairyGauge >= 10 && JobGauge.SeraphTimer == 0,
            },

            //秘策
            Recitation = new (16542),

            //连环计
            ChainStratagem = new (7436),

            //展开战术
            DeploymentTactics = new (3585, true)
            {
                ChoiceTarget = friends =>
                {
                    foreach (var friend in friends)
                    {
                        if( friend.HaveStatus(ObjectStatus.Galvanize)) return friend;
                    }
                    return null;
                },
            },

            //炽天召唤
            SummonSeraph = new (16545),

            //慰藉
            Consolation = new (16546)
            {
                OtherCheck = b => JobGauge.SeraphTimer > 0,
            },

            //生命回生法
            Protraction = new (25867),

            //疾风怒涛之计
            Expedient = new (25868);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.范围治疗, $"GCD: {Actions.Succor.Action.Name}\n                     能力: {Actions.SacredSoil.Action.Name}, {Actions.SummonSeraph.Action.Name}, {Actions.WhisperingDawn.Action.Name}, {Actions.FeyBlessing.Action.Name}, {Actions.Indomitability.Action.Name}"},
        {DescType.单体治疗, $"GCD: {Actions.Adloquium.Action.Name}, {Actions.Physick.Action.Name}\n                     能力: {Actions.Aetherpact.Action.Name}, {Actions.Protraction.Action.Name}, {Actions.Excogitation.Action.Name}, {Actions.Lustrate.Action.Name}"},
        {DescType.范围防御, $"GCD: {Actions.Succor.Action.Name}, {Actions.SacredSoil.Action.Name}\n                     能力: {Actions.DeploymentTactics.Action.Name}, {Actions.Adloquium.Action.Name}, {Actions.SummonSeraph.Action.Name}, {Actions.FeyIllumination.Action.Name}"},
        {DescType.单体防御, $"{Actions.Adloquium.Action.Name}"},
        {DescType.移动, $"{Actions.Expedient.Action.Name}"},
    };
    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Expedient.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ChainStratagem.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsAnySameAction(true, Actions.Adloquium, Actions.Succor, 
            Actions.Indomitability, Actions.Excogitation))
        {
            if (Actions.Recitation.ShouldUse(out act)) return true;
        }

        foreach (var item in TargetUpdater.PartyMembers)
        {
            if (item.GetHealthRatio() < 0.9) continue;
            foreach (var status in item.StatusList)
            {
                if(status.StatusId == 1223 && status.SourceObject != null 
                    && status.SourceObject.OwnerId == Service.ClientState.LocalPlayer.ObjectId)
                {
                    act = Actions.Aetherpact;
                    return true;
                }
            }
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //召唤小仙女
        if (Actions.SummonEos.ShouldUse(out act)) return true;


        //AOE
        if (Actions.ArtofWar.ShouldUse(out act)) return true;
        //单体
        if (Actions.Bio.ShouldUse(out act)) return true;
        if (Actions.Broil.ShouldUse(out act)) return true;
        if (Actions.Ruin2.ShouldUse(out act)) return true;
        if (Actions.Ruin.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Adloquium.ShouldUse(out act)) return true;
        if (!Actions.Adloquium.EnoughLevel && Actions.Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        if (Actions.Adloquium.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Succor.ShouldUse(out act)) return true;
        return false;

    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {

        if (!Actions.DeploymentTactics.IsCoolDown && Actions.DeploymentTactics.EnoughLevel)
        {
            if (Actions.DeploymentTactics.ShouldUse(out act)) return true;
            if (Actions.Adloquium.ShouldUse(out act)) return true;
        }
        if (Actions.Succor.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        if (Actions.SummonSeraph.ShouldUse(out act)) return true;
        if (Actions.FeyIllumination.ShouldUse(out act)) return true;


        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        if (abilityRemain == 1)
        {
            if (Actions.Consolation.ShouldUse(out act)) return true;
            if (Actions.SummonSeraph.ShouldUse(out act)) return true;
            if (Actions.WhisperingDawn.ShouldUse(out act)) return true;
            if (Actions.FeyBlessing.ShouldUse(out act)) return true;

            if (JobGauge.Aetherflow > 0)
            {
                if (Actions.Indomitability.ShouldUse(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        if (Actions.Aetherpact.ShouldUse(out act) && JobGauge.FairyGauge >= 70) return true;

        if (Actions.Protraction.ShouldUse(out act)) return true;

        if (JobGauge.Aetherflow > 0)
        {
            if (Actions.Excogitation.ShouldUse(out act)) return true;
            if (Actions.Lustrate.ShouldUse(out act)) return true;
        }
        if (Actions.Aetherpact.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Aetherflow == 0)
        {
            if (Actions.Aetherflow.ShouldUse(out act)) return true;
        }
        else if (Actions.Aetherflow.WillHaveOneChargeGCD(3))
        {
            if (Actions.EnergyDrain.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }
}
