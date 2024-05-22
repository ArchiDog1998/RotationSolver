using ECommons.DalamudServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Updaters;
using XIVConfigUI;

namespace RotationSolver.Commands;

public static partial class RSCommands
{
    private static void DoOtherCommand(OtherCommandType otherType, string str)
    {
        switch (otherType)
        {
            case OtherCommandType.DoActions:
                DoActionCommand(str);
                break;

            case OtherCommandType.ToggleActions:
                ToggleActionCommand(str);
                break;

            case OtherCommandType.NextAction:
                DoAction();
                break;

            case OtherCommandType.ToggleActionGroup:
                ToggleActionGroup(str);
                break;
        }
    }

    private static void ToggleActionGroup(string str)
    {
        foreach (var grp in Service.Config.ActionGroup)
        {
            if (str.StartsWith(grp.Name))
            {
                var flag = str[grp.Name.Length..].Trim();

                grp.Enable = bool.TryParse(flag, out var parse) ? parse : !grp.Enable;

                if (Service.Config.ShowToggledActionInChat)
                {
                    Svc.Chat.Print($"Toggled {grp.Name} : {grp.Enable}");
                }

                return;
            }
        }
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
                        Svc.Toasts.ShowQuest(string.Format(UiString.CommandsInsertAction.Local(), time),
                            new Dalamud.Game.Gui.Toast.QuestToastOptions()
                            {
                                IconId = iAct.IconID,
                            });
                    }

                    return;
                }
            }
        }

        Svc.Chat.PrintError(UiString.CommandsInsertActionFailure.Local());
    }
}
