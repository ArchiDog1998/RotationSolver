using Dalamud.Utility;
using ECommons.ImGuiMethods;
using ImGuiScene;
using static System.Windows.Forms.LinkLabel;

namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The link to a image or web about your rotation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    /// <summary>
    /// The description.
    /// </summary>
    public LinkDescription LinkDescription { get; set; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="description"></param>
    public LinkDescriptionAttribute(string path, string description = "")
    {
        LinkDescription = new() { Path = path, Description = description };
    }
}

/// <summary>
/// Link description itself.
/// </summary>
public class LinkDescription
{
    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Url.
    /// </summary>
    public string Path { get; init; }
}
