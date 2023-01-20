using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Helpers;

internal static class ImGuiHelper
{
    public static void DrawEnableTexture<T>(this T texture, bool isSelected, Action selected, 
        Action additonalHeader = null, Action otherThing = null) where T : class, ITexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        var t = texture.GetTexture();
        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

        var able = texture as IEnable;

        var desc = able?.Description;
        HoveredString(desc);
        ImGui.SameLine();
        Spacing();


        if(ImGui.Selectable(texture.Name, isSelected))
        {
            selected?.Invoke();
        }
        HoveredString(desc);

        bool enable = false;
        if (able != null)
        {
            ImGui.SameLine();
            Spacing();

            enable = able.IsEnabled;
            if (ImGui.Checkbox("##" + texture.Name, ref enable))
            {
                able.IsEnabled = enable;
                Service.Configuration.Save();
            }
            HoveredString(desc);
        }


        additonalHeader?.Invoke();

        if (enable)
        {
            ImGui.Indent(t.Width);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
            otherThing?.Invoke();
            ImGui.PopStyleVar();
            ImGui.Unindent();
        }

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

    public  static void HoveredString(string text)
    {
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(text)) ImGui.SetTooltip(text);
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
            src_flags |= ImGuiDragDropFlags.SourceNoHoldToOpenOthers; // Because our dragging is local, we disable the feature of opening foreign treenodes/tabs while dragging
                                                                      //src_flags |= ImGuiDragDropFlags_SourceNoPreviewTooltip; // Hide the tooltip
            if (ImGui.BeginDragDropSource(src_flags))
            {
                ImGui.SetDragDropPayload("List Movement", (IntPtr)(&i), sizeof(int));
                ImGui.EndDragDropSource();
            }
            else if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_DragdropDescription);

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

    internal static void SearchItems<T>(ref string searchTxt, IEnumerable<T> actions, Func<T, string> getName, Action<T> selectAction, Action<T> extraDraw = null, Func<T, string> getDesc = null)
    {
        ImGui.Text(LocalizationManager.RightLang.Scriptwindow_SearchBar + ": ");
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
}
