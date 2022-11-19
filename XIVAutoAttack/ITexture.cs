namespace XIVAutoAttack
{
    public interface IEnableTexture : ITexture
    {
        bool IsEnabled { get; set; }
        string Author { get; }
        string Description { get; }
    }

    public interface ITexture
    {
        uint IconID { get; }
        string Name { get; }
    }
}
