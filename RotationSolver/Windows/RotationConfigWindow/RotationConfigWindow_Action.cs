using ImGuiNET;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow
{
    internal partial class RotationConfigWindow
    {
        private void DrawActionTab()
        {
            ImGui.Text(LocalizationManager.RightLang.ConfigWindow_ActionItem_Description);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


            if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
            {
                foreach (var pair in IconReplacer.RightComboBaseActions.GroupBy(a => a.CateName).OrderBy(g => g.Key))
                {
                    if (ImGui.CollapsingHeader(pair.Key))
                    {
                        foreach (var item in pair)
                        {
                            DrawAction(item);
                            ImGui.Separator();
                        }
                    }
                }
                ImGui.EndChild();
            }
            ImGui.PopStyleVar();
        }
    }
}
