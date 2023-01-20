using Dalamud.Interface;
using ImGuiNET;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Helpers
{
    internal static class ImGuiHelper
    {
        public static void DrawEnableTexture<T>(this T texture, bool isSelected, Action selected, 
            Action additonalHeader = null, Action otherThing = null) where T : class, IEnableTexture
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

            var t = texture.GetTexture();
            ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));

            var desc = texture.Description;
            HoveredString(desc);
            ImGui.SameLine();
            Spacing();


            if(ImGui.Selectable(texture.Name, isSelected))
            {
                selected?.Invoke();
            }
            HoveredString(desc);

            ImGui.SameLine();
            Spacing();

            bool enable = texture.IsEnabled;
            if (ImGui.Checkbox("##" + texture.Name, ref enable))
            {
                texture.IsEnabled = enable;
                Service.Configuration.Save();
            }
            HoveredString(desc);

            additonalHeader?.Invoke();

            if (enable)
            {
                ImGui.Indent(t.Width);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
                otherThing?.Invoke();
                ImGui.PopStyleVar();
                ImGui.Unindent();
            }

            ImGui.PopStyleVar();
        }

        public static bool IconButton(FontAwesomeIcon icon, string name)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            var result = ImGui.Button($"{icon.ToIconString()}##{name}");
            ImGui.PopFont();
            return result;
        }

        public  static void HoveredString(string text)
        {
            if (ImGui.IsItemHovered())
            {
                if (!string.IsNullOrEmpty(text)) ImGui.SetTooltip(text);
            }
        }

        internal static void Spacing(byte count = 1)
        {
            string s = string.Empty;
            for (int i = 0; i < count; i++)
            {
                s += "    ";
            }
            ImGui.Text(s);
            ImGui.SameLine();
        }
    }
}
