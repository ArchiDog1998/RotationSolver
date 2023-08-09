using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;

namespace RotationSolver.UI.SearchableConfigs;

internal class ColorEditSearchPlugin : ColorEditSearch
{
    private readonly PluginConfigVector4 _config;

    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public override Action DrawTooltip => _config.ToAction();

    public override string Command => _config.ToCommand();

    public ColorEditSearchPlugin(PluginConfigVector4 config)
    {
        _config = config;
    }

    public override void ResetToDefault(Job job)
    {
        Service.ConfigNew.SetValue(_config, Service.ConfigNew.GetDefault(_config));
    }

    protected override Vector4 GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(_config);
    }

    protected override void SetValue(Job job, Vector4 value)
    {
        Service.ConfigNew.SetValue(_config, value);
    }
}

internal abstract class ColorEditSearch : Searchable
{
    protected abstract Vector4 GetValue(Job job);
    protected abstract void SetValue(Job job, Vector4 value);
    public override void Draw(Job job)
    {
        var value = GetValue(job);
        if(ImGui.ColorEdit4($"{Name}##Config_{ID}", ref value))
        {
            SetValue(job, value);
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);
    }
}
