using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Rotations.Basic;

internal interface INinAction : IBaseAction
{
    IBaseAction[] Ninjutsus { get; }
}


internal abstract class NIN_Base : CustomRotation.CustomRotation
{
    private static NINGauge JobGauge => Service.JobGauges.Get<NINGauge>();

    /// <summary>
    /// 在风buff中
    /// </summary>
    protected static bool InHuton => JobGauge.HutonTimer > 0;

    /// <summary>
    /// 忍术点数
    /// </summary>
    protected static byte Ninki => JobGauge.Ninki;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Ninja, ClassJobID.Rogue };


    public class NinAction : BaseAction, INinAction
    {
        public IBaseAction[] Ninjutsus { get; }
        internal NinAction(ActionID actionID, params IBaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    /// <summary>
    /// 隐遁
    /// </summary>
    public static IBaseAction Hide { get; } = new BaseAction(ActionID.Hide, true);

    /// <summary>
    /// 双刃旋
    /// </summary>
    public static IBaseAction SpinningEdge { get; } = new BaseAction(ActionID.SpinningEdge);

    /// <summary>
    /// 残影
    /// </summary>
    public static IBaseAction ShadeShift { get; } = new BaseAction(ActionID.ShadeShift, true);

    /// <summary>
    /// 绝风
    /// </summary>
    public static IBaseAction GustSlash { get; } = new BaseAction(ActionID.GustSlash);

    /// <summary>
    /// 飞刀
    /// </summary>
    public static IBaseAction ThrowingDagger { get; } = new BaseAction(ActionID.ThrowingDagger);

    /// <summary>
    /// 夺取
    /// </summary>
    public static IBaseAction Mug { get; } = new BaseAction(ActionID.Mug)
    {
        ActionCheck = b => JobGauge.Ninki <= 50,
    };

    /// <summary>
    /// 攻其不备
    /// </summary>
    public static IBaseAction TrickAttack { get; } = new BaseAction(ActionID.TrickAttack)
    {
        StatusNeed = new StatusID[] { StatusID.Suiton, StatusID.Hidden },
    };

    /// <summary>
    /// 旋风刃
    /// </summary>
    public static IBaseAction AeolianEdge { get; } = new BaseAction(ActionID.AeolianEdge);

    /// <summary>
    /// 血雨飞花
    /// </summary>
    public static IBaseAction DeathBlossom { get; } = new BaseAction(ActionID.DeathBlossom);

    /// <summary>
    /// 天之印
    /// </summary>
    public static IBaseAction Ten { get; } = new BaseAction(ActionID.Ten, true);

    /// <summary>
    /// 地之印
    /// </summary>
    public static IBaseAction Chi { get; } = new BaseAction(ActionID.Chi, true);

    /// <summary>
    /// 人之印
    /// </summary>
    public static IBaseAction Jin { get; } = new BaseAction(ActionID.Jin, true);

    /// <summary>
    /// 天地人
    /// </summary>
    public static IBaseAction TenChiJin { get; } = new BaseAction(ActionID.TenChiJin, true)
    {
        StatusProvide = new[] { StatusID.Kassatsu, StatusID.TenChiJin },
        ActionCheck = b => JobGauge.HutonTimer > 0,
    };

    /// <summary>
    /// 缩地
    /// </summary>
    public static IBaseAction Shukuchi { get; } = new BaseAction(ActionID.Shukuchi, true);

    /// <summary>
    /// 断绝
    /// </summary>
    public static IBaseAction Assassinate { get; } = new BaseAction(ActionID.Assassinate);

    /// <summary>
    /// 命水
    /// </summary>
    public static IBaseAction Meisui { get; } = new BaseAction(ActionID.Meisui, true)
    {
        StatusNeed = new[] { StatusID.Suiton },
        ActionCheck = b => JobGauge.Ninki <= 50,
    };

    /// <summary>
    /// 生杀予夺
    /// </summary>
    public static IBaseAction Kassatsu { get; } = new BaseAction(ActionID.Kassatsu, true)
    {
        StatusProvide = TenChiJin.StatusProvide,
    };

    /// <summary>
    /// 八卦无刃杀
    /// </summary>
    public static IBaseAction HakkeMujinsatsu { get; } = new BaseAction(ActionID.HakkeMujinsatsu);

    /// <summary>
    /// 强甲破点突
    /// </summary>
    public static IBaseAction ArmorCrush { get; } = new BaseAction(ActionID.ArmorCrush)
    {
        ActionCheck = b => EndAfter(JobGauge.HutonTimer / 1000f, 29) && JobGauge.HutonTimer > 0,
    };

    /// <summary>
    /// 分身之术
    /// </summary>
    public static IBaseAction Bunshin { get; } = new BaseAction(ActionID.Bunshin, true)
    {
        ActionCheck = b => Ninki >= 50,
    };

    /// <summary>
    /// 通灵之术·大虾蟆
    /// </summary>
    public static IBaseAction HellfrogMedium { get; } = new BaseAction(ActionID.HellfrogMedium)
    {
        ActionCheck = Bunshin.ActionCheck,
    };

    /// <summary>
    /// 六道轮回
    /// </summary>
    public static IBaseAction Bhavacakra { get; } = new BaseAction(ActionID.Bhavacakra)
    {
        ActionCheck = Bunshin.ActionCheck,
    };

    /// <summary>
    /// 残影镰鼬
    /// </summary>
    public static IBaseAction PhantomKamaitachi { get; } = new BaseAction(ActionID.PhantomKamaitachi)
    {
        StatusNeed = new[] { StatusID.PhantomKamaitachiReady },
    };

    /// <summary>
    /// 月影雷兽牙
    /// </summary>
    public static IBaseAction FleetingRaiju { get; } = new BaseAction(ActionID.FleetingRaiju)
    {
        StatusNeed = new[] { StatusID.RaijuReady },
    };

    /// <summary>
    /// 月影雷兽爪
    /// </summary>
    public static IBaseAction ForkedRaiju { get; } = new BaseAction(ActionID.ForkedRaiju)
    {
        StatusNeed = FleetingRaiju.StatusNeed,
    };

    /// <summary>
    /// 风来刃
    /// </summary>
    public static IBaseAction Huraijin { get; } = new BaseAction(ActionID.Huraijin)
    {
        ActionCheck = b => JobGauge.HutonTimer == 0,
    };

    /// <summary>
    /// 梦幻三段
    /// </summary>
    public static IBaseAction DreamWithinaDream { get; } = new BaseAction(ActionID.DreamWithinaDream);

    /// <summary>
    /// 风魔手里剑天
    /// </summary>
    public static IBaseAction FumaShurikenTen { get; } = new BaseAction(ActionID.FumaShurikenTen);

    /// <summary>
    /// 风魔手里剑人
    /// </summary>
    public static IBaseAction FumaShurikenJin { get; } = new BaseAction(ActionID.FumaShurikenJin);

    /// <summary>
    /// 火遁之术天
    /// </summary>
    public static IBaseAction KatonTen { get; } = new BaseAction(ActionID.KatonTen);

    /// <summary>
    /// 雷遁之术地
    /// </summary>
    public static IBaseAction RaitonChi { get; } = new BaseAction(ActionID.RaitonChi);

    /// <summary>
    /// 土遁之术地
    /// </summary>
    public static IBaseAction DotonChi { get; } = new BaseAction(ActionID.DotonChi);

    /// <summary>
    /// 水遁之术人
    /// </summary>
    public static IBaseAction SuitonJin { get; } = new BaseAction(ActionID.SuitonJin);


    /// <summary>
    /// 通灵之术
    /// </summary>
    public static INinAction RabbitMedium { get; } = new NinAction(ActionID.RabbitMedium);

    /// <summary>
    /// 风魔手里剑
    /// </summary>
    public static INinAction FumaShuriken { get; } = new NinAction(ActionID.FumaShuriken, Ten);

    /// <summary>
    /// 火遁之术
    /// </summary>
    public static INinAction Katon { get; } = new NinAction(ActionID.Katon, Chi, Ten);

    /// <summary>
    /// 雷遁之术
    /// </summary>
    public static INinAction Raiton { get; } = new NinAction(ActionID.Raiton, Ten, Chi);

    /// <summary>
    /// 冰遁之术
    /// </summary>
    public static INinAction Hyoton { get; } = new NinAction(ActionID.Hyoton, Ten, Jin);

    /// <summary>
    /// 风遁之术
    /// </summary>
    public static INinAction Huton { get; } = new NinAction(ActionID.Huton, Jin, Chi, Ten)
    {
        ActionCheck = b => JobGauge.HutonTimer == 0,
    };

    /// <summary>
    /// 土遁之术
    /// </summary>
    public static INinAction Doton { get; } = new NinAction(ActionID.Doton, Jin, Ten, Chi)
    {
        StatusProvide = new[] { StatusID.Doton },
    };

    /// <summary>
    /// 水遁之术
    /// </summary>
    public static INinAction Suiton { get; } = new NinAction(ActionID.Suiton, Ten, Chi, Jin)
    {
        StatusProvide = new[] { StatusID.Suiton },
        ActionCheck = b => TrickAttack.WillHaveOneChargeGCD(1, 1),
    };

    /// <summary>
    /// 劫火灭却之术
    /// </summary>
    public static INinAction GokaMekkyaku { get; } = new NinAction(ActionID.GokaMekkyaku, Chi, Ten);

    /// <summary>
    /// 冰晶乱流之术
    /// </summary>
    public static INinAction HyoshoRanryu { get; } = new NinAction(ActionID.HyoshoRanryu, Ten, Jin);

    [RotationDesc(ActionID.Shukuchi)]
    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Shukuchi.CanUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    [RotationDesc(ActionID.Feint)]
    private protected sealed override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.ShadeShift)]
    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShadeShift.CanUse(out act)) return true;

        return false;
    }
}
