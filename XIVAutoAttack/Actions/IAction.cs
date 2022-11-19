namespace XIVAutoAttack.Actions
{
    public interface IAction : IEnableTexture
    {
        bool Use();
        uint ID { get; }
        uint AdjustedID { get; }
    }
}
