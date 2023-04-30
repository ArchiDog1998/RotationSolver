namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class RotationDescAttribute : Attribute
{
    public string Description { get; private set; } = string.Empty;
    public DescType Type { get; private set; } = DescType.None;
    public IEnumerable<ActionID> Actions { get; private set; } = Enumerable.Empty<ActionID>();
    public uint IconID => Type switch
    {
        DescType.BurstActions => 62583,

        DescType.HealAreaGCD => 62582,
        DescType.HealAreaAbility => 62582,
        DescType.HealSingleGCD => 62582,
        DescType.HealSingleAbility => 62582,

        DescType.DefenseAreaGCD => 62581,
        DescType.DefenseAreaAbility => 62581,
        DescType.DefenseSingleGCD => 62581,
        DescType.DefenseSingleAbility => 62581,

        DescType.MoveForwardGCD => 104,
        DescType.MoveForwardAbility => 104,
        DescType.MoveBackAbility => 104,

        _ => 62144,
    };

    public bool IsOnCommand
    {
        get
        {
            var command = DataCenter.SpecialType;
            switch (Type)
            {
                case DescType.BurstActions:
                    return command == SpecialCommandType.Burst;

                case DescType.HealAreaAbility:
                case DescType.HealAreaGCD:
                    return command == SpecialCommandType.HealArea;

                case DescType.HealSingleAbility:
                case DescType.HealSingleGCD:
                    return command == SpecialCommandType.HealSingle;

                case DescType.DefenseAreaGCD:
                case DescType.DefenseAreaAbility:
                    return command == SpecialCommandType.DefenseArea;

                case DescType.DefenseSingleGCD:
                case DescType.DefenseSingleAbility:
                    return command == SpecialCommandType.DefenseSingle;

                case DescType.MoveForwardGCD:
                case DescType.MoveForwardAbility:
                    return command == SpecialCommandType.MoveForward;

                case DescType.MoveBackAbility:
                    return command == SpecialCommandType.MoveBack;

                default:
                    return false;
            }
        }
    }

    internal RotationDescAttribute(DescType descType)
    {
        Type = descType;
    }
    public RotationDescAttribute(params ActionID[] actions)
        : this(string.Empty, actions)
    {
    }

    public RotationDescAttribute(string desc, params ActionID[] actions)
    {
        Description = desc;
        Actions = actions;
    }

    private RotationDescAttribute()
    {

    }

    public static IEnumerable<RotationDescAttribute[]> Merge(IEnumerable<RotationDescAttribute> rotationDescAttributes)
        => from r in rotationDescAttributes
           where r is RotationDescAttribute
           group r by r.Type into gr
           orderby gr.Key
           select gr.ToArray();

    public static RotationDescAttribute MergeToOne(IEnumerable<RotationDescAttribute> rotationDescAttributes)
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
