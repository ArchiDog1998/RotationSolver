using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal interface ICustomCombo : ITexture
    {
        ClassJob Job { get; }
        ClassJobID[] JobIDs { get; }

        ActionConfiguration Config { get; }

        SortedList<DescType, string> DescriptionDict { get; }
        Dictionary<string, string> CommandShow { get; }

        string OnCommand(string args);
        bool TryInvoke(out IAction newAction);
    }
}
