using ECommons.ExcelServices;

namespace RotationSolver.UI.SearchableSettings;

internal interface ISearchable
{
    CheckBoxSearch Parent { get; set; }

    string SearchingKeys { get; }
    bool ShowInChild { get; }

    void Draw(Job job);
}
