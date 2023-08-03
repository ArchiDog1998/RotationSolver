using ECommons.ExcelServices;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI.SearchableConfigs;

internal abstract class Searchable : ISearchable
{
    public string SearchingKey => Name + " : " + Description;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public Action DrawTooltip { get; set; }

    public abstract void Draw(Job job);

    protected void ShowTooltip()
    {
        ImguiTooltips.ShowTooltip(() =>
        {
            var showDesc = !string.IsNullOrEmpty(Description);
            if (showDesc)
            {
                ImGui.TextWrapped(Description);
            }
            if(showDesc && DrawTooltip != null)
            {
                ImGui.Separator();
            }
            DrawTooltip?.Invoke();
        });
    }
}
