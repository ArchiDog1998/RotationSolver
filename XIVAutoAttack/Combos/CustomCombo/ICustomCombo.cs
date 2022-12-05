using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Reflection;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.CustomCombo
{
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
        Dictionary<string, string> CommandShow { get; }
        BaseAction[] AllActions { get; }
        PropertyInfo[] AllBools { get; }
        PropertyInfo[] AllBytes { get; }

        MethodInfo[] AllTimes { get; }
        MethodInfo[] AllLast { get; }
        MethodInfo[] AllOther { get; }
        MethodInfo[] AllGCDs { get; }
        string OnCommand(string args);
        bool TryInvoke(out IAction newAction);
    }
}
