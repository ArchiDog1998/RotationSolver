﻿using System.IO;
using AutoAction.Combos.CustomCombo;
using AutoAction.Combos.Script.Actions;

namespace AutoAction.Combos.Script
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
