using RotationSolver.Localization;
using RotationSolver.Updaters;

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
            if (!TryGetOneEnum<SettingsCommand>(str, out var type))
            {
                RotationSolverPlugin.OpenConfigWindow();
                return;
            }

            Service.Config.SetValue(type, !Service.Config.GetValue(type));

            //Say out.
            Service.ChatGui.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettingsValue,
                type.ToString(), Service.Config.GetValue(type)));
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
                        DataCenter.AddCommandAction(act, time);

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
