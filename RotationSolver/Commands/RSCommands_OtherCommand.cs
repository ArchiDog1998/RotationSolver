using ECommons.DalamudServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.Commands;

public static partial class RSCommands
{
    private static void DoOtherCommand(OtherCommandType otherType, string str)
    {
        switch (otherType)
        {
            case OtherCommandType.Rotations:
                var customCombo = DataCenter.RightNowRotation;
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
        var job = DataCenter.Job;
        var strs = str.Split(' ');
        var value = strs.LastOrDefault();
        if (TryGetOneEnum<PluginConfigBool>(str, out var b))
        {
            var v = !Service.Config.GetValue(b);
            Service.Config.SetBoolRaw(b, v);
            value = Service.Config.GetValue(b).ToString();
        }
        else if (TryGetOneEnum<PluginConfigFloat>(str, out var f) && float.TryParse(value, out var f1))
        {
            Service.Config.SetValue(f, f1);
            value = Service.Config.GetValue(f).ToString();
        }
        else if (TryGetOneEnum<PluginConfigInt>(str, out var i) && int.TryParse(value, out var i1))
        {
            Service.Config.SetValue(i, i1);
            value = Service.Config.GetValue(i).ToString();

        }
        else if (TryGetOneEnum<JobConfigFloat>(str, out var f2) && float.TryParse(value, out f1))
        {
            Service.Config.SetValue(job, f2, f1);
            value = Service.Config.GetValue(job, f2).ToString();

        }
        else if (TryGetOneEnum<JobConfigInt>(str, out var i2) && int.TryParse(value, out i1))
        {
            Service.Config.SetValue(job, i2, i1);
            value = Service.Config.GetValue(job, i2).ToString();
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
            if (str.StartsWith(act.Name))
            {
                var flag = str[act.Name.Length..].Trim();

                act.IsEnabled = bool.TryParse(flag, out var parse) ? parse : !act.IsEnabled;

                if (Service.Config.GetValue(PluginConfigBool.ShowToggledActionInChat))
                {
                    Svc.Chat.Print($"Toggled {act.Name} : {act.IsEnabled}");
                }

                return;
            }
        }
    }

    private static void DoActionCommand(string str)
    {
        var strs = str.Split('-');

        if (strs != null && strs.Length == 2 && double.TryParse(strs[1], out var time))
        {
            var actName = strs[0];
            foreach (var iAct in RotationUpdater.RightRotationActions)
            {
                if (actName == iAct.Name)
                {
                    DataCenter.AddCommandAction(iAct, time);

                    if (Service.Config.GetValue(PluginConfigBool.ShowToastsAboutDoAction))
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
                Svc.Chat.Print(config.GetType().FullName);
                Svc.Chat.Print(str);
                Svc.Chat.Print(string.Format(LocalizationManager.RightLang.Commands_ChangeRotationConfig,
                    config.DisplayName, configs.GetDisplayString(config.Name)));

                return;
            }
        }
        Svc.Chat.PrintError(LocalizationManager.RightLang.Commands_CannotFindRotationConfig);
    }
}
