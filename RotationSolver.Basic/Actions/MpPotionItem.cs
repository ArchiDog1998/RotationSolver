using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Actions;
internal class MpPotionItem : BaseItem
{
    public uint MaxMp { get; }

    protected override bool CanUseThis => Service.Config.UseMpPotions;

    public MpPotionItem(Item item) : base(item)
    {
        var data = _item.ItemAction.Value!.DataHQ;
        MaxMp = data[1];
    }

    public override bool CanUse(out IAction item, bool clippingCheck)
    {
        item = this;
        if (!Player.Available) return false;
        if (Player.Object.MaxMp - DataCenter.CurrentMp < MaxMp) return false;
        return base.CanUse(out item, clippingCheck);
    }
}
