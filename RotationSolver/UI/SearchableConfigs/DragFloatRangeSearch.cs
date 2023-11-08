using Dalamud.Utility;
using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;
internal class DragFloatRangeSearchJob : DragFloatRangeSearch
{
    private readonly JobConfigFloat _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description
    {
        get
        {
            var baseDesc = _configMin.ToDescription();
            if (!string.IsNullOrEmpty(baseDesc))
            {
                return baseDesc + "\n" + Unit.ToDesc();
            }
            else
            {
                return Unit.ToDesc();
            }
        }
    }
    public override LinkDescription[] Tooltips => _configMin.ToAction();

    protected override bool IsJob => true;

    public DragFloatRangeSearchJob(JobConfigFloat configMin, JobConfigFloat configMax, float speed)
        : base((float)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed,
          configMin.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
    {
        _configMin = configMin;
        _configMax = configMax;
    }

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(job, _configMin, Service.Config.GetDefault(job, _configMin));
        Service.Config.SetValue(job, _configMax, Service.Config.GetDefault(job, _configMax));
    }

    protected override float GetMinValue(Job job)
    {
        return Service.Config.GetValue(job, _configMin);
    }

    protected override void SetMinValue(Job job, float value)
    {
        Service.Config.SetValue(job, _configMin, value);
    }

    protected override float GetMaxValue(Job job)
    {
        return Service.Config.GetValue(job, _configMax);
    }

    protected override void SetMaxValue(Job job, float value)
    {
        Service.Config.SetValue(job, _configMax, value);
    }
}

internal class DragFloatRangeSearchPlugin : DragFloatRangeSearch
{
    private readonly PluginConfigFloat _configMin, _configMax;

    public override string ID => _configMin.ToString();

    public override string Name => _configMin.ToName();

    public override string Description
    {
        get
        {
            var baseDesc = _configMin.ToDescription();
            if (!string.IsNullOrEmpty(baseDesc))
            {
                return baseDesc + "\n" + Unit.ToDesc();
            }
            else
            {
                return Unit.ToDesc();
            }
        }
    }

    public override LinkDescription[] Tooltips => _configMin.ToAction();


    public DragFloatRangeSearchPlugin(PluginConfigFloat configMin, PluginConfigFloat configMax, float speed, uint color = 0)
    : base((float)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed, configMin.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
    {
        _configMin = configMin;
        _configMax = configMax;
        Color = color;
    }

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(_configMin, Service.Config.GetDefault(_configMin));
        Service.Config.SetValue(_configMax, Service.Config.GetDefault(_configMax));
    }

    protected override float GetMinValue(Job job)
    {
        return Service.Config.GetValue(_configMin);
    }

    protected override void SetMinValue(Job job, float value)
    {
        Service.Config.SetValue(_configMin, value);
    }

    protected override float GetMaxValue(Job job)
    {
        return Service.Config.GetValue(_configMax);
    }

    protected override void SetMaxValue(Job job, float value)
    {
        Service.Config.SetValue(_configMax, value);
    }
}

internal abstract class DragFloatRangeSearch : Searchable
{
    public float Min { get; init; }
    public float Max { get; init; }
    public float Speed { get; init; }
    public ConfigUnitType Unit { get; init; }

    public sealed override string Command => "";

    public DragFloatRangeSearch(float min, float max, float speed, ConfigUnitType unit)
    {
        Min = min; Max = max;
        Speed = speed;
        Unit = unit;
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

        if (ImGui.DragFloatRange2($"##Config_{ID}{GetHashCode()}", ref minValue, ref maxValue, Speed, Min, Max,
     Unit == ConfigUnitType.Percent ? $"{minValue * 100:F1}{Unit.ToSymbol()}" : $"{minValue:F2}{Unit.ToSymbol()}",
    Unit == ConfigUnitType.Percent ? $"{maxValue * 100:F1}{Unit.ToSymbol()}" : $"{maxValue:F2}{Unit.ToSymbol()}"))
        {
            SetMinValue(job, Math.Min(minValue, maxValue));
            SetMaxValue(job, Math.Max(minValue, maxValue));
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
