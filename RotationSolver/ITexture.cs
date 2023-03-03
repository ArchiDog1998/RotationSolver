namespace RotationSolver;

public interface ITexture
{
    uint IconID { get; }
    string Name { get; }

    string Description { get; }

    bool IsEnabled { get; set; }
}
