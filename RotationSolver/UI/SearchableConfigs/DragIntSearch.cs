using ECommons.ExcelServices;
using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class DragIntSearchPlugin : DragIntSearch
{
    private PluginConfigInt _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public DragIntSearchPlugin(PluginConfigInt config)
    {
        _config = config;
    }

    public override void ResetToDefault()
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
    protected abstract int GetValue(Job job);
    protected abstract void SetValue(Job job, int value);
    public override void Draw(Job job)
    {
        var value = GetValue(job);
        if(ImGui.DragInt($"{Name}##Config_{ID}", ref value, Speed, Min, Max))
        {
            SetValue(job, value);
        }
        if (ImGui.IsItemHovered()) ShowTooltip();
    }
}
