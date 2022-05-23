using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class SCHCombo : CustomComboJob<SCHGauge>
{
    internal override uint JobID => 28;
    private protected override BaseAction Raise => SMNCombo.Actions.Resurrection;
    internal struct Actions
    {
        public static readonly BaseAction
            //毁灭
            Ruin = new BaseAction(17869),

            //毁坏
            Ruin2 = new BaseAction(17870),

            //朝日召唤
            SummonEos = new BaseAction(17215)
            {
            },

            //毒菌
            Bio = new BaseAction(17864, true)
            {
                TargetStatus = new ushort[] { ObjectStatus.Bio, ObjectStatus.Bio2, ObjectStatus.Biolysis },
            },

            //医术
            Physick = new BaseAction(190, true),

            //破阵法
            ArtofWar = new BaseAction(16539),

            //鼓舞激励之策
            Adloquium = new BaseAction(185, true)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.EukrasianDiagnosis,
                    ObjectStatus.EukrasianPrognosis,
                    ObjectStatus.Galvanize,
                },
            },

            //士气高扬之策
            Succor = new BaseAction(186, true),

            //仙光的低语
            WhisperingDawn = new BaseAction(16537),

            //异想的幻光
            FeyIllumination = new BaseAction(16538),

            //异想的祥光
            FeyBlessing = new BaseAction(16543),

            //以太超流
            Aetherflow = new BaseAction(166),

            //能量吸收
            EnergyDrain = new BaseAction(167),

            //生命活性法
            Lustrate = new BaseAction(189, true),

            //深谋远虑之策
            Excogitation = new BaseAction(7434, true),

            //不屈不挠之策
            Indomitability = new BaseAction(3583, true),

            //野战治疗阵
            SacredSoil = new BaseAction(188, true),

            //以太契约
            Aetherpact = new BaseAction(7437, true)
            {
                OtherCheck = b => JobGauge.FairyGauge >= 10,
            },

            //秘策
            Recitation = new BaseAction(16542),

            //连环计
            ChainStratagem = new BaseAction(7436),

            //展开战术
            DeploymentTactics = new BaseAction(3585)
            {
                ChoiceFriend = friends =>
                {
                    foreach (var friend in friends)
                    {
                        var times = BaseAction.FindStatusFromSelf(friend, ObjectStatus.Galvanize);
                        if (times != null && times.Length > 0) return friend;
                    }
                    return null;
                },
            },

            //炽天召唤
            SummonSeraph = new BaseAction(16545),

            //慰藉
            Consolation = new BaseAction(16546),

            //生命回生法
            Protraction = new BaseAction(25867),

            //疾风怒涛之计
            Expedient = new BaseAction(25868);
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Expedient.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ChainStratagem.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if(nextGCD.ActionID == Actions.Adloquium.ActionID ||
            nextGCD.ActionID == Actions.Succor.ActionID ||
            nextGCD.ActionID == Actions.Indomitability.ActionID ||
            nextGCD.ActionID == Actions.Excogitation.ActionID)
        {
            if (Actions.Recitation.ShouldUseAction(out act)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //召唤小仙女
        //if (Actions.SummonEos.ShouldUseAction(out act)) return true;

        //AOE
        if (Actions.ArtofWar.ShouldUseAction(out act)) return true;
        //单体
        if (Actions.Bio.ShouldUseAction(out act)) return true;
        if (Actions.Ruin2.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.Succor.ShouldUseAction(out act)) return true;
        if (JobGauge.Aetherflow > 0)
        {
            if (Actions.SacredSoil.ShouldUseAction(out act)) return true;
        }


        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (!Actions.DeploymentTactics.IsCoolDown)
        {
            if (Actions.DeploymentTactics.ShouldUseAction(out act)) return true;
            if (Actions.Adloquium.ShouldUseAction(out act)) return true;
        }
        if (Actions.FeyIllumination.ShouldUseAction(out act)) return true;


        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if(abilityRemain == 1)
        {
            if (Actions.WhisperingDawn.ShouldUseAction(out act)) return true;
            if (Actions.FeyBlessing.ShouldUseAction(out act)) return true;

            if (JobGauge.Aetherflow > 0)
            {
                if (Actions.SacredSoil.ShouldUseAction(out act)) return true;
                if (Actions.Indomitability.ShouldUseAction(out act)) return true;
            }
        }

        act = null;
        return false;
    }


    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Protraction.ShouldUseAction(out act)) return true;

        if (JobGauge.Aetherflow > 0)
        {
            if (Actions.Excogitation.ShouldUseAction(out act)) return true;
            if (Actions.Lustrate.ShouldUseAction(out act)) return true;
        }
        if (Actions.Aetherpact.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if(JobGauge.Aetherflow == 0)
        {
            if (Actions.Aetherflow.ShouldUseAction(out act)) return true;
        }
        else if (!Actions.Aetherflow.IsCoolDown)
        {
            if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }
}
