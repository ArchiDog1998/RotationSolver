using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using XIVComboPlus;

namespace XIVComboPlus;

public sealed class XIVComboPlusPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pcombo";

    private const string _lockCommand = "/plock";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;

    public string Name => "XIV Combo Plus";
    //public static BattleNpc[] Targets =>
    //        Service.ObjectTable.Where(obj => (BattleNpc)obj != null && DistanceToPlayer(obj) <= 25).Select(obj => (BattleNpc)obj).ToArray();

    //public static PlayerCharacter[] Friends =>
    //        Service.ObjectTable.Where(obj => (PlayerCharacter)obj != null && DistanceToPlayer(obj) <= 25).Select(obj => (PlayerCharacter)obj).ToArray();

    private static bool _lockHighMostHP = false;
    public XIVComboPlusPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();
        Service.IconReplacer = new IconReplacer();
        configWindow = new ConfigWindow();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow((Window)(object)configWindow);
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        CommandManager commandManager = Service.CommandManager;

        CommandInfo val = new CommandInfo(new CommandInfo.HandlerDelegate(OnCommand));
        val.HelpMessage = "Open a window to edit custom combo settings.";
        val.ShowInHelp = true;
        commandManager.AddHandler(_command, val);

        //CommandInfo lockInfo = new CommandInfo(new CommandInfo.HandlerDelegate(OnLock));
        //lockInfo.HelpMessage = "锁定想要的敌人。";
        //lockInfo.ShowInHelp = true;
        //commandManager.AddHandler(_lockCommand, lockInfo);

        //Service.Framework.Update += FrameworkUpdate;
    }

    private static float DistanceToPlayer(GameObject obj)
    {
        return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
    }

    //private void FrameworkUpdate(Framework framework)
    //{
    //    //Service.TargetManager.SetTarget(Targets.OrderByDescending(tar => tar.CurrentHp).First());

    //    if (_lockHighMostHP)
    //    {
    //        Service.TargetManager.SetTarget(Targets.OrderByDescending(tar => tar.CurrentHp).First());
    //        _lockHighMostHP = false;
    //    }
    //}

    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.CommandManager.RemoveHandler(_lockCommand);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        //Service.Framework.Update -= FrameworkUpdate;
        Service.IconReplacer.Dispose();
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }
    //private void OnLock(string command, string arguments)
    //{
    //    string[] array = arguments.Split();

    //    Service.TargetManager.SetTarget(Targets.OrderByDescending(tar => tar.CurrentHp).First());

    //    //_lockHighMostHP = true;

    //    switch (array[0])
    //    {
    //        case "HighMostHP":
    //            _lockHighMostHP = true;
    //            break;
    //    }
    //}
    private void OnCommand(string command, string arguments)
    {
        //string[] values = IconReplacer.CustomCombos.Select(c => c.ComboFancyName).ToArray();

        string[] array = arguments.Split();
        switch (array[0])
        {
            case "setall":
                {

                    foreach (var item in IconReplacer.CustomCombos)
                    {
                        item.IsEnabled = true;
                    }
                    Service.ChatGui.Print("All SET");
                    Service.Configuration.Save();
                    break;
                }
            case "unsetall":
                {
                    foreach (var item in IconReplacer.CustomCombos)
                    {
                        item.IsEnabled = false;
                    }
                    Service.ChatGui.Print("All UNSET");
                    Service.Configuration.Save();
                    break;
                }
            case "set":
                {
                    string text3 = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var value = IconReplacer.CustomCombos[i];
                        if (value.ComboFancyName.ToLowerInvariant() == text3)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} SET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            case "secrets":
                Service.Configuration.EnableSecretCombos = !Service.Configuration.EnableSecretCombos;
                Service.ChatGui.Print(Service.Configuration.EnableSecretCombos ? "Secret combos are now shown" : "Secret combos are now hidden");
                Service.Configuration.Save();
                break;
            case "toggle":
                {
                    string text = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var customComboPreset2 = IconReplacer.CustomCombos[i];
                        if (customComboPreset2.ComboFancyName.ToLowerInvariant() == text)
                        {
                            customComboPreset2.IsEnabled = !customComboPreset2.IsEnabled;
                            Service.ChatGui.Print(customComboPreset2.ComboFancyName + " " + (customComboPreset2.IsEnabled ? "SET": "UNSET"));
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            //case "dot":
            //    if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Remove(CustomComboPreset.SCHDotFeature);
            //    }
            //    if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Add(CustomComboPreset.SCHDotFeature);
            //    }
            //    if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Remove(CustomComboPreset.ASTdotFeature);
            //    }
            //    if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
            //    {
            //        Service.Configuration.EnabledActions.Add(CustomComboPreset.ASTdotFeature);
            //    }
            //    Service.Configuration.Save();
            //    break;
            case "unset":
                {
                    string text2 = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var value = IconReplacer.CustomCombos[i];
                        if (value.ComboFancyName.ToLowerInvariant() == text2)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} UNSET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            //case "list":
            //    switch (array.Length > 1 ? array[1].ToLowerInvariant() : "all")
            //    {
            //        case "set":
            //            foreach (bool item3 in from preset in values
            //                                   select Service.Configuration.IsEnabled(preset))
            //            {
            //                Service.ChatGui.Print(item3.ToString());
            //            }
            //            break;
            //        case "unset":
            //            foreach (bool item4 in from preset in values
            //                                   select !Service.Configuration.IsEnabled(preset))
            //            {
            //                Service.ChatGui.Print(item4.ToString());
            //            }
            //            break;
            //        case "all":
            //            {
            //                for (int i = 0; i < values.Length; i++)
            //                {
            //                    Service.ChatGui.Print(values[i]);
            //                }
            //                break;
            //            }
            //        default:
            //            Service.ChatGui.PrintError("Available list filters: set, unset, all");
            //            break;
            //    }
            //    break;
            default:
                configWindow.Toggle();
                break;
        }
        Service.Configuration.Save();
    }
}
