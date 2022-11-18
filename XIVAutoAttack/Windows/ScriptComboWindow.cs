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

namespace XIVAutoAttack.Windows
{
    internal class ScriptComboWindow : Window
    {
        public IScriptCombo TargetCombo { get; set; }

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
            string authorName = TargetCombo.AuthorName;
            if (ImGui.InputText("作者:", ref authorName, 64))
            {
                TargetCombo.AuthorName = authorName;
            }

            ImGui.SameLine();
            ComboConfigWindow.Spacing();

            if (ImGuiComponents.IconButton(FontAwesomeIcon.Folder))
            {
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = $" /select, {TargetCombo.GetFolder()}";
                p.Start();
            }
        }

        private void DisplayActionList()
        {
        }

        private void DisplayConditionList()
        {
        }
    }
}
