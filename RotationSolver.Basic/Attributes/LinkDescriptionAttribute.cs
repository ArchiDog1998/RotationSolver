using Dalamud.Utility;
using System.Net;
using System;
using ImGuiScene;
using Dalamud.Interface.Internal;
using RotationSolver.Basic.Data;

namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LinkDescriptionAttribute : Attribute
{
    public TextureWrap Texture => IconSet.GetTexture(Path);
    public string Description { get; private set; }
    public string Path { get; }
    public LinkDescriptionAttribute(string path, string description = "")
    {
        Path = path;
        Description = description;
    }
}
