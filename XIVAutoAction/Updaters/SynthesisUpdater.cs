using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAction.Updaters
{
    internal static class SynthesisUpdater
    {
        public static int CurrentProgress { get; private set; } = 0;
        public static int MaxProgress { get; private set; } = 0;

        public static int CurrentQuality { get; private set; } = 0;
        public static int MaxQuality { get; private set; } = 0;
        public static int CurrentDurability { get; private set; } = 0;
        public static int StartingDurability { get; private set; } = 0;
        public static int StepNumber { get; private set; } = 0;
        public static Recipe Recipe { get; private set; } = null;
        static SortedList<string, Recipe> _recipeDict = new SortedList<string, Recipe>();
        public static CraftCondition CraftCondition { get; private set; } = CraftCondition.Unknown;

        internal unsafe static void UpdateSynthesis()
        {
            var ptr = Service.GameGui.GetAddonByName("Synthesis", 1);
            if (ptr == IntPtr.Zero) return;
            var syn = (AddonSynthesis*)ptr;

            if (int.TryParse(syn->CurrentProgress->NodeText.ToString(), out var value)) CurrentProgress = value;

            if (int.TryParse(syn->MaxProgress->NodeText.ToString(), out value)) MaxProgress = value;

            if (int.TryParse(syn->CurrentQuality->NodeText.ToString(), out value)) CurrentQuality = value;

            if (int.TryParse(syn->MaxQuality->NodeText.ToString(), out value)) MaxQuality = value;

            if (int.TryParse(syn->CurrentDurability->NodeText.ToString(), out value)) CurrentDurability = value;

            if (int.TryParse(syn->StartingDurability->NodeText.ToString(), out value)) StartingDurability = value;

            if (int.TryParse(syn->StepNumber->NodeText.ToString(), out value)) StepNumber = value;

            CraftCondition = GetCondition(syn->Condition->NodeText.ToString());
            Recipe = GetRecipeFromNodeText(syn->ItemName->NodeText.ToString());
        }

        private static Recipe GetRecipeFromNodeText(string itemName)
        {
            itemName = itemName.Substring(14, itemName.Length - 24);
            if (itemName[^1] == '')
            {
                itemName = itemName.Remove(itemName.Length - 1, 1).Trim();
            }
            if (_recipeDict.TryGetValue(itemName, out var b))
            {
                return b;
            }
            else
            {
                return _recipeDict[itemName] = Service.DataManager.GetExcelSheet<Recipe>().FirstOrDefault(x => x.ItemResult.Value.Name!.RawString.Equals(itemName));
            }
        }

        static readonly string[] _poorCondition = new string[]
        {
            "Poor",
            "低品质",
        };
        static readonly string[] _normalCondition = new string[]
        {
            "Normal",
            "通常",
        };
        static readonly string[] _goodCondition = new string[]
        {
            "Good",
            "高品质",
        };
        static readonly string[] _excellentCondition = new string[]
        {
            "Excellent",
            "最高品质",
        };
        static readonly string[] _centeredCondition = new string[]
        {
            "Centered",
        };
        static readonly string[] _sturdyCondition = new string[]
        {
            "Sturdy",
        };
        static readonly string[] _pliantCondition = new string[]
        {
            "Pliant",
        };
        static readonly string[] _malleableCondition = new string[]
        {
            "Malleable",
        };
        static readonly string[] _primedCondition = new string[]
        {
            "Primed",
        };
        private static CraftCondition GetCondition(string conditionStr)
        {
            if (_poorCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Poor;
            }
            else if (_normalCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Normal;
            }
            else if (_goodCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Good;
            }
            else if (_excellentCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Excellent;
            }
            else if (_centeredCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Centered;
            }
            else if (_sturdyCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Sturdy;
            }
            else if (_pliantCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Pliant;
            }
            else if (_malleableCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Malleable;
            }
            else if (_primedCondition.Any(s => conditionStr.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                return CraftCondition.Primed;
            }
            else
            {
                PluginLog.Debug("Unknow Condition: " + conditionStr);
                return CraftCondition.Unknown;
            }
        }
    }

    public enum CraftCondition : byte
    {
        Unknown,
        Poor,
        Normal,
        Good,
        Excellent,
        Centered,
        Sturdy,
        Pliant,
        Malleable,
        Primed,
    }
}
