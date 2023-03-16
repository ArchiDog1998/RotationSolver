using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Basic.Rotations;
using RotationSolver.Localization;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;

namespace RotationSolver.UI;

internal static class ImGuiHelper
{
    const ImGuiWindowFlags TOOLTIP_FLAG =
      ImGuiWindowFlags.Tooltip |
      ImGuiWindowFlags.NoMove |
      ImGuiWindowFlags.NoSavedSettings |
      ImGuiWindowFlags.NoBringToFrontOnFocus |
      ImGuiWindowFlags.NoDecoration |
      ImGuiWindowFlags.NoInputs |
      ImGuiWindowFlags.AlwaysAutoResize;

    public static void DrawTooltip(Action act, string id)
    {
        if (act == null) return;
        ImGui.SetWindowPos(id, ImGui.GetIO().MousePos);
        if (ImGui.Begin(id, TOOLTIP_FLAG))
        {
            act();
            ImGui.End();
        }
    }
    public static void DrawEnableTexture<T>(this T texture, bool isSelected, Action selected,
        Action<string> showToolTip = null, Action<Action<string>> additionalHeader = null,
        Action otherThing = null) where T : class, ITexture
    {
        showToolTip ??= text =>
        {
            if (!string.IsNullOrEmpty(text)) ImGui.SetTooltip(text);
        };

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        var t = texture.GetTexture();

        ImGui.SetColumnWidth(0, t.Width + 5);

        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

        var desc = texture?.Description;
        if (ImGui.IsItemHovered())
        {
            showToolTip(desc);
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                selected?.Invoke();
            }
        }
        ImGui.NextColumn();

        bool enable = false;

        if (isSelected) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
        enable = texture.IsEnabled;
        if (ImGui.Checkbox($"{texture.Name}##{texture.Name}", ref enable))
        {
            texture.IsEnabled = enable;
            Service.Config.Save();
        }
        if (isSelected) ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            showToolTip(desc);
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

    public static bool IconButton(FontAwesomeIcon icon, string name)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        var result = ImGui.Button($"{icon.ToIconString()}##{name}");
        ImGui.PopFont();
        return result;

