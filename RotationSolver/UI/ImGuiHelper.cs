using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ECommons.DalamudServices;
using F23.StringSimilarity;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System.ComponentModel;

namespace RotationSolver.UI;

internal static class ImGuiHelper
{

    public static void DrawEnableTexture<T>(this T texture, bool isSelected, Action selected,
        Action<T> showToolTip = null, Action<Action<T>> additionalHeader = null,
        Action otherThing = null) where T : class, ITexture
    {
        var t = texture.GetTexture();

        if (t == null) return;

        showToolTip ??= text =>
        {
            ShowTooltip(text.Description);
        };

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        ImGui.SetColumnWidth(0, t.Width + 5);

        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

        var desc = texture?.Description;
        if (ImGui.IsItemHovered())
        {
            showToolTip(texture);
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                selected?.Invoke();
            }
        }
        ImGui.NextColumn();

        if (isSelected) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
        var enable = texture.IsEnabled;
        if (ImGui.Checkbox($"{texture.Name}##{texture.Name}Enabled", ref enable))
        {
            texture.IsEnabled = enable;
            Service.Config.Save();
        }
        if (isSelected) ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            showToolTip(texture);
        }

        if(texture is IAction)
        {
            ImGui.SameLine();
            Spacing();

            OtherCommandType.ToggleActions.DisplayCommandHelp(texture.ToString());
        }

        additionalHeader?.Invoke(showToolTip);

        if (enable)
        {
            ImGui.Indent(20);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
            otherThing?.Invoke();
            ImGui.PopStyleVar();
            ImGui.Unindent(20);
        }
        ImGui.Columns(1);

        ImGui.PopStyleVar();
    }

    public static bool IconButton(FontAwesomeIcon icon, string name, string description = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.Button($"{icon.ToIconString()}##{name}");
        ImGui.PopFont();
        HoveredString(description ?? icon switch
        {
            FontAwesomeIcon.Coffee => "Donate",
            FontAwesomeIcon.History => "ChangeLog",
            FontAwesomeIcon.Book => "Wiki / Help",
            FontAwesomeIcon.HandPaper => "Support",
            FontAwesomeIcon.Code => "Source Code",
            FontAwesomeIcon.ArrowUp => "Move Up",
            FontAwesomeIcon.ArrowDown => "Move Down",
            FontAwesomeIcon.Ban => "Delete",
            FontAwesomeIcon.Plus => "Add",
            FontAwesomeIcon.Download => "Download",
            FontAwesomeIcon.FileDownload => "Local load",
            _ => null,
        });
        return result;
    }

    static string _undoName;
    static DateTime _lastTime;
    public static void UndoValue<T>(string name, ref T value, T @default, Action otherThing = null)
    {
        if (UndoValue(name))
        {
            otherThing?.Invoke();
            value = @default;
            Service.Config.Save();
        }
    }

    public static void UndoValue<T>(string name, ref T value1, T default1, ref T value2, T default2, Action otherThing = null)
    {
        if (UndoValue(name))
        {
            otherThing?.Invoke();
            value1 = default1;
            value2 = default2;
            Service.Config.Save();
        }
    }

    private static bool UndoValue(string name)
    {
        ImGui.SameLine();
        Spacing();

        bool isLast = name == _undoName && DateTime.Now - _lastTime < TimeSpan.FromSeconds(2);
        bool isTime = DateTime.Now - _lastTime > TimeSpan.FromSeconds(0.5);

        bool result = false;

        if (isLast) ImGui.PushStyleColor(ImGuiCol.Text, isTime ? ImGuiColors.HealerGreen : ImGuiColors.DPSRed);
        if (IconButton(isLast ? FontAwesomeIcon.Check : FontAwesomeIcon.Undo, $"#{name}: Undo",
            !isTime ? LocalizationManager.RightLang.ConfigWindow_Param_ResetToDefaultWait :
            isLast ? LocalizationManager.RightLang.ConfigWindow_Param_ResetToDefaultSure
            : LocalizationManager.RightLang.ConfigWindow_Param_ResetToDefault)
            )
        {
            if (isLast && isTime)
            {
                result = true;
                _lastTime = DateTime.MinValue;
            }
            else
            {
                _lastTime = DateTime.Now;
                _undoName = name;
            }
        }
        if (isLast) ImGui.PopStyleColor();
        return result;
    }

    public static void HoveredString(string text, Action selected = null)
    {
        if (ImGui.IsItemHovered())
        {
            ShowTooltip(text);

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                selected?.Invoke();
            }
        }
    }

    public static void ShowTooltip(string text) => ImguiTooltips.ShowTooltip(text);
    //{
    //    if (!Service.Config.ShowTooltips) return;
    //    if (!string.IsNullOrEmpty(text)) ImGui.SetTooltip(text);
    //}

    //public static bool HoveredStringReset(string text)
    //{
    //    if (ImGui.IsItemHovered())
    //    {
    //        text = string.IsNullOrEmpty(text)? LocalizationManager.RightLang.ConfigWindow_Param_ResetToDefault
    //        : text + "\n \n" + LocalizationManager.RightLang.ConfigWindow_Param_ResetToDefault;

    //        ShowTooltip(text);

    //        return ImGui.IsMouseDown(ImGuiMouseButton.Right)
    //        && ImGui.IsKeyPressed(ImGuiKey.LeftShift)
    //        && ImGui.IsKeyPressed(ImGuiKey.LeftCtrl);
    //    }
    //    return false;
    //}

    internal unsafe static bool DrawEditorList<T>(List<T> items, Action<T> draw)
    {
        ImGui.Indent();
        int moveFrom = -1, moveTo = -1;
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            ImGuiComponents.IconButton(item.GetHashCode(), FontAwesomeIcon.ArrowsAltV);

            ImGuiDragDropFlags src_flags = 0;
            src_flags |= ImGuiDragDropFlags.SourceNoDisableHover;     // Keep the source displayed as hovered
            src_flags |= ImGuiDragDropFlags.SourceNoHoldToOpenOthers; // Because our dragging is local, we disable the feature of opening foreign tree nodes/tabs while dragging
                                                                      //src_flags |= ImGuiDragDropFlags_SourceNoPreviewTooltip; // Hide the tooltip
            if (ImGui.BeginDragDropSource(src_flags))
            {
                ImGui.SetDragDropPayload("List Movement", (IntPtr)(&i), sizeof(int));
                ImGui.EndDragDropSource();
            }
            else if (ImGui.IsItemHovered())
            {
                ImGuiHelper.ShowTooltip(LocalizationManager.RightLang.ActionSequencer_DragdropDescription);

                if ((ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl))
                    && (ImGui.IsKeyDown(ImGuiKey.LeftAlt) || ImGui.IsKeyDown(ImGuiKey.RightAlt))
                    && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                {
                    moveFrom = i;
                }
            }

            if (ImGui.BeginDragDropTarget())
            {
                ImGuiDragDropFlags target_flags = 0;
                target_flags |= ImGuiDragDropFlags.AcceptBeforeDelivery;    // Don't wait until the delivery (release mouse button on a target) to do something
                                                                            //target_flags |= ImGuiDragDropFlags.AcceptNoDrawDefaultRect; // Don't display the yellow rectangle
                var ptr = ImGui.AcceptDragDropPayload("List Movement", target_flags);

                {
                    moveFrom = *(int*)ptr.Data;
                    moveTo = i;
                }
                ImGui.EndDragDropTarget();
            }

            ImGui.SameLine();

            draw?.Invoke(item);
        }

        bool result = false;
        if (moveFrom > -1)
        {
            //Move.
            if (moveTo > -1)
            {
                if (moveFrom != moveTo)
                {
                    var moveItem = items[moveFrom];
                    items.RemoveAt(moveFrom);

                    items.Insert(moveTo, moveItem);

                    result = true;
                }
            }
            //Remove.
            else
            {
                items.RemoveAt(moveFrom);
                result = true;
            }
        }

        ImGui.Unindent();
        return result;
    }

    internal static void DrawCondition(bool? tag)
    {
        if (!tag.HasValue)
        {
            ImGui.TextColored(ImGuiColors.DalamudGrey3, "Null");
        }
        else if (tag.Value)
        {
            ImGui.TextColored(ImGuiColors.HealerGreen, "True");
        }
        else
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "False");
        }
    }

    internal static void Spacing(byte count = 1)
    {
        string s = string.Empty;
        for (int i = 0; i < count; i++)
        {
            s += "    ";
        }
        ImGui.Text(s);
        ImGui.SameLine();
    }

    internal static void SetNextWidthWithName(string name)
    {
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(name).X + 30);
    }

    internal static void SearchCombo<T>(string popId, string name, ref string searchTxt, T[] data, Action<T> selectAction) where T : ITexture
    {
        if (ImGui.BeginCombo(popId, name, ImGuiComboFlags.HeightLargest))
        {
            SearchItems(ref searchTxt, data, selectAction);
            ImGui.EndCombo();
        }
    }

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> data, Action<T> selectAction) where T : ITexture
    {
        SearchItems(ref searchTxt, data, i => i.Name, selectAction, i => ImGui.Image(i.GetTexture().ImGuiHandle, new Vector2(24, 24)), texture => texture.Description);
    }

    internal static void SearchItemsReflection<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
        ImGui.SetNextItemWidth(Math.Max(80, ImGui.CalcTextSize(name).X + 30));

        if (ImGui.BeginCombo(popId, name, ImGuiComboFlags.HeightLargest))
        {
            SearchItems(ref searchTxt, actions, i => i.GetMemberName(), selectAction, getDesc: i => i.GetMemberDescription());

            ImGui.EndCombo();
        }
    }

    public static string GetMemberName(this MemberInfo info)
    {
        if (LocalizationManager.RightLang.MemberInfoName.TryGetValue(info.Name, out var memberName)) return memberName;

        return info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
    }

    private static string GetMemberDescription(this MemberInfo info)
    {
        if (LocalizationManager.RightLang.MemberInfoDesc.TryGetValue(info.Name, out var memberDesc)) return memberDesc;

        return info.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> actions, Func<T, string> getName, Action<T> selectAction, Action<T> extraDraw = null, Func<T, string> getDesc = null)
    {
        ImGui.Text(LocalizationManager.RightLang.ActionSequencer_SearchBar + ": ");
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("##SearchBar", ref searchTxt, 16);

        if (!string.IsNullOrWhiteSpace(searchTxt))
        {
            var src = searchTxt;
            var l = new Levenshtein();
            actions = actions.OrderBy(a => l.Distance(getName(a), src)).ToArray();
        }

        if (ImGui.BeginChild($"##ActionsCandidateList", new Vector2(200, 400), true))
        {
            foreach (var item in actions)
            {
                if (extraDraw != null)
                {
                    extraDraw(item);
                    ImGui.SameLine();
                }

                if (ImGui.Selectable(getName(item)))
                {
                    selectAction?.Invoke(item);

                    ImGui.CloseCurrentPopup();
                }

                if (getDesc != null && ImGui.IsItemHovered())
                {
                    var desc = getDesc(item);
                    ShowTooltip(desc);
                }
            }
            ImGui.EndChild();
        }
    }

    const float INDENT_WIDTH = 180;

    internal static void DisplayCommandHelp<T>(this T command, string extraCommand = "", Func<T, string> getHelp = null, bool sameLine = true) where T : struct, Enum
    {
        var cmdStr = command.GetCommandStr(extraCommand);

        if (ImGui.Button(cmdStr))
        {
            Svc.Commands.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImGuiHelper.ShowTooltip($"{LocalizationManager.RightLang.ConfigWindow_Helper_RunCommand}: {cmdStr}\n{LocalizationManager.RightLang.ConfigWindow_Helper_CopyCommand}: {cmdStr}");

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }

        var help = getHelp?.Invoke(command);

        if (!string.IsNullOrEmpty(help))
        {
            if (sameLine)
            {
                ImGui.SameLine();
                ImGui.Indent(INDENT_WIDTH);
            }
            else Spacing();
            ImGui.Text(" → ");
            ImGui.SameLine();
            ImGui.TextWrapped(help);
            if (sameLine)
            {
                ImGui.Unindent(INDENT_WIDTH);
            }
        }
    }

    public unsafe static void Display(this ICustomRotation rotation, ICustomRotation[] rotations, bool canAddButton)
        => rotation.DrawEnableTexture(canAddButton, null,
        text =>
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                var t = IconSet.GetTexture(IconSet.GetJobIcon(rotation, IconType.Framed));
                ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

                if (!string.IsNullOrEmpty(text.Description))
                {
                    ImGui.SameLine();
                    ImGui.Text("  ");
                    ImGui.SameLine();
                    ImGui.Text(text.Description);
                }

                var type = text.GetType();

                var attrs = new List<RotationDescAttribute> { RotationDescAttribute.MergeToOne(type.GetCustomAttributes<RotationDescAttribute>()) };

                foreach (var m in type.GetAllMethodInfo())
                {
                    attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
                }

                try
                {
                    foreach (var a in RotationDescAttribute.Merge(attrs))
                    {
                        RotationDescAttribute.MergeToOne(a)?.Display(text);
                    }
                }
                catch (Exception ex)
                {
                    var e = ex;
                    while(e != null)
                    {
                        ImGui.Text(e.Message);
                        e = e.InnerException;
                    }
                    ImGui.Text(ex.StackTrace);
                }
            });
        },
        showToolTip =>
        {
            if (!string.IsNullOrEmpty(rotation.RotationName) && rotations != null)
            {
                ImGui.SameLine();
                ImGui.TextDisabled("  -  ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(rotation.RotationName).X + 30);
                if (ImGui.BeginCombo("##RotationName:" + rotation.Name, rotation.RotationName))
                {
                    foreach (var r in rotations)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, r.GetColor());
                        if (ImGui.Selectable(r.RotationName))
                        {
                            Service.Config.RotationChoices[rotation.ClassJob.RowId] = r.GetType().FullName;
                            Service.Config.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            showToolTip?.Invoke(r);
                        }
                        ImGui.PopStyleColor();
                    }
                    ImGui.EndCombo();
                }
                HoveredString(LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation);
            }

            ImGui.SameLine();
            ImGui.TextDisabled("   -  ");
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Text, rotation.GetColor());
            ImGui.Text(rotation.GetType().Assembly.GetName().Name);
            if(!rotation.IsValid)
            {
                HoveredString(string.Format(LocalizationManager.RightLang.ConfigWindow_Rotation_InvalidRotation, 
                    rotation.GetType().Assembly.GetInfo().Author));
            }
            else if (rotation.IsBeta())
            {
                HoveredString(LocalizationManager.RightLang.ConfigWindow_Rotation_BetaRotation);
            }
            ImGui.PopStyleColor();

            ImGui.SameLine();
            ImGui.TextDisabled("  -  " + LocalizationManager.RightLang.ConfigWindow_Helper_GameVersion + ":  ");
            ImGui.SameLine();
            ImGui.Text(rotation.GameVersion);

            //if (rotation.Configs.Any())
            //{
            //    ImGui.SameLine();
            //    Spacing();
            //    if (IconButton(FontAwesomeIcon.Undo, $"#{rotation.GetHashCode()}Undo",
            //        LocalizationManager.RightLang.ConfigWindow_Rotation_ResetToDefault))
            //    {
            //        if (Service.Config.RotationsConfigurations.TryGetValue(rotation.ClassJob.RowId, out var jobDict)
            //            && jobDict.ContainsKey(rotation.GetType().FullName))
            //        {
            //            jobDict.Remove(rotation.GetType().FullName);
            //        }
            //    }
            //}

            var link = rotation.GetType().GetCustomAttribute<SourceCodeAttribute>();
            if(link != null)
            {
                ImGui.SameLine();
                Spacing();

                if (IconButton(FontAwesomeIcon.Code, "Code" + rotation.GetHashCode().ToString()))
                {
                    try
                    {
                        Util.OpenLink(link.Url);
                    }
                    catch
                    {
                    }
                }
            }

            HoveredString(LocalizationManager.RightLang.ConfigWindow_Helper_OpenSource);

            var attrs = rotation.GetType().GetCustomAttributes<LinkDescriptionAttribute>();
            if (attrs.Any())
            {
                var display = ImGui.GetIO().DisplaySize * 0.7f;

                bool isFirst = true;
                foreach (var texture in attrs)
                {
                    ImGui.SameLine();
                    if(isFirst)
                    {
                        isFirst = false;
                        Spacing();
                    }
                    var hasTexture = texture.Texture != null;

                    if (IconButton(hasTexture ? FontAwesomeIcon.Image : FontAwesomeIcon.Question,
                        "Button" + rotation.GetHashCode().ToString() + texture.GetHashCode().ToString()))
                    {
                        try
                        {
                            Util.OpenLink(texture.Path);
                        }
                        catch
                        {
                        }
                    }
                    if (ImGui.IsItemHovered() && (hasTexture || !string.IsNullOrEmpty( texture.Description)))
                    {
                        ImguiTooltips.ShowTooltip(() =>
                        {
                            if(hasTexture)
                            {
                                var ratio = Math.Min(1, Math.Min(display.X / texture.Texture.Width, display.Y / texture.Texture.Height));
                                var size = new Vector2(texture.Texture.Width * ratio,
                                    texture.Texture.Height * ratio);
                                ImGui.Image(texture.Texture.ImGuiHandle, size);
                            }
                            if (!string.IsNullOrEmpty(texture.Description))
                            {
                                ImGui.Text(texture.Description);
                            }

                        });
                    }
                }
            }
        }, () =>
        {
            RotationConfigWindow.DrawRotationRole(rotation, canAddButton);
        });

    #region IAction
    public static void Display(this IAction action, bool IsActive)
    {
        if (action is IBaseAction act)
        {
            act.Display(IsActive);
        }
        else if (action is IBaseItem item)
        {
            item.Display(IsActive);
        }
    }
    private unsafe static void Display(this IBaseAction action, bool IsActive) => action.DrawEnableTexture(IsActive, () =>
    {
        RotationConfigWindow.ActiveAction = action;
    }, showToolTip: a => 
    {
        if(!string.IsNullOrEmpty(a.Description)) ImGui.SetTooltip(a.Description);
    },otherThing: () =>
    {
        var enable = action.IsInCooldown;
        if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Action_ShowOnCDWindow}##{action.Name}InCooldown", ref enable))
        {
            action.IsInCooldown = enable;
            Service.Config.Save();
        }

        ImGui.SameLine();
        Spacing();

        OtherCommandType.DoActions.DisplayCommandHelp($"{action}-{5}",
       type => string.Format(LocalizationManager.RightLang.ConfigWindow_Helper_InsertCommand, action, 5), false);

        if (Service.Config.InDebug)
        {
            try
            {
#if DEBUG
                ImGui.Text("Is Real GCD: " + action.IsRealGCD.ToString());
                ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, action.AdjustedID).ToString());
                ImGui.Text("Cast Time: " + action.CastTime.ToString());
                ImGui.Text("MP: " + action.MPNeed.ToString());
#endif
                ImGui.Text("Has One:" + action.HasOneCharge.ToString());
                ImGui.Text("Recast One: " + action.RecastTimeOneChargeRaw.ToString());
                ImGui.Text("Recast Elapsed: " + action.RecastTimeElapsedRaw.ToString());

                var option = CanUseOption.IgnoreTarget | CanUseOption.IgnoreClippingCheck;
                ImGui.Text($"Can Use: {action.CanUse(out _, option)} ");
                ImGui.Text("Must Use:" + action.CanUse(out _, option | CanUseOption.MustUse).ToString());
                ImGui.Text("Empty Use:" + action.CanUse(out _, option | CanUseOption.EmptyOrSkipCombo).ToString());
                ImGui.Text("Must & Empty Use:" + action.CanUse(out _, option | CanUseOption.MustUseEmpty).ToString());
                if (action.Target != null)
                {
                    ImGui.Text("Target Name: " + action.Target.Name);
                }
            }
            catch
            {

            }
        }
    });

    public unsafe static void Display(this IBaseItem item, bool IsActive)
        => item.DrawEnableTexture(IsActive, () => RotationConfigWindow.ActiveAction = item,
    showToolTip: a =>
    {
        if (!string.IsNullOrEmpty(a.Description)) ImGui.SetTooltip(a.Description);
    }, otherThing: () =>
    {
        if (Service.Config.ShowCooldownWindow)
        {
            var enable = item.IsInCooldown;
            if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Action_ShowOnCDWindow}##{item.Name}InCooldown", ref enable))
            {
                item.IsInCooldown = enable;
                Service.Config.Save();
            }

            ImGui.SameLine();
            Spacing();
        }

        OtherCommandType.DoActions.DisplayCommandHelp($"{item}-{5}",
       type => string.Format(LocalizationManager.RightLang.ConfigWindow_Helper_InsertCommand, item, 5), false);


        if (Service.Config.InDebug)
        {
            ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Item, item.ID).ToString());
            ImGui.Text("Status HQ: " + ActionManager.Instance()->GetActionStatus(ActionType.Item, item.ID + 1000000).ToString());
            var remain = ActionManager.Instance()->GetRecastTime(ActionType.Item, item.ID) - ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, item.ID);
            ImGui.Text("remain: " + remain.ToString());
            ImGui.Text("CanUse: " + item.CanUse(out _).ToString());

            if(item is HealPotionItem healPotionItem)
            {
                ImGui.Text("MaxHP:" +  healPotionItem.MaxHealHp.ToString());
            }
        }
    });
    #endregion

    public static void DisplayMacro(this MacroInfo info)
    {
        ImGui.SetNextItemWidth(50);
        if (ImGui.DragInt($"{LocalizationManager.RightLang.ConfigWindow_Events_MacroIndex}##MacroIndex{info.GetHashCode()}",
            ref info.MacroIndex, 1, -1, 99))
        {
            Service.Config.Save();
        }

        ImGui.SameLine();
        Spacing();

        if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Events_ShareMacro}##ShareMacro{info.GetHashCode()}",
            ref info.IsShared))
        {
            Service.Config.Save();
        }
    }

    public static void DisplayEvent(this ActionEventInfo info)
    {
        if (ImGui.InputText($"{LocalizationManager.RightLang.ConfigWindow_Events_ActionName}##ActionName{info.GetHashCode()}",
            ref info.Name, 100))
        {
            Service.Config.Save();
        }

        info.DisplayMacro();
    }

    #region Rotation Config Display
    internal static void Draw(this IRotationConfigSet set, bool canAddButton)
    {
        foreach (var config in set.Configs)
        {
            if (config is RotationConfigCombo c) c.Draw(set, canAddButton);
            else if (config is RotationConfigBoolean b) b.Draw(set, canAddButton);
            else if (config is RotationConfigFloat f) f.Draw(set, canAddButton);
            else if (config is RotationConfigString s) s.Draw(set, canAddButton);
            else if (config is RotationConfigInt i) i.Draw(set, canAddButton);
        }
    }

    static void Draw(this RotationConfigCombo config, IRotationConfigSet set, bool canAddButton)
    {
        var val = set.GetCombo(config.Name);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(config.Items[val]).X + 50);
        var name = $"{config.DisplayName}##{config.GetHashCode()}_{config.Name}";
        if (ImGui.BeginCombo(name, config.Items[val]))
        {
            for (int comboIndex = 0; comboIndex < config.Items.Length; comboIndex++)
            {
                if (ImGui.Selectable(config.Items[comboIndex]))
                {
                    set.SetValue(config.Name, comboIndex.ToString());
                    Service.Config.Save();
                }
                if (canAddButton)
                {
                    ImGui.SameLine();
                    Spacing();
                    OtherCommandType.Rotations.DisplayCommandHelp(config.Name + " " + comboIndex.ToString());

                    ImGui.SameLine();
                    Spacing();
                    OtherCommandType.Rotations.DisplayCommandHelp(config.Name + " " + config.Items[comboIndex]);
                }
            }
            ImGui.EndCombo();
        }
        HoveredString(LocalizationManager.RightLang.ConfigWindow_Rotation_KeyName + ": " + config.Name);

        var @default = int.TryParse(config.DefaultValue, out var f) ? f : 0;
        if (val != @default)
        {
            UndoValue(name, ref val, @default);
        }

        if (canAddButton)
        {
            ImGui.SameLine();
            Spacing();
            OtherCommandType.Rotations.DisplayCommandHelp(config.Name);
        }
    }

    static void Draw(this RotationConfigBoolean config, IRotationConfigSet set, bool canAddButton)
    {
        bool val = set.GetBool(config.Name);

        RotationConfigWindow.DrawCheckBox($"{config.DisplayName}##{config.GetHashCode()}_{config.DisplayName}", ref val, bool.TryParse(config.DefaultValue, out var f) ? f : false, LocalizationManager.RightLang.ConfigWindow_Rotation_KeyName + ": " + config.Name, () =>
        {
            set.SetValue(config.Name, val.ToString());
        });

        if (canAddButton)
        {
            ImGui.SameLine();
            Spacing();
            OtherCommandType.Rotations.DisplayCommandHelp(config.Name);
        }
    }

    static void Draw(this RotationConfigFloat config, IRotationConfigSet set, bool canAddButton)
    {
        float val = set.GetFloat(config.Name);
        RotationConfigWindow.DrawFloatNumber($"{config.DisplayName}##{config.GetHashCode()}_{config.Name}", ref val, float.TryParse(config.DefaultValue, out var f) ? f : 0, config.Speed, config.Min, config.Max, otherThing: () =>
        {
            set.SetValue(config.Name, val.ToString());
        });
    }

    static void Draw(this RotationConfigString config, IRotationConfigSet set, bool canAddButton)
    {
        string val = set.GetString(config.Name);
        if (ImGui.InputText($"{config.DisplayName}##{config.GetHashCode()}_{config.Name}", ref val, 15))
        {
            set.SetValue(config.Name, val);
            Service.Config.Save();
        }
    }

    static void Draw(this RotationConfigInt config, IRotationConfigSet set, bool canAddButton)
    {
        int val = set.GetInt(config.Name);
        RotationConfigWindow.DrawIntNumber($"{config.DisplayName}##{config.GetHashCode()}_{config.Name}", ref val, int.TryParse(config.DefaultValue, out var f) ? f : 0, config.Speed, config.Min, config.Max, otherThing: () =>
        {
            set.SetValue(config.Name, val.ToString());
        });
    }
    #endregion


    static readonly Vector2 PIC_SIZE = new(24, 24);
    const float ATTR_INDENT = 170;

    public static void Display(this RotationDescAttribute attr, ICustomRotation rotation)
    {
        var acts = rotation.AllBaseActions;

        var allActions = attr.Actions.Select(i => acts.FirstOrDefault(a => a.ID == (uint)i))
            .Where(i => i != null);

        bool hasDesc = !string.IsNullOrEmpty(attr.Description);

        if (!hasDesc && !allActions.Any()) return;
        ImGui.Separator();

        ImGui.Image(IconSet.GetTexture(attr.IconID).ImGuiHandle, PIC_SIZE);
        ImGui.SameLine();

        var isOnCommand = attr.IsOnCommand;
        if (isOnCommand) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
        ImGui.Text(" " + attr.Type.ToName());
        if (isOnCommand) ImGui.PopStyleColor();

        ImGui.SameLine();

        ImGui.Indent(ATTR_INDENT);

        if (hasDesc)
        {
            ImGui.Text(attr.Description);
        }

        bool notStart = false;
        foreach (var item in allActions)
        {
            if (item == null) continue;

            if (notStart)
            {
                ImGui.SameLine();
                ImGui.Text(" ");
                ImGui.SameLine();
            }

            ImGui.Image(item.GetTexture().ImGuiHandle, PIC_SIZE);
            notStart = true;
        }
        ImGui.Unindent(ATTR_INDENT);
    }

    public unsafe static ImFontPtr GetFont(float size)
    {
        var style = new Dalamud.Interface.GameFonts.GameFontStyle(Dalamud.Interface.GameFonts.GameFontStyle.GetRecommendedFamilyAndSize(Dalamud.Interface.GameFonts.GameFontFamily.Axis, size));
        var font = Svc.PluginInterface.UiBuilder.GetGameFontHandle(style).ImFont;

        if((IntPtr)font.NativePtr == IntPtr.Zero) 
        {
            return ImGui.GetFont();
        }
        font.Scale = size / style.BaseSizePt;
        return font;
    }
}
