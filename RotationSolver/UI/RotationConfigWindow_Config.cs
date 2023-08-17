using Dalamud.Game.ClientState.Keys;
using ECommons.ImGuiMethods;
using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI;

public partial class RotationConfigWindow
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
                    if (_searchResults.Contains(enumerator.Current)) continue;
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
        { () => "Others", () =>
        {
        foreach (var searchable in _basicParamsSearchable)
        {
            searchable?.Draw(Job);
        }
        } },
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
    };

    private static readonly ISearchable[] _basicParamsSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.ToggleManual),

        new CheckBoxSearchPlugin(PluginConfigBool.UseWorkTask),

        new DragFloatSearchPlugin(PluginConfigFloat.MistakeRatio, 0.002f),

        new CheckBoxSearchPlugin(PluginConfigBool.PreventActions, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.PreventActionsDuty),
        }),

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
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffAfterCombat,
            new DragFloatSearchPlugin(PluginConfigFloat.AutoOffAfterCombatTime, 1f)),
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

        new CheckBoxSearchPlugin(PluginConfigBool.ShowBeneficialPositions,
            new ColorEditSearchPlugin(PluginConfigVector4.BeneficialPositionColor),
            new ColorEditSearchPlugin(PluginConfigVector4.HoveredBeneficialPositionColor)
        ),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowTargetDeadTime),

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
        new CheckBoxSearchPlugin(PluginConfigBool.DrawIconAnimation),
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowControlWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.IsControlWindowLock),

            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowNextSizeRatio, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowGCDSize, 0.2f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindow0GCDSize, 0.2f),

            new ColorEditSearchPlugin(PluginConfigVector4.ControlWindowUnlockBg),
            new ColorEditSearchPlugin(PluginConfigVector4.ControlWindowLockBg),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowNextActionWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoMove),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoInputs),

            new ColorEditSearchPlugin(PluginConfigVector4.InfoWindowBg),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowCooldownWindow, new ISearchable[] 
        {
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

    #region Auto
    private static void DrawAuto()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Auto_Description);
        _autoHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _autoHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Auto_ActionUsage, () =>
            {
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Auto_ActionUsage_Description);
                ImGui.Separator();

                foreach (var searchable in _autoActionUsageSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => LocalizationManager.RightLang.ConfigWindow_Auto_ActionCondition, DrawAutoActionCondition },
    });

    private static void DrawAutoActionCondition()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Auto_ActionCondition_Description);
        ImGui.Separator();

        foreach (var searchable in _autoActionConditionSearchable_General)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _autoActionConditionSearchable_Heal)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _autoActionConditionSearchable_Raise)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _autoActionConditionSearchable_Others)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _autoActionConditionSearchable_General = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.AutoBurst),

        new DragIntSearchJob(JobConfigInt.AddDotGCDCount, 0.01f),
    };

    private static readonly ISearchable[] _autoActionConditionSearchable_Heal = new ISearchable[]
    {
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
            new DragFloatSearchJob(JobConfigFloat.HealthForDyingTanks, 0.02f)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                }
            },
            new DragFloatSearchPlugin(PluginConfigFloat.HealthDifference, 0.02f)),

        new CheckBoxSearchPlugin(PluginConfigBool.HealOutOfCombat),
        new DragFloatSearchPlugin(PluginConfigFloat.HealWhenNothingTodoBelow, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.HealthHealerRatio, 0.02f)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },
        new DragFloatSearchPlugin(PluginConfigFloat.HealthTankRatio, 0.02f)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },
        new DragFloatRangeSearchPlugin(PluginConfigFloat.HealDelayMin, PluginConfigFloat.HealDelayMin, 0.002f),
    };

    private static readonly ISearchable[] _autoActionConditionSearchable_Raise = new ISearchable[]
    {

        new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerByCasting)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseAll)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                },
                Jobs = new ECommons.ExcelServices.Job[]
                {
                    ECommons.ExcelServices.Job.RDM,
                },
            },
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseBrinkOfDeath)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                },
                Jobs = new ECommons.ExcelServices.Job[]
                {
                    ECommons.ExcelServices.Job.RDM,
                },
            },
        new DragFloatRangeSearchPlugin(PluginConfigFloat.DeathDelayMin, PluginConfigFloat.DeathDelayMax, 0.002f)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                },
                Jobs = new ECommons.ExcelServices.Job[]
                {
                    ECommons.ExcelServices.Job.RDM,
                },
            },
    };


    private static readonly ISearchable[] _autoActionConditionSearchable_Others = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.InterruptibleMoreCheck)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                    JobRole.Melee,
                    JobRole.RangedPhysical,
                }
            },

        new DragFloatRangeSearchPlugin(PluginConfigFloat.InterruptDelayMin, PluginConfigFloat.InterruptDelayMax, 0.002f)
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
        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeakenDelayMin, PluginConfigFloat.WeakenDelayMax, 0.002f)
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


        new DragFloatSearchPlugin(PluginConfigFloat.MeleeRangeOffset, 0.02f)
        {
            JobRoles = new JobRole[]
            {
                JobRole.Melee,
            },
        },
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

        new DragIntSearchPlugin(PluginConfigInt.LessMPNoRaise, 200)
        {
            JobRoles = new JobRole[]
            {
                JobRole.Healer,
            }
        },

        new CheckBoxSearchPlugin(PluginConfigBool.OnlyHotOnTanks)
        {
            JobRoles = new JobRole[]
            {
                JobRole.Healer,
            }
        },

        new CheckBoxSearchPlugin(PluginConfigBool.UseAbility, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.UseDefenseAbility),

            new CheckBoxSearchPlugin(PluginConfigBool.AutoTankStance)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                }
            },
            new CheckBoxSearchPlugin(PluginConfigBool.AutoProvokeForTank)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Tank,
                }
            },

            new CheckBoxSearchPlugin(PluginConfigBool.AutoUseTrueNorth)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Melee,
                }
            },
            new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerBySwift)
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },
            new CheckBoxSearchPlugin(PluginConfigBool.UseGroundBeneficialAbility,
            new DragIntSearchPlugin(PluginConfigInt.BeneficialAreaStrategy, () => new string[]{
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnLocations,
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnTarget,
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnCalculated }))
            {
                JobRoles = new JobRole[]
                {
                    JobRole.Healer,
                }
            },

            new CheckBoxSearchPlugin(PluginConfigBool.AutoSpeedOutOfCombat),
        }),
    };
    #endregion

    #region Target
    private static void DrawTarget()
    {
        _targetHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _targetHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Target_Config, DrawTargetConfig },
        { () => LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawTargetHostile },
    });

    private static void DrawTargetConfig()
    {
        foreach (var searchable in _targetHostileSearchable)
        {
            searchable?.Draw(Job);
        }
        ImGui.Separator();
        foreach (var searchable in _targetHostileSelectSearchable)
        {
            searchable?.Draw(Job);
        } 
        ImGui.Separator();
        foreach (var searchable in _targetMovingSearchable)
        {
            searchable?.Draw(Job);
        }
        ImGui.Separator();
        foreach (var searchable in _targetOtherSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _targetHostileSearchable = new ISearchable[]
    {
        new DragIntSearchJob(JobConfigInt.HostileType, () => new string []{
            LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType1,
            LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType2,
            LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType3 }),

        new CheckBoxSearchPlugin(PluginConfigBool.AddEnemyListToHostile),
        new CheckBoxSearchPlugin(PluginConfigBool.FilterStopMark),
        new CheckBoxSearchPlugin(PluginConfigBool.ChooseAttackMark, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.CanAttackMarkAOE),
        }),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.HostileDelayMin, PluginConfigFloat.HostileDelayMax, 0.002f),
    };

    private static readonly ISearchable[] _targetHostileSelectSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.DeadTimeBoss, 0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.DeadTimeDying, 0.02f),

        new CheckBoxSearchPlugin(PluginConfigBool.OnlyAttackInView),
        new CheckBoxSearchPlugin(PluginConfigBool.ChangeTargetForFate),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetFatePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetHuntingRelicLevePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetQuestPriority),
    };

    private static readonly ISearchable[] _targetMovingSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.MoveTowardsScreenCenter),
        new CheckBoxSearchPlugin(PluginConfigBool.MoveAreaActionFarthest),
        new DragIntSearchPlugin(PluginConfigInt.MoveTargetAngle, 0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.DistanceForMoving, 1f),
    };

    private static readonly ISearchable[] _targetOtherSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.TargetAllForFriendly),
        new CheckBoxSearchPlugin(PluginConfigBool.SwitchTargetFriendly),
    };


    private static void DrawTargetHostile()
    {
        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "Add Hostile"))
        {
            Service.Config.GlobalConfig.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);

        for (int i = 0; i < Service.Config.GlobalConfig.TargetingTypes.Count; i++)
        {
            var targetType = Service.Config.GlobalConfig.TargetingTypes[i];

            void Delete()
            {
                Service.Config.GlobalConfig.TargetingTypes.RemoveAt(i);
            };

            void Up()
            {
                Service.Config.GlobalConfig.TargetingTypes.RemoveAt(i);
                Service.Config.GlobalConfig.TargetingTypes.Insert(Math.Max(0, i - 1), targetType);
            };
            void Down()
            {
                Service.Config.GlobalConfig.TargetingTypes.RemoveAt(i);
                Service.Config.GlobalConfig.TargetingTypes.Insert(Math.Min(Service.Config.GlobalConfig.TargetingTypes.Count - 1, i + 1), targetType);
            }

            var key = $"Targeting Type Pop Up: {i}";

            Searchable.DrawHotKeysPopup(key, string.Empty,
                (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveUp, Up, new string[] { "↑" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveDown, Down, new string[] { "↓" }));

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.GlobalConfig.TargetingTypes[i];
            var text = LocalizationManager.RightLang.ConfigWindow_Param_HostileCondition;
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(text).X + 30 * _scale);
            if (ImGui.Combo(text + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.GlobalConfig.TargetingTypes[i] = (TargetingType)targingType;
            }

            Searchable.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, new VirtualKey[] { VirtualKey.DELETE }),
                (Up, new VirtualKey[] { VirtualKey.UP }),
                (Down, new VirtualKey[] { VirtualKey.DOWN }));
        }
    }
    #endregion

    #region Extra
    private static void DrawExtra()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Extra_Description);
        _extraHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _extraHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_EventItem, DrawEventTab },

        { () => LocalizationManager.RightLang.ConfigWindow_Extra_Others, () =>
            {
                foreach (var searchable in _extraSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
    });

    private static readonly ISearchable[] _extraSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.PoslockCasting,
        new DragIntSearchPlugin( PluginConfigInt.PoslockModifier, () => new string[]{ "SHIFT", "CTRL", "ALT", "Left Mouse", "Middle Mouse",  "Right Mouse" }),
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

        new CheckBoxSearchPlugin(PluginConfigBool.UseStopCasting,new ISearchable[] 
        {
            new DragFloatRangeSearchPlugin(PluginConfigFloat.StopCastingDelayMin, PluginConfigFloat.StopCastingDelayMin, 0.002f) 
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.AutoOpenChest, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.AutoCloseChestWindow),
        }),
    };
    private static void DrawEventTab()
    {
        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Events_AddEvent))
        {
            Service.Config.GlobalConfig.Events.Add(new ActionEventInfo());
        }
        ImGui.SameLine();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Events_Description);

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyStart);
        ImGui.SameLine();
        Service.Config.GlobalConfig.DutyStart.DisplayMacro();

        ImGui.Text(LocalizationManager.RightLang.ConfigWindow_Events_DutyEnd);
        ImGui.SameLine();
        Service.Config.GlobalConfig.DutyEnd.DisplayMacro();

        ImGui.Separator();

        ActionEventInfo remove = null;
        foreach (var eve in Service.Config.GlobalConfig.Events)
        {
            eve.DisplayEvent();

            ImGui.SameLine();

            if (ImGui.Button($"{LocalizationManager.RightLang.ConfigWindow_Events_RemoveEvent}##RemoveEvent{eve.GetHashCode()}"))
            {
                remove = eve;
            }
            ImGui.Separator();
        }
        if (remove != null)
        {
            Service.Config.GlobalConfig.Events.Remove(remove);
        }
    }
    #endregion
}
