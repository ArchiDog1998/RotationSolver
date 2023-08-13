using ECommons.DalamudServices;
using RotationSolver.Basic.Configuration;
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

                case OtherCommandType.NextAction:
                    DoAction();
                    break;
            }
        }

        private static void DoSettingCommand(string str)
        {
            var job = RotationUpdater.Job;
            var strs = str.Split(' ');
            var value = strs.LastOrDefault();
            if(TryGetOneEnum<PluginConfigBool>(str, out var b))
            {
                var v = !Service.ConfigNew.GetValue(b);
                Service.ConfigNew.SetValue(b, v);
                value = Service.ConfigNew.GetValue(b).ToString();
            }
            else if (TryGetOneEnum<PluginConfigFloat>(str, out var f) && float.TryParse(value, out var f1))
            {
                Service.ConfigNew.SetValue(f, f1);
                value = Service.ConfigNew.GetValue(f).ToString();
            }
            else if (TryGetOneEnum<PluginConfigInt>(str, out var i) && int.TryParse(value, out var i1))
            {
                Service.ConfigNew.SetValue(i, i1);
                value = Service.ConfigNew.GetValue(i).ToString();

            }
            else if (TryGetOneEnum<JobConfigFloat>(str, out var f2) && float.TryParse(value, out f1))
            {
                Service.ConfigNew.SetValue(job, f2, f1);
                value = Service.ConfigNew.GetValue(job, f2).ToString();

            }
            else if (TryGetOneEnum<JobConfigInt>(str, out var i2) && int.TryParse(value, out i1))
            {
                Service.ConfigNew.SetValue(job, i2, i1);
                value = Service.ConfigNew.GetValue(job, i2).ToString();
            }
            else
            {
                Svc.Chat.PrintError(LocalizationManager.RightLang.Commands_CannotFindConfig);
                return;
            }

            //Say out.
            Svc.Chat.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeSettingsValue,
                strs.FirstOrDefault(), value));
        }

        private static void ToggleActionCommand(string str)
        {
            foreach (var act in RotationUpdater.RightRotationActions)
            {
                if (str == act.Name)
                {
                    act.IsEnabled = !act.IsEnabled;

                    //Svc.Toasts.ShowQuest(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time),
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
                foreach (var iAct in RotationUpdater.RightRotationActions)
                {
                    if (actName == iAct.Name)
                    {
                        DataCenter.AddCommandAction(iAct, time);

                        if (Service.Config.ShowToastsAboutDoAction)
                        {
                            Svc.Toasts.ShowQuest(string.Format(LocalizationManager.RightLang.Commands_InsertAction, time),
                                new Dalamud.Game.Gui.Toast.QuestToastOptions()
                                {
                                    IconId = iAct.IconID,
                                });
                        }

                        return;
                    }
                }
            }

            Svc.Chat.PrintError(LocalizationManager.RightLang.Commands_InsertActionFailure);
        }


        private static void DoRotationCommand(ICustomRotation customCombo, string str)
        {
            var configs = customCombo.Configs;
            foreach (var config in configs)
            {
                if (config.DoCommand(configs, str))
                {
                    Svc.Chat.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeRotationConfig,
                        config.DisplayName, configs.GetDisplayString(config.Name)));

                    return;
                }
            }
            Svc.Chat.PrintError(LocalizationManager.RightLang.Commands_CannotFindRotationConfig);
        }
    }
}
