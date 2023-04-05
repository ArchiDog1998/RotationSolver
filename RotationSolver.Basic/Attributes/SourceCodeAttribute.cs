namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SourceCodeAttribute : Attribute
{
    public string Url { get; }

    public SourceCodeAttribute(string url)
    {
        Url = url;
    }
}
