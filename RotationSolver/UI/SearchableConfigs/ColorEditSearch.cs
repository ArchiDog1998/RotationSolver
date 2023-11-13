using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class ColorEditSearchPlugin : ColorEditSearch
{
    private readonly PluginConfigVector4 _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => "";

    protected override Vector4 Value
    {
        get => Service.Config.GetValue(_config);
        set => Service.Config.SetValue(_config, value);
    }

    public ColorEditSearchPlugin(PluginConfigVector4 config)
    {
        _config = config;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }
}

internal abstract class ColorEditSearch : Searchable
{
    protected abstract Vector4 Value { get; set; }

    protected override void DrawMain()
    {
        var value = Value;
        ImGui.SetNextItemWidth(DRAG_WIDTH * 1.5f * Scale);
        if (ImGui.ColorEdit4($"{Name}##Config_{ID}{GetHashCode()}", ref value))
        {
            Value = value;
        }
        if (ImGui.IsItemHovered()) ShowTooltip();
    }
}
