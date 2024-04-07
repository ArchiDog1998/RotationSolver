using RotationSolver.Basic.Configuration;
using RotationSolver.Data;
using RotationSolver.Localization;
using System.Linq;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;

namespace RotationSolver.UI.SearchableConfigs;

internal class AutoHealCheckBox(PropertyInfo property, object obj,  params Searchable[] otherChildren) 
    : CheckBoxSearchCondition(property, obj, otherChildren.Union(SetUp(obj)).ToArray())
{
    private readonly Searchable[] _otherChildren = otherChildren;

    private static DragFloatSearch
        _healthAreaAbility = null!,
        _healthAreaAbilityHot = null!,
        _healthAreaSpell = null!,
        _healthAreaSpellHot = null!,
        _healthSingleAbility = null!,
        _healthSingleAbilityHot = null!,
        _healthSingleSpell = null!,
        _healthSingleSpellHot = null!;

    private static Searchable[] SetUp(object obj)
    {
        _healthAreaAbility = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthAreaAbility))!, obj);
        _healthAreaAbilityHot = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthAreaAbilityHot))!, obj);
        _healthAreaSpell = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthAreaSpell))!, obj);
        _healthAreaSpellHot = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthAreaSpellHot))!, obj);
        _healthSingleAbility = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthSingleAbility))!, obj);
        _healthSingleAbilityHot = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthSingleAbilityHot))!, obj);
        _healthSingleSpell = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthSingleSpell))!, obj);
        _healthSingleSpellHot = new(typeof(Configs).GetRuntimeProperty(nameof(Configs.HealthSingleSpellHot))!, obj);

        return [
            _healthAreaAbility,
            _healthAreaAbilityHot,
            _healthAreaSpell ,
            _healthAreaSpellHot ,
            _healthSingleAbility ,
            _healthSingleAbilityHot ,
            _healthSingleSpell ,
            _healthSingleSpellHot ,
        ];
    }

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
            ImGui.TableHeader(UiString.NormalTargets.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.HotTargets.Local());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(UiString.HpAoe0Gcd.Local());

            ImGui.TableNextColumn();

            _healthAreaAbility?.Draw();

            ImGui.TableNextColumn();

            _healthAreaAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(UiString.HpAoeGcd.Local());

            ImGui.TableNextColumn();

            _healthAreaSpell?.Draw();


            ImGui.TableNextColumn();

            _healthAreaSpellHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(UiString.HpSingle0Gcd.Local());

            ImGui.TableNextColumn();

            _healthSingleAbility?.Draw();

            ImGui.TableNextColumn();

            _healthSingleAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(UiString.HpSingleGcd.Local());

            ImGui.TableNextColumn();

            _healthSingleSpell?.Draw();

            ImGui.TableNextColumn();

            _healthSingleSpellHot?.Draw();

            ImGui.EndTable();
        }
    }
}
