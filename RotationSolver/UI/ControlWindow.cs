using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using ImGuiScene;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System;
using System.Buffers.Text;
using System.Numerics;

namespace RotationSolver.UI;

internal class ControlWindow : Window
{
    const ImGuiWindowFlags _baseFlags = ImGuiWindowFlags.NoScrollbar
                                    | ImGuiWindowFlags.NoCollapse
                                    | ImGuiWindowFlags.NoTitleBar
                                    | ImGuiWindowFlags.NoNav
                                    | ImGuiWindowFlags.NoScrollWithMouse;

    public ControlWindow()
        : base(nameof(ControlWindow), _baseFlags)
    {
        this.IsOpen = true;
    }

    public override void PreDraw()
    {
        Vector4 bgColor = Service.Config.IsControlWindowLock
            ? Service.Config.ControlWindowLockBg
            : Service.Config.ControlWindowUnlockBg;
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

        Flags = _baseFlags;

        if (Service.Config.IsControlWindowLock)
        {
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(2);
        base.PostDraw();
    }

    public override void Draw()
    {
        ImGui.Columns(2, "Control Bolder", false);
        ImGui.SetColumnWidth(0, DrawNextAction() + ImGui.GetStyle().ColumnsMinSpacing * 2);

        DrawCommandAction(61822, StateCommandType.Smart, ImGuiColors.DPSRed);

        ImGui.SameLine();

        DrawCommandAction(61751, StateCommandType.Manual, ImGuiColors.DPSRed);

        DrawCommandAction(61764, StateCommandType.Cancel, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        RotationConfigWindow.DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsControlWindowLock,
            ref Service.Config.IsControlWindowLock);

        ImGui.NextColumn();

        DrawSpecials();

        ImGui.Columns(1);
        ImGui.Separator();

        if(RotationUpdater.RightNowRotation!= null)
        {
            foreach (var pair in RotationUpdater.AllGroupedActions)
            {
                ImGui.Text(pair.Key);
                bool started = false;
                foreach(var item in pair)
                {
                    if (started)
                    {
                        ImGui.SameLine();
                    }
                    DrawActionCooldown(item);
                    started = true;
                }
            }
        }
    }

    private static void DrawActionCooldown(IAction act)
    {
        if (act is IBaseAction a && a.IsGeneralGCD) return;

        var width = Service.Config.ControlWindow0GCDSize;
        var recast = act.RecastTimeOneCharge;
        var elapsed = act.RecastTimeElapsed;

        ImGui.BeginGroup();
        var pos = ImGui.GetCursorPos();
        DrawIAction(act, width);
        ImGuiHelper.HoveredString(act.Name);

        var ratio = recast == 0 ? 0 : elapsed % recast / recast;
        ImGui.SetCursorPos(new Vector2(pos.X + width * ratio, pos.Y));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 0.7f));
        ImGui.ProgressBar(0, new Vector2(width * (1 - ratio), width), string.Empty);
        ImGui.PopStyleColor();

        string time = recast == 0 || !act.EnoughLevel ? "0" : ((int)(recast - elapsed % recast)).ToString();
        var strSize = ImGui.CalcTextSize(time);
        ImGui.SetCursorPos(new Vector2(pos.X + width / 2 - strSize.X / 2, pos.Y + width / 2 - strSize.Y / 2));
        ImGui.Text(time);

        if(act is IBaseAction bAct && bAct.MaxCharges > 1)
        {
            //Draw charges here.
        }

