namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SCH_Base : CustomRotation
{
    private static SCHGauge JobGauge => Service.JobGauges.Get<SCHGauge>();

    public override MedicineType MedicineType => MedicineType.Mind;

    protected static byte FairyGauge => JobGauge.FairyGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Scholar };

    private sealed protected override IBaseAction Raise => Resurrection;

    protected static bool HasAetherflow => JobGauge.Aetherflow > 0;

    protected static bool HasSeraph => JobGauge.SeraphTimer > 0;

    #region Heal
    public static IBaseAction Physick { get; } = new BaseAction(ActionID.Physick, true, isTimeline: true);

    public static IBaseAction Adloquium { get; } = new BaseAction(ActionID.Adloquium, true, isTimeline: true)
    {
        ActionCheck = b => !b.HasStatus(false, StatusID.EukrasianDiagnosis,
            StatusID.EukrasianPrognosis,
            StatusID.Galvanize),
    };

    public static IBaseAction Resurrection { get; } = new BaseAction(ActionID.Resurrection, true);

    public static IBaseAction Succor { get; } = new BaseAction(ActionID.Succor, true, isTimeline: true)
    {
        StatusProvide = new[] { StatusID.Galvanize },
    };

    public static IBaseAction Lustrate { get; } = new BaseAction(ActionID.Lustrate, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    public static IBaseAction SacredSoil { get; } = new BaseAction(ActionID.SacredSoil, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow && !IsMoving,
    };

    public static IBaseAction Indomitability { get; } = new BaseAction(ActionID.Indomitability, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    public static IBaseAction Excogitation { get; } = new BaseAction(ActionID.Excogitation, true, isTimeline: true)
    {
        ActionCheck = b => HasAetherflow
    };

    public static IBaseAction Consolation { get; } = new BaseAction(ActionID.Consolation, true, isTimeline: true)
    {
        ActionCheck = b => HasSeraph,
    };

    public static IBaseAction Protraction { get; } = new BaseAction(ActionID.Protraction, true, isTimeline: true);
    #endregion

    #region Attack
    public static IBaseAction Bio { get; } = new BaseAction(ActionID.Bio, isEot: true)
    {
        TargetStatus = new StatusID[] { StatusID.Bio, StatusID.Bio2, StatusID.Biolysis },
    };

    public static IBaseAction Ruin { get; } = new BaseAction(ActionID.Ruin);

    public static IBaseAction Ruin2 { get; } = new BaseAction(ActionID.Ruin2);

    public static IBaseAction EnergyDrain { get; } = new BaseAction(ActionID.EnergyDrain)
    {
        ActionCheck = b => HasAetherflow
    };

    public static IBaseAction ArtOfWar { get; } = new BaseAction(ActionID.ArtOfWar);//裂阵法 25866
    #endregion

    #region Seraph
    public static IBaseAction SummonSeraph { get; } = new BaseAction(ActionID.SummonSeraph, true, isTimeline: true)
    {
        ActionCheck = b => DataCenter.HasPet,
    };

    public static IBaseAction SummonEos { get; } = new BaseAction(ActionID.SummonEos)//夕月召唤 17216
    {
        ActionCheck = b => !DataCenter.HasPet && (!Player.HasStatus(true, StatusID.Dissipation) || Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel),
    };

    public static IBaseAction WhisperingDawn { get; } = new BaseAction(ActionID.WhisperingDawn, isTimeline: true)
    {
        ActionCheck = b => DataCenter.HasPet,
    };

    public static IBaseAction FeyIllumination { get; } = new BaseAction(ActionID.FeyIllumination, isTimeline: true)
    {
        ActionCheck = b => DataCenter.HasPet,
    };

    public static IBaseAction Dissipation { get; } = new BaseAction(ActionID.Dissipation)
    {
        StatusProvide = new[] { StatusID.Dissipation },
        ActionCheck = b => !HasAetherflow && !HasSeraph && InCombat && DataCenter.HasPet,
    };

    public static IBaseAction Aetherpact { get; } = new BaseAction(ActionID.Aetherpact, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.FairyGauge >= 10 && DataCenter.HasPet && !HasSeraph
    };

    public static IBaseAction FeyBlessing { get; } = new BaseAction(ActionID.FeyBlessing, isTimeline: true)
    {
        ActionCheck = b => !HasSeraph && DataCenter.HasPet,
    };
    #endregion

    #region Others
    public static IBaseAction Aetherflow { get; } = new BaseAction(ActionID.Aetherflow)
    {
        ActionCheck = b => InCombat && !HasAetherflow
    };

    public static IBaseAction Recitation { get; } = new BaseAction(ActionID.Recitation, isTimeline: true);

    public static IBaseAction ChainStratagem { get; } = new BaseAction(ActionID.ChainStratagem)
    {
        ActionCheck = b => InCombat && IsTargetBoss
    };

    public static IBaseAction DeploymentTactics { get; } = new BaseAction(ActionID.DeploymentTactics, true, isTimeline: true)
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

    public static IBaseAction EmergencyTactics { get; } = new BaseAction(ActionID.EmergencyTactics, isTimeline: true);

    public static IBaseAction Expedient { get; } = new BaseAction(ActionID.Expedient, isTimeline: true);
    #endregion
}