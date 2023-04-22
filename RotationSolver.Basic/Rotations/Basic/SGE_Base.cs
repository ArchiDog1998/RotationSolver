namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SGE_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Strength;
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Sage };

    #region Job Gauge
    static SGEGauge JobGauge => Service.JobGauges.Get<SGEGauge>();

    protected static bool HasEukrasia => JobGauge.Eukrasia;
    protected static byte Addersgall => JobGauge.Addersgall;

    protected static byte Addersting => JobGauge.Addersting;

    protected static float AddersgallTimer => JobGauge.AddersgallTimer / 1000f;
    protected static bool AddersgallEndAfter(float time) => EndAfter(AddersgallTimer, time);

    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, float offset = 0)
        => EndAfterGCD(AddersgallTimer, gctCount, offset);
    #endregion

    #region Attack
    public static IBaseAction Dosis { get; } = new BaseAction(ActionID.Dosis);

    public static IBaseAction EukrasianDosis { get; } = new BaseAction(ActionID.EukrasianDosis, ActionOption.Dot)
    {
        TargetStatus = new StatusID[]
        {
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosis2,
             StatusID.EukrasianDosis3
        },
    };
    public static IBaseAction Dyskrasia { get; } = new BaseAction(ActionID.Dyskrasia);


    public static IBaseAction Phlegma { get; } = new BaseAction(ActionID.Phlegma);

    public static IBaseAction Phlegma2 { get; } = new BaseAction(ActionID.Phlegma2);

    public static IBaseAction Phlegma3 { get; } = new BaseAction(ActionID.Phlegma3);

    public static IBaseAction Toxikon { get; } = new BaseAction(ActionID.Toxikon)
    {
        ActionCheck = b => Addersting > 0,
    };

    public static IBaseAction Rhizomata { get; } = new BaseAction(ActionID.Rhizomata)
    {
        ActionCheck = b => Addersgall < 3,
    };

    public static IBaseAction Pneuma { get; } = new BaseAction(ActionID.Pneuma);
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Egeiro;
    public static IBaseAction Egeiro { get; } = new BaseAction(ActionID.Egeiro, ActionOption.Friendly);
    public static IBaseAction Diagnosis { get; } = new BaseAction(ActionID.Diagnosis, ActionOption.Heal);

    public static IBaseAction Kardia { get; } = new BaseAction(ActionID.Kardia, ActionOption.Heal)
    {
        StatusProvide = new StatusID[] { StatusID.Kardia },
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            targets = targets.Any() ? targets : Targets;

            if (!targets.Any()) return null;

            return TargetFilter.FindAttackedTarget(targets, mustUse);
        },
        ActionCheck = b => !b.HasStatus(true, StatusID.Kardion),
    };

    public static IBaseAction Prognosis { get; } = new BaseAction(ActionID.Prognosis, ActionOption.Heal | ActionOption.EndSpecial);

    public static IBaseAction Physis { get; } = new BaseAction(ActionID.Physis, ActionOption.Heal);

    public static IBaseAction Eukrasia { get; } = new BaseAction(ActionID.Eukrasia, ActionOption.Heal)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    public static IBaseAction Soteria { get; } = new BaseAction(ActionID.Soteria, ActionOption.Heal);

    public static IBaseAction Kerachole { get; } = new BaseAction(ActionID.Kerachole, ActionOption.Heal)
    {
        ActionCheck = b => Addersgall > 0,
    };

    public static IBaseAction Ixochole { get; } = new BaseAction(ActionID.Ixochole, ActionOption.Heal)
    {
        ActionCheck = b => Addersgall > 0,
    };

    public static IBaseAction Zoe { get; } = new BaseAction(ActionID.Zoe, ActionOption.Heal);

    public static IBaseAction Taurochole { get; } = new BaseAction(ActionID.Taurochole, ActionOption.Heal)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => Addersgall > 0,
    };

    public static IBaseAction Druochole { get; } = new BaseAction(ActionID.Druochole, ActionOption.Heal)
    {
        ActionCheck = b => Addersgall > 0,
    };
    public static IBaseAction Pepsis { get; } = new BaseAction(ActionID.Pepsis, ActionOption.Heal)
    {
        ActionCheck = b =>
        {
            foreach (var chara in DataCenter.PartyMembers)
            {
                if (chara.HasStatus(true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && b.WillStatusEndGCD(2, 0, true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && chara.GetHealthRatio() < 0.9) return true;
            }

            return false;
        },
    };

    public static IBaseAction Haima { get; } = new BaseAction(ActionID.Haima, ActionOption.Heal)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction EukrasianDiagnosis { get; } = new BaseAction(ActionID.EukrasianDiagnosis, ActionOption.Heal)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction EukrasianPrognosis { get; } = new BaseAction(ActionID.EukrasianPrognosis, ActionOption.Heal);

    public static IBaseAction Holos { get; } = new BaseAction(ActionID.Holos, ActionOption.Heal);

    public static IBaseAction Panhaima { get; } = new BaseAction(ActionID.Panhaima, ActionOption.Heal);

    public static IBaseAction Krasis { get; } = new BaseAction(ActionID.Krasis, ActionOption.Heal);
    #endregion

    public static IBaseAction Icarus { get; } = new BaseAction(ActionID.Icarus, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    [RotationDesc(ActionID.Icarus)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Icarus.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }
}
