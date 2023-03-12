using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Attributes;
using RotationSolver.Basic;
using RotationSolver.Basic.Rotations;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
namespace RotationSolver.Rotations.CustomRotation;

[RotationDesc(DescType.BurstActions)]
public abstract partial class CustomRotation : ICustomRotation
{
    public abstract ClassJobID[] JobIDs { get; }

    public abstract string GameVersion { get; }

    public ClassJob Job => Service.GetSheet<ClassJob>().GetRow((uint)JobIDs[0]);

    public string Name => Job.Abbreviation + " - " + Job.Name;

    /// <summary>
    /// 作者
    /// </summary>
    public abstract string RotationName { get; }

    public bool IsEnabled
    {
        get => !Service.Config.DisabledCombos.Contains(Name);
        set
        {
            if (value)
            {
                Service.Config.DisabledCombos.Remove(Name);
            }
            else
            {
                Service.Config.DisabledCombos.Add(Name);
            }
        }
    }

    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }

    public BattleChara MoveTarget { get; private set; }

    public virtual string Description { get; } = string.Empty;

    /// <summary>
    /// Description about the actions.
    /// </summary>
    //public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();
    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this);
        Configs = CreateConfiguration();
    }

    protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet(JobIDs[0], RotationName);
    }


    /// <summary>
    /// Update your customized field.
    /// </summary>
    protected virtual void UpdateInfo() { }
}
