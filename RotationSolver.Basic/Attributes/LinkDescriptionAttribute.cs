namespace RotationSolver.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    //public TextureWrap Texture { get; }
    public string Path { get; } 
    public LinkDescriptionAttribute(string path)
    {
        Path = path;
        //Texture = Service.DataManager.GetImGuiTexture(path);
    }
}
