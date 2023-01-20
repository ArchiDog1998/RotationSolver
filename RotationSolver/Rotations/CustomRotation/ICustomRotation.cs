using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using System.Collections.Generic;
using System.Reflection;

namespace RotationSolver.Rotations.CustomRotation;

internal interface ICustomRotation : ITexture, IEnable
{
    ClassJob Job { get; }
    ClassJobID[] JobIDs { get; }

    string GameVersion { get; }
    string RotationName { get; }
    IRotationConfigSet Configs { get; }

    BattleChara MoveTarget { get; }

    SortedList<DescType, string> DescriptionDict { get; }
    IBaseAction[] AllActions { get; }
    PropertyInfo[] AllBools { get; }
    PropertyInfo[] AllBytes { get; }

    MethodInfo[] AllTimes { get; }
    MethodInfo[] AllLast { get; }
    MethodInfo[] AllOther { get; }
    MethodInfo[] AllGCDs { get; }

    bool TryInvoke(out IAction newAction);

    void Display(ICustomRotation[] rotations, bool canAddButton);
}
