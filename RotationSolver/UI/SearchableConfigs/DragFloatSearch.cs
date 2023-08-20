using Dalamud.Utility;
using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragFloatSearchJob : DragFloatSearch
{
    private readonly JobConfigFloat _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();
    protected override bool IsJob => true;

    public DragFloatSearchJob(JobConfigFloat config, float speed)
          : base((float)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed)
    {
        _config = config;
    }

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(job, _config, Service.Config.GetDefault(job, _config));
    }

    protected override float GetValue(Job job)
    {
        return Service.Config.GetValue(job, _config);
    }

    protected override void SetValue(Job job, float value)
    {
        Service.Config.SetValue(job, _config, value);
    }
}


internal class DragFloatSearchPlugin : DragFloatSearch
{
    private readonly PluginConfigFloat _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragFloatSearchPlugin(PluginConfigFloat config, float speed, uint color = 0)
        :base((float)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed)
    {
        _config = config;
        Color = color;
    }

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }

    protected override float GetValue(Job job)
    {
        return Service.Config.GetValue(_config);
    }

    protected override void SetValue(Job job, float value)
    {
        Service.Config.SetValue(_config, value);
    }
}


internal abstract class DragFloatSearch : Searchable
{
    public float Min { get; init; }
    public float Max { get; init; }
    public float Speed { get; init; }

    public DragFloatSearch(float min, float max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }
    protected abstract float GetValue(Job job);
    protected abstract void SetValue(Job job, float value);
    protected override void DrawMain(Job job)
    {
        var value = GetValue(job);
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragFloat($"##Config_{ID}", ref value, Speed, Min, Max))
        {
            SetValue(job, value);
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);

        if (IsJob) DrawJobIcon();

        ImGui.SameLine();

        if (Color != 0) ImGui.PushStyleColor(ImGuiCol.Text, Color);
        ImGui.TextWrapped(Name);
        if (Color != 0) ImGui.PopStyleColor();

        if (ImGui.IsItemHovered()) ShowTooltip(job, false);
    }
}
