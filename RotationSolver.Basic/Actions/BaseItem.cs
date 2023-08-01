using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The item usage.
/// </summary>
public class BaseItem : IBaseItem
{
    private protected readonly Item _item = null;
    private uint A4 { get; } = 0;

    /// <summary>
    /// Item Id.
    /// </summary>
    public uint ID => _item.RowId;

    /// <summary>
    /// Item Id
    /// </summary>
    public uint AdjustedID => ID;

    /// <summary>
    /// The check about this item.
    /// </summary>
    public Func<bool> ItemCheck { get; set; }
    private unsafe bool HasIt => InventoryManager.Instance()->GetInventoryItemCount(ID, false) > 0 ||
            InventoryManager.Instance()->GetInventoryItemCount(ID, true) > 0;

    /// <summary>
    /// Icon Id.
    /// </summary>
    public uint IconID { get; }

    /// <summary>
    /// Item name.
    /// </summary>
    public string Name => _item.Name;

    /// <summary>
    /// Is item enabled.
    /// </summary>
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

    /// <summary>
    /// Is the item in the cd window.
    /// </summary>
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

    /// <summary>
    /// Description about this item.
    /// </summary>
    public string Description => string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe float RecastTimeOneChargeRaw => ActionManager.Instance()->GetRecastTime(ActionType.Item, ID);

    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe float RecastTimeElapsedRaw => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Item, ID);

    /// <summary>
    /// Get the enough level for using this item.
    /// </summary>
    public bool EnoughLevel => true;

    /// <summary>
    /// The level to use this item.
    /// </summary>
    public byte Level => 0;

    /// <summary>
    /// Is Item cooling down.
    /// </summary>
    public unsafe bool IsCoolingDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Item, ID);

    /// <summary>
    /// Sort the item key.
    /// </summary>
    public uint SortKey { get; }

    /// <summary>
    /// Items' animation lock time.
    /// </summary>
    public float AnimationLockTime => 1.1f;

    /// <summary>
    /// Is this action in action sequencer.
    /// </summary>
    public virtual bool IsActionSequencer => false;

    /// <summary>
    /// Can I use this item.
    /// </summary>
    protected virtual bool CanUseThis => true;

    /// <summary>
    /// Create by row.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="a4"></param>
    public unsafe BaseItem(uint row, uint a4 = 65535)
        :this (Service.GetSheet<Item>().GetRow(row), a4)
    {
    }

    /// <summary>
    /// Create by item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="a4"></param>
    public unsafe BaseItem(Item item, uint a4 = 65535)
    {
        _item = item;
        IconID = _item.Icon;
        A4 = a4;
        SortKey = (uint)ActionManager.Instance()->GetRecastGroup((int)ActionType.Item, ID);
    }

    /// <summary>
    /// Can Use this item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="clippingCheck"></param>
    /// <returns></returns>
    public virtual unsafe bool CanUse(out IAction item, bool clippingCheck)
    {
        item = this;
        if (_item == null) return false;
        if (!CanUseThis) return false;
        if (DataCenter.DisabledAction != null && DataCenter.DisabledAction.Contains(ID)) return false;
        if(!IsEnabled) return false;

        if (ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Item, ID))
            && ConfigurationHelper.BadStatus.Contains(ActionManager.Instance()->GetActionStatus(ActionType.Item, ID + 1000000))) return false;

        var remain = RecastTimeOneChargeRaw - RecastTimeElapsedRaw;

        if(DataCenter.WeaponRemain > 0)
        {
            if (DataCenter.NextAbilityToNextGCD > AnimationLockTime + DataCenter.Ping) return false;

            if (clippingCheck && remain > DataCenter.ActionRemain) return false;
        }

        if (ItemCheck != null && !ItemCheck()) return false;

        return HasIt;
    }

    /// <summary>
    /// Use this item.
    /// </summary>
    /// <returns></returns>
    public unsafe bool Use()
    {
        if (_item == null) return false;

        if (InventoryManager.Instance()->GetInventoryItemCount(ID, true) > 0)
        {
            return ActionManager.Instance()->UseAction(ActionType.Item, ID + 1000000, Player.Object.ObjectId, A4);
        }

        return ActionManager.Instance()->UseAction(ActionType.Item, ID, Player.Object.ObjectId, A4);
    }

    /// <summary>
    /// To String.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;
}
