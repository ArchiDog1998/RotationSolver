using Dalamud.Interface.Components;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos.Script;
using XIVAutoAttack.Combos.Script.Actions;
using XIVAutoAttack.Data;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System.IO;

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

            ImGui.Columns(2);

            var text = TargetCombo.GetTexture();

            ImGui.SetColumnWidth(0, text.Width + 5);

            ImGui.Image(text.ImGuiHandle, new Vector2(text.Width, text.Height));

            ImGui.NextColumn();

            string authorName = TargetCombo.Set.AuthorName;
            if (ImGui.InputText("作者:", ref authorName, 64))
            {
                TargetCombo.Set.AuthorName = authorName;
            }

            if (ImGuiComponents.IconButton(FontAwesomeIcon.Folder))
            {
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = $" /select, {TargetCombo.Set.GetFolder()}";
                p.Start();
            }

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            if (ImGuiComponents.IconButton(FontAwesomeIcon.Save))
            {
                File.WriteAllText(TargetCombo.Set.GetFolder(), JsonConvert.SerializeObject(TargetCombo.Set));
            }

            ImGui.Columns(1);

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
    }
}
