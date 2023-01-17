using RotationSolver.Combos.CustomCombo;
using RotationSolver.Combos.Script.Actions;
using System.IO;

namespace RotationSolver.Combos.Script
{
    internal interface IScriptCombo : ICustomRotation
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
