using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver;

namespace RotationSolver.Combos.Basic;

internal abstract class WHMCombo_Base : CustomCombo.CustomCombo
{
    private static WHMGauge JobGauge => Service.JobGauges.Get<WHMGauge>();

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
    private sealed protected override BaseAction Raise => Raise1;

    #region 治疗
    /// <summary>
    /// 治疗
    /// </summary>
    public static BaseAction Cure { get; } = new(ActionID.Cure, true, isTimeline: true);

    /// <summary>
    /// 医治
    /// </summary>
    public static BaseAction Medica { get; } = new(ActionID.Medica, true, isTimeline: true);

    /// <summary>
    /// 复活
    /// </summary>
    public static BaseAction Raise1 { get; } = new(ActionID.Raise1, true);

    /// <summary>
    /// 救疗
    /// </summary>
    public static BaseAction Cure2 { get; } = new(ActionID.Cure2, true, isTimeline: true);

    /// <summary>
    /// 医济
    /// </summary>
    public static BaseAction Medica2 { get; } = new(ActionID.Medica2, true, isEot: true, isTimeline: true)
    {
        BuffsProvide = new[] { StatusID.Medica2, StatusID.TrueMedica2 },
    };

    /// <summary>
    /// 再生
    /// </summary>
    public static BaseAction Regen { get; } = new(ActionID.Regen, true, isEot: true, isTimeline: true)
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
    public static BaseAction Cure3 { get; } = new(ActionID.Cure3, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// 天赐祝福
    /// </summary>
    public static BaseAction Benediction { get; } = new(ActionID.Benediction, true, isTimeline: true);

    /// <summary>
    /// 庇护所
    /// </summary>
    public static BaseAction Asylum { get; } = new(ActionID.Asylum, true, isTimeline: true);

    /// <summary>
    /// 安慰之心
    /// </summary>
    public static BaseAction AfflatusSolace { get; } = new(ActionID.AfflatusSolace, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    /// <summary>
    /// 神名
    /// </summary>
    public static BaseAction Tetragrammaton { get; } = new(ActionID.Tetragrammaton, true, isTimeline: true);

    /// <summary>
    /// 神祝祷
    /// </summary>
    public static BaseAction DivineBenison { get; } = new(ActionID.DivineBenison, true, isTimeline: true)
    {
        BuffsProvide = new StatusID[] { StatusID.DivineBenison },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 狂喜之心
    /// </summary>
    public static BaseAction AfflatusRapture { get; } = new(ActionID.AfflatusRapture, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    /// <summary>
    /// 水流幕
    /// </summary>
    public static BaseAction Aquaveil { get; } = new(ActionID.Aquaveil, true, isTimeline: true);

    /// <summary>
    /// 礼仪之铃
    /// </summary>
    public static BaseAction LiturgyoftheBell { get; } = new(ActionID.LiturgyoftheBell, true, isTimeline: true);
    #endregion
    #region 输出
    /// <summary>
    /// 飞石 坚石 垒石 崩石 闪耀 闪灼
    /// </summary>
    public static BaseAction Stone { get; } = new(ActionID.Stone);

    /// <summary>
    /// 疾风 烈风 天辉
    /// </summary>
    public static BaseAction Aero { get; } = new(ActionID.Aero, isEot: true)
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
    public static BaseAction Holy { get; } = new(ActionID.Holy);

    /// <summary>
    /// 法令
    /// </summary>
    public static BaseAction Assize { get; } = new(ActionID.Assize);

    /// <summary>
    /// 苦难之心
    /// </summary>
    public static BaseAction AfflatusMisery { get; } = new(ActionID.AfflatusMisery)
    {
        ActionCheck = b => JobGauge.BloodLily == 3,
    };
    #endregion
    #region buff
    /// <summary>
    /// 神速咏唱
    /// </summary>
    public static BaseAction PresenseOfMind { get; } = new(ActionID.PresenseOfMind, true)
    {
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// 无中生有
    /// </summary>
    public static BaseAction ThinAir { get; } = new(ActionID.ThinAir, true);

    /// <summary>
    /// 全大赦
    /// </summary>
    public static BaseAction PlenaryIndulgence { get; } = new(ActionID.PlenaryIndulgence, true, isTimeline: true);

    /// <summary>
    /// 节制
    /// </summary>
    public static BaseAction Temperance { get; } = new(ActionID.Temperance, true, isTimeline: true);
    #endregion

}