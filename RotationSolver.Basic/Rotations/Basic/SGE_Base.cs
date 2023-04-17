namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SGE_Base : CustomRotation
{
    private static SGEGauge JobGauge => Service.JobGauges.Get<SGEGauge>();

    protected static bool HasEukrasia => JobGauge.Eukrasia;
    protected static byte Addersgall => JobGauge.Addersgall;

    protected static byte Addersting => JobGauge.Addersting;
    public override MedicineType MedicineType => MedicineType.Mind;

    protected static bool NoOgcds => JobGauge.Addersgall == 0 && (!Physis.EnoughLevel || Physis.IsCoolingDown) && (!Haima.EnoughLevel || Haima.IsCoolingDown) && (!Panhaima.EnoughLevel || Panhaima.IsCoolingDown) && (!Holos.EnoughLevel || Holos.IsCoolingDown) && (!Soteria.EnoughLevel || Soteria.IsCoolingDown) && (!Pneuma.EnoughLevel || Pneuma.IsCoolingDown) && (!Rhizomata.EnoughLevel || Rhizomata.IsCoolingDown) && (!Krasis.EnoughLevel || Krasis.IsCoolingDown) && (!Zoe.EnoughLevel || Zoe.IsCoolingDown);

    protected static bool AddersgallEndAfter(float time)
    {
        return EndAfter(JobGauge.AddersgallTimer / 1000f, time);
    }

    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.AddersgallTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Sage };
    private sealed protected override IBaseAction Raise => Egeiro;

    public static IBaseAction Egeiro { get; } = new BaseAction(ActionID.Egeiro, true);

    public static IBaseAction Dosis { get; } = new BaseAction(ActionID.Dosis);

    public static IBaseAction EukrasianDosis { get; } = new BaseAction(ActionID.EukrasianDosis, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosis2,
             StatusID.EukrasianDosis3
        },
    };

    public static IBaseAction Phlegma { get; } = new BaseAction(ActionID.Phlegma);

    public static IBaseAction Phlegma2 { get; } = new BaseAction(ActionID.Phlegma2);

    public static IBaseAction Phlegma3 { get; } = new BaseAction(ActionID.Phlegma3);

    public static IBaseAction Diagnosis { get; } = new BaseAction(ActionID.Diagnosis, true)
    {
        ActionCheck = b => NoOgcds,
    };

    public static IBaseAction Kardia { get; } = new BaseAction(ActionID.Kardia, true)
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

    public static IBaseAction Prognosis { get; } = new BaseAction(ActionID.Prognosis, true, shouldEndSpecial: true, isTimeline: true)
    {
        ActionCheck = b => NoOgcds,
    };

    public static IBaseAction Physis { get; } = new BaseAction(ActionID.Physis, true, isTimeline: true);

    public static IBaseAction Eukrasia { get; } = new BaseAction(ActionID.Eukrasia, true, isTimeline: true)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    public static IBaseAction Soteria { get; } = new BaseAction(ActionID.Soteria, true, isTimeline: true);

    public static IBaseAction Icarus { get; } = new BaseAction(ActionID.Icarus, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    public static IBaseAction Druochole { get; } = new BaseAction(ActionID.Druochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    public static IBaseAction Dyskrasia { get; } = new BaseAction(ActionID.Dyskrasia);

    public static IBaseAction Kerachole { get; } = new BaseAction(ActionID.Kerachole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    public static IBaseAction Ixochole { get; } = new BaseAction(ActionID.Ixochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    public static IBaseAction Zoe { get; } = new BaseAction(ActionID.Zoe, isTimeline: true);

    public static IBaseAction Taurochole { get; } = new BaseAction(ActionID.Taurochole, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    public static IBaseAction Toxikon { get; } = new BaseAction(ActionID.Toxikon)
    {
        ActionCheck = b => JobGauge.Addersting > 0,
    };

    public static IBaseAction Haima { get; } = new BaseAction(ActionID.Haima, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    public static IBaseAction EukrasianDiagnosis { get; } = new BaseAction(ActionID.EukrasianDiagnosis, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => NoOgcds,
    };

    public static IBaseAction EukrasianPrognosis { get; } = new BaseAction(ActionID.EukrasianPrognosis, true, isTimeline: true)
    {
        ActionCheck = b => NoOgcds,
    };

    public static IBaseAction Rhizomata { get; } = new BaseAction(ActionID.Rhizomata, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall < 3,
    };

    public static IBaseAction Holos { get; } = new BaseAction(ActionID.Holos, true, isTimeline: true)
    {
        ActionCheck = b => (!Panhaima.IsCoolingDown || Panhaima.ElapsedAfter(60)),
    };

    public static IBaseAction Panhaima { get; } = new BaseAction(ActionID.Panhaima, true, isTimeline: true)
    {
        ActionCheck = b => (Holos.IsCoolingDown && Holos.ElapsedAfter(60)),
    };

    public static IBaseAction Krasis { get; } = new BaseAction(ActionID.Krasis, true, isTimeline: true);

    public static IBaseAction Pneuma { get; } = new BaseAction(ActionID.Pneuma, isTimeline: true);

    public static IBaseAction Pepsis { get; } = new BaseAction(ActionID.Pepsis, true, isTimeline: true)
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

    [RotationDesc(ActionID.Icarus)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        //ÉñÒí
        if (Icarus.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }
}
