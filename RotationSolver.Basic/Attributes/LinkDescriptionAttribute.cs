using ImGuiScene;

namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The link to a image or web about your rotation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    /// <summary>
    /// Image from url.
    /// </summary>
    public TextureWrap Texture => IconSet.GetTexture(Path);

    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Url.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="description"></param>
    public LinkDescriptionAttribute(string path, string description = "")
    {
        Path = path;
        Description = description;
    }
}
