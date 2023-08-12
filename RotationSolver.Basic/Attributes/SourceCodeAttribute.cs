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
    public string Path { get; set; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="path"></param>
    [Obsolete("This method will be deleted in several months.", true)]
    public SourceCodeAttribute(string path)
    {
        Path = path;
    }

    /// <summary>
    /// 
    /// </summary>
    public SourceCodeAttribute()
    {
        
    }
}
