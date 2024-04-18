using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The action cooldown information.
/// </summary>
public readonly struct ActionCooldownInfo : ICooldown
{
    private readonly IBaseAction _action;

    /// <summary>
    /// The cd group.
    /// </summary>
    public byte CoolDownGroup { get; }

    unsafe RecastDetail* CoolDownDetail => ActionIdHelper.GetCoolDownDetail(CoolDownGroup);

    private unsafe float RecastTime => CoolDownDetail == null ? 0 : CoolDownDetail->Total;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeElapsed => RecastTimeElapsedRaw - DataCenter.DefaultGCDRemain;

    /// <summary>
    /// 
    /// </summary>
    internal unsafe float RecastTimeElapsedRaw => CoolDownDetail == null ? 0 : CoolDownDetail->Elapsed;
    float ICooldown.RecastTimeElapsedRaw => RecastTimeElapsedRaw;
    /// <summary>
    /// 
    /// </summary>
    public unsafe bool IsCoolingDown => ActionIdHelper.IsCoolingDown(CoolDownGroup);

    private float RecastTimeRemain => RecastTime - RecastTimeElapsedRaw;

    /// <summary>
    /// 
    /// </summary>
    public bool HasOneCharge => !IsCoolingDown || RecastTimeElapsedRaw >= RecastTimeOneChargeRaw;

    /// <summary>
    /// 
    /// </summary>
    public unsafe ushort CurrentCharges => (ushort)ActionManager.Instance()->GetCurrentCharges(_action.Info.AdjustedID);

    /// <summary>
    /// 
    /// </summary>
    public unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(_action.Info.AdjustedID, (uint)Player.Level), (ushort)1);

    internal float RecastTimeOneChargeRaw => ActionManager.GetAdjustedRecastTime(ActionType.Action, _action.Info.AdjustedID) / 1000f;

    float ICooldown.RecastTimeOneChargeRaw => RecastTimeOneChargeRaw;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeRemainOneCharge => RecastTimeRemainOneChargeRaw - DataCenter.DefaultGCDRemain;

    float RecastTimeRemainOneChargeRaw => RecastTimeRemain % RecastTimeOneChargeRaw;

    /// <summary>
    /// 
    /// </summary>
    public float RecastTimeElapsedOneCharge => RecastTimeElapsedOneChargeRaw - DataCenter.DefaultGCDElapsed;

    float RecastTimeElapsedOneChargeRaw => RecastTimeElapsedRaw % RecastTimeOneChargeRaw;

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="action">the action.</param>
    public ActionCooldownInfo(IBaseAction action)
    {
        _action = action;
        CoolDownGroup = _action.Action.GetCoolDownGroup();
    }

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

    /// <summary>
    /// Is this action used after several time.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool JustUsedAfter(float time)
    {
        var elapsed = RecastTimeElapsedRaw % RecastTimeOneChargeRaw;
        return elapsed + DataCenter.DefaultGCDRemain < time;
    }

    internal bool CooldownCheck(bool isEmpty, bool onLastAbility, bool ignoreClippingCheck, byte gcdCountForAbility)
    {
        if (!_action.Info.IsGeneralGCD)
        {
            if (IsCoolingDown)
            {
                if (_action.Info.IsRealGCD)
                {
                    if (!WillHaveOneChargeGCD(0, 0)) return false;
                }
                else
                {
                    if (!HasOneCharge && RecastTimeRemainOneChargeRaw > DataCenter.DefaultGCDRemain) return false;
                }
            }

            if (!isEmpty)
            {
                if (RecastTimeRemain > DataCenter.DefaultGCDRemain * gcdCountForAbility)
                    return false;
            }
        }

        if (!_action.Info.IsRealGCD)
        {
            if (ActionManagerHelper.GetCurrentAnimationLock() > 0) return false;
        }
        return true;
    }
}
