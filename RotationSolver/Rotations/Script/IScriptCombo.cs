using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Rotations.Script.Actions;
using System.IO;

namespace RotationSolver.Rotations.Script
{
    internal interface IScriptCombo : ICustomRotation
    {
        public ComboSet Set { get; set; }
    }

    internal static class ScriptComboExtension
    {
        public static string GetFolder(this ComboSet set)
        {
            return Path.Combine(typeof(ScriptComboExtension).Assembly.Location, $"{(uint)set.JobID}_{set.AuthorName}.json");
        }

        public static string GetAuthor(this ComboSet set)
           => set.AuthorName + "-Script";
    }
}
