using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Condition;
using System.ComponentModel;
using XIVConfigUI;
using XIVDrawer;

namespace RotationSolver.UI.ConfigWindows;
[Description("Basic")]
public class BasicItem : ConfigWindowItemRS
{
    public override uint Icon => 14;
    public override string Description => UiString.Item_Basic.Local();
    private CollapsingHeaderGroup? _baseHeader;
    public override void Draw(ConfigWindow window)
    {
        _baseHeader ??= new(new()
            {
                { () => UiString.ConfigWindow_Basic_Timer.Local(), () => DrawBasicTimer(window) },
                { () => UiString.ConfigWindow_Basic_AutoSwitch.Local(), () => DrawBasicAutoSwitch(window) },
                { () => UiString.ConfigWindow_Basic_NamedConditions.Local(), DrawBasicNamedConditions },
                { () => UiString.ConfigWindow_Basic_Others.Local(), () => DrawBasicOthers(window) },
            });
        _baseHeader?.Draw();
    }

    private static readonly uint PING_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGreen);
    private static readonly uint LOCK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedBlue);
    private static readonly uint WEAPON_DELAY_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGold);
    private static readonly uint IDEAL_CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0f, 0f, 1f));
    private static readonly uint CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedPink);
    private static readonly uint ADVANCE_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow);
    private static readonly uint ADVANCE_ABILITY_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedOrange);
    const float gcdSize = 50, ogcdSize = 40, pingHeight = 12, spacingHeight = 8;

    private static void AddPingLockTime(ImDrawListPtr drawList, Vector2 lineStart, float sizePerTime, float ping, float animationLockTime, float advanceTime, uint color, float clickTime)
    {
        var size = new Vector2(ping * sizePerTime, pingHeight);
        drawList.AddRectFilled(lineStart, lineStart + size, ImGuiHelperRS.ChangeAlpha(PING_COLOR));
        if (ImGuiHelperRS.IsInRect(lineStart, size))
        {
            ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Basic_Ping.Local());
        }

        var rectStart = lineStart + new Vector2(ping * sizePerTime, 0);
        size = new Vector2(animationLockTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ImGuiHelperRS.ChangeAlpha(LOCK_TIME_COLOR));
        if (ImGuiHelperRS.IsInRect(rectStart, size))
        {
            ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Basic_AnimationLockTime.Local());
        }

        drawList.AddLine(lineStart - new Vector2(0, spacingHeight), lineStart + new Vector2(0, (pingHeight * 2) + (spacingHeight / 2)), IDEAL_CLICK_TIME_COLOR, 1.5f);

        rectStart = lineStart + new Vector2(-advanceTime * sizePerTime, pingHeight);
        size = new Vector2(advanceTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ImGuiHelperRS.ChangeAlpha(color));
        if (ImGuiHelperRS.IsInRect(rectStart, size))
        {
            ImGuiHelper.ShowTooltip(() =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Basic_ClickingDuration.Local());

                ImGui.Separator();

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(IDEAL_CLICK_TIME_COLOR),
                    UiString.ConfigWindow_Basic_IdealClickingTime.Local());

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(CLICK_TIME_COLOR),
                    UiString.ConfigWindow_Basic_RealClickingTime.Local());
            });
        }

        float time = 0;
        while (time < advanceTime)
        {
            var start = lineStart + new Vector2((time - advanceTime) * sizePerTime, 0);
            drawList.AddLine(start + new Vector2(0, pingHeight), start + new Vector2(0, (pingHeight * 2) + spacingHeight), CLICK_TIME_COLOR, 2.5f);

            time += clickTime;
        }
    }
    private static void DrawBasicTimer(ConfigWindow window)
    {
        var gcdTime = DataCenter.WeaponTotal;
        if (gcdTime == 0) gcdTime = 2.5f;
        var wholeWidth = ImGui.GetWindowWidth();
        var ping = DataCenter.Ping;

        ImGui.PushFont(DrawingExtensions.GetFont(14));
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
        var infoText = $"GCD: {gcdTime:F2}s Ping: {ping:F2}s";
        var infoSize = ImGui.CalcTextSize(infoText);

        ImGuiHelper.DrawItemMiddle(() =>
        {
            ImGui.Text(infoText);
        }, wholeWidth, infoSize.X);
        ImGui.PopStyleColor();
        ImGui.PopFont();

        var actionAhead = Service.Config.ActionAhead;
        var minAbilityAhead = Service.Config.MinLastAbilityAdvanced;
        var animationLockTime = DataCenter.MinAnimationLock;
        var weaponDelay = (Service.Config.WeaponDelay.X + Service.Config.WeaponDelay.Y) / 2;
        var clickingDelay = (Service.Config.ClickingDelay.X + Service.Config.ClickingDelay.Y) / 2;

        var drawList = ImGui.GetWindowDrawList();
        ImGui.Spacing();
        var startCursorPt = ImGui.GetCursorPos();
        var windowsPos = ImGui.GetWindowPos();

        var sizePerTime = (wholeWidth - gcdSize) / (gcdTime + weaponDelay + actionAhead);

        var lineStart = windowsPos + startCursorPt + new Vector2(sizePerTime * actionAhead, gcdSize + spacingHeight);
        ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * actionAhead, 0), gcdSize, 0);
        ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(wholeWidth - gcdSize, 0), gcdSize, 0);

        AddPingLockTime(drawList, lineStart, sizePerTime, ping, animationLockTime, actionAhead, ADVANCE_TIME_COLOR, clickingDelay);
        var start = lineStart + new Vector2(gcdTime * sizePerTime, 0);
        var rectSize = new Vector2(weaponDelay * sizePerTime, pingHeight);
        drawList.AddRectFilled(start, start + rectSize, WEAPON_DELAY_COLOR);
        drawList.AddRect(start, start + rectSize, uint.MaxValue, 0, ImDrawFlags.Closed, 2);
        if (ImGuiHelperRS.IsInRect(start, rectSize))
        {
            ImGuiHelper.ShowTooltip(typeof(Configs).GetProperty(nameof(Configs.WeaponDelay))!.Local());
        }
        drawList.AddLine(lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime, -spacingHeight), lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime,
            (pingHeight * 2) + spacingHeight), IDEAL_CLICK_TIME_COLOR, 2);

        ImGui.PushFont(DrawingExtensions.GetFont(20));
        const string gcdText = "GCD";
        var size = ImGui.CalcTextSize(gcdText);
        ImGui.SetCursorPos(startCursorPt + new Vector2((sizePerTime * actionAhead) + ((gcdSize - size.X) / 2), (gcdSize - size.Y) / 2));
        ImGui.Text(gcdText);
        ImGui.SetCursorPos(startCursorPt + new Vector2(wholeWidth - gcdSize + ((gcdSize - size.X) / 2), (gcdSize - size.Y) / 2));
        ImGui.Text(gcdText);
        ImGui.PopFont();

        ImGui.PushFont(DrawingExtensions.GetFont(14));
        const string ogcdText = "Off-\nGCD";
        size = ImGui.CalcTextSize(ogcdText);
        ImGui.PopFont();

        var timeStep = ping + animationLockTime;
        var time = timeStep;
        while (time < gcdTime - timeStep)
        {
            var isLast = time + (2 * timeStep) > gcdTime;
            if (isLast)
            {
                time = gcdTime - timeStep;
            }

            ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * (actionAhead + time), 0), ogcdSize, 0);
            ImGui.SetCursorPos(startCursorPt + new Vector2((sizePerTime * (actionAhead + time)) + ((ogcdSize - size.X) / 2), (ogcdSize - size.Y) / 2));

            ImGui.PushFont(DrawingExtensions.GetFont(14));
            ImGui.Text(ogcdText);
            ImGui.PopFont();

            var ogcdStart = lineStart + new Vector2(time * sizePerTime, 0);
            AddPingLockTime(drawList, ogcdStart, sizePerTime, ping, animationLockTime,
                isLast ? MathF.Max(minAbilityAhead, actionAhead) : actionAhead, isLast ? ADVANCE_ABILITY_TIME_COLOR : ADVANCE_TIME_COLOR, clickingDelay);

            time += timeStep;
        }

        ImGui.SetCursorPosY(startCursorPt.Y + gcdSize + (pingHeight * 2) + (2 * spacingHeight) + ImGui.GetStyle().ItemSpacing.Y);

        ImGui.Spacing();

        window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_Timer);
    }

    private static readonly CollapsingHeaderGroup _autoSwitch = new(new()
        {
            {
                () => UiString.ConfigWindow_Basic_SwitchCancelConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.SwitchCancelConditionSet)
            },
            {
                () => UiString.ConfigWindow_Basic_SwitchManualConditionSet.Local(),
                () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.SwitchManualConditionSet)
            },
            {
               () =>  UiString.ConfigWindow_Basic_SwitchAutoConditionSet.Local(),
               () => XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.SwitchAutoConditionSet)
            },
        })
    {
        HeaderSize = FontSize.Fourth,
    };
    private static void DrawBasicAutoSwitch(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_AutoSwitch);
        _autoSwitch?.Draw();
    }

    private static readonly Dictionary<int, bool> _isOpen = [];
    private static void DrawBasicNamedConditions()
    {
        if (!DataCenter.RightSet.NamedConditions.Any(c => string.IsNullOrEmpty(c.Name)))
        {
            DataCenter.RightSet.NamedConditions = [.. DataCenter.RightSet.NamedConditions, (string.Empty, new ConditionSet())];
        }

        ImGui.Spacing();

        int removeIndex = -1;
        for (int i = 0; i < DataCenter.RightSet.NamedConditions.Length; i++)
        {
            var value = _isOpen.TryGetValue(i, out var open) && open;

            var toggle = value ? FontAwesomeIcon.ArrowUp : FontAwesomeIcon.ArrowDown;
            var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X
                - ImGuiEx.CalcIconSize(toggle).X - (ImGui.GetStyle().ItemSpacing.X * 2) - (20 * Scale);

            ImGui.SetNextItemWidth(width);
            ImGui.InputTextWithHint($"##Rotation Solver Named Condition{i}", UiString.ConfigWindow_Condition_ConditionName.Local(),
                ref DataCenter.RightSet.NamedConditions[i].Name, 1024);

            ImGui.SameLine();

            if (ImGuiEx.IconButton(toggle, $"##Rotation Solver Toggle Named Condition{i}"))
            {
                _isOpen[i] = value = !value;
            }

            ImGui.SameLine();

            if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Named Condition{i}"))
            {
                removeIndex = i;
            }

            if (value && DataCenter.RightNowRotation != null)
            {
                XIVConfigUI.ConditionConfigs.ConditionDrawer.Draw(DataCenter.RightSet.NamedConditions[i].Condition);
            }
        }
        if (removeIndex > -1)
        {
            var list = DataCenter.RightSet.NamedConditions.ToList();
            list.RemoveAt(removeIndex);
            DataCenter.RightSet.NamedConditions = [.. list];
        }
    }

    private static void DrawBasicOthers(ConfigWindow window)
    {
        window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_Others);

        if (DownloadHelper.IsSupporter) return;

        using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);

        if (Service.Config.UseAdditionalConditions)
        {
            ImGui.TextWrapped(UiString.CantUseConditionBoolean.Local());
        }

        if (!Service.Config.IWannaBeSaidHello)
        {
            var uiName = Service.Config.GetType().GetRuntimeProperty(nameof(Configs.IWannaBeSaidHello))?.LocalUIName() ?? string.Empty;

            ImGui.TextWrapped(string.Format(UiString.IWannaBeSaidHelloWarning.Local(), uiName));
        }
    }
}