        ImGui.EndGroup();
    }

    private static void DrawSpecials()
    {
        var rotation = RotationUpdater.RightNowRotation;
        DrawCommandAction(rotation?.ActionHealAreaGCD, rotation?.ActionHealAreaAbility,
            SpecialCommandType.HealArea, ImGuiColors.HealerGreen);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionHealSingleGCD, rotation?.ActionHealSingleAbility,
            SpecialCommandType.HealSingle, ImGuiColors.HealerGreen);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionDefenseAreaGCD, rotation?.ActionDefenseAreaAbility,
            SpecialCommandType.DefenseArea, ImGuiColors.TankBlue);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionDefenseSingleGCD, rotation?.ActionDefenseSingleAbility,
            SpecialCommandType.DefenseSingle, ImGuiColors.TankBlue);

        DrawCommandAction(rotation?.ActionMoveForwardGCD, rotation?.ActionMoveForwardAbility,
            SpecialCommandType.MoveForward, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionMoveBackAbility,
            SpecialCommandType.MoveBack, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(61804, SpecialCommandType.Burst, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        DrawCommandAction(61753, SpecialCommandType.EndSpecial, ImGuiColors.DalamudWhite2);

        DrawCommandAction(rotation?.EsunaStanceNorthGCD, rotation?.EsunaStanceNorthAbility,
            SpecialCommandType.EsunaStanceNorth, ImGuiColors.ParsedGold);

        ImGui.SameLine();

        DrawCommandAction(rotation?.RaiseShirkGCD, rotation?.RaiseShirkAbility,
            SpecialCommandType.RaiseShirk, ImGuiColors.ParsedBlue);

        ImGui.SameLine();

        DrawCommandAction(rotation?.AntiKnockbackAbility,
            SpecialCommandType.AntiKnockback, ImGuiColors.DalamudWhite2);
    }

    static void DrawCommandAction(IAction gcd, IAction ability, SpecialCommandType command, Vector4 color)
    {
        var gcdW = Service.Config.ControlWindowGCDSize;
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = gcdW + abilityW + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().ItemInnerSpacing.X * 4;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        ImGui.BeginGroup();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
        ImGui.TextColored(color, str);

        var help = GetHelp(command);
        string baseId = "ImgButton" + command.ToString();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));

        DrawIAction(GetTexture(gcd).ImGuiHandle, baseId + nameof(gcd), gcdW, command, help);
        ImGui.SameLine();
        DrawIAction(GetTexture(ability).ImGuiHandle, baseId + nameof(ability), abilityW, command, help);
        ImGui.EndGroup();
    }

    static void DrawCommandAction(IAction ability, SpecialCommandType command, Vector4 color)
    {
        DrawCommandAction(GetTexture(ability), command, color);
    }

    static void DrawCommandAction(uint iconId, SpecialCommandType command, Vector4 color)
    {
        DrawCommandAction(IconSet.GetTexture(iconId), command, color);
    }

    static void DrawCommandAction(TextureWrap texture, SpecialCommandType command, Vector4 color)
    {
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = abilityW + ImGui.GetStyle().ItemInnerSpacing.X * 2;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        ImGui.BeginGroup();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
        ImGui.TextColored(color, str);

        var help = GetHelp(command);
        string baseId = "ImgButton" + command.ToString();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
        DrawIAction(texture.ImGuiHandle, baseId, abilityW, command, help);
        ImGui.EndGroup();
    }

    static void DrawCommandAction(uint iconId, StateCommandType command, Vector4 color)
    {
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = abilityW + ImGui.GetStyle().ItemInnerSpacing.X * 2;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        ImGui.BeginGroup();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
        ImGui.TextColored(color, str);

        var help = GetHelp(command);
        string baseId = "ImgButton" + command.ToString();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
        DrawIAction(IconSet.GetTexture(iconId).ImGuiHandle, baseId, abilityW, command, help);
        ImGui.EndGroup();
    }

    static string GetHelp(SpecialCommandType command)
    {
        var help = command.ToHelp() + "\n ";
        if (Service.Config.ButtonSpecial.TryGetValue(command, out var button))
        {
            help += "\n" + button.ToStr();
            if (!Service.Config.UseGamepadCommand) help += LocalizationManager.RightLang.ConfigWindow_Control_NeedToEnable;

        }
        if (Service.Config.KeySpecial.TryGetValue(command, out var key))
        {
            help += "\n" + key.ToStr();
            if (!Service.Config.UseKeyboardCommand) help += LocalizationManager.RightLang.ConfigWindow_Control_NeedToEnable;
        }
        return help += "\n \n" + LocalizationManager.RightLang.ConfigWindow_Control_ResetButtonOrKeyCommand;
    }

    static string GetHelp(StateCommandType command)
    {
        var help = command.ToHelp() + "\n ";
        if (Service.Config.ButtonState.TryGetValue(command, out var button))
        {
            help += "\n" + button.ToStr();
            if (!Service.Config.UseGamepadCommand) help += LocalizationManager.RightLang.ConfigWindow_Control_NeedToEnable;
        }
        if (Service.Config.KeyState.TryGetValue(command, out var key))
        {
            help += "\n" + key.ToStr();
            if (!Service.Config.UseKeyboardCommand) help += LocalizationManager.RightLang.ConfigWindow_Control_NeedToEnable;

        }
        return help += "\n \n" + LocalizationManager.RightLang.ConfigWindow_Control_ResetButtonOrKeyCommand;
    }

    static readonly Dictionary<uint, uint> _actionIcons = new Dictionary<uint, uint>();

    static TextureWrap GetTexture(IAction action)
    {
        uint iconId = 0;
        if(action != null && !_actionIcons.TryGetValue(action.AdjustedID, out iconId))
        {
            _actionIcons[action.AdjustedID] = iconId =
                Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(action.AdjustedID).Icon;
        }
        return IconSet.GetTexture(iconId);
    }

    static void DrawIAction(nint handle, string id, float width, SpecialCommandType command, string help)
    {
        ImGui.PushID(id);
        if (ImGui.ImageButton(handle, new Vector2(width, width)))
        {
            Service.CommandManager.ProcessCommand(command.GetCommandStr());
        }
        ImGui.PopID();
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(help)) ImGui.SetTooltip(help);
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                if(ImGui.IsKeyPressed(ImGuiKey.LeftCtrl))
                {
                    Service.Config.KeySpecial.Remove(command);
                    Service.Config.ButtonSpecial.Remove(command);
                    Service.Config.Save();

                    Service.ToastGui.ShowQuest($"Clear Recording: {command}",
                        new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = 101,
                            PlaySound = true,
                            DisplayCheckmark = true,
                        });
                }
                if(InputUpdater.RecordingSpecialType == SpecialCommandType.None)
                {
                    InputUpdater.RecordingTime = DateTime.Now;
                    InputUpdater.RecordingSpecialType = command;
                    Service.ToastGui.ShowQuest($"Recording: {command}",
                        new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = 101,
                        });
                }
            }
        }
    }

    static void DrawIAction(nint handle, string id, float width, StateCommandType command, string help)
    {
        ImGui.PushID(id);
        if (ImGui.ImageButton(handle, new Vector2(width, width)))
        {
            Service.CommandManager.ProcessCommand(command.GetCommandStr());
        }
        ImGui.PopID();
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(help)) ImGui.SetTooltip(help);
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                if (ImGui.IsKeyPressed(ImGuiKey.LeftCtrl))
                {
                    Service.Config.KeyState.Remove(command);
                    Service.Config.ButtonState.Remove(command);
                    Service.Config.Save();

                    Service.ToastGui.ShowQuest($"Clear Recording: {command}",
                        new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = 101,
                            PlaySound = true,
                            DisplayCheckmark = true,
                        });
                }
                if (InputUpdater.RecordingStateType == StateCommandType.None)
                {
                    InputUpdater.RecordingTime = DateTime.Now;
                    InputUpdater.RecordingStateType = command;
                    Service.ToastGui.ShowQuest($"Recording: {command}",
                        new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = 101,
                        });
                }
            }
        }
    }

    static void DrawIAction(IAction action, float width)
    {
        DrawIAction(GetTexture(action).ImGuiHandle, width);
    }

    static void DrawIAction(nint handle, float width)
    {
        ImGui.Image(handle, new Vector2(width, width));
    }

    static unsafe float  DrawNextAction()
    {
        var group = ActionManager.Instance()->GetRecastGroupDetail(ActionHelper.GCDCooldownGroup - 1);
        var remain = group->Total - group->Elapsed;

        var gcd = Service.Config.ControlWindowGCDSize * Service.Config.ControlWindowNextSizeRatio;
        var ability = Service.Config.ControlWindow0GCDSize * Service.Config.ControlWindowNextSizeRatio;
        var width = gcd + ability + ImGui.GetStyle().ItemSpacing.X;

        var str = "Next Action";
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.TextColored(ImGuiColors.DalamudYellow, str);
        str = remain.ToString("F2") + "s";

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.Text(str);
        ImGui.ProgressBar(group->Elapsed / group->Total, new Vector2(width, Service.Config.ControlProgressHeight), string.Empty);

        DrawIAction(ActionUpdater.NextGCDAction, gcd);

        var next = ActionUpdater.NextGCDAction != ActionUpdater.NextAction ? ActionUpdater.NextAction : null;

        ImGui.SameLine();
        DrawIAction(next, ability);

        return width;
    }
}
