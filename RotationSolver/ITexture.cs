namespace RotationSolver;

public interface IEnable
{
    string Description { get; }

    bool IsEnabled { get; set; }
}

public interface ITexture
{
    uint IconID { get; }
    string Name { get; }
}
