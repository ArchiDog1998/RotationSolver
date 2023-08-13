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

    public DragFloatSearchJob(JobConfigFloat config, float min, float max, float speed)
          : base(min, max, speed)
    {
        _config = config;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(job, _config, Service.ConfigNew.GetDefault(job, _config));
    }

    protected override float GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _config);
    }

    protected override void SetValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(job, _config, value);
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

    public DragFloatSearchPlugin(PluginConfigFloat config, float min, float max, float speed)
        :base(min, max, speed)
    {
        _config = config;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(_config, Service.ConfigNew.GetDefault(_config));
    }

    protected override float GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(_config);
    }

    protected override void SetValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(_config, value);
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
            SetValue(job, Math.Min(Math.Max(value, Min), Max));
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);

        if (IsJob) DrawJobIcon();

        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(job, false);
    }
}
