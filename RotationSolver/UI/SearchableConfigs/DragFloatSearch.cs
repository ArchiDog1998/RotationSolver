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

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragFloatSearchJob(JobConfigFloat config)
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

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragFloatSearchPlugin(PluginConfigFloat config)
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
    public float Min { get; }
    public float Max { get; }
    public float Speed { get; }
    protected abstract float GetValue(Job job);
    protected abstract void SetValue(Job job, float value);
    public override void Draw(Job job)
    {
        var value = GetValue(job);
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragFloat($"{Name}##Config_{ID}", ref value, Speed, Min, Max))
        {
            SetValue(job, Math.Min(Math.Max(value, Min), Max));
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);
    }
}
