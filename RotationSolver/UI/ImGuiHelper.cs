using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ECommons.DalamudServices;
using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System.ComponentModel;

namespace RotationSolver.UI;

internal static class ImGuiHelper
{
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
        SearchItems(ref searchTxt, data, i => i.Name, selectAction, i =>
        {
            if(i.GetTexture(out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, new Vector2(24, 24));
            }
        }, texture => texture.Description);
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


    static readonly Vector2 PIC_SIZE = new(24, 24);
    const float ATTR_INDENT = 170;


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
