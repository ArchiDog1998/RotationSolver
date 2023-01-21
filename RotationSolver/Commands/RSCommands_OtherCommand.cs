using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Commands
{
    internal static partial class RSCommands
    {
        internal record NextAct(IBaseAction act, DateTime deadTime);

        private static List<NextAct> NextActs = new List<NextAct>();
        internal static IBaseAction NextAction
        {
            get
            {
                if (TimeLineUpdater.TimeLineAction != null) return TimeLineUpdater.TimeLineAction;

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
                    var customCombo = IconReplacer.RightNowRotation;
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
            if (str.Contains(nameof(Service.Configuration.AutoBurst)))
            {
                Service.Configuration.AutoBurst = !Service.Configuration.AutoBurst;

                //Say out.
                Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeAutoBurst,
                    Service.Configuration.AutoBurst));
            }
        }

        private static void DoActionCommand(string str)
        {
            //Todo!
            var strs = str.Split('-');

            if (strs != null && strs.Length == 2 && double.TryParse(strs[1], out var time))
            {
                var actName = strs[0];
                foreach (var iAct in IconReplacer.RightRotationBaseActions)
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

                        Service.ToastGui.ShowQuest(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time), 
                            new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = act.IconID,
                        });

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
                    Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeRotationConfig,
                        config.DisplayName, configs.GetDisplayString(config.Name)));

                    return;
                }
            }
            Service.ChatGui.Print(LocalizationManager.RightLang.Commands_CannotFindRotationConfig);
        }
    }
}
