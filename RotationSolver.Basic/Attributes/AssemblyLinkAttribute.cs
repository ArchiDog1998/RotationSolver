namespace RotationSolver.Basic.Attributes;

/// <summary>
/// An attribute to add the help and support link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyLinkAttribute : Attribute
{
    public string SupportLink { get; }
    public string HelpLink { get; }
    public string ChangeLog { get; }
    public string Donate { get; }

    public AssemblyLinkAttribute(string supportLink = null, string helpLink = null,
        string changeLog = null, string donate = null)
    {
        SupportLink = supportLink;
        HelpLink = helpLink;
        ChangeLog = changeLog;
        Donate = donate;
    }
}
