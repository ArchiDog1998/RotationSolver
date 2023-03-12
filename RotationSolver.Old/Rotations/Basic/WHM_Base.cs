using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

internal abstract class WHM_Base : CustomRotation.CustomRotation
{
    private static WHMGauge JobGauge => Service.JobGauges.Get<WHMGauge>();
    public override MedicineType MedicineType => MedicineType.Mind;


    /// <summary>
    /// 百合花的数量
    /// </summary>
    protected static byte Lily => JobGauge.Lily;

    /// <summary>
    /// 血百合花的数量
    /// </summary>
    protected static byte BloodLily => JobGauge.BloodLily;

    /// <summary>
    /// 百合花多久后
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool LilyAfter(float time)
    {
        return EndAfter(JobGauge.LilyTimer / 1000f, time);
    }

    /// <summary>
    /// 这首歌啊在多久后还在唱嘛
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool LilyAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.LilyTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.WhiteMage, ClassJobID.Conjurer };
    private sealed protected override IBaseAction Raise => Raise1;

    #region 治疗
    /// <summary>
    /// 治疗
    /// </summary>
    public static IBaseAction Cure { get; } = new BaseAction(ActionID.Cure, true, isTimeline: true);

    /// <summary>
    /// 医治
    /// </summary>
    public static IBaseAction Medica { get; } = new BaseAction(ActionID.Medica, true, isTimeline: true);

    /// <summary>
    /// 复活
    /// </summary>
    public static IBaseAction Raise1 { get; } = new BaseAction(ActionID.Raise1, true);

    /// <summary>
    /// 救疗
    /// </summary>
    public static IBaseAction Cure2 { get; } = new BaseAction(ActionID.Cure2, true, isTimeline: true);

    /// <summary>
    /// 医济
    /// </summary>
    public static IBaseAction Medica2 { get; } = new BaseAction(ActionID.Medica2, true, isEot: true, isTimeline: true)
    {
        StatusProvide = new[] { StatusID.Medica2, StatusID.TrueMedica2 },
    };

    /// <summary>
    /// 再生
    /// </summary>
    public static IBaseAction Regen { get; } = new BaseAction(ActionID.Regen, true, isEot: true, isTimeline: true)
    {
        TargetStatus = new[]
        {
            StatusID.Regen1,
            StatusID.Regen2,
            StatusID.Regen3,
        }
    };

    /// <summary>
    /// 愈疗
    /// </summary>
    public static IBaseAction Cure3 { get; } = new BaseAction(ActionID.Cure3, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// 天赐祝福
    /// </summary>
    public static IBaseAction Benediction { get; } = new BaseAction(ActionID.Benediction, true, isTimeline: true);

    /// <summary>
    /// 庇护所
    /// </summary>
    public static IBaseAction Asylum { get; } = new BaseAction(ActionID.Asylum, true, isTimeline: true);

    /// <summary>
    /// 安慰之心
    /// </summary>
    public static IBaseAction AfflatusSolace { get; } = new BaseAction(ActionID.AfflatusSolace, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    /// <summary>
    /// 神名
    /// </summary>
    public static IBaseAction Tetragrammaton { get; } = new BaseAction(ActionID.Tetragrammaton, true, isTimeline: true);

    /// <summary>
    /// 神祝祷
    /// </summary>
    public static IBaseAction DivineBenison { get; } = new BaseAction(ActionID.DivineBenison, true, isTimeline: true)
    {
        StatusProvide = new StatusID[] { StatusID.DivineBenison },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 狂喜之心
    /// </summary>
    public static IBaseAction AfflatusRapture { get; } = new BaseAction(ActionID.AfflatusRapture, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    /// <summary>
    /// 水流幕
    /// </summary>
    public static IBaseAction Aquaveil { get; } = new BaseAction(ActionID.Aquaveil, true, isTimeline: true);

    /// <summary>
    /// 礼仪之铃
    /// </summary>
    public static IBaseAction LiturgyoftheBell { get; } = new BaseAction(ActionID.LiturgyoftheBell, true, isTimeline: true);
    #endregion
    #region 输出
    /// <summary>
    /// 飞石 坚石 垒石 崩石 闪耀 闪灼
    /// </summary>
    public static IBaseAction Stone { get; } = new BaseAction(ActionID.Stone);

    /// <summary>
    /// 疾风 烈风 天辉
    /// </summary>
    public static IBaseAction Aero { get; } = new BaseAction(ActionID.Aero, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
            StatusID.Aero,
            StatusID.Aero2,
            StatusID.Dia,
        }
    };

    /// <summary>
    /// 神圣 豪圣
    /// </summary>
    public static IBaseAction Holy { get; } = new BaseAction(ActionID.Holy);

    /// <summary>
    /// 法令
    /// </summary>
    public static IBaseAction Assize { get; } = new BaseAction(ActionID.Assize);

    /// <summary>
    /// 苦难之心
    /// </summary>
    public static IBaseAction AfflatusMisery { get; } = new BaseAction(ActionID.AfflatusMisery)
    {
        ActionCheck = b => JobGauge.BloodLily == 3,
    };
    #endregion
    #region buff
    /// <summary>
    /// 神速咏唱
    /// </summary>
    public static IBaseAction PresenseOfMind { get; } = new BaseAction(ActionID.PresenseOfMind, true)
    {
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// 无中生有
    /// </summary>
    public static IBaseAction ThinAir { get; } = new BaseAction(ActionID.ThinAir, true);

    /// <summary>
    /// 全大赦
    /// </summary>
    public static IBaseAction PlenaryIndulgence { get; } = new BaseAction(ActionID.PlenaryIndulgence, true, isTimeline: true);

    /// <summary>
    /// 节制
    /// </summary>
    public static IBaseAction Temperance { get; } = new BaseAction(ActionID.Temperance, true, isTimeline: true);
    #endregion

}