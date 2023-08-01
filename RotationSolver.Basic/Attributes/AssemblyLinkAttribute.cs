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
    /// A link for changelog.
    /// </summary>
    public string ChangeLog { get; set; }

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="supportLink"></param>
    /// <param name="helpLink"></param>
    /// <param name="changeLog"></param>
    /// <param name="donate"><see cref="Donate"/></param>
    [Obsolete()]
    public AssemblyLinkAttribute(string supportLink = null, string helpLink = null,
        string changeLog = null, string donate = null)
    {
        Donate = donate;
        ChangeLog = changeLog;
    }

    /// <summary>
    /// 
    /// </summary>
    public AssemblyLinkAttribute()
    {
        
    }
}
