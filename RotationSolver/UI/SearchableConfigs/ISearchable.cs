using ECommons.ExcelServices;

namespace RotationSolver.UI.SearchableSettings;

public interface ISearchable
{
    string SearchingKey { get; }

    void Draw(Job job);
}
