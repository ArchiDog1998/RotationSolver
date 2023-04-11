using Dalamud.Utility;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System.Diagnostics;
using System.Xml.Linq;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    private void DrawRotationDevTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_RotationDev_Description);

        if (ImGui.Button("Load Rotations"))
        {
            RotationUpdater.GetAllCustomRotations();
        }

        ImGui.SameLine();
        if (ImGui.Button("Dev Wiki"))
        {
            Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
        }

        ImGui.SameLine();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InDebug,
            ref Service.Config.InDebug, Service.Default.InDebug);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Rotation Devs"))
        {
            if (ImGui.BeginTabItem("Libs"))
            {
                DrawThridLibs();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Infos"))
            {
                DrawAssemblyInfos();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawThridLibs()
    {

        if (ImGui.BeginChild("Third-party Libs", new Vector2(0f, -1f), true))
        {
            if (ImGui.Button("AddOne"))
            {
                Service.Config.OtherLibs = Service.Config.OtherLibs.Append(string.Empty).ToArray();
            }
            ImGui.SameLine();
            ImGui.Text("Third-party Rotation Libraries");

            int removeIndex = -1;
            for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
            {
                if (ImGui.InputText($"##OtherLib{i}", ref Service.Config.OtherLibs[i], 1024))
                {
                    Service.Config.Save();
                }
                ImGui.SameLine();
                if (ImGui.Button($"X##Remove{i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = Service.Config.OtherLibs.ToList();
                list.RemoveAt(removeIndex);
                Service.Config.OtherLibs = list.ToArray();
                Service.Config.Save();
            }

            ImGui.EndChild();
        }
    }
    private void DrawAssemblyInfos()
    {
        var assemblies = RotationUpdater.CustomRotationsDict
            .SelectMany(d => d.Value)
            .SelectMany(g => g.rotations)
            .Select(r => r.GetType().Assembly)
            .ToHashSet();

        if(ImGui.BeginTable("AssemblyTable", 2))
        {
            foreach (var assembly in assemblies)
            {
                ImGui.TableNextRow();
                if (ImGui.Button(assembly.GetName().Name + assembly.GetName().Version))
                {
                    if (!RotationLoadContext.AssemblyPaths.TryGetValue(assembly.GetName().Name, out var path))
                        path = assembly.Location;

                    Process.Start("explorer.exe", path);
                }

                ImGui.TableNextColumn();
                ImGui.Text(assembly.GetAuthor());
            }
            ImGui.EndTable();
        }
    }
}
