using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Condition;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;

namespace RotationSolver.UI.SearchableConfigs;

internal class CheckBoxSearchCondition(PropertyInfo property, object obj, params Searchable[] children)
    : CheckBoxSearch(property, obj,
    [
        new CheckBoxEnable(property, obj),
        new CheckBoxDisable(property, obj),
        .. children,
    ])
{
    private abstract class CheckBoxConditionAbstract : CheckBoxSearch
    {
        protected readonly ConditionBoolean _condition;
        public override string SearchingKeys => string.Empty;

        public override string Command => string.Empty;

        public override string ID => base.ID + Name;

        public override bool ShowInChild => Service.Config.UseAdditionalConditions;

        public CheckBoxConditionAbstract(PropertyInfo property, object obj) : base(property, obj)
        {
            _condition = (ConditionBoolean)property.GetValue(Service.Config)!;
            AdditionalDraw = () =>
            {
                if (DataCenter.RightNowRotation == null) return;
                RotationConfigWindow.DrawSupporterWarning();
                GetCondition()?.DrawMain(DataCenter.RightNowRotation);
            };
        }

        protected abstract ConditionSet GetCondition();

        public override void ResetToDefault()
        {
            Value = false;
        }
    }

    private class CheckBoxDisable(PropertyInfo property, object obj) : CheckBoxConditionAbstract(property, obj)
    {
        public override string Name => UiString.ForcedDisableCondition.Local();

        public override string Description => UiString.ForcedDisableConditionDesc.Local();

        protected override bool Value
        {
            get => _condition.Disable;
            set => _condition.Disable = value;
        }


        protected override ConditionSet GetCondition()
        {
            return DataCenter.RightSet.GetDisableCondition(_condition.Key);
        }
    }

    private class CheckBoxEnable(PropertyInfo property, object obj) : CheckBoxConditionAbstract(property, obj)
    {
        public override string Name => UiString.ForcedEnableCondition.Local();

        public override string Description => UiString.ForcedEnableConditionDesc.Local();

        protected override bool Value
        {
            get => _condition.Enable;
            set => _condition.Enable = value;
        }

        protected override ConditionSet GetCondition()
        {
            return DataCenter.RightSet.GetEnableCondition(_condition.Key);
        }
    }

    private ConditionBoolean Condition => (ConditionBoolean)_property.GetValue(Service.Config)!;

    public override bool AlwaysShowChildren => Service.Config.UseAdditionalConditions;

    protected override bool Value
    {
        get => Condition.Value;
        set => Condition.Value = value;
    }

    public override void ResetToDefault()
    {
        Condition.ResetValue();
    }

    protected override void DrawMiddle()
    {
        if (AlwaysShowChildren)
        {
            ConditionDrawer.DrawCondition(Condition);
            ImGui.SameLine();
        }
        base.DrawMiddle();
    }

    public override void OnCommand(string value)
    {
        if (!bool.TryParse(value, out var b)) 
        {
            b = !Condition.Value;
        }

        Condition.Value = b;
    }
}