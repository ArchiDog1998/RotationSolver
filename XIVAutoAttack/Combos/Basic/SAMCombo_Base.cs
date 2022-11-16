using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SAMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
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
    /// 什么气？
    /// </summary>
    protected static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// 剑压
    /// </summary>
    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Moon);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Flower);

    /// <summary>
    /// 刃风
    /// </summary>
    public static BaseAction Hakaze { get; } = new(ActionID.Hakaze);

    /// <summary>
    /// 阵风
    /// </summary>
    public static BaseAction Jinpu { get; } = new(ActionID.Jinpu);

    /// <summary>
    /// 心眼
    /// </summary>
    public static BaseAction ThirdEye { get; } = new(ActionID.ThirdEye, true);

    /// <summary>
    /// 燕飞
    /// </summary>
    public static BaseAction Enpi { get; } = new(ActionID.Enpi);

    /// <summary>
    /// 士风
    /// </summary>
    public static BaseAction Shifu { get; } = new(ActionID.Shifu);

    /// <summary>
    /// 风雅
    /// </summary>
    public static BaseAction Fuga { get; } = new(ActionID.Fuga);

    /// <summary>
    /// 月光
    /// </summary>
    public static BaseAction Gekko { get; } = new(ActionID.Gekko);

    /// <summary>
    /// 彼岸花
    /// </summary>
    public static BaseAction Higanbana { get; } = new(ActionID.Higanbana, isEot: true)
    {
        OtherCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// 天下五剑
    /// </summary>
    public static BaseAction TenkaGoken { get; } = new(ActionID.TenkaGoken)
    {
        OtherCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// 纷乱雪月花
    /// </summary>
    public static BaseAction MidareSetsugekka { get; } = new(ActionID.MidareSetsugekka)
    {
        OtherCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// 满月
    /// </summary>
    public static BaseAction Mangetsu { get; } = new(ActionID.Mangetsu);

    /// <summary>
    /// 花车
    /// </summary>
    public static BaseAction Kasha { get; } = new(ActionID.Kasha);

    /// <summary>
    /// 樱花
    /// </summary>
    public static BaseAction Oka { get; } = new(ActionID.Oka);

    /// <summary>
    /// 明镜止水
    /// </summary>
    public static BaseAction MeikyoShisui { get; } = new(ActionID.MeikyoShisui)
    {
        BuffsProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// 雪风
    /// </summary>
    public static BaseAction Yukikaze { get; } = new(ActionID.Yukikaze);

    /// <summary>
    /// 必杀剑·晓天
    /// </summary>
    public static BaseAction HissatsuGyoten { get; } = new(ActionID.HissatsuGyoten);

    /// <summary>
    /// 必杀剑·震天
    /// </summary>
    public static BaseAction HissatsuShinten { get; } = new(ActionID.HissatsuShinten);

    /// <summary>
    /// 必杀剑·九天
    /// </summary>
    public static BaseAction HissatsuKyuten { get; } = new(ActionID.HissatsuKyuten);

    /// <summary>
    /// 意气冲天
    /// </summary>
    public static BaseAction Ikishoten { get; } = new(ActionID.Ikishoten);

    /// <summary>
    /// 必杀剑·红莲
    /// </summary>
    public static BaseAction HissatsuGuren { get; } = new(ActionID.HissatsuGuren);

    /// <summary>
    /// 必杀剑·闪影
    /// </summary>
    public static BaseAction HissatsuSenei { get; } = new(ActionID.HissatsuSenei);

    /// <summary>
    /// 回返五剑
    /// </summary>
    public static BaseAction KaeshiGoken { get; } = new(ActionID.KaeshiGoken);

    /// <summary>
    /// 回返雪月花
    /// </summary>
    public static BaseAction KaeshiSetsugekka { get; } = new(ActionID.KaeshiSetsugekka);

    /// <summary>
    /// 照破
    /// </summary>
    public static BaseAction Shoha { get; } = new(ActionID.Shoha);

    /// <summary>
    /// 无明照破
    /// </summary>
    public static BaseAction Shoha2 { get; } = new(ActionID.Shoha2);

    /// <summary>
    /// 奥义斩浪
    /// </summary>
    public static BaseAction OgiNamikiri { get; } = new(ActionID.OgiNamikiri)
    {
        OtherCheck = b => HaveFlower && HaveMoon,
        BuffsNeed = new[] { StatusID.OgiNamikiriReady },
    };

    /// <summary>
    /// 回返斩浪
    /// </summary>
    public static BaseAction KaeshiNamikiri { get; } = new(ActionID.KaeshiNamikiri);

}