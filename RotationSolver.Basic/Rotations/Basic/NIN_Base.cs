namespace RotationSolver.Basic.Rotations.Basic;

public interface INinAction : IBaseAction
{
    IBaseAction[] Ninjutsu { get; }
}

public abstract class NIN_Base : CustomRotation
{
    private static NINGauge JobGauge => Service.JobGauges.Get<NINGauge>();
    public override MedicineType MedicineType => MedicineType.Dexterity;

    protected static bool InHuton => JobGauge.HutonTimer > 0;

    protected static byte Ninki => JobGauge.Ninki;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Ninja, ClassJobID.Rogue };

    public class NinAction : BaseAction, INinAction
    {
        public IBaseAction[] Ninjutsu { get; }
        internal NinAction(ActionID actionID, params IBaseAction[] ninjutsu)
            : base(actionID, false, false)
        {
            Ninjutsu = ninjutsu;
        }
    }

    public static IBaseAction Hide { get; } = new BaseAction(ActionID.Hide, true);

    public static IBaseAction SpinningEdge { get; } = new BaseAction(ActionID.SpinningEdge);

    public static IBaseAction ShadeShift { get; } = new BaseAction(ActionID.ShadeShift, true);

    public static IBaseAction GustSlash { get; } = new BaseAction(ActionID.GustSlash);

    public static IBaseAction ThrowingDagger { get; } = new BaseAction(ActionID.ThrowingDagger)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction Mug { get; } = new BaseAction(ActionID.Mug)
    {
        ActionCheck = b => JobGauge.Ninki <= 60,
    };

    public static IBaseAction TrickAttack { get; } = new BaseAction(ActionID.TrickAttack)
    {
        StatusNeed = new StatusID[] { StatusID.Suiton, StatusID.Hidden },
    };

    public static IBaseAction AeolianEdge { get; } = new BaseAction(ActionID.AeolianEdge);

    public static IBaseAction DeathBlossom { get; } = new BaseAction(ActionID.DeathBlossom);

    public static IBaseAction Ten { get; } = new BaseAction(ActionID.Ten, true);

    public static IBaseAction Chi { get; } = new BaseAction(ActionID.Chi, true);

    public static IBaseAction Jin { get; } = new BaseAction(ActionID.Jin, true);

    public static IBaseAction TenChiJin { get; } = new BaseAction(ActionID.TenChiJin, true)
    {
        StatusProvide = new[] { StatusID.Kassatsu, StatusID.TenChiJin },
        ActionCheck = b => JobGauge.HutonTimer > 0,
    };

    public static IBaseAction Shukuchi { get; } = new BaseAction(ActionID.Shukuchi, true);

    public static IBaseAction Assassinate { get; } = new BaseAction(ActionID.Assassinate);

    public static IBaseAction Meisui { get; } = new BaseAction(ActionID.Meisui, true)
    {
        StatusNeed = new[] { StatusID.Suiton },
        ActionCheck = b => JobGauge.Ninki <= 50,
    };

    public static IBaseAction Kassatsu { get; } = new BaseAction(ActionID.Kassatsu, true)
    {
        StatusProvide = TenChiJin.StatusProvide,
    };

    public static IBaseAction HakkeMujinsatsu { get; } = new BaseAction(ActionID.HakkeMujinsatsu);

    public static IBaseAction ArmorCrush { get; } = new BaseAction(ActionID.ArmorCrush)
    {
        ActionCheck = b => EndAfter(JobGauge.HutonTimer / 1000f, 29) && JobGauge.HutonTimer > 0,
    };

    public static IBaseAction Bunshin { get; } = new BaseAction(ActionID.Bunshin, true)
    {
        ActionCheck = b => Ninki >= 50,
    };

    public static IBaseAction HellfrogMedium { get; } = new BaseAction(ActionID.HellFrogMedium)
    {
        ActionCheck = Bunshin.ActionCheck,
    };

    public static IBaseAction Bhavacakra { get; } = new BaseAction(ActionID.Bhavacakra)
    {
        ActionCheck = Bunshin.ActionCheck,
    };

    public static IBaseAction PhantomKamaitachi { get; } = new BaseAction(ActionID.PhantomKamaitachi)
    {
        StatusNeed = new[] { StatusID.PhantomKamaitachiReady },
    };

    public static IBaseAction FleetingRaiju { get; } = new BaseAction(ActionID.FleetingRaiju)
    {
        StatusNeed = new[] { StatusID.RaijuReady },
    };

    public static IBaseAction ForkedRaiju { get; } = new BaseAction(ActionID.ForkedRaiju)
    {
        StatusNeed = FleetingRaiju.StatusNeed,
    };

    public static IBaseAction Huraijin { get; } = new BaseAction(ActionID.Huraijin)
    {
        ActionCheck = b => JobGauge.HutonTimer == 0,
    };

    public static IBaseAction DreamWithinADream { get; } = new BaseAction(ActionID.DreamWithInADream);

    public static IBaseAction FumaShurikenTen { get; } = new BaseAction(ActionID.FumaShurikenTen);

    public static IBaseAction FumaShurikenJin { get; } = new BaseAction(ActionID.FumaShurikenJin);

    public static IBaseAction KatonTen { get; } = new BaseAction(ActionID.KatonTen);

    public static IBaseAction RaitonChi { get; } = new BaseAction(ActionID.RaitonChi);

    public static IBaseAction DotonChi { get; } = new BaseAction(ActionID.DotonChi);

    public static IBaseAction SuitonJin { get; } = new BaseAction(ActionID.SuitonJin);

    public static INinAction RabbitMedium { get; } = new NinAction(ActionID.RabbitMedium);

    public static INinAction FumaShuriken { get; } = new NinAction(ActionID.FumaShuriken, Ten);

    public static INinAction Katon { get; } = new NinAction(ActionID.Katon, Chi, Ten);

    public static INinAction Raiton { get; } = new NinAction(ActionID.Raiton, Ten, Chi);

    public static INinAction Hyoton { get; } = new NinAction(ActionID.Hyoton, Ten, Jin);

    public static INinAction Huton { get; } = new NinAction(ActionID.Huton, Jin, Chi, Ten)
    {
        ActionCheck = b => JobGauge.HutonTimer == 0,
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

    [RotationDesc(ActionID.Shukuchi)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Shukuchi.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;

        return false;
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.ShadeShift)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShadeShift.CanUse(out act)) return true;

        return false;
    }
}
