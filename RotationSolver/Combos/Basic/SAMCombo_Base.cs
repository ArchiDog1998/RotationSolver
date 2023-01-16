using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Combos.Basic;

internal abstract class SAMCombo_Base : CustomCombo.CustomCombo
{
    private static SAMGauge JobGauge => Service.JobGauges.Get<SAMGauge>();

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
    public static BaseAction Hakaze { get; } = new(ActionID.Hakaze);

    /// <summary>
    /// 阵风
    /// </summary>
    public static BaseAction Jinpu { get; } = new(ActionID.Jinpu);

    /// <summary>
    /// 月光
    /// </summary>
    public static BaseAction Gekko { get; } = new(ActionID.Gekko);

    /// <summary>
    /// 士风
    /// </summary>
    public static BaseAction Shifu { get; } = new(ActionID.Shifu);

    /// <summary>
    /// 花车
    /// </summary>
    public static BaseAction Kasha { get; } = new(ActionID.Kasha);

    /// <summary>
    /// 雪风
    /// </summary>
    public static BaseAction Yukikaze { get; } = new(ActionID.Yukikaze);

    /// <summary>
    /// 照破
    /// </summary>
    public static BaseAction Shoha { get; } = new(ActionID.Shoha)
    {
        ActionCheck = b => MeditationStacks == 3
    };
    #endregion

    #region AoE

    /// <summary>
    /// 风雅
    /// </summary>
    public static BaseAction Fuga { get; } = new(ActionID.Fuga);

    /// <summary>
    /// 风光
    /// </summary>
    public static BaseAction Fuko { get; } = new(ActionID.Fuko);

    /// <summary>
    /// 满月
    /// </summary>
    public static BaseAction Mangetsu { get; } = new(ActionID.Mangetsu)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };
    /// <summary>
    /// 樱花
    /// </summary>
    public static BaseAction Oka { get; } = new(ActionID.Oka)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// 无明照破
    /// </summary>
    public static BaseAction Shoha2 { get; } = new(ActionID.Shoha2)
    {
        ActionCheck = b => MeditationStacks == 3
    };

    /// <summary>
    /// 奥义斩浪
    /// </summary>
    public static BaseAction OgiNamikiri { get; } = new(ActionID.OgiNamikiri)
    {
        BuffsNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// 回返斩浪
    /// </summary>
    public static BaseAction KaeshiNamikiri { get; } = new(ActionID.KaeshiNamikiri)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.NAMIKIRI
    };
    #endregion

    #region 居合术
    /// <summary>
    /// 彼岸花
    /// </summary>
    public static BaseAction Higanbana { get; } = new(ActionID.Higanbana, isEot: true)
    {
        ActionCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// 天下五剑
    /// </summary>
    public static BaseAction TenkaGoken { get; } = new(ActionID.TenkaGoken)
    {
        ActionCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// 纷乱雪月花
    /// </summary>
    public static BaseAction MidareSetsugekka { get; } = new(ActionID.MidareSetsugekka)
    {
        ActionCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// 燕回返
    /// </summary>
    public static BaseAction TsubameGaeshi { get; } = new(ActionID.TsubameGaeshi);

    /// <summary>
    /// 回返五剑
    /// </summary>
    public static BaseAction KaeshiGoken { get; } = new(ActionID.KaeshiGoken)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.GOKEN
    };

    /// <summary>
    /// 回返雪月花
    /// </summary>
    public static BaseAction KaeshiSetsugekka { get; } = new(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.SETSUGEKKA
    };
    #endregion

    #region 其它
    /// <summary>
    /// 心眼
    /// </summary>
    public static BaseAction ThirdEye { get; } = new(ActionID.ThirdEye, true, isTimeline: true);

    /// <summary>
    /// 燕飞
    /// </summary>
    public static BaseAction Enpi { get; } = new(ActionID.Enpi);

    /// <summary>
    /// 明镜止水
    /// </summary>
    public static BaseAction MeikyoShisui { get; } = new(ActionID.MeikyoShisui)
    {
        BuffsProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// 叶隐
    /// </summary>
    public static BaseAction Hagakure { get; } = new(ActionID.Hagakure)
    {
        ActionCheck = b => SenCount > 0
    };

    /// <summary>
    /// 意气冲天
    /// </summary>
    public static BaseAction Ikishoten { get; } = new(ActionID.Ikishoten)
    {
        BuffsProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => InCombat
    };
    #endregion

    #region 必杀技
    /// <summary>
    /// 必杀剑·震天
    /// </summary>
    public static BaseAction HissatsuShinten { get; } = new(ActionID.HissatsuShinten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·晓天
    /// </summary>
    public static BaseAction HissatsuGyoten { get; } = new(ActionID.HissatsuGyoten)
    {
        ActionCheck = b => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// 必杀剑·夜天
    /// </summary>
    public static BaseAction HissatsuYaten { get; } = new(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    /// <summary>
    /// 必杀剑·九天
    /// </summary>
    public static BaseAction HissatsuKyuten { get; } = new(ActionID.HissatsuKyuten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·红莲
    /// </summary>
    public static BaseAction HissatsuGuren { get; } = new(ActionID.HissatsuGuren)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// 必杀剑·闪影
    /// </summary>
    public static BaseAction HissatsuSenei { get; } = new(ActionID.HissatsuSenei)
    {
        ActionCheck = b => Kenki >= 25
    };
    #endregion

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (HissatsuGyoten.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}