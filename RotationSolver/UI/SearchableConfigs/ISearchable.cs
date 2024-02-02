using RotationSolver.UI.SearchableConfigs;

namespace RotationSolver.UI.SearchableSettings;

internal interface ISearchable
{
    JobFilter PvPFilter { get; set; }
    JobFilter PvEFilter { get; set; }

    CheckBoxSearch? Parent { get; set; }

    string SearchingKeys { get; }
    bool ShowInChild { get; }

    void Draw();
}
