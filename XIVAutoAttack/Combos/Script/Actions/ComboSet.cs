using ImGuiNET;
using System;
using System.Numerics;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ComboSet : IDraw
    {
        public ClassJobID JobID { get; set; } = ClassJobID.Adventurer;
        public string AuthorName { get; set; } = "神秘作者";
        public string Description { get; set; } = String.Empty;

        public ActionsSet EmergencyGCDSet { get; set; } = new ActionsSet()
        {
            Name = "紧急GCD",
            Description = "用来放置最为重要的GCD",
        };
        public ActionsSet GeneralGCDSet { get; set; } = new ActionsSet()
        {
            Name = "通用GCD",
            Description = "最常规的GCD技能放这里。",
        };

        public ActionsSet DefenceAreaGCDSet { get; set; } = new ActionsSet()
        {
            Name = "范围防御GCD",
            Description = "范围防御的GCD技能放这里。",
        };

        public ActionsSet DefenceSingleGCDSet { get; set; } = new ActionsSet()
        {
            Name = "单体防御GCD",
            Description = "单体防御的GCD技能放这里。",
        };

        public ActionsSet HealAreaGCDSet { get; set; } = new ActionsSet()
        {
            Name = "范围治疗GCD",
            Description = "范围治疗的GCD技能放这里。",
        };

        public ActionsSet HealSingleGCDSet { get; set; } = new ActionsSet()
        {
            Name = "单体治疗GCD",
            Description = "单体治疗的GCD技能放这里。",
        };

        public ActionsSet MoveGCDSet { get; set; } = new ActionsSet()
        {
            Name = "移动GCD",
            Description = "移动的GCD技能放这里。",
        };

        public ActionsSet EmergencyAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "紧急能力技",
            Description = "紧急的能力技。",
        };

        public ActionsSet GeneralAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "通用能力技",
            Description = "通用能力技。",
        };

        public ActionsSet AttackAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "攻击能力技",
            Description = "攻击用的能力技。",
        };

        public ActionsSet DefenceAreaAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "范围防御能力技",
            Description = "范围防御能力技。",
        };

        public ActionsSet DefenceSingleAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "单体防御能力技",
            Description = "单体防御能力技。",
        };

        public ActionsSet HealAreaAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "范围治疗能力技",
            Description = "范围治疗能力技。",
        };

        public ActionsSet HealSingleAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "单体治疗能力技",
            Description = "单体治疗能力技。",
        };

        public ActionsSet MoveAbilitySet { get; set; } = new ActionsSet()
        {
            Name = "移动能力技",
            Description = "移动能力技。",
        };

        public void Draw(IScriptCombo combo)
        {
            var desc = Description;
            if (ImGui.InputTextMultiline("描述:", ref desc, 1024, new Vector2(250, 250)))
            {
                Description = desc;
            }

            GeneralGCDSet.DrawHeader();
        }
    }
}
