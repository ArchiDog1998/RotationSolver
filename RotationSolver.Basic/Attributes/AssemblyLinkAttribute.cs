namespace RotationSolver.Basic.Attributes;

/// <summary>
/// An attribute to add the help and support link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyLinkAttribute : Attribute
{
    public string SupportLink { get; }
    public string HelpLink { get; }

    public AssemblyLinkAttribute(string supportLink = "", string helpLink = "")
    {
        SupportLink = supportLink;
        HelpLink = helpLink;
    }
}
