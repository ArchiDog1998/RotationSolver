using System.IO;
using XIVAutoAction;
using XIVAutoAction.Combos.CustomCombo;
using XIVAutoAction.Combos.Script.Actions;

namespace XIVAutoAction.Combos.Script
{
    internal interface IScriptCombo : ICustomCombo
    {
        public ComboSet Set { get; set; }
    }

    internal static class ScriptComboExtension
    {
        public static string GetFolder(this ComboSet set)
        {
            return Path.Combine(Service.Configuration.ScriptComboFolder, $"{(uint)set.JobID}_{set.AuthorName}.json");
        }

        public static string GetAuthor(this ComboSet set)
           => set.AuthorName + "-Script";
    }
}
