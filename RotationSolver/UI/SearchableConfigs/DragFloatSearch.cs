using Dalamud.Utility;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragFloatSearchJob : DragFloatSearch
{
    private readonly JobConfigFloat _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description
    {
        get
        {
            var baseDesc = _config.ToDescription();
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
    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();
    protected override bool IsJob => true;

    protected override float Value
    {
        get => Service.Config.GetValue(_config);
        set => Service.Config.SetValue(_config, value);
    }

    public DragFloatSearchJob(JobConfigFloat config, float speed)
          : base((float)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed,
          config.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
    {
        _config = config;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }
}

internal class DragFloatSearchPlugin : DragFloatSearch
{
    private readonly PluginConfigFloat _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description
    {
        get
        {
            var baseDesc = _config.ToDescription();
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

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();

    protected override float Value
    { 
        get => Service.Config.GetValue(_config);
        set => Service.Config.SetValue(_config, value); 
    }

    public DragFloatSearchPlugin(PluginConfigFloat config, float speed, uint color = 0)
        : base((float)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0f), (float)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1f), speed,
          config.GetAttribute<UnitAttribute>()?.UnitType ?? ConfigUnitType.None)
    {
        _config = config;
        Color = color;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }
}


internal abstract class DragFloatSearch : Searchable
{
    public float Min { get; init; }
    public float Max { get; init; }
    public float Speed { get; init; }
    public ConfigUnitType Unit { get; init; }
    public DragFloatSearch(float min, float max, float speed, ConfigUnitType unit)
    {
        Min = min; Max = max;
        Speed = speed;
        Unit = unit;
    }

    protected abstract float Value { get; set; }

    protected override void DrawMain()
    {
        var value = Value;
        ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);

        if (Unit == ConfigUnitType.Percent)
        {
            if (ImGui.SliderFloat($"##Config_{ID}{GetHashCode()}", ref value, Min, Max, $"{value * 100f:F1}{Unit.ToSymbol()}"))
            {
                Value = value;
            }
        }
        else
        {
            if (ImGui.DragFloat($"##Config_{ID}{GetHashCode()}", ref value, Speed, Min, Max, $"{value:F2}{Unit.ToSymbol()}"))
            {
                Value = value;
            }
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
