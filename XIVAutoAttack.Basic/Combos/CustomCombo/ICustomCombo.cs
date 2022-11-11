using ImGuiScene;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public interface ICustomCombo : ITexture
    {
        uint[] JobIDs {  get; }

        ActionConfiguration Config { get; }

        SortedList<DescType, string> DescriptionDict { get; }
        Dictionary<string, string> CommandShow { get; }

        string OnCommand(string args);

        IAction CountDownAction(float remainTime);

        bool EmergercyGCD(out IAction act);
        bool GeneralGCD(out IAction act);
        bool MoveGCD(out IAction act);
        bool HealSingleGCD(out IAction act);
        bool HealAreaGCD(out IAction act);
        bool DefenseSingleGCD(out IAction act);
        bool DefenseAreaGCD(out IAction act);
    }
}
