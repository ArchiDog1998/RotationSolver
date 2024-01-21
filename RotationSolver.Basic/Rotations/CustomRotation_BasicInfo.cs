using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

[RotationDesc(DescType.BurstActions)]
public abstract partial class CustomRotation : ICustomRotation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract CombatType Type { get; }

    public abstract Job[] Jobs { get; }

    public abstract string GameVersion { get; }

    public ClassJob ClassJob => Service.GetSheet<ClassJob>().GetRow((uint)Jobs[0]);

    public string Name => ClassJob.Abbreviation + " - " + ClassJob.Name;

    public abstract string RotationName { get; }

    public bool IsEnabled
    {
        get => !Service.Config.GlobalConfig.DisabledJobs.Contains(Jobs.FirstOrDefault());
        set
        {
            if (value)
            {
                Service.Config.GlobalConfig.DisabledJobs.Remove(Jobs.FirstOrDefault());
            }
            else
            {
                Service.Config.GlobalConfig.DisabledJobs.Add(Jobs.FirstOrDefault());
            }
        }
    }

    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }

    public static Vector3? MoveTarget { get; internal set; }

    public virtual string Description { get; } = string.Empty;

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

    public bool IsValid { get; private set; } = true;
    public string WhyNotValid { get; private set; } = string.Empty;

    public virtual bool ShowStatus => false;

    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this);
        Configs = CreateConfiguration();
    }

    protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet(Jobs[0], GetType().FullName ?? "No Name");
    }

    public override string ToString() => RotationName;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
