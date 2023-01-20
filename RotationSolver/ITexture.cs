namespace RotationSolver;

public interface IEnableTexture : ITexture
{
    string Description { get; }

    bool IsEnabled { get; set; }
}

public interface ITexture
{
    uint IconID { get; }
    string Name { get; }
}
