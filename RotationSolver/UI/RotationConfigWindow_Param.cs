using RotationSolver.Commands;
using RotationSolver.Localization;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
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
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Condition, DrawParamCondition);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Target, DrawParamTarget);
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_Param_Advanced, DrawParamAdvanced);

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamBasic()
    {
        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ActionAhead,
            ref Service.Config.ActionAhead, Service.Default.ActionAhead, max: 0.5f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_MinLastAbilityAdvanced,
            ref Service.Config.MinLastAbilityAdvanced, Service.Default.MinLastAbilityAdvanced, max: 0.4f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_CountDownAhead,
            ref Service.Config.CountDownAhead, Service.Default.CountDownAhead, min: 0.5f, max: 0.7f);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_SpecialDuration,
            ref Service.Config.SpecialDuration, Service.Default.SpecialDuration, speed: 0.02f, min: 1, max: 20);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_AddDotGCDCount,
            ref Service.Config.AddDotGCDCount, Service.Default.AddDotGCDCount, min: 0, max: 3);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_MaxPing,
                ref Service.Config.MaxPing, Service.Default.MaxPing, min: 0.01f, max: 0.5f);

        ImGui.Spacing();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffBetweenArea,
            ref Service.Config.AutoOffBetweenArea, Service.Default.AutoOffBetweenArea);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffCutScene,
            ref Service.Config.AutoOffCutScene, Service.Default.AutoOffCutScene);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffWhenDead,
            ref Service.Config.AutoOffWhenDead, Service.Default.AutoOffWhenDead);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_AutoOffAfterCombat,
        ref Service.Config.AutoOffAfterCombat, Service.Default.AutoOffAfterCombat, min: 0, max: 10);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_StartOnCountdown,
            ref Service.Config.StartOnCountdown, Service.Default.StartOnCountdown);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_StartOnAttackedBySomeone,
            ref Service.Config.StartOnAttackedBySomeone, Service.Default.StartOnAttackedBySomeone);

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

        DrawRangedFloat(LocalizationManager.RightLang.ConfigWindow_Param_CountdownDelay,
            ref Service.Config.CountdownDelayMin, ref Service.Config.CountdownDelayMax,
            Service.Default.CountdownDelayMin, Service.Default.CountdownDelayMax);

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

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_ClickMistake, ref Service.Config.MistakeRatio, Service.Default.MistakeRatio);
    }


    private void DrawParamAdvanced()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_SayOutStateChanged,
            ref Service.Config.SayOutStateChanged, Service.Default.SayOutStateChanged);

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PoslockCasting,
        ref Service.Config.PoslockCasting, Service.Default.PoslockCasting);

        if (Service.Config.PoslockCasting)
        {
            ImGui.Indent();

            DrawCombo(LocalizationManager.RightLang.ConfigWindow_Param_PoslockModifier,
                ref Service.Config.PoslockModifier, Service.Default.PoslockModifier, EnumTranslations.ToName,
                ConfigurationHelper.Keys,
                LocalizationManager.RightLang.ConfigWindow_Param_PoslockDescription);

            DrawIconCheckBox(ActionID.PassageOfArms, ref Service.Config.PosPassageOfArms);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            DrawIconCheckBox(ActionID.TenChiJin, ref Service.Config.PosTenChiJin);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            DrawIconCheckBox(ActionID.FlameThrower, ref Service.Config.PosFlameThrower);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            DrawIconCheckBox(ActionID.Improvisation, ref Service.Config.PosImprovisation);

            ImGui.Unindent();
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

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowTooltips,
            ref Service.Config.ShowTooltips, Service.Default.ShowTooltips);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InDebug,
            ref Service.Config.InDebug, Service.Default.InDebug);
    }

    private void DrawParamDisplay()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindow,
            ref Service.Config.UseOverlayWindow, Service.Default.UseOverlayWindow,
            LocalizationManager.RightLang.ConfigWindow_Param_UseOverlayWindowDesc, otherThing: () =>
        {
            PainterManager._painter.Enable = Service.Config.UseOverlayWindow;
        });

        if (Service.Config.UseOverlayWindow)
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TeachingMode,
                ref Service.Config.TeachingMode, Service.Default.TeachingMode);

            if (Service.Config.TeachingMode)
            {
                ImGuiHelper.Spacing();

                DrawColor4(LocalizationManager.RightLang.ConfigWindow_Param_TeachingModeColor,
                    ref Service.Config.TeachingModeColor, Service.Default.TeachingModeColor, otherthing: () =>
                    {
                        PainterManager.HighlightColor = Service.Config.TeachingModeColor;
                    });
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowMoveTarget,
                ref Service.Config.ShowMoveTarget, Service.Default.ShowMoveTarget);

            if (Service.Config.ShowMoveTarget)
            {
                ImGuiHelper.Spacing();

                DrawColor4(LocalizationManager.RightLang.ConfigWindow_Param_MovingTargetColor,
                    ref Service.Config.MovingTargetColor, Service.Default.MovingTargetColor);
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ShowTarget,
                ref Service.Config.ShowTarget, Service.Default.ShowTarget);

            if (Service.Config.ShowTarget)
            {
                DrawColor4(LocalizationManager.RightLang.ConfigWindow_Param_TargetColor,
                    ref Service.Config.TargetColor, Service.Default.TargetColor);

                DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_TargetIconSize,
                    ref Service.Config.TargetIconSize, Service.Default.TargetIconSize);

                ImGuiHelper.Spacing();

                DrawColor4(LocalizationManager.RightLang.ConfigWindow_Param_SubTargetColor,
                    ref Service.Config.SubTargetColor, Service.Default.SubTargetColor);
            }

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawPositional,
                ref Service.Config.DrawPositional, Service.Default.DrawPositional);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawMeleeRange,
                ref Service.Config.DrawMeleeRange, Service.Default.DrawMeleeRange);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_DrawMeleeOffset,
                ref Service.Config.DrawMeleeOffset, Service.Default.DrawMeleeOffset);

            DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_DrawingHeight, ref Service.Config.DrawingHeight, Service.Default.DrawingHeight, 0.02f, 0, 8, otherThing: () =>
            {
                PainterManager._painter.DrawingHeight = Service.Config.DrawingHeight;
            });

            DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_SampleLength, ref Service.Config.SampleLength, Service.Default.SampleLength, 0.005f, 0.05f, 3, otherThing: () =>
            {
                PainterManager._painter.SampleLength = Math.Max(0.05f, Service.Config.SampleLength);
            });
        }

        ImGui.Separator();

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
            ref Service.Config.NamePlateIconId, Service.Default.NamePlateIconId, 5, 0, 150000, otherThing: RSCommands.UpdateStateNamePlate);

        ImGui.Spacing();
    }

    private void DrawParamAction()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEAction,
             SettingsCommand.UseAOEAction);

        if(Service.Config.GetValue(SettingsCommand.UseAOEAction))
        {
            ImGui.Indent();
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAOEWhenManual,
                SettingsCommand.UseAOEWhenManual);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_NoNewHostiles,
                ref Service.Config.NoNewHostiles, Service.Default.NoNewHostiles,
                LocalizationManager.RightLang.ConfigWindow_Params_NoNewHostilesDesc);
            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PreventActionsIfOutOfCombat,
            SettingsCommand.PreventActions);

        if (Service.Config.GetValue(SettingsCommand.PreventActions))
        {
            ImGui.Indent();
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_PreventActionsIfDutyRing,
            SettingsCommand.PreventActionsDuty);
            ImGui.Unindent();
        }

        ImGui.Separator();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoBurst, SettingsCommand.AutoBurst);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoHeal, SettingsCommand.AutoHeal);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseTinctures,
            ref Service.Config.UseTinctures, Service.Default.UseTinctures);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseHealPotions,
            ref Service.Config.UseHealPotions, Service.Default.UseHealPotions);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseAbility,
            SettingsCommand.UseAbility);

        if (Service.Config.GetValue(SettingsCommand.UseAbility))
        {
            ImGui.Indent();

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseDefenseAbility,
                SettingsCommand.UseDefenseAbility,
                LocalizationManager.RightLang.ConfigWindow_Param_UseDefenseAbilityDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoShield,
                SettingsCommand.AutoTankStance);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTank,
                SettingsCommand.AutoProvokeForTank,
                LocalizationManager.RightLang.ConfigWindow_Param_AutoProvokeForTankDesc);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoUseTrueNorth,
                SettingsCommand.AutoUseTrueNorth);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerBySwift,
                SettingsCommand.RaisePlayerBySwift);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_AutoSpeedOutOfCombat,
                SettingsCommand.AutoSpeedOutOfCombat);

            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseGroundBeneficialAbility,
                SettingsCommand.UseGroundBeneficialAbility);

            ImGui.Unindent();
        }

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaisePlayerByCasting,
            ref Service.Config.RaisePlayerByCasting, Service.Default.RaisePlayerByCasting);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_UseHealWhenNotAHealer,
            ref Service.Config.UseHealWhenNotAHealer, Service.Default.UseHealWhenNotAHealer);

        DrawIntNumber(LocalizationManager.RightLang.ConfigWindow_Param_LessMPNoRaise,
            ref Service.Config.LessMPNoRaise, Service.Default.LessMPNoRaise, 200, 0, 2000000);
    }

    private void DrawParamCondition()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InterruptibleMoreCheck,
            ref Service.Config.InterruptibleMoreCheck, Service.Default.InterruptibleMoreCheck);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_EsunaAll,
            ref Service.Config.EsunaAll, Service.Default.EsunaAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_HealOutOfCombat,
            ref Service.Config.HealOutOfCombat, Service.Default.HealOutOfCombat);

        const float speed = 0.005f;

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealWhenNothingTodoBelow,
    ref Service.Config.HealWhenNothingTodoBelow, Service.Default.HealWhenNothingTodoBelow, speed);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_OnlyHotOnTanks,
            ref Service.Config.OnlyHotOnTanks, Service.Default.OnlyHotOnTanks);

        if (Service.Config.GetValue(SettingsCommand.UseGroundBeneficialAbility))
        {
            DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnTarget,
                ref Service.Config.BeneficialAreaOnTarget, Service.Default.BeneficialAreaOnTarget);
        }

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_DistanceForMoving,
            ref Service.Config.DistanceForMoving, Service.Default.DistanceForMoving, speed * 3);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_MeleeRangeOffset,
            ref Service.Config.MeleeRangeOffset, Service.Default.MeleeRangeOffset, 5 * speed, max: 5);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthDifference,
             ref Service.Config.HealthDifference, Service.Default.HealthDifference,
        speed * 2, 0, 0.5f);

        if (ImGui.BeginTable("Healing things", 3, ImGuiTableFlags.Borders
            | ImGuiTableFlags.Resizable
            | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader("");

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_Param_Normal);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_Param_HOT);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Param_HealthAreaAbility);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthAreaAbility),
                ref Service.Config.HealthAreaAbility, Service.Default.HealthAreaAbility, speed);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthAreaAbilityHot),
                ref Service.Config.HealthAreaAbilityHot, Service.Default.HealthAreaAbilityHot, speed);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Param_HealthAreaSpell);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthAreaSpell),
                ref Service.Config.HealthAreaSpell, Service.Default.HealthAreaSpell, speed);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthAreaSpellHot),
                ref Service.Config.HealthAreaSpellHot, Service.Default.HealthAreaSpellHot, speed);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Param_HealthSingleAbility);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthSingleAbility),
                ref Service.Config.HealthSingleAbility, Service.Default.HealthSingleAbility, speed);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthSingleAbilityHot),
                ref Service.Config.HealthSingleAbilityHot, Service.Default.HealthSingleAbilityHot, speed);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Param_HealthSingleSpell);

            ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthSingleSpell),
                ref Service.Config.HealthSingleSpell, Service.Default.HealthSingleSpell, speed);

           ImGui.TableNextColumn();

            DrawFloatNumber("##" + nameof(Service.Config.HealthSingleSpellHot),
                ref Service.Config.HealthSingleSpellHot, Service.Default.HealthSingleSpellHot, speed);

            ImGui.EndTable();
        }

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthHealerRatio,
            ref Service.Config.HealthHealerRatio, Service.Default.HealthHealerRatio, speed);

        DrawFloatNumber(LocalizationManager.RightLang.ConfigWindow_Param_HealthTankRatio,
            ref Service.Config.HealthTankRatio, Service.Default.HealthTankRatio, speed);
    }

    private void DrawParamTarget()
    {
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

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_ActionTargetFriendly,
            ref Service.Config.ActionTargetFriendly, Service.Default.ActionTargetFriendly);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseAll,
            ref Service.Config.RaiseAll, Service.Default.RaiseAll);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_RaiseBrinkOfDeath,
            ref Service.Config.RaiseBrinkOfDeath, Service.Default.RaiseBrinkOfDeath);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetFriendly,
            ref Service.Config.TargetFriendly, Service.Default.TargetFriendly);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetFatePriority,
            ref Service.Config.TargetFatePriority, Service.Default.TargetFatePriority);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetHuntingRelicLevePriority,
            ref Service.Config.TargetHuntingRelicLevePriority, Service.Default.TargetHuntingRelicLevePriority);

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_TargetQuestPriority,
            ref Service.Config.TargetQuestPriority, Service.Default.TargetQuestPriority);
    }
}
