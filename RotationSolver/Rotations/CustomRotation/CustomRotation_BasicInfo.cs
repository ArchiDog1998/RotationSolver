using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Localization;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;

internal abstract partial class CustomRotation : ICustomRotation
{
    public abstract ClassJobID[] JobIDs { get; }

    public abstract string GameVersion { get; }

    public ClassJob Job => Service.DataManager.GetExcelSheet<ClassJob>().GetRow((uint)JobIDs[0]);

    public string Name => Job.Abbreviation + " - " + Job.Name;

    /// <summary>
    /// 作者
    /// </summary>
    public abstract string RotationName { get; }

    public bool IsEnabled
    {
        get => !Service.Configuration.DisabledCombos.Contains(Name);
        set
        {
            if (value)
            {
                Service.Configuration.DisabledCombos.Remove(Name);
            }
            else
            {
                Service.Configuration.DisabledCombos.Add(Name);
            }
        }
    }

    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }
    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this);
        Configs = CreateConfiguration();
    }

    public BattleChara MoveTarget { get; private set; }

    public string Description => string.Join('\n', DescriptionDict.Select(pair => pair.Key.ToName() + " → " + pair.Value));

    /// <summary>
    /// Description about the actions.
    /// </summary>
    public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();

    private protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet(JobIDs[0], RotationName);
    }


    /// <summary>
    /// Update your customized field.
    /// </summary>
    private protected virtual void UpdateInfo() 
    {
        MoveTarget = (MoveForwardAbility(1, out var act) && act is BaseAction a) ? a.Target : null;
    }
}
