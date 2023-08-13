using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Data;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI;

public partial class RotationConfigWindowNew
{
    private static readonly Levenshtein StringComparer = new ();

    private string _searchText = string.Empty;
    private ISearchable[] _searchResults = Array.Empty<ISearchable>();
    private void SearchingBox()
    {
        if (ImGui.InputTextWithHint("##Rotation Solver Search Box", "Searching...", ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                const int MAX_RESULT_LENGTH = 20;

                _searchResults = new ISearchable[MAX_RESULT_LENGTH];

                var enumerator = GetType().GetRuntimeFields()
                    .Where(f => f.FieldType == typeof(ISearchable[]) && f.IsInitOnly)
                    .SelectMany(f => (ISearchable[])f.GetValue(this))
                    .SelectMany(GetChildren)
                    .OrderBy(i => i.SearchingKeys.Split(' ').Min(k => StringComparer.Distance(k, _searchText)))
                    .Select(GetParent).GetEnumerator();

                int index = 0;
                while (enumerator.MoveNext() && index < MAX_RESULT_LENGTH)
                {
                    _searchResults[index++] = enumerator.Current;
                }
            }
            else
            {
                _searchResults = Array.Empty<ISearchable>();
            }
        }
    }

    private static IEnumerable<ISearchable> GetChildren(ISearchable searchable)
    {
        var myself = new ISearchable[] { searchable };
        if (searchable is CheckBoxSearch c && c.Children != null)
        {
            return c.Children.SelectMany(GetChildren).Union(myself);
        } 
        else return myself;
    }

    private static ISearchable GetParent(ISearchable searchable)
    {
        if (searchable == null) return null;
        if (searchable.Parent == null) return searchable;
        return GetParent(searchable.Parent);
    }

