using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Common.Lua;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.RangedMagicial;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer;

internal sealed class SCHCombo : JobGaugeCombo<SCHGauge>
{
    internal override uint JobID => 28;

    private protected override BaseAction Raise => SMNCombo.Actions.Resurrection;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    private static bool HasAetherflow => JobGauge.Aetherflow > 0;
    private static bool HasSeraph => JobGauge.SeraphTimer > 0;
    internal struct Actions
    {
        public static readonly BaseAction
        #region 治疗
            //医术
            Physick = new(190, true),

            //鼓舞激励之策
            Adloquium = new(185, true)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.EukrasianDiagnosis,
                    ObjectStatus.EukrasianPrognosis,
                    ObjectStatus.Galvanize,
                },
            },

            //士气高扬之策
            Succor = new(186, true)
            {
                BuffsProvide = new[] { ObjectStatus.Galvanize },
            },

            //生命活性法
            Lustrate = new(189, true)
            {
                OtherCheck = b => HasAetherflow
            },

            //野战治疗阵
            SacredSoil = new(188, true)
            {
                OtherCheck = b => HasAetherflow && !IsMoving,
            },

            //不屈不挠之策
            Indomitability = new(3583, true)
            {
                OtherCheck = b => HasAetherflow
            },

            //深谋远虑之策
            Excogitation = new(7434, true)
            {
                OtherCheck = b => HasAetherflow
            },

            //慰藉
            Consolation = new(16546)
            {
                OtherCheck = b => HasSeraph,
            },

            //生命回生法
            Protraction = new(25867, true),
        #endregion
        #region 进攻
            //毒菌
            Bio = new(17864, isDot: true)//猛毒菌 17865 蛊毒法 16540
            {
                TargetStatus = new ushort[] { ObjectStatus.Bio, ObjectStatus.Bio2, ObjectStatus.Biolysis },
            },

            //毁灭
            Ruin = new(17869),//气炎法 3584 魔炎法 7435 死炎法 16541 极炎法 25865

            //毁坏
            Ruin2 = new(17870),

            //能量吸收
            EnergyDrain = new(167)
            {
                OtherCheck = b => HasAetherflow && ((Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3)) || Aetherflow.WillHaveOneChargeGCD(3))
            },

            //破阵法
            ArtofWar = new(16539),//裂阵法 25866
        #endregion
        #region 仙女
            //炽天召唤
            SummonSeraph = new(16545)
            {
                OtherCheck = b => TargetUpdater.HavePet,
            },

            //朝日召唤
            SummonEos = new(17215)//夕月召唤 17216
            {
                OtherCheck = b => !TargetUpdater.HavePet && (!Player.HaveStatus(ObjectStatus.Dissipation) || (Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel)),
            },

            //仙光的低语/天使的低语
            WhisperingDawn = new(16537)
            {
                OtherCheck = b => TargetUpdater.HavePet,
            },

            //异想的幻光/炽天的幻光
            FeyIllumination = new(16538)
            {
                OtherCheck = b => TargetUpdater.HavePet,
            },

            //转化
            Dissipation = new(3587)
            {
                OtherCheck = b => !HasAetherflow && InCombat && TargetUpdater.HavePet,
            },

            //以太契约-异想的融光
            Aetherpact = new(7437, true)
            {
                OtherCheck = b => JobGauge.FairyGauge >= 10 && TargetUpdater.HavePet && !HasSeraph
            },

            //异想的祥光
            FeyBlessing = new(16543)
            {
                OtherCheck = b => !HasSeraph && TargetUpdater.HavePet,
            },
        #endregion
        #region 其他
            //以太超流
            Aetherflow = new(166)
            {
                OtherCheck = b => InCombat && !HasAetherflow
            },

            //秘策
            Recitation = new(16542),

            //连环计
            ChainStratagem = new(7436)
            {
                OtherCheck = b => InCombat && IsTargetBoss
            },

            //展开战术
            DeploymentTactics = new(3585, true)
            {
                ChoiceTarget = friends =>
                {
                    foreach (var friend in friends)
                    {
                        if (friend.HaveStatus(ObjectStatus.Galvanize)) return friend;
                    }
                    return null;
                },
            },

            //应急战术
            EmergencyTactics = new(3586),

            //疾风怒涛之计
            Expedient = new(25868);
        #endregion
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }
    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.范围治疗, $"GCD: {Actions.Succor}\n                     能力: {Actions.SacredSoil}, {Actions.SummonSeraph}, {Actions.WhisperingDawn}, {Actions.FeyBlessing}, {Actions.Indomitability}"},
        {DescType.单体治疗, $"GCD: {Actions.Adloquium}, {Actions.Physick}\n                     能力: {Actions.SacredSoil}, {Actions.Aetherpact}, {Actions.Protraction}, {Actions.Excogitation}, {Actions.Lustrate}"},
        {DescType.范围防御, $"GCD: {Actions.Succor}\n                     能力: {Actions.SacredSoil}, {Actions.Adloquium}, {Actions.SummonSeraph}, {Actions.FeyIllumination}, {Actions.Expedient}"},
        {DescType.单体防御, $"{Actions.Adloquium}"},
    };

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //秘策绑定单盾群盾
        if (nextGCD.IsAnySameAction(true, Actions.Succor, Actions.Adloquium))
        {
            if (Actions.Recitation.ShouldUse(out act)) return true;
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

        //DoT
        if (Actions.Bio.ShouldUse(out act)) return true;


        //AOE
        if (Actions.ArtofWar.ShouldUse(out act)) return true;

        //单体
        if (Actions.Ruin.ShouldUse(out act)) return true;
        if (Actions.Ruin2.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //鼓舞激励之策
        if (Actions.Adloquium.ShouldUse(out act)) return true;

        //医术
        if (Actions.Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //以太契约
        if (Actions.Aetherpact.ShouldUse(out act) && JobGauge.FairyGauge >= 70) return true;

        //生命回生法
        if (Actions.Protraction.ShouldUse(out act)) return true;

        //野战治疗阵
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        //深谋远虑之策
        if (Actions.Excogitation.ShouldUse(out act)) return true;

        //生命活性法
        if (Actions.Lustrate.ShouldUse(out act)) return true;

        //以太契约
        if (Actions.Aetherpact.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        //深谋远虑之策
        if (Actions.Excogitation.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //士气高扬之策
        if (Actions.Succor.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //慰藉
        if (Actions.SummonSeraph.ShouldUse(out act)) return true;
        if (Actions.Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //异想的祥光
        if (Actions.FeyBlessing.ShouldUse(out act)) return true;

        //仙光的低语
        if (Actions.WhisperingDawn.ShouldUse(out act)) return true;

        //野战治疗阵
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        //不屈不挠之策
        if (Actions.Indomitability.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //士气高扬之策
        if (Actions.Succor.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //异想的幻光
        if (Actions.FeyIllumination.ShouldUse(out act)) return true;

        //疾风怒涛之计
        if (Actions.Expedient.ShouldUse(out act)) return true;

        //慰藉
        if (Actions.SummonSeraph.ShouldUse(out act)) return true;
        if (Actions.Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //野战治疗阵
        if (Actions.SacredSoil.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //连环计
            if (Actions.ChainStratagem.ShouldUse(out act)) return true;
        }

        //能量吸收
        if (Actions.EnergyDrain.ShouldUse(out act)) return true;

        //转化
        if (Actions.Dissipation.ShouldUse(out act)) return true;

        //以太超流
        if (Actions.Aetherflow.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}