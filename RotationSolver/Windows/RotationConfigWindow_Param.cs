using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

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
        ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_Params_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Param Items"))
        {
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Basic, DrawParamBasic);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Delay, DrawParamDelay);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Display, DrawParamDisplay);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Action, DrawParamAction);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Conditon, DrawParamCondition);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Target, DrawParamTarget);
            DrawParamTabItem(LocalizationManager.RightLang.Configwindow_Param_Hostile, DrawParamHostile);

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamBasic()
    {
        //Never Repalce Icon
        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_NeverReplaceIcon,
            ref Service.Configuration.NeverReplaceIcon,
            LocalizationManager.RightLang.Configwindow_Param_NeverReplaceIconDesc);

        //Use Overlay Window
        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseOverlayWindow,
            ref Service.Configuration.UseOverlayWindow,
            LocalizationManager.RightLang.Configwindow_Param_UseOverlayWindowDesc);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_WeaponFaster,
            ref Service.Configuration.WeaponFaster, max: 0.1f);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_WeaponInterval,
            ref Service.Configuration.WeaponInterval, min: 0.5f, max: 0.7f);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_SpecialDuration,
            ref Service.Configuration.SpecialDuration, speed: 0.02f, min: 1, max: 20);

        DrawIntNumber(LocalizationManager.RightLang.Configwindow_Param_AddDotGCDCount,
            ref Service.Configuration.AddDotGCDCount, min: 0, max: 3);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AutoOffBetweenArea,
            ref Service.Configuration.AutoOffBetweenArea);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseWorkTask,
            ref Service.Configuration.UseWorkTask);
    }

    private void DrawParamDelay()
    {
        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_WeaponDelay,
            ref Service.Configuration.WeaponDelayMin, ref Service.Configuration.WeaponDelayMax, speed: 0.002f, max: 1);

        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_HostileDelay,
            ref Service.Configuration.HostileDelayMin, ref Service.Configuration.HostileDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_InterruptDelay,
            ref Service.Configuration.InterruptDelayMin, ref Service.Configuration.InterruptDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_DeathDelay,
            ref Service.Configuration.DeathDelayMin, ref Service.Configuration.DeathDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_WeakenDelay,
            ref Service.Configuration.WeakenDelayMin, ref Service.Configuration.WeakenDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_HealDelay,
            ref Service.Configuration.HealDelayMin, ref Service.Configuration.HealDelayMax);

        if (Service.Configuration.UseStopCasting)
        {
            DrawRangedFloat(LocalizationManager.RightLang.Configwindow_Param_StopCastingDelay,
                ref Service.Configuration.StopCastingDelayMin, ref Service.Configuration.StopCastingDelayMax);
        }

        if (Service.Configuration.UseWorkTask)
        {
            DrawIntNumber(LocalizationManager.RightLang.Configwindow_Param_WorkTaskDelay,
                ref Service.Configuration.WorkTaskDelay, min: 0, max: 200);
        }
    }

    private void DrawParamDisplay()
    {
        var useOverlayWindow = Service.Configuration.UseOverlayWindow;

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_TeachingMode,
            ref Service.Configuration.TeachingMode);

        if (Service.Configuration.TeachingMode)
        {
            ImGuiHelper.Spacing();

            DrawColor(LocalizationManager.RightLang.Configwindow_Param_TeachingModeColor,
                ref Service.Configuration.TeachingModeColor);
        }

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ShowMoveTarget,
                ref Service.Configuration.ShowMoveTarget);

            if (Service.Configuration.ShowMoveTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.Configwindow_Param_MovingTargetColor,
                    ref Service.Configuration.MovingTargetColor);
            }

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ShowTarget,
                ref Service.Configuration.ShowTarget);

            if (Service.Configuration.ShowTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.Configwindow_Param_TargetColor,
                    ref Service.Configuration.TargetColor);

                ImGuiHelper.Spacing();

                DrawColor(LocalizationManager.RightLang.Configwindow_Params_SubTargetColor,
                    ref Service.Configuration.SubTargetColor);
            }
        }

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_KeyBoardNoise,
             ref Service.Configuration.KeyBoardNoise);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_CastingDisplay,
            ref Service.Configuration.CastingDisplay);


        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_PoslockCasting,
                ref Service.Configuration.PoslockCasting);

        if (Service.Configuration.PoslockCasting)
        {
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            DrawCombo(LocalizationManager.RightLang.Configwindow_Param_PoslockModifier,
                ref Service.Configuration.PoslockModifier, EnumTranslations.ToName,
                ConfigurationHelper.Keys,
                LocalizationManager.RightLang.Configwindow_Param_PoslockDescription);
        }

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseStopCasting,
            ref Service.Configuration.UseStopCasting);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_SayOutStateChanged,
            ref Service.Configuration.SayOutStateChanged);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ShowInfoOnDtr,
            ref Service.Configuration.ShowInfoOnDtr);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ShowInfoOnToast,
            ref Service.Configuration.ShowInfoOnToast);

        ImGui.Spacing();

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_FlytextPositional,
            ref Service.Configuration.FlytextPositional);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_SayPositional,
            ref Service.Configuration.SayPotional);

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_PositionalFeedback,
                ref Service.Configuration.PositionalFeedback,
                LocalizationManager.RightLang.Configwindow_Param_PositionalFeedbackDesc);
        }

        DrawIntNumber(LocalizationManager.RightLang.Configwindow_Params_VoiceVolume,
            ref Service.Configuration.VoiceVolume, max: 100);

        DrawInputText(LocalizationManager.RightLang.Configwindow_Param_PositionaErrorText,
            ref Service.Configuration.PositionalErrorText, 100,
            LocalizationManager.RightLang.Configwindow_Params_LocationWrongTextDesc);
    }

    private void DrawParamAction()
    {
        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseAOEWhenManual,
            ref Service.Configuration.UseAOEWhenManual);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_NoNewHostiles,
            ref Service.Configuration.NoNewHostiles,
            LocalizationManager.RightLang.Configwindow_Params_NoNewHostilesDesc);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AutoBreak,
            ref Service.Configuration.AutoBurst);

        ImGui.SameLine();
        ImGuiHelper.Spacing();
        RSCommands.DisplayCommandHelp(OtherCommandType.Settings, nameof(Service.Configuration.AutoBurst));

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseItem,
            ref Service.Configuration.UseItem,
            LocalizationManager.RightLang.Configwindow_Param_UseItemDesc);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseAbility,
            ref Service.Configuration.UseAbility);

        if (Service.Configuration.UseAbility)
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseDefenceAbility,
                ref Service.Configuration.UseDefenceAbility,
                LocalizationManager.RightLang.Configwindow_Param_UseDefenceAbilityDesc);

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AutoShield,
                ref Service.Configuration.AutoShield);

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AutoProvokeForTank,
                ref Service.Configuration.AutoProvokeForTank,
                LocalizationManager.RightLang.Configwindow_Param_AutoProvokeForTankDesc);

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AutoUseTrueNorth,
                ref Service.Configuration.AutoUseTrueNorth);

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_RaisePlayerBySwift,
                ref Service.Configuration.RaisePlayerBySwift);

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseGroundBeneficialAbility,
                ref Service.Configuration.UseGroundBeneficialAbility);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_RaisePlayerByCasting,
            ref Service.Configuration.RaisePlayerByCasting);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_UseHealWhenNotAHealer,
            ref Service.Configuration.UseHealWhenNotAHealer);

        DrawIntNumber(LocalizationManager.RightLang.Configwindow_Param_LessMPNoRaise,
            ref Service.Configuration.LessMPNoRaise, 200, 0, 10000);
    }

    private void DrawParamCondition()
    {
        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_InterruptibleMoreCheck,
            ref Service.Configuration.InterruptibleMoreCheck);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_StartOnCountdown,
            ref Service.Configuration.StartOnCountdown);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_HealOutOfCombat,
            ref Service.Configuration.HealOutOfCombat);

        const float speed = 0.005f;
        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_HealthDifference,
             ref Service.Configuration.HealthDifference,
             speed * 2, 0, 0.5f);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_HealthAreaAbility,
            ref Service.Configuration.HealthAreaAbility, speed);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_HealthAreaSpell,
            ref Service.Configuration.HealthAreafSpell, speed);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_HealthSingleAbility,
            ref Service.Configuration.HealthSingleAbility, speed);

        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_HealthSingleSpell,
            ref Service.Configuration.HealthSingleSpell, speed);
    }

    private void DrawParamTarget()
    {
        DrawFloatNumber(LocalizationManager.RightLang.Configwindow_Param_ObjectMinRadius,
            ref Service.Configuration.ObjectMinRadius, 0.02f, 0, 10);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_AddEnemyListToHostile,
            ref Service.Configuration.AddEnemyListToHostile);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ChooseAttackMark,
            ref Service.Configuration.ChooseAttackMark);

        if (Service.Configuration.ChooseAttackMark)
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_CanAttackMarkAOE,
                ref Service.Configuration.CanAttackMarkAOE,
                LocalizationManager.RightLang.Configwindow_Param_AttackMarkAOEDesc);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_FilterStopMark,
            ref Service.Configuration.FilterStopMark);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_ChangeTargetForFate,
            ref Service.Configuration.ChangeTargetForFate);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_MoveTowardsScreen,
            ref Service.Configuration.MoveTowardsScreenCenter,
            LocalizationManager.RightLang.Configwindow_Param_MoveTowardsScreenDesc);

        DrawIntNumber(LocalizationManager.RightLang.Configwindow_Param_MoveTargetAngle,
            ref Service.Configuration.MoveTargetAngle, 0.02f, 0, 100,
            LocalizationManager.RightLang.Configwindow_Param_MoveTargetAngleDesc);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_MoveAreaActionFarthest,
                ref Service.Configuration.MoveAreaActionFarthest,
                LocalizationManager.RightLang.Configwindow_Param_MoveAreaActionFarthestDesc);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_RaiseAll,
            ref Service.Configuration.RaiseAll);

        DrawCheckBox(LocalizationManager.RightLang.Configwindow_Param_RaiseBrinkofDeath,
            ref Service.Configuration.RaiseBrinkofDeath);
    }

    private void DrawParamHostile()
    {
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Param_AddHostileCondition))
        {
            Service.Configuration.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGui.Text(LocalizationManager.RightLang.Configwindow_Param_HostileDesc);
        for (int i = 0; i < Service.Configuration.TargetingTypes.Count; i++)
        {
            ImGui.Separator();

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Configuration.TargetingTypes[i];
            if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Param_HostileCondition + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Configuration.TargetingTypes[i] = (TargetingType)targingType;
                Service.Configuration.Save();
            }

            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Param_ConditionUp + "##HostileUp" + i.ToString()))
            {
                if (i != 0)
                {
                    var value = Service.Configuration.TargetingTypes[i];
                    Service.Configuration.TargetingTypes.RemoveAt(i);
                    Service.Configuration.TargetingTypes.Insert(i - 1, value);
                }
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();
            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Param_ConditionDown + "##HostileDown" + i.ToString()))
            {
                if (i < Service.Configuration.TargetingTypes.Count - 1)
                {
                    var value = Service.Configuration.TargetingTypes[i];
                    Service.Configuration.TargetingTypes.RemoveAt(i);
                    Service.Configuration.TargetingTypes.Insert(i + 1, value);
                }
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Param_ConditionDelete + "##HostileDelete" + i.ToString()))
            {
                Service.Configuration.TargetingTypes.RemoveAt(i);
            }
        }
    }

    private static void DrawCheckBox(string name, ref bool value, string description = "")
    {
        if (ImGui.Checkbox(name, ref value))
        {
            Service.Configuration.Save();
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
            Service.Configuration.Save();
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
            Service.Configuration.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }

    private static void DrawIntNumber(string name, ref int value, float speed = 0.2f, int min = 0, int max = 1, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragInt(name, ref value, speed, min, max))
        {
            Service.Configuration.Save();
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
            Service.Configuration.Save();
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
                    Service.Configuration.Save();
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
            Service.Configuration.Save();
        }
        if (!string.IsNullOrEmpty(description) && ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(description);
        }
    }
}
