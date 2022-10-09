using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.RangedMagicial;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Healer;

internal class SCHCombo : JobGaugeCombo<SCHGauge>
{
    internal override uint JobID => 28;

    private static bool _useDeploymentTactics = false;
    private protected override BaseAction Raise => SMNCombo.Actions.Resurrection;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
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
                OtherCheck = b => !TargetHelper.HavePet,
            },

            //毒菌
            Bio = new (17864, true)
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
            WhisperingDawn = new (16537)
            {
                //OtherCheck = b => JobGauge.SeraphTimer == 0,
            },


            //异想的幻光
            FeyIllumination = new (16538)
            {
                //OtherCheck = b => JobGauge.SeraphTimer == 0,
            },


            //异想的祥光
            FeyBlessing = new (16543)
            {
                OtherCheck = b => JobGauge.SeraphTimer == 0,
            },

            //以太超流
            Aetherflow = new (166)
            {
                OtherCheck = b => TargetHelper.InBattle,
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
            SacredSoil = new (188, true),

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
                ChoiceFriend = friends =>
                {
                    foreach (var friend in friends)
                    {
                        var times = StatusHelper.FindStatusFromSelf(friend, ObjectStatus.Galvanize);
                        if (times != null && times.Length > 0) return friend;
                    }
                    return null;
                },
                AfterUse = () => _useDeploymentTactics = false,
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
        if (Actions.Expedient.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ChainStratagem.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.ID == Actions.Adloquium.ID ||
            nextGCD.ID == Actions.Succor.ID ||
            nextGCD.ID == Actions.Indomitability.ID ||
            nextGCD.ID == Actions.Excogitation.ID)
        {
            if (Actions.Recitation.ShouldUseAction(out act)) return true;
        }

        foreach (var item in TargetHelper.PartyMembers)
        {
            if ((float)item.CurrentHp / item.MaxHp < 0.9) continue;
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
        if (Actions.SummonEos.ShouldUseAction(out act)) return true;

        if (_useDeploymentTactics && Actions.DeploymentTactics.ShouldUseAction(out act)) return true;

        //AOE
        if (Actions.ArtofWar.ShouldUseAction(out act)) return true;
        //单体
        if (Actions.Bio.ShouldUseAction(out act)) return true;
        if (Actions.Broil.ShouldUseAction(out act)) return true;
        if (Actions.Ruin2.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        if (Service.ClientState.LocalPlayer.Level < Actions.Adloquium.Level && Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Succor.ShouldUseAction(out act)) return true;
        return false;

    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {

        if (!Actions.DeploymentTactics.IsCoolDown && Service.ClientState.LocalPlayer.Level >= Actions.DeploymentTactics.Level)
        {
            _useDeploymentTactics = true;
            if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        }
        if (Actions.Succor.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Aetherflow > 0 && !IsMoving)
        {
            if (Actions.SacredSoil.ShouldUseAction(out act)) return true;
        }
        if (Actions.SummonSeraph.ShouldUseAction(out act)) return true;
        if (Actions.FeyIllumination.ShouldUseAction(out act)) return true;


        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Aetherflow > 0 && !IsMoving)
        {
            if (Actions.SacredSoil.ShouldUseAction(out act)) return true;
        }

        if (abilityRemain == 1)
        {
            if (Actions.Consolation.ShouldUseAction(out act)) return true;
            if (Actions.SummonSeraph.ShouldUseAction(out act)) return true;
            if (Actions.WhisperingDawn.ShouldUseAction(out act)) return true;
            if (Actions.FeyBlessing.ShouldUseAction(out act)) return true;

            if (JobGauge.Aetherflow > 0)
            {
                if (Actions.Indomitability.ShouldUseAction(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aetherpact.ShouldUseAction(out act) && JobGauge.FairyGauge >= 70) return true;

        if (Actions.Protraction.ShouldUseAction(out act)) return true;

        if (JobGauge.Aetherflow > 0)
        {
            if (Actions.Excogitation.ShouldUseAction(out act)) return true;
            if (Actions.Lustrate.ShouldUseAction(out act)) return true;
        }
        if (Actions.Aetherpact.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Aetherflow == 0)
        {
            if (Actions.Aetherflow.ShouldUseAction(out act)) return true;
        }
        else if (Actions.Aetherflow.RecastTimeRemain < 6)
        {
            if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }
}
