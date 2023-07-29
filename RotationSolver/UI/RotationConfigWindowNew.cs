using Dalamud.Interface.Windowing;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

public class RotationConfigWindowNew : Window
{
    private static float _scale => ImGuiHelpers.GlobalScale;
    public RotationConfigWindowNew()
    : base(nameof(RotationConfigWindowNew), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    public override void Draw()
    {
        ImGui.Columns(2);
        DrawSideBar();
        ImGui.NextColumn();
        DrawBody();
        ImGui.Columns(1);
    }

    private void DrawSideBar()
    {
        if (ImGui.BeginChild("Rotation Solver Side bar", Vector2.Zero))
        {
            var wholeWidth = ImGui.GetWindowSize().X;

            DrawHeader(wholeWidth);

            ImGui.Separator();

            ImGui.EndChild();
        }
    }

    private void DrawHeader(float wholeWidth)
    {
        var rotations = RotationUpdater.CustomRotations.FirstOrDefault(i => i.ClassJobIds.Contains((Job)Player.Object.ClassJob.Id))?.Rotations ?? Array.Empty<ICustomRotation>();
        var rotation = RotationUpdater.RightNowRotation;

        var size = MathF.Max(_scale * MathF.Min(wholeWidth, _scale * 150), 100);

        var logo = IconSet.GetTexture("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/docs/RotationSolverIcon_128.png");
        logo = IconSet.GetTexture("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/docs/assets/HowAndWhenToClick.svg");


        if (logo != null)
        {
            DrawItemMiddle(() =>
            {
                if (SilenceImageButton(logo.ImGuiHandle, Vector2.One * size))
                {

                }
            }, wholeWidth, size);

            ImGui.Spacing();
        }

        var iconSize = _scale * 50;
        var comboSize = ImGui.CalcTextSize(rotation.RotationName).X + _scale * 30;
        size = comboSize + iconSize + ImGui.GetStyle().ItemSpacing.X;

        DrawItemMiddle(() =>
        {
            var rotationIcon = rotation.GetTexture();
            if (rotationIcon != null && SilenceImageButton(rotationIcon.ImGuiHandle, Vector2.One * iconSize))
            {

            }

            ImGui.SameLine();

            ImGui.BeginGroup();
            ImGui.SetNextItemWidth(comboSize);
            ImGui.PushStyleColor(ImGuiCol.Text, rotation.GetColor());
            var isStartCombo = ImGui.BeginCombo("##RotationName:" + rotation.Name, rotation.RotationName);
            ImGui.PopStyleColor();

            if (isStartCombo)
            {
                foreach (var r in rotations)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, r.GetColor());
                    if (ImGui.Selectable(r.RotationName))
                    {
                        Service.Config.RotationChoices[rotation.ClassJob.RowId] = r.GetType().FullName;
                        Service.Config.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        //showToolTip?.Invoke(r);
                    }
                    ImGui.PopStyleColor();
                }
                ImGui.EndCombo();
            }

            var warning = !rotation.IsValid ? string.Format(LocalizationManager.RightLang.ConfigWindow_Rotation_InvalidRotation,
                    rotation.GetType().Assembly.GetInfo().Author)
            : rotation.IsBeta() ? LocalizationManager.RightLang.ConfigWindow_Rotation_BetaRotation : string.Empty;

            warning = string.IsNullOrEmpty(warning) ? LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation
                : warning + "\n \n" + LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation;
            ImguiTooltips.HoveredTooltip(warning);

            ImGui.TextDisabled("  -  " + LocalizationManager.RightLang.ConfigWindow_Helper_GameVersion + ": ");
            ImGui.SameLine();
            ImGui.Text(rotation.GameVersion);
            ImGui.EndGroup();

        }, wholeWidth, size, true);

    }

    private static bool SilenceImageButton(IntPtr handle, Vector2 size)
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
        ImGui.PushStyleColor(ImGuiCol.Button, 0);

        var padding = ImGui.GetStyle().FramePadding;
        ImGui.GetStyle().FramePadding = Vector2.Zero;

        var result = ImGui.ImageButton(handle, size);

        ImGui.GetStyle().FramePadding = padding;
        ImGui.PopStyleColor(2);

        return result;
    }

    private static void DrawItemMiddle(Action drawAction, float wholeWidth, float width, bool leftAlign = false)
    {
        if (drawAction == null) return;
        var distance = (wholeWidth - width) / 2;
        if (leftAlign) distance = MathF.Max(distance, 0);
        ImGui.SetCursorPosX(distance);
        drawAction();
    }

    private void DrawBody()
    {
        if (ImGui.BeginChild("Rotation Solver Body"))
        {
            ImGui.EndChild();
        }
    }
}
