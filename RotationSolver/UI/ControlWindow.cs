using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
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
    public ControlWindow()
        : base(nameof(ControlWindow), NextActionWindow.BaseFlags)
    {
        Size = new Vector2(540f, 490f);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        Vector4 bgColor = Service.Config.IsControlWindowLock
            ? Service.Config.ControlWindowLockBg
            : Service.Config.ControlWindowUnlockBg;
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

        Flags = NextActionWindow.BaseFlags;

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

        if(MajorUpdater.IsValid && Service.Config.ControlShowCooldown && RotationUpdater.RightNowRotation!= null)
        {
            ImGui.Separator();
            foreach (var pair in RotationUpdater.AllGroupedActions)
            {
                var showItems = pair.Where(i => !(i is IBaseAction a && a.IsGeneralGCD)).OrderBy(a => a.ID);

                if (!showItems.Any()) continue;

                ImGui.Text(pair.Key);

                uint started = 0;
                foreach(var item in showItems)
                {
                    if (started % 14 != 0)
                    {
                        ImGui.SameLine();
                    }
                    DrawActionCooldown(item);
                    started ++;
                }
            }
        }
    }

    static readonly uint black = ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1));
    static readonly uint white = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1));
    static readonly uint progressCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.6f, 0.6f, 0.6f));

    static void TextShade(Vector2 pos, string text, float width, uint fore, uint background)
    {
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(0, width), background, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(0, -width), background, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(width, 0), background, text);
        ImGui.GetWindowDrawList().AddText(pos - new Vector2(-width, 0), background, text);
        ImGui.GetWindowDrawList().AddText(pos, fore, text);
    }

    private static void DrawActionCooldown(IAction act)
    {
        var width = Service.Config.ControlWindow0GCDSize;
        var recast = act.RecastTimeOneCharge;
        var elapsed = act.RecastTimeElapsed;

        ImGui.BeginGroup();
        var pos = ImGui.GetCursorPos();
        var winPos = ImGui.GetWindowPos();

        DrawIAction(act, width);
        ImGuiHelper.HoveredString(act.Name);

        if (!act.EnoughLevel)
        {
            ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.X, pos.Y) + winPos,
                new Vector2(pos.X + width, pos.Y + width) + winPos, progressCol);
        }
        else if (act.IsCoolingDown)
        {
            var ratio = recast == 0 ? 0 : elapsed % recast / recast;
            ImGui.GetWindowDrawList().AddRectFilled(new Vector2(pos.X + width * ratio, pos.Y) + winPos, 
                new Vector2(pos.X + width, pos.Y + width) + winPos, progressCol);

            string time = recast == 0 || !act.EnoughLevel ? "0" : ((int)(recast - elapsed % recast) + 1).ToString();
            var strSize = ImGui.CalcTextSize(time);
            var fontPos = new Vector2(pos.X + width / 2 - strSize.X / 2, pos.Y + width / 2 - strSize.Y / 2) + winPos;

            TextShade(fontPos, time, 1.5f, white, black);
        }

        if(act.EnoughLevel && act is IBaseAction bAct && bAct.MaxCharges > 1)
        {
            for (int i = 0; i < bAct.CurrentCharges; i++)
            {
                ImGui.GetWindowDrawList().AddCircleFilled(winPos + pos + (i + 0.5f) * new Vector2(6, 0), 2.5f, white);
            }
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
            iconId = action is IBaseAction ? Service.GetSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(action.AdjustedID).Icon
                : Service.GetSheet<Lumina.Excel.GeneratedSheets.Item>().GetRow(action.AdjustedID).Icon;
            _actionIcons[action.AdjustedID] = iconId;
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
                else if(InputUpdater.RecordingSpecialType == SpecialCommandType.None)
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
                else if (InputUpdater.RecordingStateType == StateCommandType.None)
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

    internal static void DrawIAction(IAction action, float width)
    {
        DrawIAction(GetTexture(action).ImGuiHandle, width);
    }

    static void DrawIAction(nint handle, float width)
    {
        ImGui.Image(handle, new Vector2(width, width));
    }

    static unsafe float  DrawNextAction()
    {
        var gcd = Service.Config.ControlWindowGCDSize * Service.Config.ControlWindowNextSizeRatio;
        var ability = Service.Config.ControlWindow0GCDSize * Service.Config.ControlWindowNextSizeRatio;
        var width = gcd + ability + ImGui.GetStyle().ItemSpacing.X;

        var str = "Next Action";
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.TextColored(ImGuiColors.DalamudYellow, str);

        NextActionWindow.DrawGcdCooldown(width, true);

        DrawIAction(ActionUpdater.NextGCDAction, gcd);

        var next = ActionUpdater.NextGCDAction != ActionUpdater.NextAction ? ActionUpdater.NextAction : null;

        ImGui.SameLine();
        DrawIAction(next, ability);

        return width;
    }
}
