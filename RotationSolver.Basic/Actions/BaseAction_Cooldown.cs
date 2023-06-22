using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace RotationSolver.Basic.Actions;

public partial class BaseAction
{
    public bool ElapsedOneChargeAfterGCD(uint gcdCount = 0, float offset = 0)
        => ElapsedOneChargeAfter(DataCenter.GCDTime(gcdCount, offset));

    public bool ElapsedOneChargeAfter(float time)
        => IsCoolingDown && time <= RecastTimeElapsedOneCharge;

    public bool ElapsedAfterGCD(uint gcdCount = 0, float offset = 0)
        => ElapsedAfter(DataCenter.GCDTime(gcdCount, offset));

    public bool ElapsedAfter(float time)
        => IsCoolingDown && time <= RecastTimeElapsed;

    public bool WillHaveOneChargeGCD(uint gcdCount = 0, float offset = 0)
        => WillHaveOneCharge(DataCenter.GCDTime(gcdCount, offset));

    public bool WillHaveOneCharge(float remain) 
        => HasOneCharge || RecastTimeRemainOneCharge <= remain;

    private unsafe RecastDetail* CoolDownDetail => ActionManager.Instance()->GetRecastGroupDetail(CoolDownGroup - 1);

    private unsafe float RecastTime => CoolDownDetail->Total;

    public float RecastTimeElapsed => RecastTimeElapsedRaw - DataCenter.WeaponElapsed;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe float RecastTimeElapsedRaw => CoolDownDetail->Elapsed;

    public unsafe bool IsCoolingDown => CoolDownDetail->IsActive != 0;

    private float RecastTimeRemain => RecastTime - RecastTimeElapsedRaw;

    public unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(AdjustedID, (uint)Player.Level), (ushort)1);

    public bool HasOneCharge => !IsCoolingDown || RecastTimeElapsedRaw >= RecastTimeOneChargeRaw;

    public ushort CurrentCharges => IsCoolingDown ? (ushort)(RecastTimeElapsedRaw / RecastTimeOneChargeRaw) : MaxCharges;

    public float RecastTimeOneChargeRaw => ActionManager.GetAdjustedRecastTime(ActionType.Spell, AdjustedID) / 1000f;

    public float RecastTimeRemainOneCharge => RecastTimeRemainOneChargeRaw - DataCenter.WeaponRemain;

    private float RecastTimeRemainOneChargeRaw => RecastTimeRemain % RecastTimeOneChargeRaw;

    public float RecastTimeElapsedOneCharge => RecastTimeElapsedOneChargeRaw - DataCenter.WeaponElapsed;

    private float RecastTimeElapsedOneChargeRaw => RecastTimeElapsedRaw % RecastTimeOneChargeRaw;
}
