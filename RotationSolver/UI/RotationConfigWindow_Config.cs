using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

public partial class RotationConfigWindow
{
    private static char[] _splitChar = new char[] { ' ', ',', '、', '.', '。' };
    internal static float Similarity(string text, string key)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        var chars = text.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries);
        var keys = key.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries);

        var startWithCount = chars.Count(i => keys.Any(k => i.StartsWith(k, StringComparison.OrdinalIgnoreCase)));

        var containCount = chars.Count(i => keys.Any(k => i.Contains(k, StringComparison.OrdinalIgnoreCase)));

        return startWithCount * 3 + containCount;
    }

    private string _searchText = string.Empty;
    private ISearchable[] _searchResults = Array.Empty<ISearchable>();
    private void SearchingBox()
    {
        if (ImGui.InputTextWithHint("##Rotation Solver Search Box", LocalizationManager.RightLang.ConfigWindow_Searching, ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                const int MAX_RESULT_LENGTH = 20;

                _searchResults = new ISearchable[MAX_RESULT_LENGTH];

                var enumerator = GetType().GetRuntimeFields()
                    .Where(f => f.FieldType == typeof(ISearchable[]) && f.IsInitOnly)
                    .SelectMany(f => (ISearchable[])f.GetValue(this))
                    .SelectMany(GetChildren)
                    .OrderByDescending(i => Similarity(i.SearchingKeys, _searchText))
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
        { () => LocalizationManager.RightLang.ConfigWindow_Basic_NamedConditions, DrawBasicNamedConditions },
        { () => LocalizationManager.RightLang.ConfigWindow_Basic_Others, DrawBasicOthers },
    });

    private static readonly uint PING_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGreen);
    private static readonly uint LOCK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedBlue);
    private static readonly uint WEAPON_DELAY_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGold);
    private static readonly uint IDEAL_CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0f, 0f, 1f));
    private static readonly uint CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedPink);
    private static readonly uint ADVANCE_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow);
    private static readonly uint ADVANCE_ABILITY_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedOrange);
    const float gcdSize = 50, ogcdSize = 40, pingHeight = 12, spacingHeight = 8;

    private static void AddPingLockTime(ImDrawListPtr drawList, Vector2 lineStart, float sizePerTime, float ping, float animationLockTime, float advanceTime, uint color, float clickTime)
    {
        var size = new Vector2(ping * sizePerTime, pingHeight);
        drawList.AddRectFilled(lineStart, lineStart + size, ChangeAlpha(PING_COLOR));
        if (ImGuiHelper.IsInRect(lineStart, size))
        {
            ImguiTooltips.ShowTooltip(LocalizationManager.RightLang.ConfigWindow_Basic_Ping);
        }

        var rectStart = lineStart + new Vector2(ping * sizePerTime, 0);
        size = new Vector2(animationLockTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(LOCK_TIME_COLOR));
        if (ImGuiHelper.IsInRect(rectStart, size))
        {
            ImguiTooltips.ShowTooltip(LocalizationManager.RightLang.ConfigWindow_Basic_AnimationLockTime);
        }

        drawList.AddLine(lineStart - new Vector2(0, spacingHeight), lineStart + new Vector2(0, pingHeight * 2 + spacingHeight / 2), IDEAL_CLICK_TIME_COLOR, 1.5f);

        rectStart = lineStart + new Vector2(-advanceTime * sizePerTime, pingHeight);
        size = new Vector2(advanceTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(color));
        if (ImGuiHelper.IsInRect(rectStart, size))
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Basic_ClickingDuration);

                ImGui.Separator();

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(IDEAL_CLICK_TIME_COLOR),
                    LocalizationManager.RightLang.ConfigWindow_Basic_IdealClickingTime);

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(CLICK_TIME_COLOR),
                    LocalizationManager.RightLang.ConfigWindow_Basic_RealClickingTime);
            });
        }

        float time = 0;
        while (time < advanceTime)
        {
            var start = lineStart + new Vector2((time - advanceTime) * sizePerTime, 0);
            drawList.AddLine(start + new Vector2(0, pingHeight), start + new Vector2(0, pingHeight * 2 + spacingHeight), CLICK_TIME_COLOR, 2.5f);

            time += clickTime;
        }
    }
    private static void DrawBasicTimer()
    {
        var gcdTime = DataCenter.WeaponTotal;
        if (gcdTime == 0) gcdTime = 2.5f;
        var wholeWidth = ImGui.GetWindowWidth();
        var ping = DataCenter.Ping;

        ImGui.PushFont(ImGuiHelper.GetFont(14));
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
        var infoText = $"GCD: {gcdTime:F2}s Ping: {ping:F2}s";
        var infoSize = ImGui.CalcTextSize(infoText);

        ImGuiHelper.DrawItemMiddle(() =>
        {
            ImGui.Text(infoText);
        }, wholeWidth, infoSize.X);
        ImGui.PopStyleColor();
        ImGui.PopFont();

        var actionAhead = Service.Config.GetValue(DataCenter.Job, JobConfigFloat.ActionAhead);
        var minAbilityAhead = Service.Config.GetValue(PluginConfigFloat.MinLastAbilityAdvanced);
        var animationLockTime = DataCenter.MinAnimationLock;
        var weaponDelay = (Service.Config.GetValue(PluginConfigFloat.WeaponDelayMin) + Service.Config.GetValue(PluginConfigFloat.WeaponDelayMax)) / 2;
        var clickingDelay = (Service.Config.GetValue(PluginConfigFloat.ClickingDelayMin) + Service.Config.GetValue(PluginConfigFloat.ClickingDelayMax)) / 2;

        var drawList = ImGui.GetWindowDrawList();
        ImGui.Spacing();
        var startCursorPt = ImGui.GetCursorPos();
        var windowsPos = ImGui.GetWindowPos();

        var sizePerTime = (wholeWidth - gcdSize) / (gcdTime + weaponDelay + actionAhead);

        var lineStart = windowsPos + startCursorPt + new Vector2(sizePerTime * actionAhead, gcdSize + spacingHeight);
        ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * actionAhead, 0), gcdSize, 0);
        ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(wholeWidth - gcdSize, 0), gcdSize, 0);

        AddPingLockTime(drawList, lineStart, sizePerTime, ping, animationLockTime, actionAhead, ADVANCE_TIME_COLOR, clickingDelay);
        var start = lineStart + new Vector2(gcdTime * sizePerTime, 0);
        var rectSize = new Vector2(weaponDelay * sizePerTime, pingHeight);
        drawList.AddRectFilled(start, start + rectSize, WEAPON_DELAY_COLOR);
        drawList.AddRect(start, start + rectSize, uint.MaxValue, 0, ImDrawFlags.Closed, 2);
        if (ImGuiHelper.IsInRect(start, rectSize))
        {
            ImguiTooltips.ShowTooltip(LocalizationManager.RightLang.ConfigWindow_Basic_WeaponDelay);
        }
        drawList.AddLine(lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime, -spacingHeight), lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime,
            pingHeight * 2 + spacingHeight), IDEAL_CLICK_TIME_COLOR, 2);

        ImGui.PushFont(ImGuiHelper.GetFont(20));
        const string gcdText = "GCD";
        var size = ImGui.CalcTextSize(gcdText);
        ImGui.SetCursorPos(startCursorPt + new Vector2(sizePerTime * actionAhead + (gcdSize - size.X) / 2, (gcdSize - size.Y) / 2));
        ImGui.Text(gcdText);
        ImGui.SetCursorPos(startCursorPt + new Vector2(wholeWidth - gcdSize + (gcdSize - size.X) / 2, (gcdSize - size.Y) / 2));
        ImGui.Text(gcdText);
        ImGui.PopFont();

        ImGui.PushFont(ImGuiHelper.GetFont(14));
        const string ogcdText = "Off-\nGCD";
        size = ImGui.CalcTextSize(ogcdText);
        ImGui.PopFont();

        var timeStep = ping + animationLockTime;
        var time = timeStep;
        while (time < gcdTime - timeStep)
        {
            var isLast = time + 2 * timeStep > gcdTime;
            if (isLast)
            {
                time = gcdTime - timeStep;
            }

            ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * (actionAhead + time), 0), ogcdSize, 0);
            ImGui.SetCursorPos(startCursorPt + new Vector2(sizePerTime * (actionAhead + time) + (ogcdSize - size.X) / 2, (ogcdSize - size.Y) / 2));

            ImGui.PushFont(ImGuiHelper.GetFont(14));
            ImGui.Text(ogcdText);
            ImGui.PopFont();

            var ogcdStart = lineStart + new Vector2(time * sizePerTime, 0);
            AddPingLockTime(drawList, ogcdStart, sizePerTime, ping, animationLockTime,
                isLast ? MathF.Max(minAbilityAhead, actionAhead) : actionAhead, isLast ? ADVANCE_ABILITY_TIME_COLOR : ADVANCE_TIME_COLOR, clickingDelay);

            time += timeStep;
        }

        ImGui.SetCursorPosY(startCursorPt.Y + gcdSize + pingHeight * 2 + 2 * spacingHeight + ImGui.GetStyle().ItemSpacing.Y);

        ImGui.Spacing();

        foreach (var searchable in _basicTimer)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _basicTimerOthers)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly CollapsingHeaderGroup _autoSwitch = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Basic_SwitchCancelConditionSet,
            () => DataCenter.RightSet.SwitchCancelConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Basic_SwitchManualConditionSet,
            () => DataCenter.RightSet.SwitchManualConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Basic_SwitchAutoConditionSet,
            () => DataCenter.RightSet.SwitchAutoConditionSet?.DrawMain(DataCenter.RightNowRotation) },
    })
    {
        HeaderSize = 18,
    };
    private static void DrawBasicAutoSwitch()
    {
        foreach (var searchable in _basicSwitchTurnOff)
        {
            searchable?.Draw(Job);
        }

        ImGui.Separator();

        foreach (var searchable in _basicSwitchTurnOn)
        {
            searchable?.Draw(Job);
        }

        _autoSwitch?.Draw();
    }

    private static readonly Dictionary<int, bool> _isOpen = new();
    private static void DrawBasicNamedConditions()
    {
        if (!DataCenter.RightSet.NamedConditions.Any(c => string.IsNullOrEmpty(c.Name)))
        {
            DataCenter.RightSet.NamedConditions = DataCenter.RightSet.NamedConditions.Append((string.Empty, new ConditionSet())).ToArray();
        }

        ImGui.Spacing();

        int removeIndex = -1;
        for (int i = 0; i < DataCenter.RightSet.NamedConditions.Length; i++)
        {
            var value = _isOpen.TryGetValue(i, out var open) && open;

            var toggle = value ? FontAwesomeIcon.ArrowUp : FontAwesomeIcon.ArrowDown;
            var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X
                - ImGuiEx.CalcIconSize(toggle).X - ImGui.GetStyle().ItemSpacing.X * 2 - 20 * Scale;

            ImGui.SetNextItemWidth(width);
            ImGui.InputTextWithHint($"##Rotation Solver Named Condition{i}", LocalizationManager.RightLang.ConfigWindow_Condition_ConditionName,
                ref DataCenter.RightSet.NamedConditions[i].Name, 1024);

            ImGui.SameLine();

            if (ImGuiEx.IconButton(toggle, $"##Rotation Solver Toggle Named Condition{i}"))
            {
                _isOpen[i] = value = !value;
            }

            ImGui.SameLine();

            if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Named Condition{i}"))
            {
                removeIndex = i;
            }

            if (value)
            {
                DataCenter.RightSet.NamedConditions[i].Condition?.DrawMain(DataCenter.RightNowRotation);
            }
        }
        if (removeIndex > -1)
        {
            var list = DataCenter.RightSet.NamedConditions.ToList();
            list.RemoveAt(removeIndex);
            DataCenter.RightSet.NamedConditions = list.ToArray();
        }
    }

    private static void DrawBasicOthers()
    {
        foreach (var searchable in _basicParamsSearchable)
        {
            searchable?.Draw(Job);
        }

        if (Service.Config.GetValue(PluginConfigBool.SayHelloToAll))
        {
            var str = SocialUpdater.EncryptString(Player.Object);
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
            ImGui.InputText("That is your HASH:", ref str, 100);

            if (!DownloadHelper.ContributorsHash.Contains(str)
                && !DownloadHelper.UsersHash.Contains(str)
                && !RotationUpdater.AuthorHashes.ContainsKey(str))
            {
                if (ImGui.Button("DM your Hash to ArchiTed for being greeted."))
                {
                    ImGui.SetClipboardText(str);
                    Notify.Success($"Your hash \"{str}\" copied to clipboard.");
                    Util.OpenLink("https://discord.com/users/1007293294100877322");
                }
            }
        }
        else
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "The author of RS loves being greeted by you!");
        }

    }

    private static readonly ISearchable[] _basicTimer = new ISearchable[]
    {
        new DragFloatSearchJob(JobConfigFloat.ActionAhead, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MinLastAbilityAdvanced, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MaxPing, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeaponDelayMin, PluginConfigFloat.WeaponDelayMax, 0.002f),
        new DragFloatRangeSearchPlugin(PluginConfigFloat.ClickingDelayMin, PluginConfigFloat.ClickingDelayMax, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MinUpdatingTime, 0.002f),
    };

    private static readonly ISearchable[] _basicTimerOthers = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.SpecialDuration, 1f),
        new DragFloatSearchPlugin(PluginConfigFloat.CountDownAhead, 0.002f)
        {
            PvPFilter = JobFilter.NoJob,
        },
    };

    private static readonly ISearchable[] _basicParamsSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.ToggleManual),
        new CheckBoxSearchPlugin(PluginConfigBool.ToggleAuto),

        new CheckBoxSearchPlugin(PluginConfigBool.UseWorkTask),

        new DragFloatSearchPlugin(PluginConfigFloat.MistakeRatio, 0.002f),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.NotInCombatDelayMin, PluginConfigFloat.NotInCombatDelayMax, 0.002f),

        new CheckBoxSearchPlugin(PluginConfigBool.UseAdditionalConditions),

        new CheckBoxSearchPlugin(PluginConfigBool.SayHelloToAll,
            new CheckBoxSearchPlugin(PluginConfigBool.SayHelloToUsers),
            new CheckBoxSearchPlugin(PluginConfigBool.JustSayHelloOnce)),
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
        new CheckBoxSearchPlugin(PluginConfigBool.AutoOffWhenDutyCompleted),
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
        new CheckBoxSearchPlugin(PluginConfigBool.ShowToggledActionInChat),

        new CheckBoxSearchPlugin(PluginConfigBool.KeyBoardNoise,
            new DragIntRangeSearchPlugin(PluginConfigInt.KeyBoardNoiseMin, PluginConfigInt.KeyBoardNoiseMax, 1)),

        new CheckBoxSearchPlugin(PluginConfigBool.SayOutStateChanged),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowTooltips),

        new CheckBoxSearchPlugin(PluginConfigBool.HideWarning),
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

        new CheckBoxSearchPlugin(PluginConfigBool.ShowTarget,
            new DragFloatSearchPlugin(PluginConfigFloat.TargetIconSize, 0.002f),
            new ColorEditSearchPlugin(PluginConfigVector4.TargetColor),
            new ColorEditSearchPlugin(PluginConfigVector4.SubTargetColor)
        ),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowTargetTimeToKill,
            new ColorEditSearchPlugin(PluginConfigVector4.TTKTextColor)),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowMoveTarget,
            new ColorEditSearchPlugin(PluginConfigVector4.MovingTargetColor)
        ),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowHostilesIcons,
            new DragFloatSearchPlugin(PluginConfigFloat.HostileIconHeight, 0.002f),
            new DragFloatSearchPlugin(PluginConfigFloat.HostileIconSize, 0.002f)
        ),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowStateIcon,
            new DragFloatSearchPlugin(PluginConfigFloat.StateIconHeight, 0.002f),
            new DragFloatSearchPlugin(PluginConfigFloat.StateIconSize, 0.002f)),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowBeneficialPositions,
            new ColorEditSearchPlugin(PluginConfigVector4.BeneficialPositionColor),
            new ColorEditSearchPlugin(PluginConfigVector4.HoveredBeneficialPositionColor)
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
        new CheckBoxSearchPlugin(PluginConfigBool.ShowCooldownWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.IsControlWindowLock),

            new CheckBoxSearchPlugin(PluginConfigBool.UseOriginalCooldown),
            new CheckBoxSearchPlugin(PluginConfigBool.ShowGCDCooldown),
            new CheckBoxSearchPlugin(PluginConfigBool.ShowItemsCooldown),

            new DragFloatSearchPlugin(PluginConfigFloat.CooldownFontSize, 0.1f),
            new DragFloatSearchPlugin(PluginConfigFloat.CooldownWindowIconSize, 0.2f),

            new ColorEditSearchPlugin(PluginConfigVector4.InfoWindowBg),
        }),

        new CheckBoxSearchPlugin(PluginConfigBool.ShowNextActionWindow, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoMove),
            new CheckBoxSearchPlugin(PluginConfigBool.IsInfoWindowNoInputs),

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
        { () => LocalizationManager.RightLang.ConfigWindow_Auto_StateCondition, () => _autoState?.Draw() },
    });

    private static readonly CollapsingHeaderGroup _autoState = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Auto_HealAreaConditionSet,
            () => DataCenter.RightSet.HealAreaConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_HealSingleConditionSet,
            () => DataCenter.RightSet.HealSingleConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_DefenseAreaConditionSet,
            () => DataCenter.RightSet.DefenseAreaConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_DefenseSingleConditionSet,
            () => DataCenter.RightSet.DefenseSingleConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_EsunaStanceNorthConditionSet,
            () => DataCenter.RightSet.EsunaStanceNorthConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_RaiseShirkConditionSet,
            () => DataCenter.RightSet.RaiseShirkConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_MoveForwardConditionSet,
            () => DataCenter.RightSet.MoveForwardConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_MoveBackConditionSet,
            () => DataCenter.RightSet.MoveBackConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_AntiKnockbackConditionSet,
            () => DataCenter.RightSet.AntiKnockbackConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_SpeedConditionSet,
            () => DataCenter.RightSet.SpeedConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => LocalizationManager.RightLang.ConfigWindow_Auto_LimitBreakConditionSet,
            () => DataCenter.RightSet.LimitBreakConditionSet?.DrawMain(DataCenter.RightNowRotation) },
    })
    {
        HeaderSize = 18,
    };

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
                PvEFilter = JobFilter.NoHealer,
                PvPFilter = JobFilter.NoJob,
            },

            new DragFloatSearchPlugin(PluginConfigFloat.AutoHealTimeToKill, 0.02f),
            new DragFloatSearchPlugin(PluginConfigFloat.HealthDifference, 0.02f)),

        new CheckBoxSearchPlugin(PluginConfigBool.OnlyHealSelfWhenNoHealer)
        {
            PvEFilter = JobFilter.NoHealer,
            PvPFilter = JobFilter.NoHealer,
        },

        new CheckBoxSearchPlugin(PluginConfigBool.OnlyHotOnTanks)
        {
            PvEFilter = JobFilter.Healer,
            PvPFilter = JobFilter.Healer,
        },

        new CheckBoxSearchPlugin(PluginConfigBool.HealOutOfCombat),

        new CheckBoxSearchPlugin(PluginConfigBool.HealWhenNothingTodo,
            new DragFloatSearchPlugin(PluginConfigFloat.HealWhenNothingTodoBelow, 0.002f),
            new DragFloatRangeSearchPlugin(PluginConfigFloat.HealWhenNothingTodoMin,
                PluginConfigFloat.HealWhenNothingTodoMax, 0.05f)),

        new DragFloatSearchPlugin(PluginConfigFloat.HealthHealerRatio, 0.02f)
            {
                PvEFilter = JobFilter.Healer,
                PvPFilter = JobFilter.Healer,
            },
        new DragFloatSearchPlugin(PluginConfigFloat.HealthTankRatio, 0.02f)
            {
                PvEFilter = JobFilter.Healer,
                PvPFilter = JobFilter.Healer,
            },
        new DragFloatRangeSearchPlugin(PluginConfigFloat.HealDelayMin, PluginConfigFloat.HealDelayMax, 0.002f),
    };

    private static readonly ISearchable[] _autoActionConditionSearchable_Raise = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerByCasting)
            {
                PvEFilter = JobFilter.Raise,
                PvPFilter = JobFilter.NoJob,
            },
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseAll)
            {
                PvEFilter = JobFilter.Raise,
                PvPFilter = JobFilter.NoJob,
            },
        new CheckBoxSearchPlugin(PluginConfigBool.RaiseBrinkOfDeath)
            {
                PvEFilter = JobFilter.Raise,
                PvPFilter = JobFilter.NoJob,
            },
        new DragFloatRangeSearchPlugin(PluginConfigFloat.DeathDelayMin, PluginConfigFloat.DeathDelayMax, 0.002f)
            {
                PvEFilter = JobFilter.Raise,
                PvPFilter = JobFilter.NoJob,
            },
    };

    private static readonly ISearchable[] _autoActionConditionSearchable_Others = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.InterruptibleMoreCheck)
            {
                PvEFilter = JobFilter.Interrupt,
                PvPFilter = JobFilter.NoJob,
            },

        new DragFloatRangeSearchPlugin(PluginConfigFloat.InterruptDelayMin, PluginConfigFloat.InterruptDelayMax, 0.002f)
            {
                PvEFilter = JobFilter.Interrupt,
                PvPFilter = JobFilter.NoJob,
            },


        new CheckBoxSearchPlugin(PluginConfigBool.EsunaAll)
            {
                PvEFilter = JobFilter.Esuna,
                PvPFilter = JobFilter.NoJob,
            },

        new DragFloatRangeSearchPlugin(PluginConfigFloat.WeakenDelayMin, PluginConfigFloat.WeakenDelayMax, 0.002f)
            {
                PvEFilter = JobFilter.Esuna,
            },


        new DragFloatSearchJob(JobConfigFloat.HealthForDyingTanks, 0.02f)
            {
                PvEFilter = JobFilter.Tank,
                PvPFilter = JobFilter.NoJob,
            },

        new DragFloatSearchPlugin(PluginConfigFloat.HealthForGuard, 0.02f)
            {
                PvEFilter = JobFilter.NoJob,
            },

        new DragFloatSearchPlugin(PluginConfigFloat.MeleeRangeOffset, 0.02f)
        {
            PvEFilter = JobFilter.Melee,
            PvPFilter = JobFilter.NoJob,
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
        new CheckBoxSearchPlugin(PluginConfigBool.UseResourcesAction),

        new DragIntSearchPlugin(PluginConfigInt.LessMPNoRaise, 200)
        {
            PvEFilter = JobFilter.Raise,
        },

        new CheckBoxSearchPlugin(PluginConfigBool.UseAbility, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.UseDefenseAbility,
                new DragIntSearchPlugin(PluginConfigInt.AutoDefenseNumber, 0.05f)
                {
                    PvEFilter = JobFilter.Tank,
                },

                new DragFloatSearchJob(JobConfigFloat.HealthForAutoDefense, 0.02f)
                {
                    PvEFilter = JobFilter.Tank,
                }),

            new CheckBoxSearchPlugin(PluginConfigBool.AutoTankStance)
            {
                PvEFilter = JobFilter.Tank,
            },

            new CheckBoxSearchPlugin(PluginConfigBool.AutoProvokeForTank,
                new DragFloatRangeSearchPlugin(PluginConfigFloat.ProvokeDelayMin, PluginConfigFloat.ProvokeDelayMax, 0.05f))
            {
                PvEFilter = JobFilter.Tank,
            },

            new CheckBoxSearchPlugin(PluginConfigBool.AutoUseTrueNorth)
            {
                PvEFilter = JobFilter.Melee,
            },
            new CheckBoxSearchPlugin(PluginConfigBool.RaisePlayerBySwift)
            {
                 PvEFilter = JobFilter.Healer,
            },
            new CheckBoxSearchPlugin(PluginConfigBool.UseGroundBeneficialAbility,
            new DragIntSearchPlugin(PluginConfigInt.BeneficialAreaStrategy, () => new string[]{
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnLocations,
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnlyOnLocations,
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnTarget,
                LocalizationManager.RightLang.ConfigWindow_Param_BeneficialAreaOnCalculated }),
             new CheckBoxSearchPlugin(PluginConfigBool.UseGroundBeneficialAbilityWhenMoving))
            {
                PvEFilter = JobFilter.Healer,
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
            LocalizationManager.RightLang.ConfigWindow_Param_TargetToHostileType3
        })
        {
            PvPFilter = JobFilter.NoJob,
        },

        new CheckBoxSearchPlugin(PluginConfigBool.AddEnemyListToHostile, new CheckBoxSearchPlugin(PluginConfigBool.OnlyAttackInEnemyList)),
        new CheckBoxSearchPlugin(PluginConfigBool.FilterStopMark),
        new CheckBoxSearchPlugin(PluginConfigBool.ChooseAttackMark, new ISearchable[]
        {
            new CheckBoxSearchPlugin(PluginConfigBool.CanAttackMarkAOE),
        }),

        new DragFloatRangeSearchPlugin(PluginConfigFloat.HostileDelayMin, PluginConfigFloat.HostileDelayMax, 0.002f),
    };

    private static readonly ISearchable[] _targetHostileSelectSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.BossTimeToKill, 0.02f),
        new DragFloatSearchPlugin(PluginConfigFloat.DyingTimeToKill, 0.02f),

        new CheckBoxSearchPlugin(PluginConfigBool.OnlyAttackInView),
        new CheckBoxSearchPlugin(PluginConfigBool.OnlyAttackInVisionCone,
            new DragFloatSearchPlugin(PluginConfigFloat.AngleOfVisionCone, 0.02f)),

        new CheckBoxSearchPlugin(PluginConfigBool.ChangeTargetForFate),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetFatePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetHuntingRelicLevePriority),
        new CheckBoxSearchPlugin(PluginConfigBool.TargetQuestPriority),
    };

    private static readonly ISearchable[] _targetMovingSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.MoveTowardsScreenCenter),
        new CheckBoxSearchPlugin(PluginConfigBool.MoveAreaActionFarthest),
        new DragFloatSearchPlugin(PluginConfigFloat.MoveTargetAngle, 0.02f),
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

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveUp, Up, new string[] { "↑" }),
                (LocalizationManager.RightLang.ConfigWindow_Actions_MoveDown, Down, new string[] { "↓" }));

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.GlobalConfig.TargetingTypes[i];
            var text = LocalizationManager.RightLang.ConfigWindow_Param_HostileCondition;
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(text).X + 30 * Scale);
            if (ImGui.Combo(text + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.GlobalConfig.TargetingTypes[i] = (TargetingType)targingType;
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
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
        new DragIntSearchPlugin(PluginConfigInt.PoslockModifier, () => new string[]{ "CTRL", "SHIFT", "ALT" }),
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
            new DragFloatRangeSearchPlugin(PluginConfigFloat.StopCastingDelayMin, PluginConfigFloat.StopCastingDelayMax, 0.002f)
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
