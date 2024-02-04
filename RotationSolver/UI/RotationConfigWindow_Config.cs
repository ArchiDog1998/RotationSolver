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
    private string _searchText = string.Empty;
    private ISearchable[] _searchResults = [];

    private static SearchableCollection _allSearchables = new();

    private void SearchingBox()
    {
        if (ImGui.InputTextWithHint("##Rotation Solver Search Box", "ConfigWindow_Searching".Local("Search... "), ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll))
        {
            _searchResults = _allSearchables.SearchItems(_searchText);
        }
    }

    #region Basic
    private static void DrawBasic()
    {
        _baseHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _baseHeader = new(new()
    {
        { () =>  "ConfigWindow_Basic_Timer".Local("Timer"), DrawBasicTimer },
        { () => "ConfigWindow_Basic_AutoSwitch".Local("Auto Switch"), DrawBasicAutoSwitch },
        { () => "ConfigWindow_Basic_NamedConditions".Local("Named Conditions"), DrawBasicNamedConditions },
        { () => "ConfigWindow_Basic_Others".Local("Others"), DrawBasicOthers },
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
            ImguiTooltips.ShowTooltip("ConfigWindow_Basic_Ping".Local("The ping time.\nIn RS, it means the time from sending the action request to receiving the using success message from the server."));
        }

        var rectStart = lineStart + new Vector2(ping * sizePerTime, 0);
        size = new Vector2(animationLockTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(LOCK_TIME_COLOR));
        if (ImGuiHelper.IsInRect(rectStart, size))
        {
            ImguiTooltips.ShowTooltip("ConfigWindow_Basic_AnimationLockTime".Local("The Animation lock time from individual actions. Here is 0.6s for example."));
        }

        drawList.AddLine(lineStart - new Vector2(0, spacingHeight), lineStart + new Vector2(0, pingHeight * 2 + spacingHeight / 2), IDEAL_CLICK_TIME_COLOR, 1.5f);

        rectStart = lineStart + new Vector2(-advanceTime * sizePerTime, pingHeight);
        size = new Vector2(advanceTime * sizePerTime, pingHeight);
        drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(color));
        if (ImGuiHelper.IsInRect(rectStart, size))
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                ImGui.TextWrapped("ConfigWindow_Basic_ClickingDuration".Local("The clicking duration, RS will try to click at this moment."));

                ImGui.Separator();

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(IDEAL_CLICK_TIME_COLOR),
                    "ConfigWindow_Basic_IdealClickingTime".Local("The ideal click time."));

                ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(CLICK_TIME_COLOR),
                    "ConfigWindow_Basic_RealClickingTime".Local("The real click time."));
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

        var actionAhead = Service.Config.ActionAhead;
        var minAbilityAhead = Service.Config.MinLastAbilityAdvanced;
        var animationLockTime = DataCenter.MinAnimationLock;
        var weaponDelay = (Service.Config.WeaponDelay.X + Service.Config.WeaponDelay.Y) / 2;
        var clickingDelay = (Service.Config.ClickingDelay.X + Service.Config.ClickingDelay.Y) / 2;

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
            ImguiTooltips.ShowTooltip(typeof(ConfigsNew).GetProperty(nameof(ConfigsNew.WeaponDelay))!.Local());
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

        _allSearchables.DrawItems(ConfigsNew.BasicTimer);
    }

    private static readonly CollapsingHeaderGroup _autoSwitch = new(new()
    {
        { () => "ConfigWindow_Basic_SwitchCancelConditionSet".Local("Auto turn off conditions"),
            () => DataCenter.RightSet.SwitchCancelConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => "ConfigWindow_Basic_SwitchManualConditionSet".Local("Auto turn manual conditions"),
            () => DataCenter.RightSet.SwitchManualConditionSet?.DrawMain(DataCenter.RightNowRotation) },

        { () => "ConfigWindow_Basic_SwitchAutoConditionSet".Local("Auto turn auto conditions"),
            () => DataCenter.RightSet.SwitchAutoConditionSet?.DrawMain(DataCenter.RightNowRotation) },
    })
    {
        HeaderSize = 18,
    };
    private static void DrawBasicAutoSwitch()
    {
        _allSearchables.DrawItems(ConfigsNew.BasicAutoSwitch);
        _autoSwitch?.Draw();
    }

    private static readonly Dictionary<int, bool> _isOpen = [];
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
            ImGui.InputTextWithHint($"##Rotation Solver Named Condition{i}", "ConfigWindow_Condition_ConditionName".Local("Condition Name"),
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

            if (value && DataCenter.RightNowRotation != null)
            {
                DataCenter.RightSet.NamedConditions[i].Condition?.DrawMain(DataCenter.RightNowRotation);
            }
        }
        if (removeIndex > -1)
        {
            var list = DataCenter.RightSet.NamedConditions.ToList();
            list.RemoveAt(removeIndex);
            DataCenter.RightSet.NamedConditions = [.. list];
        }
    }

    private static void DrawBasicOthers()
    {
        _allSearchables.DrawItems(ConfigsNew.BasicParams);

        if (Service.Config.SayHelloToAll)
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
    #endregion

    #region UI
    private static void DrawUI()
    {
        _UIHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _UIHeader = new(new()
    {
        { 
            () => "ConfigWindow_UI_Information".Local("Information"),
            () => _allSearchables.DrawItems(ConfigsNew.UiInformation)
        },
        {
            () => "ConfigWindow_UI_Overlay".Local("Overlay"),
            () =>_allSearchables.DrawItems(ConfigsNew.UiOverlay)
        },
        { 
            () => "ConfigWindow_UI_Windows".Local("Windows"),
            () =>_allSearchables.DrawItems(ConfigsNew.UiWindows)
        },
    });

    #endregion

    #region Auto
    private static void DrawAuto()
    {
        ImGui.TextWrapped("ConfigWindow_Auto_Description".Local("Change the way that RS atomatically uses actions."));
        _autoHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _autoHeader = new(new()
    {
        { () => "ConfigWindow_Auto_ActionUsage".Local("Action Usage"), () =>
            {
                ImGui.TextWrapped("ConfigWindow_Auto_ActionUsage_Description"
                    .Local("Which actions Rotation Solver can use."));
                ImGui.Separator();

                _allSearchables.DrawItems(ConfigsNew.AutoActionUsage);
            }
        },
        { () => "ConfigWindow_Auto_ActionCondition".Local("Action Condition"), DrawAutoActionCondition },
        { () => "ConfigWindow_Auto_StateCondition".Local("State Condition"), () => _autoState?.Draw() },
    });


    private static readonly CollapsingHeaderGroup _autoState = new(new()
    {
        { 
            () => "ConfigWindow_Auto_HealAreaConditionSet".Local("Heal Area Forced Condition"),
            () => DataCenter.RightSet.HealAreaConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },

        { 
            () => "ConfigWindow_Auto_HealSingleConditionSet".Local("Heal Single Forced Condition"),
            () => DataCenter.RightSet.HealSingleConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },

        { 
            () => "ConfigWindow_Auto_DefenseAreaConditionSet".Local("Defense Area Forced Condition"),
            () => DataCenter.RightSet.DefenseAreaConditionSet?.DrawMain(DataCenter.RightNowRotation)
        },

        { 
            () =>"ConfigWindow_Auto_DefenseSingleConditionSet".Local("Defense Single Forced Condition"),
            () => DataCenter.RightSet.DefenseSingleConditionSet?.DrawMain(DataCenter.RightNowRotation)
        },

        { 
            () => "Esuna Stance North Forced Condition".Local("Esuna Stance North Forced Condition"),
            () => DataCenter.RightSet.EsunaStanceNorthConditionSet?.DrawMain(DataCenter.RightNowRotation)
        },

        { 
            () => "ConfigWindow_Auto_RaiseShirkConditionSet".Local("Raise Shirk Forced Condition"),
            () => DataCenter.RightSet.RaiseShirkConditionSet?.DrawMain(DataCenter.RightNowRotation)
        },

        { 
            () => "ConfigWindow_Auto_MoveForwardConditionSet".Local("Move Forward Forced Condition"),
            () => DataCenter.RightSet.MoveForwardConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },

        {
            () => "ConfigWindow_Auto_MoveBackConditionSet".Local("Move Back Forced Condition"),
            () => DataCenter.RightSet.MoveBackConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },

        { 
            () => "ConfigWindow_Auto_AntiKnockbackConditionSet".Local("Anti Knockback Forced Condition"),
            () => DataCenter.RightSet.AntiKnockbackConditionSet?.DrawMain(DataCenter.RightNowRotation)
        },

        { 
            () => "ConfigWindow_Auto_SpeedConditionSet".Local("Speed Forced Condition"),
            () => DataCenter.RightSet.SpeedConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },

        {
            () => "ConfigWindow_Auto_LimitBreakConditionSet".Local("Limit Break Condition"),
            () => DataCenter.RightSet.LimitBreakConditionSet?.DrawMain(DataCenter.RightNowRotation) 
        },
    })
    {
        HeaderSize = 18,
    };

    private static void DrawAutoActionCondition()
    {
        ImGui.TextWrapped("ConfigWindow_Auto_ActionCondition_Description".Local("This will change the way that Rotation Solver uses actions."));
        ImGui.Separator();

        _allSearchables.DrawItems(ConfigsNew.AutoActionCondition);
    }
    #endregion

    #region Target
    private static void DrawTarget()
    {
        _targetHeader?.Draw();
    }

    private static readonly CollapsingHeaderGroup _targetHeader = new(new()
    {
        { () => "ConfigWindow_Target_Config".Local( "Configuration"), DrawTargetConfig },
        { () => "ConfigWindow_List_Hostile".Local( "Hostile"), DrawTargetHostile },
    });

    private static void DrawTargetConfig()
    {
        _allSearchables.DrawItems(ConfigsNew.TargetConfig);
    }

    private static void DrawTargetHostile()
    {
        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "Add Hostile"))
        {
            Service.Config.TargetingTypes.Add(TargetingType.Big);
        }
        ImGui.SameLine();
        ImGui.TextWrapped("ConfigWindow_Param_HostileDesc".Local("You can The logic of hostile target selection to allow flexibility in switching the logic of selecting hostile in battle."));

        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            var targetType = Service.Config.TargetingTypes[i];

            void Delete()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
            };

            void Up()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
                Service.Config.TargetingTypes.Insert(Math.Max(0, i - 1), targetType);
            };
            void Down()
            {
                Service.Config.TargetingTypes.RemoveAt(i);
                Service.Config.TargetingTypes.Insert(Math.Min(Service.Config.TargetingTypes.Count - 1, i + 1), targetType);
            }

            var key = $"Targeting Type Pop Up: {i}";

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                ("ConfigWindow_List_Remove".Local("Remove"), Delete, ["Delete"]),
                ("ConfigWindow_Actions_MoveUp".Local("Move Up"), Up, ["↑"]),
                ("ConfigWindow_Actions_MoveDown".Local("Move Down"), Down, ["↓"]));

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];
            var text = "ConfigWindow_Param_HostileCondition".Local("Hostile target selection condition");
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(text).X + 30 * Scale);
            if (ImGui.Combo(text + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.TargetingTypes[i] = (TargetingType)targingType;
            }

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                (Delete, [VirtualKey.DELETE]),
                (Up, [VirtualKey.UP]),
                (Down, [VirtualKey.DOWN]));
        }
    }
    #endregion

    #region Extra
    private static void DrawExtra()
    {
        ImGui.TextWrapped("ConfigWindow_Extra_Description".Local("Rotation Solver focuses on the rotation itself. These are side features. If there are some other plugins can do that, these features will be deleted."));
        _extraHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _extraHeader = new(new()
    {
        { () => "ConfigWindow_EventItem".Local("Event"), DrawEventTab },

        { 
            () => "ConfigWindow_Extra_Others".Local("Others"), 
            () => _allSearchables.DrawItems(ConfigsNew.Extra)
        },
    });

    private static void DrawEventTab()
    {
        if (ImGui.Button("ConfigWindow_Events_AddEvent".Local("Add Events")))
        {
            Service.Config.Events.Add(new ActionEventInfo());
        }
        ImGui.SameLine();

        ImGui.TextWrapped("ConfigWindow_Events_Description".Local("In this window, you can set what macro will be trigger after using an action."));

        ImGui.Text("ConfigWindow_Events_DutyStart".Local("Duty Start: "));
        ImGui.SameLine();
        Service.Config.DutyStart.DisplayMacro();

        ImGui.Text("ConfigWindow_Events_DutyEnd".Local("Duty End: "));
        ImGui.SameLine();
        Service.Config.DutyEnd.DisplayMacro();

        ImGui.Separator();

        ActionEventInfo? remove = null;
        foreach (var eve in Service.Config.Events)
        {
            eve.DisplayEvent();

            ImGui.SameLine();

            if (ImGui.Button($"{"ConfigWindow_Events_RemoveEvent".Local("Delete Event")}##RemoveEvent{eve.GetHashCode()}"))
            {
                remove = eve;
            }
            ImGui.Separator();
        }
        if (remove != null)
        {
            Service.Config.Events.Remove(remove);
        }
    }
    #endregion
}
