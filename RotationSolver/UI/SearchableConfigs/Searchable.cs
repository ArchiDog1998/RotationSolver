using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;
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

    public uint Color { get; set; } = 0;

    /// <summary>
    /// Only these job roles can get this setting.
    /// </summary>
    public JobRole[] JobRoles { get; set; }
    /// <summary>
    /// Or these jobs.
    /// </summary>
    public Job[] Jobs { get; set; }

    public unsafe void Draw(Job job)
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

        if (!canDraw)
        {
            var textColor = *ImGui.GetStyleColorVec4(ImGuiCol.Text);

            ImGui.PushStyleColor(ImGuiCol.Text, *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled));

            var cursor = ImGui.GetCursorPos() + ImGui.GetWindowPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
            ImGui.TextWrapped(Name);
            ImGui.PopStyleColor();

            var step = ImGui.CalcTextSize(Name);
            var size = ImGui.GetItemRectSize();
            var height = step.Y / 2;
            var wholeWidth = step.X;
            while (height < size.Y)
            {
                var pt = cursor + new Vector2(0, height);
                ImGui.GetWindowDrawList().AddLine(pt, pt + new Vector2(Math.Min(wholeWidth, size.X), 0), ImGui.ColorConvertFloat4ToU32(textColor));
                height += step.Y;
                wholeWidth -= size.X;
            }

            var jobs = JobRoles.SelectMany(JobRoleExtension.ToJobs).Union(Jobs ?? Array.Empty<Job>());
            var roleOrJob = string.Join("\n",
                jobs.Select(job => Svc.Data.GetExcelSheet<ClassJob>()?.GetRow((uint)job)?.Name ?? job.ToString()));

            ImguiTooltips.HoveredTooltip(string.Format(LocalizationManager.RightLang.ConfigWindow_NotInJob, roleOrJob));
            return;
        }

        DrawMain(job);

        ImGuiHelper.PrepareGroup(Popup_Key, Command, () => ResetToDefault(job));
    }

    protected abstract void DrawMain(Job job);

    public abstract void ResetToDefault(Job job);

    protected void ShowTooltip(Job job, bool showHand = true)
    {
        var showDesc = !string.IsNullOrEmpty(Description);
        if (showDesc || Tooltips != null && Tooltips.Length > 0)
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
