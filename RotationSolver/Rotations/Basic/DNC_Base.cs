using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.Data;

namespace RotationSolver.Rotations.Basic;
internal abstract class DNCRotation_Base : CustomRotation.CustomRotation
{
    private static DNCGauge JobGauge => Service.JobGauges.Get<DNCGauge>();

    /// <summary>
    /// 正在跳舞
    /// </summary>
    protected static bool IsDancing => JobGauge.IsDancing;

    /// <summary>
    /// 伶俐
    /// </summary>
    protected static byte Esprit => JobGauge.Esprit;

    /// <summary>
    /// 幻扇数
    /// </summary>
    protected static byte Feathers => JobGauge.Feathers;

    protected static byte CompletedSteps => JobGauge.CompletedSteps;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    /// <summary>
    /// 瀑泻
    /// </summary>
    public static IBaseAction Cascade { get; } = new BaseAction(ActionID.Cascade)
    {
        StatusProvide = new[] { StatusID.SilkenSymmetry }
    };

    /// <summary>
    /// 喷泉
    /// </summary>
    public static IBaseAction Fountain { get; } = new BaseAction(ActionID.Fountain)
    {
        StatusProvide = new[] { StatusID.SilkenFlow }
    };

    /// <summary>
    /// 逆瀑泻
    /// </summary>
    public static IBaseAction ReverseCascade { get; } = new BaseAction(ActionID.ReverseCascade)
    {
        StatusNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
    };

    /// <summary>
    /// 坠喷泉
    /// </summary>
    public static IBaseAction Fountainfall { get; } = new BaseAction(ActionID.Fountainfall)
    {
        StatusNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
    };

