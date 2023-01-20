using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Attributes;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Rotations.CustomRotation;

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

    /// <summary>
    /// 目标是否将要死亡
    /// </summary>
    internal static bool IsTargetDying
    {
        get
        {
            if (Target == null) return false;
            return Target.IsDying();
        }
    }

    /// <summary>
    /// 目标是否是Boss
    /// </summary>
    internal static bool IsTargetBoss
    {
        get
        {
            if (Target == null) return false;
            return Target.IsBoss();
        }
    }

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
    /// <summary>
    /// 有即刻相关Buff
    /// </summary>
    internal static bool HaveSwift => Player.HasStatus(true, Swiftcast.StatusProvide);

    /// <summary>
    /// 有盾姿，如果为非T那么始终为true
    /// </summary>
    [ReflectableMember]
    internal static bool HaveShield => Player.HasStatus(true, StatusHelper.SheildStatus);


    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }
    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this);
        Configs = CreateConfiguration();
    }

    public BattleChara MoveTarget
    {
        get
        {
            if (MoveForwardAbility(1, out var act) && act is BaseAction a) return a.Target;
            return null;
        }
    }

    private protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet(JobIDs[0], RotationName);
    }
}
