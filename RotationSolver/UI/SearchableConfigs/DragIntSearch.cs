using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragIntSearchJob : DragIntSearch
{
    private readonly JobConfigInt _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragIntSearchJob(JobConfigInt config, int min, int max, float speed)
        :base (min, max, speed)
    {
        _config = config;
    }

    protected override void DrawMain(Job job)
    {
        base.DrawMain(job);
        DrawJobIcon();
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(job, _config, Service.ConfigNew.GetDefault(job, _config));
    }

    protected override int GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _config);
    }

    protected override void SetValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(job, _config, value);
    }
}

internal class DragIntSearchPlugin : DragIntSearch
{
    private readonly PluginConfigInt _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragIntSearchPlugin(PluginConfigInt config, int min, int max, float speed)
        :base(min, max, speed)
    {
        _config = config;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(_config, Service.ConfigNew.GetDefault(_config));
    }

    protected override int GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(_config);
    }

    protected override void SetValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(_config, value);
    }
}

internal abstract class DragIntSearch : Searchable
{
    public int Min { get; }
    public int Max { get; }
    public float Speed { get; }
    public DragIntSearch(int min, int max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }
    protected abstract int GetValue(Job job);
    protected abstract void SetValue(Job job, int value);
    protected override void DrawMain(Job job)
    {
        var value = GetValue(job);
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if(ImGui.DragInt($"{Name}##Config_{ID}", ref value, Speed, Min, Max))
        {
            SetValue(job, Math.Min(Math.Max(value, Min), Max));
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);
    }
}
