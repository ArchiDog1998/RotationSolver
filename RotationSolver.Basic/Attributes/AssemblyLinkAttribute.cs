namespace RotationSolver.Basic.Attributes;

/// <summary>
/// An attribute to add the help and support link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyLinkAttribute : Attribute
{
    /// <summary>
    /// The support link
    /// </summary>
    public string SupportLink { get; }

    /// <summary>
    /// Wiki link.
    /// </summary>
    public string HelpLink { get; }

    /// <summary>
    /// A link to change log.
    /// </summary>
    public string ChangeLog { get; }

    /// <summary>
    /// A link for donate.
    /// </summary>
    public string Donate { get; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supportLink"><see cref="SupportLink"/></param>
    /// <param name="helpLink"><see cref="HelpLink"/></param>
    /// <param name="changeLog"><see cref="ChangeLog"/></param>
    /// <param name="donate"><see cref="Donate"/></param>
    public AssemblyLinkAttribute(string supportLink = null, string helpLink = null,
        string changeLog = null, string donate = null)
    {
        SupportLink = supportLink;
        HelpLink = helpLink;
        ChangeLog = changeLog;
        Donate = donate;
    }
}
