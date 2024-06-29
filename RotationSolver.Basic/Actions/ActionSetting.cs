using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

internal enum SpecialActionType : byte
{
    None,
    MeleeRange,
    MovingForward,
}

/// <summary>
/// Setting from the developer.
/// </summary>
public class ActionSetting()
{
    /// <summary>
    /// The Ninjutsu action of this action.
    /// </summary>
    public IBaseAction[]? Ninjutsu { get; set; } = null;

    /// <summary>
    /// The override of the <see cref="ActionBasicInfo.MPNeed"/>.
    /// </summary>
    public Func<uint?>? MPOverride { get; set; } = null;

    /// <summary>
    /// Is this action in the melee range.
    /// </summary>
    internal SpecialActionType SpecialType { get; set; }

    /// <summary>
    /// Is this status is added by the plyer.
    /// </summary>
    public bool StatusFromSelf { get; set; } = true;
    
    /// <summary>
    /// The status that it provides to the target.
    /// </summary>
    public StatusID[]? TargetStatusProvide { get; set; } = null;

    /// <summary>
    /// The status that it needs on the target.
    /// </summary>
    public StatusID[]? TargetStatusNeed { get; set; } = null;

    /// <summary>
    /// Can the target be targeted.
    /// </summary>
    public Func<IBattleChara, bool> CanTarget { get; set; } = t => true;

    /// <summary>
    /// The additional not combo ids.
    /// </summary>
    public ActionID[]? ComboIdsNot { get; set; }

    /// <summary>
    /// The additional combo ids.
    /// </summary>
    public ActionID[]? ComboIds { get; set; }

    /// <summary>
    /// Status that this action provides.
    /// </summary>
    public StatusID[]? StatusProvide { get; set; } = null;

    /// <summary>
    /// Status that this action needs.
    /// </summary>
    public StatusID[]? StatusNeed { get; set; } = null;

    /// <summary>
    /// Your custom rotation check for your rotation.
    /// </summary>
    public Func<bool>? RotationCheck { get; set; } = null;

    internal Func<bool>? ActionCheck { get; set; } = null;

    internal Func<ActionConfig>? CreateConfig { get; set; } = null;

    /// <summary>
    /// Is this action friendly.
    /// </summary>
    public bool IsFriendly { get; set; }

    private TargetType _type = TargetType.Big;

    /// <summary>
    /// The strategy to target the target.
    /// </summary>
    public TargetType TargetType 
    {
        get
        {
            var type = IBaseAction.TargetOverride ?? _type;
            if (IsFriendly)
            {

            }
            else
            {
                switch (type)
                {
                    case TargetType.BeAttacked:
                        return _type;
                }
            }

            return type;
        }
        set => _type = value; 
    }

    /// <summary>
    /// The enemy positional for this action.
    /// </summary>
    public EnemyPositional EnemyPositional { get; set; } = EnemyPositional.None;

    /// <summary>
    /// Should end the special.
    /// </summary>
    public bool EndSpecial { get; set; }

    /// <summary>
    /// The quest ID that unlocks this action.
    /// 0 means no quest.
    /// </summary>
    public uint UnlockedByQuestID { get; set; } = 0;
}
