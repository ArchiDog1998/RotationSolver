using Dalamud.Interface.Windowing;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Actions;
using RotationSolver.Commands;
using RotationSolver.Basic.Helpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Interface.Colors;
using RotationSolver.Localization;
using Lumina.Excel.GeneratedSheets;
using ImGuiScene;

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
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoMouseInputs;
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

    static void DrawCommand<T>(T command, Func<T, string> getHelp = null) where T : struct, Enum
    {
        var cmdStr = command.GetCommandStr();

        if (ImGui.Button(command.ToString()))
        {
            Service.CommandManager.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            var help = getHelp?.Invoke(command);
            if (!string.IsNullOrEmpty(help))
            {
                ImGui.SetTooltip(help);
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }
    }

    public override  void Draw()
    {
        ImGui.Columns(2, "Control Bolder", false);
        ImGui.SetColumnWidth(0, DrawNextAction() + ImGui.GetStyle().ColumnsMinSpacing * 2);

        DrawCommand(StateCommandType.Smart, EnumTranslations.ToHelp);

        DrawCommand(StateCommandType.Manual, EnumTranslations.ToHelp);

        DrawCommand(StateCommandType.Cancel, EnumTranslations.ToHelp);


        ImGui.NextColumn();
        var rotation = RotationUpdater.RightNowRotation;
        DrawCommandAction(rotation.ActionHealAreaGCD, rotation.ActionHealAreaAbility, 
            SpecialCommandType.HealArea, ImGuiColors.HealerGreen);

        ImGui.SameLine();

        DrawCommandAction(rotation.ActionHealSingleGCD, rotation.ActionHealSingleAbility,
            SpecialCommandType.HealSingle, ImGuiColors.HealerGreen);

        ImGui.SameLine();

        DrawCommandAction(rotation.ActionDefenseAreaGCD, rotation.ActionDefenseAreaAbility,
            SpecialCommandType.DefenseArea, ImGuiColors.TankBlue);

        ImGui.SameLine();

        DrawCommandAction(rotation.ActionDefenseSingleGCD, rotation.ActionDefenseSingleAbility,
            SpecialCommandType.DefenseSingle, ImGuiColors.TankBlue);

        DrawCommandAction(rotation.ActionMoveForwardGCD, rotation.ActionMoveForwardAbility,
            SpecialCommandType.MoveForward, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(null, rotation.ActionMoveBackAbility,
            SpecialCommandType.MoveBack, ImGuiColors.DalamudOrange);

        DrawCommandAction(rotation.EsunaStanceNorthGCD, rotation.EsunaStanceNorthAbility,
            SpecialCommandType.EsunaStanceNorth, ImGuiColors.ParsedGold);

        ImGui.SameLine();

        DrawCommandAction(rotation.RaiseShirkGCD, rotation.RaiseShirkAbility,
            SpecialCommandType.RaiseShirk, ImGuiColors.ParsedBlue);

        ImGui.SameLine();

        DrawCommandAction(null, rotation.AntiKnockbackAbility,
            SpecialCommandType.AntiKnockback, ImGuiColors.DalamudWhite2);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

        DrawCommand(SpecialCommandType.Burst, EnumTranslations.ToHelp);

        ImGui.SameLine();

        DrawCommand(SpecialCommandType.EndSpecial, EnumTranslations.ToHelp);
    }

    static void DrawCommandAction(IAction gcd, IAction ability, SpecialCommandType command, Vector4 color)
    {
        var gcdW = Service.Config.ControlWindowGCDSize;
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = gcdW + abilityW + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().ItemInnerSpacing.X * 4;

        ImGui.BeginGroup();
        var str = command.ToString();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.TextColored(color, str);

        DrawIAction(gcd, gcdW, command);
        ImGuiHelper.HoveredString(command.ToHelp());

        ImGui.SameLine();
        DrawIAction(ability, abilityW, command);
        ImGuiHelper.HoveredString(command.ToHelp());
        ImGui.EndGroup();
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

    static void DrawIAction(IAction action, float width, SpecialCommandType command)
    {
        if(ImGui.ImageButton(GetTexture(action).ImGuiHandle, new Vector2(width, width)))
        {
            Service.CommandManager.ProcessCommand(command.GetCommandStr());
        }
    }

    static void DrawIAction(IAction action, float width)
    {
        ImGui.Image(GetTexture(action).ImGuiHandle, new Vector2(width, width));
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
        ImGui.ProgressBar(group->Elapsed / group->Total, new Vector2(width, 5));

        DrawIAction(ActionUpdater.NextGCDAction, gcd);

        var next = ActionUpdater.NextGCDAction != ActionUpdater.NextAction ? ActionUpdater.NextAction : null;

        ImGui.SameLine();
        DrawIAction(next, ability);

        return width;
    }
}
