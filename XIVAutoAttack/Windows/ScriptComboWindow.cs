using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Combos.Script;
using XIVAutoAttack.Combos.Script.Actions;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Windows
{
    internal class ScriptComboWindow : Window
    {
        public IScriptCombo TargetCombo { get; set; }

        public ActionConditions ActiveAction { get; set; }
        public ActionsSet ActiveSet { get; set; }

        public ScriptComboWindow()
            : base("自定义循环设置 v" + typeof(ScriptComboWindow).Assembly.GetName().Version.ToString(), 0, false)
        {
            Size = new Vector2(525, 600);
            SizeCondition = ImGuiCond.FirstUseEver;
            RespectCloseHotkey = false;
        }
        public override void Draw()
        {
            ImGui.Columns(3);
            //ImGui.SetColumnWidth(0, 260);
            this.DisplayFunctionList();

            ImGui.NextColumn();

            this.DisplayActionList();

            ImGui.NextColumn();

            this.DisplayConditionList();

            ImGui.Columns(1);
        }

        private void DisplayFunctionList()
        {
            if (TargetCombo == null) return;

            if (ImGui.BeginTable("MyTable", 2))
            {
                ImGui.TableNextColumn();

                var text = TargetCombo.GetTexture();

                ImGui.Image(text.ImGuiHandle, new Vector2(text.Width, text.Height));

                ImGui.TableNextColumn();

                ImGui.Text("作者:  ");
                ImGui.SameLine();


                string authorName = TargetCombo.Set.AuthorName;
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(authorName).X + 10);
                if (ImGui.InputText($"##{TargetCombo.Name}作者", ref authorName, 32))
                {
                    TargetCombo.Set.AuthorName = authorName;
                }

                ImGui.Text("描述:");

                ImGui.SameLine();
                
                ComboConfigWindow.ComboConfigWindow.Spacing();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Folder))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "explorer.exe";
                    p.StartInfo.Arguments = $" /select, {TargetCombo.Set.GetFolder()}";
                    p.Start();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("打开源文件");
                }

                ImGui.SameLine();
                ComboConfigWindow.ComboConfigWindow.Spacing();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Save))
                {
                    File.WriteAllText(TargetCombo.Set.GetFolder(), JsonConvert.SerializeObject(TargetCombo.Set, Formatting.Indented, new JsonSerializerSettings()
                    {
                         TypeNameHandling = TypeNameHandling.All,
                    }));
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("保存修改");
                }


                ImGui.EndTable();
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

        internal static void AddPopup<T>(string popId, string special, Action act, ref string searchTxt, T[] actions, Action<T> selectAction) where T : ITexture
        {
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
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

        internal static bool DrawEditorList<T>(List<T> items, Action<T> draw)
        {
            int index = -1;
            int type = -1;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (ImGuiComponents.IconButton(item.GetHashCode(), FontAwesomeIcon.ArrowsAltV))
                {
                    type = 0;
                    index = i;
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("左键上移，右键下移动，ctrl + alt + 中键删除。");

                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                    {
                        type = 1;
                        index = i;
                    }

                    if( (ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl))
                        && (ImGui.IsKeyDown(ImGuiKey.LeftAlt) || ImGui.IsKeyDown(ImGuiKey.RightAlt))
                        && ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
                    {
                        type = 2;
                        index = i;
                    }
                }

                ImGui.SameLine();

                draw?.Invoke(item);

                ImGui.Separator();
            }
            switch (type)
            {
                case 0:
                    if (index > 0)
                    {
                        var item = items[index];
                        items.RemoveAt(index);
                        items.Insert(index - 1, item);
                    }
                    break;

                case 1:

                    if (index < items.Count - 1)
                    {
                        var item = items[index];
                        items.RemoveAt(index);
                        items.Insert(index + 1, item);
                    }
                    break;

                case 2:
                    items.RemoveAt(index);
                    break;
            }
            return index != -1;
        }

        internal static void SearchItems<T>(ref string searchTxt, T[] actions, Action<T> selectAction) where T : ITexture
        {
            ImGui.Text("搜索框：");
            ImGui.SetNextItemWidth(150);
            ImGui.InputText("##搜索框", ref searchTxt, 16);

            if (!string.IsNullOrWhiteSpace(searchTxt))
            {
                var src = searchTxt;
                actions = actions.OrderBy(a => !a.Name.Contains(src)).ToArray();
            }

            if (ImGui.BeginChild($"##技能候选列表", new Vector2(150, 400), true))
            {
                foreach (var item in actions)
                {
                    ImGui.Image(item.GetTexture().ImGuiHandle,
                        new Vector2(24, 24));

                    ImGui.SameLine();
                    if (ImGui.Selectable(item.Name))
                    {
                        selectAction?.Invoke(item);

                        ImGui.CloseCurrentPopup();
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
}
