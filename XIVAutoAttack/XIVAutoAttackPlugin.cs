using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Windows;
using Action = Lumina.Excel.GeneratedSheets.Action;

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
    }

    private void UiBuilder_Draw()
    {
        const int COUNT = 20;

        if (CustomCombo.EnemyTarget == null) return;
        if (CustomCombo.ShouldLocation is Actions.EnemyLocation.None or Actions.EnemyLocation.Front) return;

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
            case Actions.EnemyLocation.Side:
                SectorPlots(ref pts, pPosition, Math.PI * 0.25 + rotation, radius, COUNT);
                pts.Add(scrPos);
                SectorPlots(ref pts, pPosition, Math.PI * 1.25 + rotation, radius, COUNT);
                break;
            case Actions.EnemyLocation.Back:
                SectorPlots(ref pts, pPosition, Math.PI * 0.75 + rotation, radius, COUNT);
                break;
            default:
                return;
        }
        pts.Add(scrPos);

        bool wrong = CustomCombo.ShouldLocation != CustomCombo.FindEnemyLocation(CustomCombo.EnemyTarget)
                    && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TrueNorth);
        var color = wrong ? new Vector3(0.3f, 0.8f, 0.2f) : new Vector3(1, 1, 1);

        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 1f)), ImDrawFlags.None, 3);
        pts.ForEach(pt => ImGui.GetWindowDrawList().PathLineTo(pt));
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(new Vector4(color.X, color.Y, color.Z, 0.2f)));

        ImGui.End();
        ImGui.PopStyleVar();
    }

    private void SectorPlots(ref List<Vector2> pts, Vector3 centre, double rotation, float radius, int segments)
    {
        var step = Math.PI / 2 / segments;
        var rstep = radius / (segments * 5);
        for (int i = 0; i <= segments; i++)
        {
            for (var tryRadius = radius; tryRadius >= 0; tryRadius -= rstep)
            {
                if (Service.GameGui.WorldToScreen(ChangePoint(centre, tryRadius, rotation + i * step),
                    out var pt))
                {
                    pts.Add(pt);
                    break;
                }
            }
        }
    }

    private Vector3 ChangePoint(Vector3 pt, double radius, double rotation)
    {
        var x = Math.Sin(rotation) * radius + pt.X;
        var z = Math.Cos(rotation) * radius + pt.Z;
        return new Vector3((float)x, pt.Y, (float)z);
    }

    //private void DrawWindow()
    //{

    //    var actor = _cs.LocalPlayer;
    //    if (!_gui.WorldToScreen(
    //        new Num.Vector3(actor.Position.X, actor.Position.Y, actor.Position.Z),
    //        out var pos)) return;

    //    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));
    //    ImGuiHelpers.ForceNextWindowMainViewport();
    //    ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(0, 0));
    //    ImGui.Begin("Ring",
    //        ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
    //        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
    //    ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

    //    if (_enabled)
    //    {
    //        ImGui.GetWindowDrawList().AddCircleFilled(
    //            new Num.Vector2(pos.X, pos.Y),
    //            2f,
    //            ImGui.GetColorU32(_col),
    //            100);
    //    }
    //    if (_circle)
    //    {
    //        ImGui.GetWindowDrawList().AddCircle(
    //            new Num.Vector2(pos.X, pos.Y),
    //            2.2f,
    //            ImGui.GetColorU32(_col2),
    //            100);
    //    }

    //    if (_ring)
    //    {
    //        DrawRingWorld(_cs.LocalPlayer, _radius, _segments, _thickness,
    //            ImGui.GetColorU32(_colRing));
    //    }
    //    if (_ring2)
    //    {
    //        DrawRingWorld(_cs.LocalPlayer, _radius2, _segments2, _thickness2,
    //            ImGui.GetColorU32(_colRing2));
    //    }

    //    if (_north1)
    //    {
    //        //Tip of arrow
    //        _gui.WorldToScreen(new Num.Vector3(
    //                    actor.Position.X + ((_lineLength + _lineOffset) * (float)Math.Sin(Math.PI)),
    //                    actor.Position.Y,
    //                    actor.Position.Z + ((_lineLength + _lineOffset) * (float)Math.Cos(Math.PI))
    //                ),
    //                out Num.Vector2 lineTip);
    //        //Player + offset
    //        _gui.WorldToScreen(new Num.Vector3(
    //                actor.Position.X + (_lineOffset * (float)Math.Sin(Math.PI)),
    //                actor.Position.Y,
    //                actor.Position.Z + (_lineOffset * (float)Math.Cos(Math.PI))
    //            ),
    //            out Num.Vector2 lineOffset);
    //        //Chev offset1
    //        _gui.WorldToScreen(new Num.Vector3(
    //                actor.Position.X + (_chevOffset * (float)Math.Sin(Math.PI / _chevRad) * _chevSin),
    //                actor.Position.Y,
    //                actor.Position.Z + (_chevOffset * (float)Math.Cos(Math.PI / _chevRad) * _chevSin)
    //            ),
    //            out Num.Vector2 chevOffset1);
    //        //Chev offset2
    //        _gui.WorldToScreen(new Num.Vector3(
    //                actor.Position.X + (_chevOffset * (float)Math.Sin(Math.PI / -_chevRad) * _chevSin),
    //                actor.Position.Y,
    //                actor.Position.Z + (_chevOffset * (float)Math.Cos(Math.PI / -_chevRad) * _chevSin)
    //            ),
    //            out Num.Vector2 chevOffset2);
    //        //Chev Tip
    //        _gui.WorldToScreen(new Num.Vector3(
    //                actor.Position.X + ((_chevOffset + _chevLength) * (float)Math.Sin(Math.PI)),
    //                actor.Position.Y,
    //                actor.Position.Z + ((_chevOffset + _chevLength) * (float)Math.Cos(Math.PI))
    //            ),
    //            out Num.Vector2 chevTip);
    //        if (_north2)
    //        {
    //            ImGui.GetWindowDrawList().AddLine(new Num.Vector2(lineTip.X, lineTip.Y), new Num.Vector2(lineOffset.X, lineOffset.Y),
    //                ImGui.GetColorU32(_lineCol), _lineThicc);
    //        }
    //        if (_north3)
    //        {
    //            ImGui.GetWindowDrawList().AddLine(new Num.Vector2(chevTip.X, chevTip.Y), new Num.Vector2(chevOffset1.X, chevOffset1.Y),
    //                ImGui.GetColorU32(_chevCol), _chevThicc);
    //            ImGui.GetWindowDrawList().AddLine(new Num.Vector2(chevTip.X, chevTip.Y), new Num.Vector2(chevOffset2.X, chevOffset2.Y),
    //                ImGui.GetColorU32(_chevCol), _chevThicc);
    //        }
    //    }

    //    ImGui.End();
    //    ImGui.PopStyleVar();
    //}


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

        watcher.Dispose();
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
            case "EsunaShield":
                IconReplacer.StartEsunaOrShield();
                break;
            case "RaiseShirk":
                IconReplacer.StartRaiseOrShirk();
                break;
            case "Move":
                IconReplacer.StartMove();
                break;
            case "AntiRepulsion":
                IconReplacer.StartAntiRepulsion();
                break;
            case "BreakProvoke":
                IconReplacer.StartBreakOrProvoke();
                break;
            case "AutoBreak":
                Service.Configuration.AutoBreak = !Service.Configuration.AutoBreak;
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

            default:
                Service.ChatGui.PrintError("无法识别：" + str);
                break;
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
