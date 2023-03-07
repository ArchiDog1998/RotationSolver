using ImGuiScene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class LinkDescAttribute : Attribute
{
    public TextureWrap Texture { get; }
    public string Path { get; } 
    public LinkDescAttribute(string path)
    {
        Path = path;
        Texture = Service.DataManager.GetImGuiTexture(path);
    }
}
