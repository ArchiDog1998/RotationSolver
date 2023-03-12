using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

public abstract class SAM_Base : CustomRotation.CustomRotation
{
    private static SAMGauge JobGauge => Service.JobGauges.Get<SAMGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;


    /// <summary>
    /// 雪闪
    /// </summary>
    protected static bool HasSetsu => JobGauge.HasSetsu;

    /// <summary>
    /// 月闪
    /// </summary>
    protected static bool HasGetsu => JobGauge.HasGetsu;

    /// <summary>
    /// 花闪
    /// </summary>
    protected static bool HasKa => JobGauge.HasKa;

    /// <summary>
    /// 剑气
    /// </summary>
    protected static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// 剑压
    /// </summary>
    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    /// <summary>
    /// 闪的数量
    /// </summary>
    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Fugetsu);
    protected static float MoonTime => Player.StatusTime(true, StatusID.Fugetsu);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Fuka);
    protected static float FlowerTime => Player.StatusTime(true, StatusID.Fuka);

    #region 单体
    /// <summary>
    /// 刃风
    /// </summary>
    public static IBaseAction Hakaze { get; } = new BaseAction(ActionID.Hakaze);

    /// <summary>
    /// 阵风
    /// </summary>
    public static IBaseAction Jinpu { get; } = new BaseAction(ActionID.Jinpu);

    /// <summary>
    /// 月光
    /// </summary>
    public static IBaseAction Gekko { get; } = new BaseAction(ActionID.Gekko);

    /// <summary>
    /// 士风
    /// </summary>
    public static IBaseAction Shifu { get; } = new BaseAction(ActionID.Shifu);

    /// <summary>
    /// 花车
    /// </summary>
    public static IBaseAction Kasha { get; } = new BaseAction(ActionID.Kasha);

    /// <summary>
    /// 雪风
    /// </summary>
    public static IBaseAction Yukikaze { get; } = new BaseAction(ActionID.Yukikaze);

    /// <summary>
    /// 照破
    /// </summary>
    public static IBaseAction Shoha { get; } = new BaseAction(ActionID.Shoha)
    {
        ActionCheck = b => MeditationStacks == 3
    };
    #endregion

    #region AoE

    /// <summary>
    /// 风雅
    /// </summary>
    public static IBaseAction Fuga { get; } = new BaseAction(ActionID.Fuga);

    /// <summary>
    /// 风光
    /// </summary>
    public static IBaseAction Fuko { get; } = new BaseAction(ActionID.Fuko);

    /// <summary>
    /// 满月
    /// </summary>
    public static IBaseAction Mangetsu { get; } = new BaseAction(ActionID.Mangetsu)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };
    /// <summary>
    /// 樱花
    /// </summary>
    public static IBaseAction Oka { get; } = new BaseAction(ActionID.Oka)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// 无明照破
    /// </summary>
    public static IBaseAction Shoha2 { get; } = new BaseAction(ActionID.Shoha2)
    {
        ActionCheck = b => MeditationStacks == 3
    };

    /// <summary>
    /// 奥义斩浪
    /// </summary>
    public static IBaseAction OgiNamikiri { get; } = new BaseAction(ActionID.OgiNamikiri)
    {
        StatusNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// 回返斩浪
    /// </summary>
    public static IBaseAction KaeshiNamikiri { get; } = new BaseAction(ActionID.KaeshiNamikiri)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.NAMIKIRI
    };
    #endregion

    #region 居合术
    /// <summary>
    /// 彼岸花
    /// </summary>
    public static IBaseAction Higanbana { get; } = new BaseAction(ActionID.Higanbana, isEot: true)
    {
        ActionCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// 天下五剑
    /// </summary>
    public static IBaseAction TenkaGoken { get; } = new BaseAction(ActionID.TenkaGoken)
    {
        ActionCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// 纷乱雪月花
    /// </summary>
    public static IBaseAction MidareSetsugekka { get; } = new BaseAction(ActionID.MidareSetsugekka)
    {
        ActionCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// 燕回返
    /// </summary>
    public static IBaseAction TsubameGaeshi { get; } = new BaseAction(ActionID.TsubameGaeshi);

    /// <summary>
    /// 回返五剑
    /// </summary>
    public static IBaseAction KaeshiGoken { get; } = new BaseAction(ActionID.KaeshiGoken)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.GOKEN
    };

    /// <summary>
    /// 回返雪月花
    /// </summary>
    public static IBaseAction KaeshiSetsugekka { get; } = new BaseAction(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.SETSUGEKKA
    };
    #endregion

    #region 其它
    /// <summary>
    /// 心眼
    /// </summary>
    public static IBaseAction ThirdEye { get; } = new BaseAction(ActionID.ThirdEye, true, isTimeline: true);

    /// <summary>
    /// 燕飞
    /// </summary>
    public static IBaseAction Enpi { get; } = new BaseAction(ActionID.Enpi)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
    };

    /// <summary>
    /// 明镜止水
    /// </summary>
    public static IBaseAction MeikyoShisui { get; } = new BaseAction(ActionID.MeikyoShisui)
    {
        StatusProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// 叶隐
    /// </summary>
    public static IBaseAction Hagakure { get; } = new BaseAction(ActionID.Hagakure)
    {
        ActionCheck = b => SenCount > 0
    };

    /// <summary>
    /// 意气冲天
    /// </summary>
    public static IBaseAction Ikishoten { get; } = new BaseAction(ActionID.Ikishoten)
    {
        StatusProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => InCombat
    };
    #endregion

    #region 必杀技
    /// <summary>
    /// 必杀剑·震天
    /// </summary>
    public static IBaseAction HissatsuShinten { get; } = new BaseAction(ActionID.HissatsuShinten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·晓天
    /// </summary>
    public static IBaseAction HissatsuGyoten { get; } = new BaseAction(ActionID.HissatsuGyoten)
    {
        ActionCheck = b => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// 必杀剑·夜天
    /// </summary>
    public static IBaseAction HissatsuYaten { get; } = new BaseAction(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    /// <summary>
    /// 必杀剑·九天
    /// </summary>
    public static IBaseAction HissatsuKyuten { get; } = new BaseAction(ActionID.HissatsuKyuten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·红莲
    /// </summary>
    public static IBaseAction HissatsuGuren { get; } = new BaseAction(ActionID.HissatsuGuren)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·闪影
    /// </summary>
    public static IBaseAction HissatsuSenei { get; } = new BaseAction(ActionID.HissatsuSenei)
    {
        ActionCheck = b => Kenki >= 25
    };
    #endregion

    [RotationDesc(ActionID.HissatsuGyoten)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        if (HissatsuGyoten.CanUse(out act, emptyOrSkipCombo: true, recordTarget: recordTarget)) return true;
        return false;
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.ThirdEye)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ThirdEye.CanUse(out act)) return true;
        return false;
    }
}