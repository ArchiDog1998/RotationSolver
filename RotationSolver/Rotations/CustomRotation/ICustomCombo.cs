using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver;
using RotationSolver.Actions;
using RotationSolver.Configuration;
using RotationSolver.Data;
using System.Collections.Generic;
using System.Reflection;

namespace RotationSolver.Combos.CustomCombo;

internal interface ICustomCombo : IEnableTexture
{
    ClassJob Job { get; }
    ClassJobID[] JobIDs { get; }
    string Description { get; }

    string GameVersion { get; }
    string Author { get; }
    ActionConfiguration Config { get; }

    BattleChara MoveTarget { get; }

    SortedList<DescType, string> DescriptionDict { get; }
    IAction[] AllActions { get; }
    PropertyInfo[] AllBools { get; }
    PropertyInfo[] AllBytes { get; }

    MethodInfo[] AllTimes { get; }
    MethodInfo[] AllLast { get; }
    MethodInfo[] AllOther { get; }
    MethodInfo[] AllGCDs { get; }

    bool TryInvoke(out IAction newAction);
}
