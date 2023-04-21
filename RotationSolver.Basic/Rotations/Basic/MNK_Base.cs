namespace RotationSolver.Basic.Rotations.Basic;

public abstract class MNK_Base : CustomRotation
{
    private static MNKGauge JobGauge => Service.JobGauges.Get<MNKGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    protected static BeastChakra[] BeastChakras => JobGauge.BeastChakra;

    protected static byte Chakra => JobGauge.Chakra;

    protected static bool HasSolar => (JobGauge.Nadi & Nadi.SOLAR) != 0;
    protected static bool HasLunar => (JobGauge.Nadi & Nadi.LUNAR) != 0;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Monk, ClassJobID.Pugilist };

    public static IBaseAction DragonKick { get; } = new BaseAction(ActionID.DragonKick)
    {
        StatusProvide = new[] { StatusID.LeadenFist },
    };

    public static IBaseAction BootShine { get; } = new BaseAction(ActionID.BootShine);

    public static IBaseAction ArmOfTheDestroyer { get; } = new BaseAction(ActionID.ArmOfTheDestroyer);

    public static IBaseAction ShadowOfTheDestroyer { get; } = new BaseAction(ActionID.ShadowOfTheDestroyer);

    public static IBaseAction TwinSnakes { get; } = new BaseAction(ActionID.TwinSnakes, ActionOption.Dot);

    public static IBaseAction TrueStrike { get; } = new BaseAction(ActionID.TrueStrike);

    public static IBaseAction FourPointFury { get; } = new BaseAction(ActionID.FourPointFury);

    public static IBaseAction Demolish { get; } = new BaseAction(ActionID.Demolish, ActionOption.Dot)
    {
        TargetStatus = new StatusID[] { StatusID.Demolish },
        GetDotGcdCount = () => 3,
    };

    public static IBaseAction SnapPunch { get; } = new BaseAction(ActionID.SnapPunch);

    public static IBaseAction RockBreaker { get; } = new BaseAction(ActionID.RockBreaker);

    public static IBaseAction Meditation { get; } = new BaseAction(ActionID.Meditation, ActionOption.Buff);

    public static IBaseAction SteelPeak { get; } = new BaseAction(ActionID.SteelPeak)
    {
        ActionCheck = b => InCombat && Chakra == 5,
    };

    public static IBaseAction HowlingFist { get; } = new BaseAction(ActionID.HowlingFist)
    {
        ActionCheck = SteelPeak.ActionCheck,
    };

    public static IBaseAction Brotherhood { get; } = new BaseAction(ActionID.Brotherhood, ActionOption.Buff);

    public static IBaseAction RiddleOfFire { get; } = new BaseAction(ActionID.RiddleOfFire, ActionOption.Buff);

    public static IBaseAction Thunderclap { get; } = new BaseAction(ActionID.Thunderclap, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Mantra { get; } = new BaseAction(ActionID.Mantra, ActionOption.Heal);

    public static IBaseAction PerfectBalance { get; } = new BaseAction(ActionID.PerfectBalance)
    {
        ActionCheck = b => InCombat,
    };

    public static IBaseAction ElixirField { get; } = new BaseAction(ActionID.ElixirField);

    public static IBaseAction FlintStrike { get; } = new BaseAction(ActionID.FlintStrike);

    public static IBaseAction CelestialRevolution { get; } = new BaseAction(ActionID.CelestialRevolution);

    public static IBaseAction RisingPhoenix { get; } = new BaseAction(ActionID.RisingPhoenix);

    public static IBaseAction TornadoKick { get; } = new BaseAction(ActionID.TornadoKick);
    public static IBaseAction PhantomRush { get; } = new BaseAction(ActionID.PhantomRush);

    public static IBaseAction FormShift { get; } = new BaseAction(ActionID.FormShift, ActionOption.Buff)
    {
        StatusProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    public static IBaseAction RiddleOfEarth { get; } = new BaseAction(ActionID.RiddleOfEarth, ActionOption.Defense)
    {
        StatusProvide = new[] { StatusID.RiddleOfEarth },
    };

    public static IBaseAction RiddleOfWind { get; } = new BaseAction(ActionID.RiddleOfWind, ActionOption.Buff);

    [RotationDesc(ActionID.Thunderclap)]
    protected sealed override bool MoveForwardAbility(out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (Thunderclap.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option | CanUseOption.IgnoreClippingCheck)) return true;
        return false;
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Mantra)]
    protected sealed override bool HealAreaAbility(out IAction act)
    {
        if (Mantra.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.RiddleOfEarth)]
    protected sealed override bool DefenseSingleAbility(out IAction act)
    {
        if (RiddleOfEarth.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        return false;
    }
}
