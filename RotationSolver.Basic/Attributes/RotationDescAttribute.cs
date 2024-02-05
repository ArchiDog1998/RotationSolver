namespace RotationSolver.Basic.Attributes;

/// <summary>
/// The description about the macro. If it tag at the rotation class, it means Burst. Others means the macro that this method belongs to.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class RotationDescAttribute : Attribute
{
    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Description type.
    /// </summary>
    public DescType Type { get; private set; } = DescType.None;

    /// <summary>
    /// What actions this linked.
    /// </summary>
    public IEnumerable<ActionID> Actions { get; private set; } = Enumerable.Empty<ActionID>();

    internal uint IconID => Type switch
    {
        DescType.BurstActions => 62583,

        DescType.HealAreaGCD or DescType.HealAreaAbility or
        DescType.HealSingleGCD or DescType.HealSingleAbility => 62582,

        DescType.DefenseAreaGCD or DescType.DefenseAreaAbility or
        DescType.DefenseSingleGCD or DescType.DefenseSingleAbility => 62581,

        DescType.MoveForwardGCD or DescType.MoveForwardAbility or
        DescType.MoveBackAbility => 104,

        DescType.SpeedAbility => 844,

        _ => 62144,
    };

    internal bool IsOnCommand
    {
        get
        {
            var command = DataCenter.SpecialType;
            return Type switch
            {
                DescType.BurstActions => command == SpecialCommandType.Burst,
                DescType.HealAreaAbility or DescType.HealAreaGCD => command == SpecialCommandType.HealArea,
                DescType.HealSingleAbility or DescType.HealSingleGCD => command == SpecialCommandType.HealSingle,
                DescType.DefenseAreaGCD or DescType.DefenseAreaAbility => command == SpecialCommandType.DefenseArea,
                DescType.DefenseSingleGCD or DescType.DefenseSingleAbility => command == SpecialCommandType.DefenseSingle,
                DescType.MoveForwardGCD or DescType.MoveForwardAbility => command == SpecialCommandType.MoveForward,
                DescType.MoveBackAbility => command == SpecialCommandType.MoveBack,
                DescType.SpeedAbility => command == SpecialCommandType.Speed,
                _ => false,
            };
        }
    }

    internal RotationDescAttribute(DescType descType)
    {
        Type = descType;
    }

    /// <summary>
    /// Constructer
    /// </summary>
    /// <param name="actions"></param>
    public RotationDescAttribute(params ActionID[] actions)
        : this(string.Empty, actions)
    {
    }

    /// <summary>
    /// Constructer
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="actions"></param>
    public RotationDescAttribute(string desc, params ActionID[] actions)
    {
        Description = desc;
        Actions = actions;
    }

    private RotationDescAttribute()
    {

    }

    internal static IEnumerable<RotationDescAttribute[]> Merge(IEnumerable<RotationDescAttribute?> rotationDescAttributes)
        => from r in rotationDescAttributes
           where r is not null
           group r by r.Type into gr
           orderby gr.Key
           select gr.ToArray();

    internal static RotationDescAttribute? MergeToOne(IEnumerable<RotationDescAttribute> rotationDescAttributes)
    {
        var result = new RotationDescAttribute();
        foreach (var attr in rotationDescAttributes)
        {
            if (attr == null) continue;
            if (!string.IsNullOrEmpty(attr.Description))
            {
                result.Description = attr.Description;
            }
            if (attr.Type != DescType.None)
            {
                result.Type = attr.Type;
            }
            result.Actions = result.Actions.Union(attr.Actions);
        }

        if (result.Type == DescType.None) return null;
        return result;
    }
}
