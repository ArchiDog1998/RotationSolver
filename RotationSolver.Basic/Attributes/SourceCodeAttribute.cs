namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The attribute for the rotation's source code link.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SourceCodeAttribute : Attribute
{
    /// <summary>
    /// The link to the source code.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="url"></param>
    public SourceCodeAttribute(string url)
    {
        Url = url;
    }
}
