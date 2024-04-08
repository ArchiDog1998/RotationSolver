using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using ECommons.ExcelServices;
using RotationSolver.Data;
using System.IO;
using XIVConfigUI;

namespace RotationSolver.UI.SearchableConfigs;
internal class SearchableConfigRS : SearchableConfig
{
    public override bool ShowTooltip => Service.Config.ShowTooltips;

    public override bool IsPropertyValid(PropertyInfo property)
    {
        var filter = GetFilter(property);
        return filter.CanDraw;
    }

    public override void PreNameDrawing(PropertyInfo property)
    {
        if (property.GetCustomAttribute<JobConfigAttribute>() != null
        || property.GetCustomAttribute<JobChoiceConfigAttribute>() != null)
        {
            DrawJobIcon(DataCenter.Job);
        }
    }

    public override void PropertyInvalidTooltip(PropertyInfo property)
    {
        ImGui.Text(UiString.NotInJob.Local());

        ImGui.NewLine();

        var filter = GetFilter(property);
        foreach (var job in filter.AllJobs)
        {
            DrawJobIcon(job);
        }
    }

    private static JobFilter GetFilter(PropertyInfo info)
    {
        var attr = info.GetCustomAttribute<JobFilterAttribute>();
        if (attr == null) return new();

        return new(DataCenter.IsPvP ? attr.PvP : attr.PvE);
    }

    private static void DrawJobIcon(Job job)
    {
        ImGui.SameLine();

        if (IconSet.GetTexture(IconSet.GetJobIcon(job, IconType.Framed), out var texture))
        {
            ImGui.Image(texture.ImGuiHandle, Vector2.One * 24 * ImGuiHelpers.GlobalScale);
            ImguiTooltips.HoveredTooltip(UiString.JobConfigTip.Local());
        }
    }

    public override bool GetTexture(string path, out IDalamudTextureWrap texture, bool loadingIcon = false)
    => IconSet.GetTexture(path, out texture, loadingIcon);

    public override bool GetTexture(uint id, out IDalamudTextureWrap texture, uint @default = 0)
    => IconSet.GetTexture(id, out texture, @default);

}
