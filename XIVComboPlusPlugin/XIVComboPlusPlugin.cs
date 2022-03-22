using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using XIVComboPlus;

namespace XIVComboPlus;

public sealed class XIVComboPlusPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pcombo";

    private const string _lockCommand = "/plock";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;
    private static IntPtr func;

    public string Name => "XIV Combo Plus";
    public static BattleNpc[] Targets25 => GetObjectInRadius(Targets, 25f);

    private static BattleNpc[] Targets =>
            Service.ObjectTable.Where(obj => obj is BattleNpc && ((BattleNpc)obj).CurrentHp != 0 && ((BattleNpc)obj).BattleNpcKind == BattleNpcSubKind.Enemy && canAttack(obj)).Select(obj => (BattleNpc)obj).ToArray();

    public static PlayerCharacter[] PartyMembers =>
            AllianceMenbers.Where(fri => (fri.StatusFlags&StatusFlags.AllianceMember) != 0).ToArray();
    public static PlayerCharacter[] AllianceMenbers =>
         Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();
    public XIVComboPlusPlugin(DalamudPluginInterface pluginInterface, Framework framework, CommandManager commandManager, SigScanner sigScanner, [RequiredVersion("1.0")] ClientState clientState)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup();
        Service.IconReplacer = new IconReplacer();
        configWindow = new ConfigWindow();
        windowSystem = new WindowSystem(Name);
        windowSystem.AddWindow(configWindow);
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;

        func = sigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");

        CommandInfo val = new CommandInfo(new CommandInfo.HandlerDelegate(OnCommand));
        val.HelpMessage = "Open a window to edit custom combo settings.";
        val.ShowInHelp = true;
        commandManager.AddHandler(_command, val);

        CommandInfo lockInfo = new CommandInfo(new CommandInfo.HandlerDelegate(OnLock));
        lockInfo.HelpMessage = "锁定想要的敌人。";
        lockInfo.ShowInHelp = true;
        commandManager.AddHandler(_lockCommand, lockInfo);
    }

    public unsafe static bool canAttack(GameObject actor)
    {
        if ((object)actor == null)
        {
            return false;
        }
        return ((delegate*<long, IntPtr, long>)(void*)func)(142L, actor.Address) == 1;
    }

    private static T[] GetObjectInRadius<T>(T[] objects, float radius) where T : GameObject
    {
        return objects.Where(o => DistanceToPlayer(o) <= radius + o.HitboxRadius).ToArray();
    }

    private static T GetMostObjectInRadius<T>(T[] objects, float radius, float range) where T : BattleChara
    {
        return (from t in GetObjectInRadius(objects, radius)
                   select (t, Calculate(t, objects, radius, range)) into set
                   where set.Item2 > 1
                   orderby set.Item2 select set.t).Last();

        static float Calculate(T t, T[] objects, float radius, float range)
        {
            byte count = 0;
            foreach (T obj in GetObjectInRadius(objects, radius + range))
            {
                if (Vector3.Distance(t.Position, obj.Position) <= range + obj.HitboxRadius)
                {
                    count++;
                }
            }
            return count + (float)t.CurrentHp / t.MaxHp;
        }
    }

    private static float DistanceToPlayer(GameObject obj)
    {
        return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
    }

    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(_command);
        Service.CommandManager.RemoveHandler(_lockCommand);
        Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
        Service.IconReplacer.Dispose();
    }

    private void OnOpenConfigUi()
    {
        configWindow.IsOpen = true;
    }
    private void OnLock(string command, string arguments)
    {
        string[] array = arguments.Split();

        switch (array[0])
        {
            case "HMHP":
                SetTarget(Targets25.OrderByDescending(tar => tar.MaxHp).First());
                break;

            case "LMHP":
                SetTarget(Targets25.OrderBy(tar => tar.MaxHp).First());
                break;

            case "Area":
                SetTarget(GetMostObjectInRadius(Targets, 25, 5));                
                break;

            case "PLHP60":
                PlayerCharacter lowChara = PartyMembers.OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
                if((float)lowChara.CurrentHp / lowChara.MaxHp < 0.6) SetTarget(lowChara);
                break;

            case "HArea":
                SetTarget(GetMostObjectInRadius(PartyMembers, 30, 8));
                break;
        }
    }

    private void SetTarget(GameObject? obj)
    {
        Service.TargetManager.SetTarget(obj);
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
