using ECommons.ExcelServices;

namespace RotationSolver.UI.SearchableSettings;

internal interface ISearchable
{
    CheckBoxSearch Parent { get; set; }

    string SearchingKeys { get; }

    void Draw(Job job, bool mustDraw = false);
}
