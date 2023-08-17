using Dalamud.Game.ClientState.Keys;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableSettings;
using RotationSolver.Updaters;

namespace RotationSolver.UI.SearchableConfigs;

internal abstract class Searchable : ISearchable
{
    public const float DRAG_WIDTH = 150;
    protected static float Scale => ImGuiHelpers.GlobalScale;
    public CheckBoxSearch Parent { get; set; }

    public string SearchingKeys => Name + " " + Description;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Command { get; }
    public abstract LinkDescription[] Tooltips { get; }
    public abstract string ID { get; }
    private string Popup_Key => "Rotation Solver RightClicking: " + ID;
    protected virtual bool IsJob => false;

    /// <summary>
    /// Only these job roles can get this setting.
    /// </summary>
    public JobRole[] JobRoles { get; set; }
    public Job[] Jobs { get; set; }

    public void Draw(Job job, bool mustDraw = false)
    {
        if (!mustDraw)
        {
            var canDraw = true;
            if (JobRoles != null)
            {
                var role = RotationUpdater.RightNowRotation?.ClassJob?.GetJobRole();
                if (role.HasValue)
                {
                    canDraw = JobRoles.Contains(role.Value);
                }
            }

            if (Jobs != null)
            {
                canDraw |= Jobs.Contains(DataCenter.Job);
            }

            if (!canDraw) return;
        }

        DrawMain(job);

        ImGuiHelper.PrepareGroup(Popup_Key, Command, () => ResetToDefault(job));
    }

    protected abstract void DrawMain(Job job);

    public abstract void ResetToDefault(Job job);

    protected void ShowTooltip(Job job, bool showHand = true)
    {
        var showDesc = !string.IsNullOrEmpty(Description);
        if (showDesc || Tooltips != null)
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                if (showDesc)
                {
                    ImGui.TextWrapped(Description);
                }
                if (showDesc && Tooltips != null && Tooltips.Length > 0)
                {
                    ImGui.Separator();
                }
                var wholeWidth = ImGui.GetWindowWidth();

                if(Tooltips != null) foreach (var tooltip in Tooltips)
                {
                    RotationConfigWindow.DrawLinkDescription(tooltip, wholeWidth, false);
                }
            });
        }

        ImGuiHelper.ReactPopup(Popup_Key, Command, () => ResetToDefault(job), showHand);
    }




    protected static void DrawJobIcon()
    {
        ImGui.SameLine();

        if (IconSet.GetTexture(IconSet.GetJobIcon(DataCenter.Job, IconType.Framed), out var texture))
        {
            ImGui.Image(texture.ImGuiHandle, Vector2.One * 24 * ImGuiHelpers.GlobalScale);
            ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_Configs_JobConfigTip);
        }
    }
}
