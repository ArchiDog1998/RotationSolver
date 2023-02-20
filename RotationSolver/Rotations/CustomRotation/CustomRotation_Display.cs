using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Attributes;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Timeline;
using RotationSolver.Windows.RotationConfigWindow;
using System.Linq;
using System.Reflection;

namespace RotationSolver.Rotations.CustomRotation
{
    internal abstract partial class CustomRotation
    {
        const ImGuiWindowFlags flags = 
              ImGuiWindowFlags.Tooltip |
              ImGuiWindowFlags.NoTitleBar |
              ImGuiWindowFlags.NoMove |
              ImGuiWindowFlags.NoResize |
              ImGuiWindowFlags.NoSavedSettings |
              ImGuiWindowFlags.AlwaysAutoResize;

        public unsafe void Display(ICustomRotation[] rotations, bool canAddButton)
            => this.DrawEnableTexture(canAddButton, null,
        text =>
        {
            var id = "Popup" + GetHashCode().ToString();

            ImGui.SetWindowPos(id, ImGui.GetIO().MousePos);
            ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(400, 0), new System.Numerics.Vector2(1000, 1000));
            if (ImGui.Begin(id, flags))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    ImGui.TextColored(ImGuiColors.DalamudYellow, text);
                }

                var type = this.GetType();

                var attrs = type.GetCustomAttributes<RotationDescAttribute>().ToList();
                foreach (var m in type.GetAllMethodInfo())
                {
                    attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
                }

                bool last = true;
                foreach (var a in RotationDescAttribute.Merge(attrs))
                {
                    if (last) ImGui.Separator();
                    last = RotationDescAttribute.MergeToOne(a)?.Display(this) ?? false;
                }

                ImGui.End();
            }

        }, () =>
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
                            Service.Configuration.RotationChoices[Job.RowId] = r.RotationName;
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
