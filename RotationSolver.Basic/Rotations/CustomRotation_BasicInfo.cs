using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

[Jobs()]
partial class CustomRotation : ICustomRotation
{
    private Job? _job = null;
    public Job Job => _job ??= this.GetType().GetCustomAttribute<JobsAttribute>()?.Jobs[0] ?? Job.ADV;

    private JobRole? _role = null;
    public JobRole Role  => _role ??= Svc.Data.GetExcelSheet<ClassJob>()!.GetRow((uint)Job)!.GetJobRole();
    private string? _name = null;
    public string Name
    {
        get
        {
            if (_name != null) return _name;

            var classJob = Svc.Data.GetExcelSheet<ClassJob>()?.GetRow((uint)Job)!;

            return _name = classJob.Abbreviation + " - " + classJob.Name;
        }
    }

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

    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }

    public static Vector3? MoveTarget { get; internal set; }

    public string Description => this.GetType().GetCustomAttribute<RotationAttribute>()?.Description ?? string.Empty;

    public IAction? ActionHealAreaGCD { get; private set; }

    public IAction? ActionHealAreaAbility { get; private set; }

    public IAction? ActionHealSingleGCD { get; private set; }

    public IAction? ActionHealSingleAbility { get; private set; }

    public IAction? ActionDefenseAreaGCD { get; private set; }

    public IAction? ActionDefenseAreaAbility { get; private set; }

    public IAction? ActionDefenseSingleGCD { get; private set; }

    public IAction? ActionDefenseSingleAbility { get; private set; }

    public IAction? ActionMoveForwardGCD { get; private set; }

    public IAction? ActionMoveForwardAbility { get; private set; }

    public IAction? ActionMoveBackAbility { get; private set; }

    public IAction? ActionSpeedAbility { get; private set; }

    public IAction? ActionDispelStancePositionalGCD { get; private set; }

    public IAction? ActionDispelStancePositionalAbility { get; private set; }

    public IAction? ActionRaiseShirkGCD { get; private set; }

    public IAction? ActionRaiseShirkAbility { get; private set; }

    public IAction? ActionAntiKnockbackAbility { get; private set; }

    [Description("Is this rotation valid")]
    public bool IsValid { get; private set; } = true;
    public string WhyNotValid { get; private set; } = string.Empty;

    [Description("Show the status")]
    public virtual bool ShowStatus => false;

    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this.Job);
        Configs = CreateConfiguration();
    }

    protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet();
    }

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
}
