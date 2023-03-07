using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Windows.RotationConfigWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RotationSolver.Rotations.CustomRotation;

internal abstract partial class CustomRotation
{
    public unsafe void Display(ICustomRotation[] rotations, bool canAddButton)
        => this.DrawEnableTexture(canAddButton, null,
    text =>
    {
        ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(1000, 1500));
        ImGuiHelper.DrawTooltip(() =>
        {
            var t = IconSet.GetTexture(IconSet.GetJobIcon(this, IconType.Framed));
            ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

            if (!string.IsNullOrEmpty(text))
            {
                ImGui.SameLine();
                ImGui.Text("  ");
                ImGui.SameLine();
                ImGui.Text(text);
            }

            var type = GetType();

            var attrs = new List<RotationDescAttribute> { RotationDescAttribute.MergeToOne(type.GetCustomAttributes<RotationDescAttribute>()) };

            foreach (var m in type.GetAllMethodInfo())
            {
                attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
            }

            foreach (var a in RotationDescAttribute.Merge(attrs))
            {
                RotationDescAttribute.MergeToOne(a)?.Display(this);
            }
        }, "Popup" + GetHashCode().ToString());
    },
    showToolTip =>
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
                        showToolTip?.Invoke(r.Description);
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

        ImGui.SameLine();
        ImGuiHelper.Spacing();

        foreach (var texture in GetType().GetCustomAttributes<LinkDescAttribute>())
        {
            if (ImGuiHelper.IconButton(FontAwesomeIcon.HandsHelping,
                "Button" + texture.GetHashCode().ToString()))
            {
                Util.OpenLink(texture.Path);
            }
            if (ImGui.IsItemHovered() && texture.Texture != null)
            {
                ImGuiHelper.DrawTooltip(() =>
                {
                    var ratio = Math.Min(1, 1000f / texture.Texture.Width);
                    var size = new Vector2(texture.Texture.Width * ratio,
                        texture.Texture.Height * ratio);
                    ImGui.Image(texture.Texture.ImGuiHandle, size);
                }, "Picture" + texture.GetHashCode().ToString());
            }
        }
    }, () =>
    {
        RotationConfigWindow.DrawRotationRole(this);

        Configs.Draw(canAddButton);
    });
}
