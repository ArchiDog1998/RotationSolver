using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using XIVConfigUI;

namespace RotationSolver.UI;

internal class ControlWindow : CtrlWindow
{
    public static IAction? Wrong { get; set; }
    public static DateTime DidTime { get; set; }

    public ControlWindow()
        : base(nameof(ControlWindow))
    {
        Size = new Vector2(570f, 300f);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override unsafe void Draw()
    {
        ImGui.Columns(3, "Control Bolder", false);
        var gcd = Service.Config.ControlWindowGCDSize
            * Service.Config.ControlWindowNextSizeRatio;
        var ability = Service.Config.ControlWindow0GCDSize
            * Service.Config.ControlWindowNextSizeRatio;
        var width = gcd + ability + ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetColumnWidth(1, 8);

        DrawNextAction(gcd, ability, width);

        ImGui.SameLine();
        var columnWidth = ImGui.GetCursorPosX();
        ImGui.NewLine();

        ImGui.Spacing();

        DrawCommandAction(61751, StateCommandType.Manual, ImGuiColors.DPSRed);

        ImGui.SameLine();
        DrawCommandAction(61764, StateCommandType.Cancel, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();
        columnWidth = Math.Max(columnWidth, ImGui.GetCursorPosX());
        ImGui.NewLine();

        DrawCommandAction(61822, StateCommandType.Auto, ImGuiColors.DPSRed);

        ImGui.SameLine();

        ImGui.BeginGroup();

        ImGui.TextColored(ImGuiColors.DPSRed, DataCenter.TargetingType.Local());

        var value = Service.Config.IsControlWindowLock ? 0 : 1;
        if (ImGuiHelperRS.SelectableCombo("Rotation Solver Lock the Control Window",
        [
            UiString.InfoWindowNoMove.Local(),
            UiString.InfoWindowMove.Local(),
        ], ref value))
        {
            Service.Config.IsControlWindowLock.Value = value == 0;
        }

        ImGui.EndGroup();

        ImGui.SameLine();
        columnWidth = Math.Max(columnWidth, ImGui.GetCursorPosX());
        ImGui.NewLine();

        var color = *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled);

        var isAoe = Service.Config.UseAoeAction
            && (!DataCenter.IsManual
            || Service.Config.UseAoeWhenManual);

        if (!isAoe) ImGui.PushStyleColor(ImGuiCol.Text, color);
        if (ImGuiHelperRS.SelectableButton("AOE"))
        {
            Service.Config.UseAoeAction.Value = !isAoe;
            Service.Config.UseAoeWhenManual.Value = !isAoe;
        }
        if (!isAoe) ImGui.PopStyleColor();

        ImGui.SameLine();

        var isBurst = Service.Config.AutoBurst;
        if (!isBurst) ImGui.PushStyleColor(ImGuiCol.Text, color);
        if (ImGuiHelperRS.SelectableButton("Burst"))
        {
            Service.Config.AutoBurst.Value = !isBurst;
        }
        if (!isBurst) ImGui.PopStyleColor();
        ImGui.SameLine();
        columnWidth = Math.Max(columnWidth, ImGui.GetCursorPosX());
        ImGui.NewLine();

        ImGui.SetColumnWidth(0, columnWidth);

        ImGui.NextColumn();
        ImGui.NextColumn();

        DrawSpecials();

        ImGui.Columns(1);
    }

    private static void DrawSpecials()
    {
        var rotation = DataCenter.RightNowRotation;

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

        ImGui.Spacing();

        DrawCommandAction(rotation?.ActionMoveForwardGCD, rotation?.ActionMoveForwardAbility,
            SpecialCommandType.MoveForward, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionMoveBackAbility,
            SpecialCommandType.MoveBack, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(61397, SpecialCommandType.NoCasting, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        DrawCommandAction(61804, SpecialCommandType.Burst, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionLimitBreak, SpecialCommandType.LimitBreak, ImGuiColors.DalamudWhite2);

        ImGui.Spacing();

        DrawCommandAction(rotation?.ActionDispelStancePositionalGCD, rotation?.ActionDispelStancePositionalAbility,
            SpecialCommandType.DispelStancePositional, ImGuiColors.ParsedGold);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionRaiseShirkGCD, rotation?.ActionRaiseShirkAbility,
            SpecialCommandType.RaiseShirk, ImGuiColors.ParsedBlue);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionAntiKnockbackAbility,
            SpecialCommandType.AntiKnockback, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionSpeedAbility,
            SpecialCommandType.Speed, ImGuiColors.DalamudWhite2);

        ImGui.Spacing();

        DrawCommandAction(61753, SpecialCommandType.EndSpecial, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        using (var group = ImRaii.Group())
        {
            if (group)
            {
                ImGui.Text("CMD:");
                DrawIAction(DataCenter.CommandNextAction, Service.Config.ControlWindow0GCDSize, 1);
            }
        }

        ImGui.SameLine();

        using (var group = ImRaii.Group())
        {
            if (group)
            {
                ImGui.TextWrapped(DataCenter.RightNowTargetToHostileType.Local());
                ImGui.Text("Auto: " + DataCenter.AutoStatus.ToString());
            }
        }

        if (Service.Config.MistakeRatio > 0)
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DPSRed, "    | Mistake | \n    | Mistake | ");
            ImGui.SameLine();

            DrawIAction(DateTime.Now - DidTime < TimeSpan.FromSeconds(5) ? Wrong : null,
                Service.Config.ControlWindowGCDSize, 1);
        }
    }

