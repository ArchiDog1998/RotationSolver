using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of SCH.
/// </summary>
public abstract class SCH_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new[] { Job.SCH };

    #region Job Gauge
    static SCHGauge JobGauge => Svc.Gauges.Get<SCHGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte FairyGauge => JobGauge.FairyGauge;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasAetherflow => JobGauge.Aetherflow > 0;

    static float SeraphTimeRaw => JobGauge.SeraphTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float SeraphTime => SeraphTimeRaw - DataCenter.WeaponRemain;
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Resurrection;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Adloquium { get; } = new BaseAction(ActionID.Adloquium, ActionOption.Heal)
    {
        ActionCheck = (b, m) => !b.HasStatus(false, StatusID.EukrasianDiagnosis,
            StatusID.EukrasianPrognosis, StatusID.Galvanize),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.Resurrection, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Succor { get; } = new BaseAction(ActionID.Succor, ActionOption.Heal)
    {
        StatusProvide = new[] { StatusID.Galvanize },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Lustrate { get; } = new BaseAction(ActionID.Lustrate, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SacredSoil { get; } = new BaseAction(ActionID.SacredSoil, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflow && !IsMoving,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Indomitability { get; } = new BaseAction(ActionID.Indomitability, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Excogitation { get; } = new BaseAction(ActionID.Excogitation, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Consolation { get; } = new BaseAction(ActionID.Consolation, ActionOption.Heal)
    {
        ActionCheck = (b, m) => SeraphTime > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Protraction { get; } = new BaseAction(ActionID.Protraction, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };
    #endregion

    #region Attack
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Bio { get; } = new BaseAction(ActionID.Bio, ActionOption.Dot)
    {
        TargetStatus = [StatusID.Bio, StatusID.BioIi, StatusID.Biolysis],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.Ruin);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ruin2 { get; } = new BaseAction(ActionID.Ruin2);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrain, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ArtOfWar { get; } = new BaseAction(ActionID.ArtOfWar);
    #endregion

    #region Seraph
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonSeraph { get; } = new BaseAction(ActionID.SummonSeraph, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction SummonEos { get; } = new BaseAction(ActionID.SummonEos)//夕月召唤 17216
    {
        ActionCheck = (b, m) => !DataCenter.HasPet && (!Player.HasStatus(true, StatusID.Dissipation) || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction WhisperingDawn { get; } = new BaseAction(ActionID.WhisperingDawn, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FeyIllumination { get; } = new BaseAction(ActionID.FeyIllumination, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Dissipation { get; } = new BaseAction(ActionID.Dissipation)
    {
        StatusProvide = new[] { StatusID.Dissipation },
        ActionCheck = (b, m) => !HasAetherflow && SeraphTime <= 0 && InCombat && DataCenter.HasPet,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Aetherpact { get; } = new BaseAction(ActionID.Aetherpact, ActionOption.Heal)
    {
        ActionCheck = (b, m) => JobGauge.FairyGauge >= 10 && DataCenter.HasPet && SeraphTime <= 0
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction FeyBlessing { get; } = new BaseAction(ActionID.FeyBlessing, ActionOption.Heal)
    {
        ActionCheck = (b, m) => SeraphTime <= 0 && DataCenter.HasPet,
    };
    #endregion

    #region Others
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Aetherflow { get; } = new BaseAction(ActionID.Aetherflow)
    {
        ActionCheck = (b, m) => InCombat && !HasAetherflow
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Recitation { get; } = new BaseAction(ActionID.Recitation, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ChainStratagem { get; } = new BaseAction(ActionID.ChainStratagem)
    {
        ActionCheck = (b, m) => InCombat && IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DeploymentTactics { get; } = new BaseAction(ActionID.DeploymentTactics, ActionOption.Heal)
    {
        ChoiceTarget = (friends, mustUse) =>
        {
            foreach (var friend in friends)
            {
                if (friend.HasStatus(true, StatusID.Galvanize)) return friend;
            }
            return null;
        },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction EmergencyTactics { get; } = new BaseAction(ActionID.EmergencyTactics, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Expedient { get; } = new BaseAction(ActionID.Expedient, ActionOption.Heal);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BroilMastery2 { get; } = new BaseTrait(184);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BroilMastery { get; } = new BaseTrait(214);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CorruptionMastery2 { get; } = new BaseTrait(311);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BroilMastery3 { get; } = new BaseTrait(312);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedSacredSoil { get; } = new BaseTrait(313);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait CorruptionMastery { get; } = new BaseTrait(324);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait BroilMastery4 { get; } = new BaseTrait(491);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait ArtOfWarMastery { get; } = new BaseTrait(492);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedHealingMagic { get; } = new BaseTrait(493);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedDeploymentTactics { get; } = new BaseTrait(494);
    #endregion

    private protected override IBaseAction LimitBreak => AngelFeathers;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction AngelFeathers { get; } = new BaseAction(ActionID.AngelFeathers, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };

    /// <inheritdoc/>
    protected override bool SpeedAbility(out IAction act)
    {
        if (InCombat && Expedient.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.SpeedAbility(out act);
    }
}
