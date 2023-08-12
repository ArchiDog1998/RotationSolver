namespace RotationSolver.Basic.Attributes;

/// <summary>
/// An attribute to add the help and support link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyLinkAttribute : Attribute
{
    /// <summary>
    /// A link for donate.
    /// </summary>
    public string Donate { get; set; }

    /// <summary>
    /// A link for your user name in GitHub.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// A link for the repo in your GitHub.
    /// </summary>
    public string Repository { get; set; }
    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supportLink"></param>
    /// <param name="helpLink"></param>
    /// <param name="changeLog"></param>
    /// <param name="donate"><see cref="Donate"/></param>
    [Obsolete("Never use it anymore, it'll be deleted in a month!")]
    public AssemblyLinkAttribute(string supportLink = null, string helpLink = null,
        string changeLog = null, string donate = null)
    {
        Donate = donate;
    }

    /// <summary>
    /// 
    /// </summary>
    public AssemblyLinkAttribute()
    {
        
    }
}
