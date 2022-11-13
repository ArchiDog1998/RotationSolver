using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class NINCombo_Base<TCmd> : JobGaugeCombo<NINGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Ninja, ClassJobID.Rogue };
    public class NinAction : BaseAction
    {
        internal BaseAction[] Ninjutsus { get; }
        internal NinAction(uint actionID, params BaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    /// <summary>
    /// 隐遁
    /// </summary>
    public static BaseAction Hide { get; } = new(ActionID.Hide);

    /// <summary>
    /// 双刃旋
    /// </summary>
    public static BaseAction SpinningEdge { get; } = new(ActionID.SpinningEdge);

    /// <summary>
    /// 残影
    /// </summary>
    public static BaseAction ShadeShift { get; } = new(ActionID.ShadeShift);

    /// <summary>
    /// 绝风
    /// </summary>
    public static BaseAction GustSlash { get; } = new(ActionID.GustSlash);

    /// <summary>
    /// 飞刀
    /// </summary>
    public static BaseAction ThrowingDagger { get; } = new(ActionID.ThrowingDagger);

    /// <summary>
    /// 夺取
    /// </summary>
    public static BaseAction Mug { get; } = new(ActionID.Mug)
    {
        OtherCheck = b => JobGauge.Ninki <= 50,
    };

    /// <summary>
    /// 攻其不备
    /// </summary>
    public static BaseAction TrickAttack { get; } = new(ActionID.TrickAttack)
    {
        BuffsNeed = new StatusID[] { StatusID.Suiton, StatusID.Hidden },
    };

    /// <summary>
    /// 旋风刃
    /// </summary>
    public static BaseAction AeolianEdge { get; } = new(ActionID.AeolianEdge);

    /// <summary>
    /// 血雨飞花
    /// </summary>
    public static BaseAction DeathBlossom { get; } = new(ActionID.DeathBlossom);

    /// <summary>
    /// 天之印
    /// </summary>
    public static BaseAction Ten { get; } = new(ActionID.Ten);

    /// <summary>
    /// 地之印
    /// </summary>
    public static BaseAction Chi { get; } = new(ActionID.Chi);

    /// <summary>
    /// 人之印
    /// </summary>
    public static BaseAction Jin { get; } = new(ActionID.Jin);

    /// <summary>
    /// 天地人
    /// </summary>
    public static BaseAction TenChiJin { get; } = new(ActionID.TenChiJin)
    {
        BuffsProvide = new[] { StatusID.Kassatsu, StatusID.TenChiJin },
        OtherCheck = b => JobGauge.HutonTimer > 0,
    };

    /// <summary>
    /// 缩地
    /// </summary>
    public static BaseAction Shukuchi { get; } = new(ActionID.Shukuchi, true);

    /// <summary>
    /// 断绝
    /// </summary>
    public static BaseAction Assassinate { get; } = new(ActionID.Assassinate);

    /// <summary>
    /// 命水
    /// </summary>
    public static BaseAction Meisui { get; } = new(ActionID.Meisui)
    {
        BuffsNeed = new[] { StatusID.Suiton },
        OtherCheck = b => JobGauge.Ninki <= 50,
    };

    /// <summary>
    /// 生杀予夺
    /// </summary>
    public static BaseAction Kassatsu { get; } = new(ActionID.Kassatsu, isFriendly: true)
    {
        BuffsProvide = new[] { StatusID.Kassatsu, StatusID.TenChiJin },
    };

    /// <summary>
    /// 八卦无刃杀
    /// </summary>
    public static BaseAction HakkeMujinsatsu { get; } = new(ActionID.HakkeMujinsatsu);

    /// <summary>
    /// 强甲破点突
    /// </summary>
    public static BaseAction ArmorCrush { get; } = new(ActionID.ArmorCrush)
    {
        OtherCheck = b => RemainAfter(JobGauge.HutonTimer / 1000f, 29) && JobGauge.HutonTimer > 0,
    };

    /// <summary>
    /// 通灵之术·大虾蟆
    /// </summary>
    public static BaseAction HellfrogMedium { get; } = new(ActionID.HellfrogMedium);

    /// <summary>
    /// 六道轮回
    /// </summary>
    public static BaseAction Bhavacakra { get; } = new(ActionID.Bhavacakra);

    /// <summary>
    /// 分身之术
    /// </summary>
    public static BaseAction Bunshin { get; } = new(ActionID.Bunshin);

    /// <summary>
    /// 残影镰鼬
    /// </summary>
    public static BaseAction PhantomKamaitachi { get; } = new(ActionID.PhantomKamaitachi)
    {
        BuffsNeed = new[] { StatusID.PhantomKamaitachiReady },
    };

    /// <summary>
    /// 月影雷兽牙
    /// </summary>
    public static BaseAction FleetingRaiju { get; } = new(ActionID.FleetingRaiju)
    {
        BuffsNeed = new[] { StatusID.RaijuReady },
    };

    /// <summary>
    /// 月影雷兽爪
    /// </summary>
    public static BaseAction ForkedRaiju { get; } = new(ActionID.ForkedRaiju)
    {
        BuffsNeed = new[] { StatusID.RaijuReady },
    };

    /// <summary>
    /// 风来刃
    /// </summary>
    public static BaseAction Huraijin { get; } = new(ActionID.Huraijin)
    {
        OtherCheck = b => JobGauge.HutonTimer == 0,
    };

    /// <summary>
    /// 梦幻三段
    /// </summary>
    public static BaseAction DreamWithinaDream { get; } = new(ActionID.DreamWithinaDream);

    /// <summary>
    /// 风魔手里剑天
    /// </summary>
    public static BaseAction FumaShurikenTen { get; } = new(ActionID.FumaShurikenTen);

    /// <summary>
    /// 风魔手里剑人
    /// </summary>
    public static BaseAction FumaShurikenJin { get; } = new(ActionID.FumaShurikenJin);

    /// <summary>
    /// 火遁之术天
    /// </summary>
    public static BaseAction KatonTen { get; } = new(ActionID.KatonTen);

    /// <summary>
    /// 雷遁之术地
    /// </summary>
    public static BaseAction RaitonChi { get; } = new(ActionID.RaitonChi);

    /// <summary>
    /// 土遁之术地
    /// </summary>
    public static BaseAction DotonChi { get; } = new(ActionID.DotonChi);

    /// <summary>
    /// 水遁之术人
    /// </summary>
    public static BaseAction SuitonJin { get; } = new(ActionID.SuitonJin);


    /// <summary>
    /// 通灵之术
    /// </summary>
    public static NinAction RabbitMedium { get; } = new(ActionID.RabbitMedium);

    /// <summary>
    /// 风魔手里剑
    /// </summary>
    public static NinAction FumaShuriken { get; } = new(ActionID.FumaShuriken, Ten);

    /// <summary>
    /// 火遁之术
    /// </summary>
    public static NinAction Katon { get; } = new(ActionID.Katon, Chi, Ten);

    /// <summary>
    /// 雷遁之术
    /// </summary>
    public static NinAction Raiton { get; } = new(ActionID.Raiton, Ten, Chi);

    /// <summary>
    /// 冰遁之术
    /// </summary>
    public static NinAction Hyoton { get; } = new(ActionID.Hyoton, Ten, Jin);

    /// <summary>
    /// 风遁之术
    /// </summary>
    public static NinAction Huton { get; } = new(ActionID.Huton, Jin, Chi, Ten)
    {
        OtherCheck = b => JobGauge.HutonTimer == 0,
    };

    /// <summary>
    /// 土遁之术
    /// </summary>
    public static NinAction Doton { get; } = new(ActionID.Doton, Jin, Ten, Chi)
    {
        BuffsProvide = new[] { StatusID.Doton },
    };

    /// <summary>
    /// 水遁之术
    /// </summary>
    public static NinAction Suiton { get; } = new(ActionID.SuitonActionID.Suiton, Ten, Chi, Jin)
    {
        BuffsProvide = new[] { StatusID.Suiton },
        OtherCheck = b => TrickAttack.WillHaveOneChargeGCD(1, 1),
    };

    /// <summary>
    /// 劫火灭却之术
    /// </summary>
    public static NinAction GokaMekkyaku { get; } = new(ActionID.GokaMekkyaku, Chi, Ten);

    /// <summary>
    /// 冰晶乱流之术
    /// </summary>
    public static NinAction HyoshoRanryu { get; } = new(ActionID.HyoshoRanryu, Ten, Jin);

}
