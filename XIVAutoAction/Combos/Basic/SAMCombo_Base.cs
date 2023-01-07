using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using AutoAction.Actions;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;

namespace AutoAction.Combos.Basic;

internal abstract class SAMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static SAMGauge JobGauge => Service.JobGauges.Get<SAMGauge>();

    /// <summary>
    /// ѩ��
    /// </summary>
    protected static bool HasSetsu => JobGauge.HasSetsu;

    /// <summary>
    /// ����
    /// </summary>
    protected static bool HasGetsu => JobGauge.HasGetsu;

    /// <summary>
    /// ����
    /// </summary>
    protected static bool HasKa => JobGauge.HasKa;

    /// <summary>
    /// ����
    /// </summary>
    protected static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// ��ѹ
    /// </summary>
    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    /// <summary>
    /// ��������
    /// </summary>
    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Fugetsu);
    protected static float MoonTime => Player.StatusTime(true, StatusID.Fugetsu);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Fuka);
    protected static float FlowerTime => Player.StatusTime(true, StatusID.Fuka);

    #region ����
    /// <summary>
    /// �з�
    /// </summary>
    public static BaseAction Hakaze { get; } = new(ActionID.Hakaze);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Jinpu { get; } = new(ActionID.Jinpu);

    /// <summary>
    /// �¹�
    /// </summary>
    public static BaseAction Gekko { get; } = new(ActionID.Gekko);

    /// <summary>
    /// ʿ��
    /// </summary>
    public static BaseAction Shifu { get; } = new(ActionID.Shifu);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Kasha { get; } = new(ActionID.Kasha);

    /// <summary>
    /// ѩ��
    /// </summary>
    public static BaseAction Yukikaze { get; } = new(ActionID.Yukikaze);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Shoha { get; } = new(ActionID.Shoha)
    {
        ActionCheck = b => MeditationStacks == 3
    };
    #endregion

    #region AoE

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Fuga { get; } = new(ActionID.Fuga);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Fuko { get; } = new(ActionID.Fuko);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Mangetsu { get; } = new(ActionID.Mangetsu)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };
    /// <summary>
    /// ӣ��
    /// </summary>
    public static BaseAction Oka { get; } = new(ActionID.Oka)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Shoha2 { get; } = new(ActionID.Shoha2)
    {
        ActionCheck = b => MeditationStacks == 3
    };

    /// <summary>
    /// ����ն��
    /// </summary>
    public static BaseAction OgiNamikiri { get; } = new(ActionID.OgiNamikiri)
    {
        BuffsNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// �ط�ն��
    /// </summary>
    public static BaseAction KaeshiNamikiri { get; } = new(ActionID.KaeshiNamikiri)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.NAMIKIRI
    };
    #endregion

    #region �Ӻ���
    /// <summary>
    /// �˰���
    /// </summary>
    public static BaseAction Higanbana { get; } = new(ActionID.Higanbana, isEot: true)
    {
        ActionCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// �����彣
    /// </summary>
    public static BaseAction TenkaGoken { get; } = new(ActionID.TenkaGoken)
    {
        ActionCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// ����ѩ�»�
    /// </summary>
    public static BaseAction MidareSetsugekka { get; } = new(ActionID.MidareSetsugekka)
    {
        ActionCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// ��ط�
    /// </summary>
    public static BaseAction TsubameGaeshi { get; } = new(ActionID.TsubameGaeshi);

    /// <summary>
    /// �ط��彣
    /// </summary>
    public static BaseAction KaeshiGoken { get; } = new(ActionID.KaeshiGoken)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.GOKEN
    };

    /// <summary>
    /// �ط�ѩ�»�
    /// </summary>
    public static BaseAction KaeshiSetsugekka { get; } = new(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.SETSUGEKKA
    };
    #endregion

    #region ����
    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction ThirdEye { get; } = new(ActionID.ThirdEye, true);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Enpi { get; } = new(ActionID.Enpi);

    /// <summary>
    /// ����ֹˮ
    /// </summary>
    public static BaseAction MeikyoShisui { get; } = new(ActionID.MeikyoShisui)
    {
        BuffsProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// Ҷ��
    /// </summary>
    public static BaseAction Hagakure { get; } = new(ActionID.Hagakure)
    {
        ActionCheck = b => SenCount > 0
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Ikishoten { get; } = new(ActionID.Ikishoten)
    {
        BuffsProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => InCombat
    };
    #endregion

    #region ��ɱ��
    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuShinten { get; } = new(ActionID.HissatsuShinten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuGyoten { get; } = new(ActionID.HissatsuGyoten)
    {
        ActionCheck = b => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// ��ɱ����ҹ��
    /// </summary>
    public static BaseAction HissatsuYaten { get; } = new(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuKyuten { get; } = new(ActionID.HissatsuKyuten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuGuren { get; } = new(ActionID.HissatsuGuren)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ������Ӱ
    /// </summary>
    public static BaseAction HissatsuSenei { get; } = new(ActionID.HissatsuSenei)
    {
        ActionCheck = b => Kenki >= 25
    };
    #endregion

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (HissatsuGyoten.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}