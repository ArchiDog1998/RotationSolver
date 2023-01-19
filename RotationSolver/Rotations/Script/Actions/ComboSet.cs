using ImGuiNET;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.Script;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RotationSolver.Rotations.Script.Actions
{
    internal class ComboSet : IDraw
    {
        public ClassJobID JobID { get; set; } = ClassJobID.Adventurer;
        public string AuthorName { get; set; } = LocalizationManager.RightLang.Scriptwindow_ComboSetAuthorDefault;
        public string GameVersion { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ActionsSet EmergencyGCDSet { get; set; } = new ActionsSet();

        public ActionsSet GeneralGCDSet { get; set; } = new ActionsSet();

        public ActionsSet DefenceAreaGCDSet { get; set; } = new ActionsSet();

        public ActionsSet DefenceSingleGCDSet { get; set; } = new ActionsSet();

        public ActionsSet HealAreaGCDSet { get; set; } = new ActionsSet();

        public ActionsSet HealSingleGCDSet { get; set; } = new ActionsSet();

        public ActionsSet MoveGCDSet { get; set; } = new ActionsSet();

        public ActionsSet EmergencyAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
            IsEmergency = true,
        };

        public ActionsSet GeneralAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public ActionsSet AttackAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };
        public ActionsSet DefenceAreaAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public ActionsSet DefenceSingleAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public ActionsSet HealAreaAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public ActionsSet HealSingleAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public ActionsSet MoveAbilitySet { get; set; } = new ActionsSet()
        {
            IsAbility = true,
        };

        public CountDownSet CountDown { get; set; } = new CountDownSet();


        public void Draw(IScriptCombo combo)
        {
            var desc = Description;

            ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - 20);
            if (ImGui.InputTextMultiline($"##TheDescOf{JobID}From{AuthorName}", ref desc, 1024, new Vector2(0, 100)))
            {
                Description = desc;
            }

            //if (ImGui.BeginChild($"##ActionDescOf{JobID}From{AuthorName}", new Vector2(-5f, -1f), true))
            //{
            //    if (ImGui.Selectable(LocalizationManager.RightLang.Scriptwindow_CountDown, CountDown == RotationSolverPlugin._scriptComboWindow.ActiveSet))
            //    {
            //        RotationSolverPlugin._scriptComboWindow.ActiveSet = CountDown;
            //    }
            //    if (ImGui.IsItemHovered())
            //    {
            //        ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_CountDownDesc);
            //    }

            //    foreach (var p in from prop in GetType().GetRuntimeProperties()
            //                      where prop.PropertyType == typeof(ActionsSet)
            //                      select prop)
            //    {
            //        var value = p.GetValue(this) as ActionsSet;
            //        if (ImGui.Selectable(p.GetMemberName(), value == RotationSolverPlugin._scriptComboWindow.ActiveSet))
            //        {
            //            RotationSolverPlugin._scriptComboWindow.ActiveSet = value;
            //        }

            //        var d = p.GetMemberDescription();
            //        if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(d))
            //        {
            //            ImGui.SetTooltip(d);
            //        }
            //    }

            //    ImGui.EndChild();
            //}
        }
    }
}
