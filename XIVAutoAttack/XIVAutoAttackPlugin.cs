using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack;

public sealed class XIVAutoAttackPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pattack";

    private const string _lockCommand = "/aauto";

    private readonly WindowSystem windowSystem;
    private readonly OverlayWindow overlayWindow;

    private readonly ConfigWindow configWindow;
    //private readonly SystemSound sound;
    public string Name => "XIV Auto Attack";

    internal static readonly ClassJob[] AllJobs = Service.DataManager.GetExcelSheet<ClassJob>().ToArray();

    public XIVAutoAttackPlugin(DalamudPluginInterface pluginInterface, CommandManager commandManager)
    {
        commandManager.AddHandler(_command, new CommandInfo(OnCommand)
        {
            HelpMessage = "打开一个设置各个职业是否启用自动攻击的窗口",
            ShowInHelp = true,
        });

        commandManager.AddHandler(_lockCommand, new CommandInfo(TargetObject)
        {
            HelpMessage = "设置攻击的模式",
            ShowInHelp = true,
        });

        pluginInterface.Create<Service>();
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();

        Service.IconReplacer = new IconReplacer();

        configWindow = new ConfigWindow();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(configWindow);

        overlayWindow = new OverlayWindow();
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += overlayWindow.Draw;
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        MajorUpdater.Enable();
        Watcher.Enable();

    }


    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
        CommandController.AutoAttack = false;
    }

    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.CommandManager.RemoveHandler(_lockCommand);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.Interface.UiBuilder.Draw -= overlayWindow.Draw;
        Service.IconReplacer.Dispose();

        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;

        MajorUpdater.Dispose();
        Watcher.Dispose();
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }

    internal void TargetObject(string command, string arguments)
    {
        string[] array = arguments.Split();

        if (array.Length > 0)
        {
            CommandController.DoAutoAttack(array[0]);
        }
    }



    private void OnCommand(string command, string arguments)
    {

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
                        if (value.JobName.ToLowerInvariant() == text3)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} SET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            case "toggle":
                {
                    string text = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var customComboPreset2 = IconReplacer.CustomCombos[i];
                        if (customComboPreset2.JobName.ToLowerInvariant() == text)
                        {
                            customComboPreset2.IsEnabled = !customComboPreset2.IsEnabled;
                            Service.ChatGui.Print(customComboPreset2.JobName + " " + (customComboPreset2.IsEnabled ? "SET" : "UNSET"));
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            case "unset":
                {
                    string text2 = array[1].ToLowerInvariant();
                    for (int i = 0; i < IconReplacer.CustomCombos.Length; i++)
                    {
                        var value = IconReplacer.CustomCombos[i];
                        if (value.JobName.ToLowerInvariant() == text2)
                        {
                            value.IsEnabled = true;
                            Service.ChatGui.Print($"{value} UNSET");
                            break;
                        }
                    }
                    Service.Configuration.Save();
                    break;
                }
            default:
                configWindow.Toggle();
                break;
        }
        Service.Configuration.Save();
    }
}
