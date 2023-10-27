using Dalamud.Utility;
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

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();

    protected override bool IsJob => true;

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
    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(job, _config, Service.Config.GetDefault(job, _config));
    }

    protected override int GetValue(Job job)
    {
        return Service.Config.GetValue(job, _config);
    }

    protected override void SetValue(Job job, int value)
    {
        Service.Config.SetValue(job, _config, value);
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

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetValue(_config, Service.Config.GetDefault(_config));
    }

    protected override int GetValue(Job job)
    {
        return Service.Config.GetValue(_config);
    }

    protected override void SetValue(Job job, int value)
    {
        Service.Config.SetValue(_config, value);
    }
}

internal abstract class DragIntSearch : Searchable
{
    public int Min { get; }
    public int Max { get; }
    public float Speed { get; }
    public Func<string[]> GetNames { get; }
    public DragIntSearch(int min, int max, float speed)
    {
        Min = min; Max = max;
        Speed = speed;
    }
    public DragIntSearch(Func<string[]> getNames)
    {
        GetNames = getNames;
    }
    protected abstract int GetValue(Job job);
    protected abstract void SetValue(Job job, int value);
    protected override void DrawMain(Job job)
    {
        var value = GetValue(job);

        if (GetNames != null && GetNames() is string[] strs && strs.Length > 0)
        {
            ImGui.SetNextItemWidth(Math.Max(ImGui.CalcTextSize(strs[value % strs.Length]).X + 30, DRAG_WIDTH) * Scale);
            if (ImGui.Combo($"##Config_{ID}{GetHashCode()}", ref value, strs, strs.Length))
            {
                SetValue(job, value);
            }
        }
        else
        {
            ImGui.SetNextItemWidth(Scale * DRAG_WIDTH);
            if (ImGui.DragInt($"##Config_{ID}{GetHashCode()}", ref value, Speed, Min, Max))
            {
                SetValue(job, value);
            }
        }

        if (ImGui.IsItemHovered()) ShowTooltip(job);

        if (IsJob) DrawJobIcon();
        ImGui.SameLine();
        ImGui.TextWrapped(Name);
        if (ImGui.IsItemHovered()) ShowTooltip(job, false);
    }
}
