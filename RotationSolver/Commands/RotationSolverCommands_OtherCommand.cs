using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Commands
{
    internal static partial class RotationSolverCommands
    {
        internal record NextAct(BaseAction act, DateTime deadTime);

        private static List<NextAct> NextActs = new List<NextAct>();
        internal static BaseAction NextAction
        {
            get
            {
                var next = NextActs.FirstOrDefault();

                while (next != null && NextActs.Count > 0 && (next.deadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.act)))
                {
                    NextActs.RemoveAt(0);
                    next = NextActs.FirstOrDefault();
                }
                return next?.act;
            }
        }

        private static void DoOtherCommand(OtherCommandType otherType, string str)
        {
            switch (otherType)
            {
                case OtherCommandType.Combos:
                    var customCombo = IconReplacer.RightNowCombo;
                    if (customCombo == null) return;

                    DoComboCommand(customCombo, str);
                    break;

                case OtherCommandType.Actions:
                    DoActionCommand(str);
                    break;

                case OtherCommandType.Settings:
                    DoSettingCommand(str);
                    break;
            }
        }

        private static void DoSettingCommand(string str)
        {

        }

        private static void DoActionCommand(string str)
        {
            var strs = str.Split('-');

            if (strs != null && strs.Length == 2 && double.TryParse(strs[1], out var time))
            {
                var actName = strs[0];
                foreach (var iAct in IconReplacer.RightComboBaseActions)
                {
                    if (iAct is not BaseAction act) continue;
                    if (!act.IsTimeline) continue;

                    if (actName == act.Name)
                    {
                        var index = NextActs.FindIndex(i => i.act.ID == act.ID);
                        var newItem = new NextAct(act, DateTime.Now.AddSeconds(time));
                        if (index < 0)
                        {
                            NextActs.Add(newItem);
                        }
                        else
                        {
                            NextActs[index] = newItem;
                        }
                        NextActs = NextActs.OrderBy(i => i.deadTime).ToList();

                        Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time, act.Name));
                        return;
                    }
                }
            }
        }


        private static void DoComboCommand(ICustomCombo customCombo, string str)
        {
            foreach (var boolean in customCombo.Config.bools)
            {
                if (boolean.name == str)
                {
                    boolean.value = !boolean.value;

                    Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettings, boolean.description, boolean.value));

                    return;
                }
            }

            foreach (var combo in customCombo.Config.combos)
            {
                if (str.StartsWith(combo.name))
                {
                    var numStr = str.Substring(combo.name.Length);

                    if (string.IsNullOrEmpty(numStr) || str.Length == 0)
                    {
                        combo.value = (combo.value + 1) % combo.items.Length;

                    }
                    else if (int.TryParse(numStr, out int num))
                    {
                        combo.value = num % combo.items.Length;
                    }
                    else
                    {
                        for (int i = 0; i < combo.items.Length; i++)
                        {
                            if (combo.items[i] == str)
                            {
                                combo.value = i;
                            }
                        }
                    }

                    Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettings, combo.description, combo.items[combo.value]));

                    return;
                }
            }
        }
    }
}
