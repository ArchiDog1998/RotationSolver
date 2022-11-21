using ImGuiNET;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ComboSet : IDraw
    {
        public ClassJobID JobID { get; set; } = ClassJobID.Adventurer;
        public string AuthorName { get; set; } = "神秘作者";
        public string Description { get; set; } = string.Empty;

        [DisplayName("紧急GCD")]
        [Description("用来放置最为重要的GCD")]
        public ActionsSet EmergencyGCDSet { get; set; } = new ActionsSet();

        [DisplayName("通用GCD")]
        [Description("最常规的GCD技能放这里。")]
        public ActionsSet GeneralGCDSet { get; set; } = new ActionsSet();

        [DisplayName("范围防御GCD")]
        [Description("范围防御的GCD技能放这里。")]
        public ActionsSet DefenceAreaGCDSet { get; set; } = new ActionsSet();

        [DisplayName("单体防御GCD")]
        [Description("单体防御的GCD技能放这里。")]
        public ActionsSet DefenceSingleGCDSet { get; set; } = new ActionsSet();

        [DisplayName("范围治疗GCD")]
        [Description("范围治疗的GCD技能放这里。")]
        public ActionsSet HealAreaGCDSet { get; set; } = new ActionsSet();

        [DisplayName("单体治疗GCD")]
        [Description("单体治疗的GCD技能放这里。")]
        public ActionsSet HealSingleGCDSet { get; set; } = new ActionsSet();

        [DisplayName("移动GCD")]
        [Description("移动的GCD技能放这里。")]
        public ActionsSet MoveGCDSet { get; set; } = new ActionsSet();

        [DisplayName("紧急能力技")]
        [Description("紧急的能力技。")]
        public ActionsSet EmergencyAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
            IsEmergency = true,
        };

        [DisplayName("通用能力技")]
        [Description("通用能力技")]
        public ActionsSet GeneralAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("攻击能力技")]
        [Description("攻击用的能力技")]
        public ActionsSet AttackAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("范围防御能力技")]
        [Description("范围防御能力技")]
        public ActionsSet DefenceAreaAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("单体防御能力技")]
        [Description("单体防御能力技")]
        public ActionsSet DefenceSingleAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("范围治疗能力技")]
        [Description("范围治疗能力技")]
        public ActionsSet HealAreaAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("单体治疗能力技")]
        [Description("单体治疗能力技")]
        public ActionsSet HealSingleAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        [DisplayName("移动能力技")]
        [Description("移动能力技")]
        public ActionsSet MoveAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public void Draw(IScriptCombo combo)
        {
            var desc = Description;

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
            if (ImGui.InputTextMultiline($"##{AuthorName}的{JobID}描述", ref desc, 1024, new Vector2(0, 100)))
            {
                Description = desc;
            }

            if (ImGui.BeginChild($"##{AuthorName}的{JobID}技能描述", new Vector2(-5f, -1f), true))
            {
                foreach (var p in from prop in GetType().GetRuntimeProperties()
                                  where prop.PropertyType == typeof(ActionsSet)
                                  select prop)
                {
                    var value = p.GetValue(this) as ActionsSet;
                    if (ImGui.Selectable(p.GetMemberName(), value == XIVAutoAttackPlugin._scriptComboWindow.ActiveSet))
                    {
                        XIVAutoAttackPlugin._scriptComboWindow.ActiveSet = value;
                    }

                    var d = p.GetMemberDescription();
                    if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(d))
                    {
                        ImGui.SetTooltip(d);
                    }
                }

                ImGui.EndChild();
            }
        }
    }
}
