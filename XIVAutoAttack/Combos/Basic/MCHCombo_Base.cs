using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class MCHCombo_Base<TCmd> : JobGaugeCombo<MCHGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Machinist };

    //分裂弹
    public static BaseAction SplitShot { get; } = new(ActionID.SplitShot);

    /// <summary>
    /// 独头弹
    /// </summary>
    public static BaseAction SlugShot { get; } = new(ActionID.SlugShot)
    {
        OtherIDsCombo = new[] { 7411u },
    };

    /// <summary>
    /// 狙击弹
    /// </summary>
    public static BaseAction CleanShot { get; } = new(ActionID.CleanShot)
    {
        OtherIDsCombo = new[] { 7412u },
    };

    /// <summary>
    /// 热冲击
    /// </summary>
    public static BaseAction HeatBlast { get; } = new(ActionID.HeatBlast);

    /// <summary>
    /// 散射
    /// </summary>
    public static BaseAction SpreadShot { get; } = new(ActionID.SpreadShot);

    /// <summary>
    /// 自动弩
    /// </summary>
    public static BaseAction AutoCrossbow { get; } = new(ActionID.AutoCrossbow);

    /// <summary>
    /// 热弹
    /// </summary>
    public static BaseAction HotShot { get; } = new(ActionID.HotShot);

    /// <summary>
    /// 空气锚
    /// </summary>
    public static BaseAction AirAnchor { get; } = new(ActionID.AirAnchor);

    /// <summary>
    /// 钻头
    /// </summary>
    public static BaseAction Drill { get; } = new(ActionID.Drill);

    /// <summary>
    /// 回转飞锯
    /// </summary>
    public static BaseAction ChainSaw { get; } = new(ActionID.ChainSaw);

    /// <summary>
    /// 毒菌冲击
    /// </summary>
    public static BaseAction Bioblaster { get; } = new(ActionID.Bioblaster, isEot: true);

    /// <summary>
    /// 整备
    /// </summary>
    public static BaseAction Reassemble { get; } = new(ActionID.Reassemble)
    {
        BuffsProvide = new StatusID[] { StatusID.Reassemble },
        OtherCheck = b => HaveHostileInRange,
    };

    /// <summary>
    /// 超荷
    /// </summary>
    public static BaseAction Hypercharge { get; } = new(ActionID.Hypercharge)
    {
        OtherCheck = b => !JobGauge.IsOverheated && JobGauge.Heat >= 50,
    };

    /// <summary>
    /// 野火
    /// </summary>
    public static BaseAction Wildfire { get; } = new(ActionID.Wildfire)
    {
        OtherCheck = b => JobGauge.Heat >= 50,
    };

    /// <summary>
    /// 虹吸弹
    /// </summary>
    public static BaseAction GaussRound { get; } = new(ActionID.GaussRound);

    /// <summary>
    /// 弹射
    /// </summary>
    public static BaseAction Ricochet { get; } = new(ActionID.Ricochet);

    /// <summary>
    /// 枪管加热
    /// </summary>
    public static BaseAction BarrelStabilizer { get; } = new(ActionID.BarrelStabilizer)
    {
        OtherCheck = b => JobGauge.Heat <= 50 && !IsLastWeaponSkill(false, ChainSaw),
    };

    /// <summary>
    /// 车式浮空炮塔
    /// </summary>
    public static BaseAction RookAutoturret { get; } = new(ActionID.RookAutoturret)
    {
        OtherCheck = b => JobGauge.Battery >= 50 && !JobGauge.IsRobotActive,
    };

    /// <summary>
    /// 策动
    /// </summary>
    public static BaseAction Tactician { get; } = new(ActionID.Tactician, true)
    {
        BuffsProvide = new[]
        {
                    StatusID.Troubadour,
                    StatusID.Tactician1,
                    StatusID.Tactician2,
                    StatusID.ShieldSamba,
            },
    };
}
