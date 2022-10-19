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
using XIVAutoAttack.Windows;

namespace XIVAutoAttack;

public sealed class XIVAutoAttackPlugin : IDalamudPlugin, IDisposable
{
    private const string _command = "/pattack";

    private const string _lockCommand = "/aauto";

    private readonly WindowSystem windowSystem;

    private readonly ConfigWindow configWindow;
    //private readonly SystemSound sound;
    public string Name => "XIV Auto Attack";

    internal static Lumina.Excel.GeneratedSheets.ClassJob[] AllJobs;

    internal static DtrBarEntry dtrEntry;

    internal static Watcher watcher;

    internal static MovingController movingController;
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
        Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += windowSystem.Draw;
        Service.Interface.UiBuilder.Draw += UiBuilder_Draw;
        Service.Framework.Update += TargetHelper.Framework_Update;

        AllJobs = Service.DataManager.GetExcelSheet<ClassJob>().ToArray();

        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        watcher = new Watcher();
        watcher.Enable();

        movingController = new MovingController();
    }

    private void UiBuilder_Draw()
    {
        const int COUNT = 20;

        if (CustomCombo.EnemyTarget == null || !Service.Configuration.SayoutLocationWrong) return;
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TrueNorth))return;
        if (CustomCombo.ShouldLocation is EnemyLocation.None or EnemyLocation.Front) return;

        float radius = CustomCombo.EnemyTarget.HitboxRadius + 3.5f;
        float rotation = CustomCombo.EnemyTarget.Rotation;

        Vector3 pPosition = CustomCombo.EnemyTarget.Position;

        if (Service.GameGui == null) return;
        if (!Service.GameGui.WorldToScreen(pPosition, out var scrPos)) return;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGuiHelpers.ForceNextWindowMainViewport();
        ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
        ImGui.Begin("Ring",
            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

        List<Vector2> pts = new List<Vector2>(2 * COUNT + 2);

        pts.Add(scrPos);
        switch (CustomCombo.ShouldLocation)
        {
            case EnemyLocation.Side:
                SectorPlots(ref pts, pPosition, radius, Math.PI * 0.25 + rotation, COUNT);
                pts.Add(scrPos);
                SectorPlots(ref pts, pPosition, radius, Math.PI * 1.25 + rotation, COUNT);
                break;
            case EnemyLocation.Back:
                SectorPlots(ref pts, pPosition, radius, Math.PI * 0.75 + rotation, COUNT);
                break;
            default:
                return;
        }
        pts.Add(scrPos);

        bool wrong = CustomCombo.ShouldLocation != CustomCombo.FindEnemyLocation(CustomCombo.EnemyTarget);
        var color = wrong ? new Vector3(0.3f, 0.8f, 0.2f) : new Vector3(1, 1, 1);

        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 3);
        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 0.2f)));

        ImGui.End();
        ImGui.PopStyleVar();
    }

    private void SectorPlots(ref List<Vector2> pts, Vector3 centre, float radius, double rotation, int segments)
    {
        var step = Math.PI / 2 / segments;
        for (int i = 0; i <= segments; i++)
        {
            Service.GameGui.WorldToScreen(ChangePoint(centre, radius, rotation + i * step), out var pt);
            pts.Add(pt);
        }
    }

    private Vector3 ChangePoint(Vector3 pt, double radius, double rotation)
    {
        var x = Math.Sin(rotation) * radius + pt.X;
        var z = Math.Cos(rotation) * radius + pt.Z;
        return new Vector3((float)x, pt.Y, (float)z);
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
        Service.Interface.UiBuilder.Draw -= UiBuilder_Draw;
        Service.IconReplacer.Dispose();
        TargetHelper.Dispose();
        Service.Framework.Update -= TargetHelper.Framework_Update;
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;

        dtrEntry?.Dispose();
        watcher?.Dispose();
        movingController?.Dispose();
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
            DoAutoAttack(array[0]);
        }
    }

    internal static void DoAutoAttack(string str)
    {
        switch (str)
        {
            case "HealArea":
                IconReplacer.StartHealArea();
                return;
            case "HealSingle":
                IconReplacer.StartHealSingle();
                return;
            case "DefenseArea":
                IconReplacer.StartDefenseArea();
                return;
            case "DefenseSingle":
                IconReplacer.StartDefenseSingle();
                return;
            case "EsunaShield":
                IconReplacer.StartEsunaOrShield();
                return;
            case "RaiseShirk":
                IconReplacer.StartRaiseOrShirk();
                return;
            case "Move":
                IconReplacer.StartMove();
                return;
            case "AntiRepulsion":
                IconReplacer.StartAntiRepulsion();
                return;
            case "BreakProvoke":
                IconReplacer.StartBreakOrProvoke();
                return;
            case "AutoBreak":
                Service.Configuration.AutoBreak = !Service.Configuration.AutoBreak;
                return;
            case "AttackBig":
                IconReplacer.AttackBig = true;
                return;
            case "AttackSmall":
                IconReplacer.AttackBig = false;
                return;
            case "AttackManual":
                IconReplacer.AutoTarget = false;
                IconReplacer.AutoAttack = true;
                return;
            case "AttackCancel":
                IconReplacer.AutoAttack = false;
                return;

            default:
                foreach (CustomCombo customCombo in IconReplacer.CustomCombos)
                {
                    if (customCombo.JobID != Service.ClientState.LocalPlayer.ClassJob.Id) continue;

                    foreach (var boolean in customCombo.Config.bools)
                    {
                        if(boolean.name == str)
                        {
                            boolean.value = !boolean.value;
                            return;
                        }
                    }

                    break;
                }
                Service.ChatGui.PrintError("无法识别：" + str);
                return;
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
