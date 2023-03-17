using Dalamud.Interface.Colors;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.UI;
using System;
using System.Numerics;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    private static void DrawParamTabItem(string name, Action act)
    {
        if (act == null) return;
        if (ImGui.BeginTabItem(name))
        {
            if (ImGui.BeginChild("Param", new Vector2(0f, -1f), true))
            {
                act();
                ImGui.EndChild();
            }
            ImGui.EndTabItem();
        }
    }

    private void DrawParamTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Params_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Param Items"))
        {
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Basic, DrawParamBasic);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Delay, DrawParamDelay);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Display, DrawParamDisplay);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Action, DrawParamAction);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Conditon, DrawParamCondition);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Target, DrawParamTarget);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Hostile, DrawParamHostile);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Advanced, DrawParamAdvanced);

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamBasic()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_NeverReplaceIcon,
            ref Service.Config.NeverReplaceIcon,
            LocalizationManager.RightLang.ConfigWindow_Param_NeverReplaceIconDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindow,
            ref Service.Config.UseOverlayWindow,
            LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindowDesc);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_AbilitiesInterval,
            ref Service.Config.AbilitiesInterval, min: 0.5f, max: 0.7f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ActionAhead,
            ref Service.Config.ActionAhead, max: 0.1f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_CountDownAhead,
            ref Service.Config.CountDownAhead, min: 0.5f, max: 0.7f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_SpecialDuration,
            ref Service.Config.SpecialDuration, speed: 0.02f, min: 1, max: 20);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_AddDotGCDCount,
            ref Service.Config.AddDotGCDCount, min: 0, max: 3);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffBetweenArea,
            ref Service.Config.AutoOffBetweenArea);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffCutScene,
            ref Service.Config.AutoOffCutScene);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffWhenDead,
            ref Service.Config.AutoOffWhenDead);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_StartOnCountdown,
            ref Service.Config.StartOnCountdown);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseWorkTask,
            ref Service.Config.UseWorkTask);
    }

    private void DrawParamDelay()
    {
        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_WeaponDelay,
            ref Service.Config.WeaponDelayMin, ref Service.Config.WeaponDelayMax, speed: 0.002f, max: 1);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_HostileDelay,
            ref Service.Config.HostileDelayMin, ref Service.Config.HostileDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_InterruptDelay,
            ref Service.Config.InterruptDelayMin, ref Service.Config.InterruptDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_DeathDelay,
            ref Service.Config.DeathDelayMin, ref Service.Config.DeathDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_WeakenDelay,
            ref Service.Config.WeakenDelayMin, ref Service.Config.WeakenDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_HealDelay,
            ref Service.Config.HealDelayMin, ref Service.Config.HealDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_NotInCombatDelay,
            ref Service.Config.NotInCombatDelayMin, ref Service.Config.NotInCombatDelayMax);

        if (Service.Config.UseStopCasting)
        {
            DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_StopCastingDelay,
                ref Service.Config.StopCastingDelayMin, ref Service.Config.StopCastingDelayMax);
        }
    }

    private void DrawParamAdvanced()
    {
        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Params_VoiceVolume,
    ref Service.Config.VoiceVolume, max: 100);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_SayOutStateChanged,
            ref Service.Config.SayOutStateChanged);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_SayPositional,
            ref Service.Config.SayPositional);

        DrawInputText(LocalizationManager.RightLang.ConfigWindow_Param_PositionaErrorText,
            ref Service.Config.PositionalErrorText, 100,
            LocalizationManager.RightLang.ConfigWindow_Params_LocationWrongTextDesc);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PoslockCasting,
        ref Service.Config.PoslockCasting);

        if (Service.Config.PoslockCasting)
        {
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            DrawCombo(LocalizationManager.RightLang.ConfigWindow_Param_PoslockModifier,
                ref Service.Config.PoslockModifier, EnumTranslations.ToName,
                ConfigurationHelper.Keys,
                LocalizationManager.RightLang.ConfigWindow_Param_PoslockDescription);
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseStopCasting,
            ref Service.Config.UseStopCasting);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowHealthRatio,
            ref Service.Config.ShowHealthRatio);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioBoss,
            ref Service.Config.HealthRatioBoss, speed: 0.02f, min: 0, max: 10);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioDying,
            ref Service.Config.HealthRatioDying, speed: 0.02f, min: 0, max: 10);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioDot,
            ref Service.Config.HealthRatioDot, speed: 0.02f, min: 0, max: 10);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowActionFlag,
            ref Service.Config.ShowActionFlag);

        ImGui.Separator();


    }

    private void DrawParamDisplay()
    {
        var useOverlayWindow = Service.Config.UseOverlayWindow;

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TeachingMode,
            ref Service.Config.TeachingMode);

        if (Service.Config.TeachingMode)
        {
            ImGuiHelper.Spacing();

            DrawColor(LocalizationManager.RightLang.ConfigWindow_Param_TeachingModeColor,
                ref Service.Config.TeachingModeColor);
        }

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowMoveTarget,
                ref Service.Config.ShowMoveTarget);

            if (Service.Config.ShowMoveTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.ConfigWindow_Param_MovingTargetColor,
                    ref Service.Config.MovingTargetColor);
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowTarget,
                ref Service.Config.ShowTarget);

            if (Service.Config.ShowTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.ConfigWindow_Param_TargetColor,
                    ref Service.Config.TargetColor);

                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.ConfigWindow_Params_SubTargetColor,
                    ref Service.Config.SubTargetColor);
            }
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_KeyBoardNoise,
            ref Service.Config.KeyBoardNoise);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowInfoOnDtr,
            ref Service.Config.ShowInfoOnDtr);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowInfoOnToast,
            ref Service.Config.ShowInfoOnToast);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_NamePlateIconId,
            ref Service.Config.NamePlateIconId, 5, 0, 150000, otherThing: RSCommands.UpdateStateNamePlate);

        ImGui.Spacing();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_CastingDisplay,
            ref Service.Config.CastingDisplay);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_FlytextPositional,
            ref Service.Config.FlytextPositional);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedback,
            ref Service.Config.PositionalFeedback,
            LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedbackDesc);

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawPositional,
                ref Service.Config.DrawPositional,
                LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedbackDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawMeleeRange,
                ref Service.Config.DrawMeleeRange);
        }
    }

    private void DrawParamAction()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEAction,
            ref Service.Config.UseAOEAction);

        if(Service.Config.UseAOEAction)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEWhenManual,
                ref Service.Config.UseAOEWhenManual);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_NoNewHostiles,
                ref Service.Config.NoNewHostiles,
                LocalizationManager.RightLang.ConfigWindow_Params_NoNewHostilesDesc);
        }

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoBurst,
            ref Service.Config.AutoBurst);

        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGuiHelper.DisplayCommandHelp(OtherCommandType.Settings, nameof(Service.Config.AutoBurst));

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseItem,
            ref Service.Config.UseItem,
            LocalizationManager.RightLang.ConfigWindow_Param_UseItemDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAbility,
            ref Service.Config.UseAbility);

        if (Service.Config.UseAbility)
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseDefenceAbility,
                ref Service.Config.UseDefenseAbility,
                LocalizationManager.RightLang.ConfigWindow_Param_UseDefenceAbilityDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoShield,
                ref Service.Config.AutoShield);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTank,
                ref Service.Config.AutoProvokeForTank,
                LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTankDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoUseTrueNorth,
                ref Service.Config.AutoUseTrueNorth);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerBySwift,
                ref Service.Config.RaisePlayerBySwift);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseGroundBeneficialAbility,
                ref Service.Config.UseGroundBeneficialAbility);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerByCasting,
            ref Service.Config.RaisePlayerByCasting);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseHealWhenNotAHealer,
            ref Service.Config.UseHealWhenNotAHealer);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_LessMPNoRaise,
            ref Service.Config.LessMPNoRaise, 200, 0, 2000000);
    }

    private void DrawParamCondition()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InterruptibleMoreCheck,
            ref Service.Config.InterruptibleMoreCheck);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_EsunaAll,
            ref Service.Config.EsunaAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_HealOutOfCombat,
            ref Service.Config.HealOutOfCombat);

        const float speed = 0.005f;

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_MeleeRangeOffset,
            ref Service.Config.MeleeRangeOffset, 5 * speed, max: 5);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthDifference,
             ref Service.Config.HealthDifference,
             speed * 2, 0, 0.5f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthAreaAbility,
            ref Service.Config.HealthAreaAbility, speed);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthAreaSpell,
            ref Service.Config.HealthAreaSpell, speed);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthSingleAbility,
            ref Service.Config.HealthSingleAbility, speed);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthSingleSpell,
            ref Service.Config.HealthSingleSpell, speed);
    }

    private void DrawParamTarget()
    {
        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ObjectMinRadius,
            ref Service.Config.ObjectMinRadius, 0.02f, 0, 10);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AddEnemyListToHostile,
            ref Service.Config.AddEnemyListToHostile);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ChooseAttackMark,
            ref Service.Config.ChooseAttackMark);

        if (Service.Config.ChooseAttackMark && Service.Config.UseAOEAction)
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_CanAttackMarkAOE,
                ref Service.Config.CanAttackMarkAOE,
                LocalizationManager.RightLang.ConfigWindow_Param_AttackMarkAOEDesc);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_FilterStopMark,
            ref Service.Config.FilterStopMark);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ChangeTargetForFate,
            ref Service.Config.ChangeTargetForFate);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_OnlyAttackInView,
            ref Service.Config.OnlyAttackInView);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_MoveTowardsScreen,
            ref Service.Config.MoveTowardsScreenCenter,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveTowardsScreenDesc);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_MoveTargetAngle,
            ref Service.Config.MoveTargetAngle, 0.02f, 0, 100,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveTargetAngleDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_MoveAreaActionFarthest,
            ref Service.Config.MoveAreaActionFarthest,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveAreaActionFarthestDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetFriendly,
            ref Service.Config.TargetFriendly);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseAll,
            ref Service.Config.RaiseAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseBrinkofDeath,
            ref Service.Config.RaiseBrinkOfDeath);
    }

    private void DrawParamHostile()
    {
        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddHostileCondition))
        {
            Service.Config.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);
        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            ImGui.Separator();

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];
            if (ImGui.Combo(LocalizationManager.RightLang.ConfigWindow_Param_HostileCondition + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.TargetingTypes[i] = (TargetingType)targingType;
                Service.Config.Save();
            }

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionUp + "##HostileUp" + i.ToString()))
            {
                if (i != 0)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i - 1, value);
                }
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();
            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDown + "##HostileDown" + i.ToString()))
            {
                if (i < Service.Config.TargetingTypes.Count - 1)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i + 1, value);
                }
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDelete + "##HostileDelete" + i.ToString()))
            {
                Service.Config.TargetingTypes.RemoveAt(i);
            }
        }
    }

    private static void DrawCheckBox(string name, ref bool value, string description = "", Action otherThing = null)
    {
        if (ImGui.Checkbox(name, ref value))
        {
            Service.Config.Save();
            otherThing?.Invoke();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawRangedFloat(string name, ref float minValue, ref float maxValue, float speed = 0.01f, float min = 0, float max = 3, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloatRange2(name, ref minValue, ref maxValue, speed, min, max))
        {
            Service.Config.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawFloatNumber(string name, ref float value, float speed = 0.002f, float min = 0, float max = 1, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloat(name, ref value, speed, min, max))
        {
            Service.Config.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawIntNumber(string name, ref int value, float speed = 0.2f, int min = 0, int max = 1, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragInt(name, ref value, speed, min, max))
        {
            Service.Config.Save();
            otherThing?.Invoke();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawColor(string name, ref Vector3 value, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.ColorEdit3(name, ref value))
        {
            Service.Config.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawCombo<T>(string name, ref int value, Func<T, string> toString, T[] choices = null, string description = "") where T : struct, Enum
    {
        choices = choices ?? Enum.GetValues<T>();

        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo(name, toString(choices[value])))
        {
            for (int i = 0; i < choices.Length; i++)
            {
                if (ImGui.Selectable(toString(choices[i])))
                {
                    value = i;
                    Service.Config.Save();
                }
            }
            ImGui.EndCombo();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawInputText(string name, ref string value, uint maxLength, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.InputText(name, ref value, maxLength))
        {
            Service.Config.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }
}
