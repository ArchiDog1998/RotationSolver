using ImGuiScene;

namespace XIVAutoAttack.Actions
{
    public interface IAction
    {
       bool Use();
       uint ID { get; }
       TextureWrap Icon { get; }
       uint AdjustedID { get; }
    }
}
