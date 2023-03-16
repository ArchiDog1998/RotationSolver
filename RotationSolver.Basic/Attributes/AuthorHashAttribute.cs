namespace RotationSolver.Basic.Attributes;

/// <summary>
/// Put your hash code from debug tab into this.
/// In this case if someone is using this plugin with you in the same duty. They'll use some emote to you!
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AuthorHashAttribute : Attribute
{
    public string Hash { get; }
    public AuthorHashAttribute(string hash)
    {
        Hash = hash;
    }
}
