using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration.RotationConfig;

public interface IRotationConfig
{
    string Name { get; }
    string DisplayName { get; }
    string GetValue(Job job, string rotationName);
    string GetDisplayValue(Job job, string rotationName);
    void SetValue(Job job, string rotationName, string value);
    bool DoCommand(IRotationConfigSet set, string str);
}
