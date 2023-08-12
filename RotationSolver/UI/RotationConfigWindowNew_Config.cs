using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI;

public partial class RotationConfigWindowNew
{
    private string _searchText = string.Empty;
    private ISearchable[] _searchResults = Array.Empty<ISearchable>();
    private void SearchingBox()
    {
        if (ImGui.InputTextWithHint("##Rotation Solver Search Box", "Searching is not available", ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                const int MAX_RESULT_LENGTH = 20;

                _searchResults = new ISearchable[MAX_RESULT_LENGTH];
                var l = new Levenshtein();

                var enumerator = GetType().GetRuntimeFields()
                    .Where(f => f.FieldType == typeof(ISearchable[]) && f.IsInitOnly)
                    .SelectMany(f => (ISearchable[])f.GetValue(this))
                    .OrderBy(i => l.Distance(i.SearchingKey, _searchText))
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

    private static ISearchable GetParent(ISearchable searchable)
    {
        if (searchable == null) return null;
        if (searchable.Parent == null) return searchable;
        return GetParent(searchable.Parent);
    }

    // 基础参数,不常更改的一些参数，基本设置好都不怎么会在改动的设置
    private static readonly ISearchable[] _basicParamsSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.UseWorkTask),
        new DragIntSearchJob(JobConfigInt.AddDotGCDCount,0,3,1),
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MinLastAbilityAdvanced, 0, 0.4f, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.CountDownAhead, 0.5f, 0.7f, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.SpecialDuration, 1.0f, 20.0f, 1f),
        new DragFloatSearchPlugin(PluginConfigFloat.MaxPing, 0.01f, 0.5f, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MistakeRatio, 0, 1, 0.002f),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeaponDelayMin, PluginConfigFloat.WeaponDelayMax, 0, 1f, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.HostileDelayMin, PluginConfigFloat.HostileDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.InterruptDelayMin, PluginConfigFloat.InterruptDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.DeathDelayMin, PluginConfigFloat.DeathDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeakenDelayMin, PluginConfigFloat.WeakenDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.HealDelayMin, PluginConfigFloat.HealDelayMin, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.CountdownDelayMin, PluginConfigFloat.CountdownDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.NotInCombatDelayMin, PluginConfigFloat.NotInCombatDelayMax, 0, 3, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.ClickingDelayMin, PluginConfigFloat.ClickingDelayMax, 0.05f, 0.25f, 0.002f),
    };

    // 插件整体控制，如自动开关，模式变换啥的
    private static readonly ISearchable[] _basicOtherSearchable = new ISearchable[]
    {
        // 插件开关控制
        new CheckBoxSearchPlugin(PluginConfigBool.StartOnCountdown),
        new CheckBoxSearchPlugin(PluginConfigBool.StartOnAttackedBySomeone),
        new CheckBoxSearchPlugin(PluginConfigBool.ToggleManual),

        
        // 自动关闭控制
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffBetweenArea),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffCutScene),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffWhenDead),
        new DragFloatSearchPlugin(PluginConfigFloat.AutoOffAfterCombat, 0f, 10f, 1f),     
    };

    private static readonly CollapsingHeaderGroup _baseHeader = new(new()
    {
        { () => "基础参数设置", () =>
            {
                foreach (var searchable in _basicParamsSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => "模式及开关设置", () =>
            {
                foreach (var searchable in _basicOtherSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
    });

    private static void DrawBasic()
    {
        _baseHeader?.Draw();
    }

    // 提示通知相关的设置
    private static readonly ISearchable[] _uiInfoSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.ShowInfoOnDtr),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowInfoOnToast),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowToastsAboutDoAction),
    };

    // 画图，遮罩层相关的设置
    private static readonly ISearchable[] _uiDrawSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.TeachingMode, new ISearchable[]
        {
            new ColorEditSearchPlugin(PluginConfigVector4.TeachingModeColor)
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.KeyBoardNoise, new ISearchable[]
        {
            new DragIntRangeSearchPlugin(PluginConfigInt.KeyBoardNoiseMin, PluginConfigInt.KeyBoardNoiseMax, 0, 3, 1)
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowMoveTarget, new ISearchable[]
        {
            new ColorEditSearchPlugin(PluginConfigVector4.MovingTargetColor)
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowTarget, new ISearchable[]
        {
            new DragFloatSearchPlugin(PluginConfigFloat.TargetIconSize,0, 1, 0.002f),
            new ColorEditSearchPlugin(PluginConfigVector4.TargetColor),
            new ColorEditSearchPlugin(PluginConfigVector4.SubTargetColor),
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.DrawPositional, new ISearchable[]
        {
            new DragFloatSearchPlugin(PluginConfigFloat.DrawingHeight,0, 8, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.SampleLength,0.005f, 0.05f, 0.001f),
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.DrawMeleeOffset, new ISearchable[]
        {
            new DragFloatSearchPlugin(PluginConfigFloat.DrawingHeight,0, 8, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.SampleLength,0.005f, 0.05f, 0.001f),
        }),

    };

    // 悬浮窗设置
    private static readonly ISearchable[] _uiOverlaySearchable = new ISearchable[]
    {
        // 只在有敌人或在副本显示
        //new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowControlWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.OnlyShowWithHostileOrInDuty),
            new CheckBoxSearchPlugin(PluginConfigBool.IsControlWindowLock),

            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowNextSizeRatio,0f, 10f, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindowGCDSize,0f, 80f, 0.2f),
            new DragFloatSearchPlugin(PluginConfigFloat.ControlWindow0GCDSize,0f, 80f, 0.2f),

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

            new DragIntSearchPlugin(PluginConfigInt.CooldownActionOneLine,1, 30, 1),
            new DragFloatSearchPlugin(PluginConfigFloat.CooldownFontSize,9.6f, 96f, 0.1f),
            new DragFloatSearchPlugin(PluginConfigFloat.CooldownWindowIconSize,0f, 80f, 0.2f),

            new ColorEditSearchPlugin(PluginConfigVector4.InfoWindowBg),
        }),
    };

    private static readonly CollapsingHeaderGroup _UIHeader = new(new()
    {
        { () => "通知提示设置", () =>
            {
                foreach (var searchable in _uiInfoSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => "绘图遮罩设置", () =>
            {
                foreach (var searchable in _uiDrawSearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
        { () => "悬浮窗设置", () =>
            {
                foreach (var searchable in _uiOverlaySearchable)
                {
                    searchable?.Draw(Job);
                }
            }
        },
    });

    private static void DrawUI()
    {
        _UIHeader?.Draw();
    }

    private static readonly ISearchable[] _autoSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.UseAOEAction,  new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.UseAOEWhenManual),
            new CheckBoxSearchPlugin(PluginConfigBool.NoNewHostiles),
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.PreventActions, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.PreventActionsDuty),
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoBurst),
        new CheckBoxSearchPlugin(PluginConfigBool.AutoHeal),
        new CheckBoxSearchPlugin(PluginConfigBool.UseTinctures),
        new CheckBoxSearchPlugin(PluginConfigBool.UseHealPotions),
        new CheckBoxSearchPlugin(PluginConfigBool.UseAbility, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.UseDefenseAbility),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoTankStance),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoProvokeForTank),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoUseTrueNorth),
            new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerBySwift),
            new CheckBoxSearchPlugin(PluginConfigBool.AutoSpeedOutOfCombat),
            new CheckBoxSearchPlugin(PluginConfigBool.UseGroundBeneficialAbility),
        }),
        new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerByCasting),
        new CheckBoxSearchPlugin(PluginConfigBool.UseHealWhenNotAHealer),
        new DragIntSearchPlugin(PluginConfigInt.LessMPNoRaise, 0, 10000, 200),

        new CheckBoxSearchPlugin(PluginConfigBool.InterruptibleMoreCheck),
        new CheckBoxSearchPlugin(PluginConfigBool.EsunaAll),
        new CheckBoxSearchPlugin(PluginConfigBool.HealOutOfCombat),
        new DragFloatSearchPlugin(PluginConfigFloat.HealWhenNothingTodoBelow,0,1,0.002f),
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyHotOnTanks),
        new CheckBoxSearchPlugin(PluginConfigBool.BeneficialAreaOnTarget),
        new DragFloatSearchPlugin(PluginConfigFloat.DistanceForMoving,0,30,1f),
        new DragFloatSearchPlugin(PluginConfigFloat.MeleeRangeOffset,0,5,0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.HealthDifference,0,0.5f,0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.HealthHealerRatio,0,1,0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.HealthTankRatio,0,1,0.02f),

    };

    private static readonly CollapsingHeaderGroup _autoHeader = new(new()
    {
        { () => "通用", () =>
            {
                foreach (var searchable in _autoSearchable)
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
        new DragIntSearchPlugin(PluginConfigInt.MoveTargetAngle,0,100,0.02f),
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
        new CheckBoxSearchPlugin(PluginConfigBool.SayOutStateChanged),
        new CheckBoxSearchPlugin(PluginConfigBool.PoslockCasting, new ISearchable[]
        {

        }),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowHealthRatio, new ISearchable[]
        {
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioBoss, 0, 10, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioDying, 0, 10, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.HealthRatioDot, 0, 10, 0.02f),

        }),
        new CheckBoxSearchPlugin(PluginConfigBool.ShowTooltips),
        new CheckBoxSearchPlugin(PluginConfigBool.InDebug),

        // 死亡停止读条，待定
        new CheckBoxSearchPlugin(PluginConfigBool.UseStopCasting,new ISearchable[] 
        {
            new DragFloatRangeSearchPlugin(PluginConfigFloat.StopCastingDelayMin, PluginConfigFloat.StopCastingDelayMin, 0, 3, 0.002f) 
        }),

    };
    private static void DrawExtra()
    {
        foreach (var searchable in _extraSearchable)
        {
            searchable?.Draw(Job);
        }
    }
}
