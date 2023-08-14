using Dalamud.Logging;

namespace RotationSolver.UI;

internal class CollapsingHeaderGroup
{
    private readonly Dictionary<Func<string>, Action> _headers = new Dictionary<Func<string>, Action>();
    private int _openedIndex = -1;

    public float HeaderSize { get; set; } = 24;

    public CollapsingHeaderGroup()
    {
        
    }

    public CollapsingHeaderGroup(Dictionary<Func<string>, Action> headers)
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
            if(string.IsNullOrEmpty(name)) continue;

            try
            {
                ImGui.Spacing();
                ImGui.Separator();
                var selected = index == _openedIndex;
                ImGui.PushFont(ImGuiHelper.GetFont(HeaderSize));
                var changed = ImGui.Selectable(name, selected);
                ImGui.PopFont();
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
                PluginLog.Warning(ex, "Something wrong with header drawing.");
            }
        }
    }
}
