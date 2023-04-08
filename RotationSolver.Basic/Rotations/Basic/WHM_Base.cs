namespace RotationSolver.Basic.Rotations.Basic;

public abstract class WHM_Base : CustomRotation
{
    private static WHMGauge JobGauge => Service.JobGauges.Get<WHMGauge>();
    public override MedicineType MedicineType => MedicineType.Mind;

    protected static byte Lily => JobGauge.Lily;

    protected static byte BloodLily => JobGauge.BloodLily;

    protected static bool LilyAfter(float time)
    {
        return EndAfter(JobGauge.LilyTimer / 1000f, time);
    }

    protected static bool LilyAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.LilyTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.WhiteMage, ClassJobID.Conjurer };
    private sealed protected override IBaseAction Raise => Raise1;

    #region Heal
    public static IBaseAction Cure { get; } = new BaseAction(ActionID.Cure, true, isTimeline: true);

    public static IBaseAction Medica { get; } = new BaseAction(ActionID.Medica, true, isTimeline: true);

    public static IBaseAction Raise1 { get; } = new BaseAction(ActionID.Raise1, true);

    public static IBaseAction Cure2 { get; } = new BaseAction(ActionID.Cure2, true, isTimeline: true);

    public static IBaseAction Medica2 { get; } = new BaseAction(ActionID.Medica2, true, isEot: true, isTimeline: true)
    {
        StatusProvide = new[] { StatusID.Medica2, StatusID.TrueMedica2 },
    };

    public static IBaseAction Regen { get; } = new BaseAction(ActionID.Regen, true, isEot: true, isTimeline: true)
    {
        TargetStatus = new[]
        {
            StatusID.Regen1,
            StatusID.Regen2,
            StatusID.Regen3,
        }
    };

    public static IBaseAction Cure3 { get; } = new BaseAction(ActionID.Cure3, true, shouldEndSpecial: true, isTimeline: true);

    public static IBaseAction Benediction { get; } = new BaseAction(ActionID.Benediction, true, isTimeline: true);

    public static IBaseAction Asylum { get; } = new BaseAction(ActionID.Asylum, true, isTimeline: true);

    public static IBaseAction AfflatusSolace { get; } = new BaseAction(ActionID.AfflatusSolace, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    public static IBaseAction Tetragrammaton { get; } = new BaseAction(ActionID.Tetragrammaton, true, isTimeline: true);

    public static IBaseAction DivineBenison { get; } = new BaseAction(ActionID.DivineBenison, true, isTimeline: true)
    {
        StatusProvide = new StatusID[] { StatusID.DivineBenison },
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction AfflatusRapture { get; } = new BaseAction(ActionID.AfflatusRapture, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Lily > 0,
    };

    public static IBaseAction Aquaveil { get; } = new BaseAction(ActionID.Aquaveil, true, isTimeline: true);

    public static IBaseAction LiturgyOfTheBell { get; } = new BaseAction(ActionID.LiturgyOfTheBell, true, isTimeline: true);
    #endregion

    #region Attack
    public static IBaseAction Stone { get; } = new BaseAction(ActionID.Stone);

    public static IBaseAction Aero { get; } = new BaseAction(ActionID.Aero, isEot: true)
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
    public static IBaseAction PresenseOfMind { get; } = new BaseAction(ActionID.PresenseOfMind, true)
    {
        ActionCheck = b => !IsMoving
    };

    public static IBaseAction ThinAir { get; } = new BaseAction(ActionID.ThinAir, true);

    public static IBaseAction PlenaryIndulgence { get; } = new BaseAction(ActionID.PlenaryIndulgence, true, isTimeline: true);

    public static IBaseAction Temperance { get; } = new BaseAction(ActionID.Temperance, true, isTimeline: true);
    #endregion
}