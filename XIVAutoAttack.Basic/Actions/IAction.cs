using ImGuiScene;

namespace XIVAutoAttack.Actions
{
    public interface IAction : ITexture
    {
       bool Use();
       uint ID { get; }
       uint AdjustedID { get; }
    }
}
