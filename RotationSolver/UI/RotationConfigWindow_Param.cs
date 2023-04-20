using RotationSolver.Commands;
using RotationSolver.Localization;

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
        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ActionAhead,
            ref Service.Config.ActionAhead, Service.Default.ActionAhead, max: 0.5f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_CountDownAhead,
            ref Service.Config.CountDownAhead, Service.Default.CountDownAhead, min: 0.5f, max: 0.7f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_SpecialDuration,
            ref Service.Config.SpecialDuration, Service.Default.SpecialDuration, speed: 0.02f, min: 1, max: 20);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_AddDotGCDCount,
            ref Service.Config.AddDotGCDCount, Service.Default.AddDotGCDCount, min: 0, max: 3);

        ImGui.Spacing();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffBetweenArea,
            ref Service.Config.AutoOffBetweenArea, Service.Default.AutoOffBetweenArea);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffCutScene,
            ref Service.Config.AutoOffCutScene, Service.Default.AutoOffCutScene);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffWhenDead,
            ref Service.Config.AutoOffWhenDead, Service.Default.AutoOffWhenDead);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_StartOnCountdown,
            ref Service.Config.StartOnCountdown, Service.Default.StartOnCountdown);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindow,
            ref Service.Config.UseOverlayWindow, Service.Default.UseOverlayWindow,
            LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindowDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseWorkTask,
            ref Service.Config.UseWorkTask, Service.Default.UseWorkTask);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ToggleManual,
            ref Service.Config.ToggleManual, Service.Default.ToggleManual);
    }

    private void DrawParamDelay()
    {
        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_WeaponDelay,
            ref Service.Config.WeaponDelayMin, ref Service.Config.WeaponDelayMax,
             Service.Default.WeaponDelayMin, Service.Default.WeaponDelayMax,
            speed: 0.002f, max: 1);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_HostileDelay,
            ref Service.Config.HostileDelayMin, ref Service.Config.HostileDelayMax,
            Service.Default.HostileDelayMin, Service.Default.HostileDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_InterruptDelay,
            ref Service.Config.InterruptDelayMin, ref Service.Config.InterruptDelayMax,
             Service.Default.InterruptDelayMin, Service.Default.InterruptDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_DeathDelay,
            ref Service.Config.DeathDelayMin, ref Service.Config.DeathDelayMax,
            Service.Default.DeathDelayMin, Service.Default.DeathDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_WeakenDelay, 
            ref Service.Config.WeakenDelayMin, ref Service.Config.WeakenDelayMax, 
            Service.Default.WeakenDelayMin, Service.Default.WeakenDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_HealDelay, 
            ref Service.Config.HealDelayMin, ref Service.Config.HealDelayMax, 
            Service.Default.HealDelayMin, Service.Default.HealDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_NotInCombatDelay, 
            ref Service.Config.NotInCombatDelayMin, ref Service.Config.NotInCombatDelayMax,
            Service.Default.NotInCombatDelayMin, Service.Default.NotInCombatDelayMax);

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_ClickingDelay,
            ref Service.Config.ClickingDelayMin, ref Service.Config.ClickingDelayMax,
            Service.Default.ClickingDelayMin, Service.Default.ClickingDelayMax
            ,min : 0.05f, max: 0.25f);

        if (Service.Config.UseStopCasting)
        {
            DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_StopCastingDelay, 
                ref Service.Config.StopCastingDelayMin, ref Service.Config.StopCastingDelayMax, 
                Service.Default.StopCastingDelayMin, Service.Default.StopCastingDelayMax);
        }
    }

    private void DrawParamAdvanced()
    {
        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_VoiceVolume,
            ref Service.Config.VoiceVolume, Service.Default.VoiceVolume, max: 100);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_SayOutStateChanged,
            ref Service.Config.SayOutStateChanged, Service.Default.SayOutStateChanged);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_SayPositional,
            ref Service.Config.SayPositional, Service.Default.SayPositional);

        DrawInputText(LocalizationManager.RightLang.ConfigWindow_Param_PositionalErrorText,
            ref Service.Config.PositionalErrorText, 100,
            LocalizationManager.RightLang.ConfigWindow_Params_LocationWrongTextDesc);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PoslockCasting,
        ref Service.Config.PoslockCasting, Service.Default.PoslockCasting);

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
            ref Service.Config.UseStopCasting, Service.Default.UseStopCasting);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowHealthRatio,
            ref Service.Config.ShowHealthRatio, Service.Default.ShowHealthRatio);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioBoss,
            ref Service.Config.HealthRatioBoss, Service.Default.HealthRatioBoss,
            speed: 0.02f, min: 0, max: 10);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioDying,
            ref Service.Config.HealthRatioDying, Service.Default.HealthRatioDying,
            speed: 0.02f, min: 0, max: 10);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthRatioDot,
            ref Service.Config.HealthRatioDot, Service.Default.HealthRatioDot, 
            speed: 0.02f, min: 0, max: 10);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowActionFlag,
            ref Service.Config.ShowActionFlag, Service.Default.ShowActionFlag);
    }

    private void DrawParamDisplay()
    {
        var useOverlayWindow = Service.Config.UseOverlayWindow;

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TeachingMode,
            ref Service.Config.TeachingMode, Service.Default.TeachingMode);

        if (Service.Config.TeachingMode)
        {
            ImGuiHelper.Spacing();

            DrawColor3(LocalizationManager.RightLang.ConfigWindow_Param_TeachingModeColor,
                ref Service.Config.TeachingModeColor, Service.Default.TeachingModeColor);
        }

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowMoveTarget,
                ref Service.Config.ShowMoveTarget, Service.Default.ShowMoveTarget);

            if (Service.Config.ShowMoveTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor3(LocalizationManager.RightLang.ConfigWindow_Param_MovingTargetColor,
                    ref Service.Config.MovingTargetColor, Service.Default.MovingTargetColor);
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowTarget,
                ref Service.Config.ShowTarget, Service.Default.ShowTarget);

            if (Service.Config.ShowTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor3(LocalizationManager.RightLang.ConfigWindow_Param_TargetColor,
                    ref Service.Config.TargetColor, Service.Default.TargetColor);

                ImGuiHelper.Spacing();

                DrawColor3(LocalizationManager.RightLang.ConfigWindow_Param_SubTargetColor,
                    ref Service.Config.SubTargetColor, Service.Default.SubTargetColor);
            }
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_KeyBoardNoise,
            ref Service.Config.KeyBoardNoise, Service.Default.KeyBoardNoise);

        if (Service.Config.KeyBoardNoise)
        {
            ImGui.Indent();

            DrawRangedInt(LocalizationManager.RightLang.ConfigWindow_Param_KeyBoardNoiseTimes,
                ref Service.Config.KeyBoardNoiseMin, ref Service.Config.KeyBoardNoiseMax,
                Service.Default.KeyBoardNoiseMin, Service.Default.KeyBoardNoiseMax);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowInfoOnDtr,
            ref Service.Config.ShowInfoOnDtr, Service.Default.ShowInfoOnDtr);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowInfoOnToast,
            ref Service.Config.ShowInfoOnToast, Service.Default.ShowInfoOnToast);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_NamePlateIconId,
            ref Service.Config.NamePlateIconId, 5, 0, 150000, otherThing: RSCommands.UpdateStateNamePlate);

        ImGui.Spacing();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_CastingDisplay,
            ref Service.Config.CastingDisplay, Service.Default.CastingDisplay);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ToastPositional,
            ref Service.Config.ToastPositional, Service.Default.ToastPositional);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedback,
            ref Service.Config.PositionalFeedback, Service.Default.PositionalFeedback,
            LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedbackDesc);

        if (useOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawPositional,
                ref Service.Config.DrawPositional, Service.Default.DrawPositional,
                LocalizationManager.RightLang.ConfigWindow_Param_PositionalFeedbackDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawMeleeRange,
                ref Service.Config.DrawMeleeRange, Service.Default.DrawMeleeRange);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawMeleeOffset,
                ref Service.Config.DrawMeleeOffset, Service.Default.DrawMeleeOffset);
        }
    }

    private void DrawParamAction()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEAction,
             SettingsCommand.UseAOEAction);

        if(Service.Config.GetValue(SettingsCommand.UseAOEAction))
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEWhenManual,
                SettingsCommand.UseAOEWhenManual);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_NoNewHostiles,
                ref Service.Config.NoNewHostiles, Service.Default.NoNewHostiles,
                LocalizationManager.RightLang.ConfigWindow_Params_NoNewHostilesDesc);
        }

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoBurst, SettingsCommand.AutoBurst);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseItem,
            ref Service.Config.UseItem, Service.Default.UseItem,
            LocalizationManager.RightLang.ConfigWindow_Param_UseItemDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAbility,
            SettingsCommand.UseAbility);

        if (Service.Config.GetValue(SettingsCommand.UseAbility))
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseDefenceAbility,
                SettingsCommand.UseDefenseAbility,
                LocalizationManager.RightLang.ConfigWindow_Param_UseDefenceAbilityDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoShield,
                SettingsCommand.AutoTankStance);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTank,
                SettingsCommand.AutoProvokeForTank,
                LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTankDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoUseTrueNorth,
                SettingsCommand.AutoUseTrueNorth);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerBySwift,
                SettingsCommand.RaisePlayerBySwift);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseGroundBeneficialAbility,
                SettingsCommand.UseGroundBeneficialAbility);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerByCasting,
            ref Service.Config.RaisePlayerByCasting, Service.Default.RaisePlayerByCasting);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseHealWhenNotAHealer,
            ref Service.Config.UseHealWhenNotAHealer, Service.Default.UseHealWhenNotAHealer);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_LessMPNoRaise,
            ref Service.Config.LessMPNoRaise, 200, 0, 2000000);
    }

    private void DrawParamCondition()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InterruptibleMoreCheck,
            ref Service.Config.InterruptibleMoreCheck, Service.Default.InterruptibleMoreCheck);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_EsunaAll,
            ref Service.Config.EsunaAll, Service.Default.EsunaAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_HealOutOfCombat,
            ref Service.Config.HealOutOfCombat, Service.Default.HealOutOfCombat);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_OnlyHotOnTanks,
            ref Service.Config.OnlyHotOnTanks, Service.Default.OnlyHotOnTanks);

        if (Service.Config.GetValue(SettingsCommand.UseGroundBeneficialAbility))
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnTarget,
                ref Service.Config.BeneficialAreaOnTarget, Service.Default.BeneficialAreaOnTarget);
        }

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

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthTankHealerRatio,
            ref Service.Config.HealthTankHealerRatio, speed);
    }

    private void DrawParamTarget()
    {
        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ObjectMinRadius,
            ref Service.Config.ObjectMinRadius, 0.02f, 0, 10);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AddEnemyListToHostile,
            ref Service.Config.AddEnemyListToHostile, Service.Default.AddEnemyListToHostile);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ChooseAttackMark,
            ref Service.Config.ChooseAttackMark, Service.Default.ChooseAttackMark);

        if (Service.Config.ChooseAttackMark && Service.Config.GetValue(SettingsCommand.UseAOEAction))
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_CanAttackMarkAOE,
                ref Service.Config.CanAttackMarkAOE, Service.Default.CanAttackMarkAOE,
                LocalizationManager.RightLang.ConfigWindow_Param_AttackMarkAOEDesc);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_FilterStopMark,
            ref Service.Config.FilterStopMark, Service.Default.FilterStopMark);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ChangeTargetForFate,
            ref Service.Config.ChangeTargetForFate, Service.Default.ChangeTargetForFate);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_OnlyAttackInView,
            ref Service.Config.OnlyAttackInView, Service.Default.OnlyAttackInView);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_MoveTowardsScreen,
            ref Service.Config.MoveTowardsScreenCenter, Service.Default.MoveTowardsScreenCenter,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveTowardsScreenDesc);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_MoveTargetAngle,
            ref Service.Config.MoveTargetAngle, Service.Default.MoveTargetAngle, 0.02f, 0, 100,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveTargetAngleDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_MoveAreaActionFarthest,
            ref Service.Config.MoveAreaActionFarthest, Service.Default.MoveAreaActionFarthest,
            LocalizationManager.RightLang.ConfigWindow_Param_MoveAreaActionFarthestDesc);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetFriendly,
            ref Service.Config.TargetFriendly, Service.Default.TargetFriendly);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseAll,
            ref Service.Config.RaiseAll, Service.Default.RaiseAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseBrinkOfDeath,
            ref Service.Config.RaiseBrinkOfDeath, Service.Default.RaiseBrinkOfDeath);
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

    internal static void DrawCheckBox(string name, SettingsCommand command, string description = "", Action otherThing = null)
    {
        var value = Service.Config.GetValue(command);
        DrawCheckBox(name, ref value, command.GetDefault(), description, () =>
        {
            Service.Config.SetValue(command, value);
            otherThing?.Invoke();
        });

        ImGui.SameLine();
        ImGuiHelper.Spacing();
        ImGuiHelper.DisplayCommandHelp(OtherCommandType.Settings, command.ToString());
    }

    internal static void DrawCheckBox(string name, ref bool value, bool @default, string description = "", Action otherThing = null)
    {
        if (ImGui.Checkbox(name, ref value))
        {
            otherThing?.Invoke();
            Service.Config.Save();
        }
        if (ImGuiHelper.HoveredStringReset(description) && value != @default)
        {
            otherThing?.Invoke();
            value = @default;
            Service.Config.Save();
        }
    }

    private static void DrawRangedFloat(string name, ref float minValue, ref float maxValue, float defaultMin, float defaultMax, float speed = 0.01f, float min = 0, float max = 3, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloatRange2(name, ref minValue, ref maxValue, speed, min, max))
        {
            Service.Config.Save();
        }
        if (ImGuiHelper.HoveredStringReset(description) && (minValue != defaultMin || maxValue != defaultMax))
        {
            minValue = defaultMin;
            maxValue = defaultMax;
            Service.Config.Save();
        }
    }

    private static void DrawRangedInt(string name, ref int minValue, ref int maxValue, int defaultMin, int defaultMax, float speed = 0.01f, int min = 0, int max = 3, string description = "")
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragIntRange2(name, ref minValue, ref maxValue, speed, min, max))
        {
            Service.Config.Save();
        }
        if (ImGuiHelper.HoveredStringReset(description) && (minValue != defaultMin || maxValue != defaultMax))
        {
            minValue = defaultMin;
            maxValue = defaultMax;
            Service.Config.Save();
        }
    }

    private static void DrawFloatNumber(string name, ref float value, float @default, float speed = 0.002f, float min = 0, float max = 1, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloat(name, ref value, speed, min, max))
        {
            Service.Config.Save();
            otherThing?.Invoke();
        }
        if (ImGuiHelper.HoveredStringReset(description) && value != @default)
        {
            value = @default;
            Service.Config.Save();
            otherThing?.Invoke();
        }
    }

    private static void DrawIntNumber(string name, ref int value, int @default, float speed = 0.2f, int min = 0, int max = 1, string description = "", Action otherThing = null)
    {
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragInt(name, ref value, speed, min, max))
        {
            Service.Config.Save();
            otherThing?.Invoke();
        }
        if (ImGuiHelper.HoveredStringReset(description) && value != @default)
        {
            value = @default;
            Service.Config.Save();
            otherThing?.Invoke();
        }
    }

    private static void DrawColor3(string name, ref Vector3 value, Vector3 @default, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.ColorEdit3(name, ref value))
        {
            Service.Config.Save();
        }
        if (ImGuiHelper.HoveredStringReset(description) && value != @default)
        {
            value = @default;
            Service.Config.Save();
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
        ImGuiHelper.HoveredString(description);
    }

    private static void DrawInputText(string name, ref string value, uint maxLength, string description = "")
    {
        ImGui.SetNextItemWidth(210);
        if (ImGui.InputText(name, ref value, maxLength))
        {
            Service.Config.Save();
        }
        ImGuiHelper.HoveredString(description);
    }
}
