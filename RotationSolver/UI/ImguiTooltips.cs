using Dalamud.Interface.Colors;

namespace RotationSolver.UI;

internal static class ImguiTooltips
{
    const ImGuiWindowFlags TOOLTIP_FLAG =
          ImGuiWindowFlags.Tooltip |
          ImGuiWindowFlags.NoMove |
          ImGuiWindowFlags.NoSavedSettings |
          ImGuiWindowFlags.NoBringToFrontOnFocus |
          ImGuiWindowFlags.NoDecoration |
          ImGuiWindowFlags.NoInputs |
          ImGuiWindowFlags.AlwaysAutoResize;

    const string TOOLTIP_ID = "RotationSolver Tooltips";

    public static void ShowTooltip(string text)
    {
        if(string.IsNullOrEmpty(text)) return;
        ShowTooltip(() => ImGui.Text(text));
    }

    public static void ShowTooltip(Action act)
    {
        if (act == null) return;
        if (!Service.Config.ShowTooltips) return;

        ImGui.SetNextWindowBgAlpha(1);
        ImGui.PushStyleColor(ImGuiCol.BorderShadow, ImGuiColors.DalamudWhite);

        //ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(800, 1500));
        ImGui.SetWindowPos(TOOLTIP_ID, ImGui.GetIO().MousePos);

        if (ImGui.Begin(TOOLTIP_ID, TOOLTIP_FLAG))
        {
            act();
            ImGui.End();
        }

        ImGui.PopStyleColor();
    }
}
