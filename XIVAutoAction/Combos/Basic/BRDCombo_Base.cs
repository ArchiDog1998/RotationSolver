using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using AutoAction.Actions;
using AutoAction.Actions.BaseAction;
using AutoAction.Combos.CustomCombo;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;

namespace AutoAction.Combos.Basic;

internal abstract class BRDCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static BRDGauge JobGauge => Service.JobGauges.Get<BRDGauge>();

    /// <summary>
    /// ʫ������
    /// </summary>
    protected static byte Repertoire => JobGauge.Repertoire;

    /// <summary>
    /// ��ǰ���ڳ��ĸ�
    /// </summary>
    protected static Song Song => JobGauge.Song;

    /// <summary>
    /// ��һ�׳��ĸ�
    /// </summary>
    protected static Song LastSong => JobGauge.LastSong;

    /// <summary>
    /// ���֮��
    /// </summary>
    protected static byte SoulVoice => JobGauge.SoulVoice;

    /// <summary>
    /// ���׸谡�ڶ�ú��ڳ���(�Ƿ��Ѿ�����)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SongEndAfter(float time)
    {
        return EndAfter(JobGauge.SongTimer / 1000f, time) && JobGauge.SongTimer / 1000f <= time;
    }

    /// <summary>
    /// ���׸谡�ڶ�ú��ڳ���
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool SongEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.SongTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new[] { ClassJobID.Bard, ClassJobID.Archer };


    /// <summary>
    /// ǿ�����
    /// </summary>
    public static BaseAction HeavyShoot { get; } = new(ActionID.HeavyShoot) { BuffsProvide = new[] { StatusID.StraightShotReady } };

    /// <summary>
    /// ֱ�����
    /// </summary>
    public static BaseAction StraitShoot { get; } = new(ActionID.StraitShoot) { BuffsNeed = new[] { StatusID.StraightShotReady } };

    /// <summary>
    /// ��ҧ��
    /// </summary>
    public static BaseAction VenomousBite { get; } = new(ActionID.VenomousBite, isEot: true)
    {
        TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite }
    };

    /// <summary>
    /// ��ʴ��
    /// </summary>
    public static BaseAction Windbite { get; } = new(ActionID.Windbite, isEot: true)
    {
        TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite }
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction IronJaws { get; } = new(ActionID.IronJaws, isEot: true)
    {
        TargetStatus = VenomousBite.TargetStatus.Union(Windbite.TargetStatus).ToArray(),
        ActionCheck = b => b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, Windbite.TargetStatus),
    };

    /// <summary>
    /// �������С������
    /// </summary>
    public static BaseAction WanderersMinuet { get; } = new(ActionID.WanderersMinuet);

    /// <summary>
    /// ���ߵ�����ҥ
    /// </summary>
    public static BaseAction MagesBallad { get; } = new(ActionID.MagesBallad);

    /// <summary>
    /// �����������
    /// </summary>
    public static BaseAction ArmysPaeon { get; } = new(ActionID.ArmysPaeon);

    /// <summary>
    /// ս��֮��
    /// </summary>
    public static BaseAction BattleVoice { get; } = new(ActionID.BattleVoice, true);

    /// <summary>
    /// ����ǿ��
    /// </summary>
    public static BaseAction RagingStrikes { get; } = new(ActionID.RagingStrikes, true);

    /// <summary>
    /// ���������������
    /// </summary>
    public static BaseAction RadiantFinale { get; } = new(ActionID.RadiantFinale, true)
    {
        ActionCheck = b => JobGauge.Coda.Any(s => s != Song.NONE),
    };

    /// <summary>
    /// ���Ҽ�
    /// </summary>
    public static BaseAction Barrage { get; } = new(ActionID.Barrage);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction EmpyrealArrow { get; } = new(ActionID.EmpyrealArrow);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction PitchPerfect { get; } = new(ActionID.PitchPerfect)
    {
        ActionCheck = b => JobGauge.Song == Song.WANDERER,
    };

    /// <summary>
    /// ʧѪ��
    /// </summary>
    public static BaseAction Bloodletter { get; } = new(ActionID.Bloodletter);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction RainofDeath { get; } = new(ActionID.RainofDeath);

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction QuickNock { get; } = new(ActionID.QuickNock)
    {
        BuffsProvide = new[] { StatusID.ShadowbiteReady }
    };

    /// <summary>
    /// Ӱ�ɼ�
    /// </summary>
    public static BaseAction Shadowbite { get; } = new(ActionID.Shadowbite)
    {
        BuffsNeed = new[] { StatusID.ShadowbiteReady }
    };

    /// <summary>
    /// ����������޿���
    /// </summary>
    public static BaseAction WardensPaean { get; } = new(ActionID.WardensPaean, true);

    /// <summary>
    /// ��������������
    /// </summary>
    public static BaseAction NaturesMinne { get; } = new(ActionID.NaturesMinne, true);

    /// <summary>
    /// ����յ���
    /// </summary>
    public static BaseAction Sidewinder { get; } = new(ActionID.Sidewinder);

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction ApexArrow { get; } = new(ActionID.ApexArrow)
    {
        ActionCheck = b => JobGauge.SoulVoice >= 20 && !Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// ���Ƽ�
    /// </summary>
    public static BaseAction BlastArrow { get; } = new(ActionID.BlastArrow)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Troubadour { get; } = new(ActionID.Troubadour, true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ĳЩ�ǳ�Σ�յ�״̬��
        if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Any() || TargetUpdater.DyingPeople.Any())
        {
            if (WardensPaean.ShouldUse(out act, mustUse: true)) return true;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }
}
