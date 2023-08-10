using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragIntRangeSearchJob : DragIntRangeSearch
{
    private readonly JobConfigInt _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description => _configMin.ToDescription();

    public override LinkDescription[] Tooltips => _configMin.ToAction();

    public override string Command => _configMin.ToCommand();

    public DragIntRangeSearchJob(JobConfigInt configMin, JobConfigInt configMax, int min, int max, float speed)
        :base (min, max, speed)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    protected override void DrawMain(Job job)
    {
        base.DrawMain(job);
        DrawJobIcon();
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(job, _configMin, Service.ConfigNew.GetDefault(job, _configMin));
        Service.ConfigNew.SetValue(job, _configMax, Service.ConfigNew.GetDefault(job, _configMax));
    }

    protected override int GetMinValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _configMin);
    }

    protected override void SetMinValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(job, _configMin, value);
    }

    protected override int GetMaxValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _configMax);
    }

    protected override void SetMaxValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(job, _configMax, value);
    }
}


internal class DragIntRangeSearchPlugin : DragIntRangeSearch
{
    private readonly PluginConfigInt _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description => _configMin.ToDescription();

    public override LinkDescription[] Tooltips => _configMin.ToAction();

    public override string Command => _configMin.ToCommand();

    public DragIntRangeSearchPlugin(PluginConfigInt configMin, PluginConfigInt configMax, int min, int max, float speed)
        : base(min, max, speed)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(_configMin, Service.ConfigNew.GetDefault(_configMin));
        Service.ConfigNew.SetValue(_configMax, Service.ConfigNew.GetDefault(_configMax));
    }

    protected override int GetMinValue(Job job)
    {
        return Service.ConfigNew.GetValue(_configMin);
    }

    protected override void SetMinValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(_configMin, value);
    }

    protected override int GetMaxValue(Job job)
    {
        return Service.ConfigNew.GetValue(_configMax);
    }

    protected override void SetMaxValue(Job job, int value)
    {
        Service.ConfigNew.SetValue(_configMax, value);
    }
}

internal abstract class DragIntRangeSearch : Searchable
{
    public int Min { get; init; }
    public int Max { get; init; }
    public float Speed { get; init; }

    public DragIntRangeSearch(int min, int max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }

    protected abstract int GetMinValue(Job job);
    protected abstract void SetMinValue(Job job, int value);
    protected abstract int GetMaxValue(Job job);
    protected abstract void SetMaxValue(Job job, int value);
    protected override void DrawMain(Job job)
    {
        var minValue = GetMinValue(job);
        var maxValue = GetMaxValue(job);
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragIntRange2($"{Name}##Config_{ID}", ref minValue, ref maxValue, Speed, Min, Max))
        {
            SetMinValue(job, Math.Max(Math.Min(minValue, maxValue), Min));
            SetMaxValue(job, Math.Min(Math.Max(minValue, maxValue), Max));
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);
    }
}
