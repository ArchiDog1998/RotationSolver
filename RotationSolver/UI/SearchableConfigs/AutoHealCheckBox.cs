using ECommons.ExcelServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI.SearchableConfigs;

internal class AutoHealCheckBox : CheckBoxSearchPlugin
{
    private readonly ISearchable[] _otherChildren;

    public AutoHealCheckBox(params ISearchable[] otherChildren)
        : base(PluginConfigBool.AutoHeal, otherChildren.Union(new ISearchable[]
        {
            _healthAreaAbility,
            _healthAreaAbilityHot,
            _healthAreaSpell ,
            _healthAreaSpellHot ,
            _healthSingleAbility ,
            _healthSingleAbilityHot ,
            _healthSingleSpell ,
            _healthSingleSpellHot ,
        }).ToArray())
    {
        _otherChildren = otherChildren;
    }
    private const float speed = 0.005f;

    private static readonly DragFloatSearchJob
        _healthAreaAbility = new(JobConfigFloat.HealthAreaAbility, speed),
        _healthAreaAbilityHot = new(JobConfigFloat.HealthAreaAbilityHot, speed),
        _healthAreaSpell = new(JobConfigFloat.HealthAreaSpell, speed),
        _healthAreaSpellHot = new(JobConfigFloat.HealthAreaSpellHot, speed),
        _healthSingleAbility = new(JobConfigFloat.HealthSingleAbility, speed),
        _healthSingleAbilityHot = new(JobConfigFloat.HealthSingleAbilityHot, speed),
        _healthSingleSpell = new(JobConfigFloat.HealthSingleSpell, speed),
        _healthSingleSpellHot = new(JobConfigFloat.HealthSingleSpellHot, speed);

    protected override void DrawChildren()
    {
        foreach (var child in _otherChildren)
        {
            child.Draw();
        }

        if (ImGui.BeginTable("Healing things", 3, ImGuiTableFlags.Borders
    | ImGuiTableFlags.Resizable
    | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader("");

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager._rightLang.ConfigWindow_Param_Normal);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager._rightLang.ConfigWindow_Param_HOT);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager._rightLang.ConfigWindow_Param_HealthAreaAbility);

            ImGui.TableNextColumn();

            _healthAreaAbility?.Draw();

            ImGui.TableNextColumn();

            _healthAreaAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager._rightLang.ConfigWindow_Param_HealthAreaSpell);

            ImGui.TableNextColumn();

            _healthAreaSpell?.Draw();


            ImGui.TableNextColumn();

            _healthAreaSpellHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager._rightLang.ConfigWindow_Param_HealthSingleAbility);

            ImGui.TableNextColumn();

            _healthSingleAbility?.Draw();

            ImGui.TableNextColumn();

            _healthSingleAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager._rightLang.ConfigWindow_Param_HealthSingleSpell);

            ImGui.TableNextColumn();

            _healthSingleSpell?.Draw();

            ImGui.TableNextColumn();

            _healthSingleSpellHot?.Draw();

            ImGui.EndTable();
        }
    }
}
