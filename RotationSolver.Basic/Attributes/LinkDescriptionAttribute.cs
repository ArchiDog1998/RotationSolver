namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The link to a image or web about your rotation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    /// <summary>
    /// The description.
    /// </summary>
    public LinkDescription LinkDescription { get; set; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="description"></param>
    public LinkDescriptionAttribute(string url, string description = "")
    {
        LinkDescription = new() { Url = url, Description = description };
    }
}

/// <summary>
/// Link description itself.
/// </summary>
public readonly record struct LinkDescription
{
    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Url.
    /// </summary>
    public string Url { get; init; }
}
