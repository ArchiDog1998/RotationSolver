namespace RotationSolver.Basic.Rotations.Basic;

public interface INinAction : IBaseAction
{
    IBaseAction[] Ninjutsu { get; }
}

public abstract class NIN_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Dexterity;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Ninja, ClassJobID.Rogue };

    #region Job Gauge
    static NINGauge JobGauge => Service.JobGauges.Get<NINGauge>();

    [Obsolete("Better not use this.")]
    protected static bool InHuton => HutonTime > 0;

    protected static byte Ninki => JobGauge.Ninki;

    protected static float HutonTime => JobGauge.HutonTimer / 1000f;

    protected static bool HutonEndAfter(float time) => EndAfter(HutonTime, time);

    protected static bool HutonEndAfterGCD(uint gctCount = 0, float offset = 0)
        => EndAfterGCD(HutonTime, gctCount, offset);
    #endregion



    #region Attack Single
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction SpinningEdge { get; } = new BaseAction(ActionID.SpinningEdge);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction GustSlash { get; } = new BaseAction(ActionID.GustSlash);

    /// <summary>
    /// 3
    /// </summary>
    public static IBaseAction AeolianEdge { get; } = new BaseAction(ActionID.AeolianEdge);

    /// <summary>
    /// 4
    /// </summary>
    public static IBaseAction ArmorCrush { get; } = new BaseAction(ActionID.ArmorCrush)
    {
        ActionCheck = b => HutonEndAfter(25) && !HutonEndAfterGCD(),
    };

    public static IBaseAction Huraijin { get; } = new BaseAction(ActionID.Huraijin)
    {
        ActionCheck = b => HutonEndAfterGCD(),
    };

    public static IBaseAction PhantomKamaitachi { get; } = new BaseAction(ActionID.PhantomKamaitachi)
    {
        StatusNeed = new[] { StatusID.PhantomKamaitachiReady },
    };

    public static IBaseAction ThrowingDagger { get; } = new BaseAction(ActionID.ThrowingDagger)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Assassinate { get; } = new BaseAction(ActionID.Assassinate);
    public static IBaseAction DreamWithinADream { get; } = new BaseAction(ActionID.DreamWithInADream);

    public static IBaseAction Bhavacakra { get; } = new BaseAction(ActionID.Bhavacakra)
    {
        ActionCheck = b => Ninki >= 50,
    };

    #endregion

    #region Attack Area
    /// <summary>
    /// 1
    /// </summary>
    public static IBaseAction DeathBlossom { get; } = new BaseAction(ActionID.DeathBlossom);

    /// <summary>
    /// 2
    /// </summary>
    public static IBaseAction HakkeMujinsatsu { get; } = new BaseAction(ActionID.HakkeMujinsatsu);

    public static IBaseAction HellfrogMedium { get; } = new BaseAction(ActionID.HellFrogMedium)
    {
        ActionCheck = Bhavacakra.ActionCheck,
    };
    #endregion

    #region Support
    public static IBaseAction Meisui { get; } = new BaseAction(ActionID.Meisui)
    {
        StatusNeed = new[] { StatusID.Suiton },
        ActionCheck = b => JobGauge.Ninki <= 50,
    };
    public static IBaseAction Hide { get; } = new BaseAction(ActionID.Hide, ActionOption.Buff);
    public static IBaseAction ShadeShift { get; } = new BaseAction(ActionID.ShadeShift, ActionOption.Defense);

    public static IBaseAction Mug { get; } = new BaseAction(ActionID.Mug)
    {
        ActionCheck = b => JobGauge.Ninki <= 60,
    };

    public static IBaseAction TrickAttack { get; } = new BaseAction(ActionID.TrickAttack)
    {
        StatusNeed = new StatusID[] { StatusID.Suiton, StatusID.Hidden },
    };

    public static IBaseAction Bunshin { get; } = new BaseAction(ActionID.Bunshin)
    {
        ActionCheck = Bhavacakra.ActionCheck,
    };
    #endregion


    #region Ninjutsu
    public static IBaseAction Ten { get; } = new BaseAction(ActionID.Ten, ActionOption.Buff);

    public static IBaseAction Chi { get; } = new BaseAction(ActionID.Chi, ActionOption.Buff);

    public static IBaseAction Jin { get; } = new BaseAction(ActionID.Jin, ActionOption.Buff);

    public static IBaseAction TenChiJin { get; } = new BaseAction(ActionID.TenChiJin)
    {
        StatusProvide = new[] { StatusID.Kassatsu, StatusID.TenChiJin },
        ActionCheck = b => !HutonEndAfterGCD(),
    };

    public static IBaseAction FumaShurikenTen { get; } = new BaseAction(ActionID.FumaShurikenTen);

    public static IBaseAction FumaShurikenJin { get; } = new BaseAction(ActionID.FumaShurikenJin);

    public static IBaseAction KatonTen { get; } = new BaseAction(ActionID.KatonTen);

    public static IBaseAction RaitonChi { get; } = new BaseAction(ActionID.RaitonChi);

    public static IBaseAction DotonChi { get; } = new BaseAction(ActionID.DotonChi);

    public static IBaseAction SuitonJin { get; } = new BaseAction(ActionID.SuitonJin);

    public static IBaseAction Kassatsu { get; } = new BaseAction(ActionID.Kassatsu)
    {
        StatusProvide = TenChiJin.StatusProvide,
    };

    public class NinAction : BaseAction, INinAction
    {
        public IBaseAction[] Ninjutsu { get; }

        internal NinAction(ActionID actionID, params IBaseAction[] ninjutsu)
            : base(actionID)
        {
            Ninjutsu = ninjutsu;
        }
    }

    public static INinAction RabbitMedium { get; } = new NinAction(ActionID.RabbitMedium);

    public static INinAction FumaShuriken { get; } = new NinAction(ActionID.FumaShuriken, Ten);

    public static INinAction Katon { get; } = new NinAction(ActionID.Katon, Chi, Ten);

    public static INinAction Raiton { get; } = new NinAction(ActionID.Raiton, Ten, Chi);

    public static INinAction Hyoton { get; } = new NinAction(ActionID.Hyoton, Ten, Jin);

    public static INinAction Huton { get; } = new NinAction(ActionID.Huton, Jin, Chi, Ten)
    {
        ActionCheck = b => HutonEndAfterGCD(),
    };

    public static INinAction Doton { get; } = new NinAction(ActionID.Doton, Jin, Ten, Chi)
    {
        StatusProvide = new[] { StatusID.Doton },
    };

    public static INinAction Suiton { get; } = new NinAction(ActionID.Suiton, Ten, Chi, Jin)
    {
        StatusProvide = new[] { StatusID.Suiton },
    };

    public static INinAction GokaMekkyaku { get; } = new NinAction(ActionID.GokaMekkyaku, Chi, Ten);

    public static INinAction HyoshoRanryu { get; } = new NinAction(ActionID.HyoshoRanryu, Ten, Jin);

    public static IBaseAction FleetingRaiju { get; } = new BaseAction(ActionID.FleetingRaiju)
    {
        StatusNeed = new[] { StatusID.RaijuReady },
    };

    public static IBaseAction ForkedRaiju { get; } = new BaseAction(ActionID.ForkedRaiju)
    {
        StatusNeed = FleetingRaiju.StatusNeed,
    };
    #endregion

    public static IBaseAction Shukuchi { get; } = new BaseAction(ActionID.Shukuchi, ActionOption.EndSpecial);


    [RotationDesc(ActionID.Shukuchi)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Shukuchi.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    [RotationDesc(ActionID.ShadeShift)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        if (ShadeShift.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }
}
