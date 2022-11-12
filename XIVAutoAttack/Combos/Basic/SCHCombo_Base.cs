using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SCHCombo_Base<TCmd> : JobGaugeCombo<SCHGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Scholar };

    private sealed protected override BaseAction Raise => Resurrection;
    protected static bool HasAetherflow => JobGauge.Aetherflow > 0;
    protected static bool HasSeraph => JobGauge.SeraphTimer > 0;

    public static readonly BaseAction
    #region 治疗
        //医术
        Physick = new(190, true),

        //鼓舞激励之策
        Adloquium = new(185, true)
        {
            TargetStatus = new StatusID[]
            {
                    StatusID.EukrasianDiagnosis,
                    StatusID.EukrasianPrognosis,
                    StatusID.Galvanize,
            },
        },


        //复生
        Resurrection = new(173, true),

        //士气高扬之策
        Succor = new(186, true)
        {
            BuffsProvide = new[] { StatusID.Galvanize },
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
        Bio = new(17864, isEot: true)//猛毒菌 17865 蛊毒法 16540
        {
            TargetStatus = new StatusID[] { StatusID.Bio, StatusID.Bio2, StatusID.Biolysis },
        },

        //毁灭
        Ruin = new(17869),//气炎法 3584 魔炎法 7435 死炎法 16541 极炎法 25865

        //毁坏
        Ruin2 = new(17870),

        //能量吸收
        EnergyDrain = new(167)
        {
            OtherCheck = b => HasAetherflow && (Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3) || Aetherflow.WillHaveOneChargeGCD(3))
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
            OtherCheck = b => !TargetUpdater.HavePet && (!Player.HaveStatus(StatusID.Dissipation) || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel),
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
            OtherCheck = b => !HasAetherflow && !HasSeraph && InCombat && TargetUpdater.HavePet,
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
        DeploymentTactics = new(ActionIDs.DeploymentTactics, true)
        {
            ChoiceTarget = friends =>
            {
                foreach (var friend in friends)
                {
                    if (friend.HaveStatus(StatusID.Galvanize)) return friend;
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