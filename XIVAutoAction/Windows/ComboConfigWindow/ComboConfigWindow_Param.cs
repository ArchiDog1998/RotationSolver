using Dalamud.Game.ClientState.Keys;
using ImGuiNET;
using System;
using System.Numerics;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Localization;

namespace AutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawParam()
    {
        ImGui.Text(LocalizationManager.RightLang.Configwindow_Params_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginChild("Params", new Vector2(0f, -1f), true))
        {
            bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
            if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_NeverReplaceIcon, ref neverReplaceIcon))
            {
                Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_NeverReplaceIconDesc);
            }

            bool useOverlayWindow = Service.Configuration.UseOverlayWindow;
            if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseOverlayWindow, ref useOverlayWindow))
            {
                Service.Configuration.UseOverlayWindow = useOverlayWindow;
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_UseOverlayWindowDesc);
            }

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_BasicSettings))
            {

                if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_RestoreDefault))
                {
                    Service.Configuration.WeaponInterval = 0.67f;
                    Service.Configuration.SpecialDuration = 3;
                    Service.Configuration.InterruptibleTime = 0.5f;
                    Service.Configuration.AddDotGCDCount = 2;
                    Service.Configuration.Save();
                }

                float weaponInterval = Service.Configuration.WeaponInterval;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_WeaponInterval, ref weaponInterval, 0.002f, 0.5f, 0.7f))
                {
                    Service.Configuration.WeaponInterval = weaponInterval;
                    Service.Configuration.Save();
                }

                float interruptibleTime = Service.Configuration.InterruptibleTime;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_InterruptibleTime, ref interruptibleTime, 0.002f, 0, 2))
                {
                    Service.Configuration.InterruptibleTime = interruptibleTime;
                    Service.Configuration.Save();
                }

                float specialDuration = Service.Configuration.SpecialDuration;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_SpecialDuration, ref specialDuration, 0.02f, 1, 20))
                {
                    Service.Configuration.SpecialDuration = specialDuration;
                    Service.Configuration.Save();
                }

                int addDotGCDCount = Service.Configuration.AddDotGCDCount;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragInt(LocalizationManager.RightLang.Configwindow_Params_AddDotGCDCount, ref addDotGCDCount, 0.1f, 1, 3))
                {
                    Service.Configuration.AddDotGCDCount = addDotGCDCount;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_DisplayEnhancement))
            {
                bool teachingMode = Service.Configuration.TeachingMode;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_TeachingMode, ref teachingMode))
                {
                    Service.Configuration.TeachingMode = teachingMode;
                    Service.Configuration.Save();
                }
                if (teachingMode)
                {
                    Spacing();

                    var teachingColor = Service.Configuration.TeachingModeColor;
                    ImGui.SetNextItemWidth(210);

                    if (ImGui.ColorEdit3(LocalizationManager.RightLang.Configwindow_Params_TeachingModeColor, ref teachingColor))
                    {
                        Service.Configuration.TeachingModeColor = teachingColor;
                        Service.Configuration.Save();
                    }
                }

                bool showMoveTarget = Service.Configuration.ShowMoveTarget;
                if (useOverlayWindow && ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ShowMoveTarget, ref showMoveTarget))
                {
                    Service.Configuration.ShowMoveTarget = showMoveTarget;
                    Service.Configuration.Save();
                }

                if (showMoveTarget)
                {
                    Spacing();

                    var moveTargetColor = Service.Configuration.MovingTargetColor;
                    ImGui.SetNextItemWidth(210);

                    if (ImGui.ColorEdit3(LocalizationManager.RightLang.Configwindow_Params_MovingTargetColor, ref moveTargetColor))
                    {
                        Service.Configuration.MovingTargetColor = moveTargetColor;
                        Service.Configuration.Save();
                    }
                }

                bool showTarget = Service.Configuration.ShowTarget;
                if (useOverlayWindow && ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ShowTarget, ref showTarget))
                {
                    Service.Configuration.ShowTarget = showTarget;
                    Service.Configuration.Save();
                }
                if (showTarget)
                {
                    Spacing();
                    var targetColor = Service.Configuration.TargetColor;
                    ImGui.SetNextItemWidth(210);

                    if (ImGui.ColorEdit3(LocalizationManager.RightLang.Configwindow_Params_TargetColor, ref targetColor))
                    {
                        Service.Configuration.TargetColor = targetColor;
                        Service.Configuration.Save();
                    }

                    Spacing();
                    var subTargetColor = Service.Configuration.SubTargetColor;
                    ImGui.SetNextItemWidth(210);

                    if (ImGui.ColorEdit3(LocalizationManager.RightLang.Configwindow_Params_SubTargetColor, ref subTargetColor))
                    {
                        Service.Configuration.SubTargetColor = subTargetColor;
                        Service.Configuration.Save();
                    }
                }

                bool keyBoardNoise = Service.Configuration.KeyBoardNoise;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_KeyBoardNoise, ref keyBoardNoise))
                {
                    Service.Configuration.KeyBoardNoise = keyBoardNoise;
                    Service.Configuration.Save();
                }

                ImGui.Separator();

                bool usecheckCasting = Service.Configuration.CheckForCasting;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_CheckForCasting, ref usecheckCasting))
                {
                    Service.Configuration.CheckForCasting = usecheckCasting;
                    Service.Configuration.Save();
                }

                bool poslockCasting = Service.Configuration.PoslockCasting;
                VirtualKey poslockModifier = Service.Configuration.PoslockModifier;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_PoslockCasting, ref poslockCasting))
                {
                    Service.Configuration.PoslockCasting = poslockCasting;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                Spacing();
                ImGui.SetNextItemWidth(100);
                var modifierChoices = new VirtualKey[] { VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };
                if (poslockCasting && ImGui.BeginCombo(LocalizationManager.RightLang.Configwindow_Params_PoslockModifier, poslockModifier.ToName()))
                {
                    foreach (VirtualKey k in modifierChoices)
                    {
                        if (ImGui.Selectable(k.ToName()))
                        {
                            Service.Configuration.PoslockModifier = k;
                            Service.Configuration.Save();
                        }
                    }
                    ImGui.EndCombo();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_PoslockDescription);
                }

                ImGui.Separator();

                bool autoSayingOut = Service.Configuration.AutoSayingOut;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoSayingOut, ref autoSayingOut))
                {
                    Service.Configuration.AutoSayingOut = autoSayingOut;
                    Service.Configuration.Save();
                }

                bool useDtr = Service.Configuration.UseDtr;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseDtr, ref useDtr))
                {
                    Service.Configuration.UseDtr = useDtr;
                    Service.Configuration.Save();
                }

                bool useToast = Service.Configuration.UseToast;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseToast, ref useToast))
                {
                    Service.Configuration.UseToast = useToast;
                    Service.Configuration.Save();
                }

                ImGui.Spacing();

                bool showlocation = Service.Configuration.ShowLocation;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ShowLocation, ref showlocation))
                {
                    Service.Configuration.ShowLocation = showlocation;
                    Service.Configuration.Save();
                }

                bool sayingLocation = Service.Configuration.SayingLocation;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_SayingLocation, ref sayingLocation))
                {
                    Service.Configuration.SayingLocation = sayingLocation;
                    Service.Configuration.Save();
                }

                bool showLocationWrong = Service.Configuration.ShowLocationWrong;
                if (useOverlayWindow && ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ShowLocationWrong, ref showLocationWrong))
                {
                    Service.Configuration.ShowLocationWrong = showLocationWrong;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_ShowLocationWrongDesc);
                }

                int voiceVolume = Service.Configuration.VoiceVolume;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragInt(LocalizationManager.RightLang.Configwindow_Params_VoiceVolume, ref voiceVolume, 0.2f, 0, 100))
                {
                    Service.Configuration.VoiceVolume = voiceVolume;
                    Service.Configuration.Save();
                }

                var locationWrongText = Service.Configuration.LocationWrongText;
                if (ImGui.InputText(LocalizationManager.RightLang.Configwindow_Params_LocationWrongText, ref locationWrongText, 15))
                {
                    Service.Configuration.LocationWrongText = locationWrongText;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_LocationWrongTextDesc);
                }

            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_Actions))
            {
                bool useAOEWhenManual = Service.Configuration.UseAOEWhenManual;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseAOEWhenManual, ref useAOEWhenManual))
                {
                    Service.Configuration.UseAOEWhenManual = useAOEWhenManual;
                    Service.Configuration.Save();
                }

                bool attackSafeMode = Service.Configuration.AttackSafeMode;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AttackSafeMode, ref attackSafeMode))
                {
                    Service.Configuration.AttackSafeMode = attackSafeMode;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_AttackSafeModeDesc);
                }

                ImGui.Separator();

                bool autoBreak = Service.Configuration.AutoBreak;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoBreak, ref autoBreak))
                {
                    Service.Configuration.AutoBreak = autoBreak;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                CommandHelp("AutoBreak");

                bool useItem = Service.Configuration.UseItem;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseItem, ref useItem))
                {
                    Service.Configuration.UseItem = useItem;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_UseItemDesc);
                }

                bool useAbility = Service.Configuration.UseAbility;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseAbility, ref useAbility))
                {
                    Service.Configuration.UseAbility = useAbility;
                    Service.Configuration.Save();
                }

                if (useAbility)
                {
                    ImGui.Indent();
                    bool useDefenceAbility = Service.Configuration.UseDefenceAbility;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseDefenceAbility, ref useDefenceAbility))
                    {
                        Service.Configuration.UseDefenceAbility = useDefenceAbility;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_UseDefenceAbilityDesc);
                    }

                    bool autoShieled = Service.Configuration.AutoShield;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoShield, ref autoShieled))
                    {
                        Service.Configuration.AutoShield = autoShieled;
                        Service.Configuration.Save();
                    }

                    bool autoProvokeforTank = Service.Configuration.AutoProvokeForTank;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoProvokeForTank, ref autoProvokeforTank))
                    {
                        Service.Configuration.AutoProvokeForTank = autoProvokeforTank;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_AutoProvokeForTankDesc);
                    }

                    bool autoUseTrueNorth = Service.Configuration.AutoUseTrueNorth;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoUseTrueNorth, ref autoUseTrueNorth))
                    {
                        Service.Configuration.AutoUseTrueNorth = autoUseTrueNorth;
                        Service.Configuration.Save();
                    }

                    bool raiseSwift = Service.Configuration.RaisePlayerBySwift;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_RaisePlayerBySwift, ref raiseSwift))
                    {
                        Service.Configuration.RaisePlayerBySwift = raiseSwift;
                        Service.Configuration.Save();
                    }

                    bool useAreaAbilityFriendly = Service.Configuration.UseAreaAbilityFriendly;
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseAreaAbilityFriendly, ref useAreaAbilityFriendly))
                    {
                        Service.Configuration.UseAreaAbilityFriendly = useAreaAbilityFriendly;
                        Service.Configuration.Save();
                    }
                    ImGui.Unindent();
                }

                bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_RaisePlayerByCasting, ref raiseCasting))
                {
                    Service.Configuration.RaisePlayerByCasting = raiseCasting;
                    Service.Configuration.Save();
                }

                bool useHealWhenNotAHealer = Service.Configuration.UseHealWhenNotAHealer;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_UseHealWhenNotAHealer, ref useHealWhenNotAHealer))
                {
                    Service.Configuration.UseHealWhenNotAHealer = useHealWhenNotAHealer;
                    Service.Configuration.Save();
                }

                int lessMPNoRaise = Service.Configuration.LessMPNoRaise;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragInt(LocalizationManager.RightLang.Configwindow_Params_LessMPNoRaise, ref lessMPNoRaise, 200, 0, 10000))
                {
                    Service.Configuration.LessMPNoRaise = lessMPNoRaise;
                    Service.Configuration.Save();
                }

            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_Conditons))
            {
                if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_RestoreDefault))
                {
                    Service.Configuration.HealthDifference = 0.25f;
                    Service.Configuration.HealthAreaAbility = 0.75f;
                    Service.Configuration.HealthAreafSpell = 0.65f;
                    Service.Configuration.HealthSingleAbility = 0.7f;
                    Service.Configuration.HealthSingleSpell = 0.55f;
                    Service.Configuration.Save();
                }

                bool autoStartCountdown = Service.Configuration.AutoStartCountdown;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AutoStartCountdown, ref autoStartCountdown))
                {
                    Service.Configuration.AutoStartCountdown = autoStartCountdown;
                    Service.Configuration.Save();
                }

                const float speed = 0.005f;
                float healthDiff = Service.Configuration.HealthDifference;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthDifference, ref healthDiff, speed * 2, 0, 0.5f))
                {
                    Service.Configuration.HealthDifference = healthDiff;
                    Service.Configuration.Save();
                }

                float healthAreaA = Service.Configuration.HealthAreaAbility;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthAreaAbility, ref healthAreaA, speed, 0, 1))
                {
                    Service.Configuration.HealthAreaAbility = healthAreaA;
                    Service.Configuration.Save();
                }

                float healthAreaS = Service.Configuration.HealthAreafSpell;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthAreafSpell, ref healthAreaS, speed, 0, 1))
                {
                    Service.Configuration.HealthAreafSpell = healthAreaS;
                    Service.Configuration.Save();
                }

                float healthSingleA = Service.Configuration.HealthSingleAbility;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthSingleAbility, ref healthSingleA, speed, 0, 1))
                {
                    Service.Configuration.HealthSingleAbility = healthSingleA;
                    Service.Configuration.Save();
                }

                float healthSingleS = Service.Configuration.HealthSingleSpell;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_HealthSingleSpell, ref healthSingleS, speed, 0, 1))
                {
                    Service.Configuration.HealthSingleSpell = healthSingleS;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_Targets))
            {
                float minradius = Service.Configuration.ObjectMinRadius;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragFloat(LocalizationManager.RightLang.Configwindow_Params_ObjectMinRadius, ref minradius, 0.02f, 0, 10))
                {
                    Service.Configuration.ObjectMinRadius = minradius;
                    Service.Configuration.Save();
                }



                bool addEnemyListToHostile = Service.Configuration.AddEnemyListToHostile;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AddEnemyListToHostile, ref addEnemyListToHostile))
                {
                    Service.Configuration.AddEnemyListToHostile = addEnemyListToHostile;
                    Service.Configuration.Save();
                }

                bool chooseAttackMark = Service.Configuration.ChooseAttackMark;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ChooseAttackMark, ref chooseAttackMark))
                {
                    Service.Configuration.ChooseAttackMark = chooseAttackMark;
                    Service.Configuration.Save();
                }

                if (chooseAttackMark)
                {

                    bool attackMarkAOE = Service.Configuration.AttackMarkAOE;
                    ImGui.Indent();
                    if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_AttackMarkAOE, ref attackMarkAOE))
                    {
                        Service.Configuration.AttackMarkAOE = attackMarkAOE;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_AttackMarkAOEDesc);
                    }
                    ImGui.Unindent();
                }

                bool filterStopMark = Service.Configuration.FilterStopMark;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_FilterStopMark, ref filterStopMark))
                {
                    Service.Configuration.FilterStopMark = filterStopMark;
                    Service.Configuration.Save();
                }

                bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_ChangeTargetForFate, ref changeTargetForFate))
                {
                    Service.Configuration.ChangeTargetForFate = changeTargetForFate;
                    Service.Configuration.Save();
                }

                bool moveToScreen = Service.Configuration.MoveTowardsScreen;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_MoveTowardsScreen, ref moveToScreen))
                {
                    Service.Configuration.MoveTowardsScreen = moveToScreen;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_MoveTowardsScreenDesc);
                }

                int moveTargetAngle = Service.Configuration.MoveTargetAngle;
                ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
                if (ImGui.DragInt(LocalizationManager.RightLang.Configwindow_Params_MoveTargetAngle, ref moveTargetAngle, 0.02f, 0, 10))
                {
                    Service.Configuration.MoveTargetAngle = moveTargetAngle;
                    Service.Configuration.Save();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(LocalizationManager.RightLang.Configwindow_Params_MoveTargetAngleDesc);
                }

                bool raiseAll = Service.Configuration.RaiseAll;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_RaiseAll, ref raiseAll))
                {
                    Service.Configuration.RaiseAll = raiseAll;
                    Service.Configuration.Save();
                }

                bool raiseBrinkofDeath = Service.Configuration.RaiseBrinkofDeath;
                if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Params_RaiseBrinkofDeath, ref raiseBrinkofDeath))
                {
                    Service.Configuration.RaiseBrinkofDeath = raiseBrinkofDeath;
                    Service.Configuration.Save();
                }
            }

            ImGui.Separator();

            if (ImGui.CollapsingHeader(LocalizationManager.RightLang.Configwindow_Params_Hostile))
            {
                if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_AddHostileCondition))
                {
                    Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                }
                ImGui.SameLine();
                Spacing();
                ImGui.Text(LocalizationManager.RightLang.Configwindow_Params_HostileDesc);
                for (int i = 0; i < Service.Configuration.TargetingTypes.Count; i++)
                {
                    ImGui.Separator();

                    var names = Enum.GetNames(typeof(TargetingType));
                    var targingType = (int)Service.Configuration.TargetingTypes[i];
                    if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Params_HostileCondition + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
                    {
                        Service.Configuration.TargetingTypes[i] = (TargetingType)targingType;
                        Service.Configuration.Save();
                    }

                    if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_ConditionUp + "##HostileUp" + i.ToString()))
                    {
                        if (i != 0)
                        {
                            var value = Service.Configuration.TargetingTypes[i];
                            Service.Configuration.TargetingTypes.RemoveAt(i);
                            Service.Configuration.TargetingTypes.Insert(i - 1, value);
                        }
                    }
                    ImGui.SameLine();
                    Spacing();
                    if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_ConditionDown + "##HostileDown" + i.ToString()))
                    {
                        if (i < Service.Configuration.TargetingTypes.Count - 1)
                        {
                            var value = Service.Configuration.TargetingTypes[i];
                            Service.Configuration.TargetingTypes.RemoveAt(i);
                            Service.Configuration.TargetingTypes.Insert(i + 1, value);
                        }
                    }

                    ImGui.SameLine();
                    Spacing();

                    if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Params_ConditionDelete + "##HostileDelete" + i.ToString()))
                    {
                        Service.Configuration.TargetingTypes.RemoveAt(i);
                    }
                }
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}
