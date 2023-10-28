using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public bool ElapsedOneChargeAfterGCD(uint gcdCount = 0, float offset = 0)
        => ElapsedOneChargeAfter(DataCenter.GCDTime(gcdCount, offset));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool ElapsedOneChargeAfter(float time)
        => IsCoolingDown && time <= RecastTimeElapsedOneCharge;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public bool ElapsedAfterGCD(uint gcdCount = 0, float offset = 0)
        => ElapsedAfter(DataCenter.GCDTime(gcdCount, offset));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool ElapsedAfter(float time)
        => IsCoolingDown && time <= RecastTimeElapsed;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public bool WillHaveOneChargeGCD(uint gcdCount = 0, float offset = 0)
        => WillHaveOneCharge(DataCenter.GCDTime(gcdCount, offset));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="remain"></param>
    /// <returns></returns>
    public bool WillHaveOneCharge(float remain)
        => HasOneCharge || RecastTimeRemainOneCharge <= remain;

    unsafe RecastDetail* CoolDownDetail => ActionManager.Instance()->GetRecastGroupDetail(CoolDownGroup - 1);

    private unsafe float RecastTime => CoolDownDetail == null ? 0 : CoolDownDetail->Total;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeElapsed => RecastTimeElapsedRaw - DataCenter.WeaponElapsed;

    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe float RecastTimeElapsedRaw => CoolDownDetail == null ? 0 : CoolDownDetail->Elapsed;

    /// <summary>
    /// 
    /// </summary>
    public unsafe bool IsCoolingDown => CoolDownDetail == null ? false : CoolDownDetail->IsActive != 0;

    private float RecastTimeRemain => RecastTime - RecastTimeElapsedRaw;

    /// <summary>
    /// 
    /// </summary>
    public unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(AdjustedID, (uint)Player.Level), (ushort)1);

    /// <summary>
    /// 
    /// </summary>
    public bool HasOneCharge => !IsCoolingDown || RecastTimeElapsedRaw >= RecastTimeOneChargeRaw;

    /// <summary>
    /// 
    /// </summary>
    public ushort CurrentCharges => IsCoolingDown ? (ushort)(RecastTimeElapsedRaw / RecastTimeOneChargeRaw) : MaxCharges;

    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public float RecastTimeOneChargeRaw => ActionManager.GetAdjustedRecastTime(ActionType.Action, AdjustedID) / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeRemainOneCharge => RecastTimeRemainOneChargeRaw - DataCenter.WeaponRemain;

    float RecastTimeRemainOneChargeRaw => RecastTimeRemain % RecastTimeOneChargeRaw;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeElapsedOneCharge => RecastTimeElapsedOneChargeRaw - DataCenter.WeaponElapsed;

    float RecastTimeElapsedOneChargeRaw => RecastTimeElapsedRaw % RecastTimeOneChargeRaw;
}
