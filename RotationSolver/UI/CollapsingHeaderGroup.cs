using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;

namespace RotationSolver.UI;

internal class CollapsingHeaderGroup()
{
    private readonly Dictionary<Func<string>, Action> _headers = [];
    private int _openedIndex = -1;

    public float HeaderSize { get; set; } = 24;

    public CollapsingHeaderGroup(Dictionary<Func<string>, Action> headers) : this()
    {
        _headers = headers;
    }

    public void AddCollapsingHeader(Func<string> name, Action action)
    {
        _headers.Add(name, action);
    }

    public void RemoveCollapsingHeader(Func<string> name)
    {
        _headers.Remove(name);
    }

    public void ClearCollapsingHeader()
    {
        _headers.Clear();
    }

    public void Draw()
    {
        var index = -1;
        foreach (var header in _headers)
        {
            index++;

            if (header.Key == null) continue;
            if (header.Value == null) continue;

            var name = header.Key();
            if (string.IsNullOrEmpty(name)) continue;

            try
            {
                ImGui.Spacing();
                ImGui.Separator();
                var selected = index == _openedIndex;
                var changed = false;
                using (var font = ImRaii.PushFont(FontManager.GetFont(18)))
                {
                    changed = ImGui.Selectable(name, selected, ImGuiSelectableFlags.DontClosePopups);
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }
                if (changed)
                {
                    _openedIndex = selected ? -1 : index;
                }
                if (selected)
                {
                    header.Value();
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Something wrong with header drawing.");
            }
        }
    }
}