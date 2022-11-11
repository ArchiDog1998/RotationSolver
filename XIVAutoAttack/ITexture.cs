using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack
{
    public interface ITexture
    {
        TextureWrap Texture { get; }
        bool IsEnabled { get; set; }
        string Name { get; }
        string Author { get; }
        string Description { get; }
    }
}
