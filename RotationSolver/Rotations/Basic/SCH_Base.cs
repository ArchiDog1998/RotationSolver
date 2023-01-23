using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;

namespace RotationSolver.Rotations.Basic;

internal abstract class SCH_Base : CustomRotation.CustomRotation
{
    private static SCHGauge JobGauge => Service.JobGauges.Get<SCHGauge>();

    /// <summary>
    /// 契约槽
    /// </summary>
    protected static byte FairyGauge => JobGauge.FairyGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Scholar };

    private sealed protected override IBaseAction Raise => Resurrection;
    /// <summary>
    /// 有豆子
    /// </summary>
    protected static bool HasAetherflow => JobGauge.Aetherflow > 0;
    /// <summary>
    /// 有大天使
    /// </summary>
    protected static bool HasSeraph => JobGauge.SeraphTimer > 0;

    #region 治疗
    /// <summary>
    /// 医术
    /// </summary>
    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, true, isTimeline: true);

    /// <summary>
    /// 鼓舞激励之策
    /// </summary>
    public static IBaseAction Adloquium { get; } = new BaseAction(ActionID.Adloquium, true, isTimeline: true)
    {
        ActionCheck = b => !b.HasStatus(false, StatusID.EukrasianDiagnosis,
            StatusID.EukrasianPrognosis,
            StatusID.Galvanize),
    };


    /// <summary>
    /// 复生
    /// </summary>
    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.Resurrection, true);

    /// <summary>
    /// 士气高扬之策
    /// </summary>
    public static IBaseAction Succor { get; } = new BaseAction(ActionID.Succor, true, isTimeline: true)
    {
        StatusProvide = new[] { StatusID.Galvanize },
    };

    /// <summary>
    /// 生命活性法
    /// </summary>
    public static IBaseAction Lustrate { get; } = new BaseAction(ActionID.Lustrate, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    /// <summary>
    /// 野战治疗阵
    /// </summary>
    public static IBaseAction SacredSoil { get; } = new BaseAction(ActionID.SacredSoil, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow && !IsMoving,
    };

    /// <summary>
    /// 不屈不挠之策
    /// </summary>
    public static IBaseAction Indomitability { get; } = new BaseAction(ActionID.Indomitability, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    /// <summary>
    /// 深谋远虑之策
    /// </summary>
    public static IBaseAction Excogitation { get; } = new BaseAction(ActionID.Excogitation, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    /// <summary>
    /// 慰藉
    /// </summary>
    public static IBaseAction Consolation { get; } = new BaseAction(ActionID.Consolation, true, isTimeline: true)
    {
        ActionCheck = b => HasSeraph,
    };

    /// <summary>
    /// 生命回生法
    /// </summary>
    public static IBaseAction Protraction { get; } = new BaseAction(ActionID.Protraction, true, isTimeline: true);
    #endregion
    #region 进攻
    /// <summary>
    /// 毒菌 猛毒菌 蛊毒法
    /// </summary>
    public static IBaseAction Bio { get; } = new BaseAction(ActionID.Bio, isEot: true)
    {
        TargetStatus = new StatusID[] { StatusID.Bio, StatusID.Bio2, StatusID.Biolysis },
    };

    /// <summary>
    /// 毁灭 气炎法 魔炎法 死炎法 极炎法
    /// </summary>
    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.Ruin);

    /// <summary>
    /// 毁坏
    /// </summary>
    public static IBaseAction Ruin2 { get; } = new BaseAction(ActionID.Ruin2);

    /// <summary>
    /// 能量吸收
    /// </summary>
    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrain)
    {
        ActionCheck = b => HasAetherflow
    };

    /// <summary>
    /// 破阵法
    /// </summary>
    public static IBaseAction ArtofWar { get; } = new BaseAction(ActionID.ArtofWar);//裂阵法 25866
    #endregion
    #region 仙女
    /// <summary>
    /// 炽天召唤
    /// </summary>
    public static IBaseAction SummonSeraph { get; } = new BaseAction(ActionID.SummonSeraph, true, isTimeline: true)
    {
        ActionCheck = b => TargetUpdater.HavePet,
    };

    /// <summary>
    /// 朝日召唤
    /// </summary>
    public static IBaseAction SummonEos { get; } = new BaseAction(ActionID.SummonEos)//夕月召唤 17216
    {
        ActionCheck = b => !TargetUpdater.HavePet && (!Player.HasStatus(true, StatusID.Dissipation) || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel),
    };

    /// <summary>
    /// 仙光的低语/天使的低语
    /// </summary>
    public static IBaseAction WhisperingDawn { get; } = new BaseAction(ActionID.WhisperingDawn, isTimeline: true)
    {
        ActionCheck = b => TargetUpdater.HavePet,
    };

    /// <summary>
    /// 异想的幻光/炽天的幻光
    /// </summary>
    public static IBaseAction FeyIllumination { get; } = new BaseAction(ActionID.FeyIllumination, isTimeline: true)
    {
        ActionCheck = b => TargetUpdater.HavePet,
    };

    /// <summary>
    /// 转化
    /// </summary>
    public static IBaseAction Dissipation { get; } = new BaseAction(ActionID.Dissipation)
    {
        StatusProvide = new[] { StatusID.Dissipation },
        ActionCheck = b => !HasAetherflow && !HasSeraph && InCombat && TargetUpdater.HavePet,
    };

    /// <summary>
    /// 以太契约-异想的融光
    /// </summary>
    public static IBaseAction Aetherpact { get; } = new BaseAction(ActionID.Aetherpact, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.FairyGauge >= 10 && TargetUpdater.HavePet && !HasSeraph
    };

    /// <summary>
    /// 异想的祥光
    /// </summary>
    public static IBaseAction FeyBlessing { get; } = new BaseAction(ActionID.FeyBlessing, isTimeline: true)
    {
        ActionCheck = b => !HasSeraph && TargetUpdater.HavePet,
    };
    #endregion
    #region 其他
    /// <summary>
    /// 以太超流
    /// </summary>
    public static IBaseAction Aetherflow { get; } = new BaseAction(ActionID.Aetherflow)
    {
        ActionCheck = b => InCombat && !HasAetherflow
    };

    /// <summary>
    /// 秘策
    /// </summary>
    public static IBaseAction Recitation { get; } = new BaseAction(ActionID.Recitation, isTimeline: true);

    /// <summary>
    /// 连环计
    /// </summary>
    public static IBaseAction ChainStratagem { get; } = new BaseAction(ActionID.ChainStratagem)
    {
        ActionCheck = b => InCombat && IsTargetBoss
    };

    /// <summary>
    /// 展开战术
    /// </summary>
    public static IBaseAction DeploymentTactics { get; } = new BaseAction(ActionID.DeploymentTactics, true, isTimeline: true)
    {
        ChoiceTarget = (friends, mustUse) =>
        {
            foreach (var friend in friends)
            {
                if (friend.HasStatus(true, StatusID.Galvanize)) return friend;
            }
            return null;
        },
    };

    /// <summary>
    /// 应急战术
    /// </summary>
    public static IBaseAction EmergencyTactics { get; } = new BaseAction(ActionID.EmergencyTactics, isTimeline: true);

    /// <summary>
    /// 疾风怒涛之计
    /// </summary>
    public static IBaseAction Expedient { get; } = new BaseAction(ActionID.Expedient, isTimeline: true);
    #endregion
}