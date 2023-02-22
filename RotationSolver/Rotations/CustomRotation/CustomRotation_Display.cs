using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Timeline;
using RotationSolver.Windows.RotationConfigWindow;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RotationSolver.Rotations.CustomRotation
{
    internal abstract partial class CustomRotation
    {
        const ImGuiWindowFlags flags = 
              ImGuiWindowFlags.Tooltip |
              ImGuiWindowFlags.NoMove |
              ImGuiWindowFlags.NoSavedSettings |
              ImGuiWindowFlags.NoBringToFrontOnFocus |
              ImGuiWindowFlags.NoDecoration |
              ImGuiWindowFlags.NoInputs|
              ImGuiWindowFlags.AlwaysAutoResize;

        public unsafe void Display(ICustomRotation[] rotations, bool canAddButton)
            => this.DrawEnableTexture(canAddButton, null,
        text =>
        {
            var id = "Popup" + GetHashCode().ToString();

            ImGui.SetWindowPos(id, ImGui.GetIO().MousePos);
            ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(1000, 1500));
            if (ImGui.Begin(id, flags))
            {
                var t = IconSet. GetTexture(IconSet.GetJobIcon(this, IconType.Framed));
                ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

                if (!string.IsNullOrEmpty(text))
                {
                    ImGui.SameLine();
                    ImGui.Text("  ");
                    ImGui.SameLine();
                    ImGui.TextWrapped(text);
                }

                var type = this.GetType();

                var attrs = type.GetCustomAttributes<RotationDescAttribute>().ToList();
                foreach (var m in type.GetAllMethodInfo())
                {
                    attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
                }

                foreach (var a in RotationDescAttribute.Merge(attrs))
                {
                    RotationDescAttribute.MergeToOne(a)?.Display(this);
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
