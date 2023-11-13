using Dalamud.Utility;
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
    protected override bool IsJob => true;
    protected override int MinValue
    {
        get => Service.Config.GetValue(_configMin);
        set => Service.Config.SetValue(_configMin, value);
    }

    protected override int MaxValue
    {
        get => Service.Config.GetValue(_configMax);
        set => Service.Config.SetValue(_configMax, value);
    }

    public DragIntRangeSearchJob(JobConfigInt configMin, JobConfigInt configMax, float speed)
    : base((int)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0), (int)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1), speed)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_configMin, Service.Config.GetDefault(_configMin));
        Service.Config.SetValue(_configMax, Service.Config.GetDefault(_configMax));
    }
}


internal class DragIntRangeSearchPlugin : DragIntRangeSearch
{
    private readonly PluginConfigInt _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description => _configMin.ToDescription();

    public override LinkDescription[] Tooltips => _configMin.ToAction();

    protected override int MinValue
    {
        get => Service.Config.GetValue(_configMin);
        set => Service.Config.SetValue(_configMin, value);
    }

    protected override int MaxValue
    {
        get => Service.Config.GetValue(_configMax);
        set => Service.Config.SetValue(_configMax, value);
    }

    public DragIntRangeSearchPlugin(PluginConfigInt configMin, PluginConfigInt configMax, float speed)
        : base((int)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0), (int)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1), speed)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_configMin, Service.Config.GetDefault(_configMin));
        Service.Config.SetValue(_configMax, Service.Config.GetDefault(_configMax));
    }
}

internal abstract class DragIntRangeSearch : Searchable
{
    public int Min { get; init; }
    public int Max { get; init; }
    public float Speed { get; init; }

    public sealed override string Command => "";
    protected abstract int MinValue { get; set; }
    protected abstract int MaxValue { get; set; }
    public DragIntRangeSearch(int min, int max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }

    protected override void DrawMain()
    {
        var minValue = MinValue;
        var maxValue = MaxValue;
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
        if (ImGui.DragIntRange2($"##Config_{ID}{GetHashCode()}", ref minValue, ref maxValue, Speed, Min, Max))
        {
            MinValue = Math.Min(minValue, maxValue);
            MaxValue = Math.Max(minValue, maxValue);
        }
        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}
