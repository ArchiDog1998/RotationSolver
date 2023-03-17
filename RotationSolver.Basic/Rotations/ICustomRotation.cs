using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;
using System.Reflection;

namespace RotationSolver.Basic.Rotations;

public interface ICustomRotation : ITexture
{
    ClassJob Job { get; }
    ClassJobID[] JobIDs { get; }

    string GameVersion { get; }
    string RotationName { get; }
    IRotationConfigSet Configs { get; }
    MedicineType MedicineType { get; }
    BattleChara MoveTarget { get; }

    IBaseAction[] AllBaseActions { get; }
    IAction[] AllActions { get; }
    PropertyInfo[] AllBools { get; }
    PropertyInfo[] AllBytes { get; }

    MethodInfo[] AllTimes { get; }
    MethodInfo[] AllLast { get; }
    MethodInfo[] AllGCDs { get; }

    bool TryInvoke(out IAction newAction, out IAction gcdAction);
}