    static void DrawCommandAction(IAction? gcd, IAction? ability, SpecialCommandType command, Vector4 color)
    {
        var gcdW = Service.Config.ControlWindowGCDSize;
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = gcdW + abilityW + ImGui.GetStyle().ItemSpacing.X;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        var pos = ImGui.GetCursorPos();

        using var group = ImRaii.Group();
        if (!group) return;

        using (var subGroup = ImRaii.Group())
        {
            if (subGroup)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
                ImGui.TextColored(color, str);

                var help = command.Local();
                string baseId = "ImgButton" + command.ToString();

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));

                if (IconSet.GetTexture(gcd, out var texture))
                {
                    var y = ImGui.GetCursorPosY();

                    var gcdHelp = help;
                    if (gcd != null)
                    {
                        gcdHelp += "\n" + gcd.ToString();
                    }
                    DrawIAction(texture.ImGuiHandle, baseId + nameof(gcd), gcdW, command, gcdHelp);
                    if (IconSet.GetTexture(ability, out texture))
                    {
                        ImGui.SameLine();

                        ImGui.SetCursorPosY(y);

                        var abilityHelp = help;
                        if (ability != null)
                        {
                            abilityHelp += "\n" + ability.ToString();
                        }
                        DrawIAction(texture.ImGuiHandle, baseId + nameof(ability), abilityW, command, abilityHelp);
                    }
                }
            }
        }

        if (DataCenter.SpecialType == command)
        {
            var size = ImGui.GetItemRectSize();
            var winPos = ImGui.GetWindowPos();

            HighLight(winPos + pos, size);

            if (DataCenter.SpecialTimeLeft > 0)
            {
                var time = DataCenter.SpecialTimeLeft.ToString("F2") + "s";
                var strSize = ImGui.CalcTextSize(time);
                CooldownWindow.TextShade(winPos + pos + size - strSize, time);
            }
        }
    }

    public static void HighLight(Vector2 pt, Vector2 size, float thickness = 2f)
    {
        var offset = ImGui.GetStyle().ItemSpacing / 2;
        ImGui.GetWindowDrawList().AddRect(pt - offset, pt + size + offset,
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudGrey), 5, ImDrawFlags.RoundCornersAll, thickness);
    }

    static void DrawCommandAction(IAction? ability, SpecialCommandType command, Vector4 color)
    {
        if (ability.GetTexture(out var texture)) DrawCommandAction(texture, command, color, ability?.ToString() ?? "");
    }

    static void DrawCommandAction(uint iconId, SpecialCommandType command, Vector4 color)
    {
        if (IconSet.GetTexture(iconId, out var texture)) DrawCommandAction(texture, command, color);
    }

    static void DrawCommandAction(IDalamudTextureWrap texture, SpecialCommandType command, Vector4 color, string helpAddition = "")
    {
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = abilityW + ImGui.GetStyle().ItemInnerSpacing.X * 2;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        var pos = ImGui.GetCursorPos();

        using var group = ImRaii.Group();
        if (!group) return;

        using (var subGroup = ImRaii.Group())
        {
            if (subGroup)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
                ImGui.TextColored(color, str);

                var help = command.Local();
                if (!string.IsNullOrEmpty(helpAddition))
                {
                    help += "\n" + helpAddition;
                }
                string baseId = "ImgButton" + command.ToString();

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
                if (texture != null) DrawIAction(texture.ImGuiHandle, baseId, abilityW, command, help);

            }
        }

        if (DataCenter.SpecialType == command)
        {
            var size = ImGui.GetItemRectSize();
            var winPos = ImGui.GetWindowPos();

            HighLight(winPos + pos, size);

            if (DataCenter.SpecialTimeLeft > 0)
            {
                var time = DataCenter.SpecialTimeLeft.ToString("F2") + "s";
                var strSize = ImGui.CalcTextSize(time);
                CooldownWindow.TextShade(winPos + pos + size - strSize, time);
            }
        }
    }

    static void DrawCommandAction(uint iconId, StateCommandType command, Vector4 color)
    {
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = abilityW + ImGui.GetStyle().ItemInnerSpacing.X * 2;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        var pos = ImGui.GetCursorPos();

        using (var group = ImRaii.Group())
        {
            if (group)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
                ImGui.TextColored(color, str);

                var help = command.Local();
                string baseId = "ImgButton" + command.ToString();

                if (IconSet.GetTexture(iconId, out var texture))
                {
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
                    DrawIAction(texture.ImGuiHandle, baseId, abilityW, command, help);
                }

            }
        }

        bool isMatch = false;
        switch (command)
        {
            case StateCommandType.Auto when DataCenter.State && !DataCenter.IsManual:
            case StateCommandType.Manual when DataCenter.State && DataCenter.IsManual:
            case StateCommandType.Cancel when !DataCenter.State:
                isMatch = true;
                break;
        }

        if (isMatch)
        {
            var size = ImGui.GetItemRectSize();
            var winPos = ImGui.GetWindowPos();

            HighLight(winPos + pos, size);
        }
    }

    static void DrawIAction(nint handle, string id, float width, SpecialCommandType command, string help)
    {
        var cursor = ImGui.GetCursorPos();
        if (ImGuiHelper.NoPaddingNoColorImageButton(handle, Vector2.One * width, id))
        {
            Svc.Commands.ProcessCommand(command.GetCommandStr());
        }
        ImGuiHelper.DrawActionOverlay(cursor, width, IconSet.GetTexture(0u, out var text) && text.ImGuiHandle == handle ? -1 : 1);
        ImguiTooltips.HoveredTooltip(help);
    }

    static void DrawIAction(nint handle, string id, float width, StateCommandType command, string help)
    {
        var cursor = ImGui.GetCursorPos();
        if (ImGuiHelper.NoPaddingNoColorImageButton(handle, Vector2.One * width, id))
        {
            Svc.Commands.ProcessCommand(command.GetCommandStr());
        }
        ImGuiHelper.DrawActionOverlay(cursor, width, 1);
        ImguiTooltips.HoveredTooltip(help);
    }

    internal static (Vector2, Vector2) DrawIAction(IAction? action, float width, float percent, bool isAdjust = true)
    {
        if (!action.GetTexture(out var texture, isAdjust)) return (default, default);

        var cursor = ImGui.GetCursorPos();

        var desc = action?.Name ?? string.Empty;
        if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * width, desc))
        {
            if (!DataCenter.State)
            {
                bool canDoIt = false;
                if (action is IBaseAction act)
                {
                    IBaseAction.ForceEnable = true;
                    canDoIt = act.CanUse(out _, usedUp: true, skipClippingCheck: true, skipAoeCheck: true);
                    IBaseAction.ForceEnable = false;
                }
                else if (action is IBaseItem item)
                {
                    canDoIt = item.CanUse(out _, true);
                }
                if (canDoIt) action?.Use();
            }
            else if(action != null)
            {
                DataCenter.AddCommandAction(action, 5);
            }
        }
        var size = ImGui.GetItemRectSize();
        ImGuiHelper.DrawActionOverlay(cursor, width, action == null ? -1 : percent);
        ImguiTooltips.HoveredTooltip(desc);

        return (cursor, size);
    }

    static unsafe void DrawNextAction(float gcd, float ability, float width)
    {
        using var group = ImRaii.Group();
        if (!group) return;

        var str = "Next Action";
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.TextColored(ImGuiColors.DalamudYellow, str);

        NextActionWindow.DrawGcdCooldown(width, true);

        var y = ImGui.GetCursorPosY();

        DrawIAction(ActionUpdater.NextGCDAction, gcd, 1);

        var next = ActionUpdater.NextGCDAction != ActionUpdater.NextAction ? ActionUpdater.NextAction : null;

        ImGui.SameLine();

        ImGui.SetCursorPosY(y);

        DrawIAction(next, ability, 1);
    }
}