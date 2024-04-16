using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

partial class CustomRotation : ICustomRotation
{
    private Job? _job = null;

    /// <inheritdoc/>
    public Job Job => _job ??= this.GetType().GetCustomAttribute<JobsAttribute>()?.Jobs[0] ?? Job.ADV;

    private JobRole? _role = null;

    /// <inheritdoc/>
    public JobRole Role  => _role ??= Svc.Data.GetExcelSheet<ClassJob>()!.GetRow((uint)Job)!.GetJobRole();
    private string? _name = null;

    /// <inheritdoc/>
    public string Name
    {
        get
        {
            if (_name != null) return _name;

            var classJob = Svc.Data.GetExcelSheet<ClassJob>()?.GetRow((uint)Job)!;

            return _name = classJob.Abbreviation + " - " + classJob.Name;
        }
    }

    /// <inheritdoc/>
    public bool IsEnabled
    {
        get => !Service.Config.DisabledJobs.Contains(Job);
        set
        {
            if (value)
            {
                Service.Config.DisabledJobs.Remove(Job);
            }
            else
            {
                Service.Config.DisabledJobs.Add(Job);
            }
        }
    }

    /// <inheritdoc/>
    public uint IconID { get; }

    private readonly IRotationConfigSet _configs;

    /// <inheritdoc/>
    IRotationConfigSet ICustomRotation.Configs => _configs;

    /// <inheritdoc/>
    public static Vector3? MoveTarget { get; internal set; }

    /// <inheritdoc/>
    public string Description => this.GetType().GetCustomAttribute<RotationAttribute>()?.Description ?? string.Empty;

    /// <inheritdoc/>
    public IAction? ActionHealAreaGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionHealAreaAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionHealSingleGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionHealSingleAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDefenseAreaGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDefenseAreaAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDefenseSingleGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDefenseSingleAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionMoveForwardGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionMoveForwardAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionMoveBackAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionSpeedAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDispelStancePositionalGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionDispelStancePositionalAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionRaiseShirkGCD { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionRaiseShirkAbility { get; private set; }

    /// <inheritdoc/>
    public IAction? ActionAntiKnockbackAbility { get; private set; }

    /// <summary>
    /// Is this action valid.
    /// </summary>
    [Description("Is this rotation valid")]
    public bool IsValid { get; private set; } = true;

    /// <summary>
    /// Why this action is not valid.
    /// </summary>
    public string WhyNotValid { get; private set; } = string.Empty;

    /// <summary>
    /// Should show the status to the users.
    /// </summary>
    [Description("Show the status")]
    public virtual bool ShowStatus => false;

    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this.Job);
        _configs = new RotationConfigSet(this);
    }

    /// <inheritdoc/>
    public override string ToString() => this.GetType().GetCustomAttribute<RotationAttribute>()?.Name ?? this.GetType().Name;

    /// <summary>
    /// Update your customized field.
    /// </summary>
    protected virtual void UpdateInfo() { }

    /// <summary>
    /// Some extra display things.
    /// </summary>
    public virtual void DisplayStatus()
    {
        ImGui.TextWrapped($"If you want to Display some extra information on this panel. Please override {nameof(DisplayStatus)} method!");
    }

    /// <summary>
    /// The things on territory changed.
    /// </summary>
    public virtual void OnTerritoryChanged() { }

    /// <summary>
    /// Creates a system warning to display to the end-user
    /// </summary>
    public void CreateSystemWarning(string warning) => WarningHelper.AddSystemWarning(warning);
}
