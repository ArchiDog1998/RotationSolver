using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ImGuiScene;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

internal class ControlWindow : Window
{
    public const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoScrollbar
                            | ImGuiWindowFlags.NoCollapse
                            | ImGuiWindowFlags.NoTitleBar
                            | ImGuiWindowFlags.NoNav
                            | ImGuiWindowFlags.NoScrollWithMouse;

    public ControlWindow()
        : base(nameof(ControlWindow), BaseFlags)
    {
        Size = new Vector2(540f, 300f);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        Vector4 bgColor = Service.Config.IsControlWindowLock
            ? Service.Config.ControlWindowLockBg
            : Service.Config.ControlWindowUnlockBg;
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

        Flags = BaseFlags;

        if (Service.Config.IsControlWindowLock)
        {
            Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        base.PostDraw();
    }

    public override void Draw()
    {
        ImGui.Columns(3, "Control Bolder", false);
        var gcd = Service.Config.ControlWindowGCDSize * Service.Config.ControlWindowNextSizeRatio;
        var ability = Service.Config.ControlWindow0GCDSize * Service.Config.ControlWindowNextSizeRatio;
        var width = gcd + ability + ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetColumnWidth(0, width + ImGui.GetStyle().ColumnsMinSpacing * 2);
        ImGui.SetColumnWidth(1, 8);

        DrawNextAction(gcd, ability, width);

        ImGui.Spacing();

        DrawCommandAction(61751, StateCommandType.Manual, ImGuiColors.DPSRed);

        ImGui.SameLine();
        DrawCommandAction(61764, StateCommandType.Cancel, ImGuiColors.DalamudWhite2);

        DrawCommandAction(61822, StateCommandType.Smart, ImGuiColors.DPSRed);

        ImGui.SameLine();

        ImGui.BeginGroup();

        ImGui.Text(DataCenter.TargetingType.ToName());

        RotationConfigWindow.DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsInfoWindowNoMove,
            ref Service.Config.IsControlWindowLock, Service.Default.IsControlWindowLock);
        ImGui.EndGroup();

        ImGui.NextColumn();
        ImGui.NextColumn();

        DrawSpecials();

        ImGui.Columns(1);
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

        ImGui.Spacing();

        DrawCommandAction(rotation?.ActionMoveForwardGCD, rotation?.ActionMoveForwardAbility,
            SpecialCommandType.MoveForward, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(rotation?.ActionMoveBackAbility,
            SpecialCommandType.MoveBack, ImGuiColors.DalamudOrange);

        ImGui.SameLine();

        DrawCommandAction(61804, SpecialCommandType.Burst, ImGuiColors.DalamudWhite2);

        ImGui.SameLine();

        DrawCommandAction(61753, SpecialCommandType.EndSpecial, ImGuiColors.DalamudWhite2);

        ImGui.Spacing();

        DrawCommandAction(rotation?.EsunaStanceNorthGCD, rotation?.EsunaStanceNorthAbility,
            SpecialCommandType.EsunaStanceNorth, ImGuiColors.ParsedGold);

        ImGui.SameLine();

        DrawCommandAction(rotation?.RaiseShirkGCD, rotation?.RaiseShirkAbility,
            SpecialCommandType.RaiseShirk, ImGuiColors.ParsedBlue);

        ImGui.SameLine();

        DrawCommandAction(rotation?.AntiKnockbackAbility,
            SpecialCommandType.AntiKnockback, ImGuiColors.DalamudWhite2);

        ImGui.Spacing();

        ImGui.Text("CMD:");
        ImGui.SameLine();
        DrawIAction(DataCenter.CommandNextAction, Service.Config.ControlWindow0GCDSize, 1);

        ImGui.SameLine();

        ImGui.BeginGroup();
        ImGui.Text(DataCenter.RightNowTargetToHostileType switch
        {
             TargetHostileType.AllTargetsCanAttack => LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType1,
             TargetHostileType.TargetsHaveTargetOrAllTargetsCanAttack => LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType2,
             TargetHostileType.TargetsHaveTarget => LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType3,
             _ => string.Empty,
        });

        ImGui.Text("Auto: " + DataCenter.AutoStatus.ToString());
        ImGui.EndGroup();
    }

    static void DrawCommandAction(IAction gcd, IAction ability, SpecialCommandType command, Vector4 color)
    {
        var gcdW = Service.Config.ControlWindowGCDSize;
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = gcdW + abilityW + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().ItemInnerSpacing.X * 4;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        var pos = ImGui.GetCursorPos();
        ImGui.BeginGroup();
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

        if (DataCenter.SpecialType == command)
        {
            var size = ImGui.GetItemRectSize();
            var winPos = ImGui.GetWindowPos();

            HighLight(winPos + pos, size);

            if(DataCenter.SpecialTimeLeft > 0)
            {
                var time = DataCenter.SpecialTimeLeft.ToString("F2") + "s";
                var strSize = ImGui.CalcTextSize(time);
                CooldownWindow.TextShade(winPos + pos + size - strSize, time);
            }
        }
        ImGui.EndGroup();
    }

    static void HighLight(Vector2 pt, Vector2 size, float thickness = 2f)
    {
        var offset = ImGui.GetStyle().ItemSpacing / 2;
        ImGui.GetWindowDrawList().AddRect(pt - offset, pt + size + offset,
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudGrey), 5, ImDrawFlags.RoundCornersAll, thickness);
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

        var pos = ImGui.GetCursorPos();
        ImGui.BeginGroup();
        ImGui.BeginGroup();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
        ImGui.TextColored(color, str);

        var help = GetHelp(command);
        string baseId = "ImgButton" + command.ToString();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
        DrawIAction(texture.ImGuiHandle, baseId, abilityW, command, help);
        ImGui.EndGroup();

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
        ImGui.EndGroup();
    }

