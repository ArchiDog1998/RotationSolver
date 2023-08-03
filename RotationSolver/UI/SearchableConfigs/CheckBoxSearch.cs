namespace RotationSolver.UI.SearchableSettings;

internal class CheckBoxSearch : ISearchable
{
    public string SearchingKey => throw new NotImplementedException();

    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public void Draw()
    {

    }
}