        //ImGuiComponents.IconButton(icon)
    }

    public static void HoveredString(string text, Action selected = null)
    {
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(text)) ImGui.SetTooltip(text);
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                selected?.Invoke();
            }
        }
    }

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
                ImGui.SetTooltip(LocalizationManager.RightLang.Timeline_DragdropDescription);

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

    internal static void SearchCombo<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : ITexture
    {
        if (ImGui.BeginCombo(popId, name, ImGuiComboFlags.HeightLargest))
        {
            SearchItems(ref searchTxt, actions, selectAction);

            ImGui.EndCombo();
        }
    }

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> actions, Action<T> selectAction) where T : ITexture
    {
        SearchItems(ref searchTxt, actions, i => i.Name, selectAction, i => ImGui.Image(i.GetTexture().ImGuiHandle, new Vector2(24, 24)));
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
        ImGui.Text(LocalizationManager.RightLang.Timeline_SearchBar + ": ");
        ImGui.SetNextItemWidth(150);
        ImGui.InputText("##SearchBar", ref searchTxt, 16);

        if (!string.IsNullOrWhiteSpace(searchTxt))
        {
            var src = searchTxt;
            actions = actions.OrderBy(a => !getName(a).Contains(src)).ToArray();
        }

        if (ImGui.BeginChild($"##ActionsCandidateList", new Vector2(150, 400), true))
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
                    if (!string.IsNullOrEmpty(desc))
                    {
                        ImGui.SetTooltip(desc);
                    }
                }
            }
            ImGui.EndChild();
        }
    }

    const float INDENT_WIDTH = 180;

    internal static void DisplayCommandHelp<T>(this T command, string extraCommand = "", Func<T, string> getHelp = null, bool sameLine = true) where T : struct, Enum
    {
        var cmdStr = Service.Command + " " + command.ToString();
        if (!string.IsNullOrEmpty(extraCommand))
        {
            cmdStr += " " + extraCommand;
        }

        if (ImGui.Button(cmdStr))
        {
            Service.CommandManager.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"{LocalizationManager.RightLang.ConfigWindow_Helper_RunCommand}: {cmdStr}\n{LocalizationManager.RightLang.ConfigWindow_Helper_CopyCommand}: {cmdStr}");

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
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
            ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(1000, 1500));
            DrawTooltip(() =>
            {
                var t = IconSet.GetTexture(IconSet.GetJobIcon(rotation, IconType.Framed));
                ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

                if (!string.IsNullOrEmpty(text))
                {
                    ImGui.SameLine();
                    ImGui.Text("  ");
                    ImGui.SameLine();
                    ImGui.Text(text);
                }

                var type = rotation.GetType();

                var attrs = new List<RotationDescAttribute> { RotationDescAttribute.MergeToOne(type.GetCustomAttributes<RotationDescAttribute>()) };

                foreach (var m in type.GetAllMethodInfo())
                {
                    attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
                }

                try
                {
                    foreach (var a in RotationDescAttribute.Merge(attrs))
                    {
                        RotationDescAttribute.MergeToOne(a)?.Display(rotation);
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
                    ImGui.TextWrapped(ex.StackTrace);
                }
            }, "Popup" + rotation.GetHashCode().ToString());
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
                        if (ImGui.Selectable(r.RotationName))
                        {
                            Service.Config.RotationChoices[rotation.Job.RowId] = r.RotationName;
                            Service.Config.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            showToolTip?.Invoke(r.Description);
                        }
                    }
                    ImGui.EndCombo();
                }
                HoveredString(LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation);
            }

            ImGui.SameLine();
            ImGui.TextDisabled("   -  ");
            ImGui.SameLine();

            var isAllowed = rotation.IsAllowed(out _);
            ImGui.TextColored(isAllowed ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudViolet,
                rotation.GetAuthor());
            if (!isAllowed)
                HoveredString("This rotation is not allowed to be used in High-end Duty!");
            
            ImGui.SameLine();
            ImGui.TextDisabled("  -  " + LocalizationManager.RightLang.ConfigWindow_Helper_GameVersion + ":    ");
            ImGui.SameLine();
            ImGui.Text(rotation.GameVersion);

            var link = rotation.GetType().GetCustomAttribute<SourceCodeAttribute>();
            if(link != null)
            {
                ImGui.SameLine();
                Spacing();

                if (IconButton(FontAwesomeIcon.Globe, "Code" + rotation.GetHashCode().ToString()))
                {
                    var url = link.Url;
                    Util.OpenLink(url);
                }
            }

            HoveredString(LocalizationManager.RightLang.ConfigWindow_Helper_OpenSource);

            var attrs = rotation.GetType().GetCustomAttributes<LinkDescriptionAttribute>();
            if (attrs.Any())
            {
                ImGui.SameLine();
                Spacing();

                foreach (var texture in attrs)
                {
                    if (IconButton(FontAwesomeIcon.Question,
                        "Button" + texture.GetHashCode().ToString()))
                    {
                        Util.OpenLink(texture.Path);
                    }
                    //if (ImGui.IsItemHovered() && texture.Texture != null)
                    //{
                    //    ImGuiHelper.DrawTooltip(() =>
                    //    {
                    //        var ratio = Math.Min(1, 1000f /       texture.Texture.Width);
                    //        var size = new Vector2(texture.Texture.Width *      ratio,
                    //            texture.Texture.Height * ratio);
                    //        ImGui.Image(texture.Texture.ImGuiHandle, size);
                    //    }, "Picture" + texture.GetHashCode().ToString());
                    //}
                }
            }
        }, () =>
        {
            RotationConfigWindow.DrawRotationRole(rotation);

            rotation.Configs.Draw(canAddButton);
        });

    #region IAction
    public static void Display(this IAction action, bool IsActive)
    {
        if (action is BaseAction act)
        {
            act.Display(IsActive);
        }
        else if (action is IBaseItem item)
        {
            item.Display(IsActive);
        }
    }
    private unsafe static void Display(this BaseAction action, bool IsActive) => action.DrawEnableTexture(IsActive, () =>
    {
        if (action.IsTimeline) RotationConfigWindow.ActiveAction = action;
    }, otherThing: () =>
    {
        ImGui.SameLine();
        Spacing();

        OtherCommandType.ToggleActions.DisplayCommandHelp(action.ToString());

        if (action.IsTimeline) OtherCommandType.DoActions.DisplayCommandHelp($"{action}-{5}",
           type => string.Format(LocalizationManager.RightLang.ConfigWindow_Helper_InsertCommand, action), false);

        if (Service.Config.InDebug)
        {
            try
            {
                ImGui.Text("Can Target: " + action.CanUseTo(action.Target));
                ImGui.Text("Have One:" + action.HaveOneCharge.ToString());
                ImGui.Text("Is Real GCD: " + action.IsRealGCD.ToString());
                ImGui.Text("Recast One: " + action.RecastTimeOneCharge.ToString());
                ImGui.Text("Recast Elapsed: " + action.RecastTimeElapsed.ToString());
                ImGui.Text("Recast Remain: " + action.RecastTimeRemain.ToString());
                ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, action.AdjustedID).ToString());

                ImGui.Text("Cast Time: " + action.CastTime.ToString());
                ImGui.Text("MP: " + action.MPNeed.ToString());
                ImGui.Text($"Can Use: {action.CanUse(out _)} ");
                ImGui.Text("Must Use:" + action.CanUse(out _, mustUse: true).ToString());
                ImGui.Text("Empty Use:" + action.CanUse(out _, emptyOrSkipCombo: true).ToString());
                ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(action.AdjustedID).ToString());
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

    public unsafe static void Display(this IBaseItem item, bool IsActive) => item.DrawEnableTexture(false, null, otherThing: () =>
    {
        if (Service.Config.InDebug)
        {
            ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Item, item.ID).ToString());
            var remain = ActionManager.Instance()->GetRecastTime(ActionType.Item, item.ID) - ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, item.ID);
            ImGui.Text("remain: " + remain.ToString());
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
    static void Draw(this IRotationConfigSet set, bool canAddButton)
    {
        foreach (var config in set.Configs)
        {
            if (config is RotationConfigCombo c) c.Draw(set, canAddButton);
            else if (config is RotationConfigBoolean b) b.Draw(set, canAddButton);
            else if (config is RotationConfigFloat f) f.Draw(set, canAddButton);
            else if (config is RotationConfigString s) s.Draw(set, canAddButton);
        }
    }

    static void Draw(this RotationConfigCombo config, IRotationConfigSet set, bool canAddButton)
    {
        var val = set.GetCombo(config.Name);
        if (ImGui.BeginCombo($"{config.DisplayName}##{config.GetHashCode()}_{config.Name}", config.Items[val]))
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
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.ConfigWindow_Rotation_KeyName + ": " + config.Name);
        }

        //显示可以设置的按键
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
        if (ImGui.Checkbox($"{config.DisplayName}##{config.GetHashCode()}_{config.DisplayName}", ref val))
        {
            set.SetValue(config.Name, val.ToString());
            Service.Config.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.ConfigWindow_Rotation_KeyName + ": " + config.Name);
        }

        //显示可以设置的案件
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
        ImGui.SetNextItemWidth(100);
        if (ImGui.DragFloat($"{config.DisplayName}##{config.GetHashCode()}_{config.Name}", ref val, config.Speed, config.Min, config.Max))
        {
            set.SetValue(config.Name, val.ToString());
            Service.Config.Save();
        }
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
    #endregion


    static readonly Vector2 PIC_SIZE = new Vector2(24, 24);
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
}
