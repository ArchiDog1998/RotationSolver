using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;

namespace RotationSolver.UI.SearchableSettings;

internal class CheckBoxSearchPlugin : CheckBoxSearch
{
    private PluginConfigBool _config;
    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => _config.ToDescription();

    public CheckBoxSearchPlugin(PluginConfigBool config, params ISearchable[] children)
    {
        _config = config;
        Children = children;
    }

    protected override bool GetValue(Job job)
    {
        return Service.ConfigNew.GetValue(_config);
    }

    protected override void SetValue(Job job, bool value)
    {
        Service.ConfigNew.SetValue(_config, value);
    }
}

internal abstract class CheckBoxSearch : Searchable
{
    public abstract string ID { get; }

    public ISearchable[] Children { get; protected set; }

    protected abstract bool GetValue(Job job);
    protected abstract void SetValue(Job job, bool value);

    public override void Draw(Job job)
    {
        var enable = GetValue(job);
        if (ImGui.Checkbox($"##{ID}", ref enable))
        {
            SetValue(job, enable);
        }
        if(ImGui.IsItemHovered()) ShowTooltip();

        var name = $"{Name}##Config_{ID}";
        if (enable)
        {
            var x = ImGui.GetCursorPosX();
            var drawBody = ImGui.TreeNode(name) && Children != null && Children.Length > 0;
            if (ImGui.IsItemHovered()) ShowTooltip();

            if (drawBody)
            {
                ImGui.SetCursorPosX(x);
                ImGui.BeginGroup();
                foreach (var child in Children)
                {
                    child.Draw(job);
                }
                ImGui.EndGroup();
                ImGui.TreePop();
            }
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, 0x0);
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, 0x0);
            ImGui.TreeNodeEx(name, ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen);
            if (ImGui.IsItemHovered()) ShowTooltip();

            ImGui.PopStyleColor(2);
        }
    }
}
