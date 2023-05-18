using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Actions;

public class BaseItem : IBaseItem
{
    private readonly Item _item = null;
    private uint A4 { get; } = 0;

    public uint ID => _item.RowId;
    public uint AdjustedID => ID;

    public Func<bool> OtherCheck { private get; set; }
    private unsafe bool HasIt => InventoryManager.Instance()->GetInventoryItemCount(ID, false) > 0 ||
            InventoryManager.Instance()->GetInventoryItemCount(ID, true) > 0;

    public uint IconID { get; }

    public string Name => _item.Name;

    public bool IsEnabled
    {
        get => !Service.Config.DisabledItems.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.DisabledItems.Remove(ID);
            }
            else
            {
                Service.Config.DisabledItems.Add(ID);
            }
        }
    }

    public bool IsInCooldown
    {
        get => !Service.Config.NotInCoolDownItems.Contains(ID);
        set
        {
            if (value)
            {
                Service.Config.NotInCoolDownItems.Remove(ID);
            }
            else
            {
                Service.Config.NotInCoolDownItems.Add(ID);
            }
        }
    }

    public string Description => string.Empty;

    public unsafe float RecastTimeOneCharge => ActionManager.Instance()->GetRecastTime(ActionType.Item, ID);

    public unsafe float RecastTimeElapsed => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, ID);

    public bool EnoughLevel => true;

    public byte Level => 0;

    public unsafe bool IsCoolingDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Item, ID);

    public uint SortKey { get; }

    public float AnimationLockTime => OtherConfiguration.AnimationLockTime.TryGetValue(AdjustedID, out var time) ? time : 1.1f;

    public virtual bool IsActionSequencer => false;

    protected virtual bool CanUseThis => true;

    public unsafe BaseItem(uint row, uint a4 = 65535)
    {
        _item = Service.GetSheet<Item>().GetRow(row);
        IconID = _item.Icon;
        A4 = a4;
        SortKey = (uint)ActionManager.Instance()->GetRecastGroup((int)ActionType.Item, ID);
    }

    public virtual unsafe bool CanUse(out IAction item)
    {
        item = this;
        if (_item == null) return false;
        if (!CanUseThis) return false;
        if (DataCenter.DisabledAction != null && DataCenter.DisabledAction.Contains(ID)) return false;
        if(!IsEnabled) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Item, ID))
            && ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Item, ID + 1000000))) return false;

        var remain = RecastTimeOneCharge - RecastTimeElapsed;

        if(DataCenter.WeaponRemain > 0)
        {
            if (DataCenter.NextAbilityToNextGCD > AnimationLockTime + DataCenter.Ping) return false;

            if (!CooldownHelper.RecastAfter(remain, DataCenter.ActionRemain,  false)) return false;
        }

        if (OtherCheck != null && !OtherCheck()) return false;

        return HasIt;
    }

    public unsafe bool Use()
    {
        if (_item == null) return false;

        if (InventoryManager.Instance()->GetInventoryItemCount(ID, true) > 0)
        {
            return ActionManager.Instance()->UseAction(ActionType.Item, ID + 1000000, Player.Object.ObjectId, A4);
        }

        return ActionManager.Instance()->UseAction(ActionType.Item, ID, Player.Object.ObjectId, A4);
    }

    public override string ToString() => Name;
}
