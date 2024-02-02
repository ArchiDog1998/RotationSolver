namespace RotationSolver.Basic.Data;

/// <summary>
/// The Type of description.
/// </summary>
public enum DescType : byte
{
    /// <summary>
    /// 
    /// </summary>
    None,

    /// <summary>
    /// 
    /// </summary>
    [Description("Burst Actions")]
    BurstActions,

    /// <summary>
    /// 
    /// </summary>
    [Description("Heal Area GCD")]
    HealAreaGCD,

    /// <summary>
    /// 
    /// </summary>
    [Description("Heal Area Ability")]
    HealAreaAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Heal Single GCD")]
    HealSingleGCD,

    /// <summary>
    /// 
    /// </summary>
    [Description("Heal Single Ability")]
    HealSingleAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Defense Area GCD")]
    DefenseAreaGCD,

    /// <summary>
    /// 
    /// </summary>
    [Description("Defense Area Ability")]
    DefenseAreaAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Defense Single GCD")]
    DefenseSingleGCD,

    /// <summary>
    /// 
    /// </summary>
    [Description("Defense Single Ability")]
    DefenseSingleAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Move Forward GCD")]
    MoveForwardGCD,

    /// <summary>
    /// 
    /// </summary>
    [Description("Move Forward Ability")]
    MoveForwardAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Move Back Ability")]
    MoveBackAbility,

    /// <summary>
    /// 
    /// </summary>
    [Description("Speed Ability")]
    SpeedAbility,
}
