using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of SGE.
/// </summary>
public abstract class SGE_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.SGE };

    #region Job Gauge
    static SGEGauge JobGauge => Svc.Gauges.Get<SGEGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static bool HasEukrasia => JobGauge.Eukrasia;

    /// <summary>
    /// 
    /// </summary>
    public static byte Addersgall => JobGauge.Addersgall;

    /// <summary>
    /// 
    /// </summary>
    public static byte Addersting => JobGauge.Addersting;

    static float AddersgallTimerRaw => JobGauge.AddersgallTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float AddersgallTime => AddersgallTimerRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfter(float time) => AddersgallTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, float offset = 0)
        => AddersgallEndAfter(GCDTime(gctCount, offset));
    #endregion

    #region Attack
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Dosis { get; } = new BaseAction(ActionID.Dosis);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EukrasianDosis { get; } = new BaseAction(ActionID.EukrasianDosis, ActionOption.Dot)
    {
        TargetStatus =
        [
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosisIi,
             StatusID.EukrasianDosisIii
        ],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Dyskrasia { get; } = new BaseAction(ActionID.Dyskrasia);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Phlegma { get; } = new BaseAction(ActionID.Phlegma);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Phlegma2 { get; } = new BaseAction(ActionID.Phlegma2);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Phlegma3 { get; } = new BaseAction(ActionID.Phlegma3);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Toxikon { get; } = new BaseAction(ActionID.Toxikon)
    {
        ActionCheck = (b, m) => Addersting > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Rhizomata { get; } = new BaseAction(ActionID.Rhizomata)
    {
        ActionCheck = (b, m) => Addersgall < 3,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Pneuma { get; } = new BaseAction(ActionID.Pneuma);
    #endregion

    #region Heal
    private protected sealed override IBaseAction Raise => Egeiro;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Egeiro { get; } = new BaseAction(ActionID.Egeiro, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Diagnosis { get; } = new BaseAction(ActionID.Diagnosis, ActionOption.Heal);

    static RandomDelay noTankDelay = new(() => (3, 5));
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Kardia { get; } = new BaseAction(ActionID.Kardia, ActionOption.Heal)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            if (noTankDelay.Delay(!targets.Any()))
            {
                targets = targets.Any() ? targets : Targets;
            }

            if (!targets.Any()) return null;

            return TargetFilter.FindAttackedTarget(targets, mustUse);
        },

        TargetStatus = [StatusID.Kardion],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Prognosis { get; } = new BaseAction(ActionID.Prognosis, ActionOption.Heal | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Physis { get; } = new BaseAction(ActionID.Physis, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Physis2 { get; } = new BaseAction(ActionID.Physis2, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Eukrasia { get; } = new BaseAction(ActionID.Eukrasia, ActionOption.Heal)
    {
        ActionCheck = (b, m) => !HasEukrasia,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Soteria { get; } = new BaseAction(ActionID.Soteria, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Kerachole { get; } = new BaseAction(ActionID.Kerachole, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Addersgall > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ixochole { get; } = new BaseAction(ActionID.Ixochole, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Addersgall > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Zoe { get; } = new BaseAction(ActionID.Zoe, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Taurochole { get; } = new BaseAction(ActionID.Taurochole, ActionOption.Heal | ActionOption.UseResources)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = (b, m) => Addersgall > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Druochole { get; } = new BaseAction(ActionID.Druochole, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Addersgall > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Pepsis { get; } = new BaseAction(ActionID.Pepsis, ActionOption.Heal)
    {
        ActionCheck = (b, m) =>
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

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Haima { get; } = new BaseAction(ActionID.Haima, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EukrasianDiagnosis { get; } = new BaseAction(ActionID.EukrasianDiagnosis, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EukrasianPrognosis { get; } = new BaseAction(ActionID.EukrasianPrognosis, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Holos { get; } = new BaseAction(ActionID.Holos, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Panhaima { get; } = new BaseAction(ActionID.Panhaima, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Krasis { get; } = new BaseAction(ActionID.Krasis, ActionOption.Heal);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Icarus { get; } = new BaseAction(ActionID.Icarus, ActionOption.EndSpecial)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend { get; } = new BaseTrait(368);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(369);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait AddersgallTrait { get; } = new BaseTrait(370);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait SomanouticOath { get; } = new BaseTrait(371);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait SomanouticOath2 { get; } = new BaseTrait(372);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait AdderstingTrait { get; } = new BaseTrait(373);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait OffensiveMagicMastery { get; } = new BaseTrait(374);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedKerachole { get; } = new BaseTrait(375);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait OffensiveMagicMastery2 { get; } = new BaseTrait(376);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedHealingMagic { get; } = new BaseTrait(377);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedZoe { get; } = new BaseTrait(378);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait PhysisMastery { get; } = new BaseTrait(510);
    #endregion

    private protected override IBaseAction LimitBreak => TechneMakre;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction TechneMakre { get; } = new BaseAction(ActionID.TechneMakre, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <inheritdoc/>
    [RotationDesc(ActionID.Icarus)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (Icarus.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }
}