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
        var strs = str.Split(' ');
        var value = strs.LastOrDefault();

        foreach (var property in typeof(ConfigsNew).GetRuntimeProperties()
            .Where(p => (p.GetMethod?.IsPublic ?? false) && (p.SetMethod?.IsPublic ?? false)))
        {
            if (!str.StartsWith(property.Name, StringComparison.OrdinalIgnoreCase)) continue;

            var type = property.PropertyType;

            if (type == typeof(ConditionBoolean))
            {
                type = typeof(bool);
            }

            var v = Convert.ChangeType(value, type);

            if (v == null && type == typeof(bool))
            {
                v = !(bool)property.GetValue(Service.Config)!;
            }

            if (property.PropertyType == typeof(ConditionBoolean))
            {
                var relay = (ConditionBoolean)property.GetValue(Service.Config)!;
                relay.Value = (bool)v!;
                v = relay;
            }

            if (v == null)
            {
#if DEBUG
                Svc.Chat.Print("Failed to get the value.");
#endif
                continue;
            }

            property.SetValue(Service.Config, v);
            value = v.ToString();   

            //Say out.
            Svc.Chat.Print(string.Format( "CommandsChangeSettingsValue".Local("Modify {0} to {1}"), property.Name, value));

            return;

        }

        Svc.Chat.PrintError("CommandsCannotFindConfig".Local("Failed to find the config in this rotation, please check it."));
    }

    private static void ToggleActionCommand(string str)
    {
        foreach (var act in RotationUpdater.RightRotationActions)
        {
            if (str.StartsWith(act.Name))
            {
                var flag = str[act.Name.Length..].Trim();

                act.IsEnabled = bool.TryParse(flag, out var parse) ? parse : !act.IsEnabled;

                if (Service.Config.ShowToggledActionInChat)
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

                    if (Service.Config.ShowToastsAboutDoAction)
                    {
                        Svc.Toasts.ShowQuest(string.Format("CommandsInsertAction".Local("Will use it within {0}s"), time),
                            new Dalamud.Game.Gui.Toast.QuestToastOptions()
                            {
                                IconId = iAct.IconID,
                            });
                    }

                    return;
                }
            }
        }

        Svc.Chat.PrintError("CommandsInsertActionFailure".Local("Can not find the action, please check the action name."));
    }


    private static void DoRotationCommand(ICustomRotation customCombo, string str)
    {
        var configs = customCombo.Configs;
        foreach (var config in configs)
        {
            if (config.DoCommand(configs, str))
            {
                Svc.Chat.Print(string.Format("CommandsChangeSettingsValue".Local("Modify {0} to {1}"),
                    config.DisplayName, configs.GetDisplayString(config.Name)));

                return;
            }
        }

        Svc.Chat.PrintError("CommandsCannotFindConfig".Local("Failed to find the config, please check it."));
    }
}
