namespace XIVAutoAttack
{
    public interface ITexture
    {
        uint IconID { get; }
        bool IsEnabled { get; set; }
        string Name { get; }
        string Author { get; }
        string Description { get; }
    }
}
