namespace RotationSolver.Basic.Rotations.Basic;

public abstract class WHM_Base : CustomRotation
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.WhiteMage, ClassJobID.Conjurer };
    public override MedicineType MedicineType => MedicineType.Mind;

    #region Job Gauge
    private static WHMGauge JobGauge => Service.JobGauges.Get<WHMGauge>();

    protected static byte Lily => JobGauge.Lily;

    protected static byte BloodLily => JobGauge.BloodLily;

    protected static bool LilyAfter(float time)
    {
        return EndAfter(JobGauge.LilyTimer / 1000f, time);
    }

    protected static bool LilyAfterGCD(uint gctCount = 0, float offset = 0)
    {
        return EndAfterGCD(JobGauge.LilyTimer / 1000f, gctCount, offset);
    }
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Raise1;
    public static IBaseAction Raise1 { get; } = new BaseAction(ActionID.Raise1, ActionOption.Friendly);

    public static IBaseAction Cure { get; } = new BaseAction(ActionID.Cure, ActionOption.Heal);

    public static IBaseAction Medica { get; } = new BaseAction(ActionID.Medica, ActionOption.Heal);


    public static IBaseAction Cure2 { get; } = new BaseAction(ActionID.Cure2, ActionOption.Heal);

    public static IBaseAction Medica2 { get; } = new BaseAction(ActionID.Medica2, ActionOption.Hot)
    {
        StatusProvide = new[] { StatusID.Medica2, StatusID.TrueMedica2 },
    };

    public static IBaseAction Regen { get; } = new BaseAction(ActionID.Regen, ActionOption.Hot)
    {
        TargetStatus = new[]
        {
            StatusID.Regen1,
            StatusID.Regen2,
            StatusID.Regen3,
        }
    };

    public static IBaseAction Cure3 { get; } = new BaseAction(ActionID.Cure3, ActionOption.Heal | ActionOption.EndSpecial);

    public static IBaseAction Benediction { get; } = new BaseAction(ActionID.Benediction, ActionOption.Heal);

    public static IBaseAction Asylum { get; } = new BaseAction(ActionID.Asylum, ActionOption.Heal);

    public static IBaseAction AfflatusSolace { get; } = new BaseAction(ActionID.AfflatusSolace, ActionOption.Heal)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    public static IBaseAction Tetragrammaton { get; } = new BaseAction(ActionID.Tetragrammaton, ActionOption.Heal);

    public static IBaseAction DivineBenison { get; } = new BaseAction(ActionID.DivineBenison, ActionOption.Heal)
    {
        StatusProvide = new StatusID[] { StatusID.DivineBenison },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction AfflatusRapture { get; } = new BaseAction(ActionID.AfflatusRapture, ActionOption.Heal)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    public static IBaseAction Aquaveil { get; } = new BaseAction(ActionID.Aquaveil, ActionOption.Heal);

    public static IBaseAction LiturgyOfTheBell { get; } = new BaseAction(ActionID.LiturgyOfTheBell, ActionOption.Heal);
    #endregion

    #region Attack
    public static IBaseAction Stone { get; } = new BaseAction(ActionID.Stone);

    public static IBaseAction Aero { get; } = new BaseAction(ActionID.Aero, ActionOption.Dot)
    {
        TargetStatus = new StatusID[]
        {
            StatusID.Aero,
            StatusID.Aero2,
            StatusID.Dia,
        }
    };

    public static IBaseAction Holy { get; } = new BaseAction(ActionID.Holy);

    public static IBaseAction Assize { get; } = new BaseAction(ActionID.Assize);

    public static IBaseAction AfflatusMisery { get; } = new BaseAction(ActionID.AfflatusMisery)
    {
        ActionCheck = b => JobGauge.BloodLily == 3,
    };
    #endregion

    #region buff
    public static IBaseAction PresenseOfMind { get; } = new BaseAction(ActionID.PresenseOfMind, ActionOption.Buff)
    {
        ActionCheck = b => !IsMoving
    };

    public static IBaseAction ThinAir { get; } = new BaseAction(ActionID.ThinAir, ActionOption.Buff);

    public static IBaseAction PlenaryIndulgence { get; } = new BaseAction(ActionID.PlenaryIndulgence, ActionOption.Heal);

    public static IBaseAction Temperance { get; } = new BaseAction(ActionID.Temperance, ActionOption.Heal);
    #endregion
}