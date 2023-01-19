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
    internal static partial class RSCommands
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
                case OtherCommandType.Rotations:
                    var customCombo = IconReplacer.RightNowCombo;
                    if (customCombo == null) return;

                    DoRotationCommand(customCombo, str);
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
            if (str.Contains(nameof(Service.Configuration.AutoBreak)))
            {
                Service.Configuration.AutoBreak = !Service.Configuration.AutoBreak;

                //Say out.
                Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeRotationConfig,
    nameof(Service.Configuration.AutoBreak), Service.Configuration.AutoBreak));

            }
        }

        private static void DoActionCommand(string str)
        {
            //Todo!
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

            Service.ChatGui.Print(LocalizationManager.RightLang.Commands_InsertActionFailure);
        }


        private static void DoRotationCommand(ICustomRotation customCombo, string str)
        {
            var configs = customCombo.Configs;
            foreach (var config in configs)
            {
                if(config.DoCommand(configs, str))
                {
                    //Say out.
                    Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeRotationConfig,
                        config.DisplayName, configs.GetDisplayString(config.Name)));

                    return;
                }
            }
            Service.ChatGui.Print(LocalizationManager.RightLang.Commands_CannotFindRotationConfig);
        }
    }
}
