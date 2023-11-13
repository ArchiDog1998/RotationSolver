using Dalamud.Utility;
using ECommons.Configuration;
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

    protected override float MinValue
    {
        get => Service.Config.GetValue(_configMin);
        set => Service.Config.SetValue(_configMin, value);
    }

    protected override float MaxValue
    {
        get => Service.Config.GetValue(_configMax);
        set => Service.Config.SetValue(_configMax, value);
    }

    public DragFloatRangeSearchJob(JobConfigFloat configMin, JobConfigFloat configMax, float speed)
        : base((float)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed,
          configMin.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
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

    protected override float MinValue
    {
        get => Service.Config.GetValue(_configMin);
        set => Service.Config.SetValue(_configMin, value);
    }

    protected override float MaxValue
    {
        get => Service.Config.GetValue(_configMax);
        set => Service.Config.SetValue(_configMax, value);
    }

    public DragFloatRangeSearchPlugin(PluginConfigFloat configMin, PluginConfigFloat configMax, float speed, uint color = 0)
    : base((float)(configMin.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(configMin.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed, configMin.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
    {
        _configMin = configMin;
        _configMax = configMax;
        Color = color;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_configMin, Service.Config.GetDefault(_configMin));
        Service.Config.SetValue(_configMax, Service.Config.GetDefault(_configMax));
    }
}

internal abstract class DragFloatRangeSearch : Searchable
{
    public float Min { get; init; }
    public float Max { get; init; }
    public float Speed { get; init; }
    public ConfigUnitType Unit { get; init; }

    public sealed override string Command => "";
    protected abstract float MinValue { get; set; }
    protected abstract float MaxValue { get; set; }
    public DragFloatRangeSearch(float min, float max, float speed, ConfigUnitType unit)
    {
        Min = min; Max = max;
        Speed = speed;
        Unit = unit;
    }

    protected override void DrawMain()
    {
        var minValue = MinValue;
        var maxValue = MaxValue;
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);

        if (ImGui.DragFloatRange2($"##Config_{ID}{GetHashCode()}", ref minValue, ref maxValue, Speed, Min, Max,
     Unit == ConfigUnitType.Percent ? $"{minValue * 100:F1}{Unit.ToSymbol()}" : $"{minValue:F2}{Unit.ToSymbol()}",
    Unit == ConfigUnitType.Percent ? $"{maxValue * 100:F1}{Unit.ToSymbol()}" : $"{maxValue:F2}{Unit.ToSymbol()}"))
        {
            MinValue = Math.Min(minValue, maxValue);
            MaxValue = Math.Max(minValue, maxValue);
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        if (Color != 0) ImGui.PushStyleColor(ImGuiCol.Text, Color);
        ImGui.TextWrapped(Name);
        if (Color != 0) ImGui.PopStyleColor();
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}
