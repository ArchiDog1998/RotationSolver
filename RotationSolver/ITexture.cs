namespace RotationSolver;

public interface IEnableTexture : ITexture
{
    bool IsEnabled { get; set; }
}

public interface ITexture
{
    uint IconID { get; }
    string Name { get; }
}