    /// <summary>
    /// 扇舞·序
    /// </summary>
    public static IBaseAction FanDance { get; } = new BaseAction(ActionID.FanDance)
    {
        ActionCheck = b => JobGauge.Feathers > 0,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// 风车
    /// </summary>
    public static IBaseAction Windmill { get; } = new BaseAction(ActionID.Windmill)
    {
        StatusProvide = Cascade.StatusProvide,
    };

    /// <summary>
    /// 落刃雨
    /// </summary>
    public static IBaseAction Bladeshower { get; } = new BaseAction(ActionID.Bladeshower)
    {
        StatusProvide = Fountain.StatusProvide,
    };

    /// <summary>
    /// 升风车
    /// </summary>
    public static IBaseAction RisingWindmill { get; } = new BaseAction(ActionID.RisingWindmill)
    {
        StatusNeed = ReverseCascade.StatusNeed,
    };

    /// <summary>
    /// 落血雨
    /// </summary>
    public static IBaseAction Bloodshower { get; } = new BaseAction(ActionID.Bloodshower)
    {
        AOECount = 2,
        StatusNeed = Fountainfall.StatusNeed,
    };

    /// <summary>
    /// 扇舞·破
    /// </summary>
    public static IBaseAction FanDance2 { get; } = new BaseAction(ActionID.FanDance2)
    {
        ActionCheck = b => Feathers > 0,
        AOECount = 2,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// 扇舞·急
    /// </summary>
    public static IBaseAction FanDance3 { get; } = new BaseAction(ActionID.FanDance3)
    {
        StatusNeed = FanDance2.StatusProvide,
    };

    /// <summary>
    /// 扇舞·终
    /// </summary>
    public static IBaseAction FanDance4 { get; } = new BaseAction(ActionID.FanDance4)
    {
        StatusNeed = new[] { StatusID.FourfoldFanDance },
    };

    /// <summary>
    /// 剑舞
    /// </summary>
    public static IBaseAction SaberDance { get; } = new BaseAction(ActionID.SaberDance)
    {
        ActionCheck = b => Esprit >= 50,
    };

    /// <summary>
    /// 流星舞
    /// </summary>
    public static IBaseAction StarfallDance { get; } = new BaseAction(ActionID.StarfallDance)
    {
        StatusNeed = new[] { StatusID.FlourishingStarfall },
    };

    /// <summary>
    /// 前冲步
    /// </summary>
    public static IBaseAction EnAvant { get; } = new BaseAction(ActionID.EnAvant, true, shouldEndSpecial: true);

    /// <summary>
    /// 蔷薇曲脚步
    /// </summary>
    private static IBaseAction Emboite { get; } = new BaseAction(ActionID.Emboite, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Emboite,
    };

    /// <summary>
    /// 小鸟交叠跳
    /// </summary>
    private static IBaseAction Entrechat { get; } = new BaseAction(ActionID.Entrechat, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
    };

    /// <summary>
    /// 绿叶小踢腿
    /// </summary>
    private static IBaseAction Jete { get; } = new BaseAction(ActionID.Jete, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Jete,
    };

    /// <summary>
    /// 金冠趾尖转
    /// </summary>
    private static IBaseAction Pirouette { get; } = new BaseAction(ActionID.Pirouette, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
    };

    /// <summary>
    /// 标准舞步
    /// </summary>
    public static IBaseAction StandardStep { get; } = new BaseAction(ActionID.StandardStep)
    {
        StatusProvide = new[]
        {
            StatusID.StandardStep,
            StatusID.TechnicalStep,
        },
    };

    /// <summary>
    /// 技巧舞步
    /// </summary>
    public static IBaseAction TechnicalStep { get; } = new BaseAction(ActionID.TechnicalStep)
    {
        StatusNeed = new[]
        {
            StatusID.StandardFinish,
        },
        StatusProvide = StandardStep.StatusProvide,
    };

    /// <summary>
    /// 标准舞步结束
    /// </summary>
    private static IBaseAction StandardFinish { get; } = new BaseAction(ActionID.StandardFinish)
    {
        StatusNeed = new[] { StatusID.StandardStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 2,
    };

    /// <summary>
    /// 技巧舞步结束
    /// </summary>
    private static IBaseAction TechnicalFinish { get; } = new BaseAction(ActionID.TechnicalFinish)
    {
        StatusNeed = new[] { StatusID.TechnicalStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 4,
    };

    /// <summary>
    /// 防守之桑巴
    /// </summary>
    public static IBaseAction ShieldSamba { get; } = new BaseAction(ActionID.ShieldSamba, true, isTimeline: true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    /// <summary>
    /// 治疗之华尔兹
    /// </summary>
    public static IBaseAction CuringWaltz { get; } = new BaseAction(ActionID.CuringWaltz, true, isTimeline: true);

    /// <summary>
    /// 闭式舞姿
    /// </summary>
    public static IBaseAction ClosedPosition { get; } = new BaseAction(ActionID.ClosedPosition, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
            //Remove Weak
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)
            //Remove other partner.
            && !b.HasStatus(false, StatusID.ClosedPosition2) | b.HasStatus(true, StatusID.ClosedPosition2)
            ).ToArray();

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical).FirstOrDefault();
        },
    };

    /// <summary>
    /// 进攻之探戈
    /// </summary>
    public static IBaseAction Devilment { get; } = new BaseAction(ActionID.Devilment, true);

    /// <summary>
    /// 百花争艳
    /// </summary>
    public static IBaseAction Flourish { get; } = new BaseAction(ActionID.Flourish, true)
    {
        StatusNeed = new[] { StatusID.StandardFinish },
        StatusProvide = new[]
        {
            StatusID.ThreefoldFanDance,
            StatusID.FourfoldFanDance,
        },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// 即兴表演
    /// </summary>
    public static IBaseAction Improvisation { get; } = new BaseAction(ActionID.Improvisation, true);

    /// <summary>
    /// 提拉纳
    /// </summary>
    public static IBaseAction Tillana { get; } = new BaseAction(ActionID.Tillana)
    {
        StatusNeed = new[] { StatusID.FlourishingFinish },
    };

    /// <summary>
    /// 结束舞步
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool FinishStepGCD(out IAction act)
    {
        act = null;
        if (!IsDancing) return false;

        //标准舞步结束
        if (Player.HasStatus(true, StatusID.StandardStep) && Player.WillStatusEnd(1, true, StatusID.StandardStep) || StandardFinish.ShouldUse(out _, mustUse: true))
        {
            act = StandardStep;
            return true;
        }

        //技巧舞步结束
        if (Player.HasStatus(true, StatusID.TechnicalStep) && Player.WillStatusEnd(1, true, StatusID.TechnicalStep) || TechnicalFinish.ShouldUse(out _, mustUse: true))
        {
            act = TechnicalStep;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 执行舞步
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool ExcutionStepGCD(out IAction act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.StandardStep, StatusID.TechnicalStep)) return false;
        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2) return false;
        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4) return false;

        if (Emboite.ShouldUse(out act)) return true;
        if (Entrechat.ShouldUse(out act)) return true;
        if (Jete.ShouldUse(out act)) return true;
        if (Pirouette.ShouldUse(out act)) return true;
        return false;
    }
}
