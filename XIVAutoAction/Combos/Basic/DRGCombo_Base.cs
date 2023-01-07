using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;

namespace AutoAction.Combos.Basic;

internal abstract class DRGCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };

    /// <summary>
    /// ��׼��
    /// </summary>
    public static BaseAction TrueThrust { get; } = new(ActionID.TrueThrust);

    /// <summary>
    /// ��ͨ��
    /// </summary>
    public static BaseAction VorpalThrust { get; } = new(ActionID.VorpalThrust)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ֱ��
    /// </summary>
    public static BaseAction FullThrust { get; } = new(ActionID.FullThrust);

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static BaseAction Disembowel { get; } = new(ActionID.Disembowel)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ӣ��ŭ��
    /// </summary>
    public static BaseAction ChaosThrust { get; } = new(ActionID.ChaosThrust);

    /// <summary>
    /// ������צ
    /// </summary>
    public static BaseAction FangandClaw { get; } = new(ActionID.FangandClaw)
    {
        BuffsNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    /// <summary>
    /// ��β�����
    /// </summary>
    public static BaseAction WheelingThrust { get; } = new(ActionID.WheelingThrust)
    {
        BuffsNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    /// <summary>
    /// �ᴩ��
    /// </summary>
    public static BaseAction PiercingTalon { get; } = new(ActionID.PiercingTalon);

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static BaseAction DoomSpike { get; } = new(ActionID.DoomSpike);

    /// <summary>
    /// ���ٴ�
    /// </summary>
    public static BaseAction SonicThrust { get; } = new(ActionID.SonicThrust)
    {
        OtherIDsCombo = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// ɽ������
    /// </summary>
    public static BaseAction CoerthanTorment { get; } = new(ActionID.CoerthanTorment);

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction SpineshatterDive { get; } = new(ActionID.SpineshatterDive);

    /// <summary>
    /// ���׳�
    /// </summary>
    public static BaseAction DragonfireDive { get; } = new(ActionID.DragonfireDive);

    /// <summary>
    /// ��Ծ
    /// </summary>
    public static BaseAction Jump { get; } = new(ActionID.Jump)
    {
        BuffsProvide = new StatusID[] { StatusID.DiveReady },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction HighJump { get; } = new(ActionID.HighJump)
    {
        BuffsProvide = Jump.BuffsProvide,
    };

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction MirageDive { get; } = new(ActionID.MirageDive)
    {
        BuffsNeed = Jump.BuffsProvide,
    };

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static BaseAction Geirskogul { get; } = new(ActionID.Geirskogul);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static BaseAction Nastrond { get; } = new(ActionID.Nastrond)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ׹�ǳ�
    /// </summary>
    public static BaseAction Stardiver { get; } = new(ActionID.Stardiver)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// �����㾦
    /// </summary>
    public static BaseAction WyrmwindThrust { get; } = new(ActionID.WyrmwindThrust)
    {
        ActionCheck = b => JobGauge.FirstmindsFocusCount == 2,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction LifeSurge { get; } = new(ActionID.LifeSurge, true)
    {
        BuffsProvide = new[] { StatusID.LifeSurge },

        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    /// <summary>
    /// ��ǹ
    /// </summary>
    public static BaseAction LanceCharge { get; } = new(ActionID.LanceCharge, true);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction DragonSight { get; } = new(ActionID.DragonSight, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();

            if (Targets.Count() == 0) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    /// <summary>
    /// ս������
    /// </summary>
    public static BaseAction BattleLitany { get; } = new(ActionID.BattleLitany, true)
    {
        BuffsNeed = new[] { StatusID.PowerSurge },
    };
}
