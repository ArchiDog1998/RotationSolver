using Dalamud.Utility;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragIntSearchJob : DragIntSearch
{
    private readonly JobConfigInt _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();

    protected override bool IsJob => true;
    protected override int Value
    {
        get => Service.Config.GetValue(_config);
        set => Service.Config.SetValue(_config, value);
    }

    public DragIntSearchJob(JobConfigInt config, float speed)
        : base((int)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0), (int)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1), speed)
    {
        _config = config;
    }

    public DragIntSearchJob(JobConfigInt config, Func<string[]> getNames)
    : base(getNames)
    {
        _config = config;
    }
    public override void ResetToDefault()
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }
}

internal class DragIntSearchPlugin : DragIntSearch
{
    private readonly PluginConfigInt _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();
    protected override int Value
    {
        get => Service.Config.GetValue(_config);
        set => Service.Config.SetValue(_config, value);
    }

    public DragIntSearchPlugin(PluginConfigInt config, float speed)
        : base((int)(config.GetAttribute<DefaultAttribute>()?.Min ?? 0), (int)(config.GetAttribute<DefaultAttribute>()?.Max ?? 1), speed)
    {
        _config = config;
    }

    public DragIntSearchPlugin(PluginConfigInt config, Func<string[]> getNames)
        : base(getNames)
    {
        _config = config;
    }

    public override void ResetToDefault()
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }
}

internal abstract class DragIntSearch : Searchable
{
    public int Min { get; }
    public int Max { get; }
    public float Speed { get; }
    public Func<string[]> GetNames { get; }
    protected abstract int Value { get; set; }

    public DragIntSearch(int min, int max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }
    public DragIntSearch(Func<string[]> getNames)
    {
        GetNames = getNames;
    }
    protected override void DrawMain()
    {
        var value = Value;

        if (GetNames != null && GetNames() is string[] strs && strs.Length > 0)
        {
            ImGui.SetNextItemWidth(Math.Max(ImGui.CalcTextSize(strs[value % strs.Length]).X + 30, DRAG_WIDTH) * Scale);
            if (ImGui.Combo($"##Config_{ID}{GetHashCode()}", ref value, strs, strs.Length))
            {
                Value = value;
            }
        }
        else
        {
            ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
            if (ImGui.DragInt($"##Config_{ID}{GetHashCode()}", ref value, Speed, Min, Max))
            {
                Value = value;
            }
        }

        if (ImGui.IsItemHovered()) ShowTooltip();

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(false);
    }
}
