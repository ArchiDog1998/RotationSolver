using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Script.Actions;

namespace XIVAutoAttack.Combos.Script
{
    internal interface IScriptCombo : ICustomCombo
    {
        public ActionsSet GeneralGCDSet { get; set; }
        string AuthorName { get; set; }
    }

    internal static class ScriptComboExtension
    {
        public static string GetFolder(this IScriptCombo combo)
           => $"{Service.Configuration.ScriptComboFolder}{combo.Job.RowId}_{combo.AuthorName}.json";

        public static string GetAuthor(this IScriptCombo combo)
           => combo.AuthorName + "-Script";
    }
}
