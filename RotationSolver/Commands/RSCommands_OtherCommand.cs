using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        private static void DoOtherCommand(OtherCommandType otherType, string str)
        {
            switch (otherType)
            {
                case OtherCommandType.Rotations:
                    var customCombo = RotationUpdater.RightNowRotation;
                    if (customCombo == null) return;

                    DoRotationCommand(customCombo, str);
                    break;

                case OtherCommandType.DoActions:
                    DoActionCommand(str);
                    break;

                case OtherCommandType.ToggleActions:
                    ToggleActionCommand(str);
                    break;

                case OtherCommandType.Settings:
                    DoSettingCommand(str);
                    break;
            }
        }

        private static void DoSettingCommand(string str)
        {
            if (str.Contains(nameof(Service.Config.AutoBurst)))
            {
                Service.Config.AutoBurst = !Service.Config.AutoBurst;

                //Say out.
                Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeAutoBurst,
                    Service.Config.AutoBurst));
            }
        }

        private static void ToggleActionCommand(string str)
        {
            foreach (var act in RotationUpdater.RightRotationBaseActions)
            {
                if (str == act.Name)
                {
                    act.IsEnabled = !act.IsEnabled;

                    //Service.ToastGui.ShowQuest(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time),
                    //    new Dalamud.Game.Gui.Toast.QuestToastOptions()
                    //    {
                    //        IconId = act.IconID,
                    //    });

                    return;
                }
            }
        }

        private static void DoActionCommand(string str)
        {
            //Todo!
            var strs = str.Split('-');

            if (strs != null && strs.Length == 2 && double.TryParse(strs[1], out var time))
            {
                var actName = strs[0];
                foreach (var iAct in RotationUpdater.RightRotationBaseActions)
                {
                    if (iAct is not IBaseAction act) continue;
                    if (!act.IsTimeline) continue;

                    if (actName == act.Name)
                    {
                        DataCenter.AddOneTimelineAction(act, time);

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
                if (config.DoCommand(configs, str))
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
