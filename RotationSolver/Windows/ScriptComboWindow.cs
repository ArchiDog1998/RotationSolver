using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Newtonsoft.Json;
using RotationSolver.Combos.Script;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RotationSolver.Windows;

internal class ScriptComboWindow : Window
{
    IScriptCombo _targetCombo;
    public IScriptCombo TargetCombo
    {
        get => _targetCombo;
        set
        {
            _targetCombo = value;
            ActiveSet = null;
        }
    }

    IDraw _activeSet;
    public IDraw ActiveSet
    {
        get => _activeSet;
        set
        {
            _activeSet = value;
            ActiveAction = null;
        }
    }

    public IDraw ActiveAction { get; set; }



    public ScriptComboWindow()
        : base(nameof(ScriptComboWindow), 0, false)
    {
        Size = new Vector2(525, 600);
        SizeCondition = ImGuiCond.FirstUseEver;
        RespectCloseHotkey = false;
    }
    public override void Draw()
    {
        ImGui.Columns(3);

        DisplayFunctionList();

        ImGui.NextColumn();

        DisplayActionList();

        ImGui.NextColumn();

        DisplayConditionList();

        ImGui.Columns(1);
    }

    private void DisplayFunctionList()
    {
        if (TargetCombo == null) return;

        var text = TargetCombo.GetTexture();

        ImGui.Image(text.ImGuiHandle, new Vector2(text.Width, text.Height));

        ImGui.SameLine();

        ImGui.Text($"{LocalizationManager.RightLang.Scriptwindow_Author}: ");

        ImGui.SameLine();

        string authorName = TargetCombo.Set.AuthorName;
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(authorName).X + 10);
        if (ImGui.InputText($"##{TargetCombo.Name}_Author", ref authorName, 32, ImGuiInputTextFlags.AutoSelectAll))
        {
            TargetCombo.Set.AuthorName = authorName;
        }


        ImGui.SameLine();

        ImGui.Text($"  {LocalizationManager.RightLang.Configwindow_Helper_GameVersion}: ");

        ImGui.SameLine();

        string gameVersion = TargetCombo.Set.GameVersion;
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(gameVersion).X + 10);
        if (ImGui.InputText($"##{TargetCombo.Name}_Version", ref gameVersion, 32, ImGuiInputTextFlags.AutoSelectAll))
        {
            TargetCombo.Set.GameVersion = gameVersion;
        }

        ImGui.SameLine();


        if (ImGuiComponents.IconButton(FontAwesomeIcon.Folder))
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = $" /select, {TargetCombo.Set.GetFolder()}";
            p.Start();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_OpenSourceFile);
        }

        ImGui.SameLine();

        if (ImGuiComponents.IconButton(FontAwesomeIcon.Save))
        {
            File.WriteAllText(TargetCombo.Set.GetFolder(), JsonConvert.SerializeObject(TargetCombo.Set, Formatting.Indented));
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(LocalizationManager.RightLang.Scriptwindow_Save);
        }

        TargetCombo.Set.Draw(TargetCombo);
    }

    private void DisplayActionList()
    {
        ActiveSet?.Draw(TargetCombo);
    }

    private void DisplayConditionList()
    {
        ActiveAction?.Draw(TargetCombo);
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

    internal static void AddPopup<T>(string popId, string special, Action act, ref string searchTxt, IEnumerable<T> actions, Action<T> selectAction) where T : ITexture
    {
        if (ImGuiComponents.IconButton(popId.GetHashCode(), FontAwesomeIcon.Plus))
        {
            ImGui.OpenPopup(popId);
        }

        if (ImGui.BeginPopup(popId))
        {
            if (!string.IsNullOrWhiteSpace(special))
            {
                if (ImGui.Selectable(special))
                {
                    act?.Invoke();
                    ImGui.CloseCurrentPopup();
                }

                ImGui.Separator();
            }

            SearchItems(ref searchTxt, actions, selectAction);

            ImGui.EndPopup();
        }
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

    internal static void AddPopup<T>(string popId, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
        if (ImGuiComponents.IconButton(popId.GetHashCode(), FontAwesomeIcon.Plus))
        {
            ImGui.OpenPopup(popId);
        }

        if (ImGui.BeginPopup(popId))
        {
            SearchItems(ref searchTxt, actions, i => i.GetMemberName(), selectAction, getDesc: i => i.GetMemberDescription());

            ImGui.EndPopup();
        }
    }

    internal static void SearchItemsReflection<T>(string popId, string name, ref string searchTxt, T[] actions, Action<T> selectAction) where T : MemberInfo
    {
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
}
