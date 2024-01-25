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
        public override string Name => LocalizationManager._rightLang.ConfigWindow_Options_ForcedDisableCondition;

        public override string Description => LocalizationManager._rightLang.ConfigWindow_Options_ForcedDisableConditionDesc;

        protected override bool Value
        {
            get => Service.Config.GetDisableBoolRaw(_config);
            set => Service.Config.SetDisableBoolRaw(_config, value);
        }

        public CheckBoxDisable(PluginConfigBool config) : base(config)
        {
        }
        public override void ResetToDefault()
        {
            Service.Config.SetDisableBoolRaw(_config, false);
        }

        protected override ConditionSet GetCondition(Job job)
        {
            return DataCenter.RightSet.GetDisableCondition(_config);
        }
    }

    private class CheckBoxEnable : CheckBoxConditionAbstract
    {
        public override string Name => LocalizationManager._rightLang.ConfigWindow_Options_ForcedEnableCondition;

        public override string Description => LocalizationManager._rightLang.ConfigWindow_Options_ForcedEnableConditionDesc;

        protected override bool Value 
        { 
            get => Service.Config.GetEnableBoolRaw(_config);
            set => Service.Config.SetEnableBoolRaw(_config, value);
        }

        public CheckBoxEnable(PluginConfigBool config) : base(config)
        {
        }

        public override void ResetToDefault()
        {
            Service.Config.SetEnableBoolRaw(_config, false);
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

    protected override bool Value
    {
        get => Service.Config.GetBoolRaw(_config);
        set => Service.Config.SetBoolRaw(_config, value);
    }

    public CheckBoxSearchPlugin(PluginConfigBool config, params ISearchable[] children)
        : base(config == PluginConfigBool.UseAdditionalConditions ? children
            : new ISearchable[]
        {
            new CheckBoxEnable(config), new CheckBoxDisable(config),
        }.Concat(children).ToArray())
    {
        _config = config;
    }

    public override void ResetToDefault()
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

    protected abstract bool Value { get; set; }

    protected virtual void DrawChildren()
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

            child.Draw();
        }
    }

    protected virtual void DrawMiddle()
    {

    }

    protected override void DrawMain()
    {
        var hasChild = Children != null && Children.Any(c => c.ShowInChild);
        var hasAdditional = AdditionalDraw != null;
        var hasSub = hasChild || hasAdditional;
        IDalamudTextureWrap texture = null;
        var hasIcon = Action != ActionID.None && IconSet.GetTexture(Action, out texture);

        var enable = Value;
        if (ImGui.Checkbox($"##{ID}", ref enable))
        {
            Value = enable;
        }
        if (ImGui.IsItemHovered()) ShowTooltip();

        ImGui.SameLine();

        var name = $"{Name}##Config_{ID}{GetHashCode()}";
        if (hasIcon)
        {
            ImGui.BeginGroup();
            var cursor = ImGui.GetCursorPos();
            var size = ImGuiHelpers.GlobalScale * 32;
            if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * size, ID))
            {
                Value = enable;
            }
            ImGuiHelper.DrawActionOverlay(cursor, size, enable ? 1 : 0);
            ImGui.EndGroup();

            if (ImGui.IsItemHovered()) ShowTooltip();
        }
        else if (hasSub)
        {
            if (enable || AlwaysShowChildren)
            {
                var x = ImGui.GetCursorPosX();
                DrawMiddle();
                var drawBody = ImGui.TreeNode(name);
                if (ImGui.IsItemHovered()) ShowTooltip();

                if (drawBody)
                {
                    ImGui.SetCursorPosX(x);
                    ImGui.BeginGroup();
                    AdditionalDraw?.Invoke();
                    if (hasChild)
                    {
                        DrawChildren();
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
                if (ImGui.IsItemHovered()) ShowTooltip(false);

                ImGui.PopStyleColor(2);
            }
        }
        else
        {
            ImGui.TextWrapped(Name);
            if (ImGui.IsItemHovered()) ShowTooltip(false);
        }
    }
}
