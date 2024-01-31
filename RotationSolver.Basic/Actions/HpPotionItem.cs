using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Actions;

internal class HpPotionItem : BaseItem
{
    readonly float _percent;
    readonly uint _maxHp;

    public uint MaxHp
    {
        get
        {
            if (!Player.Available) return 0;
            return Math.Min((uint)(Player.Object.MaxHp * _percent), _maxHp);
        }
    }

    protected override bool CanUseThis => Service.Config.UseHpPotions;

    public HpPotionItem(Item item) : base(item)
    {
        var data = _item.ItemAction.Value!.DataHQ;
        _percent = data[0] / 100f;
        _maxHp = data[1];
    }

    public override bool CanUse(out IAction item, bool clippingCheck)
    {
        item = this;
        if (!Player.Available) return false;
        if (Player.Object.GetHealthRatio() > Service.Config.HealthSingleAbilityHot) return false;
        if (Player.Object.MaxHp - Player.Object.CurrentHp < MaxHp) return false;
        return base.CanUse(out item, clippingCheck);
    }
}
