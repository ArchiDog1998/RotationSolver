using ImGuiNET;
using System.Numerics;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawEvent()
    {
        if (ImGui.Button("添加事件"))
        {
            Service.Configuration.Events.Add(new ActionEventInfo());
        }
        ImGui.SameLine();
        Spacing();
        ImGui.Text("在这个窗口，你可以设定一些技能释放后，使用什么宏。");

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


        if (ImGui.BeginChild("事件列表", new Vector2(0f, -1f), true))
        {
            for (int i = 0; i < Service.Configuration.Events.Count; i++)
            {
                string name = Service.Configuration.Events[i].Name;
                if (ImGui.InputText("技能名称" + i.ToString(), ref name, 50))
                {
                    Service.Configuration.Events[i].Name = name;
                    Service.Configuration.Save();
                }

                int macroindex = Service.Configuration.Events[i].MacroIndex;
                if (ImGui.DragInt("宏编号" + i.ToString(), ref macroindex, 1, 0, 99))
                {
                    Service.Configuration.Events[i].MacroIndex = macroindex;
                }

                bool isShared = Service.Configuration.Events[i].IsShared;
                if (ImGui.Checkbox("共享宏" + i.ToString(), ref isShared))
                {
                    Service.Configuration.Events[i].IsShared = isShared;
                    Service.Configuration.Save();
                }

                ImGui.SameLine();
                Spacing();
                if (ImGui.Button("删除事件" + i.ToString()))
                {
                    Service.Configuration.Events.RemoveAt(i);
                }
                ImGui.Separator();
            }
            ImGui.EndChild();
        }
        ImGui.PopStyleVar();

    }
}