    static void DrawCommandAction(uint iconId, StateCommandType command, Vector4 color)
    {
        var abilityW = Service.Config.ControlWindow0GCDSize;
        var width = abilityW + ImGui.GetStyle().ItemInnerSpacing.X * 2;
        var str = command.ToString();
        var strWidth = ImGui.CalcTextSize(str).X;

        var pos = ImGui.GetCursorPos();
        ImGui.BeginGroup();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, width / 2 - strWidth / 2));
        ImGui.TextColored(color, str);

        var help = GetHelp(command);
        string baseId = "ImgButton" + command.ToString();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Math.Max(0, strWidth / 2 - width / 2));
        DrawIAction(IconSet.GetTexture(iconId).ImGuiHandle, baseId, abilityW, command, help);
        ImGui.EndGroup();

        if (DataCenter.StateType == command)
        {
            var size = ImGui.GetItemRectSize();
            var winPos = ImGui.GetWindowPos();

            HighLight(winPos + pos, size);
        }
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

    static TextureWrap GetTexture(IAction action, bool isAdjust = true)
    {
        uint iconId = 0;
        if(action != null && !_actionIcons.TryGetValue(isAdjust ? action.AdjustedID : action.ID, out iconId))
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
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && InputUpdater.RecordingSpecialType == SpecialCommandType.None)
            {
                InputUpdater.RecordingTime = DateTime.Now;
                InputUpdater.RecordingSpecialType = command;
                Service.ToastGui.ShowQuest($"Recording: {command}",
                    new Dalamud.Game.Gui.Toast.QuestToastOptions()
                    {
                        IconId = 101,
                    });
            }

            if (ImGui.IsKeyPressed(ImGuiKey.LeftCtrl) && ImGui.IsMouseDown(ImGuiMouseButton.Middle))
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
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right)&& InputUpdater.RecordingStateType == StateCommandType.None)
            {
                InputUpdater.RecordingTime = DateTime.Now;
                InputUpdater.RecordingStateType = command;
                Service.ToastGui.ShowQuest($"Recording: {command}",
                    new Dalamud.Game.Gui.Toast.QuestToastOptions()
                    {
                        IconId = 101,
                    });
            }

            if (ImGui.IsKeyPressed(ImGuiKey.LeftCtrl) && ImGui.IsMouseDown(ImGuiMouseButton.Middle))
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
        }
    }

    internal static (Vector2, Vector2) DrawIAction(IAction action, float width, float percent, bool isAdjust = true)
    {
        var result = DrawIAction(GetTexture(action, isAdjust).ImGuiHandle, width, action == null ? -1 : percent);
        if (action != null) ImGuiHelper.HoveredString(action.Name, () =>
        {
            if (DataCenter.StateType == StateCommandType.Cancel)
            {
                bool canDoIt = false;
                if(action is IBaseAction act)
                {
                    canDoIt = act.CanUse(out _, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo | CanUseOption.SkipDisable | CanUseOption.IgnoreClippingCheck);
                }
                else if (action is IBaseItem item)
                {
                    canDoIt = item.CanUse(out _);
                }
                if(canDoIt) action.Use();
            }
            else
            {
                DataCenter.AddCommandAction(action, 5);
            }
        });
        return result;
    }

    static (Vector2, Vector2) DrawIAction(nint handle, float width, float percent)
    {
        var cursor = ImGui.GetCursorPos();
        ImGui.BeginGroup();

        var pt = ImGui.GetCursorPos();
        ImGui.Image(handle, new Vector2(width, width));
        var size = ImGui.GetItemRectSize();
        var pixPerUnit = width / 82;

        if (percent < 0)
        {
            var cover = IconSet.GetTexture("ui/uld/icona_frame_hr1.tex");

            if (cover != null)
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 4));

                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2((96f * 0 + 4f) / cover.Width, (96f * 2) / cover.Height);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }
        else if (percent < 1)
        {
            var cover = IconSet.GetTexture("ui/uld/icona_recast_hr1.tex");

            if (cover != null)
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 0));

                var P = (int)(percent * 81);


                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2(P % 9 * step.X, P / 9 * step.Y);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }
        else
        {
            var cover = IconSet.GetTexture("ui/uld/icona_frame_hr1.tex");

            if (cover != null)
            {

                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 4));

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    new Vector2(4f / cover.Width, 0f / cover.Height),
                    new Vector2(92f / cover.Width, 96f / cover.Height));
            }
        }

        if (percent > 1)
        {
            var cover = IconSet.GetTexture("ui/uld/icona_recast2_hr1.tex");

            if (cover != null)
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 0));

                var P = (int)(percent % 1 * 81);

                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2((P % 9 + 9) * step.X, P / 9 * step.Y);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }


        ImGui.EndGroup();
        return (pt, size);
    }

    static unsafe void DrawNextAction(float gcd, float ability, float width)
    {
        var str = "Next Action";
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + width / 2 - ImGui.CalcTextSize(str).X / 2);
        ImGui.TextColored(ImGuiColors.DalamudYellow, str);

        NextActionWindow.DrawGcdCooldown(width, true);

        DrawIAction(ActionUpdater.NextGCDAction, gcd, 1);

        var next = ActionUpdater.NextGCDAction != ActionUpdater.NextAction ? ActionUpdater.NextAction : null;

        ImGui.SameLine();
        DrawIAction(next, ability, -1);
    }
}