    #region Basic
    private static void DrawBasic()
    {
        _baseHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _baseHeader = new(new()
    {
        { () =>  LocalizationManager.RightLang.ConfigWindow_Basic_Timer, DrawBasicTimer },
        { () => LocalizationManager.RightLang.ConfigWindow_Basic_AutoSwitch, DrawBasicAutoSwitch },
    });


    private static void DrawBasicTimer()
    {
        foreach (var searchable in _basicTimer)
        {
            searchable?.Draw(Job);
        }
    }

    private static void DrawBasicAutoSwitch()
    {
        foreach (var searchable in _basicSwitchTurnOn)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _basicSwitchTurnOff)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _basicTimer = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MinLastAbilityAdvanced, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.SpecialDuration, 1f),
        new DragFloatSearchPlugin(PluginConfigFloat.CountDownAhead, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MaxPing, 0.002f),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeaponDelayMin, PluginConfigFloat.WeaponDelayMax, 0.002f),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.ClickingDelayMin, PluginConfigFloat.ClickingDelayMax, 0.002f),

        new CheckBoxSearchPlugin(PluginConfigBool.PreventActions, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.PreventActionsDuty),
        }),
    };

    private static readonly ISearchable[] _basicParamsSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.ToggleManual),

        new CheckBoxSearchPlugin(PluginConfigBool.UseWorkTask),
        new DragIntSearchJob(JobConfigInt.AddDotGCDCount, 1),
        new DragFloatSearchPlugin(PluginConfigFloat.MistakeRatio, 0.002f),


        new DragFloatRangeSearchPlugin(PluginConfigFloat.HostileDelayMin, PluginConfigFloat.HostileDelayMax, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.InterruptDelayMin, PluginConfigFloat.InterruptDelayMax, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.DeathDelayMin, PluginConfigFloat.DeathDelayMax, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeakenDelayMin, PluginConfigFloat.WeakenDelayMax, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.HealDelayMin, PluginConfigFloat.HealDelayMin, 0.002f),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.NotInCombatDelayMin, PluginConfigFloat.NotInCombatDelayMax, 0.002f),

    };



    private static readonly ISearchable[] _basicSwitchTurnOn = new ISearchable[]
    {
        // Turn on
        new CheckBoxSearchPlugin(PluginConfigBool.StartOnCountdown, 
            new DragFloatRangeSearchPlugin(PluginConfigFloat.CountdownDelayMin, PluginConfigFloat.CountdownDelayMax, 0.002f)),
        new CheckBoxSearchPlugin(PluginConfigBool.StartOnAttackedBySomeone),
    };

    private static readonly ISearchable[] _basicSwitchTurnOff = new ISearchable[]
    {
        // Turn off
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffBetweenArea),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffCutScene),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffWhenDead),
        new DragFloatSearchPlugin(PluginConfigFloat.AutoOffAfterCombat, 1f),
    };
    #endregion

    #region UI
    private static void DrawUI()
    {
        _UIHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _UIHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_UI_Information, () =>
            {
                foreach (var searchable in _uiInformationSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => LocalizationManager.RightLang.ConfigWindow_UI_Overlay, () =>
            {
                foreach (var searchable in _uiOverlaySearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => LocalizationManager.RightLang.ConfigWindow_UI_Windows, () =>
            {
                foreach (var searchable in _uiWindowsSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
    });

    // information
    private static readonly ISearchable[] _uiInformationSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.ShowInfoOnDtr),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowInfoOnToast),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowToastsAboutDoAction),

        new CheckBoxSearchPlugin(PluginConfigBool.KeyBoardNoise,
            new DragIntRangeSearchPlugin(PluginConfigInt.KeyBoardNoiseMin, PluginConfigInt.KeyBoardNoiseMax, 1)),

        new CheckBoxSearchPlugin(PluginConfigBool.SayOutStateChanged),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowTooltips),
    };

    // Overlay
    private static readonly ISearchable[] _uiOverlaySearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.UseOverlayWindow,
            new DragFloatSearchPlugin(PluginConfigFloat.DrawingHeight, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.SampleLength, 0.001f),

        new CheckBoxSearchPlugin(PluginConfigBool.TeachingMode, 
            new ColorEditSearchPlugin(PluginConfigVector4.TeachingModeColor)
        ),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowMoveTarget, 
            new ColorEditSearchPlugin(PluginConfigVector4.MovingTargetColor)
        ),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowTarget, 
            new DragFloatSearchPlugin(PluginConfigFloat.TargetIconSize, 0.002f),
            new ColorEditSearchPlugin(PluginConfigVector4.TargetColor),
            new ColorEditSearchPlugin(PluginConfigVector4.SubTargetColor)
        ),
        new CheckBoxSearchPlugin(PluginConfigBool.DrawMeleeOffset)),
    };

    // Windows
    private static readonly ISearchable[] _uiWindowsSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowControlWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),
            new CheckBoxSearchPlugin(PluginConfigBool.IsControlWindowLock),

            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowNextSizeRatio, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowGCDSize, 0.2f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindow0GCDSize, 0.2f),

            new ColorEditSearchPlugin(PluginConfigVector4.ControlWindowUnlockBg),
            new ColorEditSearchPlugin(PluginConfigVector4.ControlWindowLockBg),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowNextActionWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoMove),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoInputs),

            new ColorEditSearchPlugin(PluginConfigVector4.InfoWindowBg),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowCooldownWindow, new ISearchable[] 
        {
            new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoMove),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoInputs),
            new CheckBoxSearchPlugin(PluginConfigBool.UseOriginalCooldown),
            new CheckBoxSearchPlugin(PluginConfigBool.ShowGCDCooldown),
            new CheckBoxSearchPlugin(PluginConfigBool.ShowItemsCooldown),

            new DragIntSearchPlugin(PluginConfigInt.CooldownActionOneLine, 1),
            new DragFloatSearchPlugin(PluginConfigFloat.CooldownFontSize, 0.1f),
            new DragFloatSearchPlugin(PluginConfigFloat.CooldownWindowIconSize, 0.2f),

            new ColorEditSearchPlugin(PluginConfigVector4.InfoWindowBg),
        }),
    };
    #endregion

    private static readonly ISearchable[] _autoSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.AutoBurst),
        new AutoHealCheckBox(
            new CheckBoxSearchPlugin(PluginConfigBool.UseHealWhenNotAHealer)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                    JobRole.Melee,
                    JobRole.RangedMagical,
                    JobRole.RangedPhysical,
                }
            },
            new DragFloatSearchPlugin(PluginConfigFloat.HealthDifference, 0.02f)),

        new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerByCasting)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },
        new DragIntSearchPlugin(PluginConfigInt.LessMPNoRaise, 200)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            }, 
        new CheckBoxSearchPlugin(PluginConfigBool.AutoProvokeForTank)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                }
            },

        new CheckBoxSearchPlugin(PluginConfigBool.InterruptibleMoreCheck)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                    JobRole.Melee,
                    JobRole.RangedPhysical,
                }
            },
        new CheckBoxSearchPlugin(PluginConfigBool.EsunaAll)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                },
                Jobs = new ECommons.ExcelServices.Job[]
                {
                    ECommons.ExcelServices.Job.BRD,
                },
            },
        new CheckBoxSearchPlugin(PluginConfigBool.HealOutOfCombat),
        new DragFloatSearchPlugin(PluginConfigFloat.HealWhenNothingTodoBelow, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.DistanceForMoving, 1f),
        new DragFloatSearchPlugin(PluginConfigFloat.MeleeRangeOffset, 0.02f),
        new CheckBoxSearchPlugin(PluginConfigBool.UseDefenseAbility),

        new DragFloatSearchPlugin(PluginConfigFloat.HealthHealerRatio, 0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.HealthTankRatio, 0.02f),
    };

    private static readonly ISearchable[] _autoActionUsageSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.UseAOEAction, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.UseAOEWhenManual),
            new CheckBoxSearchPlugin(PluginConfigBool.NoNewHostiles),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.UseTinctures),
        new CheckBoxSearchPlugin(PluginConfigBool.UseHealPotions),
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyHotOnTanks),
        new CheckBoxSearchPlugin(PluginConfigBool.UseAbility, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.AutoTankStance),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoUseTrueNorth),
            new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerBySwift),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoSpeedOutOfCombat),
            new CheckBoxSearchPlugin(PluginConfigBool.UseGroundBeneficialAbility,
                new CheckBoxSearchPlugin(PluginConfigBool.BeneficialAreaOnTarget)),
        }),
    };

    private static readonly CollapsingHeaderGroup _autoHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Auto_ActionUsage, () =>
            {
                ImGui.Text("Which actions RS can use");
                foreach (var searchable in _autoActionUsageSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => "技能", () =>
            {
                foreach (var searchable in _autoSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => "目标", () =>
            {
                foreach (var searchable in _autoSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
    });
    private static void DrawAuto()
    {
        _autoHeader?.Draw();
    }

    private static readonly ISearchable[] _targetSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.AddEnemyListToHostile),
        new CheckBoxSearchPlugin(PluginConfigBool.ChooseAttackMark, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.CanAttackMarkAOE),

        }),
        new CheckBoxSearchPlugin(PluginConfigBool.FilterStopMark),
        new CheckBoxSearchPlugin(PluginConfigBool.ChangeTargetForFate),
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyAttackInView),
        new CheckBoxSearchPlugin(PluginConfigBool.MoveTowardsScreenCenter),
        new DragIntSearchPlugin(PluginConfigInt.MoveTargetAngle, 0.02f),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetAllForFriendly),
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseAll),
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseBrinkOfDeath),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetFriendly),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetFatePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetHuntingRelicLevePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetQuestPriority),

    };
    private static void DrawTarget()
    {
        foreach (var searchable in _targetSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _extraSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.PoslockCasting,
        new DragIntSearchPlugin( PluginConfigInt.PoslockModifier, "SHIFT", "CTRL", "ALT", "Left Mouse", "Middle Mouse",  "Right Mouse"),
        new CheckBoxSearchPlugin(PluginConfigBool.PosPassageOfArms)
        {
            Action = ActionID.PassageOfArms
        },
        new CheckBoxSearchPlugin(PluginConfigBool.PosTenChiJin)
        {
            Action = ActionID.TenChiJin
        },
        new CheckBoxSearchPlugin(PluginConfigBool.PosFlameThrower)
        {
            Action = ActionID.FlameThrower
        },
        new CheckBoxSearchPlugin(PluginConfigBool.PosImprovisation)
        {
            Action = ActionID.Improvisation
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowHealthRatio, new ISearchable[]
        {
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioBoss, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioDying, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioDot, 0.02f),

        }),

        new CheckBoxSearchPlugin(PluginConfigBool.UseStopCasting,new ISearchable[] 
        {
            new DragFloatRangeSearchPlugin(PluginConfigFloat.StopCastingDelayMin, PluginConfigFloat.StopCastingDelayMin, 0.002f) 
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.AutoOpenChest, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.AutoCloseChestWindow),
        }),
    };

    private static readonly CollapsingHeaderGroup _extraHeader = new(new()
    {
        { () => "Extra", () =>
            {
                foreach (var searchable in _extraSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },

        { () => LocalizationManager.RightLang.ConfigWindow_EventItem, DrawEventTab },
    });

    private static void DrawEventTab()
    {
        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Events_AddEvent))
        {
            Service.Config.Events.Add(new ActionEventInfo());
            Service.Config.Save();
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Events_Description);

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyStart);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyStart.DisplayMacro();

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyEnd);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyEnd.DisplayMacro();

        ImGui.Separator();

        ActionEventInfo remove = null;
        foreach (var eve in Service.Config.Events)
        {
            eve.DisplayEvent();

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button($"{LocalizationManager.RightLang.ConfigWindow_Events_RemoveEvent}##RemoveEvent{eve.GetHashCode()}"))
            {
                remove = eve;
            }
            ImGui.Separator();
        }
        if (remove != null)
        {
            Service.Config.Events.Remove(remove);
            Service.Config.Save();
        }
    }

    private static void DrawExtra()
    {
        ImGui.TextWrapped("Side features, should be removed");
        _extraHeader?.Draw();
    }
}
