using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;

namespace RotationSolver.UI.SearchableSettings;

internal class CheckBoxSearchPlugin : CheckBoxSearch
{
    private abstract class CheckBoxConditionAbstract : CheckBoxSearch
    {
        protected readonly PluginConfigBool _config;

        public override string SearchingKeys => string.Empty;

        public override string Command => string.Empty;

        public override LinkDescription[] Tooltips => null;

        public override string ID => _config.ToString() + Name;

        public override bool ShowInChild => Service.Config.GetValue(PluginConfigBool.UseAdditionalConditions);

        public CheckBoxConditionAbstract(PluginConfigBool config) : base()
        {
            _config = config;
            AdditionalDraw = () =>
            {
                GetCondition(DataCenter.Job)?.DrawMain(DataCenter.RightNowRotation);
            };
        }

        protected abstract ConditionSet GetCondition(Job job);
    }

    private class CheckBoxDisable : CheckBoxConditionAbstract
    {
        public override string Name => LocalizationManager.RightLang.ConfigWindow_Options_ForcedDisableCondition;

        public override string Description => LocalizationManager.RightLang.ConfigWindow_Options_ForcedDisableConditionDesc;

        public CheckBoxDisable(PluginConfigBool config) : base(config)
        {
        }
        public override void ResetToDefault(Job job)
        {
            Service.Config.SetDisableBoolRaw(_config, false);
        }

        protected override bool GetValue(Job job)
        {
            return Service.Config.GetDisableBoolRaw(_config);
        }

        protected override void SetValue(Job job, bool value)
        {
            Service.Config.SetDisableBoolRaw(_config, value);
        }

        protected override ConditionSet GetCondition(Job job)
        {
            return DataCenter.RightSet.GetDisableCondition(_config);
        }
    }

    private class CheckBoxEnable : CheckBoxConditionAbstract
    {
        public override string Name => LocalizationManager.RightLang.ConfigWindow_Options_ForcedEnableCondition;

        public override string Description => LocalizationManager.RightLang.ConfigWindow_Options_ForcedEnableConditionDesc;

        public CheckBoxEnable(PluginConfigBool config) : base(config)
        {
        }

        public override void ResetToDefault(Job job)
        {
            Service.Config.SetEnableBoolRaw(_config, false);
        }

        protected override bool GetValue(Job job)
        {
            return Service.Config.GetEnableBoolRaw(_config);
        }

        protected override void SetValue(Job job, bool value)
        {
            Service.Config.SetEnableBoolRaw(_config, value);
        }

        protected override ConditionSet GetCondition(Job job)
        {
            return DataCenter.RightSet.GetEnableCondition(_config);
        }
    }


    private readonly PluginConfigBool _config;
    public override string ID => _config.ToString();

    public override string Name => _config.ToName();

    public override string Description => Action == ActionID.None ? _config.ToDescription() : Action.ToString();

    public override LinkDescription[] Tooltips => _config.ToAction();

    public override string Command => _config.ToCommand();

    public override bool AlwaysShowChildren => Service.Config.GetValue(PluginConfigBool.UseAdditionalConditions);

    public CheckBoxSearchPlugin(PluginConfigBool config, params ISearchable[] children)
        : base(config == PluginConfigBool.UseAdditionalConditions ? children
            : new ISearchable[]
        {
            new CheckBoxEnable(config), new CheckBoxDisable(config),
        }.Concat(children).ToArray())
    {
        _config = config;
    }

    protected override bool GetValue(Job job)
    {
        return Service.Config.GetBoolRaw(_config);
    }

    protected override void SetValue(Job job, bool value)
    {
        Service.Config.SetBoolRaw(_config, value);
    }

    public override void ResetToDefault(Job job)
    {
        Service.Config.SetBoolRaw(_config, Service.Config.GetBoolRawDefault(_config));
    }

    protected override void DrawMiddle()
    {
        if (Service.Config.GetValue(PluginConfigBool.UseAdditionalConditions))
        {
            ConditionDrawer.DrawCondition(Service.Config.GetValue(_config));
            ImGui.SameLine();
        }
        base.DrawMiddle();
    }
}

internal abstract class CheckBoxSearch : Searchable
{
    public ISearchable[] Children { get; protected set; }

    public ActionID Action { get; init; } = ActionID.None;

    public Action AdditionalDraw { get; set; } = null;

    public virtual bool AlwaysShowChildren => false;

    public CheckBoxSearch(params ISearchable[] children)
    {
        Children = children;
        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    protected abstract bool GetValue(Job job);
    protected abstract void SetValue(Job job, bool value);

    protected virtual void DrawChildren(Job job)
    {
        var lastIs = false;
        foreach (var child in Children)
        {
            if (!child.ShowInChild) continue;

            var thisIs = child is CheckBoxSearch c && c.Action != ActionID.None && IconSet.GetTexture(c.Action, out var texture);
            if (lastIs && thisIs)
            {
                ImGui.SameLine();
            }
            lastIs = thisIs;

            child.Draw(job);
        }
    }

    protected virtual void DrawMiddle()
    {

    }

    protected override void DrawMain(Job job)
    {
        var hasChild = Children != null && Children.Any(c => c.ShowInChild);
        var hasAdditional = AdditionalDraw != null;
        var hasSub = hasChild || hasAdditional;
        IDalamudTextureWrap texture = null;
        var hasIcon = Action != ActionID.None && IconSet.GetTexture(Action, out texture);

        var enable = GetValue(job);
        if (ImGui.Checkbox($"##{ID}", ref enable))
        {
            SetValue(job, enable);
        }
        if (ImGui.IsItemHovered()) ShowTooltip(job);

        ImGui.SameLine();

        var name = $"{Name}##Config_{ID}{GetHashCode()}";
        if (hasIcon)
        {
            ImGui.BeginGroup();
            var cursor = ImGui.GetCursorPos();
            var size = ImGuiHelpers.GlobalScale * 32;
            if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * size, ID))
            {
                SetValue(job, !enable);
            }
            ImGuiHelper.DrawActionOverlay(cursor, size, enable ? 1 : 0);
            ImGui.EndGroup();

            if (ImGui.IsItemHovered()) ShowTooltip(job);
        }
        else if (hasSub)
        {
            if (enable || AlwaysShowChildren)
            {
                var x = ImGui.GetCursorPosX();
                DrawMiddle();
                var drawBody = ImGui.TreeNode(name);
                if (ImGui.IsItemHovered()) ShowTooltip(job);

                if (drawBody)
                {
                    ImGui.SetCursorPosX(x);
                    ImGui.BeginGroup();
                    AdditionalDraw?.Invoke();
                    if (hasChild)
                    {
                        DrawChildren(job);
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
                if (ImGui.IsItemHovered()) ShowTooltip(job, false);

                ImGui.PopStyleColor(2);
            }
        }
        else
        {
            ImGui.TextWrapped(Name);
            if (ImGui.IsItemHovered()) ShowTooltip(job, false);
        }
    }
}
