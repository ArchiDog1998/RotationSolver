using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using XIVComboPlus;

namespace XIVComboPlus;

public sealed class XIVComboPlusPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pcombo";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;

    public string Name => "XIV Combo Plus";

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
    }

    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.IconReplacer.Dispose();
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }

    private void OnCommand(string command, string arguments)
    {
        string[] values = Service.IconReplacer.CustomCombos.Select(x => x.ComboFancyName).ToArray();

        string[] array = arguments.Split();
        switch (array[0])
        {
            case "setall":
                {

                    foreach (string item in values)
                    {
                        Service.Configuration.EnabledActions.Add(item);
                    }
                    Service.ChatGui.Print("All SET");
                    Service.Configuration.Save();
                    break;
                }
            case "unsetall":
                {
                    Service.Configuration.EnabledActions.Clear();
                    Service.ChatGui.Print("All UNSET");
                    Service.Configuration.Save();
                    break;
                }
            case "set":
                {
                    string text3 = array[1].ToLowerInvariant();
                    for (int i = 0; i < values.Length; i++)
                    {
                        string value = values[i];
                        if (value.ToLowerInvariant() == text3)
                        {
                            Service.Configuration.EnabledActions.Add(value);
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
                    for (int i = 0; i < values.Length; i++)
                    {
                        string customComboPreset2 = values[i];
                        if (customComboPreset2.ToLowerInvariant() == text)
                        {
                            if (Service.Configuration.EnabledActions.Contains(customComboPreset2))
                            {
                                Service.Configuration.EnabledActions.Remove(customComboPreset2);
                                Service.ChatGui.Print($"{customComboPreset2} UNSET");
                            }
                            else
                            {
                                Service.Configuration.EnabledActions.Add(customComboPreset2);
                                Service.ChatGui.Print($"{customComboPreset2} SET");
                            }
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
                    for (int i = 0; i < values.Length; i++)
                    {
                        string customComboPreset3 = values[i];
                        if (!(customComboPreset3.ToString().ToLowerInvariant() != text2))
                        {
                            Service.Configuration.EnabledActions.Remove(customComboPreset3);
                            Service.ChatGui.Print($"{customComboPreset3} UNSET");
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            case "list":
                switch (array.Length > 1 ? array[1].ToLowerInvariant() : "all")
                {
                    case "set":
                        foreach (bool item3 in from preset in values
                                               select Service.Configuration.IsEnabled(preset))
                        {
                            Service.ChatGui.Print(item3.ToString());
                        }
                        break;
                    case "unset":
                        foreach (bool item4 in from preset in values
                                               select !Service.Configuration.IsEnabled(preset))
                        {
                            Service.ChatGui.Print(item4.ToString());
                        }
                        break;
                    case "all":
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                Service.ChatGui.Print(values[i]);
                            }
                            break;
                        }
                    default:
                        Service.ChatGui.PrintError("Available list filters: set, unset, all");
                        break;
                }
                break;
            default:
                configWindow.Toggle();
                break;
        }
        Service.Configuration.Save();
    }
}
