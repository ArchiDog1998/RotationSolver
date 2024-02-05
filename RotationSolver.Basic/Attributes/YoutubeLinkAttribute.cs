namespace RotationSolver.Basic.Attributes;

/// <summary>
/// To show your youtube video link
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class YoutubeLinkAttribute : Attribute
{
    /// <summary>
    /// The youtube link Id
    /// </summary>
    public string? ID { get; set; }
}
