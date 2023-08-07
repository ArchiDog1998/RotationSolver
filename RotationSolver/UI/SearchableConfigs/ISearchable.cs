using ECommons.ExcelServices;

namespace RotationSolver.UI.SearchableSettings;

internal interface ISearchable
{
    CheckBoxSearch Parent { get; set; }

    string SearchingKey { get; }

    void Draw(Job job);
}
