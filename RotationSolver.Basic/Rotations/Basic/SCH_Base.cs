using Dalamud.Interface.Animation;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SCH_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Mind;
    public sealed override Job[] Jobs => new [] { ECommons.ExcelServices.Job.SCH };

    #region Job Gauge
    static SCHGauge JobGauge => Svc.Gauges.Get<SCHGauge>();

    protected static byte FairyGauge => JobGauge.FairyGauge;

    protected static bool HasAetherflow => JobGauge.Aetherflow > 0;

    protected static bool HasSeraph => JobGauge.SeraphTimer > 0;
    #endregion

    #region Heal
    private sealed protected override IBaseAction Raise => Resurrection;

    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, ActionOption.Heal);

    public static IBaseAction Adloquium { get; } = new BaseAction(ActionID.Adloquium, ActionOption.Heal)
    {
        ActionCheck = (b, m) => !b.HasStatus(false, StatusID.EukrasianDiagnosis, 
            StatusID.EukrasianPrognosis, StatusID.Galvanize),
    };

    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.Resurrection, ActionOption.Friendly);

    public static IBaseAction Succor { get; } = new BaseAction(ActionID.Succor, ActionOption.Heal)
    {
        StatusProvide = new[] { StatusID.Galvanize },
    };

    public static IBaseAction Lustrate { get; } = new BaseAction(ActionID.Lustrate, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    public static IBaseAction SacredSoil { get; } = new BaseAction(ActionID.SacredSoil, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasAetherflow && !IsMoving,
    };

    public static IBaseAction Indomitability { get; } = new BaseAction(ActionID.Indomitability, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    public static IBaseAction Excogitation { get; } = new BaseAction(ActionID.Excogitation, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    public static IBaseAction Consolation { get; } = new BaseAction(ActionID.Consolation, ActionOption.Heal)
    {
        ActionCheck = (b, m) => HasSeraph,
    };

    public static IBaseAction Protraction { get; } = new BaseAction(ActionID.Protraction, ActionOption.Heal);
    #endregion

    #region Attack
    public static IBaseAction Bio { get; } = new BaseAction(ActionID.Bio, ActionOption.Dot)
    {
        TargetStatus = new StatusID[] { StatusID.Bio, StatusID.Bio2, StatusID.Biolysis },
    };

    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.Ruin);

    public static IBaseAction Ruin2 { get; } = new BaseAction(ActionID.Ruin2);

    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrain)
    {
        ActionCheck = (b, m) => HasAetherflow
    };

    public static IBaseAction ArtOfWar { get; } = new BaseAction(ActionID.ArtOfWar);//裂阵法 25866
    #endregion

    #region Seraph
    public static IBaseAction SummonSeraph { get; } = new BaseAction(ActionID.SummonSeraph, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    public static IBaseAction SummonEos { get; } = new BaseAction(ActionID.SummonEos)//夕月召唤 17216
    {
        ActionCheck = (b, m) => !DataCenter.HasPet && (!Player.HasStatus(true, StatusID.Dissipation) || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel),
    };

    public static IBaseAction WhisperingDawn { get; } = new BaseAction(ActionID.WhisperingDawn, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    public static IBaseAction FeyIllumination { get; } = new BaseAction(ActionID.FeyIllumination, ActionOption.Heal)
    {
        ActionCheck = (b, m) => DataCenter.HasPet,
    };

    public static IBaseAction Dissipation { get; } = new BaseAction(ActionID.Dissipation)
    {
        StatusProvide = new[] { StatusID.Dissipation },
        ActionCheck = (b, m) => !HasAetherflow && !HasSeraph && InCombat && DataCenter.HasPet,
    };

    public static IBaseAction Aetherpact { get; } = new BaseAction(ActionID.Aetherpact, ActionOption.Heal)
    {
        ActionCheck = (b, m) => JobGauge.FairyGauge >= 10 && DataCenter.HasPet && !HasSeraph
    };

    public static IBaseAction FeyBlessing { get; } = new BaseAction(ActionID.FeyBlessing, ActionOption.Heal)
    {
        ActionCheck = (b, m) => !HasSeraph && DataCenter.HasPet,
    };
    #endregion

    #region Others
    public static IBaseAction Aetherflow { get; } = new BaseAction(ActionID.Aetherflow)
    {
        ActionCheck = (b, m) => InCombat && !HasAetherflow
    };

    public static IBaseAction Recitation { get; } = new BaseAction(ActionID.Recitation, ActionOption.Heal);

    public static IBaseAction ChainStratagem { get; } = new BaseAction(ActionID.ChainStratagem)
    {
        ActionCheck = (b, m) => InCombat && IsTargetBoss
    };

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

    public static IBaseAction EmergencyTactics { get; } = new BaseAction(ActionID.EmergencyTactics, ActionOption.Heal);

    public static IBaseAction Expedient { get; } = new BaseAction(ActionID.Expedient, ActionOption.Heal);
    #endregion

    #region Traits
    protected static IBaseTrait BroilMastery2 { get; } = new BaseTrait(184);
    protected static IBaseTrait BroilMastery { get; } = new BaseTrait(214);
    protected static IBaseTrait CorruptionMastery2    { get; } = new BaseTrait(311);
    protected static IBaseTrait BroilMastery3 { get; } = new BaseTrait(312);
    protected static IBaseTrait EnhancedSacredSoil    { get; } = new BaseTrait(313);
    protected static IBaseTrait CorruptionMastery    { get; } = new BaseTrait(324);
    protected static IBaseTrait BroilMastery4 { get; } = new BaseTrait(491);
    protected static IBaseTrait ArtOfWarMastery { get; } = new BaseTrait(492);
    protected static IBaseTrait EnhancedHealingMagic    { get; } = new BaseTrait(493);
    protected static IBaseTrait EnhancedDeploymentTactics    { get; } = new BaseTrait(494);
    #endregion

    protected override bool SpeedAbility(out IAction act)
    {
        if(InCombat && Expedient.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.SpeedAbility(out act);
    }
}