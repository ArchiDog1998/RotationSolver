using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;
internal class DragFloatRangeSearchJob : DragFloatRangeSearch
{
    private readonly JobConfigFloat _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description => _configMin.ToDescription();

    public override LinkDescription[] Tooltips => _configMin.ToAction();

    protected override bool IsJob => true;

    public DragFloatRangeSearchJob(JobConfigFloat configMin, JobConfigFloat configMax, float min, float max, float speed)
        : base (min, max, speed)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(job, _configMin, Service.ConfigNew.GetDefault(job, _configMin));
        Service.ConfigNew.SetValue(job, _configMax, Service.ConfigNew.GetDefault(job, _configMax));
    }

    protected override float GetMinValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _configMin);
    }

    protected override void SetMinValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(job, _configMin, value);
    }

    protected override float GetMaxValue(Job job)
    {
        return Service.ConfigNew.GetValue(job, _configMax);
    }

    protected override void SetMaxValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(job, _configMax, value);
    }
}

internal class DragFloatRangeSearchPlugin : DragFloatRangeSearch
{
    private readonly PluginConfigFloat _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description => _configMin.ToDescription();

    public override LinkDescription[] Tooltips => _configMin.ToAction();


    public DragFloatRangeSearchPlugin(PluginConfigFloat configMin, PluginConfigFloat configMax, float min, float max, float speed)
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

    protected override float GetMinValue(Job job)
    {
        return Service.ConfigNew.GetValue(_configMin);
    }

    protected override void SetMinValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(_configMin, value);
    }

    protected override float GetMaxValue(Job job)
    {
        return Service.ConfigNew.GetValue(_configMax);
    }

    protected override void SetMaxValue(Job job, float value)
    {
        Service.ConfigNew.SetValue(_configMax, value);
    }
}

internal abstract class DragFloatRangeSearch : Searchable
{
    public float Min { get; init; }
    public float Max { get; init; }
    public float Speed { get; init; }

    public sealed override string Command => "";

    public DragFloatRangeSearch(float min, float max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }

    protected abstract float GetMinValue(Job job);
    protected abstract void SetMinValue(Job job, float value);
    protected abstract float GetMaxValue(Job job);
    protected abstract void SetMaxValue(Job job, float value);
    protected override void DrawMain(Job job)
    {
        var minValue = GetMinValue(job);
        var maxValue = GetMaxValue(job);
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragFloatRange2($"##Config_{ID}", ref minValue, ref maxValue, Speed, Min, Max))
        {
            SetMinValue(job, Math.Max(Math.Min(minValue, maxValue), Min));
            SetMaxValue(job, Math.Min(Math.Max(minValue, maxValue), Max));
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(job, false);
    }
}
