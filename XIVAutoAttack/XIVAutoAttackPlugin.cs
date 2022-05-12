using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using XIVComboPlus;
using XIVComboPlus.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus;

public sealed class XIVAutoAttackPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pattack";

    private const string _lockCommand = "/aauto";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;
    //private readonly SystemSound sound;
    public string Name => "XIV Auto Attack";

    private static Framework _framework;

    internal static Lumina.Excel.GeneratedSheets.ClassJob[] AllJobs;

    public  XIVAutoAttackPlugin(DalamudPluginInterface pluginInterface, Framework framework, CommandManager commandManager, SigScanner sigScanner)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();
        Service.IconReplacer = new IconReplacer();
        configWindow = new ConfigWindow();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(configWindow);
        //sound = new SystemSound();
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;

        TargetHelper.Init(sigScanner);

        CommandInfo val = new CommandInfo(new CommandInfo.HandlerDelegate(OnCommand));
        val.HelpMessage = "打开一个设置各个职业是否启用自动攻击的窗口";
        val.ShowInHelp = true;
        commandManager.AddHandler(_command, val);

        CommandInfo lockInfo = new CommandInfo(new CommandInfo.HandlerDelegate(TargetObject));
        lockInfo.HelpMessage = "设置攻击的模式";
        lockInfo.ShowInHelp = true;
        commandManager.AddHandler(_lockCommand, lockInfo);

        _framework = framework;
        framework.Update += TargetHelper.Framework_Update;

        AllJobs = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>().Where(x => x.JobIndex != 0).ToArray();

        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;


    }

    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
        IconReplacer.AutoAttack = false;
    }

    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.CommandManager.RemoveHandler(_lockCommand);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.IconReplacer.Dispose();
        _framework.Update -= TargetHelper.Framework_Update;
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }

    internal unsafe void TargetObject(string command, string arguments)
    {
        string[] array = arguments.Split();


        if (array.Length > 0)
        {
            switch (array[0])
            {
                case "HealArea":
                    IconReplacer.StartHealArea();
                    break;
                case "HealSingle":
                    IconReplacer.StartHealSingle();
                    break;
                case "DefenseArea":
                    IconReplacer.StartDefenseArea();
                    break;
                case "DefenseSingle":
                    IconReplacer.StartDefenseSingle();
                    break;
                case "Esuna":
                    IconReplacer.StartEsuna();
                    break;
                case "Raise":
                    IconReplacer.StartRaise();
                    break;
                case "Move":
                    IconReplacer.StartMove();
                    break;
                case "AntiRepulsion":
                    IconReplacer.StartAntiRepulsion();
                    break;
                case "Break":
                    IconReplacer.StartBreak();
                    break;
                case "AttackBig":
                    IconReplacer.AttackBig = true;
                    break;
                case "AttackSmall":
                    IconReplacer.AttackBig = false;
                    break;
                case "AttackManual":
                    IconReplacer.AutoTarget = false;
                    IconReplacer.AutoAttack = true;
                    break;
                case "AttackCancel":
                    IconReplacer.AutoAttack = false;
                    break;
                    break;
            }
        }
    }

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
                            Service.ChatGui.Print(customComboPreset2.JobName + " " + (customComboPreset2.IsEnabled ? "SET": "UNSET"));
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
