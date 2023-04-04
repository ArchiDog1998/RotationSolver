namespace RotationSolver.Basic.Configuration.RotationConfig;

public interface IRotationConfig
{
    string Name { get; }
    string DisplayName { get; }
    string GetValue(ClassJobID job, string rotationName);
    string GetDisplayValue(ClassJobID job, string rotationName);
    void SetValue(ClassJobID job, string rotationName, string value);
    bool DoCommand(IRotationConfigSet set, string str);
}
