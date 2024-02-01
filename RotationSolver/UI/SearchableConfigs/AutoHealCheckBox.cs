using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI.SearchableConfigs;

internal class AutoHealCheckBox(PropertyInfo property, params ISearchable[] otherChildren) 
    : CheckBoxSearchPlugin(property, otherChildren.Union(new ISearchable[]
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
    private readonly ISearchable[] _otherChildren = otherChildren;

    private static readonly DragFloatSearch
        _healthAreaAbility = new(typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthAreaAbility))!),
        _healthAreaAbilityHot = new(typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthAreaAbilityHot))!),
        _healthAreaSpell = new(typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthAreaSpell))!),
        _healthAreaSpellHot =  new (typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthAreaSpellHot))!),
        _healthSingleAbility =  new (typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthSingleAbility))!),
        _healthSingleAbilityHot =  new (typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthSingleAbilityHot))!),
        _healthSingleSpell =  new (typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthSingleSpell))!),
        _healthSingleSpellHot =  new (typeof(ConfigsNew).GetRuntimeProperty(nameof(ConfigsNew.HealthSingleSpellHot))!);

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
            ImGui.TableHeader("NormalTargets".Local("Normal Targets"));

            ImGui.TableNextColumn();
            ImGui.TableHeader("HotTargets".Local("Targets with HOT"));

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("HpAoe0Gcd".Local("HP for AoE healing oGCDs"));

            ImGui.TableNextColumn();

            _healthAreaAbility?.Draw();

            ImGui.TableNextColumn();

            _healthAreaAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("HpAoeGcd".Local("HP for AoE healing GCDs"));

            ImGui.TableNextColumn();

            _healthAreaSpell?.Draw();


            ImGui.TableNextColumn();

            _healthAreaSpellHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("HpSingle0Gcd".Local("HP for ST healing oGCDs"));

            ImGui.TableNextColumn();

            _healthSingleAbility?.Draw();

            ImGui.TableNextColumn();

            _healthSingleAbilityHot?.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("HpSingleGcd".Local("HP for ST healing GCDs"));

            ImGui.TableNextColumn();

            _healthSingleSpell?.Draw();

            ImGui.TableNextColumn();

            _healthSingleSpellHot?.Draw();

            ImGui.EndTable();
        }
    }
}
