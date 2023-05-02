namespace RotationSolver.Basic.Actions;

internal class HealPotionItem : BaseItem
{
    readonly float _percent;
    readonly uint _maxHp;

    public uint MaxHealHp
    {
        get
        {
            if (Service.Player == null) return 0;
            return Math.Min((uint)(Service.Player.MaxHp * _percent), _maxHp);
        }
    }

    protected override bool CanUseThis => Service.Config.UseHealPotions;

    public HealPotionItem(uint row, float percent, uint maxHp, uint a4 = 65535) : base(row, a4)
    {
        _percent = percent;
        _maxHp = maxHp;
    }

    public override bool CanUse(out IAction item)
    {
        item = null;
        if (Service.Player == null) return false;
        var job = (ClassJobID)Service.Player.ClassJob.Id;
        if (Service.Player.GetHealthRatio() > job.GetHealSingleAbility() - 
            job.GetHealingOfTimeSubtractSingle()) return false;
        if (Service.Player.MaxHp - Service.Player.CurrentHp < MaxHealHp) return false;
        return base.CanUse(out item);
    }
}
