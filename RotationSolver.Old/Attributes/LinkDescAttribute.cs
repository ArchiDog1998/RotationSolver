using ImGuiScene;
using System;

namespace RotationSolver.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescAttribute : Attribute
{
    //public TextureWrap Texture { get; }
    public string Path { get; } 
    public LinkDescAttribute(string path)
    {
        Path = path;
        //Texture = Service.DataManager.GetImGuiTexture(path);
    }
}
