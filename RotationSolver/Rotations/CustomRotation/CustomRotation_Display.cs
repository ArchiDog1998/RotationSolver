using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Windows.RotationConfigWindow;

namespace RotationSolver.Rotations.CustomRotation
{
    internal abstract partial class CustomRotation
    {
        public unsafe void Display(ICustomRotation[] rotations, bool canAddButton) => this.DrawEnableTexture(canAddButton, null, () =>
        {
            if (!string.IsNullOrEmpty(RotationName) && rotations != null)
            {
                ImGui.SameLine();
                ImGui.TextDisabled("  -  ");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(RotationName).X + 30);
                if (ImGui.BeginCombo("##RotationName:" + Name, RotationName))
                {
                    foreach (var r in rotations)
                    {
                        if (ImGui.Selectable(r.RotationName))
                        {
                            Service.Configuration.ComboChoices[Job.RowId] = r.RotationName;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(r.Description);
                        }
                    }
                    ImGui.EndCombo();
                }
                ImGuiHelper.HoveredString(LocalizationManager.RightLang.Configwindow_Helper_SwitchRotation);
            }

            ImGui.SameLine();
            ImGui.TextDisabled("   -  " + LocalizationManager.RightLang.Configwindow_Helper_GameVersion + ":  ");
            ImGui.SameLine();
            ImGui.Text(GameVersion);
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGuiHelper.IconButton(FontAwesomeIcon.Globe, GetHashCode().ToString()))
            {
                var url = @"https://github.com/ArchiDog1998/RotationSolver/blob/main/" + GetType().FullName.Replace(".", @"/") + ".cs";

                Util.OpenLink(url);
            }
            ImGuiHelper.HoveredString(LocalizationManager.RightLang.Configwindow_Helper_OpenSource);
        }, () =>
        {
            RotationConfigWindow.DrawRotationRole(this);

            Configs.Draw(canAddButton);
        });
    }
}
